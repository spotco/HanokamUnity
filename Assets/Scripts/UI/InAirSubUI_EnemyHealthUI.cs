using UnityEngine;
using System.Collections.Generic;

public class InAirSubUI_EnemyHealthUI : InAirGameStateUpdateable, SPNodeHierarchyElement {
	public static InAirSubUI_EnemyHealthUI cons(UIRoot ui) {
		return (new InAirSubUI_EnemyHealthUI()).i_cons(ui);
	}
	
	private SPNode _root;
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	private SPParticleSystem<EnemyHealthBarUIParticle> _active_healthbars;
	private InAirSubUI_EnemyHealthUI i_cons(UIRoot ui) {
		_root = SPNode.cons_node();
		_root.set_name("_enemy_health_ui");
		
		_active_healthbars = SPParticleSystem<EnemyHealthBarUIParticle>.cons();
		
		return this;
	}
	
	private void add_particle(EnemyHealthBarUIParticle particle) {
		_active_healthbars.add_particle(particle);
		particle.add_to_parent(_root);
	}
	
	private HashSet<int> __active_enemy_ids = new HashSet<int>();
	public void i_update(GameEngineScene g, InAirGameState state) {
		__active_enemy_ids.Clear();
		for (int i = 0; i < _active_healthbars.list().Count; i++) {
			EnemyHealthBarUIParticle itr = _active_healthbars.list()[i];
			__active_enemy_ids.Add(itr._enemy_id);
		}
		for (int i = 0; i < state._enemy_manager._active_enemies.Count; i++) {
			IAirEnemy itr = state._enemy_manager._active_enemies[i];
			int itr_id = itr.get_id();
			if (!__active_enemy_ids.Contains(itr_id)) {
				this.add_particle(EnemyHealthBarUIParticle.cons(itr_id));
				__active_enemy_ids.Add(itr_id);
			}
		}
		_active_healthbars.i_update(state);
	}
}

public class EnemyHealthBarUIParticle : SPParticle, GenericPooledObject, SPNodeHierarchyElement {
	
	public static EnemyHealthBarUIParticle cons(int enemy_id) {
		return (ObjectPool.inst().generic_depool<EnemyHealthBarUIParticle>()).i_cons(enemy_id);
	}
	
	private SPNode _root;
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	private SPSprite _bar_back, _bar_fill;
	private Rect _bar_fill_rect;
	private bool _is_active;
	public int _enemy_id;
	
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("EnemyHealthBarUIParticle");
		_root.set_scale(0.5f);
		
		_bar_back = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET, FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"enemy_small_health_bar_empty.png"));
		_bar_back.set_anchor_point(0,0.5f);
		_bar_back.set_opacity(0.85f);
		_bar_back.set_manual_sort_z_order(GameAnchorZ.HUD_BASE);
		_bar_back.set_layer(RLayer.UI);
		_root.add_child(_bar_back);
		
		_bar_fill_rect = FileCache.inst().get_texrect(RTex.HUD_SPRITESHEET,"enemy_small_health_bar_full.png");
		
		_bar_fill = SPSprite.cons_sprite_texkey_texrect(RTex.HUD_SPRITESHEET, _bar_fill_rect);
		_bar_fill.set_anchor_point(0,0.5f);
		_bar_fill.set_opacity(0.85f);
		_bar_fill.set_manual_sort_z_order(GameAnchorZ.HUD_BASE+1);
		_bar_fill.set_layer(RLayer.UI);
		_root.add_child(_bar_fill);
	}
	
	public void repool() {
		_root.repool();
		_root = null;
		_bar_back = null;
		_bar_fill = null;
	}
	
	private float _opacity_anim_t;
	private EnemyHealthBarUIParticle i_cons(int enemy_id) {
		_is_active = true;
		_enemy_id = enemy_id;
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
	
	public void i_update(System.Object context) {
		InAirGameState state;
		if (!SPUtil.cond_cast<InAirGameState>(context, out state)) return;
		
		IAirEnemy tar = null;
		for (int i = 0; i < state._enemy_manager._active_enemies.Count; i++) {
			IAirEnemy itr = state._enemy_manager._active_enemies[i];
			if (itr.get_id() == _enemy_id) {
				tar = itr;
				break;
			}
		}
		
		if (tar != null) {
			_root.set_u_pos(SPUtil.vec_add(UIRoot.u_pos_to_ui_pos(tar.get_u_pos()), tar.get_health_bar_offset()));
			this.set_bar_fill_pct(tar.get_health_bar_percent());
			
			float clamped_pct = Mathf.Clamp(tar.get_health_bar_percent(),0,1);
			if (clamped_pct == 1 || clamped_pct == 0) {
				_opacity_anim_t = SPUtil.drpt(_opacity_anim_t,0,1/10.0f);
			} else {
				_opacity_anim_t = SPUtil.drpt(_opacity_anim_t,1,1/10.0f);
			}
			this.set_ui_opacity(_opacity_anim_t);
			
		} else {
			_is_active = false;
		}
	}
	
	public bool should_remove(System.Object context) {
		return !_is_active;
	}
	public void do_remove(System.Object context) {
		ObjectPool.inst().generic_repool<EnemyHealthBarUIParticle>(this);
	}
	
	
}