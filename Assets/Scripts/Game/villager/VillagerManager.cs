using UnityEngine;
using System.Collections.Generic;

public class VillagerManager : OnGroundStateUpdateable {

	public static VillagerManager cons(GameEngineScene g) {
		return (new VillagerManager()).i_cons(g);
	}
	
	private List<Villager> _villagers;
	private List<VillageObject> _objects;
	
	private VillagerManager i_cons(GameEngineScene g) {
		//TODO -- should be generated here
		_villagers = g._bg_village.get_villagers();		
		
		VillageObject tmp = VillageObject.cons(g);
		tmp.set_u_pos(-200,308);
		_objects = new List<VillageObject>() {
			tmp
		};
		
		return this;
	}
	
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
				if (itr.is_in_chat_range_with_player(g)) {
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
	}
	
}
