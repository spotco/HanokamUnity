using UnityEngine;
using System.Collections;

public struct SPRange {
	public float _min, _max;
}

public struct SPHitRect {
	public float _x1,_y1,_x2,_y2;
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

	public static Vector2 pct_of_obj_u_with_anchorpt(SPSprite obj, float x, float y) {
		return new Vector2(obj.texrect().width*(x-obj.anchorpoint().x),obj.texrect().height*(y-obj.anchorpoint().y));
	}

	public static SPRange get_horiz_world_bounds() {
		return new SPRange(){ _min = -500, _max = 500 };
	}

	private static float _dt_scale = 1;
	public static void dt_scale_set(float val) {
		_dt_scale = val;
	}
	public static float dt_scale_get() {
		return _dt_scale;
	}
}
/*

@implementation CallBack
	@synthesize selector;
	@synthesize target;
	@synthesize context;
@end

@implementation TexRect
	@synthesize tex;
	@synthesize rect;
+(TexRect*)cons_tex:(CCTexture *)tex rect:(CGRect)rect {
    TexRect *r = [[TexRect alloc] init]; [r setTex:tex]; [r setRect:rect]; return r;
}

float drpt(float a, float b, float fric) {
	if (fric == 0) NSLog(@"ERROR FRIC 0");
	if (fric >= 1) NSLog(@"ERROR FRIC >= 1");
	float deltaf = (b - a);
	deltaf *= powf(fric,1/dt_scale_get());
	return a + deltaf;
}

float lerp(float a, float b, float t) {
	return a + (b - a) * t;
}

CGPoint lerp_point(CGPoint a, CGPoint b, float t) {
	return ccp(lerp(a.x, b.x, t),lerp(a.y, b.y, t));
}

CGRect cctexture_default_rect(CCTexture *tex) {
	return CGRectMake(0, 0, tex.pixelWidth, tex.pixelHeight);
}

int SIG(float n) {
    if (n > 0) {
        return 1;
    } else if (n < 0) {
        return -1;
    } else {
        return 0;
    }
}

CGPoint CGPointAdd(CGPoint a,CGPoint b) {
    return ccp(a.x+b.x,a.y+b.y);
}
CGPoint CGPointSub(CGPoint a,CGPoint b) {
    return ccp(a.x-b.x,a.y-b.y);
}
float CGPointDist(CGPoint a,CGPoint b) {
    return sqrtf(powf(a.x-b.x, 2)+powf(a.y-b.y, 2));
}
CGPoint CGPointMult(CGPoint a, CGPoint b) {
	return ccp(a.x*b.x,a.y*b.y);
}
CGPoint CGPointMid(CGPoint a,CGPoint b) {
	CGPoint add = CGPointAdd(a, b);
	return ccp(add.x/2,add.y/2);
}


bool fuzzyeq(float a, float b, float delta) {
	return ABS(a-b) <= delta;
}

float deg_to_rad(float degrees) {
    return degrees * M_PI / 180.0;
}

float rad_to_deg(float rad) {
    return rad * 180.0 / M_PI;
}

CGPoint line_seg_intersection_pts(CGPoint a1, CGPoint a2, CGPoint b1, CGPoint b2) {
	CGPoint null_point = CGPointMake(SEG_NO_VALUE(),SEG_NO_VALUE());
    double Ax = a1.x; double Ay = a1.y;
	double Bx = a2.x; double By = a2.y;
	double Cx = b1.x; double Cy = b1.y;
	double Dx = b2.x; double Dy = b2.y;
	double X; double Y;
	double  distAB, theCos, theSin, newX, ABpos ;
	
	if ((Ax==Bx && Ay==By) || (Cx==Dx && Cy==Dy)) return null_point; //  Fail if either line segment is zero-length.
    
	Bx-=Ax; By-=Ay;//Translate the system so that point A is on the origin.
	Cx-=Ax; Cy-=Ay;
	Dx-=Ax; Dy-=Ay;
	
	distAB=sqrt(Bx*Bx+By*By);//Discover the length of segment A-B.
	
	theCos=Bx/distAB;//Rotate the system so that point B is on the positive X axis.
	theSin=By/distAB;
    
	newX=Cx*theCos+Cy*theSin;
	Cy  =Cy*theCos-Cx*theSin; Cx=newX;
	newX=Dx*theCos+Dy*theSin;
	Dy  =Dy*theCos-Dx*theSin; Dx=newX;
	
	if ((Cy<0. && Dy<0.) || (Cy>=0. && Dy>=0.)) return null_point;//C-D must be origin crossing line
	
	ABpos=Dx+(Cx-Dx)*Dy/(Dy-Cy);//Discover the position of the intersection point along line A-B.
	
    
	if (ABpos<0. || ABpos-distAB> 0.001) {
        return null_point;//  Fail if segment C-D crosses line A-B outside of segment A-B.
	}
        
	X=Ax+ABpos*theCos;//Apply the discovered position to line A-B in the original coordinate system.
	Y=Ay+ABpos*theSin;
	
	return ccp(X,Y);
}

float shortest_angle(float src, float dest) {
	float shortest_angle=fmod((fmod((dest - src) , 360) + 540), 360) - 180;
	return shortest_angle;
}

float cubic_angular_interp(float src, float dest, float c1, float c2, float t) {
	t = cubic_interp(0, 1, c1, c2, t);
	return src + shortest_angle(src, dest) * t;
}

void scale_to_fit_screen_x(CCSprite *spr) {
	[spr setScaleX:game_screen().width/spr.textureRect.size.width];
}
void scale_to_fit_screen_y(CCSprite *spr) {
	[spr setScaleY:game_screen().height/spr.textureRect.size.height];
}

float bezier_val_for_t(float p0, float p1, float p2, float p3, float t) {
	float cp0 = (1 - t)*(1 - t)*(1 - t);
	float cp1 = 3 * t * (1-t)*(1-t);
	float cp2 = 3 * t * t * (1 - t);
	float cp3 = t * t * t;
	return cp0 * p0 + cp1 * p1 + cp2 * p2 + cp3 * p3;
}

CGPoint bezier_point_for_t(CGPoint p0, CGPoint p1, CGPoint p2, CGPoint p3, float t) {
	return ccp(
		bezier_val_for_t(p0.x, p1.x, p2.x, p3.x, t),
		bezier_val_for_t(p0.y, p1.y, p2.y, p3.y, t)
	);
}

float cubic_interp(float a, float b, float c1, float c2, float t) {
	CGPoint bez = bezier_point_for_t(ccp(0,0), ccp(0.25,c1), ccp(0.75,c2), ccp(1,1), t);
	return bez.y * (b-a) + a;
}

float low_filter(float value, float min) {
	return ABS(value) < ABS(min) ? 0 : value;
}

float running_avg(float avg, float val, float ct) {
	avg -= avg / ct;
	avg += val / ct;
	return avg;
}

CGRect scale_rect(CGRect tar, float scf) {
	return CGRectMake(tar.origin.x, tar.origin.x, tar.size.width*scf, tar.size.height*scf);
}

float y_for_point_of_2pt_line(CGPoint pt1, CGPoint pt2, float x) {
	//(y - y1)/(x - x1) = m
	float m = (pt1.y - pt2.y) / (pt1.x - pt2.x);
	//y - mx = b
	float b = pt1.y - m * pt1.x;
	return m * x + b;
}

BOOL ccnode_is_visible(CCNode* tar) {
	CCNode *itr = tar;
	while (itr != NULL) {
		if (!itr.visible) {
			return NO;
		}
		itr = itr.parent;
	}
	return YES;
}

@end

*/
