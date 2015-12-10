using UnityEngine;
using System.Collections;

public class BubbleSprite : GenericPooledObject, SPNodeHierarchyElement, SPGameUpdateable {
	public static string ANIM_IDLE = "ANIM_IDLE";
	public static string ANIM_POP = "ANIM_POP";
	
	public static BubbleSprite cons() {
		return ObjectPool.inst().generic_depool<BubbleSprite>().i_cons();
	}
	private SPSprite _img;
	public void add_to_parent(SPNode parent) { parent.add_child(_img); }
	public SPSpriteAnimator _animator;
	public void depool() {
		_img = SPSprite.cons_sprite_texkey_texrect(RTex.BUBBLE_EFFECTS,new Rect());
		_img.set_name("PufferEnemySprite");
	}
	public void repool() {
		_animator.set_target(null);
		_animator = null;
		_img.depool();
	}
	
	private BubbleSprite i_cons() {
		this.cons_anims();
		this.play_anim(ANIM_IDLE);
		return this;
	}
	
	public BubbleSprite set_manual_sort_z_order(int zord) {
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
	
	public void set_enabled(bool val) { _img.set_enabled(val); }
	
	private static MultiMap<string,Rect> __cached_anim_rects;
	private void cons_anims() {
		if (__cached_anim_rects == null) {
			__cached_anim_rects = new MultiMap<string, Rect>();
			for (int i = 0; i < 8; i++) {
				string rectname = SPUtil.sprintf("bubble_%d.png",i);
				__cached_anim_rects.add(ANIM_IDLE,FileCache.inst().get_texrect(RTex.BUBBLE_EFFECTS,rectname));
			}
			for (int i = 0; i < 7; i++) {
				string rectname = SPUtil.sprintf("bubble_pop_%d.png",i);
				__cached_anim_rects.add(ANIM_POP,FileCache.inst().get_texrect(RTex.BUBBLE_EFFECTS,rectname));
			}
			__cached_anim_rects.add(ANIM_POP,new Rect());
		}
		_animator = SPSpriteAnimator.cons(_img);
		_animator.add_anim(ANIM_IDLE, __cached_anim_rects.list(ANIM_IDLE), 7.0f, true);
		_animator.add_anim(ANIM_POP, __cached_anim_rects.list(ANIM_POP), 7.0f, false);
	}
	
}
