using UnityEngine;
using System.Collections;

public class VillageObject : OnGroundStateUpdateable, SPNodeHierarchyElement {

	public static VillageObject cons(GameEngineScene g) {
		return (new VillageObject()).i_cons(g);
	}
	
	private SPNode _root;
	private SPSprite _icon;
	private float _anim_t;
	
	public VillageObject i_cons(GameEngineScene g) {
		_root = SPNode.cons_node();
		_root.set_name("VillageObject");
		
		_icon = SPSprite.cons_sprite_texkey_texrect(RTex.DIALOG_UI,FileCache.inst().get_texrect(RTex.DIALOG_UI,"enter_arrow.png"));
		_icon.set_name("icon");
		_root.add_child(_icon);
		_icon.set_u_pos(0,0);
		
		_anim_t = 0;
		
		return this;
	}
	
	public void i_update(GameEngineScene g, OnGroundGameState state) {
		_anim_t += SPUtil.dt_scale_get() * 0.05f;
		_icon.set_u_pos(Mathf.Sin(_anim_t)*10,0);
		
		if (this.is_in_interact_range_with_player(g)) {
			_icon.set_scale(SPUtil.drpt(_icon.scale_x(), 1.0f, 1/10.0f));
			_icon.set_opacity(SPUtil.drpt(_icon.get_opacity(), 0.85f, 1/10.0f));
		} else {
			_icon.set_scale(SPUtil.drpt(_icon.scale_x(), 0.65f, 1/10.0f));
			_icon.set_opacity(SPUtil.drpt(_icon.get_opacity(), 0.5f, 1/10.0f));
		}
	}
	
	public bool is_in_interact_range_with_player(GameEngineScene g) {
		return Mathf.Abs(g._player._u_x - _root._u_x) < 100;
	}
	
	public void set_u_pos(float x, float y) { _root.set_u_pos(x,y); }
	
	public void add_to_parent(SPNode parent) {
		parent.add_child(_root);
	}
	
	public void repool() {
		_root.repool();
		_root = null;
		_icon = null;
	}
	
}
