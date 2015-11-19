using UnityEngine;
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
	private SPText _arrow_count_text;
	
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
		
		_arrow_count_text = SPText.cons_text(
			RTex.DIALOGUE_FONT,
			RFnt.DIALOGUE_FONT,
			SPText.SPTextStyle.cons(
				new Vector4(0.15f,0.15f,0.15f,1),
				new Vector4(0.75f,0.75f,0.75f,1),
				new Vector4(0,0,0,1),0,0
			));
		_arrow_count_text.set_manual_sort_z_order(GameAnchorZ.HUD_BASE+2);
		_arrow_count_text.set_layer(RLayer.UI);
		_arrow_count_text.set_text_anchor(0.5f,0.5f);
		_arrow_count_text.set_scale(0.75f);
		_arrow_count_text.set_u_pos(30,5);
		_arrow_count_text.set_markup_text("0");
		_bar_back.add_child(_arrow_count_text);
		
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
		_arrow_count_text.set_opacity(val);
	}
	
	private int _last_frame_arrow_count;
	private float _time_since_last_arrow_count_change;
	private float _opacity_anim_t;
	
	public void i_update(GameEngineScene g, InAirGameState state) {
		_root.set_u_pos(SPUtil.vec_add(UIRoot.u_pos_to_ui_pos(g._player.get_u_pos()), new Vector2(-50, 145)));
		this.set_bar_fill_pct(state._params._arrow_count / ((float)state._params.get_arrow_count_max()));
		_arrow_count_text.set_markup_text(string.Format("{0}", state._params._arrow_count));
		if (_last_frame_arrow_count != state._params._arrow_count) {
			_time_since_last_arrow_count_change = 0;
		} else {
			_time_since_last_arrow_count_change += SPUtil.dt_scale_get();
		}
		if (_time_since_last_arrow_count_change > 150) {
			_opacity_anim_t = SPUtil.drpt(_opacity_anim_t,0,1/10.0f);
		} else {
			_opacity_anim_t = SPUtil.drpt(_opacity_anim_t,1,1/10.0f);
		}
		this.set_ui_opacity(_opacity_anim_t);
		
		_last_frame_arrow_count = state._params._arrow_count;
	}
}
