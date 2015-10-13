using UnityEngine;
using System.Collections;

public class InDialogueSubUI : GameUISubUI {

	public static InDialogueSubUI cons(UIRoot ui) {
		return (new InDialogueSubUI()).i_cons(ui);
	}
	
	private SPNode _root;
	private InDialogueSubUI i_cons(UIRoot ui) {
		_root = SPNode.cons_node();
		_root.set_name("InDialogueSubUI");
		_root.set_enabled(false);
	
		
		SPSprite dialogue_window_root = SPSprite.cons_sprite_texkey_texrect(RTex.DIALOG_UI,FileCache.inst().get_texrect(RTex.DIALOG_UI,"dialogue_window.png"));
		dialogue_window_root.set_scale_x(SPUtil.fit_to_rect(dialogue_window_root,ui.get_ui_bounds()).x * 0.95f);
		dialogue_window_root.set_scale_y(dialogue_window_root.scale_x());
		dialogue_window_root.set_anchor_point(0,0);
		float padding = dialogue_window_root.texrect().size.x * 0.025f;
		dialogue_window_root.set_u_pos(ui.get_ui_bounds()._x1 + padding,ui.get_ui_bounds()._y1 + padding);
		dialogue_window_root.set_name("dialogue_window_root");
		dialogue_window_root.gameObject.layer = RLayer.get_layer(RLayer.UI);
		_root.add_child(dialogue_window_root);
		
		
		SPSprite dialogue_window_header = SPSprite.cons_sprite_texkey_texrect(RTex.DIALOG_UI,FileCache.inst().get_texrect(RTex.DIALOG_UI,"dialogue_window_header.png"));
		dialogue_window_header.set_anchor_point(0,0);
		dialogue_window_header.set_u_pos(SPUtil.vec_add(SPUtil.pct_of_obj(dialogue_window_root,0,1),new Vector2(-10,-10)));
		dialogue_window_header.set_name("dialogue_window_header");
		dialogue_window_header.gameObject.layer = RLayer.get_layer(RLayer.UI);
		dialogue_window_root.add_child(dialogue_window_header);
		
		return this;
	}

	public override void i_update(GameEngineScene g) {
	
	}
	
	public override bool should_show() { 
		return base.should_show() && (GameMain._context.get_top_scene() as GameEngineScene).get_top_game_state().get_state() == GameStateIdentifier.InDialogue;
	}
	public override void on_show() { _root.set_enabled(true); }
	public override void on_hide() { _root.set_enabled(false); }
	public override void add_to_parent(SPNode parent) { parent.add_child(_root); }
}
