using UnityEngine;
using System.Collections.Generic;

public interface InAirGameStateUpdateable {
	void i_update(GameEngineScene g, InAirGameStateUpdateable state);
}

public class InAirGameState : GameStateBase {

	public static InAirGameState cons(GameEngineScene g) {
		return (new InAirGameState()).i_cons(g);
	}

	public struct Params {
		public float _upwards_vel;
		public float _target_y;
		public float _anim_t;

		public Vector2 _initial_c_pos;
		public Vector2 _player_c_pos;
	}

	public enum Mode {
		InitialJumpOut,
		Combat,
		RescueBackToTop,
		FallToGround
	}

	private Params _params;
	private Mode _current_mode;

	private InAirGameState i_cons(GameEngineScene g) {
		g._camerac.set_camera_follow_speed(1);
		g._camerac.set_zoom_speed(1/20.0f);
		g._camerac.set_target_zoom(GameCameraController.MAX_ZOOM);
		_params._target_y = g._player._u_y + 200;
		g._camerac.set_camera_follow_speed(1);
		g._camerac.set_target_camera_y(_params._target_y);
		_params._upwards_vel = 30;

		g._player.set_trail_enabled_and_rotation(false);
		g._player.set_streak_enabled(false);
		g._player.play_anim("In Air Idle");

		_current_mode = Mode.InitialJumpOut;
		_params._initial_c_pos = _params._player_c_pos = GameCameraController.u_pos_to_c_pos(g._player.get_center());
		 
		_params._anim_t = 0;

		return this;
	}

	public override void i_update(GameEngineScene g) {
		g._player.i_update(g);

		switch (_current_mode) {
		case Mode.InitialJumpOut: {
			g._player.play_anim("In Air Idle");
			_params._target_y += _params._upwards_vel * SPUtil.dt_scale_get();
			_params._upwards_vel = SPUtil.drpt(_params._upwards_vel,0,1/50.0f);
			g._camerac.set_target_camera_y(_params._target_y);

			_params._anim_t += 0.01f * SPUtil.dt_scale_get();
			_params._player_c_pos.y = SPUtil.y_for_point_of_2pt_line(
				new Vector2(0,_params._initial_c_pos.y),
				new Vector2(1,500),
				SPUtil.bezier_val_for_t(
					new Vector2(0,0),
					new Vector2(0,0.5f),
					new Vector2(0.5f,1),
					new Vector2(1,1),
					_params._anim_t
				).y
			);
			g._player.set_center_u_pos(GameCameraController.c_pos_to_u_pos(_params._player_c_pos));

			if (_params._anim_t >= 1) {
				_current_mode = Mode.Combat;
			}

		} break;
		case Mode.Combat: {
			_params._target_y += _params._upwards_vel * SPUtil.dt_scale_get();
			_params._upwards_vel = SPUtil.drpt(_params._upwards_vel,0,1/50.0f);
			g._camerac.set_target_camera_y(_params._target_y);

			g._player.set_center_u_pos(GameCameraController.c_pos_to_u_pos(_params._player_c_pos));

		} break;
		case Mode.RescueBackToTop: {

		} break;
		case Mode.FallToGround: {

		} break;
		}
	}

	public override GameStateIdentifier get_state() { 
		return GameStateIdentifier.InAir;
	}

}
