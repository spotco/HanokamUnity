using UnityEngine;
using System.Collections;

public class SPConfigAnimParticle : SPGameEngineParticle, GenericPooledObject {

	public static SPConfigAnimParticle cons() {
		return ObjectPool.inst().generic_depool<SPConfigAnimParticle>().i_cons();
	}

	private SPSprite _img;
	public void depool() {
		_img = SPSprite.cons_sprite_texkey_texrect(RTex.BLANK,new Rect());
	}
	public void repool() {
		if (_animator != null) {
			_animator.set_target(null);
			_animator = null;
		}
		if (_timed_animator != null) {
			_timed_animator.set_target(null);
			_timed_animator = null;
		}
		_anim_lambda = null;
		_img.repool();
		_img = null;
	}

	private float _ct, _ctmax;
	private float _vr;
	private SPRange _scale;
	private SPRange _alpha;
	private Vector2 _velocity;
	private Vector2 _acceleration;

	private SPSpriteAnimator _animator;
	private SPTimedSpriteAnimator _timed_animator;
	private System.Action<SPSprite,float> _anim_lambda;

	public SPConfigAnimParticle i_cons() {
		_ct = 0;
		_vr = 0;
		_ctmax = 50;
		_scale = _alpha = new SPRange() { _min = 1, _max = 1 };
		_velocity = _acceleration = new Vector2();
		_img.set_rotation(0);
		_img.set_u_pos(0,0);
		_animator = null;
		_timed_animator = null;
		_anim_lambda = null;
		return this;
	}
	public override void i_update(GameEngineScene g) {
		_ct += SPUtil.dt_scale_get();
		float anim_t = _ct / _ctmax;
		_img.set_rotation(_img.rotation() + _vr * SPUtil.dt_scale_get());
		_img.set_scale(SPUtil.lerp(_scale._min,_scale._max,anim_t));
		_img.set_opacity(SPUtil.lerp(_alpha._min,_alpha._max,anim_t));
		_img.set_u_pos(_img._u_x + _velocity.x * SPUtil.dt_scale_get(), _img._u_y + _velocity.y * SPUtil.dt_scale_get());
		_velocity.x += _acceleration.x * SPUtil.dt_scale_get();
		_velocity.y += _acceleration.y * SPUtil.dt_scale_get();

		if (_animator != null) {
			_animator.i_update();
		}
		if (_timed_animator != null) {
			_timed_animator.show_frame_for_time(anim_t);
		}
		if (_anim_lambda != null) {
			_anim_lambda(_img, anim_t);
		}
	}
	public override bool should_remove(GameEngineScene g) {
		return _ct >= _ctmax;
	}
	public override void do_remove(GameEngineScene g) {
		ObjectPool.inst().generic_repool<SPConfigAnimParticle>(this);
	}
	public override void add_to_parent(SPNode parent) {
		parent.add_child(_img);
	}

	public SPConfigAnimParticle set_alpha(float start, float end) {
		_alpha = new SPRange() { _min = start, _max = end };
		return this;
	}
	public SPConfigAnimParticle set_vel(float vx, float vy) {
		_velocity = new Vector2(vx,vy);
		return this;
	}
	public SPConfigAnimParticle set_acceleration(float ax, float ay) {
		_acceleration = new Vector2(ax,ay);
		return this;
	}
	public SPConfigAnimParticle set_ctmax(float ctmax) {
		_ctmax = ctmax;
		return this;
	}
	public SPConfigAnimParticle set_vr(float vr) {
		_vr = vr;
		return this;
	}
	public SPConfigAnimParticle set_rotation(float rotation) {
		_img.set_rotation(rotation);
		return this;
	}
	public SPConfigAnimParticle set_scale(float min, float max) {
		_scale = new SPRange() { _min = min, _max = max };
		_img.set_scale(min);
		return this;
	}
	public SPConfigAnimParticle set_texture(Texture tex) {
		_img.manual_set_texture(tex);
		return this;
	}
	public SPConfigAnimParticle set_texrect(Rect rect) {
		_img.set_tex_rect(rect);
		return this;
	}
	public SPConfigAnimParticle set_pos(float x, float y) {
		_img.set_u_pos(x,y);
		return this;
	}
	public SPConfigAnimParticle set_name(string name) {
		_img.set_name(name);
		return this;
	}
	public SPConfigAnimParticle set_manual_sort_z_order(int z_ord) {
		_img.set_manual_sort_z_order(z_ord);
		return this;
	}
	public SPConfigAnimParticle set_anchor_point(float x, float y) {
		_img.set_anchor_point(x,y);
		return this;
	}
	public SPConfigAnimParticle set_sprite_animator(SPSpriteAnimator animator) {
		_animator = animator;
		_animator.set_target(_img);
		return this;
	}
	public SPConfigAnimParticle set_normalized_timed_sprite_animator(SPTimedSpriteAnimator timed_animator) {
		_timed_animator = timed_animator;
		_timed_animator.set_target(_img);
		_timed_animator.show_frame_for_time(_ct/_ctmax);
		return this;
	}
	public SPConfigAnimParticle set_anim_lambda(System.Action<SPSprite,float> anim_lambda) {
		_anim_lambda = anim_lambda;
		_anim_lambda(_img,_ct/_ctmax);
		return this;
	}
}