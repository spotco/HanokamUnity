using UnityEngine;
using System.Collections;

public class GameCameraController : SPGameUpdateable {

	public static GameCameraController cons(GameEngineScene g) {
		return (new GameCameraController()).i_cons(g);
	}

	public static float MIN_ZOOM = 10;
	public static float MAX_ZOOM = 1500;

	private ELMVal _camera_zoom = ELMVal.cons().set_target_vel(50.0f);
	private ELMVal _camera_x = ELMVal.cons().set_target_vel(50.0f);
	private ELMVal _camera_y = ELMVal.cons().set_target_vel(50.0f);
	
	public GameCameraController i_cons(GameEngineScene g) {
		_camera_zoom.set_current(1500);
		_camera_zoom.set_target(_camera_zoom.get_current());
		_camera_x.set_current(0);
		_camera_x.set_target(_camera_x.get_current());
		_camera_y.set_current(250);
		_camera_y.set_target(_camera_y.get_current());

		this.apply_camera_values();

		return this;
	}

	private void apply_camera_values() {
		GameMain._context._game_camera.transform.localPosition = new Vector3(_camera_x.get_current(),_camera_y.get_current(),-_camera_zoom.get_current());
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
	
	
	public void i_update(GameEngineScene g) {
		if (g._camera_active) {
			_camera_zoom.set_current(SPUtil.drpt(_camera_zoom.get_current(),_camera_zoom.get_target(),1/10.0f));
			this.apply_camera_values();
			
			SPRange camera_bounds = this.get_camera_horiz_bounds();
			_camera_x.set_current(Mathf.Clamp(SPUtil.drpt(_camera_x.get_current(),_camera_x.get_target(),1/10.0f),camera_bounds._min,camera_bounds._max));
			_camera_y.i_update(SPUtil.dt_scale_get());
			
			this.apply_camera_values();
		}
	}
	

	public void set_target_camera_y(float tar) {
		_camera_y.set_target(tar);
	}
	public float get_current_camera_y() {
		return _camera_y.get_target();
	}
	
	public void set_camera_focus_on_character(GameEngineScene g, float offset_x = 0, float offset_y = 0) {
		SPRange camera_bounds = this.get_camera_horiz_bounds();
		_camera_x.set_target(Mathf.Clamp(g._player._u_x + offset_x,camera_bounds._min,camera_bounds._max));
		_camera_y.set_target(g._player._u_y + offset_y);
	}
	
	public void set_zoom(float val) {
		_camera_zoom.set_target(Mathf.Clamp(val,MIN_ZOOM,MAX_ZOOM));
	}
	
	public float get_zoom() {
		return _camera_zoom.get_target();
	}
	


}
