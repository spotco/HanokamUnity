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
	[ProtoMember(3)] public float _section_height;
	[ProtoMember(4)] public float _spacing_bottom;
	
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
			this.cmp_yrange_point(ref y_range,this._1pt_entries[i]._start);
		}
		for (int i = 0; i < this._2pt_entries.Count; i++) {
			this.cmp_yrange_point(ref y_range,this._2pt_entries[i]._start);
			this.cmp_yrange_point(ref y_range,this._2pt_entries[i]._pt1);
			this.cmp_yrange_point(ref y_range,this._2pt_entries[i]._pt2);
		}
		_section_height = y_range._max - y_range._min;
	}
	private void cmp_yrange_point(ref SPRange y_range, Vector2 point) {
		y_range._min = Mathf.Min(y_range._min,point.y);
		y_range._max = Mathf.Max(y_range._max,point.y);
	}
	
}

[ProtoContract]
public class PatternEntry2Pt {
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
}

[ProtoContract]
public class PatternEntry1Pt {
	[ProtoMember(1)] public string _val;
	[ProtoMember(2)] public Vector2 _start;
	
	public PatternEntry1Pt copy_applied_offset(Vector2 offset) {
		return new PatternEntry1Pt() {
			_val = _val,
			_start = SPUtil.vec_add(_start,offset)
		};
	}
}
