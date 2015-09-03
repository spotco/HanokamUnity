using UnityEngine;
using System.Collections.Generic;

public class BGVillage : SPGameUpdateable {

	private SPNode _root;
	private SPSprite _docks, _bldg_1, _bldg_2, _bldg_3, _bldg_4;

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
		_bldg_1.set_name("_bldg_1");
		_root.add_child(_bldg_1);

		_docks = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_SPRITESHEET_1,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"pier_top.png")
		);
		_docks.set_manual_sort_z_order(GameAnchorZ.BGVillage_Docks);
		_docks.set_anchor_point(0.5f,0);
		_docks.set_u_pos(0,-115);
		_docks.set_scale(1.75f);
		_docks.set_name("_docks");
		_root.add_child(_docks);

		return this;
	}

	public void i_update(GameEngineScene g) {

	}

}
