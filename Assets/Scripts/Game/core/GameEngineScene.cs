using UnityEngine;
using System.Collections.Generic;

public interface SPGameUpdateable {
	void i_update(GameEngineScene g);
}

public class GameEngineScene : SPScene {
	
	public static GameEngineScene cons() {
		return (new GameEngineScene()).i_cons();
	}
	
	private SPNode _root;
	public override void set_enabled(bool val) {
		_root.set_enabled(val);
		if (val) {
			_camerac.SetGameEngineSceneDefaults();
		}
	}
	
	public BGVillage _bg_village;
	public BGSky _bg_sky;
	public BGWater _bg_water;
	
	public GameCameraController _camerac { get { return GameMain._context._camerac; } }
	public ControlManager _controls { get { return GameMain._context._controls; } }
	public UIRoot _game_ui { get { return GameMain._context._game_ui; } }
	
	public PlayerCharacter _player;
	public DelayActionQueue _delayed_actions;

	private List<SPGameUpdateable> _bg_elements;
	private SPParticleSystem<SPGameEngineParticle> _particles;
	private SPNode _particle_root;
	public void add_particle(SPGameEngineParticle particle) {
		_particles.add_particle(particle);
		particle.add_to_parent(_particle_root);
	}
	
	private List<GameStateBase> _game_state_stack;

	private GameEngineScene i_cons() {
		_root = SPNode.cons_node();
		_root.set_name("GameEngineScene");
	
		__cached_viewbox_dirty = true;
		
		_delayed_actions = DelayActionQueue.cons();
		_game_state_stack = new List<GameStateBase>(){ IdleGameState.cons() };

		_bg_village = BGVillage.cons(this);
		_bg_sky = BGSky.cons(this);
		_bg_water = BGWater.cons(this);
		
		_bg_village.add_to_parent(_root);
		_bg_sky.add_to_parent(_root);
		_bg_water.add_to_parent(_root);
		
		_bg_elements = new List<SPGameUpdateable>() {_bg_village,_bg_sky,_bg_water};
		
		_player = PlayerCharacter.cons(this);
		_player.set_u_pos(0,0);
		_player.add_to_parent(_root);

		_particle_root = SPNode.cons_node();
		_particle_root.set_name("_particle_root");
		_root.add_child(_particle_root);
		_particles = SPParticleSystem<SPGameEngineParticle>.cons();
		
		
		//SPTODO
		SPText.cons_text(RTex.DIALOGUE_FONT, RFnt.DIALOGUE_FONT);
		//this.push_game_state(OnGroundGameState.cons(this));
		this.push_game_state(InAirGameState.cons(this));
		
		return this;
	}
	
	public GameStateBase get_top_game_state() { 
		return _game_state_stack[_game_state_stack.Count-1]; 
	}
	public void pop_top_game_state() {
		int i = _game_state_stack.Count-1;
		_game_state_stack[i].on_state_end(this);
		_game_state_stack.RemoveAt(i);
	}
	public void push_game_state(GameStateBase state) {
		_game_state_stack.Add(state);
	}
	
	public override void i_update() {	
		__cached_viewbox_dirty = true;
		_delayed_actions.i_update(this);
		_particles.i_update(this);
		this.get_top_game_state().i_update(this);
		_game_ui.i_update(this);
		for (int i = 0; i < _bg_elements.Count; i++) {
			SPGameUpdateable itr = _bg_elements[i];
			itr.i_update(this);
		}

		if (GameMain._context._draw_hitboxes) {
			this.debug_draw_hitboxes();
		} else {
			GameMain._context._debug_render.clear_draw_queue();
		}
	}
	
	private void debug_draw_hitboxes() {
		SPDebugRender draw = GameMain._context._debug_render;
		draw.clear_draw_queue();
		draw.draw_hitpoly_owner(_player, new Color(0,1,0,0.25f), new Color(0,1,0,0.5f));
		this.get_top_game_state().debug_draw_hitboxes(draw);
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

	public Vector2 convert_u_pos_to_screen_pos(float x, float y) {
		Vector3 world_pos = GameMain._context.transform.TransformPoint(new Vector3(x,y));
		return GameMain._context._game_camera.WorldToScreenPoint(world_pos);
	}


}
