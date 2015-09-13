using UnityEngine;
using System.Collections;

public class ControlManager : SPGameUpdateable {

	public static ControlManager cons() {
		return (new ControlManager()).i_cons();
	}
	
	private bool _is_move;
	private Vector2 _move_vec;
	
	public ControlManager i_cons() {
		return this;
	}
	
	
	
	public void i_update(GameEngineScene g) {
		if (Input.GetKey(KeyCode.A)) {
			_move_vec.x = SPUtil.drpt(_move_vec.x,-1,1/20.0f);
			_is_move = true;
		} else if (Input.GetKey(KeyCode.D)) {
			_move_vec.x = SPUtil.drpt(_move_vec.x,1,1/20.0f);
			_is_move = true;
		} else {
			_move_vec.x = 0;
			_is_move = false;
		}
	}
	
	public bool is_move() { return _is_move; }
	public Vector2 get_move() { return _move_vec; }
	
	
	
}
