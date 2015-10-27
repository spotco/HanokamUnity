﻿using UnityEngine;
using System.Collections.Generic;

public interface InAirGameStateUpdateable {
	void i_update(GameEngineScene g, InAirGameState state);
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
		
		public float _arrow_charge_ct;
		public float _last_arrow_dir_ang, _time_dashing, _pre_dash_rotation;
		public float get_arrow_charge_ct_max() { return 50; }
		public float get_arrow_charge_pct() { return this._arrow_charge_ct / this.get_arrow_charge_ct_max(); }
		
		public bool _this_movepress_has_aimed;
		
		public enum PlayerMode {
			None,
			BowAim,
			Dash,
			SwordPlant,
			Hurt
		}
		public PlayerMode _player_mode;
		public float _dash_ct;
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
	public AirProjectileManager _projectiles;
	public AirEnemyManager _enemy_manager;
	
	private InAirGameState i_cons(GameEngineScene g) {
		_enemy_manager = AirEnemyManager.cons(g);
		_projectiles = AirProjectileManager.cons(g);
		g._player.set_manual_sort_z_order(GameAnchorZ.Player_InAir);
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
		_params._player_mode = Params.PlayerMode.None;
		
		return this;
	}

	public override void i_update(GameEngineScene g) {
		if (_enemy_manager._active_enemies.Count == 0 && _enemy_manager._queued_spawn_enemies.Count == 0) {
			Vector2 c_bottom = GameCameraController.u_pos_to_c_pos(new Vector2(0,g.get_viewbox()._y1));
			c_bottom.x = SPUtil.float_random(SPUtil.get_horiz_world_bounds()._min+200,SPUtil.get_horiz_world_bounds()._max-200);
			_enemy_manager.add_enemy(PufferBasicAirEnemy.cons(g, new Vector2(500,0)), c_bottom, 30);
		}
	
		g._player.i_update(g);
		_projectiles.i_update(g, this);
		_enemy_manager.i_update(g, this);

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
			
			this.player_control_move_x(g);
			_params._player_c_vel.y = 0;
			this.apply_c_vel(g);
			this.apply_c_pos_to_player(g);

			if (_params._anim_t >= 1) {
				_current_mode = Mode.Combat;
			}

		} break;
		case Mode.Combat: {
			this.i_update_mode_combat_player_controls(g);
			
		} break;
		case Mode.RescueBackToTop: {
			g._player.play_anim(PlayerCharacterAnims.INAIRIDLE);
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
			
			this.player_control_move_x(g);
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
	
	private void i_update_mode_combat_player_controls(GameEngineScene g) {
		_params._target_y += _params._upwards_vel * SPUtil.dt_scale_get();
		_params._upwards_vel = SPUtil.drpt(_params._upwards_vel,0,1/50.0f);
		g._camerac.set_target_camera_y(_params._target_y);
		
		bool streak_enabled = false;
		bool aim_retic_enabled = false;
		switch (_params._player_mode) {
		case Params.PlayerMode.None: {
			_params._time_dashing = 0;
			if (_params._player_anim_hold_ct > 0) {
				g._player.play_anim(_params._player_anim_hold, false);
				_params._player_anim_hold_ct -= SPUtil.dt_scale_get();
			} else {
				g._player.play_anim(PlayerCharacterAnims.INAIRIDLE);
			}
			
			if (!_params._this_movepress_has_aimed) {
				if (g._controls.is_move_x()) {
					PlayerCharacterUtil.rotate_to_rotation(g._player,0, 1/10.0f);
				}
				this.player_control_move_x(g);
				if (g._controls.is_move_y()) {
					if (g._controls.get_move().y > 0 && _params._player_c_vel.y < 0) {
						_params._player_c_vel.y = SPUtil.lmovto(_params._player_c_vel.y, -1f, 5.0f * SPUtil.dt_scale_get());
					} else if (g._controls.get_move().y < 0) {
						_params._player_c_vel.y = SPUtil.lmovto(_params._player_c_vel.y, -30, 0.45f * SPUtil.dt_scale_get());
					}
					
				} else {
					_params._player_c_vel.x = SPUtil.drpt(_params._player_c_vel.x,0,1/5.0f);
					_params._player_c_vel.y = SPUtil.lmovto(_params._player_c_vel.y, -30, 0.12f * SPUtil.dt_scale_get());
				}
			} else {
				_params._player_c_vel.x = SPUtil.drpt(_params._player_c_vel.x,0,1/5.0f);
				_params._player_c_vel.y = SPUtil.lmovto(_params._player_c_vel.y, -30, 0.12f * SPUtil.dt_scale_get());
			}
			if (g._controls.get_control_down(ControlManager.Control.ShootArrow)) {
				_params._arrow_charge_ct = 0;
				_params._player_mode = Params.PlayerMode.BowAim;
			}
			if (_params._player_c_vel.y > 0) {
				_params._player_c_vel.y = SPUtil.drpt(_params._player_c_vel.y,0,1/10.0f);
			}
			
			this.player_control_check_sword_move(g);
			
		} break;
		case Params.PlayerMode.BowAim: {
			_params._arrow_charge_ct = Mathf.Clamp(_params._arrow_charge_ct + SPUtil.dt_scale_get(), 0, _params.get_arrow_charge_ct_max());
			aim_retic_enabled = true;
			g._player._aim_retic.set_aim_variance_and_charge_pct(g,30,_params.get_arrow_charge_pct());
			
			if (_params.get_arrow_charge_pct() < 1) {
				g._player.play_anim(PlayerCharacterAnims.BOWAIM, false);
			} else {
				g._player.play_anim(PlayerCharacterAnims.BOWHOLD);
			}
			
			if (g._controls.is_move_x() || g._controls.is_move_y()) {
				_params._this_movepress_has_aimed = true;
				if (g._controls.get_control_down(ControlManager.Control.MoveLeft)) {
					if (g._player.scale_x() < 0) {
						g._player.set_rotation(g._player.rotation() + 120);
					}
					g._player.set_scale_x(1);
				} else if (g._controls.get_control_down(ControlManager.Control.MoveRight)) {
					if (g._player.scale_x() > 0) {
						g._player.set_rotation(g._player.rotation() - 120);
					}
					g._player.set_scale_x(-1);
				}
				PlayerCharacterUtil.rotate_to_rotation(g._player,g._player.get_target_rotation_for_aim_direction(g._controls.get_move()), 1/10.0f);
			}
			_params._last_arrow_dir_ang = g._player.get_arrow_target_rotation();
			
			_params._player_c_vel.x = SPUtil.drpt(_params._player_c_vel.x,0,1/5.0f);
			_params._player_c_vel.y = SPUtil.lmovto(_params._player_c_vel.y, -1f, 5.0f * SPUtil.dt_scale_get());
			
			if (!g._controls.get_control_down(ControlManager.Control.ShootArrow)) {
				Vector2 pushback_dir = SPUtil.vec_scale(
					SPUtil.ang_deg_dir(g._player.get_arrow_target_rotation() + 90).normalized,
					-1 * SPUtil.y_for_point_of_2pt_line(new Vector2(0,0.25f),new Vector2(1,4),_params.get_arrow_charge_pct()));
				_params._player_c_vel = SPUtil.vec_add(pushback_dir,pushback_dir);
				_params._upwards_vel = Mathf.Clamp(pushback_dir.y,0,10);
				
				this.shoot_arrow(g,g._player.get_arrow_target_rotation());
				_params._player_anim_hold = PlayerCharacterAnims.BOWFIRE;
				_params._player_anim_hold_ct = 30.0f;
				_params._player_mode = Params.PlayerMode.None;
				
			}
			
			this.player_control_check_sword_move(g);
			
		} break;
		case Params.PlayerMode.Dash: {
			g._player.play_anim(PlayerCharacterAnims.SPIN);
			g._player.set_rotation(0);
			
			if (g._controls.is_move_x() || g._controls.is_move_y()) {
				Vector2 move = SPUtil.vec_scale(g._controls.get_move(),20);
				_params._player_c_vel = move;
				PlayerCharacterUtil.rotate_to_rotation_for_vel(g._player,_params._player_c_vel.x,_params._player_c_vel.y,1/5.0f,0);
			} else {
				_params._player_c_vel.x = SPUtil.drpt(_params._player_c_vel.x,0,1/20.0f);
				_params._player_c_vel.y = SPUtil.drpt(_params._player_c_vel.y,0,1/20.0f);
			}
			
			if (_params._time_dashing < 20) {
				if (_params._arrow_charge_ct > 0 && g._controls.get_control_just_released(ControlManager.Control.ShootArrow)) {
					this.shoot_arrow(g,_params._last_arrow_dir_ang);
				}
			}
			_params._time_dashing += SPUtil.dt_scale_get();
			
			
			_params._dash_ct -= SPUtil.dt_scale_get();
			bool down = g._controls.get_control_down(ControlManager.Control.MoveDown);
			bool up = g._controls.get_control_down(ControlManager.Control.MoveUp);
			bool left = g._controls.get_control_down(ControlManager.Control.MoveLeft);
			bool right = g._controls.get_control_down(ControlManager.Control.MoveRight);
			
			if (_params._dash_ct <= 0) {
				g._player.set_rotation(_params._pre_dash_rotation);
				_params._player_mode = Params.PlayerMode.None;
			} else if (down && !up && !left && !right && g._controls.get_control_down(ControlManager.Control.Dash)) {
				_params._player_mode = Params.PlayerMode.SwordPlant;
			}
			
		} break;
		case Params.PlayerMode.SwordPlant: {
			g._player.play_anim(PlayerCharacterAnims.SWORDPLANT);
			g._player.set_rotation(0);
			_params._pre_dash_rotation = 0;
			streak_enabled = true;
			if (_params._time_dashing < 20) {
				if (_params._arrow_charge_ct > 0 && g._controls.get_control_just_released(ControlManager.Control.ShootArrow)) {
					this.shoot_arrow(g,_params._last_arrow_dir_ang);
				}
			}
			_params._time_dashing += SPUtil.dt_scale_get();
			_params._player_c_vel.x = SPUtil.drpt(_params._player_c_vel.x, 0, 1/10.0f);
			_params._player_c_vel.y = SPUtil.drpt(_params._player_c_vel.y, -40, 1/3.0f);
			if (!g._controls.get_control_down(ControlManager.Control.Dash)) {
				_params._player_mode = Params.PlayerMode.None;
				_params._player_c_vel.y = -10;
			}
			
		} break;
		case Params.PlayerMode.Hurt: {
			g._player.play_anim(PlayerCharacterAnims.INAIRHURT);
			
		} break;
		}
		
		if (g._controls.get_control_just_pressed(ControlManager.Control.MoveDown) ||
		    g._controls.get_control_just_pressed(ControlManager.Control.MoveUp) ||
		    g._controls.get_control_just_pressed(ControlManager.Control.MoveLeft) ||
		    g._controls.get_control_just_pressed(ControlManager.Control.MoveRight)) {
			_params._this_movepress_has_aimed = false;
		}
		
		if (_params._player_c_pos.y > 500) {
			float tar_player_c_pos_y = SPUtil.drpt(_params._player_c_pos.y, 500, 1/10.0f);
			_params._upwards_vel = Mathf.Clamp(_params._player_c_pos.y-tar_player_c_pos_y,0,10);
			_params._player_c_pos.y = tar_player_c_pos_y;
		}
		
		
		this.apply_c_vel(g);
		this.apply_c_pos_to_player(g);
		
		
		if (g._player.get_center().y < g.get_viewbox()._y1 - 200) {
			_current_mode = Mode.RescueBackToTop;
			_params._initial_c_pos = _params._player_c_pos;
			_params._anim_t = 0;
			_params._player_mode = Params.PlayerMode.None;
			_params._player_c_vel = new Vector2(0,0);
			streak_enabled = false;
			g._player.set_rotation(0);
		}
		g._player.set_streak_enabled(streak_enabled);
		g._player.set_trail_enabled_and_rotation(streak_enabled,g._player.rotation());
		g._player._aim_retic.set_enabled(aim_retic_enabled);
	}
	
	private void shoot_arrow(GameEngineScene g, float angle) {
		if (_params.get_arrow_charge_pct() < 1) {
			_projectiles.add_player_projectile(
				PlayerArrowAirProjectile.cons(
					g._player.get_center(), 
					SPUtil.ang_deg_dir(angle+90),
					SPUtil.y_for_point_of_2pt_line(new Vector2(0,13),new Vector2(1,30),_params.get_arrow_charge_pct())));
		} else {
			g._camerac.camera_shake(new Vector2(-2,3.3f),35,30);
			_projectiles.add_player_projectile(
				PlayerChargedArrowAirProjectile.cons(
					g._player.get_center(), 
					SPUtil.ang_deg_dir(angle+90),
					30));
		}
		_params._arrow_charge_ct = 0;
	}
	
	private void player_control_check_sword_move(GameEngineScene g) {
		if (g._controls.get_control_just_pressed(ControlManager.Control.Dash)) {
			bool down = g._controls.get_control_down(ControlManager.Control.MoveDown);
			bool up = g._controls.get_control_down(ControlManager.Control.MoveUp);
			bool left = g._controls.get_control_down(ControlManager.Control.MoveLeft);
			bool right = g._controls.get_control_down(ControlManager.Control.MoveRight);
			if (down && !up && !left && !right) {
				_params._player_mode = Params.PlayerMode.SwordPlant;
			} else {
				_params._pre_dash_rotation = g._player.rotation();
				_params._player_mode = Params.PlayerMode.Dash;
				_params._dash_ct = 20;
			}
		}
	}
	
	private void player_control_move_x(GameEngineScene g) {
		if (g._controls.is_move_x()) {
			Vector2 move = g._controls.get_move();
			_params._player_c_vel.x = move.x * 13;
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
	
	public override void debug_draw_hitboxes(SPDebugRender draw) {
		_projectiles.debug_draw_hitboxes(draw);
		_enemy_manager.debug_draw_hitboxes(draw);
	}

}
