using UnityEngine;
using System.Collections.Generic;

public class ShopMainUISubUI : ShopUISubUI, SPNodeHierarchyElement {

	public static ShopMainUISubUI cons(UIRoot ui) {
		return (new ShopMainUISubUI()).i_cons(ui);
	}
	
	private SPNode _root;
	
	private SPSprite _background;
	
	private SPSpriteAnimator _flag_1_anim, _flag_2_anim;
	private SPSprite _test_button_left;
	
	private ShopMainUISubUI i_cons(UIRoot ui) {
		_root = SPNode.cons_node();
		_root.set_name("ShopMainUISubUI");
		_root.set_enabled(false);
		
		_background = SPSprite.cons_sprite_texkey_texrect(RTex.SHOP_SHOPUI, FileCache.inst().get_texrect(RTex.SHOP_SHOPUI, "shop_ui_background.png"));
		_background.set_anchor_point(0,0);
		_background.set_u_pos(ui.get_ui_bounds()._x1,ui.get_ui_bounds()._y1);
		_background.set_scale_x(SPUtil.fit_to_rect(_background,ui.get_ui_bounds()).x);
		_background.set_scale_y(_background.scale_x());
		_background.gameObject.layer = RLayer.get_layer(RLayer.UI);
		_background.set_name("_background");
		_root.add_child(_background);
		
		SPSprite background_header = SPSprite.cons_sprite_texkey_texrect(RTex.SHOP_SHOPUI, FileCache.inst().get_texrect(RTex.SHOP_SHOPUI, "shop_ui_header.png"));
		background_header.set_scale_x(SPUtil.fit_to_rect(background_header,ui.get_ui_bounds()).x * SPUtil.inverse_scale(_background).x);
		background_header.set_scale_y(background_header.scale_x());
		background_header.set_anchor_point(0,0);
		background_header.set_u_pos(SPUtil.pct_of_obj(_background,0,1));
		background_header.gameObject.layer = RLayer.get_layer(RLayer.UI);
		background_header.set_name("background_header");
		_background.add_child(background_header);
		
		
		_test_button_left = SPSprite.cons_sprite_texkey_texrect(RTex.SHOP_SHOPUI, FileCache.inst().get_texrect(RTex.SHOP_SHOPUI, "button_element.png"));
		_test_button_left.gameObject.layer = RLayer.get_layer(RLayer.UI);
		_test_button_left.set_name("test_button_left");
		_test_button_left.set_scale(0.85f);
		_test_button_left.set_u_pos(286,27);
		background_header.add_child(_test_button_left);
		
		
		
		SPSprite flag_1 = SPSprite.cons_sprite_texkey_texrect(RTex.FLAGANIM, FileCache.inst().get_texrect(RTex.FLAGANIM, "flag0.png"));
		flag_1.set_anchor_point(0,1);
		flag_1.set_scale(0.6f);
		flag_1.set_u_pos(10,282);
		flag_1.set_name("flag_1");
		flag_1.gameObject.layer = RLayer.get_layer(RLayer.UI);
		
		List<Rect> flag_1_anim_rects_list = FileCache.inst().get_rects_list(RTex.FLAGANIM, "flag%d.png",0,10);
		for (int i = 0; i < 10; i++) flag_1_anim_rects_list.Add(FileCache.inst().get_texrect(RTex.FLAGANIM, "flag0.png"));
		
		_flag_1_anim = SPSpriteAnimator.cons(flag_1)
			.add_anim("play", flag_1_anim_rects_list, 8).play_anim("play");
		_background.add_child(flag_1);
		
		SPSprite flag_2 = SPSprite.cons_sprite_texkey_texrect(RTex.FLAGANIM, FileCache.inst().get_texrect(RTex.FLAGANIM, "flag0.png"));
		flag_2.set_anchor_point(0,1);
		flag_2.set_scale(0.6f);
		flag_2.set_u_pos(163,282);
		flag_2.set_name("flag_2");
		flag_2.gameObject.layer = RLayer.get_layer(RLayer.UI);
		_flag_2_anim = SPSpriteAnimator.cons(flag_2)
			.add_anim("play", flag_1_anim_rects_list, 8).set_anim_i_offset(6).play_anim("play");
		_background.add_child(flag_2);
		
		return this;
	}
	
	public override void add_to_parent(SPNode parent) { parent.add_child(_root); }
	
	public override void on_show() {
		_root.set_enabled(true);
		_test_button_left.set_scale(1);
	}
	
	public override void on_hide() {
		_root.set_enabled(false);
	}
	
	public override void i_update(ShopScene shop) {
		_flag_1_anim.i_update();
		_flag_2_anim.i_update();
		
		if (GameMain._context._controls.get_control_down(ControlManager.Control.MoveRight)) {
			_test_button_left.set_scale(SPUtil.drpt(_test_button_left.scale_x(), 1.35f, 1/10.0f));
		} else {
			_test_button_left.set_scale(SPUtil.drpt(_test_button_left.scale_x(), 0.85f, 1/10.0f));
		}
		
	}
	
	public override bool should_show() {
		return base.should_show();
	}

}
