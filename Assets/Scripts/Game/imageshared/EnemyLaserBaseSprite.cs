using UnityEngine;
using System.Collections;

public class EnemyLaserBaseSprite : GenericPooledObject, SPNodeHierarchyElement, SPGameUpdateable {
	
	public static string ANIM_POINT = "point";
	public static string ANIM_START = "start";
	
	public static EnemyLaserBaseSprite cons() {
		return ObjectPool.inst().generic_depool<EnemyLaserBaseSprite>().i_cons();
	}
	
	public SPNode _root;
	public SPSprite _sprite;
	public SPSpriteAnimator _animator;
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("EnemyLaserSprite");
		_sprite = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_LASER, FileCache.inst().get_texrect(RTex.ENEMY_LASER,"laser_point_0.png"));
		_sprite.set_manual_sort_z_order(GameAnchorZ.Enemy_FX);
		_sprite.set_name("EnemyLaserSprite_sprite");
		_root.add_child(_sprite);
		
		_animator = SPSpriteAnimator.cons(_sprite)
			.add_anim(ANIM_POINT, 
				FileCache.inst().get_rects_list(RTex.ENEMY_LASER, "laser_point_%d.png",0,2),7)
			.add_anim(ANIM_START,
				FileCache.inst().get_rects_list(RTex.ENEMY_LASER, "laser_start%04d.png",0,13),3,false)
			.play_anim(ANIM_POINT);
	}
	public void repool() {
		_root.repool();
		_animator.set_target(null);
		_animator = null;
		_root = null;
		_sprite = null;
	}
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	private EnemyLaserBaseSprite i_cons() {
		return this;
	}
	public void i_update(GameEngineScene g) { 
		if (!_root.is_enabled()) return;
		_animator.i_update(); 
	}
}
