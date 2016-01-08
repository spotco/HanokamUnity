using UnityEngine;
using System.Collections.Generic;

public class SpikeEnemySprite : GenericPooledObject, SPNodeHierarchyElement, SPGameUpdateable {
	public const string ANIM_HIT = "ANIM_HIT";
	public const string ANIM_IDLE = "ANIM_IDLE";
	
	public static SpikeEnemySprite cons() {
		return ObjectPool.inst().generic_depool<SpikeEnemySprite>().i_cons();
	}
	private SPSprite _img;
	private SPSpriteAnimator _animator;
	public void depool() {
		_img = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_SPIKE,new Rect());
		_img.set_name("SpikeEnemySprite");
	}
	public void repool() {
		_animator.set_target(null);
		_animator = null;
		_img.depool();
	}
	public void add_to_parent(SPNode parent) {
		parent.add_child(_img);
	}
	private SpikeEnemySprite i_cons() {
		this.cons_anims();
		_animator.play_anim(ANIM_IDLE);
		return this;
	}
	
	public SpikeEnemySprite set_manual_sort_z_order(int zord) {
		_img.set_manual_sort_z_order(zord);
		return this;
	}
	
	public void i_update(GameEngineScene g) {
		_animator.i_update();
	}
	
	public void play_anim(string anim) {
		_animator.play_anim(anim);
	}
	
	public void set_scale(float sc) { _img.set_scale(sc*1.75f); }
	public float scale_x() { return _img.scale_x()/1.75f; }
	
	public void set_rotation(float val) { _img.set_rotation(val); }
	public float rotation() { return _img.rotation(); }
	
	public Vector4 color() { return _img.color(); }
	public void set_color(Vector4 color) { _img.set_color(color); }
	
	public void set_enabled(bool val) { _img.set_enabled(val); }
	
	private static MultiMap<string,Rect> __cached_anim_rects;
	private void cons_anims() {
		if (__cached_anim_rects == null) {
			__cached_anim_rects = new MultiMap<string, Rect>();
			for (int i = 0; i < 25; i++) {
				string rectname = SPUtil.sprintf("Idle_%03d.png",i);
				__cached_anim_rects.add(ANIM_IDLE, FileCache.inst().get_texrect(RTex.ENEMY_SPIKE,rectname));
			}
			for (int i = 0; i < 9; i++) {
				string rectname = SPUtil.sprintf("Hit_%03d.png",i);
				__cached_anim_rects.add(ANIM_HIT, FileCache.inst().get_texrect(RTex.ENEMY_SPIKE,rectname));
			}
		}
		_animator = SPSpriteAnimator.cons(_img);
		_animator.add_anim(ANIM_IDLE, __cached_anim_rects.list(ANIM_IDLE), 3.0f, true);
		_animator.add_anim(ANIM_HIT, __cached_anim_rects.list(ANIM_HIT), 2.0f, false);
	}
	
}
