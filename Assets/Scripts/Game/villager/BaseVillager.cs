using UnityEngine;
using System.Collections;

public class BaseVillager : SPGameUpdateable {

	public virtual void i_update(GameEngineScene g) {}

}

public class TestBoyVillager : BaseVillager {
	public static TestBoyVillager cons(SpriterNode img) {
		return (new TestBoyVillager()).i_cons(img);
	}

	private enum Mode {
		Left,
		LeftToRight,
		Right,
		RightToLeft
	}

	private SpriterNode _img;
	private Mode _current_mode;
	private float _anim_t = 0;
	public TestBoyVillager i_cons(SpriterNode img) {
		_img = img;
		_current_mode = Mode.Left;
		_img.set_u_pos(-100,0);
		return this;
	}

	public override void i_update(GameEngineScene g) {
		switch (_current_mode) {
		case Mode.Left: {
			_anim_t += SPUtil.sec_to_tick(0.5f);
			_img._u_x = -200;
			_img.p_play_anim("Idle");
			if (_anim_t >= 1) {
				_anim_t = 0;
				_current_mode = Mode.LeftToRight;
			}

		} break;
		case Mode.LeftToRight: {
			_anim_t += SPUtil.sec_to_tick(1.0f);
			_img._u_x = SPUtil.lerp(-200,200,_anim_t);
			_img.p_play_anim("Walk");
			_img.set_img_scale_x(-1);
			if (_anim_t >= 1) {
				_anim_t = 0;
				_current_mode = Mode.Right;
			}
		} break;
		case Mode.Right: {
			_anim_t += SPUtil.sec_to_tick(0.5f);
			_img._u_x = 200;
			_img.p_play_anim("Idle");
			if (_anim_t >= 1) {
				_anim_t = 0;
				_current_mode = Mode.RightToLeft;
			}
		} break;
		case Mode.RightToLeft: {
			_anim_t += SPUtil.sec_to_tick(1.0f);
			_img._u_x = SPUtil.lerp(200,-200,_anim_t);
			_img.p_play_anim("Walk");
			_img.set_img_scale_x(1);
			if (_anim_t >= 1) {
				_current_mode = Mode.Left;
				_anim_t = 0;
			}
		} break;
		}	
	}

}