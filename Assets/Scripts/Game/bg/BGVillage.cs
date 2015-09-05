using UnityEngine;
using System.Collections.Generic;

public class BGVillage : SPGameUpdateable {

	private SPNode _root;
	private SPSprite _docks, _docks_front, _bldg_1, _bldg_2, _bldg_3, _bldg_4;

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
		_bldg_3.gameObject.layer = RLayer.get_layer(RLayer.REFLECTION_OBJECTS_3);
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
		_bldg_2.gameObject.layer = RLayer.get_layer(RLayer.REFLECTION_OBJECTS_2);
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
		_bldg_1.gameObject.layer = RLayer.get_layer(RLayer.REFLECTION_OBJECTS_1);
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
		_docks.gameObject.layer = RLayer.get_layer(RLayer.REFLECTION_OBJECTS_DOCKS);
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


		BGReflection.cons(_root,RLayer.REFLECTION_OBJECTS_3)
			.set_name("_bg_3_reflection")
			.set_alpha_sub(0.3f)
			.set_manual_z_order(GameAnchorZ.BGVillage_Reflection_3);

		BGReflection.cons(_root,RLayer.REFLECTION_OBJECTS_2)
			.set_name("_bg_2_reflection")
			.set_reflection_pos(0,-767,458)
			.set_camera_pos(0,822,-929)
			.set_manual_z_order(GameAnchorZ.BGVillage_Reflection_2)
			.set_scale(6.5f)
			.set_alpha_sub(0.65f);
		BGReflection.cons(_root,RLayer.REFLECTION_OBJECTS_1)
			.set_name("_bg_1_reflection")
			.set_reflection_pos(0,-681,212)
			.set_camera_pos(0,574,-929)
			.set_manual_z_order(GameAnchorZ.BGVillage_Reflection_1)
			.set_scale(5.0f)
			.set_alpha_sub(0.65f);
		BGReflection.cons(_root,RLayer.REFLECTION_OBJECTS_DOCKS)
			.set_name("_docks_reflections")
			.set_reflection_pos(0,-408,-17)
			.set_camera_pos(0,219,-929)
			.set_manual_z_order(GameAnchorZ.BGVillage_Reflection_DOCKS)
			.set_scale(4.0f)
			.set_alpha_sub(0.55f);


		return this;
	}

	public void i_update(GameEngineScene g) {
	}

}
