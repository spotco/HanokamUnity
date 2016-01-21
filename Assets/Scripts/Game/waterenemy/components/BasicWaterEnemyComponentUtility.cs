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
		public float _enemy_mass;
		public Vector2 _enemy_vel;
		public Vector2 _player_vel;
		public float _enemy_invuln_ct, _enemy_stun_ct;
		public bool _ignore_dash;
		
		public float _player_to_enemy_elasticity_coef;
		public float _enemy_to_player_elasticity_coef;
		
		public static HitParams cons_default() {
			return new HitParams() {
				_enemy_mass = 1,
				_enemy_vel = Vector2.zero,
				_player_vel = Vector2.zero,
				_enemy_stun_ct = 50,
				_enemy_invuln_ct = 15,
				_ignore_dash = false,
				_player_to_enemy_elasticity_coef = 1,
				_enemy_to_player_elasticity_coef = 1
			};
		}
		
	}
	
	public static void small_enemy_apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		BasicWaterEnemyComponentUtility.small_enemy_apply_hit(g,state,enemy,HitParams.cons_default());
	}
	
	public static void small_enemy_apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy, HitParams hit_params) {
		SPPhysics.ElasticBodyCollisionValues collision_vals = SPPhysics.elastic_body_collision(new SPPhysics.ElasticBodyCollisionParameters() {
			_body_a_pos = g._player.get_center(),
			_body_a_mass = 1,
			_body_a_vel = hit_params._player_vel,
			
			_body_b_pos = enemy.get_u_pos(),
			_body_b_mass = hit_params._enemy_mass,
			_body_b_vel = hit_params._enemy_vel,
			
			_a_to_b_elasticity_coef = hit_params._player_to_enemy_elasticity_coef,
			_b_to_a_elasticity_coef = hit_params._enemy_to_player_elasticity_coef
		});
		float player_knockback_speed = collision_vals._body_a_speed;
		player_knockback_speed = Mathf.Max(player_knockback_speed,1.5f);
		float enemy_knockback_speed = collision_vals._body_b_speed;
		
		Vector2 player_knockback_dir = collision_vals._body_a_dir;
		Vector2 enemy_knockback_dir = collision_vals._body_b_dir;
		
		enemy._params._stun_vel = SPUtil.vec_scale(enemy_knockback_dir,enemy_knockback_speed);
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
				state._params._dash_has_hit = true;
			}
			state._params._dash_ct = Mathf.Max(state._params._dash_ct, state._params.DASH_CT_MAX());
			
		} else if (!state._params.is_invuln()) {
			g._game_ui.do_red_flash();
			state._params._current_breath -= 200;
			state._params._invuln_ct = 25;
		}
		state._params._vel = SPUtil.vec_scale(player_knockback_dir,player_knockback_speed);			
	}
	
}