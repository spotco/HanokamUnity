using UnityEngine;
using System.Collections;

public class PlayerArrowAirProjectile : AirProjectileBase, GenericPooledObject {

	public static PlayerArrowAirProjectile cons(Vector2 pos, Vector2 dir) {
		return (ObjectPool.inst().generic_depool<PlayerArrowAirProjectile>()).i_cons(pos,dir);
	}
	
	private SPNode _root;
	private SPSprite _sprite, _sprite_outline, _trail;
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("PlayerArrowAirProjectile");
		_sprite = SPSprite.cons_sprite_texkey_texrect(RTex.HANOKA_EFFECTS,FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"Arrow Normal NoOutline.png"));
		_sprite.set_anchor_point(0.5f,0.5f);
		_sprite.set_manual_sort_z_order(GameAnchorZ.Player_FX);
		
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
		_root.repool();
	}
	
	public enum Mode {
		Flying,
		Stuck,
		Falling
	}
	
	private Vector2 _dir;
	private float _ct, _trail_tar_alpha;
	private Mode _current_mode;
	private Vector2 _stuck_offset;
	private Vector2 _fall_vel;
	
	private PlayerArrowAirProjectile i_cons(Vector2 pos, Vector2 dir) {
		_root.set_u_pos(pos);
		_dir = SPUtil.vec_scale(dir.normalized,30);
		_ct = 100;
		_trail_tar_alpha = 0;
		_trail.set_opacity(0);
		_sprite_outline.set_opacity(0);
		_root.set_rotation(SPUtil.dir_ang_deg(dir.x,dir.y));
		return this;
	}
	
	public override void i_update(GameEngineScene g, InAirGameState state) {
		switch (_current_mode) {
		case Mode.Flying: {
			_sprite_outline.set_opacity(SPUtil.drpt(_sprite_outline.get_opacity(),1,1/20.0f));
			// check hit
			_root.set_u_pos(SPUtil.vec_add(_root.get_u_pos(),SPUtil.vec_scale(_dir,SPUtil.dt_scale_get())));
			_ct -= SPUtil.dt_scale_get();
			if (_ct > 85) {
				_trail_tar_alpha = 0;
			} else {
				_trail_tar_alpha = 1;
			}
			_trail.set_opacity(SPUtil.drpt(_trail.get_opacity(),_trail_tar_alpha,1/10.0f));
			if (_root._u_x < SPUtil.get_horiz_world_bounds()._min || _root._u_x > SPUtil.get_horiz_world_bounds()._max) {
				_ct = 0;
			}
		} break;
		case Mode.Stuck: {
		
		} break;
		case Mode.Falling: {
		
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
	public override SPHitRect get_hit_rect() { return new SPHitRect(); }
	public override SPHitPoly get_hit_poly() { return new SPHitPoly(); }

}
