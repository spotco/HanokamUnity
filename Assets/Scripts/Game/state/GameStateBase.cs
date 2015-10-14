using UnityEngine;
using System.Collections;

public enum GameStateIdentifier {
	Dive,
	DiveReturn,
	InAir,
	OnGround,
	AirToGroundTransition,
	InDialogue
}

public abstract class GameStateBase : SPGameUpdateable {

	public virtual void i_update(GameEngineScene g) {}
	public virtual GameStateIdentifier get_state() { return GameStateIdentifier.OnGround; }
	public virtual void on_state_end(GameEngineScene g) {}
	public virtual void debug_draw_hitboxes(SPDebugRender draw){}
}

public class IdleGameState : GameStateBase {
	public static IdleGameState cons() { return (new IdleGameState()); }
	public override void i_update(GameEngineScene g) { Debug.LogError("Idle Game State"); }
}
