using UnityEngine;
using System.Collections;

public class SmallEnemyBasicWaterEnemyHitEffect : BasicWaterEnemyHitEffect {
	public static SmallEnemyBasicWaterEnemyHitEffect cons() { return new SmallEnemyBasicWaterEnemyHitEffect(); }
	public void apply_hit(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		BasicWaterEnemyComponentUtility.HitParams hit_params = BasicWaterEnemyComponentUtility.HitParams.cons_default();
		hit_params._player_vel = state._params._vel;
		hit_params._enemy_vel = enemy.get_calculated_velocity();
		hit_params._enemy_to_player_elasticity_coef = 0.7f;
		BasicWaterEnemyComponentUtility.small_enemy_apply_hit(g,state,enemy,hit_params);
	}
}

public class ShootBulletsEveryBasicWaterEnemyComponent : BasicWaterEnemyComponent {
	
	public static ShootBulletsEveryBasicWaterEnemyComponent cons(float delay) {
		return (new ShootBulletsEveryBasicWaterEnemyComponent()).i_cons(delay);
	}
	
	private FlashEvery _trigger;
	private ShootBulletsEveryBasicWaterEnemyComponent i_cons(float delay) {
		_trigger = FlashEvery.cons(delay);
		_trigger._time = _trigger._max_time;
		return this;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		_trigger.i_update();
		if (_trigger.do_flash()) {
			for (float i = 0; i < 360.0f; i += (360f/6.0f)) {
				state._projectiles.add_enemy_projectile(EnemyBulletWaterProjectile.cons(g,state,enemy._params._pos,SPUtil.ang_deg_dir(i+enemy.get_root().rotation()),5));
			}
		}
	}
}

public class PufferBasicWaterEnemy : BasicWaterEnemy, 
	VisionRangeTriggerBasicWaterEnemyComponent.Delegate, 
	DelayTriggerBasicWaterEnemyComponent.Delegate,
	ImmTriggerBasicWaterEnemyComponent.Delegate
	{
	public static PufferBasicWaterEnemy cons(GameEngineScene g, PatternEntry2Pt entry) {
		return (ObjectPool.inst().generic_depool<PufferBasicWaterEnemy>()).i_cons(g, entry);
	}
	
	public override void do_remove() {
		ObjectPool.inst().generic_repool<PufferBasicWaterEnemy>(this);
	}
	
	private PufferEnemySprite _img;
	public override void depool() {
		base.depool();
		this.get_root().set_name("PufferBasicWaterEnemy");
		_img = PufferEnemySprite.cons();
		_img.add_to_parent(this.get_root());
		_img.set_manual_sort_z_order(GameAnchorZ.Enemy_InAir);
	}
	
	public override void repool() {
		_img.repool();
		base.repool();
	}
	
	private bool _is_follow_player;
	private bool _should_do_notice_bullets;
	private BasicWaterEnemy.Mode _last_mode;
	
	private FlashEvery _flashcount;
	private PufferBasicWaterEnemy i_cons(GameEngineScene g, PatternEntry2Pt entry) {
		base.i_cons();
		_flashcount = FlashEvery.cons(15);
		_params._pos = new Vector2(entry._start.x, entry._start.y);
		
		this.add_component_for_mode(Mode.Moving, TwoPointSwimBasicWaterEnemyComponent.cons(
			entry._start, 
			entry._pt1, 
			entry._pt2,
			entry._speed
		));
		this.add_component_for_mode(Mode.Moving, VisionRangeTriggerBasicWaterEnemyComponent.cons(this,30,650));
		this.add_component_for_mode(Mode.Stunned, KnockbackStunBasicWaterEnemyComponent.cons());
		this.add_component_for_mode(Mode.StunEnded, ImmTriggerBasicWaterEnemyComponent.cons(this));
		this.add_component_for_mode(Mode.Delayed, DelayTriggerBasicWaterEnemyComponent.cons(this, 20));
		this.add_component_for_mode(Mode.Chase, ChasePlayerTest1BasicWaterEnemyComponent.cons());
		this.add_component_for_mode(Mode.Chase, ShootBulletsEveryBasicWaterEnemyComponent.cons(SPUtil.float_random(500,700)));
		this.add_hiteffect(SmallEnemyBasicWaterEnemyHitEffect.cons());
		
		_is_follow_player = false;
		_should_do_notice_bullets = true;
		_last_mode = this.get_current_mode();
		
		return this;
	}
	
	public void vision_saw_player_triggered(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		_is_follow_player = true;
		
		Vector2 pos_delta = SPUtil.vec_sub(g._player.get_u_pos(),enemy.get_u_pos()).normalized;
		Vector2 up;
		float offset;
		if (SPUtil.shortest_angle(this.get_root().rotation(),0) > 0) {
			up = SPUtil.vec_cross(SPUtil.vec_z,pos_delta).normalized;
			offset = 180 + 50;
		} else {
			up = SPUtil.vec_cross(pos_delta,SPUtil.vec_z).normalized;
			offset = -180;
		}
		if (_should_do_notice_bullets) {
			g.add_particle(EnemyNoticeParticle.cons(SPUtil.vec_add(this.get_u_pos(), SPUtil.vec_add(SPUtil.vec_scale(pos_delta,125),SPUtil.vec_scale(up,50))), SPUtil.dir_ang_deg(pos_delta.x,pos_delta.y) + offset));
		}
		
		this.transition_to_mode(g,Mode.Delayed);
	}
	
	public void delay_trigger(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		//vision seen
		if (_should_do_notice_bullets) {
			for (float i = 0; i < 360.0f; i += (360f/6.0f)) {
				state._projectiles.add_enemy_projectile(EnemyBulletWaterProjectile.cons(g,state,enemy._params._pos,SPUtil.ang_deg_dir(i+enemy.get_root().rotation()),5));
			}
		}
		this.transition_to_mode(g,Mode.Chase);
	}
	
	public void imm_trigger(GameEngineScene g, DiveGameState state, BasicWaterEnemy enemy) {
		//stun end
		_should_do_notice_bullets = false;
		if (!_is_follow_player) {
			this.vision_saw_player_triggered(g,state,enemy);
		} else {
			this.transition_to_mode(g,Mode.Chase);
		}
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		base.i_update(g,state);
		
		if (this._params._stun_ct > 0) {
			_img.play_anim(PufferEnemySprite.ANIM_HURT);
		} else if (!_is_follow_player) {
			_img.play_anim(PufferEnemySprite.ANIM_IDLE);
		} else {
			_img.play_anim(PufferEnemySprite.ANIM_FOLLOW);
		}
		_img.i_update(g);
		
		Vector4 img_color = _img.color();
		img_color.y = SPUtil.drpt(img_color.y,1,1/8.0f);
		img_color.z = SPUtil.drpt(img_color.z,1,1/8.0f);
		switch(this.get_current_mode()) {
		case(Mode.Stunned):{
			_flashcount.i_update();
			if (_flashcount.do_flash()) 	{
				img_color.y = 0;
				img_color.z = 0;
			}
		} break;
		default: {} break;
		}
		_img.set_color(img_color);
		_img.set_enabled(SPHitRect.hitrect_touch(g.get_viewbox(),this.get_hit_rect()));
		
		_last_mode = this.get_current_mode();
	}
	
	public override SPHitRect get_hit_rect() {
		return SPHitPoly.hitpoly_to_bounding_hitrect(
			this.get_hit_poly(),
			new Vector2(-10,-10),
			new Vector2(10,10)
		);
	}
	
	public override SPHitPoly get_hit_poly() {
		return SPHitPoly.cons_with_basis_offset(
			new Vector2(this.get_root()._u_x, this.get_root()._u_y),
			this.get_root().rotation(),
			new Vector2(55,90),
			new Vector2(1,1),
			1,
			new Vector2(-10,0)
		);
	}
}
