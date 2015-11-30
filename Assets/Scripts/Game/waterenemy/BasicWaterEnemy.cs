using UnityEngine;
using System.Collections;

public abstract class BasicWaterEnemyComponent {
	public virtual void notify_start_on_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public virtual void notify_transition_to_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public virtual void notify_transition_from_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public virtual void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {}
}

public abstract class BasicWaterEnemy : IWaterEnemy, GenericPooledObject {
	private SPNode _root;
	public SPNode get_root() { return _root; }
	public override Vector2 get_u_pos() { return _root.get_u_pos(); }
	public override void add_to_parent(SPNode parent) { parent.add_child(_root); }
	
	private static int __allocid = 0;
	public virtual void depool() {
		_root = SPNode.cons_node();
		_root.set_name("BasicWaterEnemy");
		_params._id = __allocid++;
	}
	public virtual void repool() {
		_root.repool();
		_root = null;
	}
	
	public enum Mode {
		Moving,
		Stunned,
		Noticed,
		AfterNotice,
		DoRemove
	}
	private Mode _current_mode;
	public Mode get_current_mode() { return _current_mode; }
	private SPDict<Mode,BasicWaterEnemyComponent> _mode_to_state;
	
	public struct Params {
		public int _id;
		
		public Vector2 _pos;
		public float _offset;
		public Vector2 _stun_vel;
		public float _stun_ct, _stun_ct_max;
	}
	public Params _params;
	
	protected BasicWaterEnemy i_cons() {
		_current_mode = Mode.Moving;
		_mode_to_state = new SPDict<Mode, BasicWaterEnemyComponent>();
		return this;
	}
	
	public override void on_added_to_manager(GameEngineScene g) {
		if (_mode_to_state.ContainsKey(_current_mode)) _mode_to_state[_current_mode].notify_start_on_state(g, this);
		this.apply_offset_to_position();	
	}
	
	public BasicWaterEnemy add_component_for_mode(Mode mode, BasicWaterEnemyComponent state) {
		_mode_to_state[mode] = state;
		return this;
	}
	
	public void transition_to_mode(GameEngineScene g, Mode mode) {
		if (_mode_to_state.ContainsKey(_current_mode)) _mode_to_state[_current_mode].notify_transition_from_state(g, this);
		_current_mode = mode;
		if (_mode_to_state.ContainsKey(_current_mode)) _mode_to_state[_current_mode].notify_transition_to_state(g, this);
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		if (_mode_to_state.ContainsKey(_current_mode)) {
			_mode_to_state[_current_mode].i_update(g,state,this);
		}
		this.apply_offset_to_position();
	}
	
	public override void apply_offset(float offset) {
		_params._offset = offset;
		this.apply_offset_to_position();
	}
	
	private void apply_offset_to_position() {
		_root.set_u_pos(_params._pos.x, _params._pos.y - _params._offset);
	}
}

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
		enemy._params._pos.x = Mathf.Clamp(enemy._params._pos.x, SPUtil.get_horiz_world_bounds()._min, SPUtil.get_horiz_world_bounds()._max);
		
		enemy._params._stun_vel.x = SPUtil.drpt(enemy._params._stun_vel.x,0,1/20.0f);
		enemy._params._stun_vel.y = SPUtil.drpt(enemy._params._stun_vel.y,0,1/20.0f);
		
		enemy._params._stun_ct -= SPUtil.dt_scale_get();
		if (enemy._params._stun_ct <= 0) {
			enemy.transition_to_mode(g, BasicWaterEnemy.Mode.Moving);
		} else {
			BasicWaterEnemyComponentUtility.test_and_apply_player_hit_stun(g,state,enemy);
		}
	}
}

public class TwoPointSwimBasicWaterEnemyComponent : BasicWaterEnemyComponent {
	public static TwoPointSwimBasicWaterEnemyComponent cons(Vector2 start, Vector2 pt1, Vector2 pt2, float speed) {
		return (new TwoPointSwimBasicWaterEnemyComponent()).i_cons(start,pt1,pt2,speed);
	}
	private Vector2 _start, _pt1, _pt2;
	private float _target_speed;
	
	private ELMVec _cur_pos;
	
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
		
		_cur_pos = new ELMVec();
		_cur_pos.set_current(_start);
		_cur_pos.set_target_vel(_target_speed);
		_cur_pos.set_target(_pt1);
		_last_position = _start;
		return this;
	}
	public override void notify_start_on_state(GameEngineScene g, BasicWaterEnemy enemy) {
		enemy._params._pos = _start;
		_cur_pos.set_current(_start);
		_cur_pos.set_target_vel(_target_speed);
		_cur_pos.set_target(_pt1);
		_current_mode = Mode.Pt1;
		_last_position = _start;
	}
	public override void notify_transition_to_state(GameEngineScene g, BasicWaterEnemy enemy) {
		_cur_pos.set_current(enemy._params._pos);
		_last_position = enemy._params._pos;
	}
	public override void notify_transition_from_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	
	private Vector2 _last_position;
	public override void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		enemy._params._pos = _cur_pos.i_update(SPUtil.dt_scale_get());
		if (_cur_pos.get_finished()) {
			if (_current_mode == Mode.Pt1) {
				_cur_pos.set_target(_pt2);
				_current_mode = Mode.Pt2;
			} else if (_current_mode == Mode.Pt2) {
				_cur_pos.set_target(_pt1);
				_current_mode = Mode.Pt1;
			}
		}
		
		Vector2 delta = SPUtil.vec_sub(enemy._params._pos,_last_position);
		float tar_rotation = SPUtil.dir_ang_deg(delta.x,delta.y) - 90;
		enemy.get_root().set_rotation(SPUtil.drpt(enemy.get_root().rotation(), enemy.get_root().rotation() + SPUtil.shortest_angle(enemy.get_root().rotation(),tar_rotation), 1/10.0f)); 
		_last_position = enemy._params._pos;
		
		BasicWaterEnemyComponentUtility.test_and_apply_player_hit_stun(g,state,enemy);
	}
}

public class BasicWaterEnemyComponentUtility {
	public static bool test_and_apply_player_hit_stun(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		bool rtv = SPHitPoly.polyowners_intersect(enemy, g._player);
		if (rtv) {
			Vector2 pos_delta = (new Vector2(enemy.get_u_pos().x-g._player.get_center().x, enemy.get_u_pos().y-g._player.get_center().y)).normalized;
			if (SPUtil.flt_cmp_delta(pos_delta.magnitude,0,0.01f)) {
				pos_delta = new Vector2(1,0);
			}
			enemy._params._stun_vel = SPUtil.vec_scale(pos_delta,20);
			enemy._params._stun_ct = enemy._params._stun_ct_max = 50;
			enemy.transition_to_mode(g, BasicWaterEnemy.Mode.Stunned);
		}
		return rtv;
	}
}
