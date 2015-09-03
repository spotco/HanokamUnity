using UnityEngine;
using System.Collections;

public class BGSky : SPGameUpdateable {

	private SPNode _root;
	private SPSprite _sky_bg;

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
		_root.add_child(_sky_bg);
		return this;
	}
	
	public void i_update(GameEngineScene g) {
		SPHitRect sky_bg_viewbox = g.get_viewbox_dist(_sky_bg._u_z);
		_sky_bg.set_tex_rect(new Rect(0,0,sky_bg_viewbox._x2-sky_bg_viewbox._x1,sky_bg_viewbox._y2-sky_bg_viewbox._y1));
		_sky_bg.set_u_pos(sky_bg_viewbox._x1,sky_bg_viewbox._y1);
	}
}
