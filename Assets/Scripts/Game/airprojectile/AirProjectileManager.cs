using UnityEngine;
using System.Collections.Generic;

public abstract class AirProjectileBase : InAirGameStateUpdateable, SPNodeHierarchyElement, SPHitPolyOwner {
	public virtual void i_update(GameEngineScene g, InAirGameState state) {}
	public virtual void do_remove(GameEngineScene g) {}
	public virtual void add_to_parent(SPNode parent) {}
	public virtual SPHitRect get_hit_rect() { return new SPHitRect(); }
	public virtual SPHitPoly get_hit_poly() { return new SPHitPoly(); }
	public virtual void do_remove() {}
}

public class AirProjectileManager : InAirGameStateUpdateable, GenericPooledObject {
	
	public static AirProjectileManager cons(GameEngineScene g) {
		return (new AirProjectileManager()).i_cons(g);
	}
	
	private SPNode _root;
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("AirProjectileManager");
	}
	public void repool() {
		for (int i = 0; i < _player_projectiles.Count; i++) {
			_player_projectiles[i].do_remove();
		}
		for (int i = 0; i < _enemy_projectiles.Count; i++) {
			_enemy_projectiles[i].do_remove();
		}
		_player_projectiles.Clear();
		_enemy_projectiles.Clear();
		_root.repool();
	}
	
	private List<AirProjectileBase> _player_projectiles = new List<AirProjectileBase>();
	private List<AirProjectileBase> _enemy_projectiles = new List<AirProjectileBase>();
	public AirProjectileManager i_cons(GameEngineScene g) {
		return this;
	}
	
	public void i_update(GameEngineScene g, InAirGameState state) {
		for (int i = 0; i < _player_projectiles.Count; i++) {
			AirProjectileBase itr = _player_projectiles[i];
			itr.i_update(g, state);
		}
		for (int i = 0; i < _enemy_projectiles.Count; i++) {
			AirProjectileBase itr = _enemy_projectiles[i];
			itr.i_update(g, state);
		}
	}
	
	public void debug_draw_hitboxes(SPDebugRender draw) {
		for (int i = 0; i < _player_projectiles.Count; i++) {
			AirProjectileBase itr = _player_projectiles[i];
			draw.draw_hitpoly_owner(itr,new Color(0.8f, 0.2f, 0.2f, 0.5f), new Color(0.8f, 0.2f, 0.2f, 0.8f));
		}
		for (int i = 0; i < _enemy_projectiles.Count; i++) {
			AirProjectileBase itr = _enemy_projectiles[i];
			draw.draw_hitpoly_owner(itr,new Color(0.8f, 0.2f, 0.2f, 0.5f), new Color(0.8f, 0.2f, 0.2f, 0.8f));
		}
	}
	
	public void add_player_projectile(AirProjectileBase proj) {
		_player_projectiles.Add(proj);
	}
	
	public void add_enemy_projectile() {
	}
	
}
