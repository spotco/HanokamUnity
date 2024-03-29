﻿using UnityEngine;
using System.Collections;

public class RShader {
	public static string DEFAULT = "Custom/SPSpriteDefault";
	public static string SURFACE_REFLECTION = "Custom/SurfaceReflection";
	public static string ALPHA_GRADIENT = "Custom/SPAlphaGradientSprite";
    public static string NOALPHA = "Custom/SPNoAlphaShader";
	public static string SPTEXTCHARACTER = "Custom/SPTextCharacter";
	public static string UNDERWATER_VIEW_SURFACE = "Custom/UnderWaterViewSurface";
	public static string LASER_COLOR_SHADER = "Custom/LaserColorShader";
}

public class ShaderResource : Object {

	public static Shader get_shader(string key) {
		return Shader.Find(key);
	}

}
