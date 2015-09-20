using UnityEngine;
using System.Collections.Generic;

public interface SPGameUpdateable {
	void i_update(GameEngineScene g);
}

public class GameEngineScene : SPScene {
	
	[SerializeField] public bool _camera_active;
	
	public static GameEngineScene cons() {
		GameEngineScene rtv = GameMain._context.gameObject.AddComponent<GameEngineScene>();
		return rtv.i_cons();
	}
	
	public GameUI _game_ui;
	public BGVillage _bg_village;
	public BGSky _bg_sky;
	public BGWater _bg_water;
	private List<SPGameUpdateable> _bg_elements;
	public GameCameraController _camerac;
	public ControlManager _controls;
	public PlayerCharacter _player;
	
	
	private List<GameStateBase> _game_state_stack;

	private GameEngineScene i_cons() {
		__cached_viewbox_dirty = true;
		_camera_active = true;
		
		_game_state_stack = new List<GameStateBase>(){ IdleGameState.cons() };
		_game_state_stack.Add(OnGroundGameState.cons(this));
		
		_game_ui = GameUI.cons(this);
		
		_camerac = GameCameraController.cons(this);
		_controls = ControlManager.cons();

		_bg_village = BGVillage.cons(this);
		_bg_sky = BGSky.cons(this);
		_bg_water = BGWater.cons(this);
		_bg_elements = new List<SPGameUpdateable>() {_bg_village,_bg_sky,_bg_water};
		
		_player = PlayerCharacter.cons(this);
		_player.set_u_pos(0,0);

		return this;
	}
	
	public GameStateBase get_top_game_state() { return _game_state_stack[_game_state_stack.Count-1]; }
	
	public override void i_update(float dt_scale) {
		SPUtil.dt_scale_set(dt_scale);
		__cached_viewbox_dirty = true;
		
		_controls.i_update(this);
		_camerac.i_update(this);
		for (int i = 0; i < _bg_elements.Count; i++) {
			SPGameUpdateable itr = _bg_elements[i];
			itr.i_update(this);
		}
		this.get_top_game_state().i_update(this);
		_game_ui.i_update(this);
	}

	private SPHitRect __cached_viewbox;
	private bool __cached_viewbox_dirty;
	public SPHitRect get_viewbox() {
		if (!__cached_viewbox_dirty) return __cached_viewbox;
		__cached_viewbox_dirty = false;
		__cached_viewbox = get_viewbox_dist(0);
		return __cached_viewbox;
	}

	public SPHitRect get_viewbox_dist(float offset_dist) {
		Vector3 bl = GameMain._context._game_camera.ScreenToWorldPoint(
			new Vector3(
			GameMain._context._game_camera.rect.x * SPUtil.view_screen().x,
			GameMain._context._game_camera.rect.y * SPUtil.view_screen().y,
			Mathf.Abs(GameMain._context._game_camera.transform.position.z - offset_dist)
		));
		Vector3 tr = GameMain._context._game_camera.ScreenToWorldPoint(
			new Vector3(
			SPUtil.game_screen().x + GameMain._context._game_camera.rect.x * SPUtil.view_screen().x,
			SPUtil.game_screen().y + GameMain._context._game_camera.rect.y * SPUtil.view_screen().y,
			Mathf.Abs(GameMain._context._game_camera.transform.position.z - offset_dist)
		));
		bl = GameMain._context.transform.InverseTransformPoint(bl);
		tr = GameMain._context.transform.InverseTransformPoint(tr);
		return new SPHitRect(){ _x1 = bl.x, _y1 = bl.y, _x2 = tr.x, _y2 = tr.y };
	}
	
	public bool is_camera_underwater() {
		return GameMain._context._game_camera.transform.localPosition.y < -100;
	}


}
