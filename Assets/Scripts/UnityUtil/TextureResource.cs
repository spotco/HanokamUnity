﻿using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class RTex {
	public static string BG_SPRITESHEET_1 = "background/bg_spritesheet_1";
	public static string SPRITER_OLDMAN = "character/oldman/Oldman";
	public static string SPRITER_FISHGIRL = "character/Fishgirl/Fishgirl";
	public static string SPRITER_HANOKA = "character/hanoka/hanoka_player";
	public static string SPRITER_HANOKA_SWORD = "character/hanoka/hanoka_sword";
	public static string SPRITER_HANOKA_BOW = "character/hanoka/hanoka_bow";
	public static string SPRITER_HANOKA_REDGARB = "character/hanoka/hanoka_armors/hanoka_player_redgarb";

	public static string BLANK = "misc/blank";

	public static string SPRITER_TEST_CHAR = "test/spriter_test";
}

public class TextureResource {

	public static TextureResource inst() { return GameMain._context._tex_resc; }

	private class TextureResourceValue {
		public Texture _tex;
		public Dictionary<string,Material> _shaderkey_to_material = new Dictionary<string, Material>();
	}

	private Dictionary<string,TextureResourceValue> _key_to_resourcevalue;

	public static TextureResource cons() {
		return (new TextureResource()).i_cons();
	}

	private TextureResource i_cons() {
		_key_to_resourcevalue = new Dictionary<string, TextureResourceValue>();

		FieldInfo[] fields = typeof(RTex).GetFields(BindingFlags.Public | BindingFlags.Static);
		foreach (FieldInfo itr in fields) {
			if (itr.FieldType == typeof(string)) {	
				string value = (string)itr.GetValue(null);
				_key_to_resourcevalue[value] = cons_texture_resource_value(value);
			}
		}

		return this;
	}

	private Texture load_texture_from_resources(string path) {
		path = System.IO.Path.Combine(Application.streamingAssetsPath, path+".png");
		Texture2D rtv = new Texture2D(0,0);
		rtv.LoadImage(SPUtil.streaming_asset_load(path));
		rtv.filterMode = FilterMode.Point;
		return rtv;
	}

	private TextureResourceValue cons_texture_resource_value(string path) {
		return new TextureResourceValue() {
			_tex = load_texture_from_resources(path)
		};
	}

	public Texture get_tex(string key) {
		return _key_to_resourcevalue[key]._tex;
	}

	public Material get_material_default(string texkey) {
		return get_material(texkey,RSha.DEFAULT);
	}

	public Material get_material(string texkey, string shaderkey) {
		TextureResourceValue tar = _key_to_resourcevalue[texkey];
		if (!tar._shaderkey_to_material.ContainsKey(shaderkey)) {
			tar._shaderkey_to_material[shaderkey] = new Material(ShaderResource.get_shader(shaderkey));
			tar._shaderkey_to_material[shaderkey].SetTexture("_MainTex",this.get_tex(texkey));
		}
		return tar._shaderkey_to_material[shaderkey];

	}

}
