using UnityEngine;
using System.Collections.Generic;

public class RTex {
	public static string BG_SPRITESHEET_1 = "background/bg_spritesheet_1";
}

public class TextureResource : Object {

	private class TextureResourceValue {
		public Texture _tex;
		public Dictionary<string,Material> _shaderkey_to_material = new Dictionary<string, Material>();
	}

	private Dictionary<string,TextureResourceValue> _key_to_resourcevalue;

	public static TextureResource cons() {
		return (new TextureResource()).i_cons();
	}

	private TextureResource i_cons() {
		_key_to_resourcevalue = new Dictionary<string, TextureResourceValue>() {
			{RTex.BG_SPRITESHEET_1, cons_texture_resource_value(RTex.BG_SPRITESHEET_1)}
		};

		return this;
	}

	private Texture load_texture_from_resources(string path) {
		path = System.IO.Path.Combine(Application.streamingAssetsPath, path+".png");
		Texture2D rtv = new Texture2D(0,0);
		rtv.LoadImage(SPUtil.streaming_asset_load(path));
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
