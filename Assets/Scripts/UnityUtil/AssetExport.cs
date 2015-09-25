#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditorInternal;
using UnityEditor;

public class AssetExport {

	[MenuItem("SPEditorUtils/Write Streaming Assets To Resources")]
	public static void write_streaming_assets_to_resources() {
		//ProtoBufSerializer.serialize_to_bytes(GameMain._context._file_cache);
	}

}
#endif
