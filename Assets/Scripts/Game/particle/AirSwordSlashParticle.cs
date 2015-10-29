using UnityEngine;
using System.Collections;

public class AirSwordSlashParticle : SPGameEngineParticle, GenericPooledObject {

	public static AirSwordSlashParticle cons(Vector2 pos, Vector2 dir) {
		return ObjectPool.inst().generic_depool<AirSwordSlashParticle>().i_cons(pos,dir);
	}
	
	private SPNode _root;
	private SPSprite _img, _img_blood;
	private SPTimedSpriteAnimator _sprite_animator, _blood_animator;
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("AirSwordSlashParticle");
		_img = SPSprite.cons_sprite_texkey_texrect(RTex.HANOKA_EFFECTS,FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"slash_000.png"));
		_img.set_name("_img");
		_img.set_manual_sort_z_order(GameAnchorZ.Player_FX);
		_img_blood = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_EFFECTS,FileCache.inst().get_texrect(RTex.ENEMY_EFFECTS,"enemy_blood_000.png"));
		_img_blood.set_name("_img_blood");
		_img_blood.set_rotation(180);
		_img_blood.set_manual_sort_z_order(GameAnchorZ.Player_FX-1);
		_root.add_child(_img);
		_root.add_child(_img_blood);
		
	}
	public void repool() {
		_img_blood.repool();
		_img.repool();
		_root.repool();
		_root = _img = _img_blood = null;
	}
	
	private float _anim_t;
	private AirSwordSlashParticle i_cons(Vector2 pos, Vector2 dir) {
		_sprite_animator = SPTimedSpriteAnimator.cons(_img)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"slash_000.png"),0.0f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"slash_001.png"),0.5f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"slash_002.png"),0.7f);
			
		_blood_animator = SPTimedSpriteAnimator.cons(_img_blood)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.ENEMY_EFFECTS,"enemy_blood_000.png"),0.0f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.ENEMY_EFFECTS,"enemy_blood_001.png"),0.25f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.ENEMY_EFFECTS,"enemy_blood_002.png"),0.5f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.ENEMY_EFFECTS,"enemy_blood_003.png"),0.75f);
			
		_root.set_u_pos(pos);
		_root.set_rotation(SPUtil.dir_ang_deg(dir.x,dir.y));
		
		_anim_t = 0;
		this.update_img();
		return this;
	}
	public override void i_update(GameEngineScene g) {
		_anim_t += SPUtil.dt_scale_get() * 0.065f;
		this.update_img();
	}
	private void update_img() {
		_sprite_animator.show_frame_for_time(_anim_t);
		_img.set_opacity(SPUtil.bezier_val_for_t(new Vector2(0,0), new Vector2(0,1.75f),new Vector2(0.75f,0.75f),new Vector2(1,0),_anim_t).y);
		_img.set_scale_x(SPUtil.bezier_val_for_t(new Vector2(0,0), new Vector2(0,1),new Vector2(1,0),new Vector2(1,1),_anim_t).y);
		_img.set_scale_y(SPUtil.bezier_val_for_t(new Vector2(0,0.5f), new Vector2(0,1),new Vector2(1,0.5f),new Vector2(1,1.5f),_anim_t).y);
		
		_blood_animator.show_frame_for_time(_anim_t);
		_img_blood.set_opacity(Mathf.Clamp(_img.get_opacity()*4,0,1));
		_img_blood.set_scale_x(_img.scale_x()*0.9f);
		_img_blood.set_scale_y(_img.scale_y()*0.9f);
	}
	public override bool should_remove(GameEngineScene g) {
		return _anim_t >= 1;
	}
	public override void do_remove(GameEngineScene g) {
		ObjectPool.inst().generic_repool<AirSwordSlashParticle>(this);
	}
	public override void add_to_parent(SPNode parent) {
		parent.add_child(_root);
	}
	
}
