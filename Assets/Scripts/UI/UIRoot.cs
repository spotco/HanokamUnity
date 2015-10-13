using UnityEngine;
using System.Collections.Generic;

public abstract class BaseSubUI : SPNodeHierarchyElement {
	public bool _is_showing = false;
	public virtual void on_show() {}
	public virtual void on_hide() {}
	public virtual bool should_show() { return false; }
	public virtual void add_to_parent(SPNode parent) { throw new System.Exception("ought to implement add_to_parent"); }
	
	public static void show_list<T>(List<T> subuis) where T : BaseSubUI {
		for (int i = 0; i < subuis.Count; i++) {
			BaseSubUI itr = subuis[i];
			if (!itr._is_showing) {
				itr.on_show();
			}
			itr._is_showing = true;
		}
	}
	public static void hide_list<T>(List<T> subuis) where T : BaseSubUI {
		for (int i = 0; i < subuis.Count; i++) {
			BaseSubUI itr = subuis[i];
			if (itr._is_showing) {
				itr.on_hide();
			}
			itr._is_showing = false;
		}
	}
	public static void update_list<T>(List<T> subuis) where T : BaseSubUI {
		for (int i = 0; i < subuis.Count; i++) {
			BaseSubUI itr = subuis[i];
			if (itr.should_show()) {
				if (!itr._is_showing) {
					itr.on_show();
				}
				itr._is_showing = true;
			} else {
				if (itr._is_showing) {
					itr.on_hide();
				}
				itr._is_showing = false;
			}
		}
	}
	
}

public abstract class GameUISubUI : BaseSubUI, SPGameUpdateable {
	public virtual void i_update(GameEngineScene g) {}
	public override bool should_show() { 
		return GameMain._context.get_top_scene().GetType() == typeof(GameEngineScene); 
	}
}

public abstract class ShopUISubUI : BaseSubUI, SPShopUpdateable {
	public virtual void i_update(ShopScene g) {}
	public override bool should_show() {
		return GameMain._context.get_top_scene().GetType() == typeof(ShopScene); 
	}
}

public class UIRoot : SPGameUpdateable {
	
	public static UIRoot cons() {
		return (new UIRoot()).i_cons();
	}
	
	public SPNode _root;
	public SPSprite _blur_cover;
	
	private List<GameUISubUI> _game_sub_uis = new List<GameUISubUI>();
	private List<ShopUISubUI> _shop_sub_uis = new List<ShopUISubUI>();
	
	public UIRoot i_cons() {
		_root = SPNode.cons_node();
		_root.transform.SetParent(GameMain._context._game_camera.transform);
		_root.set_u_pos(0,0);
		_root.set_u_z(1000);
		_root.set_name("UIRoot");
		_root.set_manual_sort_z_order(GameAnchorZ.HUD_BASE);
		
		this.add_game_ui(OnGroundSubUI.cons(this));
		this.add_game_ui(InDialogueSubUI.cons(this));
		this.add_shop_ui(ShopMainUISubUI.cons(this));
		
		GameMain._context._camerac.create_blur_texture(this);
		
		return this;
	}
	
	public void on_scene_transition() {
		SPScene top_scene = GameMain._context.get_top_scene();
		if (top_scene.GetType() == typeof(GameEngineScene)) {
			BaseSubUI.hide_list(_shop_sub_uis);
			BaseSubUI.show_list(_game_sub_uis);
			
		} else if (top_scene.GetType() == typeof(ShopScene)) {
			BaseSubUI.show_list(_shop_sub_uis);
			BaseSubUI.hide_list(_game_sub_uis);
		}
	}
	
	public void add_game_ui(GameUISubUI subui) {
		_game_sub_uis.Add(subui);
		subui.add_to_parent(_root);
	}
	
	public void add_shop_ui(ShopUISubUI subui) {
		_shop_sub_uis.Add(subui);
		subui.add_to_parent(_root);
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
		BaseSubUI.update_list(_game_sub_uis);
		for (int i = 0; i < _game_sub_uis.Count; i++) {
			_game_sub_uis[i].i_update(g);
		}
	}
	
	public void i_update(ShopScene shop) {
		BaseSubUI.update_list(_shop_sub_uis);
		for (int i = 0; i < _shop_sub_uis.Count; i++) {
			_shop_sub_uis[i].i_update(shop);
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
