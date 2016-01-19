using UnityEngine;
using System.Collections.Generic;

public class DiveSubUI : GameUISubUI {

	public static DiveSubUI cons(UIRoot ui) {
		return (new DiveSubUI()).i_cons(ui);
	}
	
	private SPNode _root;
	private DiveSubUI_Gameplay _breath_bar;
	
	private DiveSubUI i_cons(UIRoot ui) {
		_root = SPNode.cons_node();
		_root.set_name("DiveSubUI");
		_root.set_enabled(false);
		
		_breath_bar = DiveSubUI_Gameplay.cons(ui);
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

public class DiveSubUI_Gameplay : DiveGameStateUpdateable, SPNodeHierarchyElement {
	public static DiveSubUI_Gameplay cons(UIRoot ui) {
		return (new DiveSubUI_Gameplay()).i_cons(ui);
	}
	private SPNode _root;
	private SPAlphaGroup _ui_alpha;
	private SPSprite _breath_bar_fill;
	private SPText _depth_text, _breath_pct_text;
	
	private DiveSubUI_Gameplay i_cons(UIRoot ui) {
	
		_ui_alpha = SPAlphaGroup.cons();
	
		_root = SPNode.cons_node();
		_root.set_name("DiveSubUI_BreathBar");
		
		SPHitRect breath_bar_frame_rect;
		{
			SPSprite breath_bar_frame = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET,FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"neu_ui_air_gauge_frame.png"));
			breath_bar_frame.set_layer(RLayer.UI);
			breath_bar_frame.set_manual_sort_z_order(0);
			breath_bar_frame.set_name("breath_bar_frame");
			SPUILayout.layout_sprite(breath_bar_frame,ui.get_ui_bounds(),new SPUILayout.SpriteLayout() {
				_anchor_point = new Vector2(0f,1f),
				_nparent_origin = new Vector2(0f,0.925f),
				_nparent_placement = new Vector2(0.65f,1f),
				_p_origin_offset = new Vector2(10f,0f),
				_p_placement_offset = new Vector2(0f,-10f)
			});
			_root.add_child(breath_bar_frame);
			_ui_alpha.add_element(breath_bar_frame);
		
			breath_bar_frame_rect = SPUILayout.get_layout_rect(breath_bar_frame);
		}
		{
			_breath_bar_fill = SPSprite.cons_sprite_texkey_texrect(RTex.BLANK,SPUtil.texture_default_rect(RTex.BLANK));
			_breath_bar_fill.set_layer(RLayer.UI);
			_breath_bar_fill.set_manual_sort_z_order(-1);
			_breath_bar_fill.set_name("_breath_bar_fill");
			_breath_bar_fill.set_color(SPUtil.color_from_bytes(127,220,255,255));
			SPUILayout.layout_sprite(_breath_bar_fill,breath_bar_frame_rect,new SPUILayout.SpriteLayout() {
				_anchor_point = new Vector2(0f,1f),
				_nparent_origin = new Vector2(0f,0f),
				_nparent_placement = new Vector2(1f,1f),
				_p_origin_offset = new Vector2(10f,7f),
				_p_placement_offset = new Vector2(-10f,-7f)
					
			});
			_root.add_child(_breath_bar_fill);
			_ui_alpha.add_element(_breath_bar_fill);
		}
		{
			_breath_pct_text = SPText.cons_text(RTex.DIALOGUE_FONT, RFnt.DIALOGUE_FONT, SPText.SPTextStyle.cons(
				SPUtil.color_from_bytes(3,37,50,255),
				SPUtil.color_from_bytes(127,220,255,255),
				new Vector4(0,0,0,0),
				0,0
			));
			_breath_pct_text.set_layer(RLayer.UI);
			_breath_pct_text.set_manual_sort_z_order(1);
			_breath_pct_text.set_scale(1.0f);
			_breath_pct_text.set_text_anchor(0.5f,0.5f);
			_breath_pct_text.set_u_pos(
				SPUILayout.sibling_pct_of_obj(breath_bar_frame_rect,new Vector2(1.0f,0.0f))
			);
			_breath_pct_text.set_markup_text("100%");
			_root.add_child(_breath_pct_text);
			_ui_alpha.add_element(_breath_pct_text);
		}
		{
			SPSprite airword_text = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET,FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"airword_text.png"));
			airword_text.set_layer(RLayer.UI);
			airword_text.set_manual_sort_z_order(1);
			airword_text.set_scale(0.6f);
			airword_text.set_u_pos(SPUtil.vec_add(
				SPUILayout.sibling_pct_of_obj(breath_bar_frame_rect,new Vector2(1.0f,0.0f)),
				new Vector2(-100,0)
			));
			_root.add_child(airword_text);
			_ui_alpha.add_element(airword_text);
		}
		
		
		{
			SPSprite notification_frame = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET,FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"neu_ui_notification_frame.png"));
			notification_frame.set_layer(RLayer.UI);
			notification_frame.set_manual_sort_z_order(0);
			notification_frame.set_name("notification_frame");
			SPUILayout.layout_sprite(notification_frame,ui.get_ui_bounds(),new SPUILayout.SpriteLayout() {
				_anchor_point = new Vector2(1f,1f),
				_nparent_origin = new Vector2(0.85f,0.925f),
				_nparent_placement = new Vector2(1f,1f),
				_p_origin_offset = new Vector2(0f,0f),
				_p_placement_offset = new Vector2(0f,0f)
			});
			_root.add_child(notification_frame);
			_ui_alpha.add_element(notification_frame);
		}
		
		SPHitRect depth_frame_rect;
		{
			SPSprite depth_frame = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET,FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"neu_ui_depth_frame.png"));
			depth_frame.set_layer(RLayer.UI);
			depth_frame.set_manual_sort_z_order(0);
			depth_frame.set_name("depth_frame");
			SPUILayout.layout_sprite(depth_frame,ui.get_ui_bounds(),new SPUILayout.SpriteLayout() {
				_anchor_point = new Vector2(0.5f,0f),
				_nparent_origin = new Vector2(0.35f,0f),
				_nparent_placement = new Vector2(0.65f,0.1f),
				_p_origin_offset = new Vector2(0f,0f),
				_p_placement_offset = new Vector2(0f,0f)
			});
			_root.add_child(depth_frame);
			_ui_alpha.add_element(depth_frame);
			
			depth_frame_rect = SPUILayout.get_layout_rect(depth_frame);
		}
		
		{
			_depth_text = SPText.cons_text(RTex.DIALOGUE_FONT, RFnt.DIALOGUE_FONT, SPText.SPTextStyle.cons(
				SPUtil.color_from_bytes(3,37,50,255),
				SPUtil.color_from_bytes(127,220,255,255),
				new Vector4(0,0,0,0),
				0,0
			));
			_depth_text.set_layer(RLayer.UI);
			_depth_text.set_manual_sort_z_order(1);
			_depth_text.set_scale(1.0f);
			_depth_text.set_text_anchor(0.5f,0.5f);
			_depth_text.set_u_pos(
				SPUILayout.sibling_pct_of_obj(depth_frame_rect,new Vector2(0.5f,0.575f))
			);
			_depth_text.set_markup_text("0m");
			_root.add_child(_depth_text);
			_ui_alpha.add_element(_depth_text);
		}
		{
			SPSprite depthword_text = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET,FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"depthword_text.png"));
			depthword_text.set_layer(RLayer.UI);
			depthword_text.set_manual_sort_z_order(1);
			depthword_text.set_scale(0.6f);
			depthword_text.set_u_pos(
				SPUILayout.sibling_pct_of_obj(depth_frame_rect,new Vector2(0.5f,0.15f))
			);
			_root.add_child(depthword_text);
			_ui_alpha.add_element(depthword_text);
		}
		
		_ui_alpha.set_alpha_mult(0);
		
		
		return this;
	}
	
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	public void i_update(GameEngineScene g, DiveGameState state) {
		float tar_alpha = 0;
		if (state._params._mode == DiveGameState.Mode.Gameplay) {
			tar_alpha = 0.865f;			
		}
		float next_alpha = SPUtil.drpt(_ui_alpha.get_alpha(),tar_alpha,1/10.0f);
		if (next_alpha < 0.1f) {
			_root.set_enabled(false);
			_ui_alpha.set_alpha_mult(0);
		} else {
			_root.set_enabled(true);
			_ui_alpha.set_alpha_mult(next_alpha);
		}
		
		this.set_bar_pct(state._params._current_breath/state._params.MAX_BREATH());
		_depth_text.set_markup_text(string.Format("{0}m",(int)(-state._params._player_pos.y/100.0f)));
	}
	
	private float __last_pct = -1;
	private void set_bar_pct(float pct) {
		pct = Mathf.Clamp(pct,0,1);
		if (pct == __last_pct) return;
		__last_pct = pct;
		
		Rect bg_rect = SPUtil.texture_default_rect(_breath_bar_fill.texkey());
		_breath_bar_fill.set_tex_rect(new Rect(
			bg_rect.position.x,
			bg_rect.position.y,
			bg_rect.width * pct,
			bg_rect.height
		));
		_breath_pct_text.set_markup_text(string.Format("{0}%",(int)(pct*100)));
	}
	

	
}
