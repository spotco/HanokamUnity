using UnityEngine;
using System.Collections;

public class BGWaterLineBelow : SPGameUpdateable {

	public static BGWaterLineBelow cons(SPNode parent) {
		return (new BGWaterLineBelow()).i_cons(parent);
	}

	private SPSprite _waterline, _watergrad, _belowline_lightrays;
	private float _waterline_x, _belowline_lightrays_anim_theta;
	private SPNode _root;

	public BGWaterLineBelow i_cons(SPNode parent) {
		_root = SPNode.cons_node();
		_root.set_name("BGWaterLineBelow");
		parent.add_child(_root);

		_waterline = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_WATER_TOP_WATERLINE,
			SPUtil.texture_default_rect(RTex.BG_WATER_TOP_WATERLINE)
		);
		_waterline.set_anchor_point(0.5f,0.0f);
		_waterline.set_u_pos(0,13);
		_waterline.set_scale(2.5f);
		_waterline.set_name("_waterline");
		_waterline.set_opacity(1.0f);
		_waterline.set_manual_sort_z_order(GameAnchorZ.BGWater_WaterLineBelow_Line);
		_root.add_child(_waterline);

		_watergrad = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_WATER_BOTTOM_SURFACEGRAD,
			SPUtil.texture_default_rect(RTex.BG_WATER_BOTTOM_SURFACEGRAD)
		);
		_watergrad.set_anchor_point(0.5f,1.0f);
		_watergrad.set_u_pos(0,55);
		_watergrad.set_scale_x(83.3f);
		_watergrad.set_scale_y(3.3f);
		_watergrad.set_name("_watergrad");
		_watergrad.set_manual_sort_z_order(GameAnchorZ.BGWater_WaterLineBelow_Grad);
		_root.add_child(_watergrad);

		_belowline_lightrays = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_SPRITESHEET_1,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"bg_water_bottom_beams.png")
		);
		_belowline_lightrays.set_name("_belowline_lightrays");
		_belowline_lightrays.set_manual_sort_z_order(GameAnchorZ.BGWater_WaterLineBelow_Line_LightRays);
		_belowline_lightrays.set_opacity(0.65f);
		_belowline_lightrays.set_scale(1.5f);
		_belowline_lightrays.set_anchor_point(0.5f,1.0f);
		_belowline_lightrays.set_u_pos(0,970);
		_root.add_child(_belowline_lightrays);


		return this;
	}

	public BGWaterLineBelow set_u_pos(float x, float y) {
		_root.set_u_pos(x,y);
		return this;
	}
	
	public BGWaterLineBelow set_u_z(float z) {
		_root.set_u_z(z);
		return this;
	}
	
	public BGWaterLineBelow set_enabled(bool val) {
		_root.set_enabled(val);
		return this;
	}

	public void i_update(GameEngineScene g) {
		_waterline_x = (_waterline_x + SPUtil.dt_scale_get() * 0.4f) % (_waterline.texrect().size.x);
		_waterline.set_tex_rect(new Rect(
			_waterline_x,
			_waterline.texrect().position.y,
			_waterline.texrect().size.x,
			_waterline.texrect().size.y
		));
		
		_belowline_lightrays_anim_theta += 0.095f * SPUtil.dt_scale_get();
		_belowline_lightrays.set_opacity((Mathf.Cos(_belowline_lightrays_anim_theta)+1)/2.0f * 0.5f + 0.4f);
	}

}
