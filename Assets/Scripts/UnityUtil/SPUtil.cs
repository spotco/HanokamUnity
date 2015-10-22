using UnityEngine;
using System.Collections.Generic;

public struct SPRange {
	public float _min, _max;
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

	public static Vector2 pct_of_obj(SPSprite obj, float x, float y) {
		return new Vector2(
			obj.texrect().width*(x-obj.anchorpoint().x),
			obj.texrect().height*(y-obj.anchorpoint().y)
		);
	}
	
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
	
	public static Vector2 fit_to_rect(SPSprite obj, SPHitRect rect) {
		Vector2 obj_size = obj.texrect().size;
		return new Vector2(
			(rect._x2 - rect._x1)/obj_size.x,
			(rect._y2 - rect._y1)/obj_size.y
		);
	}

	public static SPRange get_horiz_world_bounds() {
		return new SPRange(){ _min = -500, _max = 500 };
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

	public Vector2 line_seg_intersection_pts(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2) {
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

	public static float fmod(float a, float b) { return a % b; }
	public static float shortest_angle(float src, float dest) {
		float shortest_angle=fmod((fmod((dest - src) , 360) + 540), 360) - 180;
		return shortest_angle;
	}


	public static float bezier_val_for_t(float p0, float p1, float p2, float p3, float t) {
		float cp0 = (1 - t)*(1 - t)*(1 - t);
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
	
	public static bool transform_is_enabled(Transform t) {
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
		Vector3 delta = SPUtil.vec_sub(b,a);
		if (delta.magnitude <= vmax) {
			return b;
		} else {
			return SPUtil.vec_add(a,SPUtil.vec_scale(delta.normalized,vmax * SPUtil.dt_scale_get()));
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

}

public struct DrptVal {
	public float _current, _target, _drptval;

	public void i_update() {
		if (_drptval <= 0) return;
		_current = SPUtil.drpt(_current,_target,_drptval);
	}
}

/*
ELM (Eased Linear Motion) Value
Matches the start-end time of a linear motion curve with constant velocity,
but has motion easing and constant acceleration.
Can set target once, or every update.
*/
public struct ELMVal {
	private float _target_vel; //target velocity
	private float _t; //time in current curve
	private float _t_max; //max time of current curve
	private float _start,_end,_current; //starting, ending and current values
	private Vector2 _dval; //last update value delta, x is last timestep and y is value delta
	private Vector2 _nhalf_cp1; //normalized*0.5 point used for quadratic bezier curve control point 1
	
	public static ELMVal cons() {
		return new ELMVal(){
			_target_vel = 1,
			_nhalf_cp1 = new Vector2(0.5f,0)
		};
	}

	public ELMVal set_target_vel(float val) { _target_vel = val; this.set_target(this.get_target()); return this; }

	public float get_current() { return _current; }
	public void set_current(float val) { 
		_current = val; 
		this.set_target(this.get_target());

	}

	public float get_target() { return _end; }
	public bool get_finished() { return _t >= _t_max; }

	public float get_target_vel() { return _target_vel; }
	public void set_target(float target) {
		_end = target;
		_start = _current;
		_t = 0;
		_t_max = Mathf.Abs(_end-_start)/_target_vel;

		if (_dval.magnitude > 0) {
			_nhalf_cp1 = new Vector2(_dval.x,Mathf.Abs(_dval.y));
			_nhalf_cp1.Normalize();
			_nhalf_cp1.Scale(new Vector2(0.5f,0.5f));
		} else {
			_nhalf_cp1 = new Vector2(0.5f,0);
		}
	}
	
	public float i_update(float dt) {
		float prev_current = _current;
		_t = Mathf.Clamp(_t+dt,0,_t_max);
		float lerp_t = 1;
		if (_t_max != 0) {
			lerp_t = SPUtil.bezier_val_for_t(
				new Vector2(0,0),
				_nhalf_cp1,
				new Vector2(0.5f,1),
				new Vector2(1,1),
				_t/_t_max
			).y;
		}
		_current = SPUtil.lerp(_start,_end,lerp_t);
		_dval.x = dt;
		_dval.y = _current - prev_current;
		return _current;
	}
}
