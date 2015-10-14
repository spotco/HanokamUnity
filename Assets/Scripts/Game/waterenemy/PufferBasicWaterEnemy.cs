using UnityEngine;
using System.Collections;

public class PufferBasicWaterEnemy : BasicWaterEnemy, GenericPooledObject, SPHitPolyOwner {

	public static PufferBasicWaterEnemy cons(GameEngineScene g, Vector2 pt1, Vector2 pt2) {
		return (ObjectPool.inst().generic_depool<PufferBasicWaterEnemy>()).i_cons(g,pt1,pt2);
	}
	public void depool() {
		_img = PufferEnemySprite.cons();
	}
	public void repool() {
		_img.repool();
	}
	
	private PufferEnemySprite _img;
	private Color _tar_color;
	private FlashCount _flashcount;
	public PufferBasicWaterEnemy i_cons(GameEngineScene g, Vector2 pt1, Vector2 pt2) {
		base.i_cons(g, pt1, pt2);
		
		this.get_root().set_name("PufferBasicWaterEnemy");
		_img.add_to_parent(this.get_root());
		
		_tar_color = Color.white;
		_flashcount = FlashCount.cons();
		_flashcount
			.add_flash_at(30)
			.add_flash_at(20)
			.add_flash_at(10);
		return this;
	}
	
	public override void i_update (GameEngineScene g, DiveGameState state) {
		base.i_update(g,state);
		
		_img.i_update(g);
	}
	
	public override void do_remove(GameEngineScene g, DiveGameState state) {
		_img.repool();
		base.do_remove(g, state);
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
			new Vector2(this.get_root()._u_x, this.get_root()._u_y),
			this.get_root().rotation(),
			new Vector2(55,90),
			new Vector2(1,1),
			1,
			new Vector2(-10,0)
		);
	}
	
}
