using UnityEngine;
using System.Collections;

public class BubbleBasicWaterEnemy : BasicWaterEnemy {
	public static BubbleBasicWaterEnemy cons(GameEngineScene g, PatternEntry1Pt entry, Vector2 offset) {
		return (ObjectPool.inst().generic_depool<BubbleBasicWaterEnemy>()).i_cons(g, entry, offset);
	}
	
	public override void do_remove() {
		ObjectPool.inst().generic_repool<BubbleBasicWaterEnemy>(this);
	}
	
	private BubbleSprite _img;
	public override void depool() {
		base.depool();
		_img = BubbleSprite.cons();
		_img.add_to_parent(this.get_root());
		_img.set_manual_sort_z_order(GameAnchorZ.Enemy_InAir);
	}
	
	public override void repool() {
		_img.repool();
		base.repool();
	}
	
	private BubbleBasicWaterEnemy i_cons(GameEngineScene g, PatternEntry1Pt entry, Vector2 offset) {
		base.i_cons();
		_params._pos = new Vector2(entry._start.x + offset.x, entry._start.y + offset.y);
		return this;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		base.i_update(g,state);
		_img.play_anim(PufferEnemySprite.ANIM_IDLE);
		_img.i_update(g);
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
			new Vector2(this.get_root()._u_x, this.get_root()._u_y),
			this.get_root().rotation(),
			new Vector2(55,90),
			new Vector2(1,1),
			1,
			new Vector2(-10,0)
		);
	}
}
