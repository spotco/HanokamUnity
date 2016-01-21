using UnityEngine;
using System.Collections.Generic;

public abstract class BasicWaterEnemyComponent {
	public virtual void notify_start_on_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public virtual void notify_transition_to_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public virtual void notify_transition_from_state(GameEngineScene g, BasicWaterEnemy enemy) {}
	public virtual void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {} //if active
	public virtual void i_always_update_pre(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {} //always
	
	public virtual void debug_draw_hitboxes(SPDebugRender draw) {}
}

public abstract class BasicWaterEnemyHitEffect {
	public virtual void apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {}
}

public abstract class BasicWaterEnemy : IWaterEnemy, GenericPooledObject, SPHitPolyOwner {
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
		StunEnded,
		Chase,
		Delayed,
		Activated,
		DoRemove
	}
	private Mode _current_mode;
	public Mode get_current_mode() { return _current_mode; }
	
	private MultiMap<Mode,BasicWaterEnemyComponent> _mode_to_state;
	private BasicWaterEnemyHitEffect _hit_effect;
	public BasicWaterEnemyHitEffect get_hit_effect() { return _hit_effect; }
	
	public struct Params {
		public int _id;
		
		public Vector2 _pos;
		public Vector2 _stun_vel;
		public float _stun_ct, _stun_ct_max;
		
		public float _invuln_ct;
	}
	public Params _params;
	
	protected BasicWaterEnemy i_cons() {
		_current_mode = Mode.Moving;
		_mode_to_state = new MultiMap<Mode, BasicWaterEnemyComponent>();
		return this;
	}
	
	public override void on_added_to_manager(GameEngineScene g) {
		{
			List<BasicWaterEnemyComponent> components = _mode_to_state.list(_current_mode);
			for (int i = 0; i < components.Count; i++) {
				components[i].notify_start_on_state(g,this);
			}
		}
		this.apply_offset_to_position();	
	}
	
	public BasicWaterEnemy add_component_for_mode(Mode mode, BasicWaterEnemyComponent component) { _mode_to_state.add(mode,component); return this; }
	public BasicWaterEnemy remove_all_components_for_mode(Mode mode) { _mode_to_state.clear(mode); return this; }
	public BasicWaterEnemy add_hiteffect(BasicWaterEnemyHitEffect hit_effect) { _hit_effect = hit_effect; return this; }
	
	public void transition_to_mode(GameEngineScene g, Mode mode) {
		{
			List<BasicWaterEnemyComponent> components = _mode_to_state.list(_current_mode);
			for (int i = 0; i < components.Count; i++) {
				components[i].notify_transition_from_state(g,this);
			}
		}
		_current_mode = mode;
		
		{
			List<BasicWaterEnemyComponent> components = _mode_to_state.list(_current_mode);
			for (int i = 0; i < components.Count; i++) {
				components[i].notify_transition_to_state(g,this);
			}
		}
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		List<Mode> modes = _mode_to_state.keys();
		for (int i = 0; i < modes.Count; i++) {
			Mode itr_mode = modes[i];
			{
				List<BasicWaterEnemyComponent> components = _mode_to_state.list(itr_mode);
				for (int j = 0; j < components.Count; j++) {
					components[j].i_always_update_pre(g,state,this);
				}
			}
		}
		
		{
			List<BasicWaterEnemyComponent> components = _mode_to_state.list(_current_mode);
			for (int i = 0; i < components.Count; i++) {
				components[i].i_update(g,state,this);
			}
		}
		
		this.apply_offset_to_position();
		_params._invuln_ct = Mathf.Max(_params._invuln_ct - SPUtil.dt_scale_get(), 0);
		
		this.calculate_velocity(g,state);
	}
	
	public override bool should_remove() {
		return _current_mode == Mode.DoRemove;
	}
	
	private float _env_offset;
	public override void apply_env_offset(float offset) {
		_env_offset = offset;
		this.apply_offset_to_position();
	}
	
	private void apply_offset_to_position() {
		_root.set_u_pos(_params._pos.x, _params._pos.y - _env_offset);
	}
	
	private Vector2 _last_frame_position;
	private Vector2 _last_frame_vel;
	private void calculate_velocity(GameEngineScene g, DiveGameState state) {
		_last_frame_vel.x = (_params._pos.x - _last_frame_position.x)/SPUtil.dt_scale_get();
		_last_frame_vel.y = (_params._pos.y - _last_frame_position.y)/SPUtil.dt_scale_get();
		_last_frame_position = _params._pos;
	}
	
	public Vector2 get_calculated_velocity() {
		return _last_frame_vel;
	}
	
	public override void debug_draw_hitboxes(SPDebugRender draw) {
		draw.draw_hitpoly_owner(this,new Color(0.8f, 0.2f, 0.2f, 0.5f), new Color(0.8f, 0.2f, 0.2f, 0.8f));
		{
			List<BasicWaterEnemyComponent> components = _mode_to_state.list(_current_mode);
			for (int i = 0; i < components.Count; i++) {
				components[i].debug_draw_hitboxes(draw);
			}
		}
	}
	public virtual SPHitRect get_hit_rect() { return new SPHitRect(); }
	public virtual SPHitPoly get_hit_poly() { return new SPHitPoly(); }
}
