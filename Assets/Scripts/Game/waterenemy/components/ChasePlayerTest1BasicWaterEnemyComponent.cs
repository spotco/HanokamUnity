using UnityEngine;
using System.Collections;

public class ChasePlayerTest1BasicWaterEnemyComponent : BasicWaterEnemyComponent {
	
	public static ChasePlayerTest1BasicWaterEnemyComponent cons() {
		return (new ChasePlayerTest1BasicWaterEnemyComponent()).i_cons();
	}
	
	private Vector2 _vel;
	private Vector2 _last_position;
	
	private float _close_speed;
	private float _far_speed;
	
	private float _rand_delta_theta_x,_rand_delta_theta_y;
	private float _rand_delta_speed_x,_rand_delta_speed_y;
	private float _rand_turn_speed;
	
	private ChasePlayerTest1BasicWaterEnemyComponent i_cons() {
		_vel = Vector2.zero;
		_close_speed = SPUtil.float_random(3f,6.5f);
		_far_speed = SPUtil.float_random(17,30);
		_rand_delta_theta_x = SPUtil.float_random(-3.14f,3.14f);
		_rand_delta_theta_y = SPUtil.float_random(-3.14f,3.14f);
		_rand_delta_speed_x = SPUtil.float_random(0.02f,0.06f);
		_rand_delta_speed_y = SPUtil.float_random(0.02f,0.06f);
		_rand_turn_speed = SPUtil.float_random(1/35.0f,1/20.0f);
		return this;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		Vector2 divestate_player_center = SPUtil.vec_add(state._params._player_pos,g._player.get_center_offset());
		
		divestate_player_center.x += 400*Mathf.Sin(_rand_delta_theta_x);
		divestate_player_center.y += 400*Mathf.Sin(_rand_delta_speed_y);
		
		_rand_delta_theta_x += _rand_delta_speed_x*SPUtil.dt_scale_get();
		_rand_delta_theta_y += _rand_delta_speed_y*SPUtil.dt_scale_get();
		
		float player_dist = Vector2.Distance(enemy._params._pos,divestate_player_center);
		Vector2 player_dir = SPUtil.vec_sub(divestate_player_center,enemy._params._pos).normalized;
		float max_speed = player_dist > 1000 ? _far_speed : _close_speed;
		
		Vector2 tar_vel = SPUtil.vec_scale(player_dir,max_speed);
		_vel.x = SPUtil.drpt(_vel.x,tar_vel.x,_rand_turn_speed);
		_vel.y = SPUtil.drpt(_vel.y,tar_vel.y,_rand_turn_speed);
		
		enemy._params._pos = SPUtil.vec_add(enemy._params._pos,SPUtil.vec_scale(_vel,SPUtil.dt_scale_get()));
		
		Vector2 delta = SPUtil.vec_sub(enemy._params._pos,_last_position);
		if (delta.magnitude > 0) {
			float tar_rotation = SPUtil.dir_ang_deg(delta.x,delta.y) - 90;
			enemy.behaviour_set_rotation(
				SPUtil.drpt(enemy.get_root().rotation(), enemy.get_root().rotation() + SPUtil.shortest_angle(enemy.get_root().rotation(),tar_rotation), 1/10.0f));
		}
		_last_position = enemy._params._pos;
		
		if (BasicWaterEnemyComponentUtility.enemy_test_hit(g,state,enemy)) {
			enemy.get_hit_effect().apply_hit(g,state,enemy);
		}
		
	}
	public override void notify_transition_to_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public override void notify_transition_from_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	
	

}
