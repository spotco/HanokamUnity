using UnityEngine;
using System.Collections;

public class OnePointBasicWaterEnemyComponent : BasicWaterEnemyComponent {
	public static OnePointBasicWaterEnemyComponent cons(Vector2 start) {
		return (new OnePointBasicWaterEnemyComponent()).i_cons(start);
	}
	private Vector2 _pos;
	
	private OnePointBasicWaterEnemyComponent i_cons(Vector2 start) {
		_pos = start;
		return this;
	}
	public override void notify_start_on_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public override void notify_transition_to_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public override void notify_transition_from_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	
	public override void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		if (BasicWaterEnemyComponentUtility.enemy_test_hit(g,state,enemy)) {
			enemy.get_hit_effect().apply_hit(g,state,enemy);
		}
	}
}
