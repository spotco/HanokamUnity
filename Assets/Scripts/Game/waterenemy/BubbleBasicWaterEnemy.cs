using UnityEngine;
using System.Collections;

public class BubbleBasicWaterEnemy : BasicWaterEnemy {
	public static BubbleBasicWaterEnemy cons(GameEngineScene g, PatternEntry1Pt entry) {
		return (ObjectPool.inst().generic_depool<BubbleBasicWaterEnemy>()).i_cons(g, entry);
	}
	public static BubbleBasicWaterEnemy cons(GameEngineScene g, PatternEntry2Pt entry) {
		return (ObjectPool.inst().generic_depool<BubbleBasicWaterEnemy>()).i_cons(g, entry);
	}
	
	public override void do_remove() {
		ObjectPool.inst().generic_repool<BubbleBasicWaterEnemy>(this);
	}
	
	public override BasicWaterEnemy behaviour_set_rotation(float deg) { return this; }
	
	public BubbleSprite _img;
	public override void depool() {
		base.depool();
		this.get_root().set_name("BubbleBasicWaterEnemy");
		_img = BubbleSprite.cons();
		_img.add_to_parent(this.get_root());
		_img.set_manual_sort_z_order(GameAnchorZ.BGWater_FX);
	}
	
	public override void repool() {
		_img.repool();
		base.repool();
	}
	
	private BubbleBasicWaterEnemy i_cons(GameEngineScene g, PatternEntry1Pt entry) {
		this.shared_i_cons_pre(g,entry._start);
		
		this.add_component_for_mode(Mode.Moving, OnePointBasicWaterEnemyComponent.cons(entry._start));		
		
		this.shared_i_cons_post(g);
		return this;
	}
	
	private BubbleBasicWaterEnemy i_cons(GameEngineScene g, PatternEntry2Pt entry) {
		this.shared_i_cons_pre(g,entry._start);
		
		this.add_component_for_mode(Mode.Moving,TwoPointSwimBasicWaterEnemyComponent.cons(
			entry._start,
			entry._pt1,
			entry._pt2,
			3.0f
		));
		
		this.shared_i_cons_post(g);
		return this;
	}
	
	private void shared_i_cons_pre(GameEngineScene g, Vector2 entry_start) {
		base.i_cons();
		_params._pos = entry_start;
	}
	
	private void shared_i_cons_post(GameEngineScene g) {
		this.add_component_for_mode(Mode.Activated, BubblePoppedBasicWaterEnemyComponent.cons());
		this.add_hiteffect(BubbleBasicWaterEnemyHitEffect.cons());
		this.transition_to_mode(g, Mode.Moving);
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		base.i_update(g,state);
		if (this.get_current_mode() == Mode.Moving) {
			_img.play_anim(BubbleSprite.ANIM_IDLE);
		}
		_img.i_update(g);
		_img.set_enabled(SPHitRect.hitrect_touch(g.get_viewbox(),this.get_hit_rect()));
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

public class BubbleBasicWaterEnemyHitEffect : BasicWaterEnemyHitEffect {
	public static BubbleBasicWaterEnemyHitEffect cons() { return new BubbleBasicWaterEnemyHitEffect(); }
	public void apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		state._params._current_breath = Mathf.Min(state._params._current_breath+500,state._params.MAX_BREATH());
		g._camerac.freeze_frame(1);
		enemy.transition_to_mode(g,BasicWaterEnemy.Mode.Activated);
	}
}

public class BubblePoppedBasicWaterEnemyComponent : BasicWaterEnemyComponent {
	public static BubblePoppedBasicWaterEnemyComponent cons() { return (new BubblePoppedBasicWaterEnemyComponent()); }
	public override void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		BubbleBasicWaterEnemy bubble_enemy;
		if (!SPUtil.cond_cast<BubbleBasicWaterEnemy>(enemy, out bubble_enemy)) {
			return;
		}
		bubble_enemy._img.play_anim(BubbleSprite.ANIM_POP);
		if (bubble_enemy._img._animator.is_finished()) {
			enemy.transition_to_mode(g,BasicWaterEnemy.Mode.DoRemove);
		}
	}
}
