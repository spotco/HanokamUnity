using UnityEngine;
using System.Collections;

public class EnemyNoticeParticle : SPGameEngineParticle, GenericPooledObject {

	public static EnemyNoticeParticle cons(Vector2 pos, float rotation) {
		return ObjectPool.inst().generic_depool<EnemyNoticeParticle>().i_cons(pos,rotation);
	}

	private SPNode _root;
	private SPSprite _img;
	private SPSprite _excl_img;
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("EnemyNoticeParticle");
		_root.set_manual_sort_z_order(GameAnchorZ.BGWater_FX);
		_img = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_EFFECTS,FileCache.inst().get_texrect(RTex.ENEMY_EFFECTS,"enemy Notice Ink.png"));
		_img.set_name("_img");
		_root.add_child(_img);
		_excl_img = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_EFFECTS,FileCache.inst().get_texrect(RTex.ENEMY_EFFECTS,"Enemy Notice !.png"));
		_excl_img.set_name("_excl_img");
		_root.add_child(_excl_img);
	}
	public void repool() {
		_root.repool();
		_root = null;
		_img = null;
		_excl_img = null;
	}

	private float _anim_t;
	
	private bool _has_first_track_y;
	private float _last_track_y;
	
	public EnemyNoticeParticle i_cons(Vector2 pos, float rotation) {
		_root.set_u_pos(pos);
		_img.set_rotation(rotation);
		_anim_t = 0;
		this.update_img();
		
		_has_first_track_y = false;
		
		return this;
	}
	private void update_img() {
		if (_anim_t <= 0) {
			_img.set_opacity(0);
			_img.set_scale_x(0.25f*4);
		} else if (_anim_t <= 0.8f) {
			_img.set_scale(SPUtil.drpt(_img.scale_x(),0.25f * 4,1/4.0f));
			_img.set_opacity(SPUtil.drpt(_img.get_opacity(),0.7f, 1/5.0f));
		} else {
			_img.set_scale(SPUtil.drpt(_img.scale_x(),0.5f*4,1/4.0f));
			_img.set_opacity(SPUtil.drpt(_img.get_opacity(),0, 1/5.0f));
		}
		_excl_img.set_scale(_img.scale_x());
		_excl_img.set_opacity(_img.get_opacity());
	}
	public override void i_update(GameEngineScene g) {
		_anim_t += 0.025f * SPUtil.dt_scale_get();
		this.update_img();
		
		if (!_has_first_track_y) {
			_has_first_track_y = true;
			_last_track_y = g._bg_water.get_y_offset();
		}
		_root._u_y = _root._u_y - (g._bg_water.get_y_offset()-_last_track_y);
		_last_track_y = g._bg_water.get_y_offset();
	}
	public override bool should_remove(GameEngineScene g) {
		return _anim_t >= 1;
	}
	public override void do_remove(GameEngineScene g) {
		ObjectPool.inst().generic_repool<EnemyNoticeParticle>(this);
	}
	public override void add_to_parent(SPNode parent) {
		parent.add_child(_root);
	}
}
