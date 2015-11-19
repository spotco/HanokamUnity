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
	
	
}
