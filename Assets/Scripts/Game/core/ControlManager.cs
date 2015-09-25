using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ControlManager : SPGameUpdateable {
	
	public enum Control {
		MoveLeft,
		MoveRight,
		OnGround_Jump,
		None
	}
	private static bool control_is_pressed(Control test) {
		switch (test) {
		case Control.MoveLeft: {
			return Input.GetKey(KeyCode.A);
		} break;
		case Control.MoveRight: {
			return Input.GetKey(KeyCode.D);
		} break;
		case Control.OnGround_Jump: {
			return Input.GetKey(KeyCode.Space);
		} break;
		}
		return false;
	}
	
	public static ControlManager cons() {
		return (new ControlManager()).i_cons();
	}
	
	private bool _is_move;
	private Vector2 _move_vec;
	private List<Control> _controls_to_test = new List<Control>();
	private Dictionary<Control,bool> _control_is_down = new Dictionary<Control, bool>();
	private Dictionary<Control,bool> _control_just_released = new Dictionary<Control, bool>();
	private Dictionary<Control,bool> _control_just_pressed = new Dictionary<Control, bool>();
	
	public ControlManager i_cons() {
		foreach (Control itr in System.Enum.GetValues(typeof(Control)).Cast<Control>()) {
			_controls_to_test.Add(itr);
			_control_is_down[itr] = false;
			_control_just_released[itr] = false;
			_control_just_pressed[itr] = false;
		}
		return this;
	}
	
	public void i_update(GameEngineScene g) {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		
		for (int i_test = 0; i_test < _controls_to_test.Count; i_test++) {
			Control itr_test = _controls_to_test[i_test];
			
			_control_just_pressed[itr_test] = false;
			_control_just_released[itr_test] = false;
			
			bool itr_test_pressed = this.control_is_pressed(itr_test);
			if (itr_test_pressed) {
				if (!_control_is_down[itr_test]) {
					_control_just_pressed[itr_test] = true;
				}
				_control_is_down[itr_test] = true;
			} else {
				if (_control_is_down[itr_test]) {
					_control_just_released[itr_test] = true;
				}
				_control_is_down[itr_test] = false;
			}
		}
	
		if (this.get_control_down(Control.MoveLeft)) {
			_move_vec.x = SPUtil.drpt(_move_vec.x,-1,1/20.0f);
			_is_move = true;
		} else if (this.get_control_down(Control.MoveRight)) {
			_move_vec.x = SPUtil.drpt(_move_vec.x,1,1/20.0f);
			_is_move = true;
		} else {
			_move_vec.x = 0;
			_is_move = false;
		}
	}
	
	public Vector2 get_cursor_move_delta() {
		Vector2 scf = new Vector2(
			SPUtil.game_screen().x * 0.1f,
			SPUtil.game_screen().y * 0.1f
		);
		return SPUtil.vec_mult(new Vector2(Input.GetAxis ("Mouse X"),Input.GetAxis ("Mouse Y")),scf);
	}
	
	public bool get_control_just_pressed(Control test) {
		return _control_just_pressed[test];
	}
	public bool get_control_just_released(Control test) {
		return _control_just_released[test];
	}
	public bool get_control_down(Control test) {
		return _control_is_down[test];
	}
	
	public bool is_move() { return _is_move; }
	public Vector2 get_move() { return _move_vec; }
}
