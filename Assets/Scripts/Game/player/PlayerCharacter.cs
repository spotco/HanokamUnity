using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacter : SPGameUpdateable {
	
	public static PlayerCharacter cons(GameEngineScene g) {
		return (new PlayerCharacter()).i_cons(g);
	}
	
	private SPNode _root;
	public SpriterNode _img;
	
	public PlayerCharacter i_cons(GameEngineScene g) {
		_root = SPNode.cons_node();
		_root.set_name("PlayerCharacter");
		
		SpriterData data = SpriterData.cons_data_from_spritesheetreaders(new List<SpriteSheetReader> { 
			SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA,RTex.SPRITER_HANOKA),
			SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA_BOW,RTex.SPRITER_HANOKA_BOW),
			SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA_SWORD,RTex.SPRITER_HANOKA_SWORD)
		}, RTex.SPRITER_HANOKA);
		_img = SpriterNode.cons_spriternode_from_data(data);
		_img.p_play_anim("Idle",true);
		_img.set_manual_sort_z_order(GameAnchorZ.Player_Ground);
		_img.set_name("_img");
		_root.add_child(_img);
		
		return this;
	}
	
	public float _u_x { get { return _root._u_x; } set { this.set_u_pos(value,this._u_y); } }
	public float _u_y { get { return _root._u_y; } set { this.set_u_pos(this._u_x,value); } }
	public PlayerCharacter set_u_pos(float x, float y) {
		_root.set_u_pos(x,y);
		return this;
	}
	
	
	
	public void i_update(GameEngineScene g) {
	
	}	
}

public struct PlayerCharacterAnims {
	public static string IDLE = "Idle";
	public static string RUN = "Run";
	public static string WALK = "Walk";
	public static string PREPDIVE = "Prep Dive";
	public static string DIVE = "Dive";
	public static string SPIN = "Spin";
	public static string SWIM = "Swim";
	public static string SWIMHURT = "Swim Hurt";
	public static string INAIRIDLE = "In Air Idle";
	public static string BOWAIM = "Bow Aim";
	public static string BOWHOLD = "Bow Hold";
	public static string BOWFIRE = "Bow Fire";
	public static string SWORDPLANT = "Sword Plant";
	public static string DASH = "Dash";
	public static string DASHSLASH = "Dash Slash";
	public static string INAIRHURT = "In Air Hurt";
	public static string FALL = "Fall";
}
