using UnityEngine;
using System.Collections.Generic;

public class BGSky : SPGameUpdateable {

	private SPNode _root;
	private SPSprite _sky_bg;

	private SPParallaxScrollSprite _bg_arcs, _bg_islands;

	private SPParallaxScrollSprite _bg_cliff_left, _bg_cliff_right;
	private ELMVal _bg_cliff_left_x = ELMVal.cons();
	private ELMVal _bg_cliff_right_x = ELMVal.cons();

	private List<SPParallaxScrollSprite> _scroll_elements;

	public static BGSky cons(GameEngineScene g) {
		return (new BGSky()).i_cons(g);
	}
	
	public BGSky i_cons(GameEngineScene g) {
		_root = SPNode.cons_node();
		_root.set_name("BGSky");

		_sky_bg = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_TILE_SKY,
			new Rect(0,0,g.get_viewbox()._x2-g.get_viewbox()._x1,g.get_viewbox()._y2-g.get_viewbox()._y1)
		);
		_sky_bg.set_anchor_point(0,0);
		_sky_bg.set_u_pos(g.get_viewbox()._x1,g.get_viewbox()._y1);
		_sky_bg.set_name("_sky_bg");
		_sky_bg.set_u_z(3000);
		_sky_bg.set_manual_sort_z_order(GameAnchorZ.BGSky_RepeatBG);
		_sky_bg.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(_sky_bg);

		_bg_arcs = SPParallaxScrollSprite.cons(
			RTex.BG_SKY_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.BG_SKY_SPRITESHEET,"bg_sky_arcs.png"),
			new Vector3(4.5f,4.5f),
			new Vector3(0,0,2800)
		);
		_bg_arcs._img.set_manual_sort_z_order(GameAnchorZ.BGSky_BG_ELE2);
		_bg_arcs._img.set_name("_bg_arcs");
		_root.add_child(_bg_arcs._img);

		_bg_islands = SPParallaxScrollSprite.cons(
			RTex.BG_SKY_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.BG_SKY_SPRITESHEET,"bg_sky_island.png"),
			new Vector3(3.49f,3.49f),
			new Vector3(0,0,2085)
			);
		_bg_islands._img.set_manual_sort_z_order(GameAnchorZ.BGSky_BG_ELE1);
		_bg_islands._img.set_name("_bg_islands");
		_root.add_child(_bg_islands._img);

		_bg_cliff_left = SPParallaxScrollSprite.cons(
			RTex.BG_SKY_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.BG_SKY_SPRITESHEET,"bg_sky_cliffs_left.png"),
			new Vector3(1.5f,1.5f),
			new Vector3(-750,0,0)
		);
		_bg_cliff_left._img.set_manual_sort_z_order(GameAnchorZ.BGSky_BG_SideCliffs);
		_bg_cliff_left._img.set_name("_bg_cliff_left");
		_bg_cliff_left_x.set_current(_bg_cliff_left._img._u_x);
		_bg_cliff_left_x.set_target_vel(25.0f);
		_root.add_child(_bg_cliff_left._img);

		_bg_cliff_right = SPParallaxScrollSprite.cons(
			RTex.BG_SKY_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.BG_SKY_SPRITESHEET,"bg_sky_cliffs_right.png"),
			new Vector3(1.5f,1.5f),
			new Vector3(750,0,0)
		);
		_bg_cliff_right._img.set_manual_sort_z_order(GameAnchorZ.BGSky_BG_SideCliffs);
		_bg_cliff_right._img.set_name("_bg_cliff_right");
		_bg_cliff_right_x.set_current(_bg_cliff_right._img._u_x);
		_bg_cliff_right_x.set_target_vel(25.0f);
		_root.add_child(_bg_cliff_right._img);

		_scroll_elements = new List<SPParallaxScrollSprite>() { _bg_islands, _bg_arcs, _bg_cliff_left, _bg_cliff_right };

		return this;
	}

	public void i_update(GameEngineScene g) {
		this.update_sky_bg(g);
	}

	private void update_sky_bg(GameEngineScene g) {
		if (!g.is_camera_underwater()) {
			_sky_bg.set_enabled(true);
			SPHitRect sky_bg_viewbox = g.get_viewbox_dist(_sky_bg.transform.position.z);
			_sky_bg.set_tex_rect(new Rect(0,0,sky_bg_viewbox._x2-sky_bg_viewbox._x1,sky_bg_viewbox._y2-sky_bg_viewbox._y1));
			_sky_bg.set_u_pos(sky_bg_viewbox._x1,sky_bg_viewbox._y1);

		} else {
			_sky_bg.set_enabled(false);
		}

		if (GameMain._context._game_camera.transform.localPosition.y > 1500) {
			_bg_cliff_left_x.set_target(-370);
			_bg_cliff_right_x.set_target(280);
		} else {
			_bg_cliff_left_x.set_target(-800);
			_bg_cliff_right_x.set_target(700);
		}
		_bg_cliff_left_x.i_update(SPUtil.dt_scale_get());
		_bg_cliff_right_x.i_update(SPUtil.dt_scale_get());
		_bg_cliff_left._img._u_x = _bg_cliff_left_x.get_current();
		_bg_cliff_right._img._u_x = _bg_cliff_right_x.get_current();

		for (int i = 0; i < _scroll_elements.Count; i++) {
			SPParallaxScrollSprite itr = _scroll_elements[i];
			if (!g.is_camera_underwater()) {
				itr.i_update(g);
				itr.set_enabled(true);
			} else {
				itr.set_enabled(false);
			}
		}

	}

}
