using UnityEngine;
using System.Collections.Generic;

public abstract class WaterProjectileBase : DiveGameStateUpdateable, SPNodeHierarchyElement, SPHitPolyOwner {
	public struct Params {
		public Vector2 _pos;
	}
	public WaterProjectileBase.Params _base_params;
	
	public virtual void i_update(GameEngineScene g, DiveGameState state) {}
	public virtual bool should_remove(GameEngineScene g, DiveGameState state) { return true; }
	
	protected float _env_offset;
	public void apply_env_offset(float y) {
		_env_offset = y;
		this.apply_offset_to_position();
	}
	protected virtual void apply_offset_to_position() {}
	
	public virtual void do_remove() {}
	public virtual void add_to_parent(SPNode parent) {}
	public virtual SPHitRect get_hit_rect() { return new SPHitRect(); }
	public virtual SPHitPoly get_hit_poly() { return new SPHitPoly(); }
}

public class WaterProjectileManager : DiveGameStateUpdateable, GenericPooledObject {
	public static WaterProjectileManager cons(GameEngineScene g) {
		return (ObjectPool.inst().generic_depool<WaterProjectileManager>()).i_cons(g);
	}
	
	private SPNode _root;
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("WaterProjectileManager");
	}
	public void repool() {
		for (int i = 0; i < _enemy_projectiles.Count; i++) {
			_enemy_projectiles[i].do_remove();
		}
		_enemy_projectiles.Clear();
		_root.repool();
	}
	
	public List<WaterProjectileBase> _enemy_projectiles = new List<WaterProjectileBase>();
	public WaterProjectileManager i_cons(GameEngineScene g) {
		return this;
	}
	
	public void i_update(GameEngineScene g, DiveGameState state) {
		for (int i = _enemy_projectiles.Count-1; i >= 0; i--) {
			WaterProjectileBase itr = _enemy_projectiles[i];
			itr.i_update(g, state);
			if (itr.should_remove(g, state)) {
				itr.do_remove();
				_enemy_projectiles.RemoveAt(i);
			}
		}
	}
	
	public void debug_draw_hitboxes(SPDebugRender draw) {
		for (int i = 0; i < _enemy_projectiles.Count; i++) {
			WaterProjectileBase itr = _enemy_projectiles[i];
			draw.draw_hitpoly_owner(itr,new Color(0.8f, 0.2f, 0.2f, 0.5f), new Color(0.8f, 0.2f, 0.2f, 0.8f));
		}
	}
	
	private float _last_env_offset_y;
	public void set_env_offset_y(float offset_y) {
		_last_env_offset_y = offset_y;
		for (int i = _enemy_projectiles.Count-1; i >= 0; i--) {
			WaterProjectileBase itr = _enemy_projectiles[i];
			itr.apply_env_offset(offset_y);
		}
	}
	
	public void add_enemy_projectile(WaterProjectileBase proj) {
		_enemy_projectiles.Add(proj);
		proj.add_to_parent(_root);
		proj.apply_env_offset(_last_env_offset_y);
	}
}