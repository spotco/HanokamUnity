using UnityEngine;
using System.Collections;

public class GameUICursor : SPGameUpdateable {

	public static GameUICursor cons(SPNode parent) {
		return (new GameUICursor()).i_cons(parent);
	}
	
	public SPNode _root;
	public SPSprite _image;
	
	public GameUICursor i_cons(SPNode parent) {
		_root = SPNode.cons_node();
		_root.set_u_pos(0,0);
		_root.set_name("GameUICursor");
		parent.add_child(_root);
		
		_image = SPSprite.cons_sprite_texkey_texrect(
			RTex.HUD_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"mouse_target.png")
		);
		_image.set_scale(0.25f);
		_image.set_color(SPUtil.color_from_bytes(255,237,114,191));
		_root.add_child(_image);
		
		return this;
	}
	
	public GameUICursor set_u_pos(float x, float y) {
		_root.set_u_pos(x,y);
		return this;
	}
	
	public GameUICursor set_enabled(bool val) {
		_root.set_enabled(val);
		return this;
	}
	
	public Vector2 get_game_pos() {
		Vector3 screen_pos = GameMain._context._game_camera.WorldToScreenPoint(_root.transform.position);
		Vector3 world_pos = GameMain._context._game_camera.ScreenToWorldPoint(new Vector3(
			screen_pos.x,
			screen_pos.y,
			Mathf.Abs(GameMain._context._game_camera.transform.position.z)
		));
		Vector3 game_pos = GameMain._context.transform.InverseTransformPoint(world_pos);
		return new Vector2(game_pos.x,game_pos.y);
	}
	
	public void i_update(GameEngineScene g) {
		_root.set_u_pos(
			Mathf.Clamp(_root._u_x + g._controls.get_cursor_move_delta().x ,g._game_ui.get_ui_bounds()._x1,g._game_ui.get_ui_bounds()._x2),
			Mathf.Clamp(_root._u_y + g._controls.get_cursor_move_delta().y ,g._game_ui.get_ui_bounds()._y1,g._game_ui.get_ui_bounds()._y2)
		);
		_image.set_rotation((_image.rotation() - 1.25f * SPUtil.dt_scale_get()) % 360.0f);
	}

}

