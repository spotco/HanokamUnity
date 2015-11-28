using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ProtoBuf;
using System;

public class RPattern {
	public static string TEST_1 = "patterns/water/testpattern_1";
}

[ProtoContract]
public class PatternFile {	
	
	[ProtoMember(1)] public List<PatternEntry2Pt> _2pt_entries = new List<PatternEntry2Pt>();
	
	public static PatternFile cons_from_string(string file_text) {
		PatternFile rtv = new PatternFile();
		
		JSONObject root = JSONObject.Parse(file_text);
		JSONArray entries = root.GetArray("entries");
		
		for (int i = 0; i < entries.Length; i++) {
			JSONObject itr = entries[0].Obj;
			string type = itr.GetString("type");
			string val = itr.GetString("val");
			JSONObject pt1_obj = itr.GetObject("pt1");
			Vector2 pt1 = new Vector2((float)pt1_obj.GetNumber("x"),(float)pt1_obj.GetNumber("y"));
			JSONObject pt2_obj = itr.GetObject("pt2");
			Vector2 pt2 = new Vector2((float)pt2_obj.GetNumber("x"),(float)pt2_obj.GetNumber("y"));
			
			rtv._2pt_entries.Add(new PatternEntry2Pt() {
				_val = val,
				_pt1 = pt1,
				_pt2 = pt2
			});
		}		
		return rtv;
	}
}

[ProtoContract]
public class PatternEntry2Pt {
	[ProtoMember(1)] public string _val;
	[ProtoMember(2)] public Vector2 _pt1;
	[ProtoMember(3)] public Vector2 _pt2;
	
}
