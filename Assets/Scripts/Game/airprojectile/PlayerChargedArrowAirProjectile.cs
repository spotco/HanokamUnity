using UnityEngine;
using System.Collections.Generic;

public class PlayerChargedArrowAirProjectile : AirProjectileBase, GenericPooledObject {
	
	public static PlayerChargedArrowAirProjectile cons(Vector2 pos, Vector2 dir, float vel) {
		return (ObjectPool.inst().generic_depool<PlayerChargedArrowAirProjectile>()).i_cons(pos,dir,vel);
	}
	
	private SPNode _root;
	private SPSprite _sprite;
	private SPSpriteAnimator _animator;
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("PlayerChargedArrowAirProjectile");
		_sprite = SPSprite.cons_sprite_texkey_texrect(RTex.HANOKA_EFFECTS,FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"arrow_charge_shot_000.png"));
		_sprite.set_manual_sort_z_order(GameAnchorZ.Player_FX);
		_animator = SPSpriteAnimator.cons(_sprite).add_anim("play", FileCache.inst().get_rects_list(RTex.HANOKA_EFFECTS,"arrow_charge_shot_00%d.png",0,4), 3).play_anim("play");
		_root.add_child(_sprite);
	}
	public void repool() {
		_animator = null;
		_root.repool();
	}
	
	private Vector2 _vel;
	private float _ct;
	private FlashEvery _trail_spawn_ct;
	
	private HashSet<int> _hit_ids = new HashSet<int>();
	
	private PlayerChargedArrowAirProjectile i_cons(Vector2 pos, Vector2 dir, float vel) {
		_root.set_u_pos(pos);
		_vel = SPUtil.vec_scale(dir.normalized,vel);
		_ct = 100;
		_root.set_rotation(SPUtil.dir_ang_deg(_vel.x,_vel.y));
		_trail_spawn_ct = FlashEvery.cons(2.2f);
		_hit_ids.Clear();
		return this;
	}
	
	public override void i_update(GameEngineScene g, InAirGameState state) {
		_animator.i_update();
		_root.set_rotation(SPUtil.dir_ang_deg(_vel.x,_vel.y));
		_root.set_u_pos(SPUtil.vec_add(_root.get_u_pos(),SPUtil.vec_scale(_vel,SPUtil.dt_scale_get())));
		_ct -= SPUtil.dt_scale_get();
		
		if (_root._u_x < SPUtil.get_horiz_world_bounds()._min-500 || _root._u_x > SPUtil.get_horiz_world_bounds()._max+500) {
			_ct = 0;
		}
		
		_trail_spawn_ct.i_update(g);
		if (_trail_spawn_ct.do_flash()) {
			g.add_particle(SPConfigAnimParticle.cons()
			    .set_texture(TextureResource.inst().get_tex(RTex.HANOKA_EFFECTS))
				.set_texrect(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"Arrow-_fire-trail-1.png"))
				.set_scale(1.0f,0.0f)
				.set_ctmax(15)
				.set_alpha(0.8f,0.0f)
				.set_pos(_root._u_x,_root._u_y)
				.set_rotation(_root.rotation())
			    .set_manual_sort_z_order(GameAnchorZ.Player_FX-1)
				.set_normalized_timed_sprite_animator(
					SPTimedSpriteAnimator.cons(null)
						.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"Arrow-_fire-trail-1.png"),0.0f)
						.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"Arrow-_fire-trail-2.png"),0.2f)
						.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"Arrow-_fire-trail-3.png"),0.4f)
						.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"Arrow-_fire-trail-4.png"),0.6f)
						.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"Arrow-_fire-trail-5.png"),0.7f)
				));
		}
		
		for (int i_enemy = 0; i_enemy < state._enemy_manager._active_enemies.Count; i_enemy++) {
			BaseAirEnemy itr_enemy = state._enemy_manager._active_enemies[i_enemy];
			if (!_hit_ids.Contains(itr_enemy.get_id()) && SPHitPoly.polyowners_intersect(this,itr_enemy)) {
				itr_enemy.apply_hit(g,BaseAirEnemyHitType.PersistentProjectile,200,SPUtil.vec_sub(itr_enemy.get_u_pos(),_root.get_u_pos()));
				_hit_ids.Add(itr_enemy.get_id());
				g._camerac.freeze_frame(2);
				g._camerac.camera_shake(new Vector2(-1.5f,1.7f),15,30);
			}
		}
	}
	public override bool should_remove(GameEngineScene g, InAirGameState state) { return _ct <= 0; }
	public override void do_remove() {
		ObjectPool.inst().generic_repool<PlayerChargedArrowAirProjectile>(this);
	}
	public override void add_to_parent(SPNode parent) {
		parent.add_child(_root);
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
			new Vector2(25,100),
			new Vector2(1,1),
			1,
			new Vector2(-30,0)
		);
	}
}