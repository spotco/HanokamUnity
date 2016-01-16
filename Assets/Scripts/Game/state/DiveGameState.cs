using UnityEngine;
using System.Collections;

public interface DiveGameStateUpdateable {
	void i_update(GameEngineScene g, DiveGameState state);
}

public class DiveGameState : GameStateBase {

	public struct Params {
		public Vector2 _player_pos;
		public Vector2 _vel;
		public DiveGameState.Mode _mode;
		
		public bool _dashing;
		public float _dash_ct;
		public bool _dash_has_hit;
		public float _turn_mode_delay_ct;

		public float _invuln_ct;
		public bool is_invuln() {
			return _invuln_ct > 0;
		}

		public float _ground_depth;
		public void set_ground_depth(GameEngineScene g, float val) {
			_ground_depth = val;
			g._bg_water.set_ground_depth(_ground_depth);
		}
		
		public float DASH_SPEED() { return 23; }
		public float MAX_MOVE_SPEED() { return 10; }
		public float TURN_SPEED() { return 3; }
		public float DASH_CT_MAX() { return 20; }
		
		public float _current_breath;
		public float MAX_BREATH() { return 10000; }
		
		public float _camera_offset_y;
		
		public float _transition_anim_t;
	}

	public enum Mode {
		TransitionIn,
		
		Gameplay,
		
		OutOfAirAnimationZoomIn,
		OutOfAirAnimationFallOff,
		
		SwimToUnderwaterTreasure,
		PickupTreasure
	}

	public static DiveGameState cons(GameEngineScene g) {
		return (new DiveGameState()).i_cons(g);
	}

	public Params _params;
	private FlashEvery _bubble_every;
	public WaterEnemyManager _enemy_manager;

	public DiveGameState i_cons(GameEngineScene g) {
		_params._vel = new Vector2(0,-22);
		_params._mode = Mode.TransitionIn;
		_params._dashing = false;
		_params.set_ground_depth(g,-10000);
		_params._player_pos = g._player.get_u_pos();
		_params._current_breath = _params.MAX_BREATH();

		
		g._player.play_anim(PlayerCharacterAnims.SWIM);
		g._player.set_layer(RLayer.DEFAULT);
		g._player.set_rotation(-180);
		
		MiscEffects.do_underwater_splash(g);
		g._delayed_actions.enqueue_action(new DelayedAction() {
			_time_left = 20,
			_callback = (__g) => { MiscEffects.do_player_bubble(g); }
		});
		
		g._player.set_manual_sort_z_order(GameAnchorZ.Player_UnderWater);
		_bubble_every = FlashEvery.cons(30);
		_enemy_manager = WaterEnemyManager.cons(g,this);
		UnderwaterBubbleParticle.proc_multiple_bubbles(g);
		return this;
	}
	
	public override void i_update(GameEngineScene g) {
		g._player.i_update(g);
		_enemy_manager.i_update(g, this);

		switch (_params._mode) {
		case (Mode.TransitionIn): {
			g._camerac.set_target_camera_focus_on_character(g,0,-200);
			g._camerac.set_target_zoom(1000);
			if (g._controls.is_move_x()) {
				Vector2 move = g._controls.get_move();
				_params._vel.x = Mathf.Clamp(_params._vel.x + move.x * _params.TURN_SPEED(), -_params.MAX_MOVE_SPEED(), _params.MAX_MOVE_SPEED());
			} else {
				_params._vel.x = SPUtil.drpt(_params._vel.x,0,1/30.0f);
			}
			PlayerCharacterUtil.rotate_to_rotation_for_vel(g._player,_params._vel.x,_params._vel.y,1/10.0f);
			
			_params._player_pos = PlayerCharacterUtil.pos_in_bounds(
				_params._player_pos.x + _params._vel.x * SPUtil.dt_scale_get(),
				_params._player_pos.y
			);
			_params._player_pos.y += _params._vel.y * SPUtil.dt_scale_get();
			if (_params._player_pos.y < -1000) {
				_params._mode = Mode.Gameplay;
				g._camerac.set_target_zoom(1500);
			}
			
			this.apply_environment_offset_pos(g);
			this.apply_player_offset_position(g);
			
		} break;
		case (Mode.Gameplay): {
			float UPPER_CAMERA_MAX_OFFSET = -200;
			float LOWER_CAMERA_MAX_OFFSET = 400;
			float tar_camera_offset_y = SPUtil.eclamp(
				SPUtil.y_for_point_of_2pt_line(
					new Vector2(-_params.MAX_MOVE_SPEED(),UPPER_CAMERA_MAX_OFFSET),
					new Vector2(_params.MAX_MOVE_SPEED(),LOWER_CAMERA_MAX_OFFSET),
					_params._vel.y
				), 
				UPPER_CAMERA_MAX_OFFSET, LOWER_CAMERA_MAX_OFFSET, 
				new Vector2(0.5f,0), new Vector2(0.5f,1));
			_params._camera_offset_y = SPUtil.drpt(_params._camera_offset_y,tar_camera_offset_y,1/10.0f);
			g._camerac.set_target_camera_focus_on_character(g,0,_params._camera_offset_y);
			
			bool turn_mode = false;
			if (!(_params._dashing && _params._dash_has_hit) && !(_params.is_invuln())) {
				if (g._controls.get_control_down(ControlManager.Control.ShootArrow) && !_params._dashing && _params._turn_mode_delay_ct <= 0) {
					if (g._controls.is_move_x() || g._controls.is_move_y()) {
						Vector2 dir = g._controls.get_move();
						float rotation_pre = g._player.rotation();
						PlayerCharacterUtil.rotate_to_rotation_for_vel(g._player,dir.x,dir.y,1/10.0f);
						float rotation_post = g._player.rotation();
						if (Mathf.Abs(rotation_post-rotation_pre) > 1.0f) {
							turn_mode = true;
						}
					}
					_params._vel.x = SPUtil.drpt(_params._vel.x,0,1/7.0f);
					_params._vel.y = SPUtil.drpt(_params._vel.y,0,1/7.0f);
					
					
				} else {
					bool is_move = (g._controls.is_move_x() || g._controls.is_move_y());
					Vector2 move = g._controls.get_move().normalized;
				
					if (_params._dashing && is_move) {
						float cancel_angle = SPUtil.rad_to_deg(Mathf.Acos(SPUtil.vec_dot(move,_params._vel.normalized)));
						if (cancel_angle > 120) {
							_params._dash_ct = 0;
							_params._dashing = false;
						}
					}
				
					float move_speed = _params._dashing ? _params.DASH_SPEED() : _params.MAX_MOVE_SPEED();					
					
					if (is_move) {
						Vector2 cur_vel = _params._vel;
						Vector2 tar_vel = SPUtil.vec_scale(move,move_speed);
						Vector2 delta = SPUtil.vec_sub(tar_vel,cur_vel);
						float delta_mag = delta.magnitude;
						
						float movto_drpt_val = _params._dashing ? 0.1f : 0.15f;
						
						_params._vel = SPUtil.vec_add(
							_params._vel, 
							SPUtil.vec_scale(delta, SPUtil.drpty(movto_drpt_val))
						);
						
					} else if (!_params._dashing) {
						_params._vel.x = SPUtil.drpt(_params._vel.x,0,1/30.0f);
						_params._vel.y = SPUtil.drpt(_params._vel.y,0,1/30.0f);
					}
					
					PlayerCharacterUtil.rotate_to_rotation_for_vel(g._player,_params._vel.x,_params._vel.y,1/10.0f);
				}
			}
			
			if (_params.is_invuln()) {
				g._player.play_anim(PlayerCharacterAnims.SWIMHURT);
				_params._dash_ct = 0;
				_params._dashing = false;
			
			} else if (_params._dashing) {
				if (_params._dash_has_hit) {
					g._player.play_anim(PlayerCharacterAnims.SPIN);
				} else {
					g._player.show_waterdash_for(1.0f);
					g._player.play_anim(PlayerCharacterAnims.SWIM_SPIN);
				}
				_params._dash_ct -= SPUtil.dt_scale_get();
				if (_params._dash_ct <= 0) {
					_params._dashing = false;
					_params._turn_mode_delay_ct = 10;
				}
			} else {
				if (turn_mode) {
					g._player.play_anim(PlayerCharacterAnims.SWIM);
				} else if (_params._vel.magnitude > 2) {
					g._player.play_anim(PlayerCharacterAnims.SWIM);
				} else {
					g._player.play_anim(PlayerCharacterAnims.SWIM_SLOW);
				}
			}
			
			_params._invuln_ct = Mathf.Max(_params._invuln_ct-SPUtil.dt_scale_get(),0);
			_params._turn_mode_delay_ct = Mathf.Max(_params._turn_mode_delay_ct-SPUtil.dt_scale_get(),0);
			
			if (g._controls.get_control_just_pressed(ControlManager.Control.Dash) && !_params._dashing && !_params.is_invuln()) {
				Vector2 dir = SPUtil.ang_deg_dir(g._player.rotation() + 90);
				if (dir.y < -0.65f && SPUtil.float_random(0,3) < 1) {
					MiscEffects.do_player_bubble(g);
				}
				dir = SPUtil.vec_scale(dir,_params.DASH_SPEED());
				_params._vel = dir;
				_params._dashing = true;
				_params._dash_has_hit = false;
				_params._dash_ct = _params.DASH_CT_MAX();
				UnderwaterBubbleParticle.proc_multiple_bubbles(g);
			}
			
			_bubble_every.i_update();
			if (_bubble_every.do_flash()) {
				_bubble_every._max_time = SPUtil.int_random(0,4) == 0 ? SPUtil.float_random(1,3) : SPUtil.float_random(20,40);
				UnderwaterBubbleParticle.proc_bubble(g);
			}
			
			_params._current_breath -= SPUtil.dt_scale_get();
			if (_params._current_breath <= 0) {
				_params._transition_anim_t = 70;
				_params._mode = Mode.OutOfAirAnimationZoomIn;
				
			} else if (_params._player_pos.y > -500) {
				_params._player_pos.y = -500;
				
			} else if (_params._player_pos.y < _params._ground_depth + 700) {
				UnderwaterBubbleParticle.proc_multiple_bubbles(g);
				_params._mode = Mode.SwimToUnderwaterTreasure;
			}
			_params._player_pos = PlayerCharacterUtil.pos_in_bounds(
				_params._player_pos.x + _params._vel.x * SPUtil.dt_scale_get(),
				_params._player_pos.y
				);
			_params._player_pos.y += _params._vel.y * SPUtil.dt_scale_get();
			
			this.apply_environment_offset_pos(g);
			this.apply_player_offset_position(g);
			
		} break;
		case Mode.OutOfAirAnimationZoomIn: {
			g._player.play_anim(PlayerCharacterAnims.SWIMHURT);
			g._camerac.set_target_camera_focus_on_character(g,0,50);
			g._camerac.set_target_zoom(700);
			_params._transition_anim_t -= SPUtil.dt_scale_get();
			if (_params._transition_anim_t <= 0) {
				_params._transition_anim_t = 200;
				_params._mode = Mode.OutOfAirAnimationFallOff;
			}
			
			this.apply_environment_offset_pos(g);
			this.apply_player_offset_position(g);
			
		} break;
		case Mode.OutOfAirAnimationFallOff: {
			g._player.play_anim(PlayerCharacterAnims.SWIMHURT);
			g._player.set_rotation(g._player.rotation() + 5 * SPUtil.dt_scale_get());
			g._player._u_y = g._player._u_y - 6 * SPUtil.dt_scale_get();
			
			_params._transition_anim_t -= SPUtil.dt_scale_get();
			if (_params._transition_anim_t <= 0) {
				DiveReturnGameState neu_state = DiveReturnGameState.cons(g,_params,_enemy_manager);
				g.pop_top_game_state();
				g.push_game_state(neu_state);
				return;
			}
			this.apply_environment_offset_pos(g);
			
		} break;
		default: break;
		}
	}
	
	private void apply_environment_offset_pos(GameEngineScene g) {
		g._bg_water.set_y_offset(_params._player_pos.y);
		g._bg_village.set_u_pos(0, -_params._player_pos.y);
		_enemy_manager.set_offset_y(_params._player_pos.y);
	}
	
	private void apply_player_offset_position(GameEngineScene g) {
		g._player.set_u_pos(_params._player_pos.x, 0);
	}

	public override void debug_draw_hitboxes(SPDebugRender draw) {
		_enemy_manager.debug_draw_hitboxes(draw);
	}

	public override void on_state_end(GameEngineScene g) {}

	public override GameStateIdentifier get_state() { 
		return GameStateIdentifier.Dive; 
	}
}
