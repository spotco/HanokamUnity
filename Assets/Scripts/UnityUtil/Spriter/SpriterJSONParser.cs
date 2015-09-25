using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;

[ProtoContract]
public class SpriterJSONFrame {
	[ProtoMember(1)] public Rect _val;

	public static SpriterJSONFrame cons_from_rect(Rect rect) {
		SpriterJSONFrame rtv = new SpriterJSONFrame();
		rtv._val = rect;
		return rtv;
	}
}

[ProtoContract]
public class SpriterJSONParser {
	[ProtoMember(1)] public Dictionary<string,SpriterJSONFrame> _frames = new Dictionary<string, SpriterJSONFrame>();
	[ProtoMember(2)] public string _texkey;
	[ProtoMember(3)] public string _filepath;

	public static SpriterJSONParser cons_from_texture_and_file(string texkey, string filepath) {
		return (new SpriterJSONParser()).i_cons_from_texture_and_file(texkey,filepath);
	}

	private SpriterJSONParser i_cons_from_texture_and_file(string texkey, string filepath) {
		_texkey = texkey;
		_filepath = filepath;
		this.parse_file(filepath);
		return this;
	}

	public string texkey() { return _texkey; }
	public string filepath() { return _filepath; }

	private SpriterJSONParser parse_file(string filepath) {
		_frames = new Dictionary<string, SpriterJSONFrame>();

		string jsonData = System.Text.Encoding.Default.GetString(
			SPUtil.streaming_asset_load(
				System.IO.Path.Combine(Application.streamingAssetsPath, filepath+".json"))
		);

		JSONObject root = JSONObject.Parse(jsonData);
		JSONObject frames = root.GetObject("frames");

		foreach (KeyValuePair<string,JSONValue> kvp in frames) {
			string key = kvp.Key;
			JSONObject frame = frames.GetObject(key).GetObject("frame");
			JSONObject sprite_source_size = frames.GetObject(key).GetObject("spriteSourceSize");
			JSONObject source_size = frames.GetObject(key).GetObject("sourceSize");

			//hack in TexturePacker trim model, this won't actually work if it clips another spritesheet image
			Rect rect = new Rect(
				(float)(frame.GetNumber("x")-sprite_source_size.GetNumber("x")),
			    (float)(frame.GetNumber("y")-sprite_source_size.GetNumber("y")),
			    (float)(source_size.GetNumber("w")),
			    (float)(source_size.GetNumber("h"))
			);
			_frames[key] = SpriterJSONFrame.cons_from_rect(rect);
		}

		return null;
	}

	public Rect rect_for_frame(string key) {
		return _frames.ContainsKey(key)?_frames[key]._val:new Rect(0,0,0,0);
	}
}
