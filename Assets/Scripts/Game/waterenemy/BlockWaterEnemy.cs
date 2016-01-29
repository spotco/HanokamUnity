using UnityEngine;
using System.Collections.Generic;

public class BlockWaterEnemy : IWaterObstacle, GenericPooledObject {
	
	public static BlockWaterEnemy cons(GameEngineScene g, PatternEntryPolygon entry) {
		return ObjectPool.inst().generic_depool<BlockWaterEnemy>().i_cons(g,entry);
	}
	
	private SPNode _root;
	private SPSprite _main_fill;
	private List<SPSprite> _border_fills = new List<SPSprite>();
	private List<SPSprite> _corner_fills = new List<SPSprite>();
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("BlockWaterEnemy");
	
		_main_fill = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_BLOCK_BODY_FILL,SPUtil.texture_default_rect(RTex.ENEMY_BLOCK_BODY_FILL));
		_main_fill.set_name("_main_fill");
		_root.add_child(_main_fill);
		_border_fills.Clear();
		_corner_fills.Clear();
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
		this.apply_env_offset(0);
		
		_pt0 = entry._pt0;
		_pt1 = entry._pt1;
		_pt2 = entry._pt2;
		_pt3 = entry._pt3;
		
		this.set_main_fill();
		
		return this;
	}
	
	private class PolyIntersectionEdgeLink {
		public Vector2 _n0, _n1;
		public PolyIntersectionEdgeLink _next, _prev;
		public SPSprite _sprite;
		public SPLineSegment line_seg() {
			return new SPLineSegment() {
				_pt0 = _n0, _pt1 = _n1
			};
		}
	}
	
	public override void load_postprocess(GameEngineScene g, DiveGameState state) {
		Vector3 out_cross_dir;
		float scale_y;
		{
			int i = 0;
			Vector2 vtx_delta = SPUtil.vec_sub(_main_fill.u_pos_of_vertex((i+1)%4),_main_fill.u_pos_of_vertex(i));
			Vector2 vtx_delta_next = SPUtil.vec_sub(_main_fill.u_pos_of_vertex((i+2)%4),_main_fill.u_pos_of_vertex((i+1)%4));
			out_cross_dir = SPUtil.vec_cross(vtx_delta,vtx_delta_next).normalized;
			scale_y = -out_cross_dir.z;
		}
	
		List<Vector2> initial_nodes = new List<Vector2>();
		List<PolyIntersectionEdgeLink> links = new List<PolyIntersectionEdgeLink>();
		int VTX_CT = 4;
		for (int i = 0; i < VTX_CT; i++) {
			Vector2 itr_pos = _main_fill.u_pos_of_vertex(i);
			Vector2 next_dir = SPUtil.vec_sub(_main_fill.u_pos_of_vertex((i+1)%VTX_CT),itr_pos);
			Vector2 out_dir = SPUtil.vec_cross(next_dir,out_cross_dir).normalized;
			initial_nodes.Add(SPUtil.vec_add(itr_pos,SPUtil.vec_scale(out_dir,-50)));
		}
		
		for (int i = 0; i < initial_nodes.Count; i++) {
			links.Add(new PolyIntersectionEdgeLink() {
				_n0 = initial_nodes[i], 
				_n1 = initial_nodes[(i+1)%initial_nodes.Count]
			});
		}
		for (int i = 0; i < links.Count; i++) {
			links[i]._next = links[(i+1)%links.Count];
			links[i]._next._prev = links[i];
		}
		
		int fail_ct = 0;
		while (true) {
			fail_ct++;
			if (fail_ct > 5) {
				Debug.LogError("POSTPROCESS BlockWaterEnemy fail -- inf loop");
				break;
			}
			bool intersection_found = false;
			
			for (int i_test_obstacle = 0; i_test_obstacle < state._enemy_manager._obstacles.Count; i_test_obstacle++) {
				IWaterObstacle itr_test_obstacle = state._enemy_manager._obstacles[i_test_obstacle];
				if (itr_test_obstacle != this && SPHitPoly.polyowners_intersect(this,itr_test_obstacle)) {
					SPHitPoly test_poly = itr_test_obstacle.get_hit_poly();
					SPHitRect test_poly_hitrect = SPHitPoly.hitpoly_to_bounding_hitrect(test_poly,Vector2.zero,Vector2.zero);
					
					for (int i_link_seg = links.Count-1; i_link_seg >= 0; i_link_seg--) {
						PolyIntersectionEdgeLink itr_link = links[i_link_seg];
						SPLineSegment itr_link_seg = itr_link.line_seg();
						Vector2 itr_link_seg_dir = SPUtil.vec_sub(itr_link_seg._pt1,itr_link_seg._pt0).normalized;
						
						for (int i_test_vtx_seg = 0; i_test_vtx_seg < test_poly.length; i_test_vtx_seg++) {
							SPLineSegment itr_test_vtx_seg = new SPLineSegment() {
								_pt0 = test_poly.pts(i_test_vtx_seg), 
								_pt1 = test_poly.pts((i_test_vtx_seg+1)%test_poly.length)
							};
							
							bool itr_line_pt0_contained = false;
							bool itr_line_pt1_contained = false;
							if (SPHitRect.hitrect_contains_pt(test_poly_hitrect,itr_link_seg._pt0) || SPHitRect.hitrect_contains_pt(test_poly_hitrect,itr_link_seg._pt1)) {
								itr_line_pt0_contained = test_poly.contains_point(itr_link_seg._pt0);
								itr_line_pt1_contained = test_poly.contains_point(itr_link_seg._pt1);
							}

							if (itr_line_pt0_contained && itr_line_pt1_contained) {
								if (itr_link._next != null) itr_link._next._prev = null; 
								itr_link._next = null;
								if (itr_link._prev != null) itr_link._prev._next = null;
								itr_link._prev = null;
								
								links.RemoveAt(i_link_seg);
								intersection_found = true;
								break;
								
							} else {
								Vector2 intersection = SPLineSegment.line_seg_intersection(itr_link_seg,itr_test_vtx_seg);
								bool is_intersection = (!float.IsNaN(intersection.x) && !float.IsNaN(intersection.y));
								
								if (is_intersection) {
									if (itr_line_pt0_contained) {
										itr_link._n0 = SPUtil.vec_add(intersection,SPUtil.vec_scale(itr_link_seg_dir,1));
										
										if (itr_link._prev != null) itr_link._prev._next = null;
										itr_link._prev = null;
										
									} else if (itr_line_pt1_contained) {
										itr_link._n1 = SPUtil.vec_add(intersection,SPUtil.vec_scale(itr_link_seg_dir,-1));
										
										if (itr_link._next != null) itr_link._next._prev = null; 
										itr_link._next = null;
										
									} else {
										PolyIntersectionEdgeLink neu_link = new PolyIntersectionEdgeLink() {
											_n0 = SPUtil.vec_add(intersection,SPUtil.vec_scale(itr_link_seg_dir,1)),
											_n1 = itr_link._n1
										};
										if (itr_link._next != null) itr_link._next._prev = neu_link;
										neu_link._next = itr_link._next;            
										itr_link._next = null;
										links.Add(neu_link);
										
										itr_link._n1 = SPUtil.vec_add(intersection,SPUtil.vec_scale(itr_link_seg_dir,-1));
									}
									itr_link_seg = itr_link.line_seg();
									intersection_found = true;
								}
							}
						}
					}
				}
			}
			if (!intersection_found) break;
		}
		
		for (int i = 0; i < links.Count; i++) {
			PolyIntersectionEdgeLink itr_link = links[i];
			if (!SPUtil.get_horiz_world_bounds().contains(itr_link._n0.x) && !SPUtil.get_horiz_world_bounds().contains(itr_link._n1.x)) continue;
		
			SPSprite itr_fill = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_BLOCK_TOP_SECTION,SPUtil.texture_default_rect(RTex.ENEMY_BLOCK_TOP_SECTION));
			itr_fill.set_name(string.Format("_border_fills[{0}]",i));
			itr_fill.set_manual_sort_z_order(GameAnchorZ.BGWater_Underwater_Obstacle_TOP);
			_root.add_child(itr_fill);
			
			Vector2 vtx_delta = SPUtil.vec_sub(itr_link._n1,itr_link._n0);
			Rect itr_texrect = itr_fill.texrect();
			itr_texrect.position = new Vector2(SPUtil.float_random(-1,1),itr_texrect.position.y);
			itr_texrect.size = new Vector2(itr_texrect.size.x * vtx_delta.magnitude / TextureResource.inst().get_tex(itr_fill.texkey()).width, itr_texrect.size.y);
			itr_fill.set_tex_rect(itr_texrect);
			itr_fill.manual_set_mesh_size(vtx_delta.magnitude,itr_fill.texrect().height);
			itr_fill.set_anchor_point(0,0.5f);
			itr_fill.set_rotation(SPUtil.dir_ang_deg(vtx_delta.x,vtx_delta.y));
			itr_fill.set_scale_y(scale_y);
			itr_fill.set_u_pos(SPUtil.vec_sub(itr_link._n0,_pos));
			
			itr_link._sprite = itr_fill;
			_border_fills.Add(itr_fill);		
		}
		
		
		for (int i = 0; i < links.Count; i++) {
			PolyIntersectionEdgeLink itr_link = links[i];
			if (itr_link._next != null) {
				PolyIntersectionEdgeLink itr_next_link = itr_link._next;
				SPSprite itr_sprite = itr_link._sprite;
				SPSprite itr_next_sprite = itr_next_link._sprite;
				if (itr_sprite == null || itr_next_sprite == null) continue;
				
				SPLineSegment itr_seg = new SPLineSegment() {
					_pt0 = itr_sprite.u_pos_of_vertex(SPSprite.VTX_1_1),
					_pt1 = itr_sprite.u_pos_of_vertex(SPSprite.VTX_1_0)
				};
				SPLineSegment itr_next_seg = new SPLineSegment() {
					_pt0 = itr_next_sprite.u_pos_of_vertex(SPSprite.VTX_0_1),
					_pt1 = itr_next_sprite.u_pos_of_vertex(SPSprite.VTX_0_0)
				};
				Vector2 tri_pt_intersection = SPLineSegment.line_seg_intersection(itr_seg,itr_next_seg);
				if (float.IsNaN(tri_pt_intersection.x) || float.IsNaN(tri_pt_intersection.y)) {
					Debug.LogError("tri pt intersection corner is NaN");
					continue;
				}
				Vector2 tri_pt_itr = itr_sprite.u_pos_of_vertex(SPSprite.VTX_1_1);
				Vector2 tri_pt_itr_next = itr_next_sprite.u_pos_of_vertex(SPSprite.VTX_0_1);

				SPSprite itr_corner = SPSprite.cons_sprite_texkey_texrect(itr_sprite.texkey(),itr_sprite.texrect());
				itr_corner.set_name(string.Format("corner({0})",itr_sprite.gameObject.name));
				itr_corner.set_manual_sort_z_order(GameAnchorZ.BGWater_Underwater_Obstacle_TOP);
				itr_corner.set_u_pos(SPUtil.vec_sub(tri_pt_intersection,_pos));
				itr_corner.manual_set_vertex(SPSprite.VTX_0_0,Vector2.zero);
				itr_corner.manual_set_vertex(SPSprite.VTX_1_0,SPUtil.vec_sub(tri_pt_itr,tri_pt_intersection));
				itr_corner.manual_set_vertex(SPSprite.VTX_1_1,SPUtil.vec_sub(tri_pt_itr_next,tri_pt_intersection));
				itr_corner.manual_set_vertex(SPSprite.VTX_0_1,Vector2.zero);
				
				float uv_t = SPUtil.vec_dist(tri_pt_intersection,itr_seg._pt0) / SPUtil.vec_dist(itr_seg._pt1,itr_seg._pt0);
				Vector2 uv0 = Vector2.Lerp(itr_sprite.uv_at_vertex(SPSprite.VTX_1_1), itr_sprite.uv_at_vertex(SPSprite.VTX_1_0), uv_t);
												
				itr_corner.manual_set_uv(SPSprite.VTX_0_0, uv0);
				itr_corner.manual_set_uv(SPSprite.VTX_1_0, itr_sprite.uv_at_vertex(SPSprite.VTX_1_1));
				itr_corner.manual_set_uv(SPSprite.VTX_1_1, 
					SPUtil.vec_add(
						itr_sprite.uv_at_vertex(SPSprite.VTX_1_1), 
						new Vector2(
							SPUtil.vec_dist(tri_pt_itr_next,tri_pt_itr) / TextureResource.inst().get_tex(itr_corner.texkey()).width,
							0)));
				itr_corner.manual_set_uv(SPSprite.VTX_0_1, uv0);
				
				_root.add_child(itr_corner);
			}		
		}
	}
	
	private static bool __TMP = false;
	
	private void set_main_fill() {
		Vector2 recenter_offset;
		{
			int vtx = SPSprite.VTX_0_0;
			Vector2 vtx_rel_pos = SPUtil.vec_sub(_pt0,_pt0);
			_main_fill.manual_set_vertex(vtx,vtx_rel_pos);
			
			Vector2 vtx_uv_pos = this.vtx_rel_pos_to_world_uv(vtx_rel_pos,TextureResource.inst().get_tex(_main_fill.texkey()));
			recenter_offset = SPUtil.vec_scale(new Vector2(Mathf.Floor(vtx_uv_pos.x),Mathf.Floor(vtx_uv_pos.y)),-1);
			_main_fill.manual_set_uv(vtx,SPUtil.vec_add(vtx_uv_pos,recenter_offset));
		}
		{
			int vtx = SPSprite.VTX_0_1;
			Vector2 vtx_rel_pos = SPUtil.vec_sub(_pt1,_pt0);
			_main_fill.manual_set_vertex(vtx,vtx_rel_pos);
			
			Vector2 vtx_uv_pos = this.vtx_rel_pos_to_world_uv(vtx_rel_pos,TextureResource.inst().get_tex(_main_fill.texkey()));
			_main_fill.manual_set_uv(vtx,SPUtil.vec_add(vtx_uv_pos,recenter_offset));
		}
		{
			int vtx = SPSprite.VTX_1_1;
			Vector2 vtx_rel_pos = SPUtil.vec_sub(_pt2,_pt0);
			_main_fill.manual_set_vertex(vtx,vtx_rel_pos);
			
			Vector2 vtx_uv_pos = this.vtx_rel_pos_to_world_uv(vtx_rel_pos,TextureResource.inst().get_tex(_main_fill.texkey()));
			_main_fill.manual_set_uv(vtx,SPUtil.vec_add(vtx_uv_pos,recenter_offset));
		}
		{
			int vtx = SPSprite.VTX_1_0;
			Vector2 vtx_rel_pos = SPUtil.vec_sub(_pt3,_pt0);
			_main_fill.manual_set_vertex(vtx,vtx_rel_pos);
			
			Vector2 vtx_uv_pos = this.vtx_rel_pos_to_world_uv(vtx_rel_pos,TextureResource.inst().get_tex(_main_fill.texkey()));
			_main_fill.manual_set_uv(vtx,SPUtil.vec_add(vtx_uv_pos,recenter_offset));
		}
		_main_fill.set_manual_sort_z_order(GameAnchorZ.BGWater_Underwater_Obstacle_FILL);
	}
	
	private Vector2 vtx_rel_pos_to_world_uv(Vector2 vtx_rel_pos, Texture tex) {
		Vector2 world_pos = SPUtil.vec_add(vtx_rel_pos,_pos);
		Vector2 tex_size = new Vector2(tex.width,tex.height); 
		return SPUtil.vec_div(world_pos,tex_size);
	}
	
	public override void on_added_to_manager(GameEngineScene g, DiveGameState state) {
		state._enemy_manager.register_obstacle(this);
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {}
	
	public override SPHitRect get_hit_rect() {
		return SPHitPoly.hitpoly_to_bounding_hitrect(this.get_hit_poly(),Vector2.zero,Vector2.zero);
	}
	public override SPHitPoly get_hit_poly() {
		Vector2 point = _root.get_u_pos();
		return new SPHitPoly() {
			_pts0 = point,
			_pts1 = SPUtil.vec_add(point,SPUtil.vec_sub(_pt1,_pt0)),
			_pts2 = SPUtil.vec_add(point,SPUtil.vec_sub(_pt2,_pt0)),
			_pts3 = SPUtil.vec_add(point,SPUtil.vec_sub(_pt3,_pt0))
		};
	}
	
	public override void debug_draw_hitboxes(SPDebugRender draw) {
		draw.draw_hitpoly_owner(this,new Color(0.8f, 0.2f, 0.2f, 0.3f), new Color(0.8f, 0.2f, 0.2f, 0.65f));
	}
}
