using UnityEngine;
using System.Collections.Generic;

public class InAirSubUI_PlayerHealthUI : InAirGameStateUpdateable, SPNodeHierarchyElement {

	public static InAirSubUI_PlayerHealthUI cons(UIRoot ui) {
		return (new InAirSubUI_PlayerHealthUI()).i_cons(ui);
	}
	
	private SPNode _root;
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	
	private SPSprite _background;
	
	private UIRoot _ui_root;
	
	private InAirSubUI_PlayerHealthUI i_cons(UIRoot ui) {
		_ui_root = ui;
		_root = SPNode.cons_node();
		_root.set_name("_player_health_ui");
		
		_background = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET, FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"UI-Heart-Container.png"));
		_background.set_anchor_point(0,1);
		_background.set_scale(0.6f);
		_background.set_opacity(0.85f);
		_background.set_u_pos(ui.get_ui_bounds()._x1,ui.get_ui_bounds()._y2);
		_background.set_manual_sort_z_order(GameAnchorZ.HUD_BASE);
		_background.set_layer(RLayer.UI);
		_root.add_child(_background);
		
		return this;
	}
	
	private int __current_setup_ui_max_health = -1;
	private List<UIHealthHeartSprite> _ui_hearts = new List<UIHealthHeartSprite>();
	private void setup_ui_for_player_params(PlayerCharacter.Params player_params) {
		for (int i = 0; i < _ui_hearts.Count; i++) _ui_hearts[i].repool();
		_ui_hearts.Clear();
		
		Vector2 margin_topleft = new Vector2(20,-20);
		Vector2 heart_size = new Vector2(35,35);
		Vector2 current_position = SPUtil.vec_add(new Vector2(_ui_root.get_ui_bounds()._x1,_ui_root.get_ui_bounds()._y2), margin_topleft);
		
		int z_ord = GameAnchorZ.HUD_BASE;
		for (int i = 0; i < player_params._max_health; i++) {
			UIHealthHeartSprite itr = UIHealthHeartSprite.cons();
			itr.set_u_pos(current_position.x, current_position.y);
			itr.add_to_parent(_root);
			itr.set_manual_sort_z_order(++z_ord);
			_ui_hearts.Add(itr);
			current_position.x += heart_size.x;
		}
		
		__current_setup_ui_max_health = player_params._max_health;
	}
	
	
	public void i_update(GameEngineScene g, InAirGameState state) {
		if (__current_setup_ui_max_health != g._player._params._max_health) {
			this.setup_ui_for_player_params(g._player._params);
		}
		float player_health = state._params._player_health;
		
		UIHealthHeartSprite last = null;
		for (int i = 0; i < _ui_hearts.Count; i++) {
			UIHealthHeartSprite itr = _ui_hearts[i];
			
			if (i < Mathf.Floor(player_health)) {
				itr.set_percent(1);
				last = itr;
			} else if (i == ((int)Mathf.Floor(player_health))) {
				itr.set_percent(player_health - i);
				last = itr;
			} else {
				itr.set_percent(0);
			}
		}
		for (int i = 0; i < _ui_hearts.Count; i++) {
			UIHealthHeartSprite itr = _ui_hearts[i];
			itr.i_update(itr == last);	
		}
	}
	
	public class UIHealthHeartSprite : SPNodeHierarchyElement {
		private SPNode _root;
		public void add_to_parent(SPNode parent) { parent.add_child(_root); }
		private SPSprite _h0, _h1, _h2, _h3, _background;
		
		public static UIHealthHeartSprite cons() { return (new UIHealthHeartSprite()).i_cons(); }
		private UIHealthHeartSprite i_cons() {
			_root = SPNode.cons_node();
			_root.set_name("UIHeartHealthSprite");
			_root.set_scale(0.25f);
			
			_background = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET,FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"UI-Heart-BG.png"));
			_background.set_layer(RLayer.UI);
			_background.set_opacity(0.85f);
			_root.add_child(_background);
			
			Rect heart_full_rect = FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"UI-Heart.png");
			
			_h0 = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET, this.calc_r_i_pos(heart_full_rect,0));
			_h0.set_layer(RLayer.UI);
			_h0.set_name("_h0");
			_h0.set_anchor_point(1,0);
			_root.add_child(_h0);
			
			_h1 = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET, this.calc_r_i_pos(heart_full_rect,1));
			_h1.set_layer(RLayer.UI);
			_h1.set_name("_h1");
			_h1.set_anchor_point(1,1);
			_root.add_child(_h1);
			
			_h2 = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET, this.calc_r_i_pos(heart_full_rect,2));
			_h2.set_layer(RLayer.UI);
			_h2.set_name("_h2");
			_h2.set_anchor_point(0,1);
			_root.add_child(_h2);
			
			_h3 = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET, this.calc_r_i_pos(heart_full_rect,3));
			_h3.set_layer(RLayer.UI);
			_h3.set_name("_h3");
			_h3.set_anchor_point(0,0);
			_root.add_child(_h3);
			
			return this;
		}
		
		public void set_manual_sort_z_order(int val) {
			_background.set_manual_sort_z_order(val);
			_h0.set_manual_sort_z_order(val+1);
			_h1.set_manual_sort_z_order(val+1);
			_h2.set_manual_sort_z_order(val+1);
			_h3.set_manual_sort_z_order(val+1);
		}
		
		private float _anim_theta;
		public void i_update(bool selected) {
			if (selected) {
				_anim_theta = (_anim_theta + SPUtil.dt_scale_get() * 0.05f) % 3.14f;
				_root.set_scale(0.3f +  0.05f * Mathf.Sin(_anim_theta));
					
			} else {
				_anim_theta = 0;
				_root.set_scale(SPUtil.drpt(_root.scale_x(),0.25f,1/5.0f));
			}
		}
		
		public void set_percent(float val) {
			_h0.set_enabled(val > 0);
			_h1.set_enabled(val > 0.25f);
			_h2.set_enabled(val > 0.5f);
			_h3.set_enabled(val > 0.75f);
		}
		
		public void set_u_pos(float x, float y) { _root.set_u_pos(x,y); }
		
		private Rect calc_r_i_pos(Rect r_full, int i) {
			switch (i) {
			case 0: return new Rect(r_full.xMin, r_full.yMin, r_full.width * 0.5f, r_full.height * 0.5f);
			case 1: return new Rect(r_full.xMin, r_full.yMin + r_full.height * 0.5f, r_full.width * 0.5f, r_full.height * 0.5f); 
			case 2: return new Rect(r_full.xMin + r_full.width * 0.5f, r_full.yMin + r_full.height * 0.5f, r_full.width * 0.5f, r_full.height * 0.5f);
			case 3: return new Rect(r_full.xMin + r_full.width * 0.5f, r_full.yMin, r_full.width * 0.5f, r_full.height * 0.5f);
			default: return new Rect();
			}
		}
		
		public void repool() {
			_root.repool();
			_root = null;
			_h0 = _h1 = _h2 = _h3 = _background = null;
		}
	}
	
}
