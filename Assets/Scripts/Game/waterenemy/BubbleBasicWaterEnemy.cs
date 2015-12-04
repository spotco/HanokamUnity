using UnityEngine;
using System.Collections;

public class BubbleBasicWaterEnemy : BasicWaterEnemy {
	public static BubbleBasicWaterEnemy cons(GameEngineScene g, PatternEntry1Pt entry, Vector2 offset) {
		return (ObjectPool.inst().generic_depool<BubbleBasicWaterEnemy>()).i_cons(g, entry, offset);
	}
	
	public override void do_remove() {
		ObjectPool.inst().generic_repool<BubbleBasicWaterEnemy>(this);
	}
	
	public BubbleSprite _img;
	public override void depool() {
		base.depool();
		this.get_root().set_name("BubbleBasicWaterEnemy");
		_img = BubbleSprite.cons();
		_img.add_to_parent(this.get_root());
		_img.set_manual_sort_z_order(GameAnchorZ.Enemy_InAir);
	}
	
	public override void repool() {
		_img.repool();
		base.repool();
	}
	
	private BubbleBasicWaterEnemy i_cons(GameEngineScene g, PatternEntry1Pt entry, Vector2 offset) {
		base.i_cons();
		_params._pos = new Vector2(entry._start.x + offset.x, entry._start.y + offset.y);
		
		this.add_component_for_mode(Mode.Moving, BubbleUpwardsBasicWaterEnemyComponent.cons(_params._pos));
		this.transition_to_mode(g, Mode.Moving);
		
		return this;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		base.i_update(g,state);
		_img.i_update(g);
	}
	
	public override SPHitRect get_hit_rect() {
		return SPHitPoly.hitpoly_to_bounding_hitrect(
			this.get_hit_poly(),
			new Vector2(-10,-10),
			new Vector2(10,10)
		);
	}
	
	public override SPHitPoly get_hit_poly() {
		return SPHitPoly.cons_with_basis_offset(
			new Vector2(this.get_root()._u_x, this.get_root()._u_y),
			this.get_root().rotation(),
			new Vector2(110,110),
			new Vector2(1,1),
			1,
			new Vector2(0,0)
		);
	}
}

public class BubbleUpwardsBasicWaterEnemyComponent : BasicWaterEnemyComponent {
	public static BubbleUpwardsBasicWaterEnemyComponent cons(Vector2 start) {
		return (new BubbleUpwardsBasicWaterEnemyComponent()).i_cons(start);
	}
	private Vector2 _pos;
	private enum Mode {
		Active,
		Collected,
		DoRemove
	}
	private Mode _current_mode;
	
	private BubbleUpwardsBasicWaterEnemyComponent i_cons(Vector2 start) {
		_pos = start;
		_current_mode = Mode.Active;
		return this;
	}
	public override void notify_start_on_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public override void notify_transition_to_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public override void notify_transition_from_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	
	public override void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		BubbleBasicWaterEnemy bubble_enemy;
		if (!SPUtil.cond_cast<BubbleBasicWaterEnemy>(enemy, out bubble_enemy)) {
			return;
		}
		switch (_current_mode) {
		case Mode.Active: {
			bubble_enemy._img.play_anim(BubbleSprite.ANIM_IDLE);
			if (SPHitPoly.polyowners_intersect(bubble_enemy,g._player)) {
				_current_mode = Mode.Collected;
				
				g._camerac.freeze_frame(1);
				g._camerac.camera_shake(new Vector2(-1.5f,1.7f),10,15);
			}
		
		} break;
		case Mode.Collected: {
			bubble_enemy._img.play_anim(BubbleSprite.ANIM_POP);
			if (bubble_enemy._img._animator.is_finished()) {
				_current_mode = Mode.DoRemove;
			}
		
		} break;
		case Mode.DoRemove: {
			bubble_enemy.transition_to_mode(g, BasicWaterEnemy.Mode.DoRemove);
			
		} break;
		}
		
		bubble_enemy._params._pos = _pos;
	}
}
