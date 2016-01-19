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
	private SPSprite _charge_bar_fill;
	
	private DiveSubUI_Gameplay i_cons(UIRoot ui) {
	
		_ui_alpha = SPAlphaGroup.cons();
	
		_root = SPNode.cons_node();
		_root.set_name("DiveSubUI_BreathBar");
		
		SPHitRect charge_bar_frame_rect;
		{
			SPSprite charge_bar_frame = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET,FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"neu_ui_air_gauge_frame.png"));
			charge_bar_frame.set_layer(RLayer.UI);
			charge_bar_frame.set_manual_sort_z_order(0);
			charge_bar_frame.set_name("charge_bar_frame");
			SPUILayout.layout_sprite(charge_bar_frame,ui.get_ui_bounds(),new SPUILayout.SpriteLayout() {
				_anchor_point = new Vector2(0f,1f),
				_nparent_origin = new Vector2(0f,0.925f),
				_nparent_placement = new Vector2(0.65f,1f),
				_p_origin_offset = new Vector2(10f,0f),
				_p_placement_offset = new Vector2(0f,-10f)
			});
			charge_bar_frame_rect = SPUILayout.get_layout_rect(charge_bar_frame);
			_root.add_child(charge_bar_frame);
			_ui_alpha.add_sprite(charge_bar_frame);
		}
		{
			_charge_bar_fill = SPSprite.cons_sprite_texkey_texrect(RTex.BLANK,SPUtil.texture_default_rect(RTex.BLANK));
			_charge_bar_fill.set_layer(RLayer.UI);
			_charge_bar_fill.set_manual_sort_z_order(-1);
			_charge_bar_fill.set_name("_charge_bar_fill");
			_charge_bar_fill.set_color(SPUtil.color_from_bytes(127,220,255,255));
			SPUILayout.layout_sprite(_charge_bar_fill,charge_bar_frame_rect,new SPUILayout.SpriteLayout() {
				_anchor_point = new Vector2(0f,1f),
				_nparent_origin = new Vector2(0f,0f),
				_nparent_placement = new Vector2(1f,1f),
				_p_origin_offset = new Vector2(10f,7f),
				_p_placement_offset = new Vector2(-10f,-7f)
					
			});
			_root.add_child(_charge_bar_fill);
			_ui_alpha.add_sprite(_charge_bar_fill);
		}
		this.set_bar_pct(0.5f);
		
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
			_ui_alpha.add_sprite(notification_frame);
		}
		
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
			_ui_alpha.add_sprite(depth_frame);
		}
		
		_ui_alpha.set_alpha_mult(0.75f);
		
		
		return this;
	}
	
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	public void i_update(GameEngineScene g, DiveGameState state) {
		_root.set_enabled(true);
	}
	
	private float __last_pct = -1;
	private void set_bar_pct(float pct) {
		pct = Mathf.Clamp(pct,0,1);
		if (pct == __last_pct) return;
		__last_pct = pct;
		Rect bg_rect = _charge_bar_fill.texrect();
		_charge_bar_fill.set_tex_rect(new Rect(
			bg_rect.position.x,
			bg_rect.position.y,
			bg_rect.width * pct,
			bg_rect.height
		));
	}
	

	
}
