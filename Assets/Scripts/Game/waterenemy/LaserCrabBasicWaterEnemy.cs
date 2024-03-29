﻿using UnityEngine;
using System.Collections.Generic;

public class LaserCrabMovementBasicWaterEnemyComponent : BasicWaterEnemyComponent, BasicWaterEnemyHitEffect, SPPhysics.SolidCollisionUpdateDelegate {
	public static LaserCrabMovementBasicWaterEnemyComponent cons(Vector2 pos, Vector2 dir) {
		return (new LaserCrabMovementBasicWaterEnemyComponent()).i_cons(pos,dir);
	}
	
	private Vector2 _starting_pos, _starting_dir;
	
	private LaserCrabMovementBasicWaterEnemyComponent i_cons(Vector2 starting_pos, Vector2 starting_dir) {
		_starting_pos = starting_pos;
		_starting_dir = starting_dir;
		return this;
	}
	
	public enum Mode {
		OnGround,
		InWater
	}
	public class Params {
		public Mode _mode;
		public Vector2 _tar_facing_direction;
	}
	public interface ParamHolder {
		LaserCrabMovementBasicWaterEnemyComponent.Params get_params();
	}
	
	private void face_to(BasicWaterEnemy enemy, Vector2 dir, bool imm = false, float drpty_spd = 1/10.0f) { 
		float tar_ang = SPUtil.dir_ang_deg(dir.x,dir.y)-90;
		if (imm) {
			enemy.get_root().set_rotation(tar_ang); 
		} else {
			enemy.get_root().set_rotation(enemy.get_root().rotation()+SPUtil.drpty(drpty_spd)*SPUtil.shortest_angle(enemy.get_root().rotation(),tar_ang));
		}
	}
	
	public override void load_postprocess(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		Params movement_params = ((LaserCrabMovementBasicWaterEnemyComponent.ParamHolder)enemy).get_params();
					
		bool obstacle_attach_found = false;
		SPLineSegment obstacle_attach = new SPLineSegment();
		Vector2 obstacle_attach_point = new Vector2();
		
		SPLineSegment imm_attach_range = new SPLineSegment() {
			_pt0 = _starting_pos,
			_pt1 = SPUtil.vec_add(_starting_pos, SPUtil.vec_scale(_starting_dir.normalized, -200))
		};
		for (int i_obstacle = 0; i_obstacle < state._enemy_manager._obstacles.Count; i_obstacle++) {
			IWaterObstacle itr_obstacle = state._enemy_manager._obstacles[i_obstacle];
			if (SPHitRect.hitrect_touch(imm_attach_range.bounding_rect(),itr_obstacle.get_hit_rect())) {
				SPHitPoly itr_obstacle_poly = itr_obstacle.get_hit_poly();
				
				for (int i_ptseg = 0; i_ptseg < itr_obstacle_poly.length; i_ptseg++) {
					SPLineSegment itr_seg = itr_obstacle_poly.line_segment_of_point(i_ptseg);
					Vector2 intersect = SPLineSegment.line_seg_intersection(imm_attach_range,itr_seg);
					if (SPLineSegment.is_intersect_point(intersect)) {
						obstacle_attach_found = true;
						obstacle_attach = itr_seg;
						obstacle_attach_point = intersect;
					}
				}
			}
		}
		
		if (obstacle_attach_found) {
			enemy._params._pos = SPUtil.vec_add(obstacle_attach_point, SPUtil.vec_scale(_starting_dir.normalized,50));
			Vector2 intersection_dir = SPUtil.point_line_intersection_dir(_starting_pos,obstacle_attach._pt0,SPUtil.vec_sub(obstacle_attach._pt1,obstacle_attach._pt0));
			movement_params._tar_facing_direction = intersection_dir;
			this.face_to(enemy,movement_params._tar_facing_direction,true);
			movement_params._mode = Mode.OnGround;
			
		} else {
			movement_params._mode = Mode.InWater;
			enemy._params._pos = _starting_pos;
			movement_params._tar_facing_direction = _starting_dir;
			this.face_to(enemy,movement_params._tar_facing_direction,true);
		}
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		Params movement_params = ((LaserCrabMovementBasicWaterEnemyComponent.ParamHolder)enemy).get_params();
		
		switch (movement_params._mode) {
		case Mode.OnGround: {
			if (enemy._params._invuln_ct <= 0 && !state._params.is_invuln() && SPHitPoly.polyowners_intersect(enemy,g._player)) {
				enemy.get_hit_effect().apply_hit(g,state,enemy);
			}
		
		} break;
		case Mode.InWater: {
			Vector2 pos_prev = enemy._params._pos;
			enemy._params._pos = SPUtil.vec_add(enemy._params._pos, SPUtil.vec_scale(enemy._params._stun_vel,SPUtil.dt_scale_get()));
			enemy.apply_offset_to_position();
			
			__test_move_delta_pos_pre = enemy._params._pos;
			
			List<SPHitPolyOwner> tmp = SPPhysics.tmp_list();
			for (int i = 0; i < state._enemy_manager._obstacles.Count; i++) {
				tmp.Add(state._enemy_manager._obstacles[i]);
			}
			SPPhysics.SolidCollisionValues collision_vals = SPPhysics.solid_collision_update_frame(
				enemy,
				tmp,
				this,
				Mathf.Floor(enemy._params._stun_vel.magnitude),
				SPUtil.dir_ang_deg(-enemy._params._stun_vel.x,-enemy._params._stun_vel.y),
				(System.Object)enemy,
				SPPhysics.get_world_bounds_extra_obstacles(enemy.get_hit_rect())
			);
			tmp.Clear();
			
			if (enemy._params._invuln_ct > 0) {
				enemy._params._invuln_ct -= SPUtil.dt_scale_get();
			} else if (enemy._params._stun_ct > 0) {
				enemy._params._stun_ct -= SPUtil.dt_scale_get();
			}
			
			if (enemy._params._invuln_ct <= 0 && !state._params.is_invuln() && SPHitPoly.polyowners_intersect(enemy,g._player)) {
				enemy.get_hit_effect().apply_hit(g,state,enemy);
			} 
			
			if (collision_vals._do_not_move || collision_vals._is_collide_this_frame) {
				Vector2 tangent_basis = collision_vals._collision_tangent;
				Vector2 normal_basis = collision_vals._collision_normal;
				enemy._params._stun_vel = SPUtil.vec_add(
					SPUtil.vec_scale(normal_basis,SPUtil.vec_dot(enemy._params._stun_vel,normal_basis) * -0.9f),
					SPUtil.vec_scale(tangent_basis,SPUtil.vec_dot(enemy._params._stun_vel,tangent_basis))
				);
				if ((enemy._params._invuln_ct <= 0 && enemy._params._stun_ct <= 0) && normal_basis.normalized.y > 0.2f) {
					movement_params._mode = Mode.OnGround;
					movement_params._tar_facing_direction = normal_basis;
				}
			} else {
				if (enemy._params._stun_vel.magnitude > 0) {
					movement_params._tar_facing_direction = enemy._params._stun_vel.normalized;
				}
			}
			enemy.apply_offset_to_position();
			enemy._params._stun_vel.y -= 0.3f * SPUtil.dt_scale_get();
			
		} break;
		}
		this.face_to(enemy,movement_params._tar_facing_direction,false, movement_params._mode == Mode.OnGround ? 1/10.0f : 1/35.0f);
	}
	
	private Vector2 __test_move_delta_pos_pre;
	public void test_move_delta(Vector2 pos_delta, List<SPHitPolyOwner> obstacles, System.Object parameters) {
		BasicWaterEnemy enemy = (BasicWaterEnemy)parameters;
		enemy._params._pos = SPUtil.vec_add(__test_move_delta_pos_pre,pos_delta);
		enemy.apply_offset_to_position();
	}
	
	public void apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		Params movement_params = ((LaserCrabMovementBasicWaterEnemyComponent.ParamHolder)enemy).get_params();
		
		BasicWaterEnemyComponentUtility.HitParams hit_params = BasicWaterEnemyComponentUtility.HitParams.cons_default();
		hit_params._player_vel = state._params._vel;
		hit_params._enemy_vel = enemy.get_calculated_velocity();
		hit_params._enemy_to_player_elasticity_coef = 0.9f;
		hit_params._transition_mode = false;
		hit_params._enemy_invuln_ct = 30;
		hit_params._enemy_stun_ct = 50;
		BasicWaterEnemyComponentUtility.small_enemy_apply_hit(g,state,enemy,hit_params);
		
		movement_params._mode = Mode.InWater;
	}
}

public class LaserCrabBasicWaterEnemy :
	BasicWaterEnemy,
	LaserCrabMovementBasicWaterEnemyComponent.ParamHolder
	{

	public static LaserCrabBasicWaterEnemy cons(GameEngineScene g, PatternEntryDirectional entry) {
		return (ObjectPool.inst().generic_depool<LaserCrabBasicWaterEnemy>()).i_cons(g, entry);
	}
	
	public override void do_remove() {
		ObjectPool.inst().generic_repool<LaserCrabBasicWaterEnemy>(this);
	}
	
	private LaserCrabEnemySprite _img;
	public override void depool() {
		base.depool();
		this.get_root().set_name("LaserCrabBasicWaterEnemy");
		_img = LaserCrabEnemySprite.cons();
		_img.add_to_parent(this.get_root());
		_img.set_manual_sort_z_order(GameAnchorZ.Enemy_InAir);
	}
	
	public override void repool() {
		ObjectPool.inst().generic_repool<LaserCrabEnemySprite>(_img);
		_img = null;
		base.repool();
	}
	
	private LaserCrabMovementBasicWaterEnemyComponent.Params _movement_params;
	public LaserCrabMovementBasicWaterEnemyComponent.Params get_params() { return _movement_params; }
	
	private FlashEvery _invuln_flash;
	private bool _invuln_flash_toggle;
	
	private LaserCrabBasicWaterEnemy i_cons(GameEngineScene g, PatternEntryDirectional entry) {
		base.i_cons();
		
		_invuln_flash = FlashEvery.cons(5);
		_invuln_flash_toggle = false;
		
		this._current_mode = Mode.Activated;
		_movement_params = new LaserCrabMovementBasicWaterEnemyComponent.Params();
		
		LaserCrabMovementBasicWaterEnemyComponent movement_component = LaserCrabMovementBasicWaterEnemyComponent.cons(entry._start,entry._dir);
		this.add_component_for_mode(Mode.Activated, movement_component);
		this.add_hiteffect(movement_component);
		
		return this;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		base.i_update(g,state);
		
		if (_movement_params._mode == LaserCrabMovementBasicWaterEnemyComponent.Mode.OnGround) {
			_img._img.set_opacity(1);
			_img.play_anim(LaserCrabEnemySprite.ANIM_UNDERWATER_IDLE);
			
		} else if (_movement_params._mode == LaserCrabMovementBasicWaterEnemyComponent.Mode.InWater) {
			if (_params._invuln_ct > 0 || _params._stun_ct > 0) {
				_invuln_flash.i_update();
				if (_invuln_flash.do_flash()) {
					_invuln_flash_toggle = !_invuln_flash_toggle;
				}
				_img._img.set_opacity(_invuln_flash_toggle ? 0.8f : 0.45f);
				
			} else {
				_img._img.set_opacity(1);
			}
		
			_img.play_anim(LaserCrabEnemySprite.ANIM_AIR_IDLE);
		}
		
		_img.i_update(g);
	}
	
	public override SPHitRect get_hit_rect() { return _img.get_hit_rect(this.get_root().get_u_pos(),this.get_root().rotation()); }
	public override SPHitPoly get_hit_poly() { return _img.get_hit_poly(this.get_root().get_u_pos(),this.get_root().rotation()); }
}
