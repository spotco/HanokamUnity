using UnityEngine;
using System.Collections;

public class VisionRangeTriggerBasicWaterEnemyComponent : BasicWaterEnemyComponent {
	
	public interface Delegate {
		void vision_saw_player_triggered(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy);
	}
	
	public static VisionRangeTriggerBasicWaterEnemyComponent cons(VisionRangeTriggerBasicWaterEnemyComponent.Delegate delegate_obj, float vision_angle = 35, float vision_dist = 450) {
		return (new VisionRangeTriggerBasicWaterEnemyComponent()).i_cons(delegate_obj, vision_angle, vision_dist);
	}
	
	private VisionRangeTriggerBasicWaterEnemyComponent.Delegate _delegate_obj;
	private SPHitPoly _trigger_poly;
	private float _vision_angle;
	private float _vision_dist;
	private float _vision_ct;
	
	private VisionRangeTriggerBasicWaterEnemyComponent i_cons(VisionRangeTriggerBasicWaterEnemyComponent.Delegate delegate_obj, float vision_angle, float vision_dist) {
		_delegate_obj = delegate_obj;
		_vision_angle = vision_angle;
		_vision_dist = vision_dist;
		_vision_ct = 0;
		return this;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {		
		if (GameMain._context._draw_hitboxes) this.update_debug_draw_trigger_poly(g,state,enemy);
		float player_dist = Vector2.Distance(enemy.get_u_pos(),g._player.get_center());
		float player_angle = SPUtil.dir_ang_deg(g._player.get_center().x-enemy.get_u_pos().x,g._player.get_center().y-enemy.get_u_pos().y);
		float facing_dir_ang = enemy.get_root().rotation() + 90;
		
		if (Mathf.Abs(SPUtil.shortest_angle(facing_dir_ang,player_angle)) <= _vision_angle && player_dist < _vision_dist) {
			_vision_ct += SPUtil.dt_scale_get();
			if (_vision_ct > 7) {
				_delegate_obj.vision_saw_player_triggered(g,state,enemy);
			}
			
		} else {
			_vision_ct = 0;
		}
	}
	
	private void update_debug_draw_trigger_poly(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		float facing_dir_ang = enemy.get_root().rotation() + 90;
		Vector2 dir_left = SPUtil.ang_deg_dir(facing_dir_ang - _vision_angle).normalized;
		Vector2 dir_right = SPUtil.ang_deg_dir(facing_dir_ang + _vision_angle).normalized;
		_trigger_poly._pts0 = enemy.get_root().get_u_pos();
		_trigger_poly._pts1 = enemy.get_root().get_u_pos();
		_trigger_poly._pts2 = SPUtil.vec_add(SPUtil.vec_scale(dir_left,_vision_dist),enemy.get_root().get_u_pos());
		_trigger_poly._pts3 = SPUtil.vec_add(SPUtil.vec_scale(dir_right,_vision_dist),enemy.get_root().get_u_pos());
	}
	
	public override void debug_draw_hitboxes(SPDebugRender draw) {
		GameMain._context._debug_render.draw_hit_poly(
			_trigger_poly,
			new Color(0.8f, 0.8f, 0.2f, 0.8f)
		);
	}
	
}
