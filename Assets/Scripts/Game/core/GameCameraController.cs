using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class GameCameraController : SPMainUpdateable {

	public static GameCameraController cons() {
		return (new GameCameraController()).i_cons();
	}

	public static float MIN_ZOOM = 10;
	public static float MAX_ZOOM = 1500;

	private DrptVec _camera_pos;

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
		_motion_blur.preview = true;
		
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
		_camera_pos = new DrptVec() {
			_current = new Vector3(0,250,1500),
			_target = new Vector3(0,250,1500),
			_drptval = 1/10.0f
		};
		this.apply_camera_values();
	}
	public void SetShopSceneDefaults() {
		_camera_pos = new DrptVec() {
			_current = new Vector3(0,-680,1500),
			_target = new Vector3(0,-680,1500),
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
			_camera_pos._current.x + camera_shake.x,
			_camera_pos._current.y + camera_shake.y,
			-_camera_pos._current.z
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
		rtv._min = Mathf.Min(rtv._min,0);
		rtv._max = Mathf.Max(rtv._max,0);
		return rtv;
	}
	private SPRange get_camera_horiz_bounds_at_zoom(float zoom) {
		Vector3 camera_pos_pre = GameMain._context._game_camera.transform.localPosition;
		GameMain._context._game_camera.transform.localPosition = new Vector3(0,0,-zoom);
		SPRange rtv = this.get_camera_horiz_bounds();
		GameMain._context._game_camera.transform.localPosition = camera_pos_pre;
		return rtv;
	}
	
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
	
	public float _freeze_frame;
	public void freeze_frame(float duration) {
		_freeze_frame = duration;
	}
	
	private Vector3 _last_frame_camera_position;
	private Vector3 _avg_frame_camera_delta;
	public void i_update() {
		if (GameMain._context._camera_active) {
			_camera_shake_ct = Mathf.Max(0,_camera_shake_ct - SPUtil.dt_scale_get());
			_camera_shake_theta = (_camera_shake_theta + SPUtil.dt_scale_get() * 0.1f) % (2 * Mathf.PI);
			_camera_pos.i_update();
			this.apply_camera_values();
			
			_avg_frame_camera_delta = SPUtil.running_avg_vec(_avg_frame_camera_delta,SPUtil.vec_sub(_camera_pos._current,_last_frame_camera_position),3.0f);
			_last_frame_camera_position = _camera_pos._current;
			float frame_camera_move_dist = _avg_frame_camera_delta.magnitude;
			float MIN_BLUR_DIST = 5;
			
			if (_has_camera_motion_blur) {
				_motion_blur.enabled = true;
				float pct = Mathf.Clamp(SPUtil.bezier_val_for_t(
					new Vector2(0,1), new Vector2(0,1.5f), new Vector2(1,1.5f), new Vector2(1,0), _camera_motion_blur_ct / _camera_motion_blur_ct_max
					).y,0,1);
				_motion_blur.previewScale = SPUtil.vec_scale(_camera_motion_blur_mag,pct);
				_camera_motion_blur_ct += SPUtil.dt_scale_get();
				if (_camera_motion_blur_ct >= _camera_motion_blur_ct_max) {
					_has_camera_motion_blur = false;
				}
				
			} else if (frame_camera_move_dist > MIN_BLUR_DIST) {
				_motion_blur.enabled = true;
				
				Vector3 blur = _avg_frame_camera_delta * 
					SPUtil.bezier_val_for_t(new Vector2(0,0),new Vector2(0f,0.5f),new Vector2(0.5f,1.0f),new Vector2(1,1),
						Mathf.Clamp(SPUtil.y_for_point_of_2pt_line(new Vector2(MIN_BLUR_DIST,0),new Vector2(150,1),frame_camera_move_dist),0,1)).y;
				blur.z *= -1;
				_motion_blur.previewScale = blur;
				
			} else {
				_motion_blur.enabled = false;
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
		_camera_pos._target.y = tar;
	}
	public float get_current_camera_y() {
		return _camera_pos._current.y;
	}
	public float get_target_camera_y() {
		return _camera_pos._target.y;
	}
	
	// set target zoom first before setting focus on character
	public void set_target_zoom(float val) {
		_camera_pos._target.z = (Mathf.Clamp(val,MIN_ZOOM,MAX_ZOOM));
	}
	public void set_target_camera_focus_on_character(GameEngineScene g, float offset_x = 0, float offset_y = 0) {
		SPRange camera_bounds = this.get_camera_horiz_bounds_at_zoom(_camera_pos._target.z);
		_camera_pos._target.x = (Mathf.Clamp(Mathf.Clamp(g._player._u_x,camera_bounds._min,camera_bounds._max) + offset_x,camera_bounds._min,camera_bounds._max));
		_camera_pos._target.y = (g._player._u_y + offset_y);
	}
	
	public float get_zoom() {
		return _camera_pos._current.z;
	}

	public static Vector2 u_pos_to_c_pos(Vector2 vec) { return GameCameraController.u_pos_to_c_pos(vec.x,vec.y); }
	public static Vector2 u_pos_to_c_pos(float x, float y) {
		Vector2 u_pos = new Vector2(x,y);
		Vector2 camera = GameMain._context._camerac._camera_pos._current;
		return SPUtil.vec_sub(u_pos, new Vector2(camera.x,camera.y));
	}
	public static Vector2 c_pos_to_u_pos(Vector2 vec) { return GameCameraController.c_pos_to_u_pos(vec.x,vec.y); }
	public static Vector2 c_pos_to_u_pos(float x, float y) {
		Vector2 c_pos = new Vector2(x,y);
		Vector2 camera = GameMain._context._camerac._camera_pos._current;
		return SPUtil.vec_add(c_pos,new Vector2(camera.x,camera.y));
	}


}
