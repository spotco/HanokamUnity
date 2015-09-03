using UnityEngine;
using System.Collections.Generic;

public interface SPGameUpdateable {
	void i_update(GameEngineScene g);
}

public class GameEngineScene : SPScene {

	public BGVillage _bg_village;
	public BGSky _bg_sky;
	private List<SPGameUpdateable> _bg_elements;

	public static GameEngineScene cons() {
		GameEngineScene rtv = GameMain._context.gameObject.AddComponent<GameEngineScene>();
		return rtv.i_cons();
	}

	private GameEngineScene i_cons() {
		__cached_viewbox_dirty = true;

		_bg_village = BGVillage.cons(this);
		_bg_sky = BGSky.cons(this);
		_bg_elements = new List<SPGameUpdateable>() {_bg_village,_bg_sky};

		{
			SpriterData data = SpriterData.cons_data_from_spritesheetreaders(new List<SpriteSheetReader> { 
					SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA,RTex.SPRITER_HANOKA),
					SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA_BOW,RTex.SPRITER_HANOKA_BOW),
					SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA_SWORD,RTex.SPRITER_HANOKA_SWORD)
				}, RTex.SPRITER_HANOKA
			);
			SpriterNode test_node = SpriterNode.cons_spriternode_from_data(data);
			test_node.p_play_anim("Idle",true);
			test_node.set_u_pos(0,0);
			test_node.set_manual_sort_z_order(GameAnchorZ.Player_Ground);
		}


		return this;
	}

	public override void i_update(float dt_scale) {
		SPUtil.dt_scale_set(dt_scale);
		__cached_viewbox_dirty = true;

		for (int i = 0; i < _bg_elements.Count; i++) {
			SPGameUpdateable itr = _bg_elements[i];
			itr.i_update(this);
		}
	}

	private SPHitRect __cached_viewbox;
	private bool __cached_viewbox_dirty;
	public SPHitRect get_viewbox() {
		if (!__cached_viewbox_dirty) return __cached_viewbox;
		__cached_viewbox_dirty = false;
		__cached_viewbox = get_viewbox_dist(0);
		return __cached_viewbox;
	}

	public SPHitRect get_viewbox_dist(float offset_dist) {
		Vector3 bl = GameMain._context._game_camera.ScreenToWorldPoint(
			new Vector3(
			GameMain._context._game_camera.rect.x * SPUtil.view_screen().x,
			GameMain._context._game_camera.rect.y * SPUtil.view_screen().y,
			Mathf.Abs(GameMain._context._game_camera.transform.position.z) + offset_dist
			));
		Vector3 tr = GameMain._context._game_camera.ScreenToWorldPoint(
			new Vector3(
			SPUtil.game_screen().x + GameMain._context._game_camera.rect.x * SPUtil.view_screen().x,
			SPUtil.game_screen().y + GameMain._context._game_camera.rect.y * SPUtil.view_screen().y,
			Mathf.Abs(GameMain._context._game_camera.transform.position.z) + offset_dist
			));
		bl = GameMain._context.transform.InverseTransformPoint(bl);
		tr = GameMain._context.transform.InverseTransformPoint(tr);
		return new SPHitRect(){ _x1 = bl.x, _y1 = bl.y, _x2 = tr.x, _y2 = tr.y };
	}

}
