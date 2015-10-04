using UnityEngine;
using System.Collections;

public class DiveGameState : GameStateBase {

	public struct DiveGameStateParams {
		public Vector2 _vel;
		public DiveGameState.State _state;
	}

	public enum State {
		TransitionIn,
		Gameplay
	}

	public static DiveGameState cons(GameEngineScene g) {
		return (new DiveGameState()).i_cons(g);
	}

	public DiveGameStateParams _params;
	private FlashEvery _bubble_every;

	public DiveGameState i_cons(GameEngineScene g) {
		_params._vel = new Vector2(0,-22);
		_params._state = State.TransitionIn;

		g._player.play_anim(PlayerCharacterAnims.SWIM);

		g.add_particle(SPConfigAnimParticle.cons()
			.set_name("uw_splash")
			.set_texture(TextureResource.inst().get_tex(RTex.FX_SPLASH))
			.set_texrect(FileCache.inst().get_texrect(RTex.FX_SPLASH,"uw_splash_0.png"))
			.set_ctmax(65)
			.set_pos(g._player._u_x,-100)
			.set_anim_lambda((SPSprite _img, float anim_t) => {
				_img.set_opacity(SPUtil.bezier_val_for_t(new Vector2(0,0),new Vector2(0,1),new Vector2(0.3f,1.25f),new Vector2(1,0),anim_t).y);
				_img.set_scale_x(SPUtil.bezier_val_for_t(new Vector2(0,0.75f),new Vector2(0,1.5f),new Vector2(0.6f,2.5f),new Vector2(1,0.65f),anim_t).y * 3.0f);
				_img.set_scale_y(SPUtil.bezier_val_for_t(new Vector2(0,0.75f),new Vector2(0,1.2f),new Vector2(0.75f,1.25f),new Vector2(1,0.75f),anim_t).y * 3.0f);
			})
			.set_anchor_point(0.5f,0.85f)
			.set_manual_sort_z_order(GameAnchorZ.BGWater_FX)
			.set_normalized_timed_sprite_animator(SPTimedSpriteAnimator.cons(null)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"uw_splash_0.png"),0.0f)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"uw_splash_1.png"),0.15f)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"uw_splash_2.png"),0.24f)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"uw_splash_3.png"),0.36f)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"uw_splash_4.png"),0.48f)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"uw_splash_5.png"),0.60f)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"uw_splash_6.png"),0.72f)
				.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"uw_splash_7.png"),0.84f)
			));
		
		g._player.set_manual_sort_z_order(GameAnchorZ.Player_UnderWater);
		
		_bubble_every = FlashEvery.cons(30);
		
		return this;
	}

	float MAX_MOVE_SPEED = 10;
	float TURN_SPEED = 3;
	public override void i_update(GameEngineScene g) {
		switch (_params._state) {
		case (State.TransitionIn): {
			g._camerac.set_target_camera_focus_on_character(g,0,0);
			g._camerac.set_target_zoom(1000);
			if (g._controls.is_move_x()) {
				Vector2 move = g._controls.get_move();
				_params._vel.x = Mathf.Clamp(_params._vel.x + move.x * TURN_SPEED, -MAX_MOVE_SPEED, MAX_MOVE_SPEED);
			} else {
				_params._vel.x = SPUtil.drpt(_params._vel.x,0,1/60.0f);
			}
			PlayerCharacterUtil.rotate_to_rotation_for_vel(g._player,_params._vel.x,_params._vel.y,1/10.0f);
			g._player.set_u_pos(
				g._player._u_x + _params._vel.x * SPUtil.dt_scale_get(),
				g._player._u_y + _params._vel.y * SPUtil.dt_scale_get()
			);
			if (g._player._u_y < -1000) {
				_params._state = State.Gameplay;
				g._camerac.set_zoom_speed(1/100.0f);
				g._camerac.set_target_zoom(1500);
				g._game_ui._cursor.set_enabled(true);
			}

		} break;
		case (State.Gameplay): {
			g._camerac.set_target_camera_focus_on_character(g,0,150);
			
			Vector2 move = g._controls.get_move();
			if (g._controls.is_move_x()) {
				_params._vel.x = Mathf.Clamp(_params._vel.x + move.x * TURN_SPEED, -MAX_MOVE_SPEED, MAX_MOVE_SPEED);
			} else {
				_params._vel.x = SPUtil.drpt(_params._vel.x,0,1/60.0f);
			}
			if (g._controls.is_move_y()) {
				_params._vel.y = Mathf.Clamp(_params._vel.y + move.y * TURN_SPEED, -MAX_MOVE_SPEED, MAX_MOVE_SPEED);
			} else {
				_params._vel.y = SPUtil.drpt(_params._vel.y,0,1/60.0f);
			}
			PlayerCharacterUtil.move_in_bounds(
				g._player,
				g._player._u_x + _params._vel.x * SPUtil.dt_scale_get(),
				g._player._u_y + _params._vel.y * SPUtil.dt_scale_get()
			);
			if (_params._vel.magnitude > 2) {
				g._player.play_anim(PlayerCharacterAnims.SWIM);
			} else {
				g._player.play_anim(PlayerCharacterAnims.SWIM_SLOW);
			}
			PlayerCharacterUtil.rotate_to_rotation_for_vel(g._player,_params._vel.x,_params._vel.y,1/10.0f);
			
			_bubble_every.i_update(g);
			if (_bubble_every.do_flash()) {
				_bubble_every._max_time = SPUtil.int_random(0,4) == 0 ? SPUtil.float_random(1,3) : SPUtil.float_random(20,40);
				UnderwaterBubbleParticle.proc_bubble(g);
			}
			
		} break;
		}
	}

	public override GameStateIdentifier get_state() { 
		return GameStateIdentifier.Dive; 
	}
}
