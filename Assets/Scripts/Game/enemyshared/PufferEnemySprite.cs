using UnityEngine;
using System.Collections.Generic;

public class PufferEnemySprite : GenericPooledObject, SPNodeHierarchyElement, SPGameUpdateable {
	public const string ANIM_ATTACK = "ANIM_ATTACK";
	public const string ANIM_DIE = "ANIM_DIE";
	public const string ANIM_FOLLOW = "ANIM_FOLLOW";
	public const string ANIM_HURT = "ANIM_HURT";
	public const string ANIM_IDLE = "ANIM_IDLE";
	
	public static PufferEnemySprite cons() {
		return ObjectPool.inst().generic_depool<PufferEnemySprite>().i_cons();
	}
	private SPSprite _img;
	private SPSpriteAnimator _animator;
	public void depool() {
		_img = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_PUFFER,new Rect());
		_img.set_name("PufferEnemySprite");
	}
	public void repool() {
		_animator.set_target(null);
		_animator = null;
		_img.depool();
	}
	public void add_to_parent(SPNode parent) {
		parent.add_child(_img);
	}
	private PufferEnemySprite i_cons() {
		this.cons_anims();
		_animator.play_anim(ANIM_IDLE);
		return this;
	}
	
	public PufferEnemySprite set_manual_sort_z_order(int zord) {
		_img.set_manual_sort_z_order(zord);
		return this;
	}
	
	public void i_update(GameEngineScene g) {
		_animator.i_update();
	}
	
	public void play_anim(string anim) {
		_animator.play_anim(anim);
	}
	
	public void set_rotation(float val) { _img.set_rotation(val); }
	public float rotation() { return _img.rotation(); }

	public Vector4 color() { return _img.color(); }
	public void set_color(Vector4 color) { _img.set_color(color); }

	private static MultiMap<string,Rect> __cached_anim_rects;
	private void cons_anims() {
		if (__cached_anim_rects == null) {
			__cached_anim_rects = new MultiMap<string, Rect>();
			for (int i = 0; i < 27; i++) {
				string rectname = SPUtil.sprintf("puffer_idle__%03d.png",i);
				__cached_anim_rects.add(ANIM_IDLE, FileCache.inst().get_texrect(RTex.ENEMY_PUFFER,rectname));
			}
			for (int i = 0; i < 5; i++) {
				string rectname = SPUtil.sprintf("puffer_attack__%03d.png",i);
				__cached_anim_rects.add(ANIM_ATTACK,FileCache.inst().get_texrect(RTex.ENEMY_PUFFER,rectname));
			}
			for (int i = 0; i < 14; i++) {
				string rectname = SPUtil.sprintf("puffer_die__%03d.png",i);
				__cached_anim_rects.add(ANIM_DIE,FileCache.inst().get_texrect(RTex.ENEMY_PUFFER,rectname));
			}
			for (int i = 0; i < 5; i++) {
				string rectname = SPUtil.sprintf("puffer_follow__%03d.png",i);
				__cached_anim_rects.add(ANIM_FOLLOW,FileCache.inst().get_texrect(RTex.ENEMY_PUFFER,rectname));
			}
			for (int i = 0; i < 5; i++) {
				string rectname = SPUtil.sprintf("puffer_hurt__%03d.png",i);
				__cached_anim_rects.add(ANIM_HURT,FileCache.inst().get_texrect(RTex.ENEMY_PUFFER,rectname));
			}
		}
		_animator = SPSpriteAnimator.cons(_img);
		_animator.add_anim(ANIM_IDLE, __cached_anim_rects.list(ANIM_IDLE), 2.5f, true);
		_animator.add_anim(ANIM_ATTACK, __cached_anim_rects.list(ANIM_ATTACK), 2.5f, true);
		_animator.add_anim(ANIM_DIE, __cached_anim_rects.list(ANIM_DIE), 2.5f, false);
		_animator.add_anim(ANIM_FOLLOW, __cached_anim_rects.list(ANIM_FOLLOW), 2.5f, true);
		_animator.add_anim(ANIM_HURT, __cached_anim_rects.list(ANIM_HURT), 2.5f * 1.25f, true);
	}
	
}
