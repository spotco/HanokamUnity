using UnityEngine;
using System.Collections;

public abstract class BasicWaterEnemy : BaseWaterEnemy {

	public enum Mode {
		IdleMove,
		IdleNoticed,
		InPack,
		Stunned
	}
	
	public enum DiveReturnMode {
		Normal,
		Hit
	}
	
	public Mode _current_mode;
	public DiveReturnMode _current_divereturn_mode; 
	
	private Vector2 _pt1, _pt2;
	private float _idle_move_anim_theta;
	private Vector2 _last_pos, _stun_vel;
	private float _stunned_anim_ct_max;
	private float _anim_theta;
	private float _stunned_anim_ct;
	private float _last_move_rotation;
	
	private float _idle_noticed_ct;
	
	private float _pack_lr_theta;
	private float _pack_ud_theta;
	private float _pack_lr_vtheta, _pack_ud_vtheta;
	private float _pack_cur_vel;
	private float _pack_x_var,_pack_y_var;
	
	private SPNode _root;
	
	protected BasicWaterEnemy i_cons(GameEngineScene g, Vector2 pt1, Vector2 pt2) {
		_root = SPNode.cons_node();
		_root.set_name("BasicWaterEnemy");
		
		this._current_mode = Mode.IdleMove;
		this._current_divereturn_mode = DiveReturnMode.Normal;
		
		_pt1 = pt1;
		_pt2 = pt2;
		_last_pos = Vector2.zero;
		this.set_u_pos(_pt1.x,_pt1.y);
		_idle_move_anim_theta = SPUtil.float_random(-3.14f,3.14f);
		_pack_lr_theta = SPUtil.float_random(-3.14f, 3.14f);
		_pack_ud_theta = SPUtil.float_random(-3.14f, 3.14f);
		_pack_lr_vtheta = SPUtil.float_random(0.045f, 0.065f);
		_pack_ud_vtheta = SPUtil.float_random(0.03f, 0.04f);
		_pack_x_var = SPUtil.float_random(70f, SPUtil.get_horiz_world_bounds()._max);
		_pack_y_var = SPUtil.float_random(70f, 150f);
		return this;
	}
	
	public BasicWaterEnemy set_u_pos(float x, float y) { _root.set_u_pos(x,y); return this; } 
	public BasicWaterEnemy set_u_pos(Vector2 pos) { return this.set_u_pos(pos.x, pos.y); }
	public SPNode get_root() { return _root; }
	public override void add_to_parent(SPNode parent) {
		parent.add_child(_root);
	}
	public override void do_remove(GameEngineScene g, DiveGameState state) {
		_root.repool();
		_root = null;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		if (g.get_top_game_state().get_state() == GameStateIdentifier.Dive) {
			this.i_update_dive(g, state);
		}
	}
	
	private void i_update_dive(GameEngineScene g, DiveGameState state) {
		switch (this._current_mode) {
		case Mode.IdleMove: {
			_idle_move_anim_theta += 0.025f * SPUtil.dt_scale_get();
			this.set_u_pos(Vector2.Lerp(_pt1,_pt2,(Mathf.Sin(_idle_move_anim_theta)+1)/2.0f));
			
		} break;
		case Mode.IdleNoticed: {
		
		} break;
		case Mode.Stunned: {
		
		} break;
		case Mode.InPack: {
		
		} break;
		}
	}
	
	
	

}
