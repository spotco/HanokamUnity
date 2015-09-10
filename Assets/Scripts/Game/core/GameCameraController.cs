using UnityEngine;
using System.Collections;

public class GameCameraController : SPGameUpdateable {

	public static GameCameraController cons(GameEngineScene g) {
		return (new GameCameraController()).i_cons(g);
	}

	public static float MIN_ZOOM = 10;
	public static float MAX_ZOOM = 1500;

	private ELMVal _camera_zoom = ELMVal.cons();
	private ELMVal _camera_height = ELMVal.cons();
	
	public GameCameraController i_cons(GameEngineScene g) {
		_camera_zoom.set_current(1500);
		_camera_zoom.set_target_vel(50.0f);

		_camera_height.set_current(250);
		_camera_height.set_target_vel(50.0f);

		this.apply_camera_values();

		return this;
	}

	private void apply_camera_values() {
		GameMain._context._game_camera.transform.localPosition = new Vector3(0,_camera_height.get_current(),-_camera_zoom.get_current());
	}
	
	public void i_update(GameEngineScene g) {
		_camera_zoom.i_update(SPUtil.dt_scale_get());
		_camera_height.i_update(SPUtil.dt_scale_get());
		this.apply_camera_values();
	}
	

	public void set_tar_camera_height(float tar) {
		_camera_height.set_target(tar);
	}
	
	public void set_zoom(float val) {
		_camera_zoom.set_target(val);
	}
	
	public float get_zoom() {
		return _camera_zoom.get_target();
	}
	
	public float get_current_camera_height() {
		return _camera_height.get_target();
	}

}
