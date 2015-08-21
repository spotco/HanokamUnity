using UnityEngine;
using System.Collections.Generic;

public class SPNode : SPBaseBehavior {

	public static T generic_cons<T>() where T : SPNode {
		T rtv = GameMain._context._objpool.depool<T>();
		rtv.transform.parent = GameMain._context.gameObject.transform;
		rtv.__ACTIVE = true;
		rtv.set_name(typeof(T).ToString());
		rtv.i_spnode_cons();
		return rtv;
	}

	public static void generic_repool<T>(T obj) where T : SPNode {
		if (obj._parent != null) obj._parent.remove_child(obj);
		obj._parent = null;
		obj.remove_all_children_with_cleanup();
		obj.__ACTIVE = false;
		GameMain._context._objpool.repool<T>(obj);
	}

	public static SPNode cons() {
		return SPNode.generic_cons<SPNode>();
	}

	[SerializeField] public bool __ACTIVE = false;
	public virtual void repool() {
		SPNode.generic_repool<SPNode>(this);
	}
	
	private SPNode i_spnode_cons() {
		this.transform.localScale = SPUtil.valv(1.0f);
		this._cached_s_pos_dirty = true;
		this._child_sort_z_offset = 0;
		this._manual_set_child_sort_z_offset = false;
		this.set_u_pos(0,0);
		this.set_u_z(0);
		this.set_rotation(0);
		this.set_scale(1);
		return this;
	}

	public SPNode set_name(string name) {
		this.gameObject.name = name;
		return this;
	}

	[SerializeField] private float _rotation;
	[SerializeField] private float _scale_x, _scale_y;
	[SerializeField] private Vector3 _u_pos = new Vector3();
	[SerializeField] private Vector2 _cached_s_pos = new Vector2();
	[SerializeField] private float _child_sort_z_offset = 0;
	[SerializeField] private bool _manual_set_child_sort_z_offset = false;
	[SerializeField] private bool _cached_s_pos_dirty = true;
	
	private void set_u_pos(Vector3 val) {
		_u_pos = val; 
		this.transform.localPosition = new Vector3(_u_pos.x,_u_pos.y,_u_pos.z - _child_sort_z_offset);
		_cached_s_pos_dirty = true;
	}
	
	public float _u_x {
		get { 
			return _u_pos.x; 
		} 
		set { 
			_u_pos.x = value; 
			this.set_u_pos(_u_pos);
		}
	}
	public float _u_y {
		get { 
			return _u_pos.y; 
		} 
		set { 
			_u_pos.y = value; 
			this.set_u_pos(_u_pos);
		}
	}
	public float _u_z {
		get { 
			return _u_pos.z; 
		} 
		set { 
			_u_pos.z = value;
			this.set_u_pos(_u_pos);
		}
	}
	public SPNode set_u_pos(float x, float y) { _u_x = x; _u_y = y;  return this; }
	public SPNode set_u_pos(Vector2 u_pos) { return this.set_u_pos(u_pos.x,u_pos.y); }
	public SPNode set_u_z(float z) { _u_z = z; return this; }
	
	
	public SPNode set_rotation(float deg) { this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,this.transform.localEulerAngles.y, deg); _rotation = deg; return this; }
	public float rotation() { return _rotation; }
	
	public SPNode set_scale_x(float scx) { this.transform.localScale = new Vector3(scx, this.transform.localScale.y, this.transform.localScale.z); _scale_x = scx; return this; }
	public SPNode set_scale_y(float scy) { this.transform.localScale = new Vector3(this.transform.localScale.x, scy, this.transform.localScale.z); _scale_y = scy; return this; }
	public SPNode set_scale(float sc) { this.transform.localScale = new Vector3(sc, sc, this.transform.localScale.z); _scale_x = sc; _scale_y = sc; return this; }
	public float scale_x() { return _scale_x; }
	public float scale_y() { return _scale_y; }
	
	private void update_s_pos() {
		_cached_s_pos = GameMain._context._game_camera.WorldToScreenPoint(this.transform.position);
		_cached_s_pos_dirty = false;
	}
	
	private void s_pos_set(float valx, float valy) {
		if (_cached_s_pos_dirty) update_s_pos();
		Vector3 sw_tar_pos = GameMain._context._game_camera.ScreenToWorldPoint(
			new Vector3(
			valx,
			valy,
			Mathf.Abs(this.transform.position.z - GameMain._context._game_camera.transform.position.z)
			));
		sw_tar_pos.z = this.transform.position.z;
		this.transform.position = sw_tar_pos; //world->local inverse done on assign
		_u_pos = this.transform.localPosition;
		this.set_u_pos(_u_pos);
	}
	
	public float _s_x {
		get {
			if (_cached_s_pos_dirty) update_s_pos();
			return _cached_s_pos.x;
		}
		set {
			if (_cached_s_pos_dirty) update_s_pos();
			s_pos_set(value,_cached_s_pos.y);
		}
	}
	
	public float _s_y {
		get {
			if (_cached_s_pos_dirty) update_s_pos();
			return _cached_s_pos.y;
		}
		set {
			if (_cached_s_pos_dirty) update_s_pos();
			s_pos_set(_cached_s_pos.x,value);
		}
	}
	
	public SPNode set_s_pos(float x, float y) {
		_s_x = x;
		_s_y = y;
		return this;
	}
	
	public SPNode set_s_pos(Vector2 s_pos) { return this.set_s_pos(s_pos.x,s_pos.y); }
	
	[SerializeField] public List<SPNode> _children = new List<SPNode>();
	[SerializeField] public SPNode _parent;
	
	public void add_child(SPNode child) {
		if (child._parent != null) {
			Debug.LogError("Child already has parent");
		}
		child._parent = this;
		child.transform.parent = this.transform;
		child.set_u_pos(child._u_x,child._u_y);
		child.set_scale(child.scale_x());
		child.set_rotation(child.rotation());
		_children.Add(child);
		sort_children();
	}
	
	public void remove_child(SPNode child) {
		bool found = false;
		for (int i = 0; i < _children.Count; i++ ){
			SPNode itr = _children[i];
			if (itr == child) {
				child._parent = null;
				child.transform.parent = null;
				_children.RemoveRange(i,1);
				found = true;
				break;
			}
		}
		if (!found) {
			Debug.LogError("REMOVE_CHILD NOT FOUND");
		}
		sort_children();
	}
	
	public void remove_all_children_with_cleanup() {
		while (_children.Count > 0) {
			SPNode itr = _children[0];
			itr._parent = null;
			itr.transform.parent = null;
			itr.repool();
			_children.RemoveAt(0);
		}
	}
	
	private void sort_children() {
		for (int i = 0; i < _children.Count; i++ ){
			SPNode itr = _children[i];
			if (!itr._manual_set_child_sort_z_offset) itr._child_sort_z_offset = i+1;
			itr.set_u_z(itr._u_z);
		}
	}
	
	public void set_manual_child_sort_z_offset(float val) {
		_manual_set_child_sort_z_offset = true;
		_child_sort_z_offset = val;
	}

}
