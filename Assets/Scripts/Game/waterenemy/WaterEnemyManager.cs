using UnityEngine;
using System.Collections.Generic;

public abstract class BaseWaterEnemy : DiveGameStateUpdateable, SPNodeHierarchyElement, SPHitPolyOwner {
	public virtual void i_update(GameEngineScene g, DiveGameState state) {}
	public virtual void add_to_parent(SPNode parent) {}
	public virtual bool should_remove(GameEngineScene g, DiveGameState state) { return false; }
	public virtual void do_remove(GameEngineScene g, DiveGameState state) {}
	public virtual SPHitRect get_hit_rect() { return new SPHitRect(); }
	public virtual SPHitPoly get_hit_poly() { return new SPHitPoly(); }
}

public class WaterEnemyManager : DiveGameStateUpdateable, GenericPooledObject {	
	public static WaterEnemyManager cons(GameEngineScene g) {
		return (ObjectPool.inst().generic_depool<WaterEnemyManager>()).i_cons(g);
	}
	
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("WaterEnemyRoot");
	}
	public void repool() {
		_root.repool();
	}
	
	private SPNode _root;
	private List<BaseWaterEnemy> _enemies = new List<BaseWaterEnemy>();
	public WaterEnemyManager i_cons(GameEngineScene g) {
	
		this.add_enemy(PufferBasicWaterEnemy.cons(
			g, new Vector2(-250,-1500), new Vector2(250,-1500)
		));
	
		return this;
	}
	
	public void i_update(GameEngineScene g, DiveGameState state) {
		for (int i = _enemies.Count-1; i >= 0; i--) {
			BaseWaterEnemy itr = _enemies[i];
			itr.i_update(g, state);
			if (itr.should_remove(g, state)) {
				itr.do_remove(g, state);
				_enemies.RemoveAt(i);
			}	
		}
	}
	
	public void add_enemy(BaseWaterEnemy enemy) {
		_enemies.Add(enemy);
		enemy.add_to_parent(_root);
	}

}
