using UnityEngine;
using System.Collections;

public class GameEngineScene : SPScene {

	public static GameEngineScene cons() {
		GameEngineScene rtv = GameMain._context.gameObject.AddComponent<GameEngineScene>();
		return rtv.i_cons();
	}

	private GameEngineScene i_cons() {
		SPSprite test = SPSprite.cons(RTex.BG_SPRITESHEET_1,new Rect());

		return this;
	}

	public override void i_update(float dt_scale) {

	}
}
