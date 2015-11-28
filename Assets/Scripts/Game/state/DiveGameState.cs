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

		public float _ground_depth;
		
		public float DASH_SPEED() { return 23; }
		public float MAX_MOVE_SPEED() { return 10; }
		public float TURN_SPEED() { return 3; }
		public float _camera_offset_y;
	}

	public enum Mode {
		TransitionIn,
		Gameplay,
		SwimToUnderwaterTreasure,
		PickupTreasure
	}

	public static DiveGameState cons(GameEngineScene g) {
		return (new DiveGameState()).i_cons(g);
	}

	public Params _params;
	private FlashEvery _bubble_every;
	private WaterEnemyManager _enemy_manager;

	public DiveGameState i_cons(GameEngineScene g) {
		_params._vel = new Vector2(0,-22);
		_params._mode = Mode.TransitionIn;
		_params._dashing = false;
		_params._ground_depth = -10000;
		_params._player_pos = g._player.get_u_pos();

		g._bg_water.set_ground_depth(_params._ground_depth);
		g._player.play_anim(PlayerCharacterAnims.SWIM);
		MiscEffects.do_underwater_splash(g);
		g._delayed_actions.enqueue_action(new DelayedAction() {
			_time_left = 20,
			_callback = (__g) => { MiscEffects.do_player_bubble(g); }
		});
		
		g._player.set_manual_sort_z_order(GameAnchorZ.Player_UnderWater);
		_bubble_every = FlashEvery.cons(30);
		_enemy_manager = WaterEnemyManager.cons(g);
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
				g._camerac.set_camera_follow_speed(1/15.0f);
				g._camerac.set_target_zoom(1500);
			}
			
		} break;
		case (Mode.Gameplay): {
			float MIN_Y_OFFSET = -400;
			float MAX_Y_OFFSET = 400;
			_params._camera_offset_y = SPUtil.eclamp(
				SPUtil.y_for_point_of_2pt_line(
					new Vector2(-_params.MAX_MOVE_SPEED(),MIN_Y_OFFSET),
					new Vector2(_params.MAX_MOVE_SPEED(),MAX_Y_OFFSET),
					_params._vel.y
				), 
				MIN_Y_OFFSET, MAX_Y_OFFSET, 
				new Vector2(0.5f,0), new Vector2(0.5f,1));
			g._camerac.set_target_camera_focus_on_character(g,0,_params._camera_offset_y);
			
			if (_params._dashing) {
				_params._camera_offset_y = SPUtil.y_for_point_of_2pt_line(new Vector2(-10,-450),new Vector2(10,100),_params._vel.y);
				g._player.play_anim(PlayerCharacterAnims.SPIN);
				_params._dash_ct -= SPUtil.dt_scale_get();
				if (_params._dash_ct <= 0) {
					_params._dashing = false;
				}
			} else {
				if (_params._vel.magnitude > 2) {
					g._player.play_anim(PlayerCharacterAnims.SWIM);
				} else {
					g._player.play_anim(PlayerCharacterAnims.SWIM_SLOW);
				}
			}
			
			float move_speed = _params._dashing ? _params.DASH_SPEED() : _params.MAX_MOVE_SPEED();
								
			Vector2 move = g._controls.get_move();
			if (move.magnitude > 1) move.Normalize();
			if (g._controls.is_move_x()) {
				float cur_vel_x = _params._vel.x;
				float tar_vel_x = Mathf.Clamp(_params._vel.x + move.x * _params.TURN_SPEED(), -move_speed, move_speed);
				float drpt_spd = Mathf.Abs(cur_vel_x) < _params.MAX_MOVE_SPEED() ? 1.0f : 1/2.0f;
				_params._vel.x = SPUtil.drpt(cur_vel_x,tar_vel_x,drpt_spd);
			} else {
				_params._vel.x = SPUtil.drpt(_params._vel.x,0,1/30.0f);
			}
			if (g._controls.is_move_y()) {
				float cur_vel_y = _params._vel.y;
				float tar_vel_y = Mathf.Clamp(_params._vel.y + move.y * _params.TURN_SPEED(), -move_speed, move_speed);
				float drpt_spd = Mathf.Abs(cur_vel_y) < move_speed ? 1.0f : 1/2.0f;
				_params._vel.y = SPUtil.drpt(cur_vel_y,tar_vel_y,drpt_spd);
			} else {
				_params._vel.y = SPUtil.drpt(_params._vel.y,0,1/30.0f);
			}
			
			if (g._controls.get_control_just_released(ControlManager.Control.Dash)) {
				Vector2 dir = SPUtil.ang_deg_dir(g._player.rotation() + 90);
				if (dir.y < -0.65f && SPUtil.float_random(0,3) < 1) {
					MiscEffects.do_player_bubble(g);
				}
				dir = SPUtil.vec_scale(dir,_params.DASH_SPEED());
				_params._vel = dir;
				_params._dashing = true;
				_params._dash_ct = 20;
				UnderwaterBubbleParticle.proc_multiple_bubbles(g);
			}
		
			_params._player_pos = PlayerCharacterUtil.pos_in_bounds(
				_params._player_pos.x + _params._vel.x * SPUtil.dt_scale_get(),
				_params._player_pos.y
				);
			_params._player_pos.y += _params._vel.y * SPUtil.dt_scale_get();
			
			PlayerCharacterUtil.rotate_to_rotation_for_vel(g._player,_params._vel.x,_params._vel.y,1/10.0f);
			
			_bubble_every.i_update(g);
			if (_bubble_every.do_flash()) {
				_bubble_every._max_time = SPUtil.int_random(0,4) == 0 ? SPUtil.float_random(1,3) : SPUtil.float_random(20,40);
				UnderwaterBubbleParticle.proc_bubble(g);
			}

			if (g._player._u_y < _params._ground_depth + 700) {
				UnderwaterBubbleParticle.proc_multiple_bubbles(g);
				_params._mode = Mode.SwimToUnderwaterTreasure;
			}
		} break;
		default: break;
		}
		
		g._bg_water.set_y_offset(_params._player_pos.y);
		g._bg_village.set_u_pos(0, -_params._player_pos.y);
		g._player.set_u_pos(_params._player_pos.x, 0);
		_enemy_manager.set_offset_y(_params._player_pos.y);
		
		/*
		switch (_params._mode) {
		case (Mode.TransitionIn): {


		} break;
		case (Mode.Gameplay): {

		} break;
		case Mode.SwimToUnderwaterTreasure: {
			g._player.play_anim(PlayerCharacterAnims.SWIM);
			g._camerac.set_target_zoom(850);
			g._camerac.set_target_camera_focus_on_character(g,0,200);

			float cur_vel = _params._vel.magnitude;
			float next_vel = SPUtil.drpt(_params._vel.magnitude,MAX_MOVE_SPEED,1/10.0f);
			_params._vel = SPUtil.vec_scale(_params._vel.normalized,next_vel);

			Vector2 player_pos = SPUtil.vec_lmovto(g._player.get_center(),g._bg_water.get_underwater_treasure_position(),_params._vel.magnitude);

			if (SPUtil.vec_dist(player_pos,g._bg_water.get_underwater_treasure_position()) < 1) {
				_params._mode = Mode.PickupTreasure;
			} else {
				PlayerCharacterUtil.rotate_to_rotation_for_vel(g._player,player_pos.x - g._player.get_center().x,player_pos.y - g._player.get_center().y,1/10.0f);
				g._player.set_center_u_pos(player_pos.x,player_pos.y);
			}
			
		} break;
		case Mode.PickupTreasure: {
			UnderwaterBubbleParticle.proc_multiple_bubbles(g);
			DiveReturnGameState neu_state = DiveReturnGameState.cons(g,_params,_enemy_manager);
			g.pop_top_game_state();
			g.push_game_state(neu_state);
			return;

		} break;
		}
		*/
	}

	public override void debug_draw_hitboxes(SPDebugRender draw) {
		_enemy_manager.debug_draw_hitboxes(draw);
	}

	public override void on_state_end(GameEngineScene g) {}

	public override GameStateIdentifier get_state() { 
		return GameStateIdentifier.Dive; 
	}
}
