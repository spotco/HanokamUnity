using UnityEngine;
using System.Collections;

public class InDialogueSubUI : GameUISubUI {

	public static InDialogueSubUI cons(UIRoot ui) {
		return (new InDialogueSubUI()).i_cons(ui);
	}
	
	private SPNode _root;
	private SPSprite _dialogue_window_root, _dialogue_window_header;
	
	private SPText _header_text;
	private SPText _body_text;	
	private SPSprite _character_head;
	private float _head_anim_theta;
	
	private SPSprite _next_page_indicator;
	
	private InDialogueSubUI i_cons(UIRoot ui) {
		_root = SPNode.cons_node();
		_root.set_name("InDialogueSubUI");
		_root.set_enabled(false);
		
		_dialogue_window_root = SPSprite.cons_sprite_texkey_texrect(RTex.DIALOG_UI,FileCache.inst().get_texrect(RTex.DIALOG_UI,"dialogue_window.png"));
		_dialogue_window_root.set_scale_x(SPUtil.fit_to_rect(_dialogue_window_root,ui.get_ui_bounds()).x * 0.95f);
		_dialogue_window_root.set_scale_y(_dialogue_window_root.scale_x());
		_dialogue_window_root.set_anchor_point(0,0);
		float padding = _dialogue_window_root.texrect().size.x * 0.025f;
		_dialogue_window_root.set_u_pos(ui.get_ui_bounds()._x1 + padding,ui.get_ui_bounds()._y1 + padding);
		_dialogue_window_root.set_name("_dialogue_window_root");
		_dialogue_window_root.set_layer(RLayer.UI);
		_root.add_child(_dialogue_window_root);
		
		
		_dialogue_window_header = SPSprite.cons_sprite_texkey_texrect(RTex.DIALOG_UI,FileCache.inst().get_texrect(RTex.DIALOG_UI,"dialogue_window_header.png"));
		_dialogue_window_header.set_anchor_point(0,0);
		_dialogue_window_header.set_u_pos(SPUtil.vec_add(SPUtil.pct_of_obj(_dialogue_window_root,0,1),new Vector2(-10,-10)));
		_dialogue_window_header.set_name("dialogue_window_header");
		_dialogue_window_header.gameObject.layer = RLayer.get_layer(RLayer.UI);
		_dialogue_window_root.add_child(_dialogue_window_header);
		
		_body_text = SPText.cons_text(
			RTex.DIALOGUE_FONT,
			RFnt.DIALOGUE_FONT, 
			SPText.SPTextStyle.cons(
				new Vector4(0.44f,0.41f,0.36f,1),
				new Vector4(0.86f,0.78f,0.6f,1),
				new Vector4(0,0,0,1),1.5f,0.5f
			));
		_body_text.add_style("emph",
			SPText.SPTextStyle.cons(
				new Vector4(0.33f,0.32f,0.29f,1.0f),
				new Vector4(0.7f,0.89f,0.9f,1.0f),
				new Vector4(0,0,0,1),7,0.75f));
		_body_text.set_u_pos(50,180);
		_body_text.set_scale(0.5f);
		_body_text.set_text_anchor(0,1);
		_body_text.set_layer(RLayer.UI);
		_body_text.set_manual_sort_z_order(GameAnchorZ.HUD_TEXT);
		_body_text.set_markup_text("Oh hey sis, let me guess...\nOff to [emph]save the world@\nagain or something?");
		_dialogue_window_root.add_child(_body_text);
		
		_header_text = SPText.cons_text(
			RTex.DIALOGUE_FONT,
			RFnt.DIALOGUE_FONT,
			SPText.SPTextStyle.cons(
				new Vector4(0.44f,0.41f,0.36f,1),
				new Vector4(0.86f,0.78f,0.6f,1),
				new Vector4(0,0,0,1),1.5f,0.5f
			));
		_header_text.set_layer(RLayer.UI);
		_header_text.set_u_pos(65,15);
		_header_text.set_scale(0.45f);
		_header_text.set_manual_sort_z_order(GameAnchorZ.HUD_TEXT);
		_header_text.set_markup_text("Test");
		_dialogue_window_header.add_child(_header_text);
		
		_character_head = SPSprite.cons_sprite_texkey_texrect(RTex.DIALOGUE_HEADICONS, FileCache.inst().get_texrect(RTex.DIALOGUE_HEADICONS,"head_fishgirl.png"));
		_character_head.set_u_pos(35,28);
		_character_head.set_manual_sort_z_order(GameAnchorZ.HUD_TEXT);
		_character_head.set_layer(RLayer.UI);
		_dialogue_window_header.add_child(_character_head);
		
		_next_page_indicator = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_EFFECTS, FileCache.inst().get_texrect(RTex.ENEMY_EFFECTS,"Enemy Warning.png"));
		_next_page_indicator.set_u_pos(470,25);
		_next_page_indicator.set_scale(0.4f);
		_next_page_indicator.set_manual_sort_z_order(GameAnchorZ.HUD_TEXT);
		_next_page_indicator.set_layer(RLayer.UI);
		_dialogue_window_root.add_child(_next_page_indicator);
		
		return this;
	}

	public override void i_update(GameEngineScene g) {
		_head_anim_theta += 0.075f * SPUtil.dt_scale_get();
		_character_head.set_rotation(7.5f * Mathf.Sin(_head_anim_theta));
		
		_next_page_indicator.set_opacity(0.5f*(Mathf.Sin(_head_anim_theta)+1));
		
		_header_text.i_update();
		_body_text.i_update();
	}
	
	public override bool should_show() { 
		return base.should_show() && (GameMain._context.get_top_scene() as GameEngineScene).get_top_game_state().get_state() == GameStateIdentifier.InDialogue;
	}
	public override void on_show() { _root.set_enabled(true); }
	public override void on_hide() { _root.set_enabled(false); }
	public override void add_to_parent(SPNode parent) { parent.add_child(_root); }
}
