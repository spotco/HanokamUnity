using UnityEngine;
using System.Collections;

public class GameEngineScene : SPScene {

	public static GameEngineScene cons() {
		GameEngineScene rtv = GameMain._context.gameObject.AddComponent<GameEngineScene>();
		return rtv.i_cons();
	}

	private GameEngineScene i_cons() {
		SPSprite.cons(RTex.BG_SPRITESHEET_1,GameMain._context._file_cache.get_texrect(RTex.BG_SPRITESHEET_1,"bg_3.png")).set_pos(0.75f,0.75f).set_z(0.2f).set_color(new Vector4(1,0,1,1f));
		SPSprite.cons(RTex.BG_SPRITESHEET_1,GameMain._context._file_cache.get_texrect(RTex.BG_SPRITESHEET_1,"bg_3.png")).set_pos(0.75f,0.75f).set_z(0.2f).set_color(new Vector4(1,1,1,1f));
		SPSprite.cons(RTex.BG_SPRITESHEET_1,GameMain._context._file_cache.get_texrect(RTex.BG_SPRITESHEET_1,"bg_3.png")).set_pos(0.75f,0.75f).set_z(0.2f).set_color(new Vector4(0,1,1,1f));
		SPSprite.cons(RTex.BG_SPRITESHEET_1,GameMain._context._file_cache.get_texrect(RTex.BG_SPRITESHEET_1,"bg_3.png")).set_pos(0.75f,0.75f).set_z(0.2f).set_color(new Vector4(0,0,1,1f));
		SPSprite.cons(RTex.BG_SPRITESHEET_1,GameMain._context._file_cache.get_texrect(RTex.BG_SPRITESHEET_1,"bg_3.png")).set_pos(0.25f,1f);
		SPSprite.cons(RTex.BG_SPRITESHEET_1,GameMain._context._file_cache.get_texrect(RTex.BG_SPRITESHEET_1,"bg_3.png")).set_pos(0.75f,0.75f).set_z(0.2f).set_color(new Vector4(1,0,1,1f));
		SPSprite.cons(RTex.BG_SPRITESHEET_1,GameMain._context._file_cache.get_texrect(RTex.BG_SPRITESHEET_1,"bg_3.png")).set_pos(-0.25f,-0.5f).set_z(-0.1f).set_color(new Vector4(1.0f,1,1,1.0f));	
		SPSprite.cons(RTex.BG_SPRITESHEET_1,GameMain._context._file_cache.get_texrect(RTex.BG_SPRITESHEET_1,"bg_3.png")).set_pos(-0.35f,0.25f).set_z(-0.2f).set_color(new Vector4(1.0f,1,1,1.0f));

		return this;
	}

	public override void i_update(float dt_scale) {

	}
}
