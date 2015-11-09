using UnityEngine;
using System.Collections.Generic;

public class EnemyBulletProjectile : AirProjectileBase, GenericPooledObject {
	
	public static EnemyBulletProjectile cons(Vector2 pos, Vector2 dir, float vel) {
		return (ObjectPool.inst().generic_depool<EnemyBulletProjectile>()).i_cons(pos,dir,vel);
	}
	
	private SPNode _root;
	public override void add_to_parent(SPNode parent) { parent.add_child(_root); }
	private SPSprite _sprite;
	private SPSpriteAnimator _animator;
	
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("EnemyBulletProjectile");
		_sprite = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_EFFECTS, FileCache.inst().get_texrect(RTex.ENEMY_EFFECTS,"enemy_bullet_normal_000.png"));
		_root.add_child(_sprite);
		_animator = SPSpriteAnimator.cons(_sprite)
			.add_anim("play", 
				FileCache.inst().get_rects_list(RTex.ENEMY_EFFECTS, "enemy_bullet_normal_00%d.png",0,4),4)
			.play_anim("play");
	}
	public void repool() {
		_root.repool();
		_root = null;
		_sprite = null;
	}
	
	private Vector2 _vel;
	private float _ct;
	
	private EnemyBulletProjectile i_cons(Vector2 pos, Vector2 dir, float vel) {
		_root.set_u_pos(pos);
		_vel = SPUtil.vec_scale(dir.normalized,vel);
		_ct = 500;
		_root.set_rotation(SPUtil.dir_ang_deg(_vel.x,_vel.y) - 90);
		return this;
	}
	
	public override void i_update(GameEngineScene g, InAirGameState state) {
		bool kill = false;
		if (SPHitPoly.polyowners_intersect(this,g._player)) {
			bool do_pushback_reverse = true;
			if (state._params._invuln_ct <= 0) {
				if (state._params._player_mode == InAirGameState.Params.PlayerMode.Dash ||
					state._params._player_mode == InAirGameState.Params.PlayerMode.SwordPlant) {
					do_pushback_reverse = false;
					g._camerac.freeze_frame(1);
					g._camerac.camera_shake(new Vector2(-1.2f,1.3f),10,20);
					
				} else {
					g._game_ui.do_red_flash();
					state._params._player_mode = InAirGameState.Params.PlayerMode.Hurt;
					state._params._player_health -= 0.5f;
					state._params._hurt_ct = 25;
					state._params._invuln_ct = 25;
					
					g._camerac.freeze_frame(2);
					g._camerac.camera_shake(new Vector2(-1.5f,1.7f),15,30);
				}				
			}
			
			if (do_pushback_reverse) {
				state._params._upwards_vel = 10;
				Vector2 hit_reverse_dir = SPUtil.vec_sub(g._player.get_u_pos(),_root.get_u_pos()).normalized;
				Vector2 up_dir = new Vector2(0,1);
				state._params._player_c_vel = SPUtil.vec_scale(SPUtil.vec_add(hit_reverse_dir,up_dir).normalized,15);
			}
			kill = true;
		} else {
			for (int i = 0; i < state._projectiles._player_projectiles.Count; i++) {
				AirProjectileBase itr = state._projectiles._player_projectiles[i];
				if (SPHitPoly.polyowners_intersect(this,itr)) {
					kill = true;
				}
			}
		}
		
		if (kill) {
			SPConfigAnimParticle neu_particle = SPConfigAnimParticle.cons()
				.set_texture(TextureResource.inst().get_tex(RTex.PARTICLES_SPRITESHEET))
					.set_texrect(FileCache.inst().get_texrect(RTex.PARTICLES_SPRITESHEET,"grey_particle"))
					.set_pos(_root._u_x,_root._u_y)
					.set_ctmax(15)
					.set_manual_sort_z_order(GameAnchorZ.Player_FX)
					.set_scale(3,6)
					.set_alpha(1.0f,0.0f);
			g.add_particle(neu_particle);
			_ct = 0;
			return;
		}
		
		_animator.i_update();
	
		_root.set_u_pos(SPUtil.vec_add(_root.get_u_pos(),SPUtil.vec_scale(_vel,SPUtil.dt_scale_get())));
		_ct -= SPUtil.dt_scale_get();
		if (_root._u_x < SPUtil.get_horiz_world_bounds()._min-500 || _root._u_x > SPUtil.get_horiz_world_bounds()._max+500) {
			_ct = 0;
		}
	}
	
	public override bool should_remove(GameEngineScene g, InAirGameState state) { return _ct <= 0; }
	public override void do_remove() {
		ObjectPool.inst().generic_repool<EnemyBulletProjectile>(this);
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
			_root.get_u_pos(),
			_root.rotation()+90,
			new Vector2(50,50),
			new Vector2(1,1),
			1,
			new Vector2(0,0)
		);
	}
}