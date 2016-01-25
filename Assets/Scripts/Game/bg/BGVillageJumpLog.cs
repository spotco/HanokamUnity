using UnityEngine;
using System.Collections.Generic;

public class BGVillageJumpLog : SPNodeHierarchyElement {

	public static BGVillageJumpLog cons() {
		return (new BGVillageJumpLog()).i_cons();
	}
	
	private SPNode _root;
	
	private SPSprite _log;
	
	private SPSprite _ripple;
	private SPSpriteAnimator _ripple_anim;
	
	private Vector3 _position;
	private Vector3 _offset;
	
	static int _test = 0;
	
	private BGVillageJumpLog i_cons() {
		_root = SPNode.cons_node();
	
		_log = SPSprite.cons_sprite_texkey_texrect(
			RTex.BG_NVILLAGE_SPRITESHEET,
			FileCache.inst().get_texrect(RTex.BG_NVILLAGE_SPRITESHEET,"step_log.png")
		);
		_log.set_manual_sort_z_order(GameAnchorZ.BGVillage_Docks_Front);
		_log.set_anchor_point(0.5f,0);
		_root.add_child(_log);
		
		_ripple = SPSprite.cons_sprite_texkey_texrect(
			RTex.GROUND_EFFECTS,
			FileCache.inst().get_texrect(RTex.GROUND_EFFECTS,"water_log_ripple_1.png")
		);
		_ripple.set_name("_ripple");
		_ripple.set_opacity(0.75f);
		_ripple.set_manual_sort_z_order(GameAnchorZ.BGVillage_Docks_Front-1);
		_root.add_child(_ripple);
		
		
		_ripple_anim = SPSpriteAnimator.cons(_ripple);
		_ripple_anim.add_anim("play",
			FileCache.inst().get_rects_list(RTex.GROUND_EFFECTS,"water_log_ripple_%d.png",2,6),10,false);
		_ripple_anim.add_anim("idle",
			FileCache.inst().get_rects_list(RTex.GROUND_EFFECTS,"water_log_ripple_%d.png",1,2),100);
		_ripple_anim.play_anim("idle");
		
		return this;
	}
	
	public void set_position(Vector3 pos) {
		_position = pos;
		this.apply_position(_position);
	}
	private void apply_position(Vector3 pos) {
		_root.set_u_pos(pos.x,pos.y);
		_root.set_u_z(pos.z);
	}
	public void set_scale(float scale) {
		_log.set_scale(scale);
		_ripple.set_scale(scale*0.9f);
		_ripple.set_u_pos(0,SPUtil.y_for_point_of_2pt_line(new Vector2(1.5f,17.5f),new Vector2(1,10.0f),scale));
	}
	public void set_name(string name) { _log.set_name(name); }
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	
	public void trigger_particle(GameEngineScene g) {
		SPConfigAnimParticle neu_particle = SPConfigAnimParticle.cons();
		neu_particle.set_ctmax(17);
		neu_particle.set_manual_sort_z_order(GameAnchorZ.BGVillage_Docks_Front+1);
		neu_particle.set_pos(SPUtil.vec_add(
			_root.get_u_pos(),
			new Vector2(0,
				SPUtil.y_for_point_of_2pt_line(new Vector2(1,45.0f),new Vector2(1.5f,60f),_log.scale_x())
		)));
		neu_particle.set_u_z(_root._u_z);
		neu_particle.set_texture(TextureResource.inst().get_tex(RTex.GROUND_EFFECTS));
		neu_particle.set_texrect(new Rect());
		neu_particle.set_normalized_timed_sprite_animator(SPTimedSpriteAnimator.cons(null)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.GROUND_EFFECTS,"log_dust0000.png"),0.0f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.GROUND_EFFECTS,"log_dust0001.png"),0.2f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.GROUND_EFFECTS,"log_dust0002.png"),0.4f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.GROUND_EFFECTS,"log_dust0003.png"),0.6f)
			.add_frame_at_time(FileCache.inst().get_texrect(RTex.GROUND_EFFECTS,"log_dust0004.png"),0.8f)
		);
		g.add_particle(neu_particle);
	}
	
	public void trigger_ripple() {
		_ripple_anim.play_anim("play",true);
	}
	
	public void i_update(GameEngineScene g) {
		_root.set_enabled(!g._bg_water.is_underwater(g));
		if (_root.is_enabled()) {
			_ripple_anim.i_update();
			if (!_ripple_anim.is_current_anim_repeating() && _ripple_anim.is_finished()) {
				_ripple_anim.play_anim("idle");
			}
			if (SPUtil.float_random(0,200) < 1) {
				this.trigger_ripple();
			}
		}
	}

}
