using UnityEngine;
using System.Collections;

public class DiveGameState : GameStateBase {

	public struct DiveGameStateParams {
		public Vector2 _vel;
	}

	public enum State {
		TransitionIn,
		Gameplay,
		SwimToUnderwaterTreasure,
		PickupUnderwaterTreasure
	}

	public static DiveGameState cons(GameEngineScene g, Vector2 initial_velocity) {
		return (new DiveGameState()).i_cons(g, initial_velocity);
	}

	public DiveGameStateParams _params;

	public DiveGameState i_cons(GameEngineScene g, Vector2 initial_velocity) {
		_params._vel = initial_velocity;
		return this;
	}

	public override void i_update(GameEngineScene g) {
		g._player.set_manual_sort_z_order(GameAnchorZ.Player_UnderWater);

		g._player.set_u_pos(
			g._player._u_x + _params._vel.x * SPUtil.dt_scale_get(),
			g._player._u_y + _params._vel.y * SPUtil.dt_scale_get()
		);
	}

	public override GameStateIdentifier get_state() { 
		return GameStateIdentifier.Dive; 
	}

}
