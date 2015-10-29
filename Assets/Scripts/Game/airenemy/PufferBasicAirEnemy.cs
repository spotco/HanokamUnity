using UnityEngine;
using System.Collections;

public class PufferBasicAirEnemy : BasicAirEnemy {

	public static PufferBasicAirEnemy cons(GameEngineScene g, Vector3 end_pt) {
		return (ObjectPool.inst().generic_depool<PufferBasicAirEnemy>()).i_cons(g,end_pt);
	}
	
	private PufferEnemySprite _img;
	public override void depool() {
		base.depool();
		_img = PufferEnemySprite.cons();
		_img.add_to_parent(this.get_root());
		_img.set_manual_sort_z_order(GameAnchorZ.Enemy_InAir);
	}
	
	public override void repool() {
		_img.repool();
		base.repool();
	}
	
	private Color _tar_color;
	private FlashCount _flashcount;
	
	private PufferBasicAirEnemy i_cons(GameEngineScene g, Vector3 end_pt) {
		base.i_cons();
		this.add_component_for_mode(Mode.Moving, CurveMoveBasicAirEnemyModeComponent.cons(end_pt));
		this.add_component_for_mode(Mode.Stunned, KnockbackStunBasicAirEnemyModeComponent.cons());
		this.add_component_for_mode(Mode.Dying, DeathAnimDelayBasicAirEnemyModeComponent.cons(50));
		
		_tar_color = Color.white;
		_flashcount = FlashCount.cons();
		_flashcount
			.add_flash_at(150)
			.add_flash_at(135)
			.add_flash_at(120)
			.add_flash_at(100)
			.add_flash_at(80)
			.add_flash_at(60)
			.add_flash_at(40)
			.add_flash_at(20)
			.add_flash_at(10);
		
		return this;
	}
	
	
	public override void i_update(GameEngineScene g, InAirGameState state) {
		base.i_update(g,state);
		
		Vector4 img_color = _img.color();
		img_color.y = SPUtil.drpt(img_color.y,1,1/8.0f);
		img_color.z = SPUtil.drpt(img_color.z,1,1/8.0f);
		
		switch (this.get_current_mode()) {
		case Mode.Moving: {
			_img.play_anim(PufferEnemySprite.ANIM_IDLE);
			_flashcount.reset();
		} break;
		case Mode.Stunned: {
			_img.play_anim(PufferEnemySprite.ANIM_HURT);
			if (_flashcount.do_flash_given_time(this._params._stun_ct_max-this._params._stun_ct)) {
				img_color.y = 0;
				img_color.z = 0;
			}
		} break;
		case Mode.Dying: {
			_img.play_anim(PufferEnemySprite.ANIM_DIE);
		} break;
		default: break;
		}
		
		_img.set_color(img_color);
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
