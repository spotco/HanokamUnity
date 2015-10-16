using UnityEngine;
using System.Collections;

public class RShader {
	public static string DEFAULT = "Custom/SPSpriteDefault";
	public static string SURFACE_REFLECTION = "Custom/SurfaceReflection";
	public static string ALPHA_GRADIENT = "Custom/SPAlphaGradientSprite";
    public static string NOALPHA = "Custom/SPNoAlphaShader";

}

public class ShaderResource : Object {

	public static Shader get_shader(string key) {
		return Shader.Find(key);
	}

}
