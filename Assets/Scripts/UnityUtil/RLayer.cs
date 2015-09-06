using UnityEngine;
using System.Collections;

public class RLayer {

	public static string REFLECTION_OBJECTS_1 = "ReflectionObjects1";
	public static string REFLECTION_OBJECTS_2 = "ReflectionObjects2";
	public static string REFLECTION_OBJECTS_3 = "ReflectionObjects3";
	public static string REFLECTION_OBJECTS_DOCKS = "ReflectionObjects4";
	public static string REFLECTIONS = "Reflections";
	public static string SPRITER_NODE = "SpriterNode";
	public static string UNDERWATER_ELEMENTS = "UnderwaterElements";

	public static int get_layer(string key) {
		return LayerMask.NameToLayer(key);
	}

}
