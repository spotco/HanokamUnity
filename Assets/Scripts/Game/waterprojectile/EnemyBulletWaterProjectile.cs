using UnityEngine;
using System.Collections;

public class EnemyBulletWaterProjectile : WaterProjectileBase, GenericPooledObject {
	public static EnemyBulletWaterProjectile cons(GameEngineScene g, DiveGameState state, Vector2 pos, Vector2 dir, float vel) {
		return (ObjectPool.inst().generic_depool<EnemyBulletWaterProjectile>()).i_cons(g, state, pos,dir,vel);
	}
	
	private EnemyBulletSprite _sprite;
	public override void add_to_parent(SPNode parent) { _sprite.add_to_parent(parent); }
	
	public void depool() {
		_sprite = EnemyBulletSprite.cons();
	}
	public void repool() {
		ObjectPool.inst().generic_repool<EnemyBulletSprite>(_sprite);
		_sprite = null;
	}
	
	private Vector2 _vel;
	private float _ct;
	
	private FlashEvery _fade_anim;
	private bool _fade_toggle;
	
	private EnemyBulletWaterProjectile i_cons(GameEngineScene g, DiveGameState state, Vector2 pos, Vector2 dir, float vel) {
		_base_params._pos = pos;
		_vel = SPUtil.vec_scale(dir.normalized,vel);
		_ct = 300;
		_sprite.set_rotation(SPUtil.dir_ang_deg(_vel.x,_vel.y) - 90);
		
		_fade_anim = FlashEvery.cons(5);
		_fade_toggle = false;
		
		return this;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		_sprite.i_update(g);
		_base_params._pos = SPUtil.vec_add(_base_params._pos,SPUtil.vec_scale(_vel,SPUtil.dt_scale_get()));
		//this.apply_offset_to_position();
		
		_ct -= SPUtil.dt_scale_get();
		
		if (SPHitPoly.polyowners_intersect(this,g._player)) {
			EnemyBulletSprite.do_hit_particle(g,_sprite._root.get_u_pos(),true);
			_ct = 0;
			
			if (state._params._dashing || state._params.is_invuln()) {
				g._camerac.freeze_frame(1);
				g._camerac.camera_shake(new Vector2(-1.2f,1.3f),10,20);
				
			} else {
				g._game_ui.do_red_flash();
				g._camerac.freeze_frame(2);
				g._camerac.camera_shake(new Vector2(-1.5f,1.7f),15,30);
				
				state._params._current_breath -= 200;
				state._params._invuln_ct = 25;
				
			}
		}
		if (!SPUtil.get_horiz_world_bounds().extend(100).contains(_base_params._pos.x)) {
			_ct = 0;
		}
		
		if (_ct < 80) {
			_fade_anim.i_update();
			if (_fade_anim.do_flash()) {
				if (_fade_toggle) {
					_sprite.set_opacity(0.75f);
				} else {
					_sprite.set_opacity(0.2f);
				}
				_fade_toggle = !_fade_toggle;
			}
			
		}
		
		_sprite.set_enabled(SPHitRect.hitrect_touch(g.get_viewbox(),this.get_hit_rect()));
	}
	
	public override bool should_remove(GameEngineScene g, DiveGameState state) { return _ct <= 0; }
	public override void do_remove() {
		ObjectPool.inst().generic_repool<EnemyBulletWaterProjectile>(this);
	}
	
	protected override void apply_offset_to_position() {
		_sprite.set_u_pos(_base_params._pos.x,_base_params._pos.y - _env_offset);
	}
	
	public override SPHitRect get_hit_rect() { return _sprite.get_hit_rect(); }
	public override SPHitPoly get_hit_poly() { return _sprite.get_hit_poly(); }
	
}
