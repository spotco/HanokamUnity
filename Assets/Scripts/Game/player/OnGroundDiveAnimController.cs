using UnityEngine;
using System.Collections;

public class OnGroundDiveAnimController : OnGroundStateUpdateable {

	public static OnGroundDiveAnimController cons() { return (new OnGroundDiveAnimController()); }
	
	public void setup_dive_anim(GameEngineScene g, OnGroundGameState state) {
	
	}
	
	public void i_update(GameEngineScene g, OnGroundGameState state) {
		Debug.Log("dive anim controller update");
	}
	
	public bool is_finished_and_should_transition_to_divestate() {
		return true;
	}
	
}
