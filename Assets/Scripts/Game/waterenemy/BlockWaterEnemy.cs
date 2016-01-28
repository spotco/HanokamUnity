using UnityEngine;
using System.Collections;

public class BlockWaterEnemy : IWaterObstacle, GenericPooledObject {
	
	public static BlockWaterEnemy cons(GameEngineScene g, PatternEntryPolygon entry) {
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
	
	private Vector2 _pos;
	private Vector2 _pt0, _pt1, _pt2, _pt3;
	
	private BlockWaterEnemy i_cons(GameEngineScene g, PatternEntryPolygon entry) {
		_pos = entry._pt0;
		
		_pt0 = entry._pt0;
		_pt1 = entry._pt1;
		_pt2 = entry._pt2;
		_pt3 = entry._pt3;
		
		_img.set_anchor_point(0,0.5f);
		_img.set_tex_rect(FileCache.inst().get_texrect(RTex.ENEMY_BLOCK,"block_left.png"));
		
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
		return new SPHitPoly() {
			_pts0 = root_pos,
			_pts1 = SPUtil.vec_add(root_pos,SPUtil.vec_sub(_pt1,_pt0)),
			_pts2 = SPUtil.vec_add(root_pos,SPUtil.vec_sub(_pt2,_pt0)),
			_pts3 = SPUtil.vec_add(root_pos,SPUtil.vec_sub(_pt3,_pt0))
		};
	}
	
	public override void debug_draw_hitboxes(SPDebugRender draw) {
		draw.draw_hitpoly_owner(this,new Color(0.8f, 0.2f, 0.2f, 0.3f), new Color(0.8f, 0.2f, 0.2f, 0.65f));
	}
}
