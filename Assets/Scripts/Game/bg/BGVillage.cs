using UnityEngine;
using System.Collections.Generic;

public class BGVillage : SPGameUpdateable, CameraRenderHookDelegate {

	private SPNode _root;
	private SPSprite _docks, _docks_front, _bldg_1, _bldg_2, _bldg_3, _bldg_4;

	private SPSprite _reflection_image;
	private Camera _reflection_render_cam;
	private RenderTexture _reflection_tex;

	public static BGVillage cons(GameEngineScene g) {
		return (new BGVillage()).i_cons(g);
	}

	public BGVillage i_cons(GameEngineScene g) {
		_root = SPNode.cons_node();
		_root.set_name("BGVillage");

		_bldg_4 = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_SPRITESHEET_1,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"bg_4.png")
		);
		_bldg_4.set_manual_sort_z_order(GameAnchorZ.BGVillage_BG4);
		_bldg_4.set_u_z(2000f);
		_bldg_4.set_scale(4.0f);
		_bldg_4.set_anchor_point(0.5f,0);
		_bldg_4.set_u_pos(0,500);
		_bldg_4.set_name("_bldg_4");
		_root.add_child(_bldg_4);

		_bldg_3 = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_SPRITESHEET_1,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"bg_3.png")
		);
		_bldg_3.set_manual_sort_z_order(GameAnchorZ.BGVillage_BG3);
		_bldg_3.set_u_z(1200f);
		_bldg_3.set_scale(2.75f);
		_bldg_3.set_anchor_point(0.5f,0);
		_bldg_3.set_name("_bldg_3");
		_bldg_3.gameObject.layer = LayerMask.NameToLayer("ReflectionObjects");
		_root.add_child(_bldg_3);

		_bldg_2 = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_SPRITESHEET_1,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"bg_2.png")
		);
		_bldg_2.set_manual_sort_z_order(GameAnchorZ.BGVillage_BG2);
		_bldg_2.set_u_z(520f);
		_bldg_2.set_scale(2.1f);
		_bldg_2.set_anchor_point(0.5f,0);
		_bldg_2.set_name("_bldg_2");
		//_bldg_2.gameObject.layer = LayerMask.NameToLayer("ReflectionObjects");
		_root.add_child(_bldg_2);

		_bldg_1 = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_SPRITESHEET_1,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"bg_1.png")
		);
		_bldg_1.set_manual_sort_z_order(GameAnchorZ.BGVillage_BG1);
		_bldg_1.set_u_z(215f);
		_bldg_1.set_u_pos(-329,-71);
		_bldg_1.set_scale(1.9f);
		_bldg_1.set_anchor_point(0.5f,0);
		//_bldg_1.gameObject.layer = LayerMask.NameToLayer("ReflectionObjects");
		_bldg_1.set_name("_bldg_1");
		_root.add_child(_bldg_1);

		_docks = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_SPRITESHEET_1,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"pier_top.png")
		);
		_docks.set_manual_sort_z_order(GameAnchorZ.BGVillage_Docks);
		_docks.set_anchor_point(0.5f,0);
		_docks.set_u_pos(0,-104);
		_docks.set_scale(1.75f);
		_docks.set_name("_docks");
		//_docks.gameObject.layer = LayerMask.NameToLayer("ReflectionObjects");
		_root.add_child(_docks);

		_docks_front = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_SPRITESHEET_1,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"pier_top_front_pillars.png")
		);
		_docks_front.set_manual_sort_z_order(GameAnchorZ.BGVillage_Docks_Front);
		_docks_front.set_anchor_point(0.5f,0);
		_docks_front.set_u_pos(0,-104);
		_docks_front.set_scale(1.75f);
		_docks_front.set_name("_docks_front");
		_root.add_child(_docks_front);

		GameObject reflection_render_gameobj = new GameObject("_reflection_render_cam");
		reflection_render_gameobj.transform.parent = _root.transform;
		_reflection_render_cam = reflection_render_gameobj.AddComponent<Camera>();
		_reflection_render_cam.cullingMask = (1 << LayerMask.NameToLayer("ReflectionObjects"));
		_reflection_render_cam.transform.localPosition = new Vector3(0,1252,-929);
		reflection_render_gameobj.AddComponent<CameraRenderHookDispatcher>()._delegate = this;
		
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
		_reflection_image.gameObject.layer = LayerMask.NameToLayer("Reflections");
		_reflection_image.set_manual_sort_z_order(GameAnchorZ.BGVillage_Reflection);
		_reflection_image.set_shader(RSha.SURFACE_REFLECTION);
		_root.add_child(_reflection_image);


		return this;
	}

	public void i_update(GameEngineScene g) {
	}

	private Dictionary<string,float> _pre_reflection_render_positions = new Dictionary<string, float>();
	public void on_pre_render() {

	}
	
	public void on_post_render() {
	}

}
