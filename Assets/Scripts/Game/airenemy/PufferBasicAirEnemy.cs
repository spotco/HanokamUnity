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
	private FlashEvery _flashcount;
	
	private PufferBasicAirEnemy i_cons(GameEngineScene g, Vector3 end_pt) {
		base.i_cons();
		this.add_component_for_mode(Mode.Moving, CurveMoveBasicAirEnemyModeComponent.cons(end_pt));
		this.add_component_for_mode(Mode.Stunned, KnockbackStunBasicAirEnemyModeComponent.cons());
		this.add_component_for_mode(Mode.Dying, DeathAnimDelayBasicAirEnemyModeComponent.cons(50));
		
		_tar_color = Color.white;
		_flashcount = FlashEvery.cons(15);
		
		return this;
	}
	
	public override float get_max_health() { return 10; }
	public override Vector2 get_health_bar_offset() { return new Vector2(-25,35); }
	
	public override void i_update(GameEngineScene g, InAirGameState state) {
		base.i_update(g,state);
		
		Vector4 img_color = _img.color();
		img_color.y = SPUtil.drpt(img_color.y,1,1/8.0f);
		img_color.z = SPUtil.drpt(img_color.z,1,1/8.0f);
		
		switch (this.get_current_mode()) {
		case Mode.Moving: {
			_img.play_anim(PufferEnemySprite.ANIM_IDLE);
		} break;
		case Mode.Stunned: {
			_img.play_anim(PufferEnemySprite.ANIM_HURT);
			_flashcount.i_update(g);
			if (_flashcount.do_flash()) {
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
