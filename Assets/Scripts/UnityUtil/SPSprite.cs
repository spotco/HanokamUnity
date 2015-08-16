using UnityEngine;
using System.Collections;

public class SPSprite : MonoBehaviour {

	public static SPSprite cons(string texkey, Rect texrect) {
		GameObject neu_obj = (new GameObject("SPSprite"));
		neu_obj.transform.parent = GameMain._context.gameObject.transform;
		return neu_obj.AddComponent<SPSprite>().i_cons(texkey,texrect);
	}

	private string _texkey;
	private SPSprite i_cons(string texkey, Rect texrect) {
		_texkey = texkey;

		this.gameObject.AddComponent<MeshFilter>().mesh = MeshGen.get_unit_quad_mesh(); 
		this.gameObject.AddComponent<MeshRenderer>().material = GameMain._context._tex_resc.get_material_default(texkey);
		this.gameObject.GetComponent<MeshRenderer>().receiveShadows = false;
		this.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

		this.set_tex_rect(texrect);
		this.set_anchor_point(0.5f,0.5f);
		this.set_color(new Vector4(1,1,1,1));
		return this;
	}

	private Vector4 _color;
	private MaterialPropertyBlock _material_block;
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

	private Vector3 _pos = new Vector3();
	public float _x {
		get { return _pos.x; } set { _pos.x = value; this.transform.localPosition = _pos; }
	}
	public float _y {
		get { return _pos.y; } set { _pos.y = value; this.transform.localPosition = _pos; }
	}
	public float _z {
		get { return _pos.z; } set { _pos.z = value; this.transform.localPosition = _pos; }
	}
	public SPSprite set_pos(float x, float y) { _x = x; _y = y;  return this; }

	public SPSprite set_z(float z) { _z = z; this.transform.localPosition = new Vector3(this.transform.localPosition.x,this.transform.localPosition.y,z); return this; }
	public float z() { return _z; }

	public float _rotation;
	public SPSprite set_rotation(float deg) { this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,this.transform.localEulerAngles.y, deg); return this; }
	public float rotation() { return _rotation; }

	public float _scale_x, _scale_y;
	public SPSprite set_scale_x(float scx) { this.transform.localScale = new Vector3(scx, this.transform.localScale.y, this.transform.localScale.z); return this; }
	public SPSprite set_scale_y(float scy) { this.transform.localScale = new Vector3(this.transform.localScale.x, scy, this.transform.localScale.z); return this; }
	public SPSprite set_scale(float sc) { this.transform.localScale = new Vector3(sc, sc, this.transform.localScale.z); return this; }
	public float scale_x() { return _scale_x; }
	public float scale_y() { return _scale_y; }

	private Rect _texrect;
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

	private const float PIXELS_TO_VERT_SCF = 0.0005f;
	private Vector2 _anchorpoint;
	public Vector2 anchorpoint() { return _anchorpoint; }
	public SPSprite set_anchor_point(float x, float y) {
		_anchorpoint.x = x;
		_anchorpoint.y = y;

		Mesh sprite_mesh = this.gameObject.GetComponent<MeshFilter>().mesh;
		Texture sprite_tex = GameMain._context._tex_resc.get_tex(_texkey);
		
		float tex_wid = sprite_tex.width;
		float tex_hei = sprite_tex.height;


		Vector3[] verts = sprite_mesh.vertices;
		verts[0] = new Vector3(
			(-_anchorpoint.x) * tex_wid * PIXELS_TO_VERT_SCF,
			(-_anchorpoint.y) * tex_hei * PIXELS_TO_VERT_SCF
		);
		verts[1] = new Vector3(
			(-_anchorpoint.x + 1) * tex_wid * PIXELS_TO_VERT_SCF,
			(-_anchorpoint.y) * tex_hei * PIXELS_TO_VERT_SCF
		);
		verts[2] = new Vector3(
			(-_anchorpoint.x + 1) * tex_wid * PIXELS_TO_VERT_SCF,
			(-_anchorpoint.y + 1) * tex_hei * PIXELS_TO_VERT_SCF
		);
		verts[3] = new Vector3(
			(-_anchorpoint.x) * tex_wid * PIXELS_TO_VERT_SCF,
			(-_anchorpoint.y + 1) * tex_hei * PIXELS_TO_VERT_SCF
		);
		sprite_mesh.vertices = verts;
		sprite_mesh.RecalculateBounds();

		return this;
	}
}
