﻿using UnityEngine;
using System.Collections;

public interface DiveReturnGameStateUpdateable {
	void i_update(GameEngineScene g, DiveReturnGameState state);
}

public class DiveReturnGameState : GameStateBase {

	public static DiveReturnGameState cons(GameEngineScene g, DiveGameState.DiveGameStateParams param, WaterEnemyManager enemy_manager) {
		return (new DiveReturnGameState()).i_cons(g,param,enemy_manager);
	}

	public enum State {
		CameraPanUp,
		CameraPanUpPostPause,
		BreakThrough,
		FollowUp
	}

	private State _current_state;
	private WaterEnemyManager _enemy_manager;
	private float _anim_t;

	public DiveReturnGameState i_cons(GameEngineScene g, DiveGameState.DiveGameStateParams param, WaterEnemyManager enemy_manager) {
		_current_state = State.CameraPanUp;
		_enemy_manager = enemy_manager;
		g._camerac.set_camera_follow_speed(1/15.0f);
		g._camerac.set_target_zoom(1000);
		g._camerac.set_target_camera_y(g._camerac.get_target_camera_y() + 900);
		return this;
	}

	public override void i_update(GameEngineScene g) {
		_enemy_manager.i_update(g, this);
		switch (_current_state) {
		case State.CameraPanUp: {
			if (SPUtil.flt_cmp_delta(g._camerac.get_current_camera_y(),g._camerac.get_target_camera_y(),10)) {
				_current_state = State.CameraPanUpPostPause;
				_anim_t = 0;
			}

		} break;
		case State.CameraPanUpPostPause: {
			_anim_t += 0.2f * SPUtil.dt_scale_get();
			if (_anim_t >= 1) {
				g._player.set_rotation(0);
				_current_state = State.BreakThrough;
				g._camerac.camera_shake(new Vector2(-2,3.3f),35,30);
			}

		} break;
		case State.BreakThrough: {
			g._player.play_anim(PlayerCharacterAnims.SWIM);
			g._player.i_update(g);
			PlayerCharacterUtil.move_in_bounds(
				g._player,
				g._player._u_x,
				g._player._u_y + 35 * SPUtil.dt_scale_get()
			);

			if (g.convert_u_pos_to_screen_pos(g._player._u_x,g._player._u_y).y > SPUtil.game_screen().y * 0.75f) {
				_current_state = State.FollowUp;
				_anim_t = 0;
			}

		} break;
		case State.FollowUp: {
			g._player.i_update(g);
			PlayerCharacterUtil.move_in_bounds(
				g._player,
				g._player._u_x,
				g._player._u_y + 35 * SPUtil.dt_scale_get()
			);
			_anim_t = Mathf.Clamp(_anim_t+0.05f*SPUtil.dt_scale_get(),0,1);
			g._camerac.set_camera_follow_speed(SPUtil.y_for_point_of_2pt_line(new Vector2(0,1/10.0f),new Vector2(1,1),_anim_t));
			g._camerac.set_target_camera_focus_on_character(g,0,200);

		} break;
		}
	}

	public override GameStateIdentifier get_state() { 
		return GameStateIdentifier.DiveReturn;
	}

}
