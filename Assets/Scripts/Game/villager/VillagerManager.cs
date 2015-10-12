using UnityEngine;
using System.Collections.Generic;

public class VillagerManager : OnGroundStateUpdateable {

	public static VillagerManager cons(GameEngineScene g) {
		return (new VillagerManager()).i_cons(g);
	}
	
	private List<Villager> _villagers;
	
	private VillagerManager i_cons(GameEngineScene g) {
		//TODO -- should be generated here
		_villagers = g._bg_village.get_villagers();		
		
		return this;
	}
	
	public void i_update(GameEngineScene g, OnGroundGameState state) {
		for (int i = 0; i < _villagers.Count; i++) {
			_villagers[i].i_update(g);
		}
		
		if (g._controls.get_control_just_released(ControlManager.Control.Chat)) {
			for (int i = 0; i < _villagers.Count; i++) {
				if (_villagers[i].is_in_chat_range_with_player(g)) {
					state.notify_chat_with_villager(_villagers[i]);
					break;
				}
			}
		}
	
	}
	
}
