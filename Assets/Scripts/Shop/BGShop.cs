using UnityEngine;
using System.Collections.Generic;

public class BGShop : SPShopUpdateable, SPNodeHierarchyElement {

	public static BGShop cons() {
		return (new BGShop()).i_cons();
	}
	
	private SPNode _root;
	private SpriterNode _shopkeeper;
	private SpriterNode _player;
	
	private SPSpriteAnimator _lamp_anim;
	
	public BGShop i_cons() {
		_root = SPNode.cons_node();
		_root.set_name("BGShop");
		
		SPSprite shop_bg = SPSprite.cons_sprite_texkey_texrect(RTex.SHOP_SHOPBG,FileCache.inst().get_texrect(RTex.SHOP_SHOPBG,"shop_bg.png"));
		shop_bg.set_name("shop_bg");
		_root.add_child(shop_bg);
		shop_bg.set_manual_sort_z_order(GameAnchorZ.SHOP_BG);
		
		{
			SPSprite fg_obj = SPSprite.cons_sprite_texkey_texrect(RTex.SHOP_SHOPBG,FileCache.inst().get_texrect(RTex.SHOP_SHOPBG,"shop_fg_1.png"));
			fg_obj.set_name("fg_1");
			_root.add_child(fg_obj);
			fg_obj.set_manual_sort_z_order(GameAnchorZ.SHOP_FG);
			fg_obj.set_u_pos(-36,187);
			fg_obj.set_u_z(-30);
			fg_obj.set_scale(1.5f);
		}
		{
			SPSprite fg_obj = SPSprite.cons_sprite_texkey_texrect(RTex.SHOP_SHOPBG,FileCache.inst().get_texrect(RTex.SHOP_SHOPBG,"shop_fg_2.png"));
			fg_obj.set_name("fg_2");
			_root.add_child(fg_obj);
			fg_obj.set_manual_sort_z_order(GameAnchorZ.SHOP_FG);
			fg_obj.set_u_pos(340,-228);
			fg_obj.set_u_z(-115);
			fg_obj.set_scale(3);
		}
		{
			SPSprite fg_obj = SPSprite.cons_sprite_texkey_texrect(RTex.SHOP_SHOPBG,FileCache.inst().get_texrect(RTex.SHOP_SHOPBG,"shop_fg_3.png"));
			fg_obj.set_name("fg_3");
			_root.add_child(fg_obj);
			fg_obj.set_manual_sort_z_order(GameAnchorZ.SHOP_FG);
			fg_obj.set_u_pos(-379,-155);
			fg_obj.set_u_z(-115);
			fg_obj.set_scale(3);
		}
		
		{
			SpriterData data = SpriterData.cons_data_from_spritesheetreaders(new List<SpriterJSONParser> { 
				SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_SHOPKEEP,RTex.SPRITER_SHOPKEEP),
			}, RTex.SPRITER_SHOPKEEP);
			
			_shopkeeper = SpriterNode.cons_spriternode_from_data(data);
			_shopkeeper.p_play_anim("idle",true);
			_shopkeeper.set_manual_sort_z_order(GameAnchorZ.SHOP_SHOPKEEPER);
			_shopkeeper.set_name("_shopkeeper");
			_shopkeeper.set_u_pos(-187,-198);
			_shopkeeper.set_u_z(5);
			
			_shopkeeper.set_rendercam_u_pos(66,35);
			_shopkeeper.set_img_scale(1.5f);
			_shopkeeper.set_img_u_pos(38,40);
			
			_root.add_child(_shopkeeper);
		}
		{
			SpriterData data = SpriterData.cons_data_from_spritesheetreaders(new List<SpriterJSONParser> { 
				SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA,RTex.SPRITER_HANOKA),
				SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA_BOW,RTex.SPRITER_HANOKA_BOW),
				SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA_SWORD,RTex.SPRITER_HANOKA_SWORD)
			}, RTex.SPRITER_HANOKA);
			
			_player = SpriterNode.cons_spriternode_from_data(data);
			_player.p_play_anim("Idle",true);
			_player.set_manual_sort_z_order(GameAnchorZ.SHOP_CHAR);
			_player.set_name("_player");
			_root.add_child(_player);
			
			_player.set_u_pos(170,-207);
			_player.set_img_scale(1.5f);
			_player.set_img_u_pos(19,191);
		}
		
		{
			SPSprite animated_lamp = SPSprite.cons_sprite_texkey_texrect(RTex.SHOP_SHOPBG,FileCache.inst().get_texrect(RTex.SHOP_SHOPBG,"Shop Store Flame Anim0000.png"));
			animated_lamp.set_name("animated_lamp");
			_root.add_child(animated_lamp);
			animated_lamp.set_manual_sort_z_order(GameAnchorZ.SHOP_FG);
			animated_lamp.set_u_pos(-270,93);
			animated_lamp.set_u_z(-18);
			animated_lamp.set_scale(1.75f);
			
			_lamp_anim = SPSpriteAnimator.cons(animated_lamp);
			_lamp_anim.add_anim(
				"play",
				FileCache.inst().get_rects_list(
					RTex.SHOP_SHOPBG,
					"Shop Store Flame Anim000%d.png",
					0,
					8
				),
				6.0f);
			_lamp_anim.play_anim("play");
			
		}
		
		return this;
	}
	
	public void add_to_parent(SPNode parent) {
		parent.add_child(_root);
	}
	
	public void i_update(ShopScene g) {
		_lamp_anim.i_update();
		_shopkeeper.i_update();
		_player.i_update();
	}
	
}
