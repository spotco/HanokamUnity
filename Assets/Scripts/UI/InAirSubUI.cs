using UnityEngine;
using System.Collections;

public class InAirSubUI : GameUISubUI {
	public static InAirSubUI cons(UIRoot ui) {
		return (new InAirSubUI()).i_cons(ui);
	}
	
	private SPNode _root;
	
	private InAirSubUI i_cons(UIRoot ui) {
		_root = SPNode.cons_node();
		_root.set_name("InAirSubUI");
		_root.set_enabled(false);
		
		return this;
	}
	
	public override void add_to_parent(SPNode parent) { parent.add_child(_root); }
	
	public override void on_show() {
		_root.set_enabled(true);
	}
	
	public override void on_hide() {
		_root.set_enabled(false);
	}
	
	public override void i_update(GameEngineScene g) {
		InAirGameState inair_state;
		if (SPUtil.cond_cast<InAirGameState>(g.get_top_game_state(),out inair_state)) {
		}
	}
	
	public override bool should_show() {
		if (base.should_show()) {
			GameEngineScene g = GameMain._context.get_top_scene() as GameEngineScene;
			return g.get_top_game_state().get_state() == GameStateIdentifier.InAir;
		}
		return false;	
	}
}
