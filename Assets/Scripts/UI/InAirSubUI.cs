using UnityEngine;
using System.Collections;

public class InAirSubUI : GameUISubUI {
	public static InAirSubUI cons(UIRoot ui) {
		return (new InAirSubUI()).i_cons(ui);
	}
	
	private SPNode _root;
	private InAirSubUI_EnemyWarningUI _enemy_warnings;
	
	private InAirSubUI i_cons(UIRoot ui) {
		_root = SPNode.cons_node();
		_root.set_name("InAirSubUI");
		_root.set_enabled(false);
		
		_enemy_warnings = InAirSubUI_EnemyWarningUI.cons(ui);
		_enemy_warnings.add_to_parent(_root);
		
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
		if (!SPUtil.cond_cast<InAirGameState>(g.get_top_game_state(),out inair_state)) return;
		
		_enemy_warnings.i_update(g, inair_state);
	}
	
	public override bool should_show() {
		if (base.should_show()) {
			GameEngineScene g = GameMain._context.get_top_scene() as GameEngineScene;
			return g.get_top_game_state().get_state() == GameStateIdentifier.InAir;
		}
		return false;	
	}
}

public class InAirSubUI_EnemyWarningUI : InAirGameStateUpdateable, SPNodeHierarchyElement {
	public static InAirSubUI_EnemyWarningUI cons(UIRoot ui) {
		return (new InAirSubUI_EnemyWarningUI()).i_cons(ui);
	}
	
	private SPNode _root;
	private SPParticleSystem<EnemyWarningUIParticle> _active_warnings;
	private InAirSubUI_EnemyWarningUI i_cons(UIRoot ui) {
		_root = SPNode.cons_node();
		_root.set_name("_enemy_warning_ui");
		
		_active_warnings = SPParticleSystem<EnemyWarningUIParticle>.cons();
		
		return this;
	}
	public void add_warning(EnemyWarningUIParticle warning) {
		_active_warnings.add_particle(warning);
		warning.add_to_parent(_root);
	}
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }

	public void i_update(GameEngineScene g, InAirGameState state) {
		_active_warnings.i_update(state);
	}
}

public class EnemyWarningUIParticle : SPParticle, SPNodeHierarchyElement {

	public void add_to_parent(SPNode parent) { /*parent.add_child(_root);*/ }

	public void i_update(System.Object context) {
	
	}
	public bool should_remove(System.Object context) {
		return true;
	}
	public void do_remove(System.Object context) {
	
	}
}
