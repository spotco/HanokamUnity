using UnityEngine;
using System.Collections;

public class PlayerAimReticule : SPNodeHierarchyElement, SPGameUpdateable {

	public static PlayerAimReticule cons() {
		return (new PlayerAimReticule()).i_cons();
	}
	
	private SPNode _root;
	private SPSprite _left_line, _right_line, _spark, _mega_arrow_line;
	private SPSpriteAnimator _spark_anim;
	
	private PlayerAimReticule i_cons() {
		_root = SPNode.cons_node();
		_root.set_name("PlayerAimReticule");
		
		_left_line = SPAlphaGradientSprite.cons_alphagradient_sprite(
			RTex.BLANK,
			new Rect(0,0,5,2000), 
			new SPRange() { _min = 0.8f, _max = 0.8f }, 
			new SPRange() { _min = 0.15f, _max = 0.8f }
		);
		_left_line.set_anchor_point(0.5f,0);
		_left_line.set_manual_sort_z_order(GameAnchorZ.Player_FX);
		_root.add_child(_left_line);
		
		_right_line = SPAlphaGradientSprite.cons_alphagradient_sprite(
			RTex.BLANK,
			new Rect(0,0,5,2000), 
			new SPRange() { _min = 0.8f, _max = 0.8f }, 
		new SPRange() { _min = 0.15f, _max = 0.8f }
		);
		_right_line.set_anchor_point(0.5f,0);
		_right_line.set_manual_sort_z_order(GameAnchorZ.Player_FX);
		_root.add_child(_right_line);
		
		_spark = SPSprite.cons_sprite_texkey_texrect(RTex.HANOKA_EFFECTS, FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"arrow_charge_start_0.png"));
		_spark.set_manual_sort_z_order(GameAnchorZ.Player_FX);
		_spark.set_opacity(0.75f);
		_root.add_child(_spark);
		_spark_anim = SPSpriteAnimator.cons(_spark).add_anim(
			"play", FileCache.inst().get_rects_list(RTex.HANOKA_EFFECTS,"arrow_charge_start_%d.png",0,6),3).play_anim("play");
			
		_mega_arrow_line = SPAlphaGradientSprite.cons_alphagradient_sprite(
			RTex.PARTICLES_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.PARTICLES_SPRITESHEET,"mega_arrow.png"), 
			new SPRange() { _min = 1.0f, _max = 1.0f }, 
			new SPRange() { _min = 0.1f, _max = 1.0f }
		);
		_mega_arrow_line.set_anchor_point(0.0f,0.5f);
		_mega_arrow_line.set_manual_sort_z_order(GameAnchorZ.Player_FX);
		_mega_arrow_line.set_scale(1.5f);
		_root.add_child(_mega_arrow_line);
		
		return this;
	}
	
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	public void set_u_pos(Vector2 pos) { _root.set_u_pos(pos); }
	public void set_enabled(bool val) { _root.set_enabled(val); }
	
	public void set_aim_variance_and_charge_pct(GameEngineScene g, float aim_variance, float charge_pct) {
		float inv_charge_pct = 1-charge_pct;
		float aim_variance_d2 = aim_variance/2.0f;
		
		float line_target_rotation = g._player.get_arrow_target_rotation();
		_left_line.set_rotation(line_target_rotation + aim_variance_d2 * inv_charge_pct);
		_right_line.set_rotation(line_target_rotation - aim_variance_d2 * inv_charge_pct);
		
		_left_line.set_opacity(SPUtil.bezier_val_for_t(
			new Vector2(0,0), new Vector2(0,1), new Vector2(0.5f,1.25f), new Vector2(1,0), charge_pct
		).y);
		_right_line.set_opacity(_left_line.get_opacity());
		
		_mega_arrow_line.set_rotation(line_target_rotation + 90);
		_mega_arrow_line.set_scale_x(Mathf.Lerp(1.5f, 1.25f, charge_pct));
		_mega_arrow_line.set_scale_y(Mathf.Lerp(1.35f, 1.25f, charge_pct));
		_mega_arrow_line.set_opacity(SPUtil.bezier_val_for_t(
			new Vector2(0,0), new Vector2(1,0), new Vector2(1.0f,0.2f), new Vector2(1,1), charge_pct
		).y);
		
		_spark.set_enabled(charge_pct >= 1);
	}
	
	public void i_update(GameEngineScene g) {
		float line_target_rotation = g._player.get_arrow_target_rotation();
		_spark.set_u_pos(SPUtil.vec_scale(SPUtil.ang_deg_dir(line_target_rotation+90),65));
		_spark_anim.i_update();
	}

}
