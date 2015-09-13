using UnityEngine;
using System.Collections.Generic;

public struct OnGroundGameStateParams {
	public Vector2 _vel;
	public static OnGroundGameStateParams cons() {
		OnGroundGameStateParams rtv = new OnGroundGameStateParams();
		return rtv;
	}
}

public class OnGroundGameState : GameStateBase {

	public static OnGroundGameState cons(GameEngineScene g) {
		return (new OnGroundGameState()).i_cons(g);
	}
	
	public OnGroundGameStateParams _params;
	
	public OnGroundGameState i_cons(GameEngineScene g) {
		_params = OnGroundGameStateParams.cons();
		
		return this;
	}
	
	public override void i_update(GameEngineScene g) {
		if (g._controls.is_move()) {
			_params._vel.x = g._controls.get_move().x * 8.0f;
			if (Mathf.Abs(_params._vel.x) > 7.5f) {
				g._player._img.p_play_anim(PlayerCharacterAnims.RUN);
			} else {
				g._player._img.p_play_anim(PlayerCharacterAnims.WALK);
			}
			if (g._controls.get_move().x > 0) {
				g._player._img.set_scale_x(-1);
				
			} else if (g._controls.get_move().x < 0) {
				g._player._img.set_scale_x(1);
			}
		} else {
			_params._vel.x = SPUtil.drpt(_params._vel.x,0,1/10.0f);
			g._player._img.p_play_anim(PlayerCharacterAnims.IDLE);
		}
		g._player._u_x = g._player._u_x + _params._vel.x * SPUtil.dt_scale_get();
		if (g._player._u_x < SPUtil.get_horiz_world_bounds()._min) {
			g._player._u_x = SPUtil.get_horiz_world_bounds()._min;
		} else if (g._player._u_x > SPUtil.get_horiz_world_bounds()._max) {
			g._player._u_x = SPUtil.get_horiz_world_bounds()._max;
		}
		g._camerac.set_camera_focus_on_character(g);
	}
	
	public override GameStateIdentifier get_state() {
		return GameStateIdentifier.OnGround;
	}
	
	public override void on_state_end(GameEngineScene g) {
		
	}
}
