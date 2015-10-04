using UnityEngine;
using System.Collections.Generic;

public class BGWater : SPGameUpdateable, CameraRenderHookDelegate {

	private SPNode _root;

	private SPSprite _surf_ele_1, _surf_ele_2, _surf_ele_3;

	private List<SPParallaxScrollSprite> _underwater_scroll_elements;
	
	private SPSprite _top_fade;
	private SPSprite _water_bg;

	private BGWaterGround _lake_bottom_ground;

	private BGWaterLineBelow _waterlinebelow;
	
	public static BGWater cons(GameEngineScene g) {
		return (new BGWater()).i_cons(g);
	}
	
	public BGWater i_cons(GameEngineScene g) {
		_root = SPNode.cons_node();
		_root.set_name("BGWater");

		_water_bg = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_TILE_WATER,
			new Rect(0,0,g.get_viewbox()._x2-g.get_viewbox()._x1,g.get_viewbox()._y2-g.get_viewbox()._y1)
			);
		_water_bg.set_anchor_point(0,0);
		_water_bg.set_u_pos(g.get_viewbox()._x1,g.get_viewbox()._y1);
		_water_bg.set_name("_water_bg");
		_water_bg.set_u_z(3000);
		_water_bg.set_manual_sort_z_order(GameAnchorZ.BGSky_RepeatBG);
		_water_bg.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(_water_bg);

		_surf_ele_3 = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_SPRITESHEET_1,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"bg_underwater_3.png")
		);
		_surf_ele_3.set_manual_sort_z_order(GameAnchorZ.BGWater_SURF_ELE3);
		_surf_ele_3.set_u_z(1088f);
		_surf_ele_3.set_scale(3.0f);
		_surf_ele_3.set_anchor_point(0.5f,1.0f);
		_surf_ele_3.set_u_pos(0,123);
		_surf_ele_3.set_name("_surf_ele_3");
		_surf_ele_3.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(_surf_ele_3);

		_surf_ele_2 = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_SPRITESHEET_1,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"bg_underwater_2.png")
		);
		_surf_ele_2.set_manual_sort_z_order(GameAnchorZ.BGWater_SURF_ELE2);
		_surf_ele_2.set_u_z(495f);
		_surf_ele_2.set_scale(2.3f);
		_surf_ele_2.set_anchor_point(0.5f,1.0f);
		_surf_ele_2.set_u_pos(0,74);
		_surf_ele_2.set_name("_surf_ele_2");
		_surf_ele_2.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(_surf_ele_2);

		_surf_ele_1 = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_SPRITESHEET_1,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"pier_bottom_cliff.png")
		);
		_surf_ele_1.set_manual_sort_z_order(GameAnchorZ.BGWater_SURF_ELE1);
		_surf_ele_1.set_scale(1.75f);
		_surf_ele_1.set_anchor_point(0.5f,1.0f);
		_surf_ele_1.set_u_pos(0,-69);
		_surf_ele_1.set_name("_surf_ele_1");
		_surf_ele_1.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(_surf_ele_1);

		SPParallaxScrollSprite underwater_element_3 = SPParallaxScrollSprite.cons(
			RTex.BG_UNDERWATER_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.BG_UNDERWATER_SPRITESHEET,"underwater_bg_3.png"),
			new Vector2(3.75f,3.75f),
			new Vector3(0,0,1046)
		);
		underwater_element_3._img.set_manual_sort_z_order(GameAnchorZ.BGWater_BG_ELE3);
		underwater_element_3._img.set_name("_underwater_element_3");
		underwater_element_3._img.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(underwater_element_3._img);

		SPParallaxScrollSprite underwater_element_2 = SPParallaxScrollSprite.cons(
			RTex.BG_UNDERWATER_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.BG_UNDERWATER_SPRITESHEET,"underwater_bg_2.png"),
			new Vector2(2.0f,2.0f),
			new Vector3(0,0,491)
		);
		underwater_element_2._img.set_manual_sort_z_order(GameAnchorZ.BGWater_BG_ELE2);
		underwater_element_2._img.set_name("_underwater_element_2");
		underwater_element_2._img.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(underwater_element_2._img);

		SPParallaxScrollSprite underwater_element_1 = SPParallaxScrollSprite.cons(
			RTex.BG_UNDERWATER_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.BG_UNDERWATER_SPRITESHEET,"underwater_bg_1.png"),
			new Vector2(1.65f,1.65f),
			new Vector3(0,0,25)
		);
		underwater_element_1._img.set_manual_sort_z_order(GameAnchorZ.BGWater_BG_ELE1);
		underwater_element_1._img.set_name("_underwater_element_1");
		underwater_element_1._img.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(underwater_element_1._img);

		_underwater_scroll_elements = new List<SPParallaxScrollSprite>() { underwater_element_1, underwater_element_2, underwater_element_3 };

		_top_fade = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_WATER_ELEMENT_FADE,
			new Rect(0,0,SPUtil.get_horiz_world_bounds()._max-SPUtil.get_horiz_world_bounds()._min,512));
		_top_fade.set_name("_top_fade");
		_top_fade.set_manual_sort_z_order(GameAnchorZ.BGWater_TOP_FADE);
		_top_fade.set_u_z(25);
		_top_fade.set_scale_x(1.1f);
		_top_fade.set_scale_y(-2.5f);
		_top_fade.set_u_pos(0,-1350);
		_top_fade.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(_top_fade);

		_surface_gradient = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_UNDERWATER_SURFACE_GRADIENT,
			new Rect(0,0,SPUtil.get_horiz_world_bounds()._max-SPUtil.get_horiz_world_bounds()._min,256));
		_surface_gradient.set_manual_sort_z_order(GameAnchorZ.BGWater_SurfaceGradient);
		_surface_gradient.set_name("_surface_gradient");
		_surface_gradient.set_anchor_point(0.5f,0);
		_surface_gradient.set_scale_y(4f);
		_surface_gradient.set_u_pos(0,-116);
		_surface_gradient.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		
		_root.add_child(_surface_gradient);

		_surface_reflection = BGReflection.cons(_root,"Default")
			.set_name("_surface_reflection")
			.set_scale(4.75f,4.75f)
			.set_camera_pos(0,526,-1040)
			.set_reflection_pos(0,473,0)
			.set_manual_z_order(GameAnchorZ.BGWater_SurfaceReflection)
			.add_camerarender_hook(this)
			.set_alpha_sub(0.25f)
			.set_opacity(0.65f)
			.manual_set_camera_cullingmask(
					int.MaxValue
						& ~(1 << RLayer.get_layer(RLayer.REFLECTIONS))
						& ~(1 << RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS))
						& ~(1 << RLayer.get_layer(RLayer.SPRITER_NODE))
			);

		_waterlinebelow = BGWaterLineBelow.cons(_root);
		_waterlinebelow.set_u_pos(0,-168);
		_waterlinebelow.set_u_z(0);

		_lake_bottom_ground = BGWaterGround.cons(g,_root);
		_lake_bottom_ground.set_u_pos(0,-5000);

		return this;
	}

	private BGVillage _surface_reflection_bgvillage_hook_target = null;
	private SPDict<string,Vector3> __bgvillage_hook_lpos_prev = new SPDict<string, Vector3>();
	private SPDict<string,Vector3> __bgvillage_hook_scale_prev = new SPDict<string, Vector3>();
	private void __bgvillage_hook_prev_record(SPNode node) {
		__bgvillage_hook_lpos_prev[node.gameObject.name] = node.transform.localPosition;
		__bgvillage_hook_scale_prev[node.gameObject.name] = node.transform.localScale;
	}
	private void __bgvillage_hook_prev_set(SPNode node) {
		node.transform.localPosition = __bgvillage_hook_lpos_prev[node.gameObject.name];
		node.transform.localScale = __bgvillage_hook_scale_prev[node.gameObject.name];
	}

	public void on_pre_render() {
		if (_surface_reflection_bgvillage_hook_target == null) return;
		__bgvillage_hook_prev_record(_surface_reflection_bgvillage_hook_target._bldg_3);
		__bgvillage_hook_prev_record(_surface_reflection_bgvillage_hook_target._bldg_2);
		__bgvillage_hook_prev_record(_surface_reflection_bgvillage_hook_target._bldg_1);
		__bgvillage_hook_prev_record(_surface_reflection_bgvillage_hook_target._docks);

		_surface_reflection_bgvillage_hook_target._bldg_3.transform.localPosition = new Vector3(0,-109,0);
		_surface_reflection_bgvillage_hook_target._bldg_3.transform.localScale = SPUtil.valv(1.5f);
		_surface_reflection_bgvillage_hook_target._bldg_2.transform.localPosition = new Vector3(0,-164,0);
		_surface_reflection_bgvillage_hook_target._bldg_2.transform.localScale = SPUtil.valv(1.5f);
		_surface_reflection_bgvillage_hook_target._bldg_1.transform.localPosition = new Vector3(-275,-71,0);
		_surface_reflection_bgvillage_hook_target._bldg_1.transform.localScale = SPUtil.valv(1.5f);
		_surface_reflection_bgvillage_hook_target._docks.transform.localPosition = new Vector3(0,-107,0);
		_surface_reflection_bgvillage_hook_target._docks.transform.localScale = SPUtil.valv(1.5f);
	}
	public void on_post_render() {
		if (_surface_reflection_bgvillage_hook_target == null) return;
		__bgvillage_hook_prev_set(_surface_reflection_bgvillage_hook_target._bldg_3);
		__bgvillage_hook_prev_set(_surface_reflection_bgvillage_hook_target._bldg_2);
		__bgvillage_hook_prev_set(_surface_reflection_bgvillage_hook_target._bldg_1);
		__bgvillage_hook_prev_set(_surface_reflection_bgvillage_hook_target._docks);
	}

	private SPSprite _surface_gradient;
	private BGReflection _surface_reflection;
	
	public void i_update(GameEngineScene g) {
		_surface_reflection_bgvillage_hook_target = g._bg_village;
		this.update_water_bg(g);
		_surf_ele_3.set_enabled(g.get_viewbox_dist(_surf_ele_3.transform.position.z).get_center().y > -90);

		for (int i = 0; i < this._underwater_scroll_elements.Count; i++) {
			SPParallaxScrollSprite itr = this._underwater_scroll_elements[i];
			if (g.is_camera_underwater()) {
				itr.set_enabled(true);
				itr.i_update(g);
			} else {
				itr.set_enabled(false);
			}
		}
	}

	private void update_water_bg(GameEngineScene g) {
		if (g.is_camera_underwater()) {
			_water_bg.set_enabled(true);
			SPHitRect water_bg_viewbox = g.get_viewbox_dist(_water_bg.transform.position.z);
			_water_bg.set_tex_rect(new Rect(0,0,water_bg_viewbox._x2-water_bg_viewbox._x1,water_bg_viewbox._y2-water_bg_viewbox._y1));
			_water_bg.set_u_pos(water_bg_viewbox._x1,water_bg_viewbox._y1);

			_surface_gradient.set_enabled(true);
			_surface_reflection.set_enabled(true);
			_waterlinebelow.set_enabled(true);
			_waterlinebelow.i_update(g);

			_lake_bottom_ground.i_update(g);

		} else {
			_surface_gradient.set_enabled(false);
			_surface_reflection.set_enabled(false);
			_water_bg.set_enabled(false);
			_waterlinebelow.set_enabled(false);
		}
	}
}
