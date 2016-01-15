using UnityEngine;
using System.Collections;

public class OnGroundDiveAnimController : OnGroundStateUpdateable {

	public static OnGroundDiveAnimController cons() { return (new OnGroundDiveAnimController()); }
	
	private enum Mode {
		RUN_FORWARD_PREPARE,
		RUN_FORWARD,
		
		DIVE_FLIP_AND_JUMP,
		DIVE_FORWARD_SPIN,
		DIVE_TRANSITION,
		DIVE_FALL
	}
	private Mode _current_mode;
	
	public void setup_dive_anim(GameEngineScene g, OnGroundGameState state) {
		_current_mode = Mode.RUN_FORWARD_PREPARE;
		g._player.play_anim(PlayerCharacterAnims.DIVE_RUN_FORWARD_PREPARE,false);
	}
	
	public void i_update(GameEngineScene g, OnGroundGameState state) {
		switch (_current_mode) {
		case Mode.RUN_FORWARD_PREPARE: {
			if (g._player.is_anim_finished()) {
				g._player.play_anim(PlayerCharacterAnims.DIVE_RUN_FORWARD,false);
				_current_mode = Mode.RUN_FORWARD;
			}
			
		} break;
		case Mode.RUN_FORWARD: {
			if (g._player.is_anim_finished()) {
				g._player.play_anim(PlayerCharacterAnims.DIVE_FLIP_AND_JUMP,false);
				_current_mode = Mode.DIVE_FLIP_AND_JUMP;
			}
			
		} break;
		
		case Mode.DIVE_FLIP_AND_JUMP: {
			if (g._player.is_anim_finished()) {
				g._player.play_anim(PlayerCharacterAnims.DIVE_FORWARD_SPIN,false);
				_current_mode = Mode.DIVE_FORWARD_SPIN;
			}
		} break;
		case Mode.DIVE_FORWARD_SPIN: {
			if (g._player.is_anim_finished()) {
				g._player.play_anim(PlayerCharacterAnims.DIVE_TRANSITION,false);
				_current_mode = Mode.DIVE_TRANSITION;
			}
		} break;
		case Mode.DIVE_TRANSITION: {
			if (g._player.is_anim_finished()) {
				g._player.play_anim(PlayerCharacterAnims.DIVE_FALL,false);
				_current_mode = Mode.DIVE_FALL;
			}
		} break;
		case Mode.DIVE_FALL: {
			if (g._player.is_anim_finished()) {
				g._player.play_anim(PlayerCharacterAnims.DIVE_RUN_FORWARD_PREPARE,false);
				_current_mode = Mode.RUN_FORWARD_PREPARE;
			}
		} break;
		
		}
	}
	
	public bool is_finished_and_should_transition_to_divestate() {
		return false;
	}
	
}
