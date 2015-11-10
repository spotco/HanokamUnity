using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ProtoBuf;

public class RFnt {
	public static string DIALOGUE_FONT = "fonts/1hoonwhayang";
}

[ProtoContract]
public class FntFile {

	public static FntFile cons_from_string(string file) {
		FntFile rtv = new FntFile();
		Debug.Log(file);
		return rtv;
	}

	[ProtoMember(1)] public string face;
	[ProtoMember(2)] public float size;
	[ProtoMember(3)] public int italic;
	[ProtoMember(4)] public string charset;
	[ProtoMember(5)] public int unicode;
	[ProtoMember(6)] public int stretchH;
	[ProtoMember(7)] public int smooth;
	[ProtoMember(8)] public int aa;
	[ProtoMember(9)] public Vector4 padding;
	[ProtoMember(10)] public Vector2 spacing;
	
	[ProtoContract]
	public class Common {
		[ProtoMember(1)] public float lineHeight;
		[ProtoMember(2)] public float Base;
		[ProtoMember(3)] public float scaleW;
		[ProtoMember(4)] public float scaleH;
		[ProtoMember(5)] public int pages;
		[ProtoMember(6)] public int packed;
	}
	
	[ProtoMember(11)] public Common common;
	
	[ProtoContract]
	public class Char {
		[ProtoMember(1)] public int id;
		[ProtoMember(2)] public float x;
		[ProtoMember(3)] public float y;
		[ProtoMember(4)] public float width;
		[ProtoMember(5)] public float height;
		[ProtoMember(6)] public float xoffset;
		[ProtoMember(7)] public float yoffset;
		[ProtoMember(8)] public float xadvance;
		[ProtoMember(9)] public int page;
		[ProtoMember(10)] public int chnl;
		[ProtoMember(11)] public string letter;
	}
	
	[ProtoMember(12)] public List<Char> chars;
}