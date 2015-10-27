using UnityEngine;
using System.Collections;

public class PufferBasicWaterEnemy : BasicWaterEnemy, SPHitPolyOwner {

	public static PufferBasicWaterEnemy cons(GameEngineScene g, Vector2 pt1, Vector2 pt2) {
		return (ObjectPool.inst().generic_depool<PufferBasicWaterEnemy>()).i_cons(g,pt1,pt2);
	}
	public override void depool() {
		base.depool();
		_img = PufferEnemySprite.cons();
	}
	public override void repool() {
		_img.repool();
		base.repool();
	}
	public override void do_remove() {
		ObjectPool.inst().generic_repool(this);
	}
	
	private PufferEnemySprite _img;
	private Color _tar_color;
	private FlashCount _flashcount;
	public new PufferBasicWaterEnemy i_cons(GameEngineScene g, Vector2 pt1, Vector2 pt2) {
		base.i_cons(g, pt1, pt2);
		
		this.get_root().set_name("PufferBasicWaterEnemy");
		_img.add_to_parent(this.get_root());
		
		_tar_color = Color.white;
		_flashcount = FlashCount.cons();
		_flashcount
			.add_flash_at(50)
			.add_flash_at(30)
			.add_flash_at(15);
		return this;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		base.i_update(g,state);

		Vector4 img_color = _img.color();
		img_color.y = SPUtil.drpt(img_color.y,1,1/8.0f);
		img_color.z = SPUtil.drpt(img_color.z,1,1/8.0f);

		switch(this._current_mode) {
		case Mode.IdleMove: {
			_img.play_anim(PufferEnemySprite.ANIM_IDLE);
		} break;
		case Mode.InPack: {
			_img.play_anim(PufferEnemySprite.ANIM_FOLLOW);
			_flashcount.reset();
		} break;
		case Mode.Stunned: {
			_img.play_anim(PufferEnemySprite.ANIM_HURT);
			if (_flashcount.do_flash_given_time(this._stunned_anim_ct)) {
				img_color.y = 0;
				img_color.z = 0;
			}
		} break;
		default: break;
		}

		_img.set_color(img_color);
		_img.i_update(g);
	}

	public override void i_update(GameEngineScene g, DiveReturnGameState state) {
		base.i_update(g, state);

		switch(this._current_divereturn_mode) {
		case DiveReturnMode.Normal: {
			_img.play_anim(PufferEnemySprite.ANIM_FOLLOW);
		} break;
		case DiveReturnMode.Hit: {
			_img.play_anim(PufferEnemySprite.ANIM_HURT);
		} break;
		}
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
