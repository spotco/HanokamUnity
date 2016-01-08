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
	
	private SpikeEnemySprite _img;
	public override void depool() {
		base.depool();
		this.get_root().set_name("SpikeBasicWaterEnemy");
		_img = SpikeEnemySprite.cons();
		_img.set_manual_sort_z_order(GameAnchorZ.Enemy_InAir);
		_img.play_anim(SpikeEnemySprite.ANIM_IDLE);
		
		_img.add_to_parent(this.get_root());
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
			1.5f //default "move back to location" speed
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
			entry._speed
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
			_img.play_anim(SpikeEnemySprite.ANIM_HIT);
		} else {
			if (_img._animator.is_finished()) {
				_img.play_anim(SpikeEnemySprite.ANIM_IDLE);
			}
			_img.set_scale(SPUtil.drpt(_img.scale_x(),1,1/10.0f));
			
		}
		_last_mode = this.get_current_mode();
		
		bool enabled = SPHitRect.hitrect_touch(g.get_viewbox(),this.get_hit_rect());
		_img.set_enabled(enabled);
		if (enabled) {
			_img.i_update(g);	
		}
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
	
	public override void apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		BasicWaterEnemyComponentUtility.HitParams hit_params = BasicWaterEnemyComponentUtility.HitParams.cons_default();
		hit_params._enemy_mass = 2.0f;
		hit_params._ignore_dash = true;
		hit_params._player_vel = state._params._vel;
		hit_params._enemy_vel = enemy.get_calculated_velocity();
		hit_params._enemy_to_player_elasticity_coef = 0.4f;
		
		BasicWaterEnemyComponentUtility.small_enemy_apply_hit(g,state,enemy,hit_params);
	}
}
