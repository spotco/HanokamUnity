using UnityEngine;
using System.Collections.Generic;

public class PlayerArrowAirProjectile : AirProjectileBase, GenericPooledObject {

	public static PlayerArrowAirProjectile cons(Vector2 pos, Vector2 dir, float vel) {
		return (ObjectPool.inst().generic_depool<PlayerArrowAirProjectile>()).i_cons(pos,dir,vel);
	}
	
	private SPNode _root;
	private SPSprite _sprite, _sprite_outline, _trail;
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("PlayerArrowAirProjectile");
		_sprite = SPSprite.cons_sprite_texkey_texrect(RTex.HANOKA_EFFECTS,FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"Arrow Normal NoOutline.png"));
		_sprite.set_anchor_point(0.5f,0.5f);
		_sprite.set_manual_sort_z_order(GameAnchorZ.Player_Arrow);
		
		_sprite_outline = SPSprite.cons_sprite_texkey_texrect(RTex.HANOKA_EFFECTS,FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"Arrow Normal.png"));
		_sprite_outline.set_anchor_point(0.5f,0.5f);
		_sprite_outline.set_manual_sort_z_order(GameAnchorZ.Player_FX);
		
		_trail = SPSprite.cons_sprite_texkey_texrect(RTex.HANOKA_EFFECTS,FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"arrow_trail.png"));
		_trail.set_anchor_point(1,0.5f);
		_trail.set_u_pos(-60,0);
		_trail.set_manual_sort_z_order(GameAnchorZ.Player_FX-1);
		
		_current_mode = Mode.Flying;
		_root.set_scale(0.625f);
		
		_root.add_child(_sprite);
		_root.add_child(_sprite_outline);
		_root.add_child(_trail);
	}
	public void repool() {
		_stuck_enemy = null;
		_root.repool();
	}
	
	public enum Mode {
		Flying,
		Stuck,
		Falling
	}
	
	private Vector2 _vel;
	private float _ct, _trail_tar_alpha;
	private Mode _current_mode;
	private Vector2 _stuck_offset;
	private Vector2 _fall_vel;
	private IAirEnemy _stuck_enemy;
	private float _initial_vel;
	
	private PlayerArrowAirProjectile i_cons(Vector2 pos, Vector2 dir, float vel) {
		_root.set_u_pos(pos);
		_initial_vel = vel;
		_vel = SPUtil.vec_scale(dir.normalized,vel);
		_ct = 500;
		_trail_tar_alpha = 0;
		_trail.set_opacity(0);
		_sprite_outline.set_opacity(0);
		_root.set_rotation(SPUtil.dir_ang_deg(_vel.x,_vel.y));
		return this;
	}
	
	public override void i_update(GameEngineScene g, InAirGameState state) {
		switch (_current_mode) {
		case Mode.Flying: {
			_sprite_outline.set_opacity(SPUtil.drpt(_sprite_outline.get_opacity(),1,1/20.0f));
			
			List<IAirEnemy> air_enemies = state._enemy_manager._active_enemies;
			for (int i = 0; i < air_enemies.Count; i++) {
				IAirEnemy itr = air_enemies[i];
				if (SPHitPoly.polyowners_intersect(this,itr)) {
					_stuck_offset = SPUtil.vec_sub(_root.get_u_pos(),itr.get_u_pos());
					_current_mode = Mode.Stuck;
					_ct = 200;
					_stuck_enemy = itr;
					itr.apply_hit(g, BaseAirEnemyHitType.StickyProjectile, 200, _vel.normalized);
					g._camerac.freeze_frame(2);
					g._camerac.camera_shake(new Vector2(-1.5f,1.7f),15,30);
					break;
				}
			}
			
			_vel.y += -0.35f * SPUtil.dt_scale_get();
			float vel_mag_cur = _vel.magnitude;
			_vel = SPUtil.vec_scale(_vel.normalized,Mathf.Min(vel_mag_cur,_initial_vel));
			_root.set_rotation(SPUtil.dir_ang_deg(_vel.x,_vel.y));
			_root.set_u_pos(SPUtil.vec_add(_root.get_u_pos(),SPUtil.vec_scale(_vel,SPUtil.dt_scale_get())));
			_ct -= SPUtil.dt_scale_get();
			if (_ct > 485 || _vel.magnitude < 10) {
				_trail_tar_alpha = 0;
			} else {
				_trail_tar_alpha = 1;
			}
			_trail.set_opacity(SPUtil.drpt(_trail.get_opacity(),_trail_tar_alpha,1/10.0f));
			if (_root._u_x < SPUtil.get_horiz_world_bounds()._min-500 || _root._u_x > SPUtil.get_horiz_world_bounds()._max+500) {
				_ct = 0;
			}
		} break;
		case Mode.Stuck: {
			_ct -= SPUtil.dt_scale_get();
			_sprite.set_opacity(SPUtil.y_for_point_of_2pt_line(new Vector2(0,0),new Vector2(200,1),_ct));
			_sprite_outline.set_opacity(SPUtil.drpt(_sprite_outline.get_opacity(),0,1/5.0f));
			_trail.set_opacity(SPUtil.drpt(_trail.get_opacity(),0,1/5.0f));
			if (_stuck_enemy != null && _stuck_enemy.is_alive()) {
				_root.set_u_pos(SPUtil.vec_add(_stuck_enemy.get_u_pos(),_stuck_offset));
			} else {
				_current_mode = Mode.Falling;
				_stuck_enemy = null;
				_ct = (_ct/200.0f)*50.0f;
				_vel.y = 0;
			}
		} break;
		case Mode.Falling: {
			_sprite_outline.set_opacity(SPUtil.drpt(_sprite_outline.get_opacity(),0,1/5.0f));
			_trail.set_opacity(SPUtil.drpt(_trail.get_opacity(),0,1/5.0f));
			_root.set_rotation(SPUtil.drpt(_root.rotation(), _root.rotation() + SPUtil.shortest_angle(_root.rotation(),-90), 1/10.0f));
			_sprite.set_opacity(SPUtil.y_for_point_of_2pt_line(new Vector2(0,0),new Vector2(50,1),_ct));
			
			_ct -= SPUtil.dt_scale_get();
			_sprite.set_opacity(SPUtil.y_for_point_of_2pt_line(new Vector2(0,0),new Vector2(50,1),_ct));
			_vel.y -= 0.25f * SPUtil.dt_scale_get();
			_root._u_y += _vel.y * SPUtil.dt_scale_get();
			
		} break;
		}
	}
	public override bool should_remove(GameEngineScene g, InAirGameState state) { return _ct <= 0; }
	public override void do_remove() {
		ObjectPool.inst().generic_repool<PlayerArrowAirProjectile>(this);
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
			new Vector2(20,65),
			new Vector2(1,1),
			1,
			new Vector2(15,0)
		);
	}
}
