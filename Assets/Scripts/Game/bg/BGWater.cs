using UnityEngine;
using System.Collections;

public class BGWater : SPGameUpdateable {

	private SPNode _root;

	public SPSprite _surf_ele_1, _surf_ele_2, _surf_ele_3;
	
	public static BGWater cons(GameEngineScene g) {
		return (new BGWater()).i_cons(g);
	}
	
	public BGWater i_cons(GameEngineScene g) {
		_root = SPNode.cons_node();
		_root.set_name("BGWater");

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
		_root.add_child(_surf_ele_1);

		return this;
	}
	
	public void i_update(GameEngineScene g) {

	}
}
