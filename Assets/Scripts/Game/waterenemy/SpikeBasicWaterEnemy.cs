using UnityEngine;
using System.Collections;

public class SpikeBasicWaterEnemy : BasicWaterEnemy {
	public static SpikeBasicWaterEnemy cons(GameEngineScene g, PatternEntry1Pt entry, Vector2 offset) {
		return (ObjectPool.inst().generic_depool<SpikeBasicWaterEnemy>()).i_cons(g, entry, offset);
	}
	public static SpikeBasicWaterEnemy cons(GameEngineScene g, PatternEntry2Pt entry, Vector2 offset) {
		return (ObjectPool.inst().generic_depool<SpikeBasicWaterEnemy>()).i_cons(g, entry, offset);
	}
	
	public override void do_remove() {
		ObjectPool.inst().generic_repool<SpikeBasicWaterEnemy>(this);
	}
	
	public override BasicWaterEnemy behaviour_set_rotation(float deg) { return this; }
	
	private SPSprite _img;
	public override void depool() {
		base.depool();
		this.get_root().set_name("SpikeBasicWaterEnemy");
		_img = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_SPIKE,FileCache.inst().get_texrect(RTex.ENEMY_SPIKE,"spike_test.png"));
		_img.set_manual_sort_z_order(GameAnchorZ.Enemy_InAir);
		this.get_root().add_child(_img);
	}
	
	public override void repool() {
		_img.repool();
		_img = null;
		base.repool();
	}
	
	private SpikeBasicWaterEnemy i_cons(GameEngineScene g, PatternEntry1Pt entry, Vector2 offset) {
		this.shared_i_cons_pre(g,entry._start,offset);
		
		this.add_component_for_mode(Mode.Moving,TwoPointSwimBasicWaterEnemyComponent.cons(
			SPUtil.vec_add(entry._start,offset),
			SPUtil.vec_add(entry._start,offset),
			SPUtil.vec_add(entry._start,offset),
			1.5f
		));
		
		
		this.shared_i_cons_post(g);
		return this;
	}
	
	private SpikeBasicWaterEnemy i_cons(GameEngineScene g, PatternEntry2Pt entry, Vector2 offset) {
		this.shared_i_cons_pre(g,entry._start,offset);
		
		this.add_component_for_mode(Mode.Moving,TwoPointSwimBasicWaterEnemyComponent.cons(
			SPUtil.vec_add(entry._start,offset),
			SPUtil.vec_add(entry._pt1,offset),
			SPUtil.vec_add(entry._pt2,offset),
			1.5f
		));
		
		this.shared_i_cons_post(g);
		return this;
	}
	
	private void shared_i_cons_pre(GameEngineScene g, Vector2 entry_start, Vector2 offset) {
		base.i_cons();
		_params._pos = SPUtil.vec_add(entry_start,offset);
	}
	
	private void shared_i_cons_post(GameEngineScene g) {
		this.add_hiteffect(SpikeReturnToPositionHitEffect.cons());
		this.add_component_for_mode(Mode.Stunned,KnockbackStunBasicWaterEnemyComponent.cons());
		this.transition_to_mode(g, Mode.Moving);
	}
	
	private Mode _last_mode;
	public override void i_update(GameEngineScene g, DiveGameState state) {
		base.i_update(g,state);
		
		if (this.get_current_mode() == Mode.Stunned && _last_mode == Mode.Moving) {
			_img.set_scale(1.5f);
		} else {
			_img.set_scale(SPUtil.drpt(_img.scale_x(),1,1/10.0f));
		}
		_last_mode = this.get_current_mode();
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

public class SpikeReturnToPositionHitEffect : BasicWaterEnemyHitEffect {
	public static SpikeReturnToPositionHitEffect cons() { return new SpikeReturnToPositionHitEffect(); }
	public override void apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy, BasicWaterEnemyComponent current_component) {
		BasicWaterEnemyComponentUtility.HitParams hit_params = BasicWaterEnemyComponentUtility.HitParams.cons_default();
		hit_params._ignore_dash = true;
		hit_params._knockback_vel_pt1 = new Vector2(0,1.5f);
		hit_params._knockback_vel_pt2 = new Vector2(10,7.5f);
		hit_params._knockback_vel_min = 1.5f;
		BasicWaterEnemyComponentUtility.small_enemy_apply_hit(g,state,enemy,hit_params);
	}
}
