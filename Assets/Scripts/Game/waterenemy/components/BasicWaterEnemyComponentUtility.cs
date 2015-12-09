using UnityEngine;
using System.Collections.Generic;

public class BasicWaterEnemyComponentUtility {

	public static bool enemy_test_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		if (enemy._params._invuln_ct > 0) {
			return false;
		}
		return SPHitPoly.polyowners_intersect(enemy, g._player);
	}

	public static void small_enemy_apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		Vector2 enemy_hit_dir;
		Vector2 pos_delta = (new Vector2(enemy.get_u_pos().x-g._player.get_center().x, enemy.get_u_pos().y-g._player.get_center().y));
		float enemy_knockback_vel;
		if (!SPUtil.flt_cmp_delta(state._params._vel.magnitude,0,0.1f)) {
			enemy_hit_dir = SPUtil.vec_add(
				SPUtil.vec_scale(state._params._vel.normalized,0.75f),
				SPUtil.vec_scale(pos_delta.normalized,0.25f)).normalized;
			enemy_knockback_vel = SPUtil.y_for_point_of_2pt_line(new Vector2(0,5),new Vector2(10,25),Mathf.Clamp(state._params._vel.magnitude,0,10));
		} else {
			enemy_hit_dir = pos_delta.normalized;
			if (SPUtil.flt_cmp_delta(enemy_hit_dir.magnitude,0,0.01f)) {
				enemy_hit_dir = new Vector2(0,1);
			}
			enemy_knockback_vel = 5;
		}
		
		if (!state._params._dashing) {
			enemy_knockback_vel *= 0.5f;
		}
		
		enemy._params._stun_vel = SPUtil.vec_scale(enemy_hit_dir,enemy_knockback_vel);
		enemy._params._stun_ct = enemy._params._stun_ct_max = 50;
		enemy._params._invuln_ct = 15;
		
		if (enemy.get_current_mode() != BasicWaterEnemy.Mode.Stunned) {
			g._camerac.freeze_frame(2);
			g._camerac.camera_shake(new Vector2(-1.5f,1.7f),15,30);
			enemy.transition_to_mode(g, BasicWaterEnemy.Mode.Stunned);
			
			g.add_particle(SPConfigAnimParticle.cons()
				.set_texture(TextureResource.inst().get_tex(RTex.HANOKA_EFFECTS_WATER))
				.set_texrect(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS_WATER,"water_hit_0.png"))
				.set_pos(enemy.get_u_pos().x, enemy.get_u_pos().y)
				.set_ctmax(40)
				.set_alpha(0.5f, 0.1f)
				.set_scale(0.9f, 1.3f)
				.set_manual_sort_z_order(GameAnchorZ.BGWater_FX)
				.set_normalized_timed_sprite_animator(
					SPTimedSpriteAnimator.cons(null)
					.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS_WATER,"water_hit_0.png"),0)
					.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS_WATER,"water_hit_1.png"),0.2f)
					.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS_WATER,"water_hit_2.png"),0.4f)
					.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS_WATER,"water_hit_3.png"),0.6f)
					.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS_WATER,"water_hit_4.png"),0.8f))
				.set_do_track_bg_water()
			);
			
		}
		
		if (state._params._dashing) {
			if (!state._params._dash_has_hit) {
				state._params._vel = SPUtil.vec_scale(state._params._vel.normalized,-state._params.DASH_SPEED()*0.5f);
				state._params._dash_has_hit = true;
			}
			state._params._dash_ct = Mathf.Max(state._params._dash_ct, 15);
			
		} else if (!state._params.is_invuln()) {
			g._game_ui.do_red_flash();
			state._params._current_breath -= 200;
			state._params._invuln_ct = 25;
			state._params._vel = SPUtil.vec_scale(state._params._vel.normalized,-state._params.MAX_MOVE_SPEED()*0.5f);
			
		}
			
	}
}