using UnityEngine;
using System.Collections;

public class ShopScene : SPScene {

	public static ShopScene cons() {
		return (new ShopScene()).i_cons();
	}
	
	private SPNode _root;
	
	public ShopScene i_cons() {
		_root = SPNode.cons_node();
		_root.set_name("ShopScene");
		
		GameMain._context._camerac.SetGameEngineSceneDefaults();
		
		return this;
	}

	public override void i_update(){
	
		if (Input.GetKeyUp(KeyCode.P)) {
			GameMain._context.pop_scene();
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
