using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ProtoBuf;

[ProtoContract]
public class FileCache {

	public static FileCache inst() { return GameMain._context._file_cache; }

	public static FileCache cons() {
		FileCache rtv = null;
		try {
			rtv = ProtoBufSerializer.deserialize_from_bytes<FileCache>(
				Resources.Load<TextAsset>(
					CachedStreamingAssets.path_to_loadpath(CachedStreamingAssets.FILECACHE_PATH)
				).bytes
			);
		} catch (System.Exception e) {
			Debug.LogError("filecache not loaded from resources:"+e.Message);
			rtv = (new FileCache());
		}
		return rtv.i_cons();
	}

	[ProtoMember(1)] private Dictionary<string,Dictionary<string,Rect>> _texkey_to_rectkey_to_rect = new Dictionary<string,Dictionary<string,Rect>>();

	private FileCache i_cons() {
		return this;
	}

	public Rect get_texrect(string texkey, string rectname) {
		if (!_texkey_to_rectkey_to_rect.ContainsKey(texkey)) add_plist_file_to_cache(texkey);
		if (_texkey_to_rectkey_to_rect[texkey].ContainsKey(rectname)) {
			return _texkey_to_rectkey_to_rect[texkey][rectname];
		} else {
			return new Rect();
		}
	}

	private void add_plist_file_to_cache(string texkey) {
		Debug.LogError("lookup from streaming");
		_texkey_to_rectkey_to_rect[texkey] = new Dictionary<string,Rect>();

		Dictionary<string,object> frames = (Dictionary<string,object>)((Dictionary<string,object>) PlistCS.Plist.readPlistSource(this.load_plist_from_path(texkey)))["frames"];
		foreach (string key in frames.Keys) {
			Dictionary<string,object> frame = (Dictionary<string,object>)frames[key];
			string texture_rect_string = (string)frame["textureRect"];
			
			MatchCollection matches = Regex.Matches(texture_rect_string,"([0-9]+)");
			
			List<int> coords = new List<int>();
			foreach (Match match in matches) {
				foreach (Capture capture in match.Captures) {
					coords.Add(System.Int32.Parse(capture.Value));
				}
			}
			_texkey_to_rectkey_to_rect[texkey][key] = new Rect(coords[0],coords[1],coords[2],coords[3]);
		}
	}

	private string load_plist_from_path(string key) {
		string path = System.IO.Path.Combine(Application.streamingAssetsPath, key+".plist");
		return System.Text.Encoding.Default.GetString(SPUtil.streaming_asset_load(path));
	}


	[ProtoMember(2)] private Dictionary<string,SpriterJSONParser> _key_to_spriter_json_parser = new Dictionary<string,SpriterJSONParser>();
	public SpriterJSONParser get_spriter_json_parser(string texkey, string filepath) {
		string cachekey = SpriterJSONParser.cachekey(texkey,filepath);
		if (!_key_to_spriter_json_parser.ContainsKey(cachekey)) {
			_key_to_spriter_json_parser[cachekey] = SpriterJSONParser._cons_from_texture_and_file(texkey,filepath);
		}
		return _key_to_spriter_json_parser[cachekey];
	}

	[ProtoMember(3)] private Dictionary<string,SpriterData> _key_to_spriter_data = new Dictionary<string, SpriterData>();
	public SpriterData get_spriter_data(List<SpriterJSONParser> sheetreaders,string key) {
		string cachekey = key;
		if (!_key_to_spriter_data.ContainsKey(cachekey)) {
			_key_to_spriter_data[cachekey] = SpriterData._cons_data_from_spritesheetreaders(sheetreaders,key);
		}
		return _key_to_spriter_data[cachekey];
	}


}
