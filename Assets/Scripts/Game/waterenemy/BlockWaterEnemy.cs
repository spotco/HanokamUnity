using UnityEngine;
using System.Collections;

public class BlockWaterEnemy : IWaterObstacle, GenericPooledObject {
	
	public static BlockWaterEnemy cons(GameEngineScene g, PatternEntryLine entry) {
		return ObjectPool.inst().generic_depool<BlockWaterEnemy>().i_cons(g,entry);
	}
	
	private SPNode _root;
	private SPSprite _img;
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("BlockWaterEnemy");
	
		_img = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_BLOCK,FileCache.inst().get_texrect(RTex.ENEMY_BLOCK,"block_left.png"));
		_root.add_child(_img);
	}
	public void repool() {
		_root.repool();
	}
	public override void add_to_parent(SPNode parent) { parent.add_child(_root); }
	public override Vector2 get_u_pos() { return _root.get_u_pos(); }
	public override void apply_env_offset(float offset) { _root.set_u_pos(_pos.x, _pos.y - offset); }
	
	private Vector2 _pt1_to_pt2_delta;
	private Vector2 _pos;
	
	private BlockWaterEnemy i_cons(GameEngineScene g, PatternEntryLine entry) {
		_pos = entry._pt1;
		
		Vector2 pt_delta = SPUtil.vec_sub(entry._pt2,entry._pt1);
		_pt1_to_pt2_delta = pt_delta;
		
		if (entry._val == "blockleft") {
			_img.set_anchor_point(0,0.5f);
			_img.set_tex_rect(FileCache.inst().get_texrect(RTex.ENEMY_BLOCK,"block_left.png"));
			_img.set_rotation(SPUtil.dir_ang_deg(pt_delta.x,pt_delta.y));
			
		} else if (entry._val == "blockright") {
			_img.set_anchor_point(1,0.5f);
			_img.set_tex_rect(FileCache.inst().get_texrect(RTex.ENEMY_BLOCK,"block_right.png"));
			_img.set_rotation(SPUtil.dir_ang_deg(pt_delta.x,pt_delta.y)-180);	
		}
		return this;
	}
	public override void on_added_to_manager(GameEngineScene g, DiveGameState state) {
		state._enemy_manager.register_obstacle(this);
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {}
	
	public override SPHitRect get_hit_rect() {
		return SPHitPoly.hitpoly_to_bounding_hitrect(this.get_hit_poly(),Vector2.zero,Vector2.zero);
	}
	public override SPHitPoly get_hit_poly() {
		Vector2 root_pos = _root.get_u_pos();
		Vector2 up_dir = _pt1_to_pt2_delta.normalized;
		Vector2 right_dir = SPUtil.vec_cross(up_dir,SPUtil.vec_z).normalized;
		return new SPHitPoly() {
			_pts0 = SPUtil.vec_add(root_pos,SPUtil.vec_add(SPUtil.vec_scale(up_dir,0),SPUtil.vec_scale(right_dir,-50))),
			_pts1 = SPUtil.vec_add(root_pos,SPUtil.vec_add(SPUtil.vec_scale(up_dir,0),SPUtil.vec_scale(right_dir,50))),
			_pts3 = SPUtil.vec_add(root_pos,SPUtil.vec_add(SPUtil.vec_scale(up_dir,_pt1_to_pt2_delta.magnitude),SPUtil.vec_scale(right_dir,-50))),
			_pts2 = SPUtil.vec_add(root_pos,SPUtil.vec_add(SPUtil.vec_scale(up_dir,_pt1_to_pt2_delta.magnitude),SPUtil.vec_scale(right_dir,50)))
		};
	}
	
	public override void debug_draw_hitboxes(SPDebugRender draw) {
		draw.draw_hitpoly_owner(this,new Color(0.8f, 0.2f, 0.2f, 0.3f), new Color(0.8f, 0.2f, 0.2f, 0.65f));
	}
}
