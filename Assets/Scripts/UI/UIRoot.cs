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
	private SPSprite _red_flash_overlay;
	private SPSprite _fadeout_overlay;
	
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
		this.add_game_ui(InAirSubUI.cons(this));
		this.add_game_ui(DiveSubUI.cons(this));
		
		this.add_shop_ui(ShopMainUISubUI.cons(this));
		
		GameMain._context._camerac.create_blur_texture(this);
		
		_red_flash_overlay = SPSprite.cons_sprite_texkey_texrect(RTex.BLANK, SPUtil.texture_default_rect(RTex.BLANK));
		_red_flash_overlay.set_layer(RLayer.UI);
		_red_flash_overlay.set_scale_x((this.get_ui_bounds()._x2 - this.get_ui_bounds()._x1)/_red_flash_overlay.texrect().size.x);
		_red_flash_overlay.set_scale_y((this.get_ui_bounds()._y2 - this.get_ui_bounds()._y1)/_red_flash_overlay.texrect().size.y);
		_red_flash_overlay.set_color(new Vector4(1,0.2f,0.2f,1));
		_red_flash_overlay.set_opacity(0f);
		_root.add_child(_red_flash_overlay);
		
		_fadeout_overlay = SPSprite.cons_sprite_texkey_texrect(RTex.BLANK, SPUtil.texture_default_rect(RTex.BLANK));
		_fadeout_overlay.set_layer(RLayer.UI);
		_fadeout_overlay.set_scale_x((this.get_ui_bounds()._x2 - this.get_ui_bounds()._x1)/_fadeout_overlay.texrect().size.x);
		_fadeout_overlay.set_scale_y((this.get_ui_bounds()._y2 - this.get_ui_bounds()._y1)/_fadeout_overlay.texrect().size.y);
		_fadeout_overlay.set_color(new Vector4(0,0,0,1));
		_fadeout_overlay.set_opacity(0f);
		_root.add_child(_fadeout_overlay);
		
		return this;
	}
	
	public void do_red_flash() {
		_red_flash_overlay.set_opacity(0.7f);
	}
	
	private bool _fadeout_overlay_target;
	public void set_fadeout_overlay(bool val) {
		_fadeout_overlay_target = val;
	}
	public void set_fadeout_overlay_imm(bool val) {
		this.set_fadeout_overlay(val);
		_fadeout_overlay.set_opacity(val?1:0);
		_fadeout_overlay.set_enabled(true);
	}
	public bool get_fadeout_overlay_anim_finished_for_target(bool val) {
		return SPUtil.flt_cmp_delta(_fadeout_overlay.get_opacity(),val?1:0,0.01f);
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
		_red_flash_overlay.set_opacity(SPUtil.drpt(_red_flash_overlay.get_opacity(),0,1/25.0f));
		_red_flash_overlay.set_enabled(!SPUtil.flt_cmp_delta(_red_flash_overlay.get_opacity(),0,0.01f));
	
		_fadeout_overlay.set_opacity(SPUtil.drpt(_fadeout_overlay.get_opacity(),_fadeout_overlay_target?1:0,1/15.0f));
		_fadeout_overlay.set_enabled(!SPUtil.flt_cmp_delta(_fadeout_overlay.get_opacity(),0,0.01f));
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
	
	public static Vector2 u_pos_to_ui_pos(Vector2 u_pos) {
		Vector3 world_pos = GameMain._context.transform.TransformPoint(u_pos);
		Vector3 screen_pos = GameMain._context._ui_camera.WorldToScreenPoint(world_pos);
		screen_pos.z = SPUtil.vec_sub(GameMain._context._game_ui._root.transform.position,GameMain._context._game_camera.transform.position).z;
		Vector3 ui_z_world_pos = GameMain._context._ui_camera.ScreenToWorldPoint(screen_pos);
		return GameMain._context._game_ui._root.transform.InverseTransformPoint(ui_z_world_pos);
	}
	
}
