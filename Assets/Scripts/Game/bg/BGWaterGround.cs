using UnityEngine;
using System.Collections;

public class BGWaterGround : SPGameUpdateable {

	public static BGWaterGround cons(GameEngineScene g, SPNode parent) {
		return (new BGWaterGround()).i_cons(g,parent);
	}

	private SPSprite _ground, _ground_fill;
	private SPSprite _bottom_fade;
	private BGUnderwaterTreasure _underwater_temple_treasure;

	private SPNode _root;

	public BGWaterGround i_cons(GameEngineScene g, SPNode parent) {
		_root = SPNode.cons_node();
		_root.set_name("BGWaterGround");
		parent.add_child(_root);

		_ground = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_SPRITESHEET_1,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"underwater_temple.png")
		);
		_ground.set_manual_sort_z_order(GameAnchorZ.BGWater_Ground);
		_ground.set_anchor_point(0.5f,0.0f);
		_ground.set_u_pos(0,-130);
		_ground.set_name("_ground");
		_ground.set_scale(1.65f);
		_root.add_child(_ground);

		_ground_fill = SPSprite.cons_sprite_texkey_texrect(
			RTex.BLANK,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"underwater_temple.png")
		);
		_ground_fill.set_manual_sort_z_order(GameAnchorZ.BGWater_Ground_Fill);
		_ground_fill.set_color(new Vector4(8.0f/255.0f,25.0f/255.0f,55.0f/255.0f,1.0f));
		_ground_fill.set_anchor_point(0.5f,1.0f);
		_ground_fill.set_u_pos(0,-130);
		_ground_fill.set_name("_ground_fill");
		_ground_fill.set_scale_x(1.65f);
		_ground_fill.set_scale_y(3.0f);
		_root.add_child(_ground_fill);

		_bottom_fade = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_WATER_ELEMENT_FADE,
			new Rect(0,0,SPUtil.get_horiz_world_bounds()._max-SPUtil.get_horiz_world_bounds()._min,512));
		_bottom_fade.set_name("_bottom_fade");
		_bottom_fade.set_manual_sort_z_order(GameAnchorZ.BGWater_Ground_BottomFade);
		_bottom_fade.set_u_z(25);
		_bottom_fade.set_scale_x(1);
		_bottom_fade.set_scale_y(3.0f);
		_bottom_fade.set_u_pos(0,0);
		_bottom_fade.set_anchor_point(0.5f,0);
		_root.add_child(_bottom_fade);

		_underwater_temple_treasure = BGUnderwaterTreasure.cons(_root);
		_underwater_temple_treasure.set_u_pos(0,256);

		return this;
	}

	public BGWaterGround set_u_pos(float x, float y) {
		_root.set_u_pos(x,y);
		return this;
	}

	public void i_update(GameEngineScene g) {
		_underwater_temple_treasure.i_update(g);
	}

	public Vector2 get_underwater_treasure_position() {
		return SPUtil.vec_add(new Vector2(_root._u_x, _root._u_y), 
			new Vector2(_underwater_temple_treasure._u_x,_underwater_temple_treasure._u_y));
	}
}
