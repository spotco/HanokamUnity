using UnityEngine;
using System.Collections.Generic;

public class BGUnderwaterTreasure : SPGameUpdateable {

	public static BGUnderwaterTreasure cons(SPNode parent) {
		return (new BGUnderwaterTreasure()).i_cons(parent);
	}

	private SPNode _root;
	private SPSprite _img;
	private List<SPSprite> _lights = new List<SPSprite>();
	//private bool _play_anim;

	public BGUnderwaterTreasure i_cons(SPNode parent) {
		_root = SPNode.cons_node();
		_root.set_name("BGUnderwaterTreasure");
		parent.add_child(_root);

		_img = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_SPRITESHEET_1,
			FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"underwater_temple_treasure.png")
		);
		_img.set_manual_sort_z_order(GameAnchorZ.BGWater_Ground_Treasure);
		_img.set_name("_img");
		_img.set_scale(0.75f);
		_root.add_child(_img);

		_lights = new List<SPSprite>();
		for (int i = 0; i < 5; i++) {
			SPSprite itr = SPSprite.cons_sprite_texkey_texrect(
				RTex.BG_SPRITESHEET_1,
				FileCache.inst().get_texrect(RTex.BG_SPRITESHEET_1,"treasure_light.png")
			);
			itr.set_anchor_point(0.5f,0.0f);
			itr.set_rotation(i*(360.0f / 5.0f));
			itr.set_manual_sort_z_order(GameAnchorZ.BGWater_Ground_Treasure_Light);
			itr.set_name(SPUtil.sprintf("_light[%d]",i));
			itr.set_scale(2.0f);
			_root.add_child(itr);
			_lights.Add(itr);
		}
		//_play_anim = true;
		return this;
	}

	public BGUnderwaterTreasure set_u_pos(float x, float y) {
		_root.set_u_pos(x,y);
		return this;
	}

	public void i_update(GameEngineScene g) {
		for (int i = 0; i < _lights.Count; i++) {
			SPSprite itr = _lights[i];
			itr.set_rotation(itr.rotation() + 1.5f * SPUtil.dt_scale_get());
		}
	}

}
