using UnityEngine;
using System.Collections;

public class SmallEnemyBasicWaterEnemyHitEffect : BasicWaterEnemyHitEffect {
	public static SmallEnemyBasicWaterEnemyHitEffect cons() { return new SmallEnemyBasicWaterEnemyHitEffect(); }
	public override void apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy, BasicWaterEnemyComponent current_component) {
		BasicWaterEnemyComponentUtility.small_enemy_apply_hit(g,state,enemy);
	}
}

public class PufferBasicWaterEnemy : BasicWaterEnemy {
	public static PufferBasicWaterEnemy cons(GameEngineScene g, PatternEntry2Pt entry, Vector2 offset) {
		return (ObjectPool.inst().generic_depool<PufferBasicWaterEnemy>()).i_cons(g, entry, offset);
	}
	
	public override void do_remove() {
		ObjectPool.inst().generic_repool<PufferBasicWaterEnemy>(this);
	}
	
	private PufferEnemySprite _img;
	public override void depool() {
		base.depool();
		this.get_root().set_name("PufferBasicWaterEnemy");
		_img = PufferEnemySprite.cons();
		_img.add_to_parent(this.get_root());
		_img.set_manual_sort_z_order(GameAnchorZ.Enemy_InAir);
	}
	
	public override void repool() {
		_img.repool();
		base.repool();
	}
	
	private FlashEvery _flashcount;
	private PufferBasicWaterEnemy i_cons(GameEngineScene g, PatternEntry2Pt entry, Vector2 offset) {
		base.i_cons();
		_flashcount = FlashEvery.cons(15);
		_params._pos = new Vector2(entry._start.x + offset.x, entry._start.y + offset.y);
		
		this.add_component_for_mode(Mode.Moving, TwoPointSwimBasicWaterEnemyComponent.cons(
			SPUtil.vec_add(entry._start,offset), 
			SPUtil.vec_add(entry._pt1,offset), 
			SPUtil.vec_add(entry._pt2,offset), 3.0f
		));
		this.add_component_for_mode(Mode.Stunned, KnockbackStunBasicWaterEnemyComponent.cons());
		this.add_hiteffect(SmallEnemyBasicWaterEnemyHitEffect.cons());
		
		return this;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		base.i_update(g,state);
		_img.play_anim(PufferEnemySprite.ANIM_IDLE);
		_img.i_update(g);
		
		Vector4 img_color = _img.color();
		img_color.y = SPUtil.drpt(img_color.y,1,1/8.0f);
		img_color.z = SPUtil.drpt(img_color.z,1,1/8.0f);
		switch(this.get_current_mode()) {
		case(Mode.Stunned):{
			_flashcount.i_update();
			if (_flashcount.do_flash()) 	{
				img_color.y = 0;
				img_color.z = 0;
			}
		} break;
		default: {} break;
		}
		_img.set_color(img_color);
		
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
			new Vector2(55,90),
			new Vector2(1,1),
			1,
			new Vector2(-10,0)
		);
	}
}
