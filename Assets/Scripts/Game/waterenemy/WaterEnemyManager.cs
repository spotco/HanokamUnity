using UnityEngine;
using System.Collections.Generic;

public abstract class BaseWaterEnemy : DiveGameStateUpdateable, DiveReturnGameStateUpdateable, SPNodeHierarchyElement, SPHitPolyOwner {
	public virtual void i_update(GameEngineScene g, DiveGameState state) {}
	public virtual void i_update(GameEngineScene g, DiveReturnGameState state) {}
	public virtual void do_remove(GameEngineScene g) {}
	public virtual void add_to_parent(SPNode parent) {}
	public virtual SPHitRect get_hit_rect() { return new SPHitRect(); }
	public virtual SPHitPoly get_hit_poly() { return new SPHitPoly(); }
	public virtual void do_remove() {}
}

public class WaterEnemyManager : DiveGameStateUpdateable, GenericPooledObject {	
	public static WaterEnemyManager cons(GameEngineScene g) {
		return (ObjectPool.inst().generic_depool<WaterEnemyManager>()).i_cons(g);
	}
	
	private SPNode _root;
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("WaterEnemyRoot");
	}
	public void repool() {
		for (int i = 0; i < _enemies.Count; i++) {
			_enemies[i].do_remove();
		}
		_enemies.Clear();
		_root.repool();
	}
	
	private List<BaseWaterEnemy> _enemies = new List<BaseWaterEnemy>();
	public WaterEnemyManager i_cons(GameEngineScene g) {
	
		PatternFile tmp = FileCache.inst().get_patternfile(RPattern.TEST_1);
		for (int i = 0; i < tmp._2pt_entries.Count; i++) {
			PatternEntry2Pt itr_2pt = tmp._2pt_entries[i];
		}
		
		return this;
	}
	
	public void i_update(GameEngineScene g, DiveGameState state) {
		for (int i = _enemies.Count-1; i >= 0; i--) {
			BaseWaterEnemy itr = _enemies[i];
			itr.i_update(g, state);
		}
	}

	public void i_update(GameEngineScene g, DiveReturnGameState state) {
		for (int i = _enemies.Count-1; i >= 0; i--) {
			BaseWaterEnemy itr = _enemies[i];
			itr.i_update(g, state);
		}
	}

	public void debug_draw_hitboxes(SPDebugRender draw) {
		for (int i = _enemies.Count-1; i >= 0; i--) {
			BaseWaterEnemy itr = _enemies[i];
			draw.draw_hitpoly_owner(itr,new Color(0.8f, 0.2f, 0.2f, 0.5f), new Color(0.8f, 0.2f, 0.2f, 0.8f));
		}
	}
	
	public void add_enemy(BaseWaterEnemy enemy) {
		_enemies.Add(enemy);
		enemy.add_to_parent(_root);
	}

}
