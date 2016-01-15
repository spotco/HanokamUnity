using UnityEngine;
using System.Collections.Generic;

public interface OnGroundStateUpdateable {
	void i_update(GameEngineScene g, OnGroundGameState state);
}

public class OnGroundGameState : GameStateBase {

	public struct Params {
		public Vector2 _vel;
		public Mode _current_state;
		public OnGroundDiveAnimController _dive_anim_controller;
		
		public static Params cons() {
			Params rtv = new Params();
			rtv._dive_anim_controller = OnGroundDiveAnimController.cons();
			return rtv;
		}
	}

	public enum Mode {
		FadeIn,
		Gameplay,
		DiveAnim
	}

	public static OnGroundGameState cons(GameEngineScene g) {
		return (new OnGroundGameState()).i_cons(g);
	}
	
	public Params _params;
	public VillagerManager _villager_manager;
	
	public OnGroundGameState i_cons(GameEngineScene g) {
		_params = Params.cons();
		
		_params._current_state = Mode.FadeIn;
		g._game_ui.set_fadeout_overlay_imm(true);
		
		g._player.set_u_pos(0,-16);
		g._player.play_anim(PlayerCharacterAnims.IDLE);
		g._bg_village.set_u_pos(0, 0);
		g._bg_water.set_u_pos(0, 0);
		g._bg_sky.set_y_offset(0);
		
		g._player.set_layer(RLayer.REFLECTION_SURFACE_CHARACTER);
		g._player.set_manual_sort_z_order(GameAnchorZ.Player_Ground);
		
		_villager_manager = VillagerManager.cons(g);
		return this;
	}
	
	public override void i_update(GameEngineScene g) {
		g._player.i_update(g);
		
		switch (_params._current_state) {
		case Mode.FadeIn: {
			g._player.i_update(g);
			g._game_ui.set_fadeout_overlay(false);
			if (g._game_ui.get_fadeout_overlay_anim_finished_for_target(false)) {
				_params._current_state = Mode.Gameplay;
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
			
			if (g._controls.get_control_just_released(ControlManager.Control.OnGround_Jump)) {
				_params._current_state = Mode.DiveAnim;
				_params._dive_anim_controller.setup_dive_anim(g,this);
			}
			
		} break;
		case Mode.DiveAnim:{
			_params._dive_anim_controller.i_update(g,this);
			if (_params._dive_anim_controller.is_finished_and_should_transition_to_divestate()) {
				_params._dive_anim_controller.finish_dive_anim(g,this);
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
