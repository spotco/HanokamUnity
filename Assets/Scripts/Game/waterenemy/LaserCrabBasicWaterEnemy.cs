using UnityEngine;
using System.Collections.Generic;

public class FaceDirectionLaserCrabBasicWaterEnemyComponent : BasicWaterEnemyComponent {
	public static FaceDirectionLaserCrabBasicWaterEnemyComponent cons(Vector2 pos, Vector2 dir) {
		return (new FaceDirectionLaserCrabBasicWaterEnemyComponent()).i_cons(pos,dir);
	}
	public Vector3 _starting_pos;
	public Vector3 _starting_dir;
	private FaceDirectionLaserCrabBasicWaterEnemyComponent i_cons(Vector2 pos, Vector2 dir) {
		_starting_pos = pos;
		_starting_dir = dir;
		return this;
	}
	public override void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		enemy._params._pos = _starting_pos;
		enemy.get_root().set_rotation(SPUtil.dir_ang_deg(_starting_dir.x,_starting_dir.y)-90);
		if (SPHitPoly.polyowners_intersect(enemy,g._player)) {
			enemy.get_hit_effect().apply_hit(g,state,enemy);
		}
	}
}

public class LaserCrabBasicWaterEnemy :
	BasicWaterEnemy,
	VisionRangeTriggerBasicWaterEnemyComponent.Delegate,
	ImmTriggerBasicWaterEnemyComponent.Delegate
	{

	public static LaserCrabBasicWaterEnemy cons(GameEngineScene g, PatternEntryDirectional entry) {
		return (ObjectPool.inst().generic_depool<LaserCrabBasicWaterEnemy>()).i_cons(g, entry);
	}
	
	public override void do_remove() {
		ObjectPool.inst().generic_repool<LaserCrabBasicWaterEnemy>(this);
	}
	
	private LaserCrabEnemySprite _img;
	public override void depool() {
		base.depool();
		this.get_root().set_name("LaserCrabBasicWaterEnemy");
		_img = LaserCrabEnemySprite.cons();
		_img.add_to_parent(this.get_root());
		_img.set_manual_sort_z_order(GameAnchorZ.Enemy_InAir);
	}
	
	public override void repool() {
		ObjectPool.inst().generic_repool<LaserCrabEnemySprite>(_img);
		_img = null;
		base.repool();
	}
	
	private FaceDirectionLaserCrabBasicWaterEnemyComponent _face_direction;
	private EnemyLaserWaterProjectile _laser;
	
	private LaserCrabBasicWaterEnemy i_cons(GameEngineScene g, PatternEntryDirectional entry) {
		base.i_cons();
		this._current_mode = Mode.UnActivated;
		
		_face_direction = FaceDirectionLaserCrabBasicWaterEnemyComponent.cons(entry._start,entry._dir);
		this.add_component_for_mode(Mode.UnActivated,_face_direction);
		this.add_component_for_mode(Mode.UnActivated,VisionRangeTriggerBasicWaterEnemyComponent.cons(this,35,1200));
		
		this.add_component_for_mode(Mode.Activated,_face_direction);
		
		this.add_component_for_mode(Mode.Stunned, KnockbackStunBasicWaterEnemyComponent.cons());
		this.add_component_for_mode(Mode.StunEnded, ImmTriggerBasicWaterEnemyComponent.cons(this));
		
		this.add_hiteffect(SmallEnemyBasicWaterEnemyHitEffect.cons());
		
		return this;
	}
	
	public void imm_trigger(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		this.transition_to_mode(g,Mode.UnActivated);
	}
	
	public void vision_saw_player_triggered(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		this.transition_to_mode(g,Mode.Activated);
		
		Vector2 laser_start_point = SPUtil.vec_add(SPUtil.vec_scale(this.get_forward_direction(),150),_params._pos);
		_laser = EnemyLaserWaterProjectile.cons(g,state,laser_start_point,_face_direction._starting_dir);
		state._projectiles.add_enemy_projectile(_laser);
	}
	
	private Vector2 get_forward_direction() {
		return SPUtil.ang_deg_dir(this.get_root().rotation()+90);
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		base.i_update(g,state);
		if (_current_mode != Mode.Activated) {
			_laser = null;
		}
		if (_laser != null) {
			_laser.owner_hold();
		}
	}
	
	public override SPHitRect get_hit_rect() { return _img.get_hit_rect(this.get_root().get_u_pos(),this.get_root().rotation()); }
	public override SPHitPoly get_hit_poly() { return _img.get_hit_poly(this.get_root().get_u_pos(),this.get_root().rotation()); }
}
