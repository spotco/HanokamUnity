using UnityEngine;
using System.Collections.Generic;

public abstract class GameUISubUI : SPGameUpdateable {
	public bool _is_showing = false;
	public virtual void on_show() {}
	public virtual void on_hide() {}
	public virtual void i_update(GameEngineScene g) {}
	public virtual bool should_show(GameEngineScene g) { return false; }
}

public class UIRoot : SPGameUpdateable {

	public static UIRoot cons() {
		return (new UIRoot()).i_cons();
	}
	
	public SPNode _root;
	public SPSprite _blur_cover;
	private List<GameUISubUI> _game_sub_uis;
	
	public UIRoot i_cons() {
		_root = SPNode.cons_node();
		_root.transform.SetParent(GameMain._context._game_camera.transform);
		_root.set_u_pos(0,0);
		_root.set_u_z(1000);
		_root.set_name("UIRoot");
		_root.set_manual_sort_z_order(GameAnchorZ.HUD_BASE);
		
		_game_sub_uis = new List<GameUISubUI>() {
			OnGroundSubUI.cons(this,_root)
		};
		
		GameMain._context._camerac.create_blur_texture(this);
		
		return this;
	}
	
	public SPSprite add_blur_cover_sprite(Texture game_camera_copy_tex) {
		if (_blur_cover == null) {
			_blur_cover = SPSprite.cons_sprite_texkey_texrect(RTex.BLANK,new Rect(0,0,0,0));
			_blur_cover.manual_set_texture(game_camera_copy_tex);
			_blur_cover.manual_set_mesh_size(game_camera_copy_tex.width,game_camera_copy_tex.height);
			_blur_cover.set_scale_x((this.get_ui_bounds()._x2 - this.get_ui_bounds()._x1)/_blur_cover.texrect().size.x);
			_blur_cover.set_scale_y((this.get_ui_bounds()._y2 - this.get_ui_bounds()._y1)/_blur_cover.texrect().size.y);
			_blur_cover.set_manual_sort_z_order(0);
			_blur_cover.gameObject.layer = RLayer.get_layer(RLayer.UI);
			_blur_cover.set_name("_blur_cover");
			_blur_cover.set_opacity(0.35f);
			_root.add_child(_blur_cover);
		}
		return _blur_cover;
	}
	
	public void i_update(GameEngineScene g) {
		for (int i = 0; i < _game_sub_uis.Count; i++) {
			GameUISubUI itr = _game_sub_uis[i];
			if (itr.should_show(g)) {
				if (!itr._is_showing) {
					itr.on_show();
				}
				itr._is_showing = true;
				itr.i_update(g);
			} else {
				if (itr._is_showing) {
					itr.on_hide();
				}
				itr._is_showing = false;
			}
		}
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
