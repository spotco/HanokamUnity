using UnityEngine;
using System.Collections;

public class LaserCrabEnemySprite : GenericPooledObject, SPNodeHierarchyElement, SPGameUpdateable {
	public static string ANIM_IDLE = "ANIM_IDLE";
	
	public static LaserCrabEnemySprite cons() {
		return ObjectPool.inst().generic_depool<LaserCrabEnemySprite>().i_cons();
	}
	private SPSprite _img;
	public void add_to_parent(SPNode parent) { parent.add_child(_img); }
	public SPSpriteAnimator _animator;
	public void depool() {
		_img = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_LASER,new Rect());
		_img.set_name("LaserCrabEnemySprite");
	}
	public void repool() {
		_animator.set_target(null);
		_animator = null;
		_img.depool();
	}
	
	private LaserCrabEnemySprite i_cons() {
		this.cons_anims();
		this.play_anim(ANIM_IDLE);
		return this;
	}
	
	public void set_manual_sort_z_order(int zord) { _img.set_manual_sort_z_order(zord); }
	
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
	
	public void set_enabled(bool val) { _img.set_enabled(val); }
	
	private static MultiMap<string,Rect> __cached_anim_rects;
	private void cons_anims() {
		if (__cached_anim_rects == null) {
			__cached_anim_rects = new MultiMap<string, Rect>();
			for (int i = 0; i < 1; i++) {
				string rectname = SPUtil.sprintf("crab_enemy_test.png",i);
				__cached_anim_rects.add(ANIM_IDLE,FileCache.inst().get_texrect(RTex.ENEMY_LASER,rectname));
			}
		}
		_animator = SPSpriteAnimator.cons(_img);
		_animator.add_anim(ANIM_IDLE, __cached_anim_rects.list(ANIM_IDLE), 7.0f, true);
		
		_img.set_scale(0.35f);
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
