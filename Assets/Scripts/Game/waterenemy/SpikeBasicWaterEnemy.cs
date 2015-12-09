using UnityEngine;
using System.Collections;

public class SpikeBasicWaterEnemy : BasicWaterEnemy {
	public static SpikeBasicWaterEnemy cons(GameEngineScene g, PatternEntry1Pt entry, Vector2 offset) {
		return (ObjectPool.inst().generic_depool<SpikeBasicWaterEnemy>()).i_cons(g, entry, offset);
	}
	public static SpikeBasicWaterEnemy cons(GameEngineScene g, PatternEntry2Pt entry, Vector2 offset) {
		return (ObjectPool.inst().generic_depool<SpikeBasicWaterEnemy>()).i_cons(g, entry, offset);
	}
	
	public override void do_remove() {
		ObjectPool.inst().generic_repool<BubbleBasicWaterEnemy>(this);
	}
	
	public override BasicWaterEnemy set_rotation(float deg) { return this; }
	
	private SPSprite _img;
	public override void depool() {
		base.depool();
		this.get_root().set_name("SpikeBasicWaterEnemy");
		_img = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_SPIKE,FileCache.inst().get_texrect(RTex.ENEMY_SPIKE,"spike_test.png"));
	}
	
	public override void repool() {
		_img.repool();
		_img = null;
		base.repool();
	}
	
	private SpikeBasicWaterEnemy i_cons(GameEngineScene g, PatternEntry1Pt entry, Vector2 offset) {
		this.shared_i_cons_pre(g,entry._start,offset);
		
		this.add_component_for_mode(Mode.Moving, BubbleNoMoveBasicWaterEnemyComponent.cons(_params._pos));		
		
		this.shared_i_cons_post(g);
		return this;
	}
	
	private SpikeBasicWaterEnemy i_cons(GameEngineScene g, PatternEntry2Pt entry, Vector2 offset) {
		this.shared_i_cons_pre(g,entry._start,offset);
		
		this.add_component_for_mode(Mode.Moving,TwoPointSwimBasicWaterEnemyComponent.cons(
			SPUtil.vec_add(entry._start,offset),
			SPUtil.vec_add(entry._pt1,offset),
			SPUtil.vec_add(entry._pt2,offset),
			3.0f
		));
		
		this.shared_i_cons_post(g);
		return this;
	}
	
	private void shared_i_cons_pre(GameEngineScene g, Vector2 entry_start, Vector2 offset) {
		base.i_cons();
		_params._pos = SPUtil.vec_add(entry_start,offset);
	}
	
	private void shared_i_cons_post(GameEngineScene g) {
		this.add_component_for_mode(Mode.Activated, BubblePoppedBasicWaterEnemyComponent.cons());
		this.add_hiteffect(BubbleBasicWaterEnemyHitEffect.cons());
		this.transition_to_mode(g, Mode.Moving);
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		base.i_update(g,state);
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
			new Vector2(110,110),
			new Vector2(1,1),
			1,
			new Vector2(0,0)
		);
	}
}
