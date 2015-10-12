using UnityEngine;
using System.Collections;

using UnityStandardAssets.ImageEffects;

public class GameCameraController : SPMainUpdateable {

	public static GameCameraController cons() {
		return (new GameCameraController()).i_cons();
	}

	public static float MIN_ZOOM = 10;
	public static float MAX_ZOOM = 1500;

	private DrptVal _camera_zoom;
	private DrptVal _camera_x;
	private DrptVal _camera_y;

	private CameraMotionBlur _motion_blur;
	private BloomOptimized _bloom;
	private VignetteAndChromaticAberration _vignette;
	
	private RenderTexture _game_camera_blur_copy;
	private SPSprite _ui_blur_cover;
	
	public GameCameraController i_cons() {
		_motion_blur = GameMain._context._game_camera.GetComponent<CameraMotionBlur>();
		_bloom = GameMain._context._game_camera.GetComponent<BloomOptimized>();
		_vignette = GameMain._context._game_camera.GetComponent<VignetteAndChromaticAberration>();
		_motion_blur.enabled = true;
		_motion_blur.excludeLayers.value = 1 << RLayer.get_layer(RLayer.UI);
		_bloom.enabled = true;
		_vignette.enabled = true;
		_has_camera_motion_blur = false;		
		
		_game_camera_blur_copy = new RenderTexture(
			GameMain._context._game_camera_out.width / 4,
			GameMain._context._game_camera_out.height / 4,
			0,
			RenderTextureFormat.Default
		);
		return this;
	}
	
	public void SetGameEngineSceneDefaults() {
		_camera_zoom = new DrptVal() {
			_current = 1500,
			_target = 1500,
			_drptval = 1/10.0f
		};
		_camera_x = new DrptVal() {
			_current = 0,
			_target = 0,
			_drptval = 1/10.0f
		};
		_camera_y = new DrptVal() {
			_current = 250,
			_target = 250,
			_drptval = 1/10.0f
		};
		this.apply_camera_values();
	}
	public void SetShopSceneDefaults() {
		_camera_zoom = new DrptVal() {
			_current = 1500,
			_target = 1500,
			_drptval = 1/10.0f
		};
		_camera_x = new DrptVal() {
			_current = 0,
			_target = 0,
			_drptval = 1/10.0f
		};
		_camera_y = new DrptVal() {
			_current = -680,
			_target = -680,
			_drptval = 1/10.0f
		};
		this.apply_camera_values();
	}
	
	public void create_blur_texture(UIRoot ui) {
		_ui_blur_cover = ui.add_blur_cover_sprite(_game_camera_blur_copy);
		_ui_blur_cover.set_opacity(0);
	}

	private Vector2 _last_shake;
	private void apply_camera_values() {
		Vector2 camera_shake = new Vector2();
		if (_camera_shake_ct > 0) {
			camera_shake.x = _camera_shake_intensity * Mathf.Cos(_camera_shake_theta * _camera_shake_speed.x);
			camera_shake.y = _camera_shake_intensity * Mathf.Sin(_camera_shake_theta * _camera_shake_speed.y);
			_camera_shake_intensity = SPUtil.drpt(_camera_shake_intensity,0,_camera_shake_friction);
			_last_shake = camera_shake;
		} else {
			_last_shake.x = SPUtil.drpt(_last_shake.x,0,1/10.0f);
			_last_shake.y = SPUtil.drpt(_last_shake.y,0,1/10.0f);
			camera_shake = _last_shake;
		}

		GameMain._context._game_camera.transform.localPosition = new Vector3(
			_camera_x._current + camera_shake.x,
			_camera_y._current + camera_shake.y,
			-_camera_zoom._current
		);
	}
	
	private SPRange get_camera_horiz_bounds() {
		Vector3 letterbox_offset = SPUtil.game_from_view_screen_offset();
		Vector3 world_left_anchor = GameMain._context.transform.TransformPoint(new Vector3(SPUtil.get_horiz_world_bounds()._min,0,0));
		Vector3 world_right_anchor = GameMain._context.transform.TransformPoint(new Vector3(SPUtil.get_horiz_world_bounds()._max,0,0));
		Vector3 screen_left_anchor = GameMain._context._game_camera.WorldToScreenPoint(world_left_anchor) - letterbox_offset;
		Vector3 screen_right_anchor = GameMain._context._game_camera.WorldToScreenPoint(world_right_anchor) - letterbox_offset;
		
		Vector3 camera_prev_lpos = GameMain._context._game_camera.transform.localPosition;
		GameMain._context._game_camera.transform.localPosition = GameMain._context._game_camera.transform.localPosition + new Vector3(-1,0,0);
		Vector3 tl1_screen_left_anchor = GameMain._context._game_camera.WorldToScreenPoint(world_left_anchor) - letterbox_offset;
		GameMain._context._game_camera.transform.localPosition = camera_prev_lpos;
		float camera_to_screen_delta_scf = (tl1_screen_left_anchor - screen_left_anchor).x; //camera 1, screen scf
		SPRange rtv = new SPRange() {
			_min = GameMain._context._game_camera.transform.localPosition.x + screen_left_anchor.x / camera_to_screen_delta_scf,
			_max = GameMain._context._game_camera.transform.localPosition.x + (screen_right_anchor.x - (Screen.width * GameMain._context._game_camera.rect.width)) / camera_to_screen_delta_scf
		};
		return rtv;
	}

	//quick jolt
	//g._camerac.camera_shake(new Vector2(-2,3.3f),20,60);
	//large jolt
	//g._camerac.camera_shake(new Vector2(-6,7.8f),40,150)
	private float _camera_shake_intensity, _camera_shake_ct, _camera_shake_theta, _camera_shake_friction = 1/30.0f;
	private Vector2 _camera_shake_speed;
	public void camera_shake(Vector2 speed, float intensity, float duration, float friction = 1/30.0f) {
		_camera_shake_speed = speed;
		_camera_shake_intensity = intensity;
		_camera_shake_ct = duration;
		_camera_shake_friction = friction;
	}
	
	private Vector3 _camera_motion_blur_mag;
	private float _camera_motion_blur_ct, _camera_motion_blur_ct_max;
	private bool _has_camera_motion_blur;
	public void camera_motion_blur(Vector3 mag, float duration) {
		_camera_motion_blur_mag = mag;
		_camera_motion_blur_ct = 0;
		_camera_motion_blur_ct_max = duration;
		_has_camera_motion_blur = true;
	}
	
	private float _blur_ct, _blur_ct_max;
	private bool _has_blur, _last_has_blur;
	public void camera_blur(float duration) {
		_blur_ct = 0;
		_blur_ct_max = duration;
		_has_blur = true;
		_last_has_blur = false;
	}

	public void i_update() {
		if (GameMain._context._camera_active) {
			_camera_shake_ct = Mathf.Max(0,_camera_shake_ct - SPUtil.dt_scale_get());
			_camera_shake_theta = (_camera_shake_theta + SPUtil.dt_scale_get() * 0.1f) % (2 * Mathf.PI);
			_camera_zoom.i_update();
			this.apply_camera_values();
			SPRange camera_horiz_range = this.get_camera_horiz_bounds();
			_camera_x.i_update();
			_camera_x._current = Mathf.Clamp(_camera_x._current,camera_horiz_range._min,camera_horiz_range._max);

			_camera_y.i_update();
			this.apply_camera_values();
			
			
			if (_has_camera_motion_blur) {
				_motion_blur.preview = true;
				float pct = Mathf.Clamp(SPUtil.bezier_val_for_t(
					new Vector2(0,1), new Vector2(0,1.5f), new Vector2(1,1.5f), new Vector2(1,0), _camera_motion_blur_ct / _camera_motion_blur_ct_max
					).y,0,1);
				_motion_blur.previewScale = SPUtil.vec_scale(_camera_motion_blur_mag,pct);
				_camera_motion_blur_ct += SPUtil.dt_scale_get();
				if (_camera_motion_blur_ct >= _camera_motion_blur_ct_max) {
					_has_camera_motion_blur = false;
				}
				
			} else {
				_motion_blur.preview = false;
			}
			
			if (_has_blur) {
				_ui_blur_cover.set_enabled(true);
				if (!_last_has_blur) Graphics.Blit(GameMain._context._game_camera_out,_game_camera_blur_copy);
				_ui_blur_cover.set_opacity(SPUtil.y_for_point_of_2pt_line(new Vector2(0,0.75f),new Vector2(1,0),_blur_ct/_blur_ct_max));
				_blur_ct += SPUtil.dt_scale_get();
				if (_blur_ct >= _blur_ct_max) {
					_has_camera_motion_blur = false;
				}
			} else {
				_ui_blur_cover.set_enabled(false);
			}
			_last_has_blur = _has_blur;
			
		}
	}

	public void set_target_camera_y(float tar) {
		_camera_y._target = tar;
	}
	public float get_current_camera_y() {
		return _camera_y._current;
	}
	
	public void set_target_camera_focus_on_character(GameEngineScene g, float offset_x = 0, float offset_y = 0) {
		SPRange camera_bounds = this.get_camera_horiz_bounds();
		_camera_x._target = (Mathf.Clamp(Mathf.Clamp(g._player._u_x,camera_bounds._min,camera_bounds._max) + offset_x,camera_bounds._min,camera_bounds._max));
		_camera_y._target = (g._player._u_y + offset_y);
	}
	
	public void set_target_zoom(float val) {
		_camera_zoom._target = (Mathf.Clamp(val,MIN_ZOOM,MAX_ZOOM));
	}
	public void set_zoom_speed(float val) {
		_camera_zoom._drptval = val;
	}
	public void set_camera_follow_speed(float val) {
		_camera_x._drptval = val;
		_camera_y._drptval = val;
	}
	
	public float get_zoom() {
		return _camera_zoom._current;
	}
	


}
