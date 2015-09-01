using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(SPNode), true)]
#endif
public class SPSprite : SPNode {
	
	public static SPSprite cons_sprite_texkey_texrect(string texkey, Rect texrect) {
		return SPNode.generic_cons<SPSprite>().i_cons_sprite_texkey_texrect(texkey,texrect);
	}

	public override SPNode repool() {
		this.GetComponent<MeshRenderer>().material = null;
		return SPNode.generic_repool<SPSprite>(this);
	}

	[SerializeField] private string _texkey;
	[SerializeField] private Vector4 _color;
	[SerializeField] private MaterialPropertyBlock _material_block;
	[SerializeField] private Rect _texrect;

	//subclasses must call this
	protected SPSprite i_cons_sprite_texkey_texrect(string texkey, Rect texrect) {
		if (this.GetComponent<MeshFilter>() == null) {
			this.gameObject.AddComponent<MeshFilter>().mesh = MeshGen.get_unit_quad_mesh();
			this.gameObject.AddComponent<MeshRenderer>();
		}
		this.set_texkey(texkey);

		this.gameObject.GetComponent<MeshRenderer>().receiveShadows = false;
		this.gameObject.GetComponent<MeshRenderer>().useLightProbes = false;
		this.gameObject.GetComponent<MeshRenderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
		this.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		
		this.set_tex_rect(texrect);
		this.set_anchor_point(0.5f,0.5f);
		this.set_color(new Vector4(1,1,1,1));
		this.set_sort_z(_sort_z);

		return this;
	}

	public string texkey() { return _texkey; }
	public SPSprite set_texkey(string texkey) {
		_texkey = texkey;
		this.set_color(_color);
		return this;
	}

	public override SPNode set_opacity(float val) {
		_color.w = val;
		return this.set_color(_color);
	}

	public override float get_opacity() { return _color.w; }

	public SPSprite set_color(Vector4 color) {
		_color = color;

		/*
		if (_color.w < 1.0) {
			this.gameObject.GetComponent<MeshRenderer>().material = GameMain._context._tex_resc.get_material(_texkey,RSha.ALPHA);
		} else {
			this.gameObject.GetComponent<MeshRenderer>().material = GameMain._context._tex_resc.get_material_default(_texkey);
		}
		*/
		this.gameObject.GetComponent<MeshRenderer>().material = GameMain._context._tex_resc.get_material_default(_texkey);

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

		this.set_anchor_point(_anchorpoint.x,_anchorpoint.y);

		return this;
	}
	
	public override SPNode set_anchor_point(float x, float y) {
		base.set_anchor_point(x,y);
		
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

	public override void set_sort_z(int zt) {
		if (this.GetComponent<MeshRenderer>() != null) this.GetComponent<MeshRenderer>().sortingOrder = zt;
		base.set_sort_z(zt);
	}

	public void manual_set_texture(Texture tex) {
		this.GetComponent<MeshRenderer>().material.SetTexture("_MainTex",tex);
	}
	public void manual_set_mesh_size(float tex_wid, float tex_hei) {
		Mesh sprite_mesh = this.gameObject.GetComponent<MeshFilter>().mesh;
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
	}

}
