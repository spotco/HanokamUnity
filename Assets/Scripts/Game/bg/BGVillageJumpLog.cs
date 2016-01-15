using UnityEngine;
using System.Collections;

public class BGVillageJumpLog : SPNodeHierarchyElement {

	public static BGVillageJumpLog cons() {
		return (new BGVillageJumpLog()).i_cons();
	}
	
	private SPSprite _log;
	private Vector3 _position;
	private Vector3 _offset;
	private float _bob_vertical_theta;
	private float _bob_leftright_theta;
	
	private BGVillageJumpLog i_cons() {
		_log = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_NVILLAGE_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"step_log.png")
		);
		_log.set_manual_sort_z_order(GameAnchorZ.BGVillage_Docks_Front);
		_log.set_anchor_point(0.5f,0);
		
		_bob_vertical_theta = SPUtil.float_random(-3.14f,3.14f);
		_bob_leftright_theta = SPUtil.float_random(-3.14f,3.14f);
		
		return this;
	}
	
	public void set_position(Vector3 pos) {
		_position = pos;
		this.apply_position(_position);
	}
	private void apply_position(Vector3 pos) {
		_log.set_u_pos(pos.x,pos.y);
		_log.set_u_z(pos.z);
	}
	public void set_scale(float scale) {
		_log.set_scale(scale);
	}
	public void set_name(string name) { _log.set_name(name); }
	
	public void add_to_parent(SPNode parent) {
		parent.add_child(_log);
	}
	
	public void apply_offset() {
		_offset = new Vector3(0,-30,0);
	}
	
	public void i_update() {
		_bob_vertical_theta += SPUtil.dt_scale_get() * 0.05f;
		_bob_leftright_theta += SPUtil.dt_scale_get() * 0.04f;
		
		_offset = SPUtil.drpt_vec(_offset,Vector3.zero,1/20.0f);
		
		this.apply_position(
			SPUtil.vec_add(_offset,
			SPUtil.vec_add(_position,new Vector3(0,Mathf.Sin(_bob_vertical_theta)*3,0)))
		);
		_log.set_rotation(Mathf.Sin(_bob_leftright_theta)*1.5f);
		
	}

}
