using UnityEngine;
using System.Collections.Generic;

public abstract class BasicAirEnemyModeComponent {
	public virtual void notify_spawn_at_pos(GameEngineScene g, BasicAirEnemy enemy, Vector2 pos) {}
	public virtual void notify_transition_to_state(GameEngineScene g, BasicAirEnemy enemy) {}
	public virtual void notify_transition_from_state(GameEngineScene g, BasicAirEnemy enemy) {}
	public virtual void i_update(GameEngineScene g, InAirGameState state, BasicAirEnemy enemy) {}
}

public abstract class BasicAirEnemy : BaseAirEnemy, GenericPooledObject {
	private SPNode _root;
	public SPNode get_root() { return _root; }
	public override void add_to_parent(SPNode parent) { parent.add_child(_root); }
	
	public virtual void depool() {
		_root = SPNode.cons_node();
		_root.set_name("BasicAirEnemy");
	}
	public virtual void repool() {
		_root.repool();
		_root = null;
	}
	
	public override void do_remove() {
		_root.repool();
		_root = null;
	}
	
	public struct Params {
		public Vector2 _last_move_delta;
		public Vector2 _stun_dir;
		public float _stun_ct, _stun_ct_max;
	}
	public Params _params;
	
	public enum Mode {
		Unspawned,
		Moving,
		Stunned,
		Dying,
		DoRemove
	}
	private Mode _current_mode;
	public Mode get_current_mode() { return _current_mode; }
	private SPDict<Mode,BasicAirEnemyModeComponent> _mode_to_state;
	
	protected BasicAirEnemy i_cons() {
		_current_mode = Mode.Unspawned;
		_params = new Params();
		_mode_to_state = new SPDict<Mode, BasicAirEnemyModeComponent>();
		_root.set_enabled(false);
		return this;
	}
	
	public BasicAirEnemy add_component_for_mode(Mode mode, BasicAirEnemyModeComponent state) {
		_mode_to_state[mode] = state;
		return this;
	}
	
	public override void spawn_at_c_position(GameEngineScene g, Vector2 pos) {
		_current_mode = Mode.Moving;
		_root.set_enabled(true);
		_root.set_u_pos(pos.x,pos.y);
		for (int i = 0; i < _mode_to_state.key_itr().Count; i++) {
			_mode_to_state[_mode_to_state.key_itr()[i]].notify_spawn_at_pos(g, this, pos);
		}
	}
	
	public void transition_to_mode(GameEngineScene g, Mode mode) {
		if (_mode_to_state.ContainsKey(_current_mode)) _mode_to_state[_current_mode].notify_transition_from_state(g, this);
		_current_mode = mode;
		if (_mode_to_state.ContainsKey(_current_mode)) _mode_to_state[_current_mode].notify_transition_to_state(g, this);
	}
	
	public override void i_update(GameEngineScene g, InAirGameState state) {
		if (_mode_to_state.ContainsKey(_current_mode)) {
			_mode_to_state[_current_mode].i_update(g,state,this);
		}
	}
	
	public void apply_c_pos(Vector2 c_pos) {
		_root.set_u_pos(GameCameraController.c_pos_to_u_pos(c_pos));
	}
	
	public override bool should_remove() { return _current_mode == Mode.DoRemove; }
}


public class CurveMoveBasicAirEnemyModeComponent : BasicAirEnemyModeComponent {
	
	private Vector2 _bez_start, _bez_end;
	private float _bez_t;
	public static CurveMoveBasicAirEnemyModeComponent cons(Vector2 end_pt) {
		return (new CurveMoveBasicAirEnemyModeComponent()).i_cons(end_pt);
	}
	private CurveMoveBasicAirEnemyModeComponent i_cons(Vector2 end_pt) {
		_bez_end = end_pt;
		_bez_t = 0;
		_offset_c_pos = Vector2.zero;
		return this;
	}
	public override void notify_spawn_at_pos(GameEngineScene g, BasicAirEnemy enemy, Vector2 pos) {
		_bez_start = pos;
		enemy.get_root().set_u_pos(GameCameraController.c_pos_to_u_pos(pos));
	}
	
	private Vector2 _offset_c_pos;
	private Vector2 _transition_from_last_c_pos;
	public override void notify_transition_to_state(GameEngineScene g, BasicAirEnemy enemy) {
		Vector2 transition_to_c_pos = GameCameraController.u_pos_to_c_pos(enemy.get_root().get_u_pos());
		_offset_c_pos = SPUtil.vec_add(_offset_c_pos, SPUtil.vec_sub(transition_to_c_pos,_transition_from_last_c_pos));
	}
	public override void notify_transition_from_state(GameEngineScene g, BasicAirEnemy enemy) {
		_transition_from_last_c_pos = GameCameraController.u_pos_to_c_pos(enemy.get_root().get_u_pos());
	}
	
	public override void i_update(GameEngineScene g, InAirGameState state, BasicAirEnemy enemy) {
		bool out_of_bounds = 
			enemy.get_root()._u_x >= SPUtil.get_horiz_world_bounds()._max + 200 ||
			enemy.get_root()._u_x <= SPUtil.get_horiz_world_bounds()._min - 200 ||
			enemy.get_root()._u_y <= g.get_viewbox()._y1 - 500;
							
		if (_bez_t <= 1) {
			_bez_t += 0.005f * SPUtil.dt_scale_get();
			Vector2 bez_ctrl1 = new Vector2(_bez_start.x, _bez_end.y + 100);
			Vector2 bez_ctrl2 = SPUtil.vec_mid(bez_ctrl1,_bez_end);
			Vector2 enemy_pos_pre = enemy.get_root().get_u_pos();
			enemy.apply_c_pos(SPUtil.vec_add(SPUtil.bezier_val_for_t(
				_bez_start,
				bez_ctrl1,
				bez_ctrl2,
				_bez_end,
				_bez_t
			),_offset_c_pos));
			Vector2 enemy_pos_post = enemy.get_root().get_u_pos();
			enemy._params._last_move_delta = SPUtil.vec_sub(enemy_pos_post,enemy_pos_pre);
			enemy.get_root().set_rotation(
				SPUtil.drpt(
					enemy.get_root().rotation(), 
					enemy.get_root().rotation() + SPUtil.shortest_angle(
						enemy.get_root().rotation(),
						SPUtil.dir_ang_deg(enemy._params._last_move_delta.x,enemy._params._last_move_delta.y)) - 90, 
					1/10.0f
			));
		} else {
			enemy.apply_c_pos(SPUtil.vec_add(GameCameraController.u_pos_to_c_pos(enemy.get_root().get_u_pos()),enemy._params._last_move_delta));
		}
				
		if (SPHitPoly.polyowners_intersect(enemy, g._player)) {
			if (state._params.is_check_dash()) {
				BasicAirEnemyModeComponentUtility.dash_hit(g,state,enemy);
			} else if (state._params.is_check_swordplant()) {
				BasicAirEnemyModeComponentUtility.swordplant_hit(g,state,enemy);
			} else {
				BasicAirEnemyModeComponentUtility.none_hit(g,state,enemy);
			}
			g._camerac.freeze_frame(2);
			g._camerac.camera_shake(new Vector2(-1.5f,1.7f),15,30);
			
		} else if (out_of_bounds) {
			enemy.transition_to_mode(g, BasicAirEnemy.Mode.DoRemove);
		}
	}
}

public class KnockbackStunBasicAirEnemyModeComponent : BasicAirEnemyModeComponent {
	public static KnockbackStunBasicAirEnemyModeComponent cons() { return new KnockbackStunBasicAirEnemyModeComponent(); }
	
	private Vector2 _c_pos;
	public override void notify_transition_to_state(GameEngineScene g, BasicAirEnemy enemy) {
		_c_pos = GameCameraController.u_pos_to_c_pos(enemy.get_root().get_u_pos());
	}
	public override void i_update(GameEngineScene g, InAirGameState state, BasicAirEnemy enemy) {
		_c_pos = SPUtil.vec_add(_c_pos,SPUtil.vec_scale(enemy._params._stun_dir,SPUtil.dt_scale_get()));
		enemy._params._stun_dir.x = SPUtil.drpt(enemy._params._stun_dir.x,0,1/20.0f);
		enemy._params._stun_dir.y = SPUtil.drpt(enemy._params._stun_dir.y,0,1/20.0f);
		enemy.apply_c_pos(_c_pos);
		
		enemy._params._stun_ct += SPUtil.dt_scale_get();
		if (enemy._params._stun_ct >= enemy._params._stun_ct_max) {
			enemy.transition_to_mode(g,BasicAirEnemy.Mode.Moving);
		}
		
		if (SPHitPoly.polyowners_intersect(enemy, g._player)) {
			if (state._params.is_check_dash()) {
				BasicAirEnemyModeComponentUtility.dash_hit(g,state,enemy);
				g._camerac.freeze_frame(2);
			} else if (state._params.is_check_swordplant()) {
				BasicAirEnemyModeComponentUtility.swordplant_hit(g,state,enemy);
				g._camerac.freeze_frame(2);
			} else {
				BasicAirEnemyModeComponentUtility.none_hit(g,state,enemy);
			}
			
			g._camerac.camera_shake(new Vector2(-1.5f,1.7f),15,30);	
		}
	}
}

public class BasicAirEnemyModeComponentUtility {
	public static void dash_hit(GameEngineScene g, InAirGameState state, BasicAirEnemy enemy) {
		state._params._dash_ct = 20;
		g.add_particle(AirSwordSlashParticle.cons(enemy.get_root().get_u_pos(),state._params._player_c_vel));
		enemy.transition_to_mode(g, BasicAirEnemy.Mode.Dying);
	}
	public static void swordplant_hit(GameEngineScene g, InAirGameState state, BasicAirEnemy enemy) {
		state._params._pre_dash_rotation = g._player.rotation();
		state._params._player_mode = InAirGameState.Params.PlayerMode.Dash;
		state._params._dash_ct = 20;
		state._params._player_c_vel.y = 30;
		state._params._this_dash_can_become_swordplant = false;
		state._params._this_dash_ignore_move_y_ct = 20;
		enemy.transition_to_mode(g, BasicAirEnemy.Mode.Dying);
		{
			SPConfigAnimParticle neu_particle = SPConfigAnimParticle.cons();
			neu_particle.set_ctmax(15);
			neu_particle.set_manual_sort_z_order(GameAnchorZ.Player_FX-1);
			neu_particle.set_anchor_point(0.5f,0.3f);
			neu_particle.set_pos(g._player.get_u_pos().x,g._player.get_u_pos().y);
			neu_particle.set_scale(0.7f,0.3f);
			neu_particle.set_texture(TextureResource.inst().get_tex(RTex.HANOKA_EFFECTS));
			neu_particle.set_texrect(new Rect());
			neu_particle.set_normalized_timed_sprite_animator(SPTimedSpriteAnimator.cons(null)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"sword_plant_hit_000.png"),0)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"sword_plant_hit_001.png"),0.2f)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"sword_plant_hit_002.png"),0.4f)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"sword_plant_hit_003.png"),0.6f)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"sword_plant_hit_004.png"),0.8f)
			);
			g.add_particle(neu_particle);
		}
		{
			SPConfigAnimParticle neu_particle = SPConfigAnimParticle.cons();
			neu_particle.set_ctmax(13);
			neu_particle.set_manual_sort_z_order(GameAnchorZ.Player_FX);
			neu_particle.set_pos(g._player.get_u_pos().x,g._player.get_u_pos().y);
			neu_particle.set_scale(0.4f,0.25f);
			neu_particle.set_texture(TextureResource.inst().get_tex(RTex.HANOKA_EFFECTS));
			neu_particle.set_texrect(new Rect());
			neu_particle.set_normalized_timed_sprite_animator(SPTimedSpriteAnimator.cons(null)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"stab_000.png"),0)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"stab_001.png"),0.3f)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"stab_002.png"),0.6f)
			);
			g.add_particle(neu_particle);
		}
	}
	public static void none_hit(GameEngineScene g, InAirGameState state, BasicAirEnemy enemy) {
		enemy._params._stun_ct = 0;
		enemy._params._stun_ct_max = 150;
		enemy._params._stun_dir = SPUtil.vec_scale(SPUtil.vec_sub(enemy.get_root().get_u_pos(),g._player.get_center()).normalized,10);
		
		state._params._player_mode = InAirGameState.Params.PlayerMode.Hurt;
		state._params._hurt_ct = 25;
		state._params._player_c_vel = SPUtil.vec_scale(SPUtil.vec_sub(g._player.get_u_pos(),enemy.get_root().get_u_pos()).normalized,15);
		
		enemy.transition_to_mode(g, BasicAirEnemy.Mode.Stunned);
	}
}

public class DeathAnimDelayBasicAirEnemyModeComponent : BasicAirEnemyModeComponent {
	public static DeathAnimDelayBasicAirEnemyModeComponent cons(float delay) { return (new DeathAnimDelayBasicAirEnemyModeComponent()).i_cons(delay); }
	private float _ct;
	private DeathAnimDelayBasicAirEnemyModeComponent i_cons(float delay) {
		_ct = delay;
		return this;
	}
	public override void i_update(GameEngineScene g, InAirGameState state, BasicAirEnemy enemy) {
		_ct -= SPUtil.dt_scale_get();
		if (_ct <= 0) {
			enemy.transition_to_mode(g,BasicAirEnemy.Mode.DoRemove);
		}
	}
}
