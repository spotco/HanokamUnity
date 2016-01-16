using UnityEngine;
using System.Collections.Generic;

public class BGVillage : SPGameUpdateable, SPNodeHierarchyElement {
	
	private SPNode _root;
	public void set_u_pos(float x, float y) { _root.set_u_pos(x,y); }
	public Vector2 get_u_pos() { return _root.get_u_pos(); }
	public void set_enabled(bool val) { _root.set_enabled(val); }
	
	public SPSprite _docks, _docks_front/*, _bldg_1, _bldg_2, _bldg_3, _bldg_4*/;
	
	private List<SPSprite> _reflection_sprites;
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
		
		{
			SPSprite hills_far = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_NVILLAGE_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"neu_village_hills_far.png")
			);
			hills_far.set_manual_sort_z_order(GameAnchorZ.BGVillage_HILLS_FAR);
			hills_far.set_u_z(2000f);
			hills_far.set_scale(4.0f);
			hills_far.set_anchor_point(0.5f,0);
			hills_far.set_u_pos(0,500);
			hills_far.gameObject.layer = RLayer.get_layer(RLayer.REFLECTION_VILLAGE_BACK_HILLS);
			hills_far.set_name("hills_far");
			_root.add_child(hills_far);
		}
		{
			SPSprite hills_near = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_NVILLAGE_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"neu_village_hills_near.png")
			);
			hills_near.set_manual_sort_z_order(GameAnchorZ.BGVillage_HILLS_NEAR);
			hills_near.set_u_z(1200f);
			hills_near.set_scale(3.15f);
			hills_near.set_anchor_point(0.5f,0);
			hills_near.set_name("hills_near");
			hills_near.gameObject.layer = RLayer.get_layer(RLayer.REFLECTION_VILLAGE_BACK_HILLS);
			_root.add_child(hills_near);
		}
		
		{
			SPSprite building_back_left = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_NVILLAGE_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"neu_village_building_back_left.png")
			);
			building_back_left.set_manual_sort_z_order(GameAnchorZ.BGVillage_BUILDINGS_BACK);
			building_back_left.set_u_pos(-700,549);
			building_back_left.set_u_z(520f);
			building_back_left.set_scale(2.1f);
			building_back_left.set_anchor_point(0.5f,0);
			building_back_left.set_name("building_back_left");
			_root.add_child(building_back_left);
		}
		{
			SPSprite building_back_right_top = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_NVILLAGE_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"neu_village_building_back_right_top.png")
			);
			building_back_right_top.set_manual_sort_z_order(GameAnchorZ.BGVillage_BUILDINGS_BACK);
			building_back_right_top.set_u_pos(762,557);
			building_back_right_top.set_u_z(520f);
			building_back_right_top.set_scale(2.1f);
			building_back_right_top.set_anchor_point(0.5f,0);
			building_back_right_top.set_name("neu_village_building_back_right_top");
			_root.add_child(building_back_right_top);
		}
		{
			SPSprite back_flag = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_NVILLAGE_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"neu_village_flag_line_2.png")
				);
			back_flag.set_manual_sort_z_order(GameAnchorZ.BGVillage_BUILDINGS_BACK-1);
			back_flag.set_u_pos(413,869);
			back_flag.set_u_z(520);
			back_flag.set_scale(1.5f);
			back_flag.set_anchor_point(1,0.5f);
			back_flag.set_name("back_flag");
			_root.add_child(back_flag);
		}
		
		
		{
			SPSprite mid_flag = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_NVILLAGE_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"neu_village_flag_line.png")
				);
			mid_flag.set_manual_sort_z_order(GameAnchorZ.BGVillage_BUILDINGS_MID-1);
			mid_flag.set_u_pos(152,595);
			mid_flag.set_u_z(402);
			mid_flag.set_scale(2.2f);
			mid_flag.set_anchor_point(1,0.5f);
			mid_flag.set_name("mid_flag");
			_root.add_child(mid_flag);
		}
		{
			SPSprite building_mid_right = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_NVILLAGE_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"neu_village_building_mid_right.png")
			);
			building_mid_right.set_manual_sort_z_order(GameAnchorZ.BGVillage_BUILDINGS_MID);
			building_mid_right.set_u_pos(464,26);
			building_mid_right.set_u_z(402f);
			building_mid_right.set_scale(2.1f);
			building_mid_right.set_anchor_point(0.5f,0);
			building_mid_right.set_name("building_mid_right");
			_root.add_child(building_mid_right);
		}
		{
			SPSprite building_mid_left = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_NVILLAGE_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"neu_village_building_mid_left.png")
			);
			building_mid_left.set_manual_sort_z_order(GameAnchorZ.BGVillage_BUILDINGS_MID);
			building_mid_left.set_u_z(402);
			building_mid_left.set_u_pos(-470,8);
			building_mid_left.set_scale(1.9f);
			building_mid_left.set_anchor_point(0.5f,0);
			building_mid_left.set_name("building_mid_left");
			_root.add_child(building_mid_left);
		}
		
		{
			SPSprite building_mid_leftmost_left = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_NVILLAGE_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"neu_village_building_mid_leftmost_left.png")
				);
			building_mid_leftmost_left.set_manual_sort_z_order(GameAnchorZ.BGVillage_BUILDINGS_MID);
			building_mid_leftmost_left.set_u_z(402);
			building_mid_leftmost_left.set_u_pos(-1043,461);
			building_mid_leftmost_left.set_scale(1.9f);
			building_mid_leftmost_left.set_anchor_point(0.5f,0);
			building_mid_leftmost_left.set_name("building_mid_leftmost_left");
			_root.add_child(building_mid_leftmost_left);
		}
		
		
		
		{
			SPSprite building_front_left_front = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_NVILLAGE_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"neu_village_building_front_left_front.png")
				);
			building_front_left_front.set_manual_sort_z_order(GameAnchorZ.BGVillage_BUILDINGS_NEAR);
			building_front_left_front.set_u_z(215f);
			building_front_left_front.set_u_pos(-674,-42);
			building_front_left_front.set_scale(1.9f);
			building_front_left_front.set_anchor_point(0.5f,0);
			building_front_left_front.set_name("neu_village_building_front_left_front");
			_root.add_child(building_front_left_front);
		}
		{
			SPSprite building_front_right = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_NVILLAGE_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"neu_village_building_front_right.png")
				);
			building_front_right.set_manual_sort_z_order(GameAnchorZ.BGVillage_BUILDINGS_NEAR);
			building_front_right.set_u_z(215f);
			building_front_right.set_u_pos(580,-108);
			building_front_right.set_scale(1.9f);
			building_front_right.set_anchor_point(0.5f,0);
			building_front_right.set_name("neu_village_building_front_right");
			_root.add_child(building_front_right);
		}
		
		
		{
			_docks = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_NVILLAGE_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"neu_village_docks.png")
			);
			_docks.set_manual_sort_z_order(GameAnchorZ.BGVillage_Docks);
			_docks.set_anchor_point(0.5f,0);
			_docks.set_u_pos(0,-182);
			_docks.set_scale(1.75f);
			_docks.set_name("_docks");
			_docks.set_layer(RLayer.REFLECTION_SURFACE_DOCK);
			_root.add_child(_docks);
	
			_docks_front = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_NVILLAGE_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"neu_village_docks_pillars.png")
			);
			_docks_front.set_manual_sort_z_order(GameAnchorZ.BGVillage_Docks_Front);
			_docks_front.set_anchor_point(_docks.anchorpoint().x,_docks.anchorpoint().y);
			_docks_front.set_u_pos(_docks.get_u_pos());
			_docks_front.set_scale(_docks.scale_x());
			_docks_front.set_name("_docks_front");
			_root.add_child(_docks_front);
		}
		
		this.setup_jumping_logs();
		
		_reflection_sprites = new List<SPSprite>();
		
		{
			_reflections = new List<BGReflection>();
			_reflections.Add(BGReflection.cons(_root,RLayer.REFLECTION_VILLAGE_BACK_HILLS)
				.set_name("_bg_3_reflection")
				.set_alpha_sub(0.3f)
				.set_scale(12.55f)
				.set_camera_pos(0,1597,-1523)
				.set_reflection_pos(0,-1450,1046)
				.set_manual_z_order(GameAnchorZ.BGVillage_Reflection_3));
				
			_reflections.Add(BGReflection.cons(_root,RLayer.REFLECTION_SURFACE_DOCK,512,128)
				.set_name("_docks_reflections")
				.set_reflection_pos(0,-378,0)
				.set_camera_pos(0,135,-447)
				.set_manual_z_order(GameAnchorZ.BGVillage_Reflection_DOCKS)
				.set_scale(4.0f)
				.set_alpha_sub(0.7f)
				.set_y_mult(25,75)
				.set_wave_ampl(0.01f,0.0025f)
			);
			_reflections.Add(BGReflection.cons(_root,RLayer.REFLECTION_SURFACE_CHARACTER,512,128)
				.set_name("_character_reflections")
				.set_reflection_pos(0,-378,0)
				.set_camera_pos(0,135,-447)
				.set_manual_z_order(GameAnchorZ.BGVillage_Reflection_DOCKS)
				.set_scale(4.0f)
				.set_alpha_sub(0.45f)
				.set_y_mult(25,75)
				.set_wave_ampl(0.01f,0.0025f)
			);
		}

		_waterlineabove = BGWaterLineAbove.cons(_root);
		_waterlineabove.set_u_pos(0,-200);
		_waterlineabove.set_u_z(-500);

		test_display_characters();

		return this;
	}
	
	public Vector3 jump_log0_position() { return new Vector3(0,-129,-8.5f); }
	public Vector3 jump_log1_position() { return new Vector3(0,-152.9f,-88); }
	public Vector3 jump_log2_position() { return new Vector3(0,-175.5f,-169.4f); }
	public Vector3 jump_log3_position() { return new Vector3(0,-194,-274.7f); }
	
	public List<BGVillageJumpLog> _jump_logs = new List<BGVillageJumpLog>();
	
	private void setup_jumping_logs() {
		{
			BGVillageJumpLog log = BGVillageJumpLog.cons();
			log.set_position(this.jump_log0_position());
			log.set_scale(1.0f);
			log.set_name("jumping_log_0");
			log.add_to_parent(_root);
			_jump_logs.Add(log);
		}
		{
			BGVillageJumpLog log = BGVillageJumpLog.cons();
			log.set_position(this.jump_log1_position());
			log.set_scale(1.15f);
			log.set_name("jumping_log_1");
			log.add_to_parent(_root);
			_jump_logs.Add(log);
		}
		{
			BGVillageJumpLog log = BGVillageJumpLog.cons();
			log.set_position(this.jump_log2_position());
			log.set_scale(1.35f);
			log.set_name("jumping_log_2");
			log.add_to_parent(_root);
			_jump_logs.Add(log);
		}
		{
			BGVillageJumpLog log = BGVillageJumpLog.cons();
			log.set_position(this.jump_log3_position());
			log.set_scale(1.5f);
			log.set_name("jumping_log_3");
			log.add_to_parent(_root);
			_jump_logs.Add(log);
		}
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
			test.set_layer(RLayer.REFLECTION_SURFACE_CHARACTER);
			
			Villager tmp = Villager.cons(test);
			tmp._can_chat = false;
			tmp.set_u_pos(-393,225);
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
			test.set_layer(RLayer.REFLECTION_SURFACE_CHARACTER);
			
			Villager tmp = Villager.cons(test);
			tmp.set_u_pos(700,-12);
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
			test.set_layer(RLayer.REFLECTION_SURFACE_CHARACTER);
			
			Villager tmp = Villager.cons(test);
			tmp.set_u_pos(-775,-12);
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
			test.set_layer(RLayer.REFLECTION_SURFACE_CHARACTER);
			
			Villager tmp = Villager.cons(test);
			tmp._can_chat = false;
			tmp.set_u_pos(200,-13);
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
			test.set_layer(RLayer.REFLECTION_SURFACE_CHARACTER);

			Villager tmp = Villager.cons(test);
			tmp._can_chat = false;
			tmp.set_u_pos(-100,-14);
			tmp.set_u_z(0);
			_villagers.Add(tmp);
			tmp.add_to_parent(_root);
		}
	}

	public void i_update(GameEngineScene g) {
		for (int i = 0; i < _reflections.Count; i++) {
			_reflections[i].set_enabled(this.is_above_water(g));
		}
		if (this.is_above_water(g)) {
			_waterlineabove.set_enabled(true);
			_waterlineabove.i_update(g);
			for (int i = 0; i < _reflection_sprites.Count; i++) {
				_reflection_sprites[i].set_layer(RLayer.REFLECTION_VILLAGE_BACK_HILLS);
			}
		} else {
			_waterlineabove.set_enabled(false);
			for (int i = 0; i < _reflection_sprites.Count; i++) {
				_reflection_sprites[i].set_layer(RLayer.SURFACEREFLECTION_ONLY);
			}
		}
		
		for (int i = 0; i < _jump_logs.Count; i++) {
			_jump_logs[i].i_update();
		}
		
	}
	
	private bool is_above_water(GameEngineScene g) {
		GameStateIdentifier cur_state = g.get_top_game_state().get_state();
		return cur_state != GameStateIdentifier.Dive;
	}

}
