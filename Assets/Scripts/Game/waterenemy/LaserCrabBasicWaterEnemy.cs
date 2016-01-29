using UnityEngine;
using System.Collections.Generic;

public class LaserCrabMovementBasicWaterEnemyComponent : BasicWaterEnemyComponent, BasicWaterEnemyHitEffect {
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
	}
	public interface ParamHolder {
		LaserCrabMovementBasicWaterEnemyComponent.Params get_params();
	}
	
	private void face_to(BasicWaterEnemy enemy, Vector2 dir) { enemy.get_root().set_rotation(SPUtil.dir_ang_deg(_starting_dir.x,_starting_dir.y)-90); }
	
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
			this.face_to(enemy,SPUtil.vec_scale(intersection_dir,-1));
			movement_params._mode = Mode.OnGround;
			
		} else {
			movement_params._mode = Mode.InWater;
			enemy._params._pos = _starting_pos;
			this.face_to(enemy,_starting_dir);
		}
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		Params movement_params = ((LaserCrabMovementBasicWaterEnemyComponent.ParamHolder)enemy).get_params();
		
		Debug.Log(movement_params._mode);
		
		switch (movement_params._mode) {
		case Mode.OnGround: {
			if (SPHitPoly.polyowners_intersect(enemy,g._player)) {
				enemy.get_hit_effect().apply_hit(g,state,enemy);
			}
		
		} break;
		case Mode.InWater: {
			enemy._params._pos = SPUtil.vec_add(enemy._params._pos, SPUtil.vec_scale(enemy._params._stun_vel,SPUtil.dt_scale_get()));
			
		} break;
		}
	}
	
	public void apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		Params movement_params = ((LaserCrabMovementBasicWaterEnemyComponent.ParamHolder)enemy).get_params();
		
		switch (movement_params._mode) {
		case Mode.OnGround: {
			BasicWaterEnemyComponentUtility.HitParams hit_params = BasicWaterEnemyComponentUtility.HitParams.cons_default();
			hit_params._player_vel = state._params._vel;
			hit_params._enemy_vel = enemy.get_calculated_velocity();
			hit_params._enemy_to_player_elasticity_coef = 0.7f;
			hit_params._transition_mode = false;
			BasicWaterEnemyComponentUtility.small_enemy_apply_hit(g,state,enemy,hit_params);
			
			movement_params._mode = Mode.InWater;
			
		} break;
		case Mode.InWater: {
			
		} break;
		}
		
		Debug.Log(movement_params._mode);
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
	
	private LaserCrabBasicWaterEnemy i_cons(GameEngineScene g, PatternEntryDirectional entry) {
		base.i_cons();
		this._current_mode = Mode.Activated;
		_movement_params = new LaserCrabMovementBasicWaterEnemyComponent.Params();
		
		LaserCrabMovementBasicWaterEnemyComponent movement_component = LaserCrabMovementBasicWaterEnemyComponent.cons(entry._start,entry._dir);
		this.add_component_for_mode(Mode.Activated, movement_component);
		this.add_hiteffect(movement_component);
		
		return this;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		base.i_update(g,state);
	}
	
	public override SPHitRect get_hit_rect() { return _img.get_hit_rect(this.get_root().get_u_pos(),this.get_root().rotation()); }
	public override SPHitPoly get_hit_poly() { return _img.get_hit_poly(this.get_root().get_u_pos(),this.get_root().rotation()); }
}
