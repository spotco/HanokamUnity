using UnityEngine;
using System.Collections.Generic;

public interface OnGroundStateUpdateable {
	void i_update(GameEngineScene g, OnGroundGameState state);
}

public class OnGroundGameState : GameStateBase {

	public struct Params {
		public float _jump_charge_t;
		public Vector2 _vel;
		
		public static Params cons() {
			Params rtv = new Params();
			return rtv;
		}
	}

	public enum Mode {
		FadeIn,
		Gameplay,
		JumpCharge,
		JumpInAir
	}

	public static OnGroundGameState cons(GameEngineScene g) {
		return (new OnGroundGameState()).i_cons(g);
	}
	
	public Params _params;
	public Mode _current_state;
	public VillagerManager _villager_manager;
	
	public OnGroundGameState i_cons(GameEngineScene g) {
		_params = Params.cons();
		
		_current_state = Mode.FadeIn;
		g._game_ui.set_fadeout_overlay_imm(true);
		
		g._player.set_u_pos(0,-16);
		g._player.play_anim(PlayerCharacterAnims.IDLE);
		g._bg_village.set_u_pos(0, 0);
		g._bg_water.set_u_pos(0, 0);
		g._bg_sky.set_y_offset(0);
		
		_villager_manager = VillagerManager.cons(g);
		return this;
	}
	
	public override void i_update(GameEngineScene g) {
		g._player.i_update(g);
		
		switch (_current_state) {
		case Mode.FadeIn: {
			g._player.i_update(g);
			g._game_ui.set_fadeout_overlay(false);
			if (g._game_ui.get_fadeout_overlay_anim_finished_for_target(false)) {
				_current_state = Mode.Gameplay;
			}
		} break;
		case Mode.Gameplay:{
			_villager_manager.i_update(g,this);
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
			
			//g._camerac.set_target_zoom(1500);
			g._camerac.set_target_zoom(1000);
			g._camerac.set_target_camera_focus_on_character(g,0,200);
			
			if (g._controls.get_control_just_pressed(ControlManager.Control.OnGround_Jump)) {
				_current_state = Mode.JumpCharge;
				g._player.play_anim(PlayerCharacterAnims.PREPDIVE,false);
				_params._jump_charge_t = 0;
			}
			
		} break;
		case Mode.JumpCharge:{
			g._camerac.set_target_zoom(500);
			g._camerac.set_target_camera_focus_on_character(g,0,120);
			
			_params._jump_charge_t = Mathf.Clamp(_params._jump_charge_t + SPUtil.dt_scale_get() * SPUtil.sec_to_tick(1.0f),0,1);
			
			if (_params._jump_charge_t >= 1) {
				_current_state = Mode.JumpInAir;
				_params._vel = new Vector2(0,15);
				
			} else if (!g._controls.get_control_down(ControlManager.Control.OnGround_Jump)) {
				_current_state = Mode.Gameplay;
			}
			
		} break;
		case Mode.JumpInAir:{
			g._player.set_manual_sort_z_order(GameAnchorZ.Player_InAir);
			
			g._camerac.set_target_zoom(1000);
			g._camerac.set_target_camera_focus_on_character(g,0,-60);
			
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
	
	public void notify_chat_with_villager(GameEngineScene g, Villager villager) {
		g.push_game_state(DialogueGameState.cons(g, villager));
	}
	
	public void notify_enter_shop(GameEngineScene g) {
		GameMain._context.push_scene(ShopScene.cons());
	}
	
	public override GameStateIdentifier get_state() {
		return GameStateIdentifier.OnGround;
	}
	
	public override void on_state_end(GameEngineScene g) {
		_villager_manager.repool();
	}
}
