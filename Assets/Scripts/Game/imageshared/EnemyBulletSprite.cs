using UnityEngine;
using System.Collections;

public class EnemyBulletSprite : GenericPooledObject, SPNodeHierarchyElement, SPGameUpdateable {

	public static EnemyBulletSprite cons() {
		return ObjectPool.inst().generic_depool<EnemyBulletSprite>().i_cons();
	}
	
	public SPNode _root;
	public SPSprite _sprite;
	public SPSpriteAnimator _animator;
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("EnemyBulletSprite");
		_sprite = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_EFFECTS, FileCache.inst().get_texrect(RTex.ENEMY_EFFECTS,"enemy_bullet_normal_000.png"));
		_sprite.set_manual_sort_z_order(GameAnchorZ.Enemy_FX);
		_root.add_child(_sprite);
		_animator = SPSpriteAnimator.cons(_sprite)
			.add_anim("play", 
			          FileCache.inst().get_rects_list(RTex.ENEMY_EFFECTS, "enemy_bullet_normal_00%d.png",0,4),4)
				.play_anim("play");
	}
	public void repool() {
		_root.repool();
		_animator.set_target(null);
		_root = null;
		_sprite = null;
	}
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	private EnemyBulletSprite i_cons() { return this; }
	public void i_update(GameEngineScene g) { 
		if (!_root.is_enabled()) return;
		_animator.i_update(); 
	}
	public void set_enabled(bool val) { _root.set_enabled(val); }
	
	public void set_rotation(float rotation) {
		_root.set_rotation(rotation);
	}
	public void set_u_pos(float x, float y) {
		_root.set_u_pos(x,y);
	}
	
	public SPHitRect get_hit_rect() {
		return SPHitPoly.hitpoly_to_bounding_hitrect(
			this.get_hit_poly(),
			new Vector2(-10,-10),
			new Vector2(10,10)
		);
	}
	
	public SPHitPoly get_hit_poly() {
		return SPHitPoly.cons_with_basis_offset(
			_root.get_u_pos(),
			_root.rotation()+90,
			new Vector2(50,50),
			new Vector2(1,1),
			1,
			new Vector2(0,0)
		);
	}
	
	public static void do_hit_particle(GameEngineScene g, Vector2 pos, bool do_track_bg_water = false) {
		SPConfigAnimParticle neu_particle = SPConfigAnimParticle.cons()
			.set_texture(TextureResource.inst().get_tex(RTex.PARTICLES_SPRITESHEET))
				.set_texrect(FileCache.inst().get_texrect(RTex.PARTICLES_SPRITESHEET,"grey_particle"))
				.set_pos(pos.x,pos.y)
				.set_ctmax(15)
				.set_manual_sort_z_order(GameAnchorZ.Player_FX)
				.set_scale(3,6)
				.set_alpha(1.0f,0.0f);
		if (do_track_bg_water) {
			neu_particle.set_do_track_bg_water();
		}
		g.add_particle(neu_particle);
	}

}
