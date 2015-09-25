#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEditor;
using System;
using System.Text;

[ProtoBuf.ProtoContract]
public class ExportTest {
	[ProtoBuf.ProtoMember(1)] public int test;
	[ProtoBuf.ProtoMember(2)] public Dictionary<int,Dictionary<string,Rect>> test2;
}

public class AssetExport {
	static byte[] _tmp;

	[MenuItem("SPEditorUtils/Write Streaming Assets To Resources")]
	public static void write_streaming_assets_to_resources() {
		if (GameMain._context == null) {
			Debug.LogError("_context null");
			return;
		}

		if (GameMain._context._file_cache != null) {
#if !UNITY_WEBPLAYER
			FileUtil.DeleteFileOrDirectory(CachedStreamingAssets.FILECACHE_PATH);
			
			System.IO.File.WriteAllBytes(CachedStreamingAssets.FILECACHE_PATH,
				ProtoBufSerializer.serialize_to_bytes<FileCache>(
					FileCache.inst()));
			SPUtil.logf("Wrote to %s",CachedStreamingAssets.FILECACHE_PATH);
#endif



		} else {
			Debug.LogError("_file_cache null");
		}
	}

	[MenuItem("SPEditorUtils/Debug Read Resources")]
	public static void debug_read_resources() {
		FileCache tmp_filecache = ProtoBufSerializer.deserialize_from_bytes<FileCache>(
				Resources.Load<TextAsset>(
					CachedStreamingAssets.path_to_loadpath(CachedStreamingAssets.FILECACHE_PATH)
				).bytes
		);
		Debug.Log(tmp_filecache.get_texrect(RTex.BG_SKY_SPRITESHEET,"bg_sky_arcs.png"));
	}

}
#endif

public class CachedStreamingAssets {
	public const string FILECACHE_PATH = "Assets/Resources/CachedStreamingAssets/filecache.bytes";

	public static string path_to_loadpath(string input) {
		return input.Replace("Assets/Resources/","").Replace(".bytes","");
	}
}