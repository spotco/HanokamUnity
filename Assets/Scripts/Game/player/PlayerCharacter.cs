using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacter : SPGameUpdateable {
	
	public static PlayerCharacter cons(GameEngineScene g) {
		return (new PlayerCharacter()).i_cons(g);
	}
	
	private SPNode _root;
	private SpriterNode _img;
	
	public PlayerCharacter i_cons(GameEngineScene g) {
		_root = SPNode.cons_node();
		_root.set_name("PlayerCharacter");
		
		SpriterData data = SpriterData.cons_data_from_spritesheetreaders(new List<SpriterJSONParser> { 
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

	public PlayerCharacter set_manual_sort_z_order(int val) { _img.set_manual_sort_z_order(val); return this; }
	public float _u_x { get { return _root._u_x; } set { this.set_u_pos(value,this._u_y); } }
	public float _u_y { get { return _root._u_y; } set { this.set_u_pos(this._u_x,value); } }
	public PlayerCharacter set_u_pos(float x, float y) { _root.set_u_pos(x,y); return this; }
	public PlayerCharacter set_scale_x(float val) { _img.set_img_scale_x(val); return this; }
	public float rotation() { return _img._rendered_img.rotation(); }
	public PlayerCharacter set_rotation(float deg) { _img._rendered_img.set_rotation(deg); return this; }
	public Vector2 get_center() {
		return new Vector2(_root._u_x,_root._u_y+128);
	}
	
	public PlayerCharacter play_anim(string anim, bool repeat = true) { 
		if (anim == PlayerCharacterAnims.SWIM_SLOW) {
			_img.set_anim_playback_speed_mult(0.2f);
			anim = PlayerCharacterAnims.SWIM;
		} else {
			_img.set_anim_playback_speed_mult(1.0f);
		}
		_img.p_play_anim(anim, repeat); 
		return this; 
	}
	
	public void i_update(GameEngineScene g) {
	
	}	
}

public class PlayerCharacterAnims {
	public static string IDLE = "Idle";
	public static string RUN = "Run";
	public static string WALK = "Walk";
	public static string PREPDIVE = "Prep Dive";
	public static string DIVE = "Dive";
	public static string SPIN = "Spin";
	
	public static string SWIM = "Swim";
	public static string SWIM_SLOW = "SwimSlow";
	
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

public class PlayerCharacterUtil {
	public static void move_in_bounds(PlayerCharacter player, float x, float y) {
		player.set_u_pos(Mathf.Clamp(x,SPUtil.get_horiz_world_bounds()._min,SPUtil.get_horiz_world_bounds()._max),y);
	}
	public static void rotate_to_rotation_for_vel(PlayerCharacter player, float vx, float vy, float fric) {
		float tar_rotation = SPUtil.dir_ang_deg(vx,vy) - 90;
		player.set_rotation(SPUtil.drpt(player.rotation(), player.rotation() + SPUtil.shortest_angle(player.rotation(),tar_rotation), fric));
	}
	
}
