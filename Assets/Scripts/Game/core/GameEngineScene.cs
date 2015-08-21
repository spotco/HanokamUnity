using UnityEngine;
using System.Collections;

public class GameEngineScene : SPScene {

	public static GameEngineScene cons() {
		GameEngineScene rtv = GameMain._context.gameObject.AddComponent<GameEngineScene>();
		return rtv.i_cons();
	}

	private GameEngineScene i_cons() {
		SPNode test_node = SPNode.cons();
		test_node.set_s_pos(SPUtil.game_screen().x/2,SPUtil.game_screen().y/2);
		test_node.set_rotation(45.0f);


		SPSprite test_sprite = SPSprite.cons(RTex.BG_SPRITESHEET_1,GameMain._context._file_cache.get_texrect(RTex.BG_SPRITESHEET_1,"bg_3.png"));
		test_node.add_child(test_sprite);
		test_sprite.set_anchor_point(0.0f,0.0f);
		test_sprite.set_rotation(-45.0f);

		test_sprite.repool();

		test_sprite = SPSprite.cons(RTex.BG_SPRITESHEET_1,GameMain._context._file_cache.get_texrect(RTex.BG_SPRITESHEET_1,"bg_3.png"));
		test_node.add_child(test_sprite);
		test_sprite.set_anchor_point(0.0f,0.0f);
		test_sprite.set_rotation(-45.0f);

		//test.set_u_z(400);
		//test.set_s_pos(SPUtil.game_screen().x/2,SPUtil.game_screen().y/2);
		//test.set_rotation(45f);

		/*
		SPSprite test2 = SPSprite.cons(RTex.BG_SPRITESHEET_1,GameMain._context._file_cache.get_texrect(RTex.BG_SPRITESHEET_1,"bg_2.png"));
		test2.set_anchor_point(1.0f,1.0f);
		test2.set_rotation(0.0f);
		test2.set_u_pos(SPUtil.pct_of_obj_u_with_anchorpt(test,1.0f,1.0f));
		test2.set_name("test2");
		test2.set_scale(3.0f);

		test.add_child(test2);

		SPSprite test3 = SPSprite.cons(RTex.BG_SPRITESHEET_1,GameMain._context._file_cache.get_texrect(RTex.BG_SPRITESHEET_1,"bg_1.png"));
		test3.set_anchor_point(1.0f,1.0f);
		test3.set_rotation(0.0f);
		test3.set_u_pos(SPUtil.pct_of_obj_u_with_anchorpt(test2,1.0f,1.0f));
		test3.set_name("test3");
		test2.add_child(test3);
		*/

		return this;
	}

	public override void i_update(float dt_scale) {

	}
}
