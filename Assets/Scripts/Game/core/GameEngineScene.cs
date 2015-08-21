using UnityEngine;
using System.Collections.Generic;

public class GameEngineScene : SPScene {

	public static GameEngineScene cons() {
		GameEngineScene rtv = GameMain._context.gameObject.AddComponent<GameEngineScene>();
		return rtv.i_cons();
	}

	private GameEngineScene i_cons() {
		{
			SPNode test_node = SPNode.cons_node();
			test_node.set_s_pos(SPUtil.game_screen().x/2,SPUtil.game_screen().y/2);
			test_node.set_rotation(0.0f);

			SPSprite test_sprite = SPSprite.cons_sprite_texkey_texrect(RTex.BG_SPRITESHEET_1,new Rect(0,0,1,1));
			test_node.add_child(test_sprite);
			test_sprite.set_anchor_point(0.5f,0.5f);
			test_sprite.set_rotation(0.0f);
			test_sprite.set_tex_rect(FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"bg_3.png"));
		}
		/*
		test_sprite = (SPSprite)test_sprite.repool();

		test_sprite = SPSprite.cons_sprite_texkey_texrect(RTex.BG_SPRITESHEET_1,FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"bg_3.png"));
		test_node.add_child(test_sprite);
		test_sprite.set_anchor_point(0.0f,0.0f);
		test_sprite.set_rotation(-45.0f);
		*/


		{
			SpriterData data = SpriterData.cons_data_from_spritesheetreaders(
				new List<SpriteSheetReader> { SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_OLDMAN,RTex.SPRITER_OLDMAN) },
				RTex.SPRITER_OLDMAN
			);
			SpriterNode test_node = SpriterNode.cons_spriternode_from_data(data);
			test_node.p_play_anim_on_finish("Sleeping","Wake up");
			test_node.set_s_pos(50,100);
		}

		{
			SpriterData data = SpriterData.cons_data_from_spritesheetreaders(
				new List<SpriteSheetReader> { SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_FISHGIRL,RTex.SPRITER_FISHGIRL) },
				RTex.SPRITER_FISHGIRL
			);
			SpriterNode test_node = SpriterNode.cons_spriternode_from_data(data);
			test_node.p_play_anim("Idle",true);
			test_node.set_s_pos(150,100);
		}

		{
			SpriterData data = SpriterData.cons_data_from_spritesheetreaders(
				new List<SpriteSheetReader> { 
					SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA,RTex.SPRITER_HANOKA),
					SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA_BOW,RTex.SPRITER_HANOKA_BOW),
					SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA_SWORD,RTex.SPRITER_HANOKA_SWORD)
				},
			RTex.SPRITER_HANOKA
			);
			SpriterNode test_node = SpriterNode.cons_spriternode_from_data(data);
			test_node.p_play_anim("Idle",true);
			test_node.set_s_pos(250,100);
		}

		return this;
	}

	public override void i_update(float dt_scale) {

	}
}
