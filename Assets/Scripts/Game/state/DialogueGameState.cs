using UnityEngine;
using System.Collections;

public class DialogueGameState : GameStateBase {
	
	public static DialogueGameState cons(GameEngineScene g, Villager target) {
		return (new DialogueGameState()).i_cons(g,target);
	}
	
	private Villager _target;
	
	private DialogueGameState i_cons(GameEngineScene g, Villager target) {
		_target = target;
		_target.set_mode(Villager.Mode.Chatting);
		
		return this;
	}
	
	public override void i_update(GameEngineScene g) {
		g._camerac.set_target_zoom(500);
		g._camerac.set_target_camera_focus_on_character(g,0,70);
		if (g._controls.get_control_just_released(ControlManager.Control.Chat)) {
			g.pop_top_game_state();
		}
		
		g._player.i_update(g);
		_target.i_update(g);
		
		
	}
	public override GameStateIdentifier get_state() { return GameStateIdentifier.InDialogue; }
	public override void on_state_end(GameEngineScene g) {
		_target.set_mode(Villager.Mode.Idle);
	}

}
