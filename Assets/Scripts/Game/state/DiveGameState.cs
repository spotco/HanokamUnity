using UnityEngine;
using System.Collections;

public class DiveGameState : GameStateBase {

	public struct DiveGameStateParams {
		public Vector2 _vel;
	}

	public enum State {
		TransitionIn,
		Gameplay,
		SwimToUnderwaterTreasure,
		PickupUnderwaterTreasure
	}

	public static DiveGameState cons(GameEngineScene g, Vector2 initial_velocity) {
		return (new DiveGameState()).i_cons(g, initial_velocity);
	}

	public DiveGameStateParams _params;

	public DiveGameState i_cons(GameEngineScene g, Vector2 initial_velocity) {
		_params._vel = initial_velocity;

		g.add_particle(SPConfigAnimParticle.cons()
				.add_to_parent(g.get_particle_root())
				.set_name("uw_splash")
				.set_texture(TextureResource.inst().get_tex(RTex.FX_SPLASH))
				.set_texrect(FileCache.inst().get_texrect(RTex.FX_SPLASH,"uw_splash_0.png"))
				.set_ctmax(65)
				.set_pos(g._player._u_x,-100)
				.set_anim_lambda((SPSprite _img, float anim_t) => {
					_img.set_opacity(SPUtil.bezier_val_for_t(new Vector2(0,0),new Vector2(0,1),new Vector2(0.3f,1.25f),new Vector2(1,0),anim_t).y);
					_img.set_scale_x(SPUtil.bezier_val_for_t(new Vector2(0,0.75f),new Vector2(0,1.5f),new Vector2(0.6f,2.5f),new Vector2(1,0.65f),anim_t).y * 2.0f);
					_img.set_scale_y(SPUtil.bezier_val_for_t(new Vector2(0,0.75f),new Vector2(0,1.2f),new Vector2(0.75f,1.25f),new Vector2(1,0.75f),anim_t).y * 2.0f);
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

		return this;
	}

	public override void i_update(GameEngineScene g) {
		g._player.set_manual_sort_z_order(GameAnchorZ.Player_UnderWater);
		g._camerac.set_target_camera_focus_on_character(g,0,120);
		g._camerac.set_target_zoom(1000);

		/*
		g._player.set_u_pos(
			g._player._u_x + _params._vel.x * SPUtil.dt_scale_get(),
			g._player._u_y + _params._vel.y * SPUtil.dt_scale_get()
		);
		*/
	}

	public override GameStateIdentifier get_state() { 
		return GameStateIdentifier.Dive; 
	}

}
