using UnityEngine;
using System.Collections;

public abstract class BasicWaterEnemyComponent {
	public virtual void notify_start_on_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public virtual void notify_transition_to_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public virtual void notify_transition_from_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public virtual void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {}
}

public abstract class BasicWaterEnemyHitEffect {
	public virtual void apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy, BasicWaterEnemyComponent current_component) {}
}

public abstract class BasicWaterEnemy : IWaterEnemy, GenericPooledObject {
	private SPNode _root;
	public SPNode get_root() { return _root; }
	public override Vector2 get_u_pos() { return _root.get_u_pos(); }
	public virtual BasicWaterEnemy behaviour_set_rotation(float deg) { _root.set_rotation(deg); return this; }
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
		
		Activated,
		
		DoRemove
	}
	private Mode _current_mode;
	public Mode get_current_mode() { return _current_mode; }
	private SPDict<Mode,BasicWaterEnemyComponent> _mode_to_state;
	private BasicWaterEnemyHitEffect _hit_effect;
	public BasicWaterEnemyHitEffect get_hit_effect() { return _hit_effect; }
	
	public struct Params {
		public int _id;
		
		public Vector2 _pos;
		public float _offset;
		public Vector2 _stun_vel;
		public float _stun_ct, _stun_ct_max;
		
		public float _invuln_ct;
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
	
	public BasicWaterEnemy add_hiteffect(BasicWaterEnemyHitEffect hit_effect) {
		_hit_effect = hit_effect;
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
		_params._invuln_ct = Mathf.Max(_params._invuln_ct - SPUtil.dt_scale_get(), 0);
	}
	
	public override bool should_remove() {
		return _current_mode == Mode.DoRemove;
	}
	
	public override void apply_offset(float offset) {
		_params._offset = offset;
		this.apply_offset_to_position();
	}
	
	private void apply_offset_to_position() {
		_root.set_u_pos(_params._pos.x, _params._pos.y - _params._offset);
	}
}
