using UnityEngine;
using System.Collections;

public class GameUI : SPGameUpdateable {

	public static GameUI cons() {
		return (new GameUI()).i_cons();
	}
	
	public SPNode _root;
	public GameUICursor _cursor;
	
	public GameUI i_cons() {
		_root = SPNode.cons_node();
		_root.transform.SetParent(GameMain._context._game_camera.transform);
		_root.set_u_pos(0,0);
		_root.set_u_z(1000);
		_root.set_name("GameUI");
		_root.set_manual_sort_z_order(GameAnchorZ.HUD_BASE);
		
		_cursor = GameUICursor.cons(_root);
		
		return this;
	}
	
	public void i_update(GameEngineScene g) {
		_cursor.i_update(g);
	}
	
	public SPHitRect get_ui_bounds() {
		if (!__has_calc_ui_bounds) this.calc_ui_bounds();
		return __cached_bounds;
	}
	
	private bool __has_calc_ui_bounds = false;
	private SPHitRect __cached_bounds;
	private void calc_ui_bounds() {
		__has_calc_ui_bounds = true;
		Vector3 letterbox_offset = SPUtil.game_from_view_screen_offset();
		Vector3 ui_bl_anchor = GameMain._context._game_camera.transform.InverseTransformPoint(
			GameMain._context._game_camera.ScreenToWorldPoint(new Vector3(
				letterbox_offset.x,
				letterbox_offset.y,
				_root.transform.position.z - GameMain._context._game_camera.transform.position.z
			))
		);
		Vector3 ui_tr_anchor = GameMain._context._game_camera.transform.InverseTransformPoint(
			GameMain._context._game_camera.ScreenToWorldPoint(new Vector3(
				letterbox_offset.x + SPUtil.game_screen().x,
				letterbox_offset.y + SPUtil.game_screen().y,
				_root.transform.position.z - GameMain._context._game_camera.transform.position.z
			))
		);
		
		__cached_bounds = new SPHitRect() {
			_x1 = ui_bl_anchor.x,
			_y1 = ui_bl_anchor.y,
			_x2 = ui_tr_anchor.x,
			_y2 = ui_tr_anchor.y
		};
	}
	
}
