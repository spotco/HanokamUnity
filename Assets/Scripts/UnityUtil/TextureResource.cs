﻿using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class RTex {
	public static string BG_SPRITESHEET_1 = "background/bg_spritesheet_1";
	public static string BG_UNDERWATER_SPRITESHEET = "background/underwater_spritesheet";
	
	public static string BG_NVILLAGE_SPRITESHEET = "background/nvillage/nvillage_spritesheet";
	
	public static string BG_NSKY1_BG_OBJS = "background/nsky/nsky1_bg_objs_ss";
	public static string BG_NSKY1_BGISLANDS = "background/nsky/nsky1_bgislands_ss";
	public static string BG_NSKY1_BGCLOUDS = "background/nsky/nsky1_bgclouds_tex";
	
	public static string BG_TILE_SKY = "background/bg_test_tile_sky";
	public static string BG_TILE_WATER = "background/bg_test_tile_water";
	public static string BG_WATER_ELEMENT_FADE = "background/bg_water_element_fade";
	public static string BG_UNDERWATER_SURFACE_GRADIENT = "background/bg_underwater_surface_gradient";
	
	public static string HUD_SPRITESHEET = "hud/hud_spritesheet";
	public static string HUD_LETTERBOX_BORDER = "hud/hud_letterbox_border_tex";
	
	public static string BG_WATER_TOP_BELOWLINE = "background/waterline_tex/bg_water_top_belowline";
	public static string BG_WATER_TOP_WATERLINE = "background/waterline_tex/bg_water_top_waterline";
	public static string BG_WATER_TOP_WATERLINEGRAD = "background/waterline_tex/bg_water_top_waterlinegrad";
	public static string BG_WATER_BOTTOM_SURFACEGRAD = "background/waterline_tex/bg_water_bottom_surfacegrad";

	public static string BG_NUNDERWATER_BG_1 = "background/nunderwater/nunderwater_bg";
	public static string BG_NUNDERWATER_BG_2 = "background/nunderwater/nunderwater_bg_2";
	public static string BG_NUNDERWATER_TEMPLE = "background/nunderwater/nunderwater_temple";
	public static string BG_NUNDERWATER_FADE = "background/nunderwater/nunderwater_fade";
	
	public static string BG_NUNDERWATER_FISH_ANIM = "background/nunderwater/nunderwater_fish_anim";
	public static string BG_NUNDERWATER_FISH_TEST = "background/nunderwater/fish_test_1";

	public static string SPRITER_OLDMAN = "character/oldman/Oldman";
	public static string SPRITER_FISHGIRL = "character/Fishgirl/Fishgirl";
	public static string SPRITER_DOG = "character/dog/dog";
	public static string SPRITER_FISHMOM = "character/Fishmom/Fishmom";
	public static string SPRITER_HANOKA = "character/hanoka/hanoka_player";
	public static string SPRITER_HANOKA_SWORD = "character/hanoka/hanoka_sword";
	public static string SPRITER_HANOKA_BOW = "character/hanoka/hanoka_bow";
	public static string SPRITER_HANOKA_REDGARB = "character/hanoka/hanoka_armors/hanoka_player_redgarb";
	public static string SPRITER_BOY = "character/Boy/boy";
	public static string SPRITER_SHOPKEEP = "character/shopkeep/shopkeep";

	public static string HANOKA_EFFECTS = "effects/effects_hanoka_ss";
	public static string HANOKA_EFFECTS_WATER = "effects/hanoka_fx_water";
	
	public static string GROUND_EFFECTS = "effects/ground_fx";
	
	public static string BUBBLE_EFFECTS = "effects/bubble_ss";
	
	public static string SHOP_SHOPBG = "shop/shopbg";
	public static string SHOP_SHOPUI = "shop/shopui";
	
	public static string DIALOG_UI = "dialog/dialog_ui_ss";
	
	public static string FLAGANIM = "hud/flaganim";
	
	public static string PARTICLES_SPRITESHEET = "effects/particles_spritesheet";
	public static string FX_SPLASH = "effects/ss_splash_fx";
	
	public static string ENEMY_PUFFER = "monster/enemy_puffer/puffer_enemy_ss";
	public static string ENEMY_SPIKE = "monster/enemy_spike/spike_enemy_ss";
	
	public static string ENEMY_LASER_CRAB = "monster/enemy_laser_crab/laser_crab_ss";
	
	public static string ENEMY_LASER = "monster/enemy_laser/enemy_laser_ss";
	public static string ENEMY_LASER_BODY = "monster/enemy_laser/enemy_laser_body_ss";
	
	public static string ENEMY_BLOCK_BODY_FILL = "monster/block/block_body_fill";
	public static string ENEMY_BLOCK_TOP_SECTION = "monster/block/block_top_section";
	
	public static string ENEMY_EFFECTS = "effects/effects_enemy_ss";

	public static string BLANK = "misc/blank";
	
	public static string DIALOGUE_HEADICONS = "dialog/headicons_ss";
	public static string DIALOGUE_FONT = "fonts/1hoonwhayang";
	
	public static string SPRITER_TEST_CHAR = "test/spriter_test";
}

public class TextureResource {

	public static TextureResource inst() { return GameMain._context._tex_resc; }

	public class TextureResourceValue {
		public Texture _tex;
		public Dictionary<string,Material> _shaderkey_to_material = new Dictionary<string, Material>();
	}

	public Dictionary<string,TextureResourceValue> _key_to_resourcevalue;

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

	private Texture load_texture_from_streamingassets(string path) {
		Debug.LogWarning("texture from streaming:"+path);
		path = System.IO.Path.Combine(Application.streamingAssetsPath, path+".png");
		Texture2D rtv = new Texture2D(0,0,TextureFormat.ARGB32,false);
		rtv.LoadImage(SPUtil.streaming_asset_load(path));
		rtv.filterMode = FilterMode.Point;
		return rtv;
	}

	private TextureResourceValue cons_texture_resource_value(string texkey) {
		Texture tex = 
		//null;
		Resources.Load<Texture2D>(CachedStreamingAssets.texture_key_to_resource_path(texkey));
		if (tex == null) {
			tex = this.load_texture_from_streamingassets(texkey);
		}
		tex.filterMode = FilterMode.Point;
		return new TextureResourceValue() {
			_tex = tex
		};
	}

	public Texture get_tex(string key) {
		return _key_to_resourcevalue[key]._tex;
	}

	public Material get_material_default(string texkey) {
		return get_material(texkey,RShader.DEFAULT);
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
