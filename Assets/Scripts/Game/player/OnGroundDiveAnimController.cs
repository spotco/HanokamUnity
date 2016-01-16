using UnityEngine;
using System.Collections;

public class OnGroundDiveAnimController : OnGroundStateUpdateable {

	public static OnGroundDiveAnimController cons() { return (new OnGroundDiveAnimController()); }
	
	private enum Mode {
		RUN_FORWARD_PREPARE,
		RUN_FORWARD,
		JUMP_LOG1,
		DO_TRANSITION
	}
	private Mode _current_mode;
	private float _delay_ct;
	private float _anim_t;
	private Vector3 _anim_initial_position;
	private Vector3 _player_position;
	
	public void setup_dive_anim(GameEngineScene g, OnGroundGameState state) {
		g._player.set_layer(RLayer.DEFAULT);
		g._player.set_manual_sort_z_order(GameAnchorZ.Player_Ground_InFrontOf_Docks_Front);
	
		_current_mode = Mode.RUN_FORWARD_PREPARE;
		g._player.play_anim(PlayerCharacterAnims.DIVE_RUN_FORWARD_PREPARE,false);
		_delay_ct = 0;
		
		_player_position = new Vector3(0,-16,0);
		this.apply_player_position(g);
	}
	
	public void finish_dive_anim(GameEngineScene g, OnGroundGameState state) {
		g._player.set_u_z(0);
	}
	
	private void apply_player_position(GameEngineScene g) {
		g._player.set_u_pos(_player_position.x,_player_position.y);
		g._player.set_u_z(_player_position.z);
	}
	
	public void i_update(GameEngineScene g, OnGroundGameState state) {
		switch (_current_mode) {
		case Mode.RUN_FORWARD_PREPARE: {
			g._camerac.set_target_zoom(600);
			g._camerac.set_target_camera_focus_on_character(g,0,70);
			
			if (g._player.is_anim_finished()) {
				_delay_ct+=SPUtil.dt_scale_get();
				if (_delay_ct > 15) {
					g._player.play_anim(PlayerCharacterAnims.DIVE_RUN_FORWARD);
					_current_mode = Mode.RUN_FORWARD;
					_anim_t = 0;
					_anim_initial_position = _player_position;
				}
			}
			
		} break;
		case Mode.RUN_FORWARD: {
			g._camerac.set_target_zoom(800);
			g._camerac.set_target_camera_focus_on_character(g,0,180);
		
			Vector3 tar_pt = SPUtil.vec_add(g._bg_village.jump_log3_position(),new Vector3(0,35,0));
			Vector3 ctrl_pt_1 = SPUtil.vec_add(new Vector3(_anim_initial_position.x,tar_pt.y,(tar_pt.z-_anim_initial_position.z)*0.5f+_anim_initial_position.z),new Vector3(0,0,0));
			
			float prev_anim_t = _anim_t;
			_anim_t = Mathf.Clamp(_anim_t+SPUtil.sec_to_tick(1f) * SPUtil.dt_scale_get(),0,1);
			_player_position = SPUtil.bezier_val_for_t(_anim_initial_position,ctrl_pt_1,ctrl_pt_1,tar_pt,_anim_t);
			
			if (prev_anim_t < 0.6f && _anim_t >= 0.6f) {
				g._player.play_anim(PlayerCharacterAnims.DIVE_FLIP_AND_JUMP,false);
			}
			if (_anim_t >= 1) {
				_current_mode = Mode.JUMP_LOG1;
				_anim_initial_position = tar_pt;
				_anim_t = 0;
				g._bg_village._jump_logs[0].apply_offset();
			}
			
		} break;
		case Mode.JUMP_LOG1: {			
			Vector3 tar_pt = SPUtil.vec_add(g._bg_village.jump_log3_position(),new Vector3(0,-100,-20));
			Vector3 ctrl_pt_1 = new Vector3(
				_anim_initial_position.x,
				_anim_initial_position.y + 400,
				(tar_pt.z-_anim_initial_position.z)*0.5f+_anim_initial_position.z);
			
			float prev_anim_t = _anim_t;
			_anim_t = Mathf.Clamp(_anim_t+SPUtil.sec_to_tick(1.25f) * SPUtil.dt_scale_get(),0,1);
			_player_position = SPUtil.bezier_val_for_t(_anim_initial_position,ctrl_pt_1,ctrl_pt_1,tar_pt,_anim_t);
			
			if (_anim_t < 0.65f) {
				g._camerac.set_target_zoom(1000);
				g._camerac.set_target_camera_focus_on_character(g,0,140);
			} else {
				g._camerac.set_target_zoom(1000);
				g._camerac.set_target_camera_focus_on_character(g,0,-50);
			}
			
			if (_anim_t < 0.5f) {
				if (g._player.is_anim_finished()) {
					g._player.play_anim(PlayerCharacterAnims.DIVE_FORWARD_SPIN);
				}
			} else {
				if (prev_anim_t < 0.5f) {
					g._player.play_anim(PlayerCharacterAnims.DIVE_TRANSITION,false);
				} else if (g._player.is_anim_finished()) {
					g._player.play_anim(PlayerCharacterAnims.DIVE_FALL);
				}
			}
			
			if (_anim_t >= 1f) {
				_current_mode = Mode.DO_TRANSITION;
			}
			
		} break;
		case Mode.DO_TRANSITION: {} break;
		}
		this.apply_player_position(g);
	}
	
	public bool is_finished_and_should_transition_to_divestate() {
		return _current_mode == Mode.DO_TRANSITION;
	}
	
}
