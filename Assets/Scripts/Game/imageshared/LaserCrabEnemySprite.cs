using UnityEngine;
using System.Collections;

public class LaserCrabEnemySprite : GenericPooledObject, SPNodeHierarchyElement, SPGameUpdateable {
	public static string ANIM_UNDERWATER_IDLE = "UW_IDLE";
	public static string ANIM_UNDERWATER_LASER_CHARGE = "UW_LASER_CHARGE";
	public static string ANIM_UNDERWATER_LASER_FIRING = "UW_LASER_FIRING";
	public static string ANIM_UNDERWATER_NOTICE = "UW_NOTICE";
	public static string ANIM_UNDERWATER_STUN = "UW_STUN";
	
	public static string ANIM_AIR_DIE = "AIR_DIE";
	public static string ANIM_AIR_HURT_ARROW = "AIR_HURT_ARROW";
	public static string ANIM_AIR_HURT_SWORD = "AIR_HURT_SWORD";
	public static string ANIM_AIR_IDLE = "AIR_IDLE";
	public static string ANIM_AIR_LASER_CHARGE = "AIR_LASER_CHARGE";
	public static string ANIM_AIR_LASER_FIRING = "AIR_LASER_FIRING";
	public static string ANIM_AIR_NOTICE = "AIR_NOTICE";
	
	public static LaserCrabEnemySprite cons() {
		return ObjectPool.inst().generic_depool<LaserCrabEnemySprite>().i_cons();
	}
	public SPSprite _img;
	public void add_to_parent(SPNode parent) { parent.add_child(_img); }
	public SPSpriteAnimator _animator;
	public void depool() {
		_img = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_LASER_CRAB,new Rect());
		_img.set_name("LaserCrabEnemySprite");
	}
	public void repool() {
		_animator.set_target(null);
		_animator = null;
		_img.depool();
	}
	
	private LaserCrabEnemySprite i_cons() {
		this.cons_anims();
		this.play_anim(ANIM_UNDERWATER_IDLE);
		return this;
	}
	
	public void set_manual_sort_z_order(int zord) { _img.set_manual_sort_z_order(zord); }
	
	public void i_update(GameEngineScene g) {
		_animator.i_update();
	}
	
	public void play_anim(string anim, bool force = false) {
		_animator.play_anim(anim, force);
	}
	
	public void set_rotation(float val) { _img.set_rotation(val); }
	public float rotation() { return _img.rotation(); }
	
	public Vector4 color() { return _img.color(); }
	public void set_color(Vector4 color) { _img.set_color(color); }
	
	public void set_enabled(bool val) { _img.set_enabled(val); }
	
	private static MultiMap<string,Rect> __cached_anim_rects;
	private void cons_anims() {
		if (__cached_anim_rects == null) {
			__cached_anim_rects = new MultiMap<string, Rect>();
			for (int i = 0; i < 20; i++) {
				string rectname = SPUtil.sprintf("UW_idle__%03d.png",i);
				__cached_anim_rects.add(ANIM_UNDERWATER_IDLE,FileCache.inst().get_texrect(RTex.ENEMY_LASER_CRAB,rectname));
			}
			for (int i = 0; i < 16; i++) {
				string rectname = SPUtil.sprintf("UW_laser_charge__%03d.png",i);
				__cached_anim_rects.add(ANIM_UNDERWATER_LASER_CHARGE,FileCache.inst().get_texrect(RTex.ENEMY_LASER_CRAB,rectname));
			}
			for (int i = 0; i < 4; i++) {
				string rectname = SPUtil.sprintf("UW_laser_firing__%03d.png",i);
				__cached_anim_rects.add(ANIM_UNDERWATER_LASER_FIRING,FileCache.inst().get_texrect(RTex.ENEMY_LASER_CRAB,rectname));
			}
			for (int i = 0; i < 5; i++) {
				string rectname = SPUtil.sprintf("UW_notice__%03d.png",i);
				__cached_anim_rects.add(ANIM_UNDERWATER_NOTICE,FileCache.inst().get_texrect(RTex.ENEMY_LASER_CRAB,rectname));
			}
			for (int i = 0; i < 5; i++) {
				string rectname = SPUtil.sprintf("UW_stun__%03d.png",i);
				__cached_anim_rects.add(ANIM_UNDERWATER_STUN,FileCache.inst().get_texrect(RTex.ENEMY_LASER_CRAB,rectname));
			}
			
			for (int i = 0; i < 5; i++) {
				string rectname = SPUtil.sprintf("AIR_die__%03d.png",i);
				__cached_anim_rects.add(ANIM_AIR_DIE,FileCache.inst().get_texrect(RTex.ENEMY_LASER_CRAB,rectname));
			}
			for (int i = 0; i < 4; i++) {
				string rectname = SPUtil.sprintf("AIR_hurt_arrow__%03d.png",i);
				__cached_anim_rects.add(ANIM_AIR_HURT_ARROW,FileCache.inst().get_texrect(RTex.ENEMY_LASER_CRAB,rectname));
			}
			for (int i = 0; i < 5; i++) {
				string rectname = SPUtil.sprintf("AIR_hurt_sword__%03d.png",i);
				__cached_anim_rects.add(ANIM_AIR_HURT_SWORD,FileCache.inst().get_texrect(RTex.ENEMY_LASER_CRAB,rectname));
			}
			for (int i = 0; i < 5; i++) {
				string rectname = SPUtil.sprintf("AIR_idle__%03d.png",i);
				__cached_anim_rects.add(ANIM_AIR_IDLE,FileCache.inst().get_texrect(RTex.ENEMY_LASER_CRAB,rectname));
			}
			for (int i = 0; i < 4; i++) {
				string rectname = SPUtil.sprintf("AIR_laser_charge__%03d.png",i);
				__cached_anim_rects.add(ANIM_AIR_LASER_CHARGE,FileCache.inst().get_texrect(RTex.ENEMY_LASER_CRAB,rectname));
			}
			for (int i = 0; i < 4; i++) {
				string rectname = SPUtil.sprintf("AIR_laser_firing__%03d.png",i);
				__cached_anim_rects.add(ANIM_AIR_LASER_FIRING,FileCache.inst().get_texrect(RTex.ENEMY_LASER_CRAB,rectname));
			}
			for (int i = 0; i < 5; i++) {
				string rectname = SPUtil.sprintf("AIR_notice__%03d.png",i);
				__cached_anim_rects.add(ANIM_AIR_NOTICE,FileCache.inst().get_texrect(RTex.ENEMY_LASER_CRAB,rectname));
			}
		}
		_animator = SPSpriteAnimator.cons(_img);
		_animator.add_anim(ANIM_UNDERWATER_IDLE, __cached_anim_rects.list(ANIM_UNDERWATER_IDLE), 7.0f, true);		
		_animator.add_anim(ANIM_UNDERWATER_LASER_CHARGE, __cached_anim_rects.list(ANIM_UNDERWATER_LASER_CHARGE), 5.0f, false);
		_animator.add_anim(ANIM_UNDERWATER_LASER_FIRING, __cached_anim_rects.list(ANIM_UNDERWATER_LASER_FIRING), 5.0f, true);
		_animator.add_anim(ANIM_UNDERWATER_NOTICE, __cached_anim_rects.list(ANIM_UNDERWATER_NOTICE), 5.0f, false);
		_animator.add_anim(ANIM_UNDERWATER_STUN, __cached_anim_rects.list(ANIM_UNDERWATER_STUN), 5.0f, true);
		
		_animator.add_anim(ANIM_AIR_DIE, __cached_anim_rects.list(ANIM_AIR_DIE), 5.0f, false);
		_animator.add_anim(ANIM_AIR_HURT_ARROW, __cached_anim_rects.list(ANIM_AIR_HURT_ARROW), 5.0f, true);
		_animator.add_anim(ANIM_AIR_HURT_SWORD, __cached_anim_rects.list(ANIM_AIR_HURT_SWORD), 5.0f, true);
		_animator.add_anim(ANIM_AIR_IDLE, __cached_anim_rects.list(ANIM_AIR_IDLE), 5.0f, true);
		_animator.add_anim(ANIM_AIR_LASER_CHARGE, __cached_anim_rects.list(ANIM_AIR_LASER_CHARGE), 5.0f, false);
		_animator.add_anim(ANIM_AIR_LASER_FIRING, __cached_anim_rects.list(ANIM_AIR_LASER_FIRING), 5.0f, true);
		_animator.add_anim(ANIM_AIR_NOTICE, __cached_anim_rects.list(ANIM_AIR_NOTICE), 5.0f, false);
		
		_animator.play_anim(ANIM_UNDERWATER_IDLE);
		
		_img.set_scale(1.5f);
	}
	
	public SPHitRect get_hit_rect(Vector2 pos, float rotation) {
		return SPHitPoly.hitpoly_to_bounding_hitrect(
			this.get_hit_poly(pos,rotation),
			new Vector2(-10,-10),
			new Vector2(10,10)
		);
	}
	
	public SPHitPoly get_hit_poly(Vector2 pos, float rotation) {
		return SPHitPoly.cons_with_basis_offset(
			pos,
			rotation,
			new Vector2(90,90),
			new Vector2(1,1),
			1,
			new Vector2(-10,0)
		);
	}
	
}
