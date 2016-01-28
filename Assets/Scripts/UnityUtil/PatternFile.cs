using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ProtoBuf;
using System;

public class RPattern {
	public static string TEST_1 = "patterns/water/testpattern_1";
	
	public static string GENPATTERN_1 = "patterns/water/genpattern_1";
	public static string GENPATTERN_2 = "patterns/water/genpattern_2";
}

[ProtoContract]
public class PatternFile {	
	
	[ProtoMember(1)] public List<PatternEntry2Pt> _2pt_entries = new List<PatternEntry2Pt>();
	[ProtoMember(2)] public List<PatternEntry1Pt> _1pt_entries = new List<PatternEntry1Pt>();
	[ProtoMember(3)] public List<PatternEntryDirectional> _directional_entries = new List<PatternEntryDirectional>();
	[ProtoMember(4)] public List<PatternEntryLine> _line_entries = new List<PatternEntryLine>();
	[ProtoMember(5)] public List<PatternEntryPolygon> _polygon_entries = new List<PatternEntryPolygon>();
	[ProtoMember(5)] public float _section_height;
	[ProtoMember(6)] public float _spacing_bottom;
	
	public static PatternFile cons_from_string(string file_text) {
		PatternFile rtv = new PatternFile();
		
		JSONObject root = JSONObject.Parse(file_text);
		rtv._spacing_bottom = (float)root.GetNumber("spacing_bottom");
		JSONArray entries = root.GetArray("entries");
		
		for (int i = 0; i < entries.Length; i++) {
			JSONObject itr = entries[i].Obj;
			string type = itr.GetString("type");
			string val = itr.GetString("val");
			
			if (type == "1pt") {
				JSONObject start_obj = itr.GetObject("start");
				Vector2 start = new Vector2((float)start_obj.GetNumber("x"),(float)start_obj.GetNumber("y"));
				
				rtv._1pt_entries.Add(new PatternEntry1Pt() {
					_val = val,
					_start = start
				});
				
			} else if (type == "2pt") {
				JSONObject pt1_obj = itr.GetObject("pt1");
				Vector2 pt1 = new Vector2((float)pt1_obj.GetNumber("x"),(float)pt1_obj.GetNumber("y"));
				JSONObject pt2_obj = itr.GetObject("pt2");
				Vector2 pt2 = new Vector2((float)pt2_obj.GetNumber("x"),(float)pt2_obj.GetNumber("y"));
				JSONObject start_obj = itr.GetObject("start");
				Vector2 start = new Vector2((float)start_obj.GetNumber("x"),(float)start_obj.GetNumber("y"));
				
				float speed = (float)itr.GetNumber("speed");
				
				rtv._2pt_entries.Add(new PatternEntry2Pt() {
					_val = val,
					_pt1 = pt1,
					_pt2 = pt2,
					_start = start,
					_speed = speed
				});
			
			} else if (type == "directional") {
				JSONObject start_obj = itr.GetObject("start");
				Vector2 start = new Vector2((float)start_obj.GetNumber("x"),(float)start_obj.GetNumber("y"));
				JSONObject dir = itr.GetObject("dir");
				Vector2 dir_vec = new Vector2((float)dir.GetNumber("x"),(float)dir.GetNumber("y"));
				rtv._directional_entries.Add(new PatternEntryDirectional() {
					_val = val,
					_start = start,
					_dir = dir_vec
				});
			
			} else if (type == "line") {
				JSONObject pt1_obj = itr.GetObject("pt1");
				Vector2 pt1 = new Vector2((float)pt1_obj.GetNumber("x"),(float)pt1_obj.GetNumber("y"));
				JSONObject pt2_obj = itr.GetObject("pt2");
				Vector2 pt2 = new Vector2((float)pt2_obj.GetNumber("x"),(float)pt2_obj.GetNumber("y"));
				rtv._line_entries.Add(new PatternEntryLine() {
					_val = val,
					_pt1 = pt1,
					_pt2 = pt2
				});
				
			} else if (type == "polygon") {
				Vector2 pt0, pt1, pt2, pt3;
				{ 
					JSONObject pt_obj = itr.GetObject("pt0");
					pt0 = new Vector2((float)pt_obj.GetNumber("x"),(float)pt_obj.GetNumber("y")); 
				}
				{ 
					JSONObject pt_obj = itr.GetObject("pt1");
					pt1 = new Vector2((float)pt_obj.GetNumber("x"),(float)pt_obj.GetNumber("y")); 
				}
				{ 
					JSONObject pt_obj = itr.GetObject("pt2");
					pt2 = new Vector2((float)pt_obj.GetNumber("x"),(float)pt_obj.GetNumber("y")); 
				}
				{ 
					JSONObject pt_obj = itr.GetObject("pt3");
					pt3 = new Vector2((float)pt_obj.GetNumber("x"),(float)pt_obj.GetNumber("y")); 
				}
				rtv._polygon_entries.Add(new PatternEntryPolygon() {
					_val = val,
					_pt0 = pt0,
					_pt1 = pt1,
					_pt2 = pt2,
					_pt3 = pt3
				});
			}
		}
		
		rtv.postprocess();
		
		return rtv;
	}
	
	private void postprocess() {
		SPRange y_range = new SPRange() {
			_min = float.MaxValue,
			_max = float.MinValue
		};
		for (int i = 0; i < this._1pt_entries.Count; i++) {
			this._1pt_entries[i].postprocess(ref y_range);
		}
		for (int i = 0; i < this._2pt_entries.Count; i++) {
			this._2pt_entries[i].postprocess(ref y_range);
		}
		for (int i = 0; i < this._directional_entries.Count; i++) {
			this._directional_entries[i].postprocess(ref y_range);
		}
		for (int i = 0; i < this._line_entries.Count; i++) {
			this._line_entries[i].postprocess(ref y_range);
		}
		for (int i = 0; i < this._polygon_entries.Count; i++) {
			this._polygon_entries[i].postprocess(ref y_range);
		}
		_section_height = y_range._max - y_range._min;
	}
	public static void cmp_yrange_point(ref SPRange y_range, Vector2 point) {
		y_range._min = Mathf.Min(y_range._min,point.y);
		y_range._max = Mathf.Max(y_range._max,point.y);
	}
	
}

public interface PatternEntry<T> {
	T copy_applied_offset(Vector2 offset);
	void postprocess(ref SPRange y_range);
	
}

[ProtoContract]
public class PatternEntryPolygon : PatternEntry<PatternEntryPolygon> {
	[ProtoMember(1)] public string _val;
	[ProtoMember(2)] public Vector2 _pt0;
	[ProtoMember(3)] public Vector2 _pt1;
	[ProtoMember(4)] public Vector2 _pt2;
	[ProtoMember(5)] public Vector2 _pt3;
	
	public PatternEntryPolygon copy_applied_offset(Vector2 offset) {
		return new PatternEntryPolygon() {
			_val = _val,
			_pt0 = SPUtil.vec_add(_pt0,offset),
			_pt1 = SPUtil.vec_add(_pt1,offset),
			_pt2 = SPUtil.vec_add(_pt2,offset),
			_pt3 = SPUtil.vec_add(_pt3,offset)
		};
	}
	
	public void postprocess(ref SPRange y_range) {
		PatternFile.cmp_yrange_point(ref y_range,this._pt0);
		PatternFile.cmp_yrange_point(ref y_range,this._pt1);
		PatternFile.cmp_yrange_point(ref y_range,this._pt2);
		PatternFile.cmp_yrange_point(ref y_range,this._pt3);
	}
}

[ProtoContract]
public class PatternEntryLine : PatternEntry<PatternEntryLine> {
	[ProtoMember(1)] public string _val;
	[ProtoMember(2)] public Vector2 _pt1;
	[ProtoMember(3)] public Vector2 _pt2;
	
	public PatternEntryLine copy_applied_offset(Vector2 offset) {
		return new PatternEntryLine() {
			_val = _val,
			_pt1 = SPUtil.vec_add(_pt1,offset),
			_pt2 = SPUtil.vec_add(_pt2,offset)
		};
	}
	public void postprocess(ref SPRange y_range) {
		PatternFile.cmp_yrange_point(ref y_range,this._pt1);
		PatternFile.cmp_yrange_point(ref y_range,this._pt2);
	}
}

[ProtoContract]
public class PatternEntryDirectional : PatternEntry<PatternEntryDirectional>  {
	[ProtoMember(1)] public string _val;
	[ProtoMember(2)] public Vector2 _start;
	[ProtoMember(3)] public Vector2 _dir;
	
	public PatternEntryDirectional copy_applied_offset(Vector2 offset) {
		return new PatternEntryDirectional() {
			_val = _val,
			_start = SPUtil.vec_add(_start,offset),
			_dir = _dir
		};
	}
	public void postprocess(ref SPRange y_range) {
		PatternFile.cmp_yrange_point(ref y_range,this._start);
	}
}

[ProtoContract]
public class PatternEntry2Pt : PatternEntry<PatternEntry2Pt> {
	[ProtoMember(1)] public string _val;
	[ProtoMember(2)] public Vector2 _pt1;
	[ProtoMember(3)] public Vector2 _pt2;
	[ProtoMember(4)] public Vector2 _start;
	[ProtoMember(5)] public float _speed;
	
	public PatternEntry2Pt copy_applied_offset(Vector2 offset) {
		return new PatternEntry2Pt() {
			_val = _val,
			_pt1 = SPUtil.vec_add(_pt1,offset),
			_pt2 = SPUtil.vec_add(_pt2,offset),
			_start = SPUtil.vec_add(_start,offset),
			_speed = _speed
		};
	}
	public void postprocess(ref SPRange y_range) {
		PatternFile.cmp_yrange_point(ref y_range,this._start);
		PatternFile.cmp_yrange_point(ref y_range,this._pt1);
		PatternFile.cmp_yrange_point(ref y_range,this._pt2);
	}
}

[ProtoContract]
public class PatternEntry1Pt : PatternEntry<PatternEntry1Pt> {
	[ProtoMember(1)] public string _val;
	[ProtoMember(2)] public Vector2 _start;
	
	public PatternEntry1Pt copy_applied_offset(Vector2 offset) {
		return new PatternEntry1Pt() {
			_val = _val,
			_start = SPUtil.vec_add(_start,offset)
		};
	}
	public void postprocess(ref SPRange y_range) {
		PatternFile.cmp_yrange_point(ref y_range,this._start);
	}
}
