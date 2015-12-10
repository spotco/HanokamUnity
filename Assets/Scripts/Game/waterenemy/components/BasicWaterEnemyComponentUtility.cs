using UnityEngine;
using System.Collections.Generic;

public class BasicWaterEnemyComponentUtility {

	public static bool enemy_test_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		if (enemy._params._invuln_ct > 0) {
			return false;
		}
		return SPHitPoly.polyowners_intersect(enemy, g._player);
	}
	
	public struct HitParams {
	
		public Vector2 _knockback_vel_pt1, _knockback_vel_pt2;
		public float _knockback_vel_min;
		
		public float _enemy_invuln_ct, _enemy_stun_ct;
		
		public bool _ignore_dash;
		
		public static HitParams cons_default() {
			return new HitParams() {
				_knockback_vel_pt1 = new Vector2(0,5),
				_knockback_vel_pt2 = new Vector2(10,25),
				_knockback_vel_min = 5,
				
				_enemy_stun_ct = 50,
				_enemy_invuln_ct = 15,
				
				_ignore_dash = false
			};
		}
		
	}
	
	public static void small_enemy_apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		BasicWaterEnemyComponentUtility.small_enemy_apply_hit(g,state,enemy,HitParams.cons_default());
	}

	public static void small_enemy_apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy, HitParams hit_params) {
		Vector2 enemy_hit_dir;
		Vector2 pos_delta = (new Vector2(enemy.get_u_pos().x-g._player.get_center().x, enemy.get_u_pos().y-g._player.get_center().y));
		float enemy_knockback_vel;
		if (!SPUtil.flt_cmp_delta(state._params._vel.magnitude,0,3.0f)) {
			enemy_hit_dir = SPUtil.vec_add(
				SPUtil.vec_scale(state._params._vel.normalized,0.75f),
				SPUtil.vec_scale(pos_delta.normalized,0.25f)
			).normalized;
			enemy_knockback_vel = SPUtil.y_for_point_of_2pt_line(
				hit_params._knockback_vel_pt1,
				hit_params._knockback_vel_pt2,
				Mathf.Clamp(state._params._vel.magnitude,0,10)
			);
		} else {
			enemy_hit_dir = pos_delta.normalized;
			if (SPUtil.flt_cmp_delta(enemy_hit_dir.magnitude,0,0.01f)) {
				enemy_hit_dir = new Vector2(0,1);
			}
			enemy_knockback_vel = hit_params._knockback_vel_min;
		}
		
		if (!state._params._dashing) {
			enemy_knockback_vel *= 0.5f;
		}
		
		enemy._params._stun_vel = SPUtil.vec_scale(enemy_hit_dir,enemy_knockback_vel);
		enemy._params._stun_ct = enemy._params._stun_ct_max = hit_params._enemy_stun_ct;
		enemy._params._invuln_ct = hit_params._enemy_invuln_ct;
		
		if (enemy.get_current_mode() != BasicWaterEnemy.Mode.Stunned) {
			g._camerac.freeze_frame(2);
			g._camerac.camera_shake(new Vector2(-1.5f,1.7f),15,30);
			enemy.transition_to_mode(g, BasicWaterEnemy.Mode.Stunned);
			
			MiscEffects.do_water_hit(g,enemy.get_u_pos());
		}
		
		if (state._params._dashing && !hit_params._ignore_dash) {
			if (!state._params._dash_has_hit) {
				state._params._vel = SPUtil.vec_scale(state._params._vel.normalized,-state._params.DASH_SPEED()*0.5f);
				state._params._dash_has_hit = true;
			}
			state._params._dash_ct = Mathf.Max(state._params._dash_ct, 25);
			
		} else if (!state._params.is_invuln()) {
			g._game_ui.do_red_flash();
			state._params._current_breath -= 200;
			state._params._invuln_ct = 25;
			state._params._vel = SPUtil.vec_scale(state._params._vel.normalized,-state._params.MAX_MOVE_SPEED()*0.5f);
			
		}
			
	}
}