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

	private InAirGameState i_cons(GameEngineScene g) {
		return this;
	}

	public override void i_update(GameEngineScene g) {
		Debug.LogError("InAirGameState update");
	}

	public override GameStateIdentifier get_state() { 
		return GameStateIdentifier.InAir;
	}

}
