using UnityEngine;
using System.Collections;

public class RSha {
	public static string DEFAULT = "UI/Default";
	//public static string DEFAULT = "Custom/SPSpriteDefault";
	//public static string ALPHA = "Custom/SPSpriteAlpha";

}

public class ShaderResource : Object {

	public static Shader get_shader(string key) {
		return Shader.Find(key);
	}

}
