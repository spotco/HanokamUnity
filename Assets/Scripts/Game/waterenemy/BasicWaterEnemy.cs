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
	
	protected Mode _current_mode;
	protected DiveReturnMode _current_divereturn_mode; 
	
	private Vector2 _pt1, _pt2;
	private float _idle_move_anim_theta;
	private Vector2 _last_pos, _stun_vel;
	private float _stunned_anim_ct_max;
	private float _anim_theta;
	protected float _stunned_anim_ct;
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
		_pack_y_var = SPUtil.float_random(100f, 300f);
		return this;
	}
	
	public BasicWaterEnemy set_u_pos(float x, float y) { _root.set_u_pos(x,y); return this; } 
	public BasicWaterEnemy set_u_pos(Vector2 pos) { return this.set_u_pos(pos.x, pos.y); }
	private void set_target_rotation(float val) { 
		_root.set_rotation(SPUtil.drpt(_root.rotation(), _root.rotation() + SPUtil.shortest_angle(_root.rotation(),val), 1/10.0f)); 
	}
	public SPNode get_root() { return _root; }
	public override void add_to_parent(SPNode parent) {
		parent.add_child(_root);
	}
	public override void do_remove(GameEngineScene g) {
		_root.repool();
		_root = null;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		switch (this._current_mode) {
		case Mode.IdleMove: {
			_idle_move_anim_theta += 0.025f * SPUtil.dt_scale_get();
			Vector2 neu_pos = Vector2.Lerp(_pt1,_pt2,(Mathf.Sin(_idle_move_anim_theta)+1)/2.0f);
			this.set_u_pos(neu_pos.x, neu_pos.y);
			if (g._player.get_center().y <= _root._u_y + 100) {
				_current_mode = Mode.IdleNoticed;
				_idle_noticed_ct = 40;
				g.add_particle(EnemyNoticeParticle.cons(new Vector2(_root._u_x,_root._u_y+80), SPUtil.dir_ang_deg(
					g._player._u_x-_root._u_x,g._player._u_y-_root._u_y
				)));
			}
			Vector2 delta = SPUtil.vec_sub(neu_pos,_last_pos);
			float tar_rotation = SPUtil.dir_ang_deg(delta.x,delta.y) - 90;
			this.set_target_rotation(tar_rotation);

			this.check_hit(g, state);
			_last_pos = neu_pos;
			_last_move_rotation = this.get_root().rotation();
			
		} break;
		case Mode.IdleNoticed: {
			this.set_target_rotation(SPUtil.dir_ang_deg(g._player.get_center().x-_root._u_x,g._player.get_center().y-_root._u_y) - 90);
			_idle_noticed_ct -= SPUtil.dt_scale_get();
			if (_idle_noticed_ct <= 0) {
				_current_mode = Mode.InPack;
			}
			this.check_hit(g,state);
			_last_move_rotation = this.get_root().rotation();

		} break;
		case Mode.Stunned: {
			_stunned_anim_ct -= SPUtil.dt_scale_get();
			_anim_theta = (_anim_theta + SPUtil.dt_scale_get() / (6.28f)) % 6.28f;
			_root.set_rotation(
				_last_move_rotation
					+ SPUtil.lerp(30, 5, 1-_stunned_anim_ct/_stunned_anim_ct_max)
					* Mathf.Sin(_anim_theta * SPUtil.lerp(1,7,1-_stunned_anim_ct/_stunned_anim_ct_max))
			);
			this.set_u_pos(SPUtil.vec_add(new Vector2(_root._u_x,_root._u_y),SPUtil.vec_scale(_stun_vel,SPUtil.dt_scale_get())));
			_stun_vel.x = SPUtil.drpt(_stun_vel.x,0,1/20.0f);
			_stun_vel.y = SPUtil.drpt(_stun_vel.y,0,1/20.0f);
			if (_stunned_anim_ct <= 0) {
				_current_mode = Mode.InPack;
			}
		
		} break;
		case Mode.InPack: {
			this.in_pack_update(g);
			this.check_hit(g, state);

		} break;
		}
	}

	private void in_pack_update(GameEngineScene g, float height_offset = 200) {
		this.set_target_rotation(SPUtil.dir_ang_deg(g._player.get_center().x-_root._u_x,g._player.get_center().y-_root._u_y) - 90);
		_pack_ud_theta += _pack_ud_vtheta * SPUtil.dt_scale_get();
		_pack_lr_theta += _pack_lr_vtheta * SPUtil.dt_scale_get();

		Vector2 target_pos = SPUtil.vec_add(
			g._player.get_center(),
			new Vector2(_pack_x_var * Mathf.Sin(_pack_lr_theta), _pack_y_var * Mathf.Cos(_pack_ud_theta) + _pack_y_var + height_offset));

		Vector2 delta = SPUtil.vec_cons_norm(target_pos.x-_root._u_x,target_pos.y-_root._u_y);
		_pack_cur_vel = Mathf.Min(
			SPUtil.drpt(_pack_cur_vel, 13, 1/15.0f),
			SPUtil.vec_dist(target_pos,new Vector2(_root._u_x,_root._u_y)));
		delta = SPUtil.vec_scale(delta,_pack_cur_vel);

		_root._u_x += delta.x * SPUtil.dt_scale_get();
		_root._u_y += delta.y * SPUtil.dt_scale_get();
		_last_move_rotation = _root.rotation();
	}

	public override void i_update(GameEngineScene g, DiveReturnGameState state) {
		switch (_current_divereturn_mode) {
		case DiveReturnMode.Normal: {
			this.in_pack_update(g, 500);

		} break;
		case DiveReturnMode.Hit: {
			_stunned_anim_ct -= SPUtil.dt_scale_get();
			_anim_theta = (_anim_theta + SPUtil.dt_scale_get() / (6.28f)) % 6.28f;
			_root.set_rotation(
				_last_move_rotation
					+ SPUtil.lerp(30, 5, 1-_stunned_anim_ct/_stunned_anim_ct_max)
					* Mathf.Sin(_anim_theta * SPUtil.lerp(1,7,1-_stunned_anim_ct/_stunned_anim_ct_max))
			);
			this.set_u_pos(SPUtil.vec_add(new Vector2(_root._u_x,_root._u_y),SPUtil.vec_scale(_stun_vel,SPUtil.dt_scale_get())));
			_stun_vel.x = SPUtil.drpt(_stun_vel.x,0,1/20.0f);
			_stun_vel.y = SPUtil.drpt(_stun_vel.y,0,1/20.0f);
			if (_stunned_anim_ct <= 0) {
				_current_divereturn_mode = DiveReturnMode.Normal;
			}

		} break;
		}
		float dist = SPUtil.vec_dist(g._player.get_center(), new Vector2(_root._u_x, _root._u_y));
		if (dist < 100) {
			_current_divereturn_mode = DiveReturnMode.Hit;
			Vector2 dir_vec = SPUtil.vec_cons_norm(_root._u_x-g._player.get_center().x,_root._u_y-g._player.get_center().y);
			dir_vec.y *= 0.75f;
			dir_vec = SPUtil.vec_scale(dir_vec,SPUtil.y_for_point_of_2pt_line(new Vector2(0,25),new Vector2(100,10),dist));
			_stun_vel = dir_vec;
			_stunned_anim_ct = _stunned_anim_ct_max = 50;
		}

		_last_move_rotation = _root.rotation();
		_last_pos = new Vector2(_root._u_x,_root._u_y);
	}

	public void check_hit(GameEngineScene g, DiveGameState state) {
		if (this.is_active() && state._params._mode == DiveGameState.Mode.Gameplay && SPHitPoly.polyowners_intersect(this, g._player)) {
			_current_mode = Mode.Stunned;

			_stunned_anim_ct = _stunned_anim_ct_max = 50;
			Vector2 pos_delta = (new Vector2(_root._u_x-g._player.get_center().x, _root._u_y-g._player.get_center().y)).normalized;
			pos_delta = SPUtil.vec_scale(pos_delta,20);
			_stun_vel = pos_delta;
		}

	}

	private bool is_active() {
		return _current_mode == Mode.IdleMove || _current_mode == Mode.IdleNoticed || _current_mode == Mode.InPack;
	}
	
	
	

}
