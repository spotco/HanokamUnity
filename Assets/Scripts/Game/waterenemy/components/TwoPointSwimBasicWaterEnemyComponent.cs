using UnityEngine;
using System.Collections.Generic;

public class TwoPointSwimBasicWaterEnemyComponent : BasicWaterEnemyComponent {
	public static TwoPointSwimBasicWaterEnemyComponent cons(Vector2 start, Vector2 pt1, Vector2 pt2, float speed) {
		return (new TwoPointSwimBasicWaterEnemyComponent()).i_cons(start,pt1,pt2,speed);
	}
	private Vector2 _start, _pt1, _pt2;
	private float _target_speed;
	
	private Vector2 _current_pos;
	private ELMVec _expected_pos;
	
	private enum Mode {
		Pt1,
		Pt2
	}
	private Mode _current_mode;
	
	private TwoPointSwimBasicWaterEnemyComponent i_cons(Vector2 start, Vector2 pt1, Vector2 pt2, float speed) {
		_start = start;
		_pt1 = pt1;
		_pt2 = pt2;
		_target_speed = speed;
		
		_current_pos = _start;
		
		_expected_pos = new ELMVec();
		_expected_pos.set_current(_start);
		_expected_pos.set_target_vel(_target_speed);
		_expected_pos.set_target(_pt1);
		
		_last_position = _start;
		return this;
	}
	public override void notify_start_on_state(GameEngineScene g, BasicWaterEnemy enemy) {
		enemy._params._pos = _start;
		
		_current_pos = _start;
		_expected_pos.set_current(_start);
		_expected_pos.set_target_vel(_target_speed);
		_expected_pos.set_target(_pt1);
		
		_current_mode = Mode.Pt1;
		_last_position = _start;
	}
	public override void notify_transition_to_state(GameEngineScene g, BasicWaterEnemy enemy) {
		_current_pos = enemy._params._pos;
		_last_position = enemy._params._pos;
	}
	public override void notify_transition_from_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	
	public override void i_always_update_pre(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		if (_expected_pos.get_finished()) {
			if (_current_mode == Mode.Pt1) {
				_expected_pos.set_target(_pt2);
				_current_mode = Mode.Pt2;
			} else if (_current_mode == Mode.Pt2) {
				_expected_pos.set_target(_pt1);
				_current_mode = Mode.Pt1;
			}
		}
		_expected_pos.i_update(SPUtil.dt_scale_get());
	}
	
	private Vector2 _last_position;
	public override void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
	
		_current_pos = SPUtil.vec_lmovto(_current_pos,_expected_pos.get_current(),_target_speed * 1.5f);
		enemy._params._pos = _current_pos;
		
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
}