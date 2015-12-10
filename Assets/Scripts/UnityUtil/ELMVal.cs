using UnityEngine;

public struct ELMVec {
	private float _target_vel; //target velocity
	private float _t; //time in current curve
	private float _t_max; //max time of current curve
	private Vector3 _start,_end,_current; //starting, ending and current values
	
	public static ELMVec cons() {
		return new ELMVec();
	}
	
	public ELMVec set_target_vel(float val) { 
		_target_vel = val; 
		this.set_target(this.get_target()); 
		return this; 
	}
	
	public Vector3 get_current() { return _current; }
	public void set_current(Vector3 val) { 
		_current = val; 
		this.set_target(this.get_target());
		
	}
	
	public Vector3 get_target() { return _end; }
	public bool get_finished() { return _t >= _t_max; }
	
	public float get_target_vel() { return _target_vel; }
	public void set_target(Vector3 target) {
		_end = target;
		_start = _current;
		_t = 0;
		_t_max = Mathf.Abs(SPUtil.vec_dist(_start,_end))/_target_vel;
	}
	
	public Vector3 i_update(float dt) {
		_t = Mathf.Clamp(_t+dt,0,_t_max);
		float lerp_t = 1;
		if (_t < _t_max) {
			lerp_t = SPUtil.bezier_val_for_t(
				new Vector2(0,0), new Vector2(0.5f,0), new Vector2(0.5f,1), new Vector2(1,1), _t / _t_max
			).y;
		}
		_current = Vector3.Lerp(_start,_end,lerp_t);
		return _current;
	}
}