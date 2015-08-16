using UnityEngine;
using System.Collections;

public class RSha {
	public static string DEFAULT = "Custom/SPSpriteDefault";
}

public class ShaderResource : Object {

	public static Shader get_shader(string key) {
		return Shader.Find(key);
	}

}
