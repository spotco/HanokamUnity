using UnityEngine;
using System.Collections;

public class OnGroundSubUI : GameUISubUI {
	public static OnGroundSubUI cons(UIRoot ui) {
		return (new OnGroundSubUI()).i_cons(ui);
	}
	
	private SPNode _root;
	
	private SPNode _gameplay_ui_root;
	private OnGroundSubUI_JumpChargeUI _jump_charge_ui_root;
				
	private OnGroundSubUI i_cons(UIRoot ui) {
		_root = SPNode.cons_node();
		_root.set_name("OnGroundSubUI");
		_root.set_enabled(false);
		
		_jump_charge_ui_root = OnGroundSubUI_JumpChargeUI.cons(ui,_root);
		
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
		OnGroundGameState ground_state;
		if (SPUtil.cond_cast<OnGroundGameState>(g.get_top_game_state(),out ground_state)) {
			if (ground_state._current_state == OnGroundGameState.State.JumpCharge) {
				_jump_charge_ui_root.i_update(g);
				_jump_charge_ui_root.set_enabled(true);
				
			} else {
				_jump_charge_ui_root.set_enabled(false);
				
			}
		}
	}
	
	public override bool should_show() {
		if (base.should_show()) {
			GameEngineScene g = GameMain._context.get_top_scene() as GameEngineScene;
			return g.get_top_game_state().get_state() == GameStateIdentifier.OnGround;
		}
		return false;	
	}
}

public class OnGroundSubUI_JumpChargeUI : SPGameUpdateable {
	public static OnGroundSubUI_JumpChargeUI cons(UIRoot ui, SPNode parent) {
		return (new OnGroundSubUI_JumpChargeUI()).i_cons(ui,parent);
	}
	
	private SPNode _root;
	private SPSprite _charge_bar_bg,_charge_bar_fill;
	
	private OnGroundSubUI_JumpChargeUI i_cons(UIRoot ui, SPNode parent) {
		_root = SPNode.cons_node();
		parent.add_child(_root);
		_root.set_name("_jumpcharge_ui_root");
		
		_charge_bar_bg = SPSprite.cons_sprite_texkey_texrect(
			RTex.HUD_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"jump_charge_bar.png")
		);
		_root.add_child(_charge_bar_bg);
		_charge_bar_bg.set_anchor_point(0,0);
		_charge_bar_bg.set_u_pos(ui.get_ui_bounds()._x1,ui.get_ui_bounds()._y1 + 5);
		_charge_bar_bg.set_scale_x((ui.get_ui_bounds()._x2 - ui.get_ui_bounds()._x1)/_charge_bar_bg.texrect().size.x);
		_charge_bar_bg.set_scale_y(0.5f);
		_charge_bar_bg.gameObject.layer = RLayer.get_layer(RLayer.UI);
		_charge_bar_bg.set_color(SPUtil.color_from_bytes(45,28,34,255));
		
		_charge_bar_fill = SPSprite.cons_sprite_texkey_texrect(
			RTex.HUD_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"jump_charge_bar.png")
		);
		_root.add_child(_charge_bar_fill);
		_charge_bar_fill.set_anchor_point(_charge_bar_bg.anchorpoint().x,_charge_bar_bg.anchorpoint().y);
		_charge_bar_fill.set_u_pos(_charge_bar_bg._u_x,_charge_bar_bg._u_y);
		_charge_bar_fill.set_scale_x(_charge_bar_bg.scale_x());
		_charge_bar_fill.gameObject.layer = RLayer.get_layer(RLayer.UI);
		_charge_bar_fill.set_scale_y(_charge_bar_bg.scale_y());		
		
		this.set_bar_pct(0.0f);
		
		return this;
	}
	
	private float __last_pct;
	private void set_bar_pct(float pct) {
		if (pct == __last_pct) return;
		__last_pct = pct;
		Rect bg_rect = _charge_bar_bg.texrect();
		_charge_bar_fill.set_tex_rect(new Rect(
			bg_rect.position.x,
			bg_rect.position.y,
			bg_rect.width * pct,
			bg_rect.height
		));
		_charge_bar_fill.set_color(Vector4.Lerp(
			SPUtil.color_from_bytes(220,100,100,180),
			SPUtil.color_from_bytes(252,220,220,180),
			pct
		));
	}
	
	public OnGroundSubUI_JumpChargeUI set_enabled(bool val) {
		_root.set_enabled(val);
		return this;
	}
	
	public void i_update(GameEngineScene g) {
		OnGroundGameState ground_state;
		if (SPUtil.cond_cast<OnGroundGameState>(g.get_top_game_state(),out ground_state)) {
			this.set_bar_pct(ground_state._params._jump_charge_t);
		}
	}
}
