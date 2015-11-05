using UnityEngine;
using System.Collections.Generic;

public enum BaseAirEnemyHitType {
	StickyProjectile,
	PersistentProjectile,
	SwordPlant,
	SwordDash
}

public abstract class BaseAirEnemy : InAirGameStateUpdateable, SPNodeHierarchyElement, SPHitPolyOwner {
	public virtual void spawn_at_c_position(GameEngineScene g, Vector2 pos) {}
	public virtual void i_update(GameEngineScene g, InAirGameState state) {}
	public virtual void add_to_parent(SPNode parent) {}
	public virtual SPHitRect get_hit_rect() { return new SPHitRect(); }
	public virtual SPHitPoly get_hit_poly() { return new SPHitPoly(); }
	public virtual bool should_remove() { return true; }
	public virtual void do_remove() {}
	public virtual Vector2 get_u_pos() { return Vector2.zero; }
	public virtual bool is_alive() { return false; }
	public virtual void apply_hit(GameEngineScene g, BaseAirEnemyHitType type, float duration, Vector2 dir){}
	public virtual int get_id() { return 0; }
	public virtual float get_health_bar_percent() { return 1.0f; }
	public virtual Vector2 get_health_bar_offset() { return Vector2.zero; }
}

public class QueuedSpawnAirEnemy {
	public BaseAirEnemy _enemy;
	public Vector3 _pos;
	public float _ct;
	public float _ct_max;
	public void do_remove() {
		_enemy.do_remove();
		_enemy = null;
	}
}

public class AirEnemyManager : InAirGameStateUpdateable, GenericPooledObject {	
	public static AirEnemyManager cons(GameEngineScene g) {
		return (ObjectPool.inst().generic_depool<AirEnemyManager>()).i_cons(g);
	}
	
	private SPNode _root;
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("AirEnemyRoot");
	}
	public void repool() {
		_root.repool();
		for (int i = _queued_spawn_enemies.Count-1; i >= 0; i--) {
			_queued_spawn_enemies[i].do_remove();
		}
		for (int i = _active_enemies.Count-1; i >= 0; i--) {
			_active_enemies[i].do_remove();
		}
		
		_queued_spawn_enemies.Clear();
		_active_enemies.Clear();
		
		_root = null;
	}
	
	public List<QueuedSpawnAirEnemy> _queued_spawn_enemies = new List<QueuedSpawnAirEnemy>();
	public List<BaseAirEnemy> _active_enemies = new List<BaseAirEnemy>();
	public AirEnemyManager i_cons(GameEngineScene g) {
		return this;
	}
	
	public void i_update(GameEngineScene g, InAirGameState state) {
		for (int i = _queued_spawn_enemies.Count-1; i >= 0; i--) {
			QueuedSpawnAirEnemy itr = _queued_spawn_enemies[i];
			itr._enemy.i_update(g,state);
			itr._ct += SPUtil.dt_scale_get();
			if (itr._ct > itr._ct_max) {
				_active_enemies.Add(itr._enemy);
				itr._enemy.spawn_at_c_position(g,itr._pos);
				_queued_spawn_enemies.RemoveAt(i);
			}
		}
		for (int i = _active_enemies.Count-1; i >= 0; i--) {
			BaseAirEnemy itr = _active_enemies[i];
			itr.i_update(g,state);
			if (itr.should_remove()) {
				itr.do_remove();
				_active_enemies.RemoveAt(i);
			}
		}
	}
	
	public void debug_draw_hitboxes(SPDebugRender draw) {
		for (int i = _active_enemies.Count-1; i >= 0; i--) {
			BaseAirEnemy itr = _active_enemies[i];
			draw.draw_hitpoly_owner(itr,new Color(0.8f, 0.2f, 0.2f, 0.5f), new Color(0.8f, 0.2f, 0.2f, 0.8f));
		}
	}
	
	public void add_enemy(BaseAirEnemy enemy, Vector2 spawn_position, float delay) {
		_queued_spawn_enemies.Add(new QueuedSpawnAirEnemy() {
			_ct = 0,
			_ct_max = delay,
			_enemy = enemy,
			_pos = spawn_position
		});
		enemy.add_to_parent(_root);
	}
	
}
