using UnityEngine;
using System.Collections;

public class BGWater : SPGameUpdateable {

	private SPNode _root;

	private SPSprite _surf_ele_1, _surf_ele_2, _surf_ele_3;
	private SPSprite _underwater_element_1, _underwater_element_2, _underwater_element_3;
	private SPSprite _top_fade;
	private SPSprite _water_bg;
	
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

		_underwater_element_3 = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_UNDERWATER_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.BG_UNDERWATER_SPRITESHEET,"underwater_bg_3.png"));
		_underwater_element_3.set_name("_underwater_element_3");
		_underwater_element_3.set_manual_sort_z_order(GameAnchorZ.BGWater_BG_ELE3);
		_underwater_element_3.set_u_z(1046);
		_underwater_element_3.set_scale(3.75f);
		_underwater_element_3.set_u_pos(0,-3400);
		_underwater_element_3.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(_underwater_element_3);

		_underwater_element_2 = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_UNDERWATER_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.BG_UNDERWATER_SPRITESHEET,"underwater_bg_2.png"));
		_underwater_element_2.set_name("_underwater_element_2");
		_underwater_element_2.set_manual_sort_z_order(GameAnchorZ.BGWater_BG_ELE2);
		_underwater_element_2.set_u_z(491);
		_underwater_element_2.set_scale(2.0f);
		_underwater_element_2.set_u_pos(0,-2400);
		_underwater_element_2.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(_underwater_element_2);

		_underwater_element_1 = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_UNDERWATER_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.BG_UNDERWATER_SPRITESHEET,"underwater_bg_1.png"));
		_underwater_element_1.set_name("_underwater_element_1");
		_underwater_element_1.set_manual_sort_z_order(GameAnchorZ.BGWater_BG_ELE1);
		_underwater_element_1.set_u_z(25);
		_underwater_element_1.set_scale(1.65f);
		_underwater_element_1.set_u_pos(0,-2400);
		_underwater_element_1.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(_underwater_element_1);

		_top_fade = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_WATER_ELEMENT_FADE,
			new Rect(0,0,SPUtil.get_horiz_world_bounds()._max-SPUtil.get_horiz_world_bounds()._min,512));
		_top_fade.set_name("_top_fade");
		_top_fade.set_manual_sort_z_order(GameAnchorZ.BGWater_TOP_FADE);
		_top_fade.set_u_z(25);
		_top_fade.set_scale_x(1);
		_top_fade.set_scale_y(-2.5f);
		_top_fade.set_u_pos(0,-1350);
		_top_fade.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(_top_fade);

		_surface_gradient = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_UNDERWATER_SURFACE_GRADIENT,
			new Rect(0,0,SPUtil.get_horiz_world_bounds()._max-SPUtil.get_horiz_world_bounds()._min,256));
		_surface_gradient.set_manual_sort_z_order(GameAnchorZ.BGWater_SurfaceGradient);
		_surface_gradient.set_name("_surface_gradient");
		_surface_gradient.set_opacity(0.75f);
		_surface_gradient.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		
		_root.add_child(_surface_gradient);

		_surface_reflection = BGReflection.cons(_root,"Default")
			.set_name("_surface_reflection")
			.set_scale(4.75f,4.75f)
			.set_camera_pos(0,255,-1214)
			.set_reflection_pos(0,219,0)
			.set_manual_z_order(GameAnchorZ.BGWater_SurfaceReflection)
			.manual_set_camera_cullingmask(
					int.MaxValue 
						& ~(1 << RLayer.get_layer(RLayer.REFLECTION_OBJECTS_3))
						& ~(1 << RLayer.get_layer(RLayer.REFLECTIONS))
						& ~(1 << RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS)));

		return this;
	}

	private SPSprite _surface_gradient;
	private BGReflection _surface_reflection;
	
	public void i_update(GameEngineScene g) {
		this.update_water_bg(g);
		_surf_ele_3.set_enabled(g.get_viewbox_dist(_surf_ele_3.transform.position.z).get_center().y > -90);

		_underwater_element_3.set_enabled(g.is_camera_underwater());
		_underwater_element_2.set_enabled(g.is_camera_underwater());
		_underwater_element_1.set_enabled(g.is_camera_underwater());

	}

	private void update_water_bg(GameEngineScene g) {
		if (g.is_camera_underwater()) {
			_water_bg.set_enabled(true);
			SPHitRect water_bg_viewbox = g.get_viewbox_dist(_water_bg.transform.position.z);
			_water_bg.set_tex_rect(new Rect(0,0,water_bg_viewbox._x2-water_bg_viewbox._x1,water_bg_viewbox._y2-water_bg_viewbox._y1));
			_water_bg.set_u_pos(water_bg_viewbox._x1,water_bg_viewbox._y1);
		} else {
			_water_bg.set_enabled(false);
		}
	}
}
