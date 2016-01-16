using UnityEngine;
using System.Collections.Generic;

public abstract class IWaterEnemy : DiveGameStateUpdateable, DiveReturnGameStateUpdateable, SPNodeHierarchyElement {
	public virtual void on_added_to_manager(GameEngineScene g) {}
	public virtual void i_update(GameEngineScene g, DiveGameState state) {}
	public virtual void i_update(GameEngineScene g, DiveReturnGameState state) {}
	public virtual void apply_offset(float offset) {}
	public virtual bool should_remove() { return false; }
	public virtual void do_remove() {}
	public virtual void add_to_parent(SPNode parent) {}
	public virtual Vector2 get_u_pos() { return Vector2.zero; }
	
	public virtual void debug_draw_hitboxes(SPDebugRender draw) {}
}

public class WaterEnemyManager : DiveGameStateUpdateable, GenericPooledObject {	
	public static WaterEnemyManager cons(GameEngineScene g, DiveGameState state) {
		return (ObjectPool.inst().generic_depool<WaterEnemyManager>()).i_cons(g,state);
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
	
	private List<IWaterEnemy> _enemies = new List<IWaterEnemy>();
	public WaterEnemyManager i_cons(GameEngineScene g, DiveGameState state) {
	
		List<PatternFile> levels_to_load = new List<PatternFile>() {
			FileCache.inst().get_patternfile(RPattern.GENPATTERN_1),
			FileCache.inst().get_patternfile(RPattern.GENPATTERN_1),
			FileCache.inst().get_patternfile(RPattern.GENPATTERN_2),
			FileCache.inst().get_patternfile(RPattern.GENPATTERN_1),
			FileCache.inst().get_patternfile(RPattern.GENPATTERN_1),
			
			//FileCache.inst().get_patternfile(RPattern.TEST_1)
			
		};
		Vector2 cur_offset = new Vector2(0,-2000);
		
		for (int i = 0; i < levels_to_load.Count; i++) {
			PatternFile itr = levels_to_load[i];
			this.load_patternfile_with_offset(g,itr,cur_offset);
			
			cur_offset.y -= itr._section_height;
			cur_offset.y -= itr._spacing_bottom;
		}
		state._params.set_ground_depth(g,cur_offset.y-1500);
		
		return this;
	}
	
	private void load_patternfile_with_offset(GameEngineScene g, PatternFile patternfile, Vector2 group_offset) {
		for (int i = 0; i < patternfile._2pt_entries.Count; i++) {
			PatternEntry2Pt itr_2pt = patternfile._2pt_entries[i];
			if (itr_2pt._val == "puffer") {
				this.add_enemy(g,PufferBasicWaterEnemy.cons(g,itr_2pt,group_offset));
			} else if (itr_2pt._val == "bubble") {
				this.add_enemy(g,BubbleBasicWaterEnemy.cons(g,itr_2pt,group_offset));
			} else if (itr_2pt._val == "spike") {
				this.add_enemy(g,SpikeBasicWaterEnemy.cons(g,itr_2pt,group_offset));
			} else {
				Debug.LogError(SPUtil.sprintf("Unknown 2pt({0})",itr_2pt._val));
			}
		}
		for (int i = 0; i < patternfile._1pt_entries.Count; i++) {
			PatternEntry1Pt itr_1pt = patternfile._1pt_entries[i];
			if (itr_1pt._val == "bubble") {
				this.add_enemy(g,BubbleBasicWaterEnemy.cons(g,itr_1pt,group_offset));
			} else if (itr_1pt._val == "spike") {
				this.add_enemy(g,SpikeBasicWaterEnemy.cons(g,itr_1pt,group_offset));
			} else {
				Debug.LogError(SPUtil.sprintf("Unknown 1pt({0})",itr_1pt._val));
			}
		}
	}
	
	public void set_offset_y(float offset_y) {
		for (int i = _enemies.Count-1; i >= 0; i--) {
			IWaterEnemy itr = _enemies[i];
			itr.apply_offset(offset_y);
		}
	}
	
	public void i_update(GameEngineScene g, DiveGameState state) {
		for (int i = _enemies.Count-1; i >= 0; i--) {
			IWaterEnemy itr = _enemies[i];
			itr.i_update(g, state);
			if (itr.should_remove()) {
				this.remove_enemy(g,itr);
			}
		}
	}

	public void i_update(GameEngineScene g, DiveReturnGameState state) {
		for (int i = _enemies.Count-1; i >= 0; i--) {
			IWaterEnemy itr = _enemies[i];
			itr.i_update(g, state);
		}
	}

	public void debug_draw_hitboxes(SPDebugRender draw) {
		for (int i = _enemies.Count-1; i >= 0; i--) {
			IWaterEnemy itr = _enemies[i];
			itr.debug_draw_hitboxes(draw);
		}
	}
	
	public void add_enemy(GameEngineScene g, IWaterEnemy enemy) {
		_enemies.Add(enemy);
		enemy.add_to_parent(_root);
		enemy.on_added_to_manager(g);
	}
	
	public void remove_enemy(GameEngineScene g, IWaterEnemy enemy) {
		for (int i = 0; i < _enemies.Count; i++) {
			if (_enemies[i] == enemy) {
				_enemies.RemoveAt(i);
				enemy.do_remove();
				return;
			}
		}
	}

}
