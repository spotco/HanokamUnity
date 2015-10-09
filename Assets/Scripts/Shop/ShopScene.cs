﻿using UnityEngine;
using System.Collections;

public interface SPShopUpdateable {
	void i_update(ShopScene g);
}

public class ShopScene : SPScene {

	public static ShopScene cons() {
		return (new ShopScene()).i_cons();
	}
	
	private SPNode _root;
	
	private BGShop _bg_shop;
	
	public ShopScene i_cons() {
		_root = SPNode.cons_node();
		_root.set_name("ShopScene");
		
		GameMain._context._camerac.SetShopSceneDefaults();
		
		_bg_shop = BGShop.cons();
		_bg_shop.add_to_parent(_root);
		
		return this;
	}

	public override void i_update(){
		_bg_shop.i_update(this);
		
		if (Input.GetKeyUp(KeyCode.P)) {
			GameMain._context.pop_scene();
			return;
		}
	
	}
	public override void set_enabled(bool val) {
		_root.set_enabled(val);
	}
	public override void do_remove() {
		_root.repool();
		_root = null;
	}
}
