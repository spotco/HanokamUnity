using UnityEngine;
using System.Collections.Generic;

public class SPSprite : SPBaseBehavior {

	[SerializeField] private bool __ACTIVE = false;

	public static SPSprite cons(string texkey, Rect texrect) {
		SPSprite rtv = GameMain._context._objpool.depool<SPSprite>();
		rtv.transform.parent = GameMain._context.gameObject.transform;
		rtv.__ACTIVE = true;
		return rtv.i_cons(texkey,texrect);
	}

	public void repool() {
		this._parent = null;
		this.GetComponent<MeshRenderer>().material = null;
		this.remove_all_children_with_cleanup();
		this.__ACTIVE = false;
		GameMain._context._objpool.repool<SPSprite>(this);
	}

	public void set_name(string name) {
		this.gameObject.name = name;
	}

	[SerializeField] private string _texkey;
	[SerializeField] private Vector4 _color;
	[SerializeField] private MaterialPropertyBlock _material_block;
	[SerializeField] private float _rotation;
	[SerializeField] private float _scale_x, _scale_y;
	[SerializeField] private Rect _texrect;
	[SerializeField] private Vector2 _anchorpoint;

	private SPSprite i_cons(string texkey, Rect texrect) {
		_texkey = texkey;
		this.set_name("SPSprite");
		this.transform.localScale = SPUtil.valv(1.0f);

		if (this.GetComponent<MeshFilter>() == null) {
			this.gameObject.AddComponent<MeshFilter>().mesh = MeshGen.get_unit_quad_mesh();
			this.gameObject.AddComponent<MeshRenderer>();
		}

		this.gameObject.GetComponent<MeshRenderer>().material = GameMain._context._tex_resc.get_material_default(texkey);
		this.gameObject.GetComponent<MeshRenderer>().receiveShadows = false;
		this.gameObject.GetComponent<MeshRenderer>().useLightProbes = false;
		this.gameObject.GetComponent<MeshRenderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

		this.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

		this._cached_s_pos_dirty = true;
		this._child_sort_z_offset = 0;
		this._manual_set_child_sort_z_offset = false;
		this.set_u_pos(0,0);
		this.set_u_z(0);
		this.set_rotation(0);
		this.set_scale(1);
		
		this.set_tex_rect(texrect);
		this.set_anchor_point(0.5f,0.5f);
		this.set_color(new Vector4(1,1,1,1));
		return this;
	}

	public SPSprite set_color(Vector4 color) {
		_color = color;

		if (_color.w < 1.0) {
			this.gameObject.GetComponent<MeshRenderer>().material = GameMain._context._tex_resc.get_material(_texkey,RSha.ALPHA);
		} else {
			this.gameObject.GetComponent<MeshRenderer>().material = GameMain._context._tex_resc.get_material_default(_texkey);
		}

		MeshRenderer renderer = this.GetComponent<MeshRenderer>();
		if (_material_block == null) {
			_material_block = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(_material_block);
		}
		_material_block.Clear();
		_material_block.AddColor("_Color", _color);
		renderer.SetPropertyBlock(_material_block);
		return this;
	}

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
	public SPSprite set_u_pos(float x, float y) { _u_x = x; _u_y = y;  return this; }
	public SPSprite set_u_pos(Vector2 u_pos) { return this.set_u_pos(u_pos.x,u_pos.y); }
	public SPSprite set_u_z(float z) { _u_z = z; return this; }


	public SPSprite set_rotation(float deg) { this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,this.transform.localEulerAngles.y, deg); _rotation = deg; return this; }
	public float rotation() { return _rotation; }
	
	public SPSprite set_scale_x(float scx) { this.transform.localScale = new Vector3(scx, this.transform.localScale.y, this.transform.localScale.z); _scale_x = scx; return this; }
	public SPSprite set_scale_y(float scy) { this.transform.localScale = new Vector3(this.transform.localScale.x, scy, this.transform.localScale.z); _scale_y = scy; return this; }
	public SPSprite set_scale(float sc) { this.transform.localScale = new Vector3(sc, sc, this.transform.localScale.z); _scale_x = sc; _scale_y = sc; return this; }
	public float scale_x() { return _scale_x; }
	public float scale_y() { return _scale_y; }
	
	public Rect texrect() { return _texrect; }
	public SPSprite set_tex_rect(Rect texrect) {
		_texrect = texrect;

		Mesh sprite_mesh = this.gameObject.GetComponent<MeshFilter>().mesh;
		Texture sprite_tex = GameMain._context._tex_resc.get_tex(_texkey);
		
		float tex_wid = sprite_tex.width;
		float tex_hei = sprite_tex.height;
		float x1 = texrect.x;
		float y1 = tex_hei-texrect.height - texrect.y;
		float x2 = texrect.x + texrect.width;
		float y2 = tex_hei-texrect.y;
		
		Vector2[] uvs = sprite_mesh.uv;
		uvs[0] = new Vector2(x1/tex_wid,y1/tex_hei); //(0,0)
		uvs[1] = new Vector2(x2/tex_wid,y1/tex_hei); //(1,0)
		uvs[2] = new Vector2(x2/tex_wid,y2/tex_hei); //(1,1)
		uvs[3] = new Vector2(x1/tex_wid,y2/tex_hei); //(0,1)
		sprite_mesh.uv = uvs;
		return this;
	}

	public Vector2 anchorpoint() { return _anchorpoint; }
	public SPSprite set_anchor_point(float x, float y) {
		_anchorpoint.x = x;
		_anchorpoint.y = y;

		Mesh sprite_mesh = this.gameObject.GetComponent<MeshFilter>().mesh;
		
		float tex_wid = _texrect.width;
		float tex_hei = _texrect.height;


		Vector3[] verts = sprite_mesh.vertices;
		verts[0] = new Vector3(
			(-_anchorpoint.x) * tex_wid,
			(-_anchorpoint.y) * tex_hei
		);
		verts[1] = new Vector3(
			(-_anchorpoint.x + 1) * tex_wid,
			(-_anchorpoint.y) * tex_hei
		);
		verts[2] = new Vector3(
			(-_anchorpoint.x + 1) * tex_wid,
			(-_anchorpoint.y + 1) * tex_hei
		);
		verts[3] = new Vector3(
			(-_anchorpoint.x) * tex_wid,
			(-_anchorpoint.y + 1) * tex_hei
		);
		sprite_mesh.vertices = verts;
		sprite_mesh.RecalculateBounds();

		return this;
	}

	private Vector2 s_pos_of_vertex(int i) {
		MeshFilter mesh = this.GetComponent<MeshFilter>();
		return GameMain._context._game_camera.WorldToScreenPoint(this.transform.TransformPoint(mesh.sharedMesh.vertices[i]));
	}

	private Vector2 u_pos_of_vertex(int i) {
		MeshFilter mesh = this.GetComponent<MeshFilter>();
		return GameMain._context.gameObject.transform.InverseTransformPoint(this.transform.TransformPoint(mesh.sharedMesh.vertices[i]));
	}

	public Rect s_bounding_box() {
		float min_x = float.PositiveInfinity;
		float min_y = float.PositiveInfinity;
		float max_x = float.NegativeInfinity;
		float max_y = float.NegativeInfinity;
		for (int i = 0; i < 4; i++) {
			Vector2 vtx_s_pos = s_pos_of_vertex(i);
			min_x = Mathf.Min(min_x,vtx_s_pos.x);
			min_y = Mathf.Min(min_y,vtx_s_pos.y);
			max_x = Mathf.Max(max_x,vtx_s_pos.x);
			max_y = Mathf.Max(max_y,vtx_s_pos.y);
		}
		return new Rect(new Vector2(min_x,min_y),new Vector2(max_x-min_x,max_y-min_y));
	}

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

	public SPSprite set_s_pos(float x, float y) {
		_s_x = x;
		_s_y = y;
		return this;
	}

	public SPSprite set_s_pos(Vector2 s_pos) { return this.set_s_pos(s_pos.x,s_pos.y); }

	[SerializeField] public List<SPSprite> _children = new List<SPSprite>();
	[SerializeField] public SPSprite _parent;

	public void add_child(SPSprite child) {
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
	
	public void remove_child(SPSprite child) {
		for (int i = 0; i < _children.Count; i++ ){
			SPSprite itr = _children[i];
			if (itr == child) {
				child._parent = null;
				child.transform.parent = null;
				_children.RemoveRange(i,1);
				return;
			}
		}
		sort_children();
	}

	public void remove_all_children_with_cleanup() {
		while (_children.Count > 0) {
			SPSprite itr = _children[0];
			itr._parent = null;
			itr.transform.parent = null;
			itr.repool();
			_children.RemoveAt(0);
		}
	}

	private void sort_children() {
		for (int i = 0; i < _children.Count; i++ ){
			SPSprite itr = _children[i];
			if (!itr._manual_set_child_sort_z_offset) itr._child_sort_z_offset = i+1;
			itr.set_u_z(itr._u_z);
		}
	}

	public void set_manual_child_sort_z_offset(float val) {
		_manual_set_child_sort_z_offset = true;
		_child_sort_z_offset = val;
	}


	public override void Update() {}
}
