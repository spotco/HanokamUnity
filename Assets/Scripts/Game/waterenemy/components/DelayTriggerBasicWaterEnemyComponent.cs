using UnityEngine;
using System.Collections;

public class DelayTriggerBasicWaterEnemyComponent : BasicWaterEnemyComponent {

	public interface Delegate {
		void delay_trigger(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy);
	}
	
	public static DelayTriggerBasicWaterEnemyComponent cons(DelayTriggerBasicWaterEnemyComponent.Delegate delegate_obj, float delay) {
		return (new DelayTriggerBasicWaterEnemyComponent()).i_cons(delegate_obj, delay);
	}
	
	private float _delay_ct,_delay_ct_max;
	private DelayTriggerBasicWaterEnemyComponent.Delegate _delegate_obj;
	private DelayTriggerBasicWaterEnemyComponent i_cons(DelayTriggerBasicWaterEnemyComponent.Delegate delegate_obj, float delay) {
		_delay_ct_max = delay;
		_delegate_obj = delegate_obj;
		return this;
	}
	
	public override void notify_transition_to_state(GameEngineScene g, BasicWaterEnemy enemy) {
		_delay_ct = _delay_ct_max;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		float delay_ct_prev = _delay_ct;
		_delay_ct -= SPUtil.dt_scale_get();
		if (delay_ct_prev > 0 && _delay_ct <= 0) {
			_delegate_obj.delay_trigger(g,state,enemy);
		}
		
		if (BasicWaterEnemyComponentUtility.enemy_test_hit(g,state,enemy)) {
			enemy.get_hit_effect().apply_hit(g,state,enemy);
		}
	}	
}

public class ImmTriggerBasicWaterEnemyComponent : BasicWaterEnemyComponent {
	
	public interface Delegate {
		void imm_trigger(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy);
	}
	
	public static ImmTriggerBasicWaterEnemyComponent cons(ImmTriggerBasicWaterEnemyComponent.Delegate delegate_obj) {
		return (new ImmTriggerBasicWaterEnemyComponent()).i_cons(delegate_obj);
	}
	
	private ImmTriggerBasicWaterEnemyComponent.Delegate _delegate_obj;
	private ImmTriggerBasicWaterEnemyComponent i_cons(ImmTriggerBasicWaterEnemyComponent.Delegate delegate_obj) {
		_delegate_obj = delegate_obj;
		return this;
	}
	
	public override void notify_transition_to_state(GameEngineScene g, BasicWaterEnemy enemy) {
		_delegate_obj.imm_trigger(g,(DiveGameState)g.get_top_game_state(),enemy);
	}
}