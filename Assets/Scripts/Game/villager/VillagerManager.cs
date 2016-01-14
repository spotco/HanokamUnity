using UnityEngine;
using System.Collections.Generic;

public class VillagerManager : OnGroundStateUpdateable, SPNodeHierarchyElement {

	public static VillagerManager cons(GameEngineScene g) {
		return (new VillagerManager()).i_cons(g);
	}
	
	private SPNode _root;
	private List<Villager> _villagers;
	private List<VillageObject> _objects;
	
	private VillagerManager i_cons(GameEngineScene g) {
		_root = SPNode.cons_node();
		_root.set_name("VillagerManager");
	
		//TODO -- should be generated here
		//TODO -- recycle the head top icons
		_villagers = g._bg_village.get_villagers();		
		
		VillageObject tmp = VillageObject.cons(g);
		tmp.set_u_pos(-485,308);
		tmp.set_u_z(150);
		_objects = new List<VillageObject>() {
			tmp
		};
		tmp.add_to_parent(_root);
		
		return this;
	}
	
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	
	public void i_update(GameEngineScene g, OnGroundGameState state) {
		for (int i = 0; i < _villagers.Count; i++) {
			_villagers[i].i_update(g);
		}
		for (int i = 0; i < _objects.Count; i++) {
			_objects[i].i_update(g, state);
			
		}
		
		if (g._controls.get_control_just_released(ControlManager.Control.Chat)) {
			for (int i = 0; i < _villagers.Count; i++) {
				Villager itr = _villagers[i];
				if (itr._can_chat && itr.is_in_chat_range_with_player(g)) {
					state.notify_chat_with_villager(g, itr);
					break;
				}
			}
			for (int i = 0; i < _objects.Count; i++) {
				VillageObject itr = _objects[i];
				if (itr.is_in_interact_range_with_player(g)) {
					state.notify_enter_shop(g);
					break;
				}
			}
		}
	}
	
	public void repool() {
		for (int i = 0; i < _objects.Count; i++) {
			_objects[i].repool();
		}
		_objects.Clear();
		_objects = null;
		_villagers = null;
		_root.repool();
		_root = null;
	}
	
}
