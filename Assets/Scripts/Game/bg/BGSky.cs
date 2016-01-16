using UnityEngine;
using System.Collections.Generic;

public class BGSky : SPGameUpdateable, SPNodeHierarchyElement {

	private SPNode _root;
	private SPSprite _sky_bg;
	
	private SPParallaxScrollSprite _bg_clouds;
	private SPParallaxScrollSprite _bg_islands_far_back;
	private SPParallaxScrollSprite _bg_islands_far_near;
	private SPParallaxScrollSprite _bg_islands;
	
	private SPParallaxScrollSprite _bg_cliff_left, _bg_cliff_right;
	private DrptVal _bg_cliff_left_x, _bg_cliff_right_x;

	private float _y_offset_in;

	private List<SPParallaxScrollSprite> _scroll_elements;

	public static BGSky cons(GameEngineScene g) {
		return (new BGSky()).i_cons(g);
	}
	
	public void add_to_parent(SPNode parent) {
		parent.add_child(_root);
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
		_sky_bg.set_u_z(5000);
		_sky_bg.set_manual_sort_z_order(GameAnchorZ.BGSky_RepeatBG);
		_sky_bg.gameObject.layer = RLayer.get_layer(RLayer.UNDERWATER_ELEMENTS);
		_root.add_child(_sky_bg);
		
		_bg_clouds = SPParallaxScrollSprite.cons(
			RTex.BG_NSKY1_BGCLOUDS,
			SPUtil.texture_default_rect(RTex.BG_NSKY1_BGCLOUDS),
			new Vector3(6.5f,6.5f),
			new Vector3(0,0,4200)
		);
		_bg_clouds.set_uv_y_size(0.75f);
		_bg_clouds._img.set_manual_sort_z_order(GameAnchorZ.BGSky_BG_ELE3);
		_bg_clouds._img.set_name("_bg_clouds");
		_root.add_child(_bg_clouds._img);
		
		_bg_islands_far_back = SPParallaxScrollSprite.cons(
			RTex.BG_NSKY1_BGISLANDS,
			FileCache.inst().get_texrect(RTex.BG_NSKY1_BGISLANDS,"sky_new_bg_islands_far_2.png"),
			new Vector3(6.2f,6.2f),
			new Vector3(0,0,3600)
			);
		_bg_islands_far_back.set_uv_y_size(0.5f);
		_bg_islands_far_back._img.set_manual_sort_z_order(GameAnchorZ.BGSky_BG_ELE2);
		_bg_islands_far_back._img.set_name("_bg_islands_far_back");
		_root.add_child(_bg_islands_far_back._img);
		
		_bg_islands_far_near = SPParallaxScrollSprite.cons(
			RTex.BG_NSKY1_BGISLANDS,
			FileCache.inst().get_texrect(RTex.BG_NSKY1_BGISLANDS,"sky_new_bg_islands_far_1.png"),
			new Vector3(5.75f,5.75f),
			new Vector3(0,0,2900)
		);
		_bg_islands_far_near.set_uv_y_size(0.45f);
		_bg_islands_far_near._img.set_manual_sort_z_order(GameAnchorZ.BGSky_BG_ELE1);
		_bg_islands_far_near._img.set_name("_bg_islands_near");
		_root.add_child(_bg_islands_far_near._img);
		
		_bg_islands = SPParallaxScrollSprite.cons(
			RTex.BG_NSKY1_BG_OBJS,
			FileCache.inst().get_texrect(RTex.BG_NSKY1_BG_OBJS,"sky_new_bg_islands.png"),
			new Vector3(5f,5f),
			new Vector3(0,0,2100)
		);
		_bg_islands.set_uv_y_size(0.45f);
		_bg_islands._img.set_manual_sort_z_order(GameAnchorZ.BGSky_BG_ELE0);
		_bg_islands._img.set_name("_bg_islands");
		_root.add_child(_bg_islands._img);
		
		_bg_cliff_left = SPParallaxScrollSprite.cons(
			RTex.BG_NSKY1_BG_OBJS,
			FileCache.inst().get_texrect(RTex.BG_NSKY1_BG_OBJS,"sky_new_left.png"),
			new Vector3(1.5f,1.5f),
			new Vector3(-550,0,500)
			);
		_bg_cliff_left._img.set_manual_sort_z_order(GameAnchorZ.BGSky_BG_SideCliffs);
		_bg_cliff_left._img.set_name("_bg_cliff_left");
		_root.add_child(_bg_cliff_left._img);
		
		_bg_cliff_right = SPParallaxScrollSprite.cons(
			RTex.BG_NSKY1_BG_OBJS,
			FileCache.inst().get_texrect(RTex.BG_NSKY1_BG_OBJS,"sky_new_right.png"),
			new Vector3(1.5f,1.5f),
			new Vector3(550,0,500)
			);
		_bg_cliff_right._img.set_manual_sort_z_order(GameAnchorZ.BGSky_BG_SideCliffs);
		_bg_cliff_right._img.set_name("_bg_cliff_right");
		_root.add_child(_bg_cliff_right._img);
		
		_bg_cliff_left_x = new DrptVal() {
			_current = _bg_cliff_left._img._u_x,
			_target = _bg_cliff_left._img._u_x,
			_drptval = 1/10.0f
		};
		_bg_cliff_right_x = new DrptVal() {
			_current = _bg_cliff_right._img._u_x,
			_target = _bg_cliff_right._img._u_x,
			_drptval = 1/10.0f
		};
		
		_scroll_elements = new List<SPParallaxScrollSprite>() {
			_bg_clouds,
			_bg_islands_far_back,
			_bg_islands_far_near,
			_bg_islands,
			_bg_cliff_left,
			_bg_cliff_right
		};
		
		_y_offset_in = 0;
		
		return this;
	}

	public void i_update(GameEngineScene g) {
		this.update_sky_bg(g);
	}
	
	private float _test = 0;
	private void update_sky_bg(GameEngineScene g) {
		if (this.is_sky_visible(g)) {
			_root.set_enabled(true);
			SPHitRect sky_bg_viewbox = g.get_viewbox_dist(_sky_bg.transform.position.z);
			_sky_bg.set_tex_rect(new Rect(0,0,sky_bg_viewbox._x2-sky_bg_viewbox._x1,sky_bg_viewbox._y2-sky_bg_viewbox._y1+2000));
			_sky_bg.set_u_pos(sky_bg_viewbox._x1,sky_bg_viewbox._y1-1000);
			
		} else {
			_root.set_enabled(false);
			return;
		}
		
		_test += 0.4f * SPUtil.dt_scale_get();
		_bg_clouds.set_x_offset(_test);
		
		if (_y_offset_in > 1750) {
			_bg_cliff_left_x._target = -790;
			_bg_cliff_right_x._target = 905;
		} else {
			_bg_cliff_left_x._target = -1900;
			_bg_cliff_right_x._target = 1900;
		}
		_bg_cliff_left_x.i_update();
		_bg_cliff_right_x.i_update();
		_bg_cliff_left._img._u_x = _bg_cliff_left_x._current;
		_bg_cliff_right._img._u_x = _bg_cliff_right_x._current;
		
		for (int i = 0; i < _scroll_elements.Count; i++) {
			SPParallaxScrollSprite itr = _scroll_elements[i];
			itr.i_update(g);
		}
	}
	
	private bool is_sky_visible(GameEngineScene g) {
		GameStateIdentifier cur_state = g.get_top_game_state().get_state();
		return cur_state == GameStateIdentifier.InAir;
	}
	
	public void set_y_offset(float val) {
		_y_offset_in = val;
		for (int i = 0; i < _scroll_elements.Count; i++) {
			SPParallaxScrollSprite itr = _scroll_elements[i];
			itr.set_y_offset(val);	
		}
	}
	public float get_y_offset() {
		return _y_offset_in;
	}

}
