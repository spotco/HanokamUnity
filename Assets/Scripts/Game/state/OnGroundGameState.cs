using UnityEngine;
using System.Collections.Generic;

public class OnGroundGameState : GameStateBase {

	public struct OnGroundGameStateParams {
	public float _jump_charge_t;
	public Vector2 _vel;
	public static OnGroundGameStateParams cons() {
		OnGroundGameStateParams rtv = new OnGroundGameStateParams();
		return rtv;
	}
}

	public enum State {
		Gameplay,
		JumpCharge,
		JumpInAir
	}

	public static OnGroundGameState cons(GameEngineScene g) {
		return (new OnGroundGameState()).i_cons(g);
	}
	
	public OnGroundGameStateParams _params;
	public State _current_state;
	
	public OnGroundGameState i_cons(GameEngineScene g) {
		_params = OnGroundGameStateParams.cons();
		_current_state = State.Gameplay;
		g._camerac.set_zoom_speed(1/10.0f);
		g._camerac.set_camera_follow_speed(1/30.0f);
		return this;
	}
	
	public override void i_update(GameEngineScene g) {

		switch (_current_state) {
		case State.Gameplay:{
			g._player.set_manual_sort_z_order(GameAnchorZ.Player_Ground);
			if (g._controls.is_move_x()) {
				_params._vel.x = g._controls.get_move().x * 8.0f;
				if (Mathf.Abs(_params._vel.x) > 7.5f) {
					g._player.play_anim(PlayerCharacterAnims.RUN);
				} else {
					g._player.play_anim(PlayerCharacterAnims.WALK);
				}
				if (g._controls.get_move().x > 0) {
					g._player.set_scale_x(-1);
					
				} else if (g._controls.get_move().x < 0) {
					g._player.set_scale_x(1);
				}
			} else {
				_params._vel.x = SPUtil.drpt(_params._vel.x,0,1/10.0f);
				g._player.play_anim(PlayerCharacterAnims.IDLE);
			}
			g._player._u_x = g._player._u_x + _params._vel.x * SPUtil.dt_scale_get();
			if (g._player._u_x < SPUtil.get_horiz_world_bounds()._min) {
				g._player._u_x = SPUtil.get_horiz_world_bounds()._min;
			} else if (g._player._u_x > SPUtil.get_horiz_world_bounds()._max) {
				g._player._u_x = SPUtil.get_horiz_world_bounds()._max;
			}
			
			/*
			Vector3 player_to_cursor_delta = SPUtil.vec_sub(g._game_ui._cursor.get_game_pos(),new Vector2(g._player._u_x,g._player._u_y));
			player_to_cursor_delta.x = SPUtil.eclamp(player_to_cursor_delta.x,-400,400,new Vector2(0.25f,0),new Vector2(0.75f,1)) * 0.2f;
			player_to_cursor_delta.y = SPUtil.eclamp(player_to_cursor_delta.y,-100,900,new Vector2(0.25f,0),new Vector2(0.75f,1)) * 0.2f;
			*/
			
			g._camerac.set_target_zoom(1300);
			//g._camerac.set_target_camera_focus_on_character(g,player_to_cursor_delta.x,player_to_cursor_delta.y);
			//g._game_ui._cursor.set_enabled(true);
			g._camerac.set_target_camera_focus_on_character(g,0,200);
			
			if (g._controls.get_control_just_pressed(ControlManager.Control.OnGround_Jump)) {
				_current_state = State.JumpCharge;
				g._player.play_anim(PlayerCharacterAnims.PREPDIVE,false);
				_params._jump_charge_t = 0;
			}
			
		} break;
		case State.JumpCharge:{
			g._camerac.set_target_camera_focus_on_character(g,0,120);
			g._camerac.set_target_zoom(500);
			
			_params._jump_charge_t = Mathf.Clamp(_params._jump_charge_t + SPUtil.dt_scale_get() * SPUtil.sec_to_tick(1.0f),0,1);
			
			if (_params._jump_charge_t >= 1) {
				_current_state = State.JumpInAir;
				_params._vel = new Vector2(0,15);
				
			} else if (!g._controls.get_control_down(ControlManager.Control.OnGround_Jump)) {
				_current_state = State.Gameplay;
				g._camerac.set_zoom_speed(1/10.0f);
			}
			
		} break;
		case State.JumpInAir:{
			g._player.set_manual_sort_z_order(GameAnchorZ.Player_InAir);
			g._camerac.set_target_camera_focus_on_character(g,0,-60);
			g._camerac.set_target_zoom(1000);
			if (g._controls.is_move_x()) {
				_params._vel.x = g._controls.get_move().x * 3.0f;
			} else {
				_params._vel.x = SPUtil.drpt(_params._vel.x,0,1/10.0f);
			}
			g._player.set_u_pos(
				g._player._u_x + _params._vel.x * SPUtil.dt_scale_get(),
				g._player._u_y + _params._vel.y * SPUtil.dt_scale_get()
			);
			_params._vel.y -= 0.4f * SPUtil.dt_scale_get();

			if (g._player._u_y > 250) {
				g._player.play_anim(PlayerCharacterAnims.SPIN);
			} else {
				g._player.play_anim(PlayerCharacterAnims.DIVE);
				PlayerCharacterUtil.rotate_to_rotation_for_vel(g._player,_params._vel.x,_params._vel.y,1/10.0f);
			}
			
			if (g._player._u_y < -250) {
				g.pop_top_game_state();
				g.push_game_state(DiveGameState.cons(g));
				g._camerac.camera_shake(new Vector2(-1.7f,2.1f),80,400, 1/100.0f);
				g._camerac.camera_motion_blur(new Vector3(0,500,500), 60.0f);
				g._camerac.camera_blur(45.0f);
			}

		} break;
		}
	}
	
	public override GameStateIdentifier get_state() {
		return GameStateIdentifier.OnGround;
	}
	
	public override void on_state_end(GameEngineScene g) {
		Debug.Log("state end");
	}
}
