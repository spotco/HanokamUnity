using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SPSpriteAnimator {
	private class SPSpriteAnimator_Animation {
		public List<Rect> _frames;
		public float _duration;
		public bool _repeating;
	}

	private Dictionary<string,SPSpriteAnimator_Animation> _anim_name_to_anim = new Dictionary<string,SPSpriteAnimator_Animation>();
	private SPSprite _target;
	private float _ct;
	private int _i;
	private string _current_anim_name;

	public static SPSpriteAnimator cons(SPSprite target) {
		return (new SPSpriteAnimator()).i_cons(target);
	}

	public SPSpriteAnimator i_cons(SPSprite target) {
		_target = target;
		return this;
	}

	public void set_target(SPSprite tar) {
		_target = tar;
	}

	public SPSpriteAnimator add_anim(string name, List<Rect> frames, float duration, bool repeating = true) {
		_anim_name_to_anim[name] = new SPSpriteAnimator_Animation() {
			_frames = frames,
			_duration = duration,
			_repeating = repeating
		};
		return this;
	}

	private bool _is_finished;
	public bool is_finished() {
		return _is_finished;
	}

	public SPSpriteAnimator play_anim(string name, bool force = false) {
		if (!_anim_name_to_anim.ContainsKey(name)) {
			SPUtil.logf("ANIM %s not found",name);
			return this;
		}
		if (_current_anim_name != name || force) {
			_current_anim_name = name;
			_i = 0;
			_ct = this.current_anim()._duration;
			if (_target != null) {
				_target.set_tex_rect(this.current_frame());
			}
			_is_finished = false;
		}
		return this;
	}

	private SPSpriteAnimator_Animation current_anim() {
		return _anim_name_to_anim[_current_anim_name];
	}
	private Rect current_frame() {
		return this.current_anim()._frames[_i];
	}

	public void i_update() {
		if (_is_finished || _target == null || _current_anim_name == null || this.current_anim()._duration <= 0) return;
		_target.set_tex_rect(this.current_frame());

		_ct -= SPUtil.dt_scale_get();
		while (_ct <= 0) {
			_ct += this.current_anim()._duration;
			if (_i+1 >= this.current_anim()._frames.Count) {
				if (this.current_anim()._repeating) {
					_i = 0;
				} else {
					_is_finished = true;
					break;
				}
			} else {
				_i++;
			}
		}
	}
}