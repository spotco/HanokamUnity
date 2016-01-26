using System;
using UnityEngine;
using System.Collections;
//stackoverflow.com/questions/10962379/how-to-check-intersection-between-2-rotated-rectangles

public interface SPHitPolyOwner {
	SPHitRect get_hit_rect();
	SPHitPoly get_hit_poly();
}

public class DelegatedSPHitPolyOwner {
	public Func<SPHitRect> _hit_rect_delegate;
	public Func<SPHitPoly> _hit_poly_delegate;
	public SPHitRect get_hit_rect() {
		return _hit_rect_delegate();
	}
	public SPHitPoly get_hit_poly() {
		return _hit_poly_delegate();
	}
}

public struct SPHitPoly {
	public Vector2 _pts0, _pts1, _pts2, _pts3;
	public int length { get { return 4; } }
	public Vector2 pts(int i) {
		switch(i) {
		case 0: return _pts0;
		case 1: return _pts1;
		case 2: return _pts2;
		case 3: return _pts3;
		default: return _pts3;
		}
	}
	public override string ToString() {
		return SPUtil.sprintf("{{%f,%f},{%f,%f},{%f,%f},{%f,%f}}",
			_pts0.x,_pts0.y,
			_pts1.x,_pts1.y,
			_pts2.x,_pts2.y,
			_pts3.x,_pts3.y
		);
	}
	
	private static SPHitPoly[] __polygons = new SPHitPoly[2];
	public static bool polyowners_intersect(SPHitPolyOwner a, SPHitPolyOwner b) {
		if (!SPHitRect.hitrect_touch(a.get_hit_rect(),b.get_hit_rect())) return false;
		
		__polygons[0] = a.get_hit_poly();
		__polygons[1] = b.get_hit_poly();
		for(int i = 0; i < 2; i++) { 
			SPHitPoly polygon = __polygons[i];
			for (int i1 = 0; i1 < 4; i1++) {
				
				int i2 = (i1 + 1) % polygon.length;
				Vector2 p1 = polygon.pts(i1);
				Vector2 p2 = polygon.pts(i2);
				
				Vector2 normal = new Vector2(p2.y-p1.y,p1.x-p2.x);
				
				float minA = float.NaN;
				float maxA = float.NaN;
				
				for (int j = 0; j < __polygons[0].length; j++) {
					float projected = normal.x * __polygons[0].pts(j).x + normal.y * __polygons[0].pts(j).y;
					if (float.IsNaN(minA) || projected < minA) {
						minA = projected;
					}
					if (float.IsNaN(maxA) || projected > maxA) {
						maxA = projected;
					}
				}
				
				float minB = float.NaN;
				float maxB = float.NaN;
				
				for (int j = 0; j < __polygons[1].length; j++) {
					float projected = normal.x * __polygons[1].pts(j).x + normal.y * __polygons[1].pts(j).y;
					
					if (float.IsNaN(minB) || projected < minB) {
						minB = projected;
					}
					if (float.IsNaN(maxB) || projected > maxB) {
						maxB = projected;
					}
				}
				
				if (maxA < minB || maxB < minA) {
					return false;
				}
			}
		}
		return true;
	}
	
	public static SPHitRect hitpoly_to_bounding_hitrect(SPHitPoly buf, Vector2 extend_min, Vector2 extend_max) {
		float min_x = Mathf.Min(buf.pts(0).x,Mathf.Min(buf.pts(1).x,Mathf.Min(buf.pts(2).x,buf.pts(3).x)));
		float min_y = Mathf.Min(buf.pts(0).y,Mathf.Min(buf.pts(1).y,Mathf.Min(buf.pts(2).y,buf.pts(3).y)));
		float max_x = Mathf.Max(buf.pts(0).x,Mathf.Max(buf.pts(1).x,Mathf.Max(buf.pts(2).x,buf.pts(3).x)));
		float max_y = Mathf.Max(buf.pts(0).y,Mathf.Max(buf.pts(1).y,Mathf.Max(buf.pts(2).y,buf.pts(3).y)));
		return new SPHitRect() {
			_x1 = min_x-extend_min.x,
			_y1 = min_y-extend_min.y,
			_x2 = max_x+extend_max.x,
			_y2 = max_y+extend_max.y
		};
	}
	
	public static SPHitPoly cons_with_basis_offset(Vector2 position, float rotation, Vector2 size, Vector2 mul_scf, float scf, Vector2 basis_offset) {
		Vector3 right = SPUtil.ang_deg_dir(rotation);
		Vector3 up = SPUtil.vec_cross(new Vector3(0,0,1), right);
		float wid = size.x/2 * scf * mul_scf.x;
		float hei = size.y/2 * scf * mul_scf.y;
		return new SPHitPoly() {
			_pts0 = SPUtil.vec_basis_transform_point(position, up, -hei+basis_offset.x, right, -wid+basis_offset.y),
			_pts1 = SPUtil.vec_basis_transform_point(position, up, -hei+basis_offset.x, right, wid+basis_offset.y),
			_pts2 = SPUtil.vec_basis_transform_point(position, up, hei+basis_offset.x, right, wid+basis_offset.y),
			_pts3 = SPUtil.vec_basis_transform_point(position, up, hei+basis_offset.x, right, -wid+basis_offset.y)
		};
	}
}