using UnityEngine;
using System.Collections.Generic;

public class DiveSubUI : GameUISubUI {

	public static DiveSubUI cons(UIRoot ui) {
		return (new DiveSubUI()).i_cons(ui);
	}
	
	private SPNode _root;
	private DiveSubUI_BreathBar _breath_bar;
	
	private DiveSubUI i_cons(UIRoot ui) {
		_root = SPNode.cons_node();
		_root.set_name("DiveSubUI");
		_root.set_enabled(false);
		
		_breath_bar = DiveSubUI_BreathBar.cons(ui);
		_breath_bar.add_to_parent(_root);
		
		return this;
	}
	
	public override void add_to_parent(SPNode parent) { parent.add_child(_root); }
	
	public override void on_show() {
		_root.set_enabled(true);
	}
	
	public override void on_hide() {
		_root.set_enabled(false);
	}
	
	public override void i_update(GameEngineScene g) {
		DiveGameState dive_state;
		if (!SPUtil.cond_cast<DiveGameState>(g.get_top_game_state(),out dive_state)) return;
		_breath_bar.i_update(g, dive_state);
	}
	
	public override bool should_show() {
		if (base.should_show()) {
			GameEngineScene g;
			if (SPUtil.cond_cast<GameEngineScene>(GameMain._context.get_top_scene(), out g)) {
				return g.get_top_game_state().get_state() == GameStateIdentifier.Dive;
			}
		}
		return false;	
	}
}

public class DiveSubUI_BreathBar : DiveGameStateUpdateable, SPNodeHierarchyElement {
	public static DiveSubUI_BreathBar cons(UIRoot ui) {
		return (new DiveSubUI_BreathBar()).i_cons(ui);
	}
	private SPNode _root;
	private SPSprite _charge_bar_bg,_charge_bar_fill;
	private SPText _breath_pct_text;
	
	private SPSprite _depth_bg;
	private SPText _depth_text;
	
	private DiveSubUI_BreathBar i_cons(UIRoot ui) {
		_root = SPNode.cons_node();
		_root.set_name("DiveSubUI_BreathBar");
				
		{
			_charge_bar_bg = SPSprite.cons_sprite_texkey_texrect(
				RTex.HUD_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"UI_Air-gauge.png")
			);
			_root.add_child(_charge_bar_bg);
			_charge_bar_bg.set_anchor_point(0,1);
			_charge_bar_bg.set_u_pos(ui.get_ui_bounds()._x1,ui.get_ui_bounds()._y2 - 5);
			_charge_bar_bg.set_scale_x((ui.get_ui_bounds()._x2 - ui.get_ui_bounds()._x1)/_charge_bar_bg.texrect().size.x);
			_charge_bar_bg.set_scale_y(_charge_bar_bg.scale_x() * 2.0f);
			_charge_bar_bg.set_color(new Vector4(0.1f, 0.1f, 0.25f, 0.85f));
			_charge_bar_bg.set_manual_sort_z_order(GameAnchorZ.HUD_BASE);
			_charge_bar_bg.gameObject.layer = RLayer.get_layer(RLayer.UI);
			
			_charge_bar_fill = SPSprite.cons_sprite_texkey_texrect(
				RTex.HUD_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"UI_Air-gauge.png")
			);
			_root.add_child(_charge_bar_fill);
			_charge_bar_fill.set_anchor_point(_charge_bar_bg.anchorpoint().x,_charge_bar_bg.anchorpoint().y);
			_charge_bar_fill.set_u_pos(_charge_bar_bg._u_x,_charge_bar_bg._u_y);
			_charge_bar_fill.set_scale_x(_charge_bar_bg.scale_x());
			_charge_bar_fill.set_scale_y(_charge_bar_bg.scale_y());
			_charge_bar_fill.set_color(new Vector4(1,1,1,0.95f));
			_charge_bar_fill.set_manual_sort_z_order(GameAnchorZ.HUD_BASE+1);
			_charge_bar_fill.gameObject.layer = RLayer.get_layer(RLayer.UI);
			
			_breath_pct_text = SPText.cons_text(RTex.DIALOGUE_FONT, RFnt.DIALOGUE_FONT, SPText.SPTextStyle.cons(
				new Vector4(189/255.0f, 247/255.0f, 255/255.0f, 1.0f), new Vector4(62/255.0f, 56/255.0f, 64/255.0f, 1.0f), new Vector4(0,0,0,0), 0, 0
			));
			_breath_pct_text.set_u_pos(
				ui.get_ui_bounds()._x1,ui.get_ui_bounds()._y2
			);
			_breath_pct_text.set_layer(RLayer.UI);
			_breath_pct_text.set_manual_sort_z_order(GameAnchorZ.HUD_TEXT);
			_breath_pct_text.set_scale(1.4f);
			_breath_pct_text.set_text_anchor(0,1);
			_breath_pct_text.set_markup_text("AIR 100%");
			_root.add_child(_breath_pct_text);
			
			this.set_bar_pct(1.0f);
		}
		
		{
			_depth_bg = SPSprite.cons_sprite_texkey_texrect(
				RTex.HUD_SPRITESHEET,
				FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"UI_Depth-BG.png")
			);
			_depth_bg.set_layer(RLayer.UI);
			_depth_bg.set_anchor_point(0,1);
			_depth_bg.set_u_pos(_charge_bar_bg._u_x,_charge_bar_bg._u_y-_charge_bar_bg.texrect().height*_charge_bar_bg.scale_y());
			_depth_bg.set_color(new Vector4(1,1,1,0.9f));
			_depth_bg.set_scale(0.35f);
			_root.add_child(_depth_bg);
		
			_depth_text = SPText.cons_text(RTex.DIALOGUE_FONT, RFnt.DIALOGUE_FONT, SPText.SPTextStyle.cons(
				new Vector4(62/255.0f, 48/255.0f, 33/255.0f, 1.0f),new Vector4(239/255.0f, 213/255.0f, 241/255.0f, 1.0f),new Vector4(0,0,0,0),0,0
			));
			_depth_text.set_layer(RLayer.UI);
			_depth_text.set_manual_sort_z_order(GameAnchorZ.HUD_TEXT);
			_depth_text.set_scale(0.65f);
			_depth_text.set_text_anchor(0,0.5f);
			_depth_text.set_u_pos(
				_depth_bg._u_x + _depth_bg.texrect().width * 0.44f * _depth_bg.scale_x(), 
				_depth_bg._u_y - _depth_bg.texrect().height * 0.5f * _depth_bg.scale_y()
			);
			_depth_text.set_markup_text("0m");
			_root.add_child(_depth_text);
			
			SPText depth_indicator = SPText.cons_text(RTex.DIALOGUE_FONT, RFnt.DIALOGUE_FONT, SPText.SPTextStyle.cons(
				new Vector4(62/255.0f, 48/255.0f, 33/255.0f, 1.0f),new Vector4(239/255.0f, 213/255.0f, 241/255.0f, 1.0f),new Vector4(0,0,0,0),0,0
			));
			depth_indicator.set_layer(RLayer.UI);
			depth_indicator.set_manual_sort_z_order(GameAnchorZ.HUD_TEXT);
			depth_indicator.set_scale(0.35f);
			depth_indicator.set_text_anchor(1,0.5f);
			depth_indicator.set_u_pos(
				_depth_bg._u_x + _depth_bg.texrect().width * 0.44f * _depth_bg.scale_x(), 
				_depth_bg._u_y - _depth_bg.texrect().height * 0.5f * _depth_bg.scale_y()
			);
			depth_indicator.set_markup_text("DEPTH");
			_root.add_child(depth_indicator);
		}
		
		return this;
	}
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	public void i_update(GameEngineScene g, DiveGameState state) {
		if (state._params._mode == DiveGameState.Mode.Gameplay) {
			_root.set_enabled(true);
			this.set_bar_pct(state._params._current_breath/state._params.MAX_BREATH());
			_depth_text.set_markup_text(string.Format("{0}m",(int)(-state._params._player_pos.y/100.0f)));
		} else {
			_root.set_enabled(false);
		}

	}
	
	private float __last_pct = -1;
	private void set_bar_pct(float pct) {
		pct = Mathf.Clamp(pct,0,1);
		if (pct == __last_pct) return;
		__last_pct = pct;
		Rect bg_rect = _charge_bar_bg.texrect();
		_charge_bar_fill.set_tex_rect(new Rect(
			bg_rect.position.x,
			bg_rect.position.y,
			bg_rect.width * pct,
			bg_rect.height
		));
		_breath_pct_text.set_markup_text(string.Format("AIR {0}%",(int)(pct*100)));
	}
	
}
