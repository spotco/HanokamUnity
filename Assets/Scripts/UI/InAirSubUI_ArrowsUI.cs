﻿using UnityEngine;
using System.Collections;

public class InAirSubUI_ArrowsUI : InAirGameStateUpdateable, SPNodeHierarchyElement {

	public static InAirSubUI_ArrowsUI cons(UIRoot ui) {
		return (new InAirSubUI_ArrowsUI()).i_cons(ui);
	}
	
	private SPNode _root;
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	private SPSprite _bar_back, _bar_fill;
	private Rect _bar_fill_rect;
	private UIRoot _ui_root;
	
	private InAirSubUI_ArrowsUI i_cons(UIRoot ui) {
		_ui_root = ui;
		_root = SPNode.cons_node();
		_root.set_name("_arrows_ui");
		_root.set_scale(0.5f);
		
		_bar_back = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET, FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"player_arrows_hud_empty.png"));
		_bar_back.set_anchor_point(0,0.5f);
		_bar_back.set_opacity(0.85f);
		_bar_back.set_manual_sort_z_order(GameAnchorZ.HUD_BASE);
		_bar_back.set_layer(RLayer.UI);
		_root.add_child(_bar_back);
		
		_bar_fill_rect = FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"player_arrows_hud_full.png");
		
		_bar_fill = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET, _bar_fill_rect);
		_bar_fill.set_anchor_point(0,0.5f);
		_bar_fill.set_opacity(0.85f);
		_bar_fill.set_manual_sort_z_order(GameAnchorZ.HUD_BASE+1);
		_bar_fill.set_layer(RLayer.UI);
		_root.add_child(_bar_fill);
		
		this.set_bar_fill_pct(0.5f);
		this.set_ui_opacity(1.0f);
		
		return this;
	}
	
	private void set_bar_fill_pct(float val) {
		Rect fill_cpy = _bar_fill_rect;
		fill_cpy.width = fill_cpy.width * val;
		_bar_fill.set_tex_rect(fill_cpy);
	}
	
	private void set_ui_opacity(float val) {
		_bar_back.set_opacity(0.85f * val);
		_bar_fill.set_opacity(_bar_back.get_opacity());
	}
	
	public void i_update(GameEngineScene g, InAirGameState state) {
		_root.set_u_pos(SPUtil.vec_add(UIRoot.u_pos_to_ui_pos(g._player.get_u_pos()), new Vector2(-50, 145)));
		
		this.set_bar_fill_pct(state._params._arrow_count / ((float)state._params.get_arrow_count_max()));
	}
}
