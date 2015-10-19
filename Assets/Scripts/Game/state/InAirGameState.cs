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
		g._camerac.set_target_camera_focus_on_character(g,0,200);
		g._camerac.set_target_zoom(GameCameraController.MAX_ZOOM);
		g._camerac.set_zoom_speed(1/20.0f);
		_current_mode = Mode.InitialJumpOut;

		return this;
	}

	public override void i_update(GameEngineScene g) {
		g._player.i_update(g);

		switch (_current_mode) {
		case Mode.InitialJumpOut: {
			PlayerCharacterUtil.move_in_bounds(
				g._player,
				g._player._u_x,
				g._player._u_y + 35 * SPUtil.dt_scale_get()
			);
			g._camerac.set_target_camera_focus_on_character(g,0,200);

		} break;
		case Mode.Combat: {

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
