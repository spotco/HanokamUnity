using UnityEngine;
using System.Collections;
using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;

public class ProtoBufSerializer {

	private static bool __has_added_unity_types = false;
	private static void lazy_init_unity_types() {
		if (__has_added_unity_types) return;
		__has_added_unity_types = true;
		RuntimeTypeModel.Default.Add(typeof(Vector2),false).Add("x","y");
		RuntimeTypeModel.Default.Add(typeof(Vector3),false).Add("x","y","z");
		RuntimeTypeModel.Default.Add(typeof(Rect),false).Add("x","y","width","height");
	}

	public static byte[] serialize_to_bytes<T>(T myData) {
		lazy_init_unity_types();
		MemoryStream stream = new MemoryStream();
		Serializer.Serialize<T>(stream, myData);
        byte [] bytes = stream.ToArray();
        stream.Close();
        return bytes;
    }

	public static T deserialize_from_bytes<T>(byte[] bytes) {
		lazy_init_unity_types();
		T myData = default(T);
		System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes,false);
		myData = Serializer.Deserialize<T>(stream);
        stream.Close();
        return myData;
    }

}
