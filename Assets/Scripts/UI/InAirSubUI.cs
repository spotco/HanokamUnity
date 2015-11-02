using UnityEngine;
using System.Collections.Generic;

public class InAirSubUI : GameUISubUI {
	public static InAirSubUI cons(UIRoot ui) {
		return (new InAirSubUI()).i_cons(ui);
	}
	
	private SPNode _root;
	private InAirSubUI_EnemyWarningUI _enemy_warnings;
	private InAirSubUI_PlayerHealthUI _player_health_ui;
	
	private InAirSubUI i_cons(UIRoot ui) {
		_root = SPNode.cons_node();
		_root.set_name("InAirSubUI");
		_root.set_enabled(false);
		
		_enemy_warnings = InAirSubUI_EnemyWarningUI.cons(ui);
		_enemy_warnings.add_to_parent(_root);
		
		_player_health_ui = InAirSubUI_PlayerHealthUI.cons(ui);
		_player_health_ui.add_to_parent(_root);
		
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
		InAirGameState inair_state;
		if (!SPUtil.cond_cast<InAirGameState>(g.get_top_game_state(),out inair_state)) return;
		
		_enemy_warnings.i_update(g, inair_state);
		_player_health_ui.i_update(g, inair_state);
	}
	
	public override bool should_show() {
		if (base.should_show()) {
			GameEngineScene g = GameMain._context.get_top_scene() as GameEngineScene;
			return g.get_top_game_state().get_state() == GameStateIdentifier.InAir;
		}
		return false;	
	}
}

public class InAirSubUI_EnemyWarningUI : InAirGameStateUpdateable, SPNodeHierarchyElement {
	public static InAirSubUI_EnemyWarningUI cons(UIRoot ui) {
		return (new InAirSubUI_EnemyWarningUI()).i_cons(ui);
	}
	
	private SPNode _root;
	private SPParticleSystem<EnemyWarningUIParticle> _active_warnings;
	private InAirSubUI_EnemyWarningUI i_cons(UIRoot ui) {
		_root = SPNode.cons_node();
		_root.set_name("_enemy_warning_ui");
		
		_active_warnings = SPParticleSystem<EnemyWarningUIParticle>.cons();
		
		return this;
	}
	public void add_warning(EnemyWarningUIParticle warning) {
		_active_warnings.add_particle(warning);
		warning.add_to_parent(_root);
	}
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	
	
	private HashSet<int> __active_warning_ids = new HashSet<int>();
	public void i_update(GameEngineScene g, InAirGameState state) {
		__active_warning_ids.Clear();
		for (int i = 0; i < _active_warnings.list().Count; i++) {
			EnemyWarningUIParticle itr = _active_warnings.list()[i];
			__active_warning_ids.Add(itr._id);
		}
		for (int i = 0; i < state._enemy_manager._queued_spawn_enemies.Count; i++) {
			QueuedSpawnAirEnemy itr = state._enemy_manager._queued_spawn_enemies[i];
			int itr_id = itr._enemy.get_id();
			if (!__active_warning_ids.Contains(itr_id)) {
				this.add_warning(EnemyWarningUIParticle.cons(state, itr_id));
				__active_warning_ids.Add(itr_id);
			}
		}
		_active_warnings.i_update(state);
	}
}

public class EnemyWarningUIParticle : SPParticle, SPNodeHierarchyElement, GenericPooledObject {
	
	public static EnemyWarningUIParticle cons(InAirGameState state, int id) {
		return (ObjectPool.inst().generic_depool<EnemyWarningUIParticle>()).i_cons(state,id);
	}
	
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("EnemyWarningUIParticle");
		_img = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_EFFECTS,FileCache.inst().get_texrect(RTex.ENEMY_EFFECTS,"Enemy Warning.png"));
		_img.set_layer(RLayer.UI);
		_img.set_scale(0.5f);
		_root.add_child(_img);
	}
	
	public void repool() {
		_root.repool();
		_root = null;
		_img = null;
	}
	
	private SPNode _root;
	private SPSprite _img;
	public int _id;
	private bool _is_active;
	private EnemyWarningUIParticle i_cons(InAirGameState state, int id) {
		_id = id;
		_is_active = true;
		
		_img.set_scale(1.0f);
		_img.set_opacity(0.0f);
		
		return this;
	}
	
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }

	public void i_update(System.Object context) {
		InAirGameState state;
		if (!SPUtil.cond_cast<InAirGameState>(context, out state)) return;
		QueuedSpawnAirEnemy tar = null;
		for (int i = 0; i < state._enemy_manager._queued_spawn_enemies.Count; i++) {
			QueuedSpawnAirEnemy itr = state._enemy_manager._queued_spawn_enemies[i];
			if (itr._enemy.get_id() == _id) {
				tar = itr;
				break;
			}
		}
		
		if (tar == null) {
			_is_active = false;
		} else {
			_is_active = true;
			float t = tar._ct / tar._ct_max;
			if (t <= 0.8f) {
				_img.set_scale(SPUtil.drpt(_img.scale_x(), 0.5f, 1/6.0f));
				_img.set_opacity(SPUtil.drpt(_img.get_opacity(), 0.8f, 1/6.0f));
			} else {
				_img.set_scale(SPUtil.drpt(_img.scale_x(), 1.0f, 1/6.0f));
				_img.set_opacity(SPUtil.drpt(_img.get_opacity(), 0, 1/6.0f));
			}
			_img.set_u_pos(SPUtil.float_random(-1,1),SPUtil.float_random(-2,2));
			
			Vector2 c_pos = tar._pos;
			Vector2 u_pos = GameCameraController.c_pos_to_u_pos(c_pos);
			Vector2 ui_pos = UIRoot.u_pos_to_ui_pos(u_pos);
			_root.set_u_pos(SPUtil.vec_add(ui_pos,new Vector2(0,35)));
		}
	}
	
	public bool should_remove(System.Object context) {
		return !_is_active;
	}
	public void do_remove(System.Object context) {
		ObjectPool.inst().generic_repool<EnemyWarningUIParticle>(this);
	}
}
