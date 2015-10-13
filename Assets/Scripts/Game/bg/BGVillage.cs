using UnityEngine;
using System.Collections.Generic;

public class BGVillage : SPGameUpdateable, SPNodeHierarchyElement {

	private SPNode _root;
	public SPSprite _docks, _docks_front, _bldg_1, _bldg_2, _bldg_3, _bldg_4;
	private List<BGReflection> _reflections;
	private BGWaterLineAbove _waterlineabove;

	private List<Villager> _villagers = new List<Villager>();

	public static BGVillage cons(GameEngineScene g) {
		return (new BGVillage()).i_cons(g);
	}
	
	public void add_to_parent(SPNode parent) {
		parent.add_child(_root);
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
		_bldg_4.gameObject.layer = RLayer.get_layer(RLayer.REFLECTION_OBJECTS_3);
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

		_reflections = new List<BGReflection>();
		_reflections.Add(BGReflection.cons(_root,RLayer.REFLECTION_OBJECTS_3)
			.set_name("_bg_3_reflection")
			.set_alpha_sub(0.3f)
			.set_manual_z_order(GameAnchorZ.BGVillage_Reflection_3));

		_reflections.Add(BGReflection.cons(_root,RLayer.REFLECTION_OBJECTS_2)
			.set_name("_bg_2_reflection")
			.set_reflection_pos(0,-483,458)
			.set_camera_pos(0,822,-929)
			.set_manual_z_order(GameAnchorZ.BGVillage_Reflection_2)
			.set_scale(6.5f,-4)
			.set_alpha_sub(0.65f));
		_reflections.Add(BGReflection.cons(_root,RLayer.REFLECTION_OBJECTS_1)
			.set_name("_bg_1_reflection")
			.set_reflection_pos(0,-458,212)
			.set_camera_pos(0,586,-929)
			.set_manual_z_order(GameAnchorZ.BGVillage_Reflection_1)
			.set_scale(5.0f,-3)
			.set_alpha_sub(0.55f));
		_reflections.Add(BGReflection.cons(_root,RLayer.REFLECTION_OBJECTS_DOCKS)
			.set_name("_docks_reflections")
			.set_reflection_pos(0,-499,-17)
			.set_camera_pos(0,363,-929)
			.set_manual_z_order(GameAnchorZ.BGVillage_Reflection_DOCKS)
			.set_scale(4.0f)
			.set_alpha_sub(0.55f));

		_waterlineabove = BGWaterLineAbove.cons(_root);
		_waterlineabove.set_u_pos(0,-200);
		_waterlineabove.set_u_z(-500);

		test_display_characters();

		return this;
	}
	
	
	public List<Villager> get_villagers() {
		return _villagers;
	}
	
	private void test_display_characters() {
		{
			SpriterData data = SpriterData.cons_data_from_spritesheetreaders(new List<SpriterJSONParser> { 
				SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_OLDMAN,RTex.SPRITER_OLDMAN),
			}, RTex.SPRITER_OLDMAN);

			SpriterNode test = SpriterNode.cons_spriternode_from_data(data);
			test.p_play_anim("Idle",true);
			test.set_manual_sort_z_order(GameAnchorZ.Player_Ground);
			test.set_name("OldMan_TEST");
			
			Villager tmp = Villager.cons(test);
			tmp._can_chat = false;
			tmp.set_u_pos(-130,225);
			tmp.set_u_z(219);
			_villagers.Add(tmp);
			tmp.add_to_parent(_root);
		}
		{
			SpriterData data = SpriterData.cons_data_from_spritesheetreaders(new List<SpriterJSONParser> { 
				SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_FISHGIRL,RTex.SPRITER_FISHGIRL),
			}, RTex.SPRITER_FISHGIRL);

			SpriterNode test = SpriterNode.cons_spriternode_from_data(data);
			test.p_play_anim("Idle",true);
			test.set_manual_sort_z_order(GameAnchorZ.Player_Ground);
			test.set_name("FishGirl_TEST");
			
			Villager tmp = Villager.cons(test);
			tmp.set_u_pos(350,-7);
			tmp.set_u_z(0);
			
			_villagers.Add(tmp);
			tmp.add_to_parent(_root);
		}
		{
			SpriterData data = SpriterData.cons_data_from_spritesheetreaders(new List<SpriterJSONParser> { 
				SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_DOG,RTex.SPRITER_DOG),
			}, RTex.SPRITER_DOG);

			SpriterNode test = SpriterNode.cons_spriternode_from_data(data);
			test.p_play_anim("idle",true);
			test.set_manual_sort_z_order(GameAnchorZ.Player_Ground);
			test.set_name("Dog_TEST");
			test.set_u_z(0);
			test.set_img_scale_x(-1);
			
			Villager tmp = Villager.cons(test);
			tmp.set_u_pos(-395,-5);
			tmp.set_u_z(0);
			_villagers.Add(tmp);
			tmp.add_to_parent(_root);
		}
		
		{
			SpriterData data = SpriterData.cons_data_from_spritesheetreaders(new List<SpriterJSONParser> { 
				SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_FISHMOM,RTex.SPRITER_FISHMOM),
			}, RTex.SPRITER_FISHMOM);

			SpriterNode test = SpriterNode.cons_spriternode_from_data(data);
			test.p_play_anim("Idle",true);
			test.set_manual_sort_z_order(GameAnchorZ.Player_Ground);
			test.set_name("FishMom_TEST");
			
			Villager tmp = Villager.cons(test);
			tmp._can_chat = false;
			tmp.set_u_pos(262,-13);
			tmp.set_u_z(0);
			_villagers.Add(tmp);
			tmp.add_to_parent(_root);
		}
		{
			SpriterData data = SpriterData.cons_data_from_spritesheetreaders(new List<SpriterJSONParser> { 
				SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_BOY,RTex.SPRITER_BOY),
			}, RTex.SPRITER_BOY);

			SpriterNode test = SpriterNode.cons_spriternode_from_data(data);
			test.p_play_anim("Idle",true);
			test.set_manual_sort_z_order(GameAnchorZ.Player_Ground);
			test.set_name("Boy_TEST");

			Villager tmp = Villager.cons(test);
			tmp._can_chat = false;
			tmp.set_u_pos(-100,10);
			tmp.set_u_z(0);
			_villagers.Add(tmp);
			tmp.add_to_parent(_root);
		}
	}

	public void i_update(GameEngineScene g) {
		for (int i = 0; i < _reflections.Count; i++) {
			_reflections[i].set_enabled(!g.is_camera_underwater());
		}
		if (g.is_camera_underwater()) {
			_waterlineabove.set_enabled(false);
			_bldg_1.gameObject.layer = RLayer.get_layer(RLayer.SURFACEREFLECTION_ONLY);
			_bldg_2.gameObject.layer = RLayer.get_layer(RLayer.SURFACEREFLECTION_ONLY);
		} else {
			_waterlineabove.set_enabled(true);
			_waterlineabove.i_update(g);

			_bldg_1.gameObject.layer = RLayer.get_layer(RLayer.REFLECTION_OBJECTS_1);
			_bldg_2.gameObject.layer = RLayer.get_layer(RLayer.REFLECTION_OBJECTS_2);
		}

	}

}
