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
		
		public Vector2 _player_c_vel;
		
		public string _player_anim_hold;
		public float _player_anim_hold_ct;
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
		g._player.set_rotation(0);
		g._player.play_anim("In Air Idle");

		_current_mode = Mode.InitialJumpOut;
		_params._initial_c_pos = _params._player_c_pos = GameCameraController.u_pos_to_c_pos(g._player.get_center());
		_params._player_c_vel = new Vector2(0,0);
		_params._anim_t = 0;
		_params._player_anim_hold = PlayerCharacterAnims.INAIRIDLE;
		_params._player_anim_hold_ct = 0;

		return this;
	}

	float MAX_MOVE_SPEED = 25;
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
			
			this.move_x(g);
			_params._player_c_vel.y = 0;
			this.apply_c_vel(g);
			this.apply_c_pos_to_player(g);

			if (_params._anim_t >= 1) {
				_current_mode = Mode.Combat;
			}

		} break;
		case Mode.Combat: {
			_params._target_y += _params._upwards_vel * SPUtil.dt_scale_get();
			_params._upwards_vel = SPUtil.drpt(_params._upwards_vel,0,1/50.0f);
			g._camerac.set_target_camera_y(_params._target_y);
			
			
			if (g._controls.get_control_down(ControlManager.Control.ShootArrow)) {
				g._player.play_anim(PlayerCharacterAnims.BOWAIM, false);
				_params._player_c_vel.y = SPUtil.lmovto(_params._player_c_vel.y, -1f, 5.0f);
				
			} else {
				if (g._controls.get_control_just_released(ControlManager.Control.ShootArrow)) {
					// SPTODO -- fire arrow
					_params._player_anim_hold = PlayerCharacterAnims.BOWFIRE;
					_params._player_anim_hold_ct = 30.0f;
				}
				
				if (_params._player_anim_hold_ct > 0) {
					g._player.play_anim(_params._player_anim_hold, false);
					_params._player_anim_hold_ct -= SPUtil.dt_scale_get();
				} else {
					g._player.play_anim(PlayerCharacterAnims.INAIRIDLE);
				}
				
				this.move_x(g);
				if (g._controls.is_move_y()) {
					if (g._controls.get_move().y > 0) {
						_params._player_c_vel.y = SPUtil.lmovto(_params._player_c_vel.y, -1f, 5.0f);
					} else {
						_params._player_c_vel.y = SPUtil.lmovto(_params._player_c_vel.y, -40, 2.5f);
					}
					
				} else {
					_params._player_c_vel.y = SPUtil.lmovto(_params._player_c_vel.y, -40, 0.12f);
				}			
			}
			
			this.apply_c_vel(g);
			this.apply_c_pos_to_player(g);
			
			
			if (g._player.get_center().y < g.get_viewbox()._y1 - 200) {
				_current_mode = Mode.RescueBackToTop;
				_params._initial_c_pos = _params._player_c_pos;
				_params._anim_t = 0;
			}
			
		} break;
		case Mode.RescueBackToTop: {
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
			
			this.move_x(g);
			_params._player_c_vel.y = 0;
			this.apply_c_vel(g);
			this.apply_c_pos_to_player(g);
			
			if (_params._anim_t >= 1) {
				_current_mode = Mode.Combat;
			}
			
		} break;
		case Mode.FallToGround: {

		} break;
		}
	}
	
	private void move_x(GameEngineScene g) {
		if (g._controls.is_move_x()) {
			Vector2 move = g._controls.get_move();
			_params._player_c_vel.x = move.x * MAX_MOVE_SPEED;
			if (move.x > 0) {
				g._player.set_scale_x(-1);
			} else {
				g._player.set_scale_x(1);
			}
		} else {
			_params._player_c_vel.x = SPUtil.drpt(_params._player_c_vel.x,0,1/5.0f);
		}
	}
	private void apply_c_vel(GameEngineScene g) {
		_params._player_c_pos = SPUtil.vec_add(_params._player_c_pos,SPUtil.vec_scale(_params._player_c_vel,SPUtil.dt_scale_get()));
	}
	private void apply_c_pos_to_player(GameEngineScene g) {
		_params._player_c_pos = InAirGameState.set_player_c_pos_in_bounds(g._player,_params._player_c_pos);
	}
	
	private static Vector2 set_player_c_pos_in_bounds(PlayerCharacter player, Vector2 c_pos) {
		Vector2 u_pos = GameCameraController.c_pos_to_u_pos(c_pos);
		PlayerCharacterUtil.move_center_in_bounds(player, u_pos.x, u_pos.y);
		return GameCameraController.u_pos_to_c_pos(player.get_center());
	}

	public override GameStateIdentifier get_state() { 
		return GameStateIdentifier.InAir;
	}

}
