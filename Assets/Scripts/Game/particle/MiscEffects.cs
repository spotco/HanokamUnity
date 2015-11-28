using UnityEngine;
using System.Collections;

public class MiscEffects : MonoBehaviour {

	public static void do_player_bubble(GameEngineScene g) {
		g.add_particle(SPConfigAnimParticle.cons()
		.set_name("player_bubble")
		.set_texture(TextureResource.inst().get_tex(RTex.FX_SPLASH))
		.set_texrect(FileCache.inst().get_texrect(RTex.FX_SPLASH,"bubble_anim_v3_0.png"))
		.set_ctmax(50)
		.set_pos(g._player._u_x,g._player._u_y + 60)
		.set_anim_lambda((SPSprite img, float pct) => {
			img.set_u_pos(g._player._u_x,g._player._u_y + 60);
		})
		.set_anchor_point(0.5f,0.15f)
		.set_manual_sort_z_order(GameAnchorZ.BGWater_FX)
		.set_alpha(0.9f,0.2f)
		.set_scale(1.5f,1.9f)
		.set_normalized_timed_sprite_animator(SPTimedSpriteAnimator.cons(null)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"bubble_anim_v3_0.png"),0.0f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"bubble_anim_v3_1.png"),0.083f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"bubble_anim_v3_2.png"),0.166f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"bubble_anim_v3_3.png"),0.249f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"bubble_anim_v3_4.png"),0.332f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"bubble_anim_v3_5.png"),0.415f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"bubble_anim_v3_6.png"),0.498f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"bubble_anim_v3_7.png"),0.581f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"bubble_anim_v3_8.png"),0.664f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"bubble_anim_v3_9.png"),0.747f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"bubble_anim_v3_10.png"),0.83f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"bubble_anim_v3_11.png"),0.91f)
		));
	}
	
	public static void do_abovewater_splash(GameEngineScene g) {
		float last_bgsky_y_offset = g._bg_sky.get_y_offset();
		g.add_particle(SPConfigAnimParticle.cons()
		.set_name("aw_splash")
		.set_texture(TextureResource.inst().get_tex(RTex.FX_SPLASH))
		.set_texrect(FileCache.inst().get_texrect(RTex.FX_SPLASH,"aw_splash_0.png"))
		.set_ctmax(65)
		.set_pos(g._player._u_x,20)
		.set_anim_lambda((SPSprite _img, float anim_t) => {
			_img.set_opacity(SPUtil.bezier_val_for_t(new Vector2(0,0),new Vector2(0,1),new Vector2(0.3f,1.25f),new Vector2(1,0),anim_t).y);
			_img.set_scale_x(SPUtil.bezier_val_for_t(new Vector2(0,0.75f),new Vector2(0,1.5f),new Vector2(0.6f,2.5f),new Vector2(1,0.65f),anim_t).y * 3.0f);
			_img.set_scale_y(SPUtil.bezier_val_for_t(new Vector2(0,0.75f),new Vector2(0,1.2f),new Vector2(0.75f,1.25f),new Vector2(1,0.75f),anim_t).y * 3.0f);
			_img._u_y = _img._u_y - (g._bg_sky.get_y_offset()-last_bgsky_y_offset);
			last_bgsky_y_offset = g._bg_sky.get_y_offset();
		})
		.set_anchor_point(0.5f,0.85f)
		.set_manual_sort_z_order(GameAnchorZ.BGWater_FX)
		.set_normalized_timed_sprite_animator(SPTimedSpriteAnimator.cons(null)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"aw_splash_0.png"),0.0f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"aw_splash_1.png"),0.083f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"aw_splash_2.png"),0.166f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"aw_splash_3.png"),0.249f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"aw_splash_4.png"),0.332f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"aw_splash_5.png"),0.415f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"aw_splash_6.png"),0.498f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"aw_splash_7.png"),0.581f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"aw_splash_8.png"),0.664f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"aw_splash_9.png"),0.747f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"aw_splash_10.png"),0.83f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.FX_SPLASH,"aw_splash_11.png"),0.91f)
		));
	}
	
	public static void do_underwater_splash(GameEngineScene g) {
		float last_bgwater_y_offset = g._bg_sky.get_y_offset();
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
				_img._u_y = _img._u_y - (g._bg_water.get_y_offset()-last_bgwater_y_offset);
				last_bgwater_y_offset = g._bg_water.get_y_offset();
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
	}
	
	
}
