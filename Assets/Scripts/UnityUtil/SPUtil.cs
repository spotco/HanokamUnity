using UnityEngine;
using System.Collections.Generic;

public struct SPRange {
	public float _min, _max;
	public bool contains(float val) {
		return (val >= this._min) && (val <= this._max);
	}
	public SPRange extend(float val) {
		return new SPRange() { _min = this._min - val, _max = this._max + val };
	}
	public override string ToString() {
		return string.Format("SPRange({0},{1})",_min,_max);
	}
	public float clamp(float val) {
		return Mathf.Clamp(val,_min,_max);
	}
	public static bool is_range_intersect(SPRange a, SPRange b) {
		return (a._min > b._min && a._min < b._max) ||
			(a._max > b._min && a._max < b._max) ||
			(b._min > a._min && b._min < a._max) ||
			(b._max > a._min && b._max < a._max);
	}
}

public struct SPHitRect {
	public float _x1,_y1,_x2,_y2;
	public Vector2 get_center() { return new Vector2((_x2-_x1)/2.0f+_x1,(_y2-_y1)/2.0f+_y1); }
	public static bool hitrect_touch(SPHitRect r1, SPHitRect r2) {
		return !(r1._x1 > r2._x2 ||
		         r2._x1 > r1._x2 ||
		         r1._y1 > r2._y2 ||
		         r2._y1 > r1._y2);
	}
	public static SPHitRect extend(SPHitRect rect, float by) {
		return new SPHitRect() {
			_x1 = rect._x1 - by,
			_y1 = rect._y1 - by,
			_x2 = rect._x2 + by,
			_y2 = rect._y2 + by
		};
	}
	public static bool hitrect_contains_pt(SPHitRect rect, Vector2 pt) {
		return (pt.x > rect._x1 && pt.x < rect._x2) && (pt.y > rect._y1 && pt.y < rect._y2);
	}
	public override string ToString() {
		return string.Format("SPHitRect(({0},{1}),({2},{3}))",_x1,_y1,_x2,_y2);
	}
}

public struct SPLineSegment {
	public Vector2 _pt0, _pt1;
	public static Vector2 line_seg_intersection(SPLineSegment a, SPLineSegment b) {
		return SPLineSegment.line_seg_intersection_pts(a._pt0,a._pt1,b._pt0,b._pt1);
	}
	
	public static Vector2 line_seg_intersection_pts(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2) {
		double Ax = a1.x; double Ay = a1.y;
		double Bx = a2.x; double By = a2.y;
		double Cx = b1.x; double Cy = b1.y;
		double Dx = b2.x; double Dy = b2.y;
		double X; double Y;
		double  distAB, theCos, theSin, newX, ABpos ;
		
		if ((Ax==Bx && Ay==By) || (Cx==Dx && Cy==Dy)) return new Vector2(float.NaN,float.NaN); //  Fail if either line segment is zero-length.
		
		Bx-=Ax; By-=Ay;//Translate the system so that point A is on the origin.
		Cx-=Ax; Cy-=Ay;
		Dx-=Ax; Dy-=Ay;
		
		distAB=System.Math.Sqrt(Bx*Bx+By*By);//Discover the length of segment A-B.
		
		theCos=Bx/distAB;//Rotate the system so that point B is on the positive X axis.
		theSin=By/distAB;
		
		newX=Cx*theCos+Cy*theSin;
		Cy  =Cy*theCos-Cx*theSin; Cx=newX;
		newX=Dx*theCos+Dy*theSin;
		Dy  =Dy*theCos-Dx*theSin; Dx=newX;
		
		if ((Cy<0.0 && Dy<0.0) || (Cy>=0.0 && Dy>=0.0)) return new Vector2(float.NaN,float.NaN); //C-D must be origin crossing line
		
		ABpos=Dx+(Cx-Dx)*Dy/(Dy-Cy);//Discover the position of the intersection point along line A-B.
		
		
		if (ABpos<0.0 || ABpos-distAB> 0.001) {
			return new Vector2(float.NaN,float.NaN);//  Fail if segment C-D crosses line A-B outside of segment A-B.
		}
		
		X=Ax+ABpos*theCos;//Apply the discovered position to line A-B in the original coordinate system.
		Y=Ay+ABpos*theSin;
		
		return new Vector2((float)X,(float)Y);
	}
	
	public SPHitRect bounding_rect() {
		return new SPHitRect() {
			_x1 = Mathf.Min(_pt0.x,_pt1.x),
			_y1 = Mathf.Min(_pt0.y,_pt1.y),
			_x2 = Mathf.Max(_pt0.x,_pt1.x),
			_y2 = Mathf.Max(_pt0.y,_pt1.y)
		};
	}
	
	public float pt_parametric_t(Vector2 pt) {
		return SPUtil.vec_dist(pt,_pt0) / SPUtil.vec_dist(_pt1,_pt0);
	}
	
	public SPLineSegment extend(float val) {
		Vector2 dir = SPUtil.vec_sub(_pt1,_pt0).normalized;
		_pt0 = SPUtil.vec_add(_pt0,SPUtil.vec_scale(dir,-val));
		_pt1 = SPUtil.vec_add(_pt1,SPUtil.vec_scale(dir,val));
		return this;
	}
	
	public static bool is_intersect_point(Vector2 pt) {
		return !float.IsNaN(pt.x) && !float.IsNaN(pt.y);
	}
	
	public override string ToString() {
		return string.Format("SPLineSegment(({0})->({1}))",_pt0,_pt1);
	}
}

public interface CameraRenderHookDelegate {
	void on_pre_render();
	void on_post_render();
}

public class CameraRenderHookDispatcher : MonoBehaviour {
	public CameraRenderHookDelegate _delegate;
	public void OnPreCull() {
		_delegate.on_pre_render();
	}
	public void OnPostRender() {
		_delegate.on_post_render();
	}
}

public class SPUtil {

	public static string sprintf(string format ,params object[] varargs) {
		return AT.MIN.Tools.sprintf(format,varargs);
	}

	public static void logf(string format ,params object[] varargs) {
		Debug.Log(SPUtil.sprintf(format,varargs));
	}

	private static System.Security.Cryptography.MD5 _md5 = System.Security.Cryptography.MD5.Create();
	public static long md5(string input) {
		byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
		byte[] hash = SPUtil._md5.ComputeHash(inputBytes);
		return System.BitConverter.ToInt64(hash,0);
	}

	public static byte[] streaming_asset_load(string filePath) {
		byte[] result;
#if UNITY_WEBPLAYER
		WWW www = new WWW(filePath);
		while(!www.isDone);
		result = www.bytes;
		return result;
#else
		if (filePath.Contains("://")) {
			WWW www = new WWW(filePath);
			while(!www.isDone);
			result = www.bytes;
		} else {
			result = System.IO.File.ReadAllBytes(filePath);
		}
		return result;
#endif
	}

	public static System.Random rand = new System.Random(69);
	public static float float_random(float min, float max) {
		float r = (float)rand.NextDouble();
		return (max-min)*r + min;
	}
	public static int int_random(int min, int max) {
		return Mathf.FloorToInt(float_random(min,max));
	}

	public static Vector3 valv(float val) {
		return new Vector3(val,val,val);
	}

	public static Vector2 view_screen() {
		return new Vector2(Screen.width,Screen.height);
	}

	public static Vector2 game_screen() {
		return new Vector2(Screen.width * GameMain._context._game_camera.rect.width ,Screen.height * GameMain._context._game_camera.rect.height );
	}
	
	public static Vector2 game_from_view_screen_offset() {
		return new Vector2(GameMain._context._game_camera.rect.x * Screen.width,GameMain._context._game_camera.rect.y * Screen.height);
	}

	public static SPRange get_horiz_world_bounds() {
		return new SPRange(){ _min = -875, _max = 875 };
	}

	public static float dt_scale_get() {
		return (Time.deltaTime) / (1 / 60.0f);
	}

	public static Rect texture_default_rect(string key) {
		Texture tex = TextureResource.inst().get_tex(key);
		return new Rect(0,0,tex.width,tex.height);
	}
	public static float sec_to_tick(float sec) {
		return (1 / 60.0f) / sec;
	}
	public static float drpt(float start, float to, float fric) {
		// y = e ^ (-a * timescale)
		fric = 1 - fric;
		float a = -Mathf.Log(fric);
		float y = 1 - Mathf.Exp(-a * SPUtil.dt_scale_get());
		
		// rtv = start + (to - start) * timescaled_friction
		float delta = (to - start) * y;
		return start + delta;
	}
	public static Vector3 drpt_vec(Vector3 start, Vector3 to, float fric) {
		float dist = Vector3.Distance(start,to);
		if (dist > 0) {
			Vector3 dir = SPUtil.vec_sub(to,start).normalized;
			float neu_dist = SPUtil.drpt(dist,0,fric);
			return SPUtil.vec_add(start,SPUtil.vec_scale(dir,dist-neu_dist));
		} else {
			return to;
		}
	}
	
	public static float drpty(float fric) {
		// y = e ^ (-a * timescale)
		fric = 1 - fric;
		float a = -Mathf.Log(fric);
		float y = 1 - Mathf.Exp(-a * SPUtil.dt_scale_get());
		return y;
	}
	public static float lerp(float a, float b, float t) {
		return a + (b - a) * t;
	}
	
	public static float dir_ang_deg(float x, float y) {
		return SPUtil.rad_to_deg(Mathf.Atan2(y,x));
	}
	public static Vector2 ang_deg_dir(float deg) {
		float rad = SPUtil.deg_to_rad(deg);
		return new Vector2(Mathf.Cos(rad),Mathf.Sin(rad));
	}
	public static bool flt_cmp_delta(float a, float b, float delta) {
		return Mathf.Abs(a-b) <= delta;
	}

	public static float deg_to_rad(float degrees) {
		return degrees * Mathf.PI / 180.0f;
	}
	
	public static float rad_to_deg(float rad) {
		return rad * 180.0f / Mathf.PI;
	}

	public static float fmod(float a, float b) { return a % b; }
	public static float shortest_angle(float src, float dest) {
		float shortest_angle=fmod((fmod((dest - src) , 360) + 540), 360) - 180;
		return shortest_angle;
	}

	public static float bezier_val_for_t(float p0, float p1, float p2, float p3, float t) {
		float cp0 = (1 - t) * (1 - t) * (1 - t);
		float cp1 = 3 * t * (1-t)*(1-t);
		float cp2 = 3 * t * t * (1 - t);
		float cp3 = t * t * t;
		return cp0 * p0 + cp1 * p1 + cp2 * p2 + cp3 * p3;
	}
	public static Vector2 bezier_val_for_t(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
		return new Vector2(
			bezier_val_for_t(p0.x,p1.x,p2.x,p3.x,t),
			bezier_val_for_t(p0.y,p1.y,p2.y,p3.y,t)
		);
	}
	public static Vector3 bezier_val_for_t(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		return new Vector3(
			bezier_val_for_t(p0.x,p1.x,p2.x,p3.x,t),
			bezier_val_for_t(p0.y,p1.y,p2.y,p3.y,t),
			bezier_val_for_t(p0.z,p1.z,p2.z,p3.z,t)
		);
	}

	public static float low_filter(float value, float min) {
		return Mathf.Abs(value) < Mathf.Abs(min) ? 0 : value;
	}
	
	public static float running_avg(float avg, float val, float ct) {
		avg -= (avg / ct) * SPUtil.dt_scale_get();
		avg += (val / ct) * SPUtil.dt_scale_get();
		return avg;
	}
	public static Vector3 running_avg_vec(Vector3 avg, Vector3 val, float ct) {
		avg.x = SPUtil.running_avg(avg.x,val.x,ct);
		avg.y = SPUtil.running_avg(avg.y,val.y,ct);
		avg.z = SPUtil.running_avg(avg.z,val.z,ct);
		return avg;
	}
	
	public static float y_for_point_of_2pt_line(Vector2 pt1, Vector2 pt2, float x) {
		//(y - y1)/(x - x1) = m
		float m = (pt1.y - pt2.y) / (pt1.x - pt2.x);
		//y - mx = b
		float b = pt1.y - m * pt1.x;
		return m * x + b;
	}
	
	public static float eclamp(float val, float min, float max, Vector2 n_bez_ctrl1, Vector2 n_bez_ctrl2) {
		float t = Mathf.Clamp((val-min)/(max-min),0,1);
		float rtv_normalized = SPUtil.bezier_val_for_t(new Vector2(0,0),n_bez_ctrl1,n_bez_ctrl2,new Vector2(1,1),t).y;
		return SPUtil.lerp(min,max,rtv_normalized);
	}
	
	public static Vector4 color_from_bytes(float r, float g, float b, float a) {
		return new Vector4(r/255.0f,g/255.0f,b/255.0f,a/255.0f);
	}
	
	public static bool transform_and_parents_is_enabled(Transform t) {
		Transform itr = t;
		while (itr != null) {
			if (!itr.gameObject.activeSelf) {
				return false;
			}
			itr = itr.parent;
		}
		return true;
	}

	public static Vector3 vec_lmovto(Vector3 a, Vector3 b, float vmax) {
		vmax = vmax * SPUtil.dt_scale_get();
		Vector3 delta = SPUtil.vec_sub(b,a);
		if (delta.magnitude <= vmax) {
			return b;
		} else {
			return SPUtil.vec_add(a,SPUtil.vec_scale(delta.normalized,vmax));
		}
	}
	
	public static float lmovto(float a, float b, float vmax) {
		float delta = Mathf.Abs(b-a);
		if (delta <= vmax) {
			return b;
		} else {
			return a+SPUtil.sig(b-a)*vmax;
		}
	}

	public static int sig(float a) {
		if (a > 0) {
			return 1;
		} else if (a < 0) {
			return -1;
		} else {
			return 0;
		}
	}
	
	public static bool cond_cast<T>(System.Object input, out T output) {
		if (input.GetType() == typeof(T)) {
			output = (T)input;
			return true;
		} else {
			output = default(T);
			return false;
		}
	}
	
	public static Vector3 vec_mid(Vector3 a, Vector3 b) {
		Vector3 add = a + b;
		return new Vector3(add.x/2,add.y/2,add.z/2);
	}
	public static Vector3 vec_mult(Vector3 a, Vector3 b) {
		return new Vector3(a.x*b.x,a.y*b.y,a.z*b.z);
	}
	public static Vector3 vec_div(Vector3 a, Vector3 b) {
		return new Vector3(a.x/b.x,a.y/b.y,a.z/b.z);
	}
	public static Vector3 vec_add(Vector3 a, Vector3 b) {
		return a + b;
	}
	public static Vector3 vec_sub(Vector3 a, Vector3 b) {
		return a - b;
	}
	public static Vector3 vec_scale(Vector3 vec, float scale) {
		return new Vector3(vec.x * scale, vec.y * scale, vec.z * scale);
	}
	public static Vector3 vec_cross(Vector3 v1, Vector3 a) {
		float x1, y1, z1;
		x1 = (v1.y*a.z) - (a.y*v1.z);
		y1 = -((v1.x*a.z) - (v1.z*a.x));
		z1 = (v1.x*a.y) - (a.x*v1.y);
		return new Vector3(x1, y1, z1);
	}
	public static float vec_dot(Vector3 a, Vector3 b) {
		return a.x*b.x+a.y*b.y+a.z*b.z;
	}
	public static bool vec_eq(Vector4 a, Vector4 b) {
		return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
	}
	public static Vector2 vec_basis_transform_point(Vector2 pt, Vector2 a, float a_s, Vector2 b, float b_s) {
		a = SPUtil.vec_scale(a.normalized,a_s);
		b = SPUtil.vec_scale(b.normalized,b_s);
		return new Vector2(
			pt.x + a.x + b.x,
			pt.y + a.y + b.y
		);
	}
	public static Vector3 vec_cons_norm(float x, float y = 0, float z = 0) {
		return (new Vector3(x,y,z)).normalized;
	}
	public static float vec_dist(Vector3 a, Vector3 b) {
		return Mathf.Sqrt(Mathf.Pow(a.x-b.x,2)+Mathf.Pow(a.y-b.y,2)+Mathf.Pow(a.z-b.z,2));
	}
	public static Vector3 vec_z = new Vector3(0,0,1);
	
	public static List<T> list_reverse<T>(List<T> list) {
		for (int i = 0; i < list.Count/2; i++) {
			T a = list[i];
			T b = list[list.Count-1-i];
			list[i] = b;
			list[list.Count-1-i] = a;
		}
		return list;
	}
	
	public static Vector2 point_line_intersection_dir(Vector2 from, Vector2 line_point, Vector2 line_dir) {
		Vector2 linept_to_pt_dir = SPUtil.vec_sub(from,line_point).normalized;
		Vector3 up = SPUtil.vec_cross(linept_to_pt_dir,line_dir);
		Vector2 dir_to = SPUtil.vec_cross(line_dir,up);
		if (dir_to.magnitude > 0) {
			return dir_to.normalized;
		} else {
			return SPUtil.vec_cross(line_dir,SPUtil.vec_z).normalized;
		}
	}
	
	public static float round_dec(float val,int i) {
		float div = Mathf.Pow(10,i);
		return Mathf.Floor(val*div)/div;
	}
	
	// deprecated please remove
	// SPTODO -- add child_pct_of_obj to SPUILayout
	public static Vector2 pct_of_obj(SPSprite obj, float x, float y) {
		return new Vector2(
			obj.texrect().width*(x-obj.anchorpoint().x),
			obj.texrect().height*(y-obj.anchorpoint().y)
		);
	}
	
	// deprecated please remove
	// SPTODO -- add add_to_parent_layout to SPUILayout
	public static Vector2 inverse_scale(SPNode obj) {
		Vector2 rtv = new Vector2(1,1);
		SPNode itr = obj;
		while (itr != null) {
			rtv.x /= itr.scale_x();
			rtv.y /= itr.scale_y();
			itr = itr._parent;
		}
		return rtv;
	}
	
	//deprecated please remove
	public static Vector2 fit_to_rect(SPSprite obj, SPHitRect rect) {
		Vector2 obj_size = obj.texrect().size;
		return new Vector2(
			(rect._x2 - rect._x1)/obj_size.x,
			(rect._y2 - rect._y1)/obj_size.y
		);
	}

}

public struct DrptVal {
	public float _current, _target, _drptval;

	public void i_update() {
		if (_drptval <= 0) return;
		_current = SPUtil.drpt(_current,_target,_drptval);
	}
	public void clamp_lt(float ltv) {
		if (SPUtil.flt_cmp_delta(_current,_target, ltv)) {
			_current = _target;
		}
	}
}

public struct DrptVec {
	public Vector3 _current, _target;
	public float _drptval;
	
	public void i_update() {
		float dist = Vector3.Distance(_target,_current);
		if (dist > 0) {
			Vector3 dir = SPUtil.vec_sub(_target,_current).normalized;
			float neu_dist = SPUtil.drpt(dist,0,_drptval);
			_current = SPUtil.vec_add(_current,SPUtil.vec_scale(dir,dist-neu_dist));
		}
	}
	public void clamp_lt(float ltv) {
		float dist = Vector3.Distance(_target,_current);
		if (dist < ltv) {
			_current = _target;
		}
	}
}

public class MultiMap<TKey, TValue> {
	public SPDict<TKey,List<TValue>> _key_to_list = new SPDict<TKey, List<TValue>>();
	public int count_of(TKey key) {
		if (!_key_to_list.ContainsKey(key)) _key_to_list[key] = new List<TValue>();
		return _key_to_list[key].Count;
	}
	public void add(TKey key, TValue val) {
		if (!_key_to_list.ContainsKey(key)) _key_to_list[key] = new List<TValue>();
		_key_to_list[key].Add(val);
	}
	public void clear(TKey key) {
		if (!_key_to_list.ContainsKey(key)) _key_to_list[key] = new List<TValue>();
		_key_to_list[key].Clear();
	}
	public bool contains_key(TKey key) {
		return _key_to_list.ContainsKey(key);
	}
	public List<TValue> list(TKey key) {
		if (!_key_to_list.ContainsKey(key)) _key_to_list[key] = new List<TValue>();
		return _key_to_list[key];
	}
	public List<TKey> keys() {
		return _key_to_list.key_itr();
	}
}

public class SPAlphaGroup {
	private List<SPAlphaGroupElement> _elements = new List<SPAlphaGroupElement>();
	private float _alpha = 1;
	public static SPAlphaGroup cons() { return new SPAlphaGroup(); }
	public void add_element(SPAlphaGroupElement tar) {
		tar.set_alpha_mult(_alpha);
		_elements.Add(tar);
	}
	public void set_alpha_mult(float mult) {
		_alpha = mult;
		for (int i = 0; i < _elements.Count; i++) {
			_elements[i].set_alpha_mult(_alpha);
		}
	}
	public float get_alpha() {
		return _alpha;
	}
}

public class SPUILayout {
	public struct SpriteLayout {
		public Vector2 _anchor_point;
		public Vector2 _nparent_origin;
		public Vector2 _nparent_placement;
		public Vector2 _p_origin_offset;
		public Vector2 _p_placement_offset;
	}
	
	public static void layout_sprite(SPSprite tar, SPHitRect parent, SPUILayout.SpriteLayout layout) {
		Vector2 p_parent_size = new Vector2(parent._x2-parent._x1,parent._y2-parent._y1);
		
		Vector2 nparent_element_size = new Vector2(layout._nparent_placement.x-layout._nparent_origin.x,layout._nparent_placement.y-layout._nparent_origin.y);
		Vector2 p_no_offset_element_size = SPUtil.vec_mult(p_parent_size,nparent_element_size);
		Vector2 p_element_size = SPUtil.vec_add(layout._p_placement_offset,
			SPUtil.vec_add(p_no_offset_element_size,SPUtil.vec_scale(layout._p_origin_offset,-1)));
		
		Vector2 p_anchor00_origin_position = SPUtil.vec_add(SPUtil.vec_mult(p_parent_size,layout._nparent_origin),new Vector2(parent._x1,parent._y1));
		Vector2 p_anchorlayout_origin_position = SPUtil.vec_add(p_anchor00_origin_position,SPUtil.vec_mult(p_element_size,layout._anchor_point));
		Vector2 p_tar_origin_position = SPUtil.vec_add(p_anchorlayout_origin_position,layout._p_origin_offset);
		
		Vector2 element_default_size = new Vector2(tar.texrect().width,tar.texrect().height);
		Vector2 element_tar_scale = SPUtil.vec_div(p_element_size,element_default_size);
		
		tar.set_scale_x(element_tar_scale.x);
		tar.set_scale_y(element_tar_scale.y);
		tar.set_anchor_point(layout._anchor_point.x,layout._anchor_point.y);
		
		tar.set_u_pos(p_tar_origin_position);
	}
	
	public static SPHitRect get_layout_rect(SPSprite tar) {
		Vector2 p_sprite_size = SPUtil.vec_mult(new Vector2(tar.texrect().width,tar.texrect().height),new Vector2(tar.scale_x(),tar.scale_y()));
		Vector2 p_anchor_offset = SPUtil.vec_scale(SPUtil.vec_mult(tar.anchorpoint(),p_sprite_size),-1);
		Vector2 p_rtv_origin = SPUtil.vec_add(p_anchor_offset,tar.get_u_pos());
	
		return new SPHitRect() {
			_x1 = p_rtv_origin.x,
			_y1 = p_rtv_origin.y,
			_x2 = p_rtv_origin.x + p_sprite_size.x,
			_y2 = p_rtv_origin.y + p_sprite_size.y
		};
	}
	
	public static Vector2 sibling_pct_of_obj(SPHitRect parent_rect, Vector2 nparent_position) {
		return new Vector2(
			(parent_rect._x2-parent_rect._x1) * nparent_position.x + parent_rect._x1,
			(parent_rect._y2-parent_rect._y1) * nparent_position.y + parent_rect._y1
		);
	}
}

public class SPPhysics {
	public struct ElasticBodyCollisionParameters {
		public Vector2 _body_a_pos, _body_b_pos;
		public Vector2 _body_a_vel, _body_b_vel;
		public float _body_a_mass, _body_b_mass;
		public float _a_to_b_elasticity_coef, _b_to_a_elasticity_coef;
	}
	public struct ElasticBodyCollisionValues {
		public Vector2 _body_a_dir, _body_b_dir;
		public float _body_a_speed, _body_b_speed;
	}
	
	public static ElasticBodyCollisionValues elastic_body_collision(ElasticBodyCollisionParameters param) {
		Vector2 pos_delta = SPUtil.vec_sub(param._body_b_pos,param._body_a_pos);
		Vector2 pos_delta_dir = pos_delta.normalized;
		
		return new SPPhysics.ElasticBodyCollisionValues() {
			_body_a_speed = (param._body_b_mass / param._body_a_mass) * param._body_b_vel.magnitude * param._b_to_a_elasticity_coef,
			_body_b_speed = (param._body_a_mass / param._body_b_mass) * param._body_a_vel.magnitude * param._a_to_b_elasticity_coef,
			_body_a_dir = SPUtil.vec_add(SPUtil.vec_scale(pos_delta_dir,-1),param._body_b_vel.normalized).normalized,
			_body_b_dir = SPUtil.vec_add(pos_delta_dir,param._body_a_vel.normalized).normalized
		};
	}
	
	
	public interface SolidCollisionUpdateDelegate {
		void test_move_delta(Vector2 pos_delta, List<SPHitPolyOwner> obstacles, System.Object parameters);
	}
	public struct SolidCollisionValues {
		public bool _do_not_move;
		public bool _is_collide_this_frame;
		public Vector2 _collide_pushback_vel;
		public Vector2 _collision_normal;
		public Vector2 _collision_tangent;
	}
	
	public static List<SPHitPolyOwner> __tmp_list = new List<SPHitPolyOwner>();
	public static List<SPHitPolyOwner> tmp_list() { __tmp_list.Clear(); return __tmp_list; }
	
	public static bool hit_any(SPHitPolyOwner target, List<SPHitPolyOwner> obstacles) {
		for (int i_obstacle = 0; i_obstacle < obstacles.Count; i_obstacle++) {
			SPHitPolyOwner itr_obstacle = (SPHitPolyOwner)obstacles[i_obstacle];
			if (SPHitPoly.polyowners_intersect(target,itr_obstacle)) return true;	
		}
		return false;
	}
	
	private static List<SPHitPolyOwner> __world_bounds_obstacles = new List<SPHitPolyOwner>();
	private static SPHitPolyOwnerImpl __world_bounds_left = new SPHitPolyOwnerImpl();
	private static SPHitPolyOwnerImpl __world_bounds_right = new SPHitPolyOwnerImpl();
	public static List<SPHitPolyOwner> get_world_bounds_extra_obstacles(SPHitRect center) {
		__world_bounds_obstacles.Clear();
		
		SPRange world_bounds = SPUtil.get_horiz_world_bounds();
		float min_y = center._y1 - 500;
		float max_y = center._y2 + 500;
		
		__world_bounds_left._poly._pts0 = new Vector2(world_bounds._min,min_y);
		__world_bounds_left._poly._pts1 = new Vector2(world_bounds._min,max_y);
		__world_bounds_left._poly._pts2 = SPUtil.vec_add(__world_bounds_left._poly._pts1, new Vector2(-500,0));
		__world_bounds_left._poly._pts3 = SPUtil.vec_add(__world_bounds_left._poly._pts0, new Vector2(-500,0));
		
		__world_bounds_right._poly._pts0 = new Vector2(world_bounds._max,min_y);
		__world_bounds_right._poly._pts1 = new Vector2(world_bounds._max,max_y);
		__world_bounds_right._poly._pts2 = SPUtil.vec_add(__world_bounds_right._poly._pts1, new Vector2(500,0));
		__world_bounds_right._poly._pts3 = SPUtil.vec_add(__world_bounds_right._poly._pts0, new Vector2(500,0));
		
		__world_bounds_obstacles.Add(__world_bounds_left);
		__world_bounds_obstacles.Add(__world_bounds_right);
		return __world_bounds_obstacles;
	}
	
	private static List<Vector2> __tmp_collision_normals = new List<Vector2>();
	private static List<Vector2> __tmp_collision_tangents = new List<Vector2>();
	private static List<SPHitPolyOwner> __test_obstacles = new List<SPHitPolyOwner>();
	public static SolidCollisionValues solid_collision_update_frame(
		SPHitPolyOwner target, 
		List<SPHitPolyOwner> obstacles, 
		SolidCollisionUpdateDelegate callback, 
		float min_check_magnitude, 
		float search_start_angle, 
		System.Object parameters,
		List<SPHitPolyOwner> extra_obstacles = null) {
		
		List<SPHitPolyOwner> test_obstacles;
		{
			SPHitRect target_rect = target.get_hit_rect();
			SPRange target_range = new SPRange() {
				_min = target_rect._y1,
				_max = target_rect._y2
			};
			
			__test_obstacles.Clear();
			test_obstacles = __test_obstacles;
			for (int i_obstacle = 0; i_obstacle < obstacles.Count; i_obstacle++) {
				SPHitPolyOwner itr_obstacle = obstacles[i_obstacle];
				SPHitRect itr_rect = itr_obstacle.get_hit_rect();
				SPRange itr_obstacle_yrange = (new SPRange() {
					_min = itr_rect._y1,
					_max = itr_rect._y2
				}).extend(100);
				
				if (SPRange.is_range_intersect(target_range,itr_obstacle_yrange)) {
					test_obstacles.Add(itr_obstacle);
				}	
			}
		}
		
		if (extra_obstacles != null) {
			for (int i_obstacle = 0; i_obstacle < extra_obstacles.Count; i_obstacle++) {
				test_obstacles.Add(extra_obstacles[i_obstacle]);
			}
		}
		
		
		SolidCollisionValues rtv = new SolidCollisionValues() {
			_is_collide_this_frame = false,
			_do_not_move = false
		};
		
		for (int i_obstacle = 0; i_obstacle < test_obstacles.Count; i_obstacle++) {
			SPHitPolyOwner itr_obstacle = (SPHitPolyOwner)test_obstacles[i_obstacle];
			if (SPHitPoly.polyowners_intersect(itr_obstacle,target)) {
				Vector2 pos_delta = Vector2.zero;
				bool loop_continue = true;
				float magnitude = Mathf.Floor(min_check_magnitude);
				int check_ct = 8;
				int loop_ct = 0;
				while (loop_continue) {
					for (float i_check_ct = 0; i_check_ct <= check_ct; i_check_ct++) {
						Vector2 delta = SPUtil.vec_scale(SPUtil.ang_deg_dir((360.0f/check_ct)*i_check_ct + search_start_angle),magnitude);
						callback.test_move_delta(delta,test_obstacles,parameters);
						if (!SPHitPoly.polyowners_intersect(itr_obstacle,target) && !SPPhysics.hit_any(target,test_obstacles)) {
							loop_continue = false;
							rtv._is_collide_this_frame = true;
							rtv._collide_pushback_vel = delta;
							
							__tmp_collision_normals.Clear();
							__tmp_collision_tangents.Clear();
							
							SPHitPoly obstacle_poly = itr_obstacle.get_hit_poly();
							SPHitPoly target_poly = target.get_hit_poly();
							for (int i_target_poly_vtx = 0; i_target_poly_vtx < target_poly.length; i_target_poly_vtx++) {
								SPLineSegment itr_target_vtx_seg = new SPLineSegment() {
									_pt0 = target_poly.pts(i_target_poly_vtx),
									_pt1 = SPUtil.vec_add(target_poly.pts(i_target_poly_vtx),SPUtil.vec_scale(delta,-1))
								}.extend(50);
								
								bool found = false;
								float lowest_t = float.PositiveInfinity;
								Vector2 tmp_normal = new Vector2();
								Vector2 tmp_tangent = new Vector2();
								
								for (int i_obstacle_poly_vtx = 0; i_obstacle_poly_vtx < obstacle_poly.length; i_obstacle_poly_vtx++) {
									SPLineSegment itr_obstacle_vtx_seg = obstacle_poly.line_segment_of_point(i_obstacle_poly_vtx);
									Vector2 intersection = SPLineSegment.line_seg_intersection(itr_target_vtx_seg,itr_obstacle_vtx_seg);
									if (SPLineSegment.is_intersect_point(intersection)) {
										found = true;
										SPLineSegment itr_obstacle_vtx_seg_next = obstacle_poly.line_segment_of_point(i_obstacle_poly_vtx+1);
										Vector2 seg_dir = SPUtil.vec_sub(itr_obstacle_vtx_seg._pt1,itr_obstacle_vtx_seg._pt0).normalized;
										Vector2 seg_next_dir = SPUtil.vec_sub(itr_obstacle_vtx_seg_next._pt1,itr_obstacle_vtx_seg_next._pt0).normalized;
										Vector3 out_cross_dir = SPUtil.vec_cross(seg_dir,seg_next_dir);
										Vector2 normal = SPUtil.vec_cross(seg_dir,out_cross_dir).normalized;
										float intersection_t = itr_target_vtx_seg.pt_parametric_t(intersection);
										
										if (intersection_t < lowest_t) {
											tmp_normal = normal;
											tmp_tangent = seg_dir;
										}
									}
								}
								
								if (found) {
									__tmp_collision_normals.Add(tmp_normal);
									__tmp_collision_tangents.Add(tmp_tangent);
								}
								
							}
							
							if (__tmp_collision_normals.Count > 0) {
								Vector2 collision_normal_avg = new Vector2();
								Vector2 collision_tangent_avg = new Vector2();
								for (int j = 0; j < __tmp_collision_normals.Count; j++) {
									collision_normal_avg = SPUtil.vec_add(collision_normal_avg,__tmp_collision_normals[j]);
									collision_tangent_avg = SPUtil.vec_add(collision_tangent_avg,__tmp_collision_tangents[j]);
								}
								collision_normal_avg = SPUtil.vec_scale(collision_normal_avg,1.0f/__tmp_collision_normals.Count);
								collision_tangent_avg = SPUtil.vec_scale(collision_tangent_avg,1.0f/__tmp_collision_normals.Count);
								rtv._collision_normal = collision_normal_avg.normalized;
								rtv._collision_tangent = collision_tangent_avg.normalized;
							} else {
								rtv._collision_normal = new Vector2(0,1);
								rtv._collision_tangent = new Vector2(1,0);
								//Debug.LogError("ERROR: collision no line segment found");
							}
							goto loop_end;
						}
					}
					magnitude += 1;
					loop_ct++;
					if (loop_ct > 35) {
						check_ct = 24;
					} else if (loop_ct > 20) {
						check_ct = 16;
					}
					if (loop_ct > 50) {
						rtv._do_not_move = true;
						goto loop_end;
					}
				}
			}
		}
		loop_end:;
		test_obstacles.Clear();
		return rtv;
	}
	
}

