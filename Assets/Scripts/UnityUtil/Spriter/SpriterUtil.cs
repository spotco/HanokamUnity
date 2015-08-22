using UnityEngine;
using System.Collections;

public class SpriterUtil {

	private static float sbezier_val_for_t(float p0, float p1, float p2, float p3, float t) {
		float cp0 = (1 - t)*(1 - t)*(1 - t);
		float cp1 = 3 * t * (1-t)*(1-t);
		float cp2 = 3 * t * t * (1 - t);
		float cp3 = t * t * t;
		return cp0 * p0 + cp1 * p1 + cp2 * p2 + cp3 * p3;
	}

	private const int TABLE_SCUBIC_SIZE = 100;
	private static float[] _table_scubic_point_for_t = new float[TABLE_SCUBIC_SIZE];
	private static bool _table_calculated = false;
	public static void calc_table_scubic_point_for_t() {
		if (_table_calculated) {
			return;
		}
		_table_calculated = true;
		float t = 0;
		float itr_add = 1.0f / TABLE_SCUBIC_SIZE;
		for (int i = 0; i < TABLE_SCUBIC_SIZE; i++) {
			_table_scubic_point_for_t[i] = sbezier_point_for_t(new Vector2(0,0), new Vector2(0.25f,0), new Vector2(0.75f,1), new Vector2(1,1), t).y;
			t += itr_add;
		}
	}
	
	private static float get_table_scubic_point_for_t(double t) {

		int tar = (int)(t*TABLE_SCUBIC_SIZE);

		if (tar >= TABLE_SCUBIC_SIZE) tar = TABLE_SCUBIC_SIZE-1;
		if (tar < 0) tar = 0;
		return _table_scubic_point_for_t[tar];
	}
	
	public static Vector2 sbezier_point_for_t(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
		return new Vector2(
			sbezier_val_for_t(p0.x, p1.x, p2.x, p3.x, t),
			sbezier_val_for_t(p0.y, p1.y, p2.y, p3.y, t)
		);
	}
	
	public static float scubic_interp(float a, float b, float t) {
		return get_table_scubic_point_for_t(t) * (b-a) + a;
	}

	private static float fmod(float a, float b) { return a % b; }

	private static float sshortest_angle(float src, float dest) {
		float shortest_angle=fmod((fmod((dest - src) , 360) + 540), 360) - 180;
		return shortest_angle;
	}
	
	public static float scubic_angular_interp(float src, float dest, float t) {
		t = scubic_interp(0, 1, t);
		return src + sshortest_angle(src, dest) * t;
	}

}
