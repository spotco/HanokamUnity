using UnityEngine;
using System.Collections;

public class BGReflection {

	public static BGReflection cons(SPNode parent, string culling_layer) {
		return (new BGReflection()).i_cons(parent, culling_layer);
	}

	private SPNode _root;
	private SPSprite _reflection_image;
	private Camera _reflection_render_cam;
	private RenderTexture _reflection_tex;

	public BGReflection i_cons(SPNode parent, string culling_layer) {

		_root = SPNode.cons_node();
		parent.add_child(_root);

		GameObject reflection_render_gameobj = new GameObject("_reflection_render_cam");
		reflection_render_gameobj.transform.parent = _root.transform;
		_reflection_render_cam = reflection_render_gameobj.AddComponent<Camera>();
		_reflection_render_cam.cullingMask = (1 << RLayer.get_layer(culling_layer));
		_reflection_render_cam.transform.localPosition = new Vector3(0,1252,-929);
		
		_reflection_tex = new RenderTexture(256,256,16,RenderTextureFormat.ARGB32);
		_reflection_tex.Create();
		_reflection_render_cam.targetTexture = _reflection_tex;
		
		_reflection_image = SPSprite.cons_sprite_texkey_texrect(RTex.BLANK,new Rect(0,0,1,1));
		_reflection_image.set_anchor_point(0.5f,0.5f);
		_reflection_image.manual_set_texture(_reflection_tex);
		_reflection_image.manual_set_mesh_size(256,256);
		_reflection_image.set_name("_reflection_image");
		_reflection_image.set_scale_x(9.75f);
		_reflection_image.set_scale_y(-9.75f);
		_reflection_image.set_u_pos(0,-1059);
		_reflection_image.set_u_z(1046.0f);
		_reflection_image.gameObject.layer = RLayer.get_layer(RLayer.REFLECTIONS);
		_reflection_image.set_manual_sort_z_order(GameAnchorZ.BGVillage_Reflection_3);
		_reflection_image.set_shader(RShader.SURFACE_REFLECTION);
		_root.add_child(_reflection_image);

		return this;
	}

	public BGReflection set_name(string name) {
		_root.gameObject.name = name;
		return this;
	}

	public BGReflection set_scale(float val) {
		_reflection_image.set_scale_x(val);
		_reflection_image.set_scale_y(-val);
		return this;
	}
	public BGReflection set_scale(float valx, float valy) {
		_reflection_image.set_scale_x(valx);
		_reflection_image.set_scale_y(valy);
		return this;
	}


	public BGReflection set_reflection_pos(float x, float y, float z) {
		_reflection_image.set_u_pos(x,y);
		_reflection_image.set_u_z(z);
		return this;
	}

	public BGReflection set_manual_z_order(int val) {
		_reflection_image.set_manual_sort_z_order(val);
		return this;
	}

	public BGReflection set_camera_pos(float x, float y, float z) {
		_reflection_render_cam.transform.localPosition = new Vector3(x,y,z);
		return this;
	}

	public BGReflection set_alpha_sub(float val) {
		_reflection_image.gameObject.GetComponent<MeshRenderer>().material.SetFloat("_alpha_sub",val);
		return this;
	}

	public BGReflection set_enabled(bool val) {
		_root.set_enabled(val);
		return this;
	}

	public BGReflection manual_set_camera_cullingmask(int val) {
		_reflection_render_cam.cullingMask = val;
		return this;
	}

	public BGReflection add_camerarender_hook(CameraRenderHookDelegate hook) {
		if (_reflection_render_cam.gameObject.GetComponent<CameraRenderHookDispatcher>() == null) _reflection_render_cam.gameObject.AddComponent<CameraRenderHookDispatcher>();
		_reflection_render_cam.gameObject.GetComponent<CameraRenderHookDispatcher>()._delegate = hook;
		return this;
	}

	public BGReflection set_opacity(float val) {
		_reflection_image.set_opacity(val);
		return this;
	}

}
