using UnityEngine;
using System.Collections;

public class FileCache : Object {

	public static FileCache cons() {
		return (new FileCache()).i_cons();
	}
	
	private FileCache i_cons() {

		return this;
	}

	private string load_plist_from_path(string path) {
		path = System.IO.Path.Combine(Application.streamingAssetsPath, path+".plist");
		return System.Text.Encoding.Default.GetString(SPUtil.streaming_asset_load(path));
	}

}
