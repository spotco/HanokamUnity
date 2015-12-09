using UnityEngine;
using System.Collections.Generic;

public class KnockbackStunBasicWaterEnemyComponent : BasicWaterEnemyComponent {
	public static KnockbackStunBasicWaterEnemyComponent cons() {
		return (new KnockbackStunBasicWaterEnemyComponent()).i_cons();
	}
	public KnockbackStunBasicWaterEnemyComponent i_cons() { return this; }
	public override void notify_transition_to_state(GameEngineScene g, BasicWaterEnemy enemy) {
		_last_move_rotation = enemy.get_root().rotation();
	}
	public override void notify_transition_from_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	
	private float _anim_theta;
	private float _last_move_rotation;
	public override void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		_anim_theta = (_anim_theta + SPUtil.dt_scale_get() / (6.28f)) % 6.28f;
		enemy.get_root().set_rotation(
			_last_move_rotation
			+ SPUtil.lerp(30, 5, 1-enemy._params._stun_ct/enemy._params._stun_ct_max)
			* Mathf.Sin(_anim_theta * SPUtil.lerp(1,7,1-enemy._params._stun_ct/enemy._params._stun_ct_max))
		);
		
		enemy._params._pos = SPUtil.vec_add(enemy._params._pos, SPUtil.vec_scale(enemy._params._stun_vel,SPUtil.dt_scale_get()));
		
		enemy._params._stun_vel.x = SPUtil.drpt(enemy._params._stun_vel.x,0,1/20.0f);
		enemy._params._stun_vel.y = SPUtil.drpt(enemy._params._stun_vel.y,0,1/20.0f);
		
		if (BasicWaterEnemyComponentUtility.enemy_test_hit(g,state,enemy)) {
			enemy.get_hit_effect().apply_hit(g,state,enemy,this);
		}
		
		enemy._params._stun_ct -= SPUtil.dt_scale_get();
		if (enemy._params._stun_ct <= 0) {
			enemy.transition_to_mode(g, BasicWaterEnemy.Mode.Moving);
		}
	}
}