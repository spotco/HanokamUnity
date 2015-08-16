using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class FileCache : Object {

	public static FileCache cons() {
		return (new FileCache()).i_cons();
	}

	private Dictionary<string,Dictionary<string,Rect>> _texkey_to_rectkey_to_rect = new Dictionary<string,Dictionary<string,Rect>>();

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

}
