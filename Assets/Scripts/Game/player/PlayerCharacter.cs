using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacter : SPGameUpdateable, SPHitPolyOwner, SPNodeHierarchyElement {
	
	public static PlayerCharacter cons(GameEngineScene g) {
		return (new PlayerCharacter()).i_cons(g);
	}
	
	private SPNode _root;
	private SpriterNode _img;

	private SPSprite _streak_left, _streak_right;
	private SPSpriteAnimator _streak_left_anim, _streak_right_anim;

	private SPSprite _trail;
	
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

		_streak_left = SPAlphaGradientSprite.cons_alphagradient_sprite(
			RTex.HANOKA_EFFECTS,
			FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"sword_plant_energy_000.png"), 
			new SPRange() { _min = 1, _max = 1 }, 
			new SPRange() { _min = 1, _max = 0 });
		_streak_left.set_anchor_point(0,0);
		_streak_left.set_manual_sort_z_order(GameAnchorZ.Player_FX);
		_streak_left.set_u_pos(24,-24);
		_root.add_child(_streak_left);

		_streak_left_anim = SPSpriteAnimator.cons(_streak_left)
			.add_anim("play",
				FileCache.inst().get_rects_list(
					RTex.HANOKA_EFFECTS,"sword_plant_energy_00%d.png",0,3),6).play_anim("play");


		_streak_right = SPAlphaGradientSprite.cons_alphagradient_sprite(
			RTex.HANOKA_EFFECTS,
			FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"sword_plant_energy_000.png"), 
			new SPRange() { _min = 1, _max = 1 }, 
			new SPRange() { _min = 1, _max = 0 });
		_streak_right.set_anchor_point(0,0);
		_streak_right.set_manual_sort_z_order(GameAnchorZ.Player_FX);
		_streak_right.set_u_pos(-24,-24);
		_streak_right.set_scale_x(-1);
		_root.add_child(_streak_right);

		_streak_right_anim = SPSpriteAnimator.cons(_streak_right)
			.add_anim("play",
				FileCache.inst().get_rects_list(
					RTex.HANOKA_EFFECTS,"sword_plant_energy_00%d.png",0,3),7).play_anim("play");

		_trail = SPSprite.cons_sprite_texkey_texrect(RTex.HANOKA_EFFECTS,FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS,"sword_stab-fall-air.png"));
		_trail.set_anchor_point(0.5f,0);
		_trail.set_manual_sort_z_order(GameAnchorZ.Player_FX);
		_root.add_child(_trail);

		this.set_streak_enabled(false);
		this.set_trail_enabled_and_rotation(false);
		
		return this;
	}

	public void set_streak_enabled(bool val) {
		_streak_right.set_enabled(val);
		_streak_left.set_enabled(val);
	}
	public void set_trail_enabled_and_rotation(bool val, float angle=0) {
		_trail.set_enabled(val);
		_trail.set_rotation(angle);
	}
	
	public void add_to_parent(SPNode parent) {
		parent.add_child(_root);
	}

	public PlayerCharacter set_manual_sort_z_order(int val) { _img.set_manual_sort_z_order(val); return this; }
	public float _u_x { get { return _root._u_x; } set { this.set_u_pos(value,this._u_y); } }
	public float _u_y { get { return _root._u_y; } set { this.set_u_pos(this._u_x,value); } }
	public Vector2 get_u_pos() { return _root.get_u_pos(); }
	public PlayerCharacter set_u_pos(float x, float y) { _root.set_u_pos(x,y); return this; }
	public PlayerCharacter set_scale_x(float val) { _img.set_img_scale_x(val); return this; }
	public float rotation() { return _img._rendered_img.rotation(); }
	public PlayerCharacter set_rotation(float deg) { _img._rendered_img.set_rotation(deg); return this; }
	public Vector2 get_center() {
		return new Vector2(_root._u_x,_root._u_y+128);
	}
	public void set_center_u_pos(Vector2 vec) { this.set_center_u_pos(vec.x,vec.y); }
	public void set_center_u_pos(float x, float y) {
		Vector2 center_delta = new Vector2(this.get_center().x-_root._u_x,this.get_center().y-_root._u_y);
		Vector2 inv_center_delta = SPUtil.vec_scale(center_delta,-1);
		this.set_u_pos(x+inv_center_delta.x,y+inv_center_delta.y);
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
		_img.i_update();
		_streak_left_anim.i_update();
		_streak_right_anim.i_update();
	}
	
	public SPHitRect get_hit_rect() {
		return SPHitPoly.hitpoly_to_bounding_hitrect(
			this.get_hit_poly(),
			new Vector2(-10,-10),
			new Vector2(10,10)
		);
	}
	
	public SPHitPoly get_hit_poly() {
		return SPHitPoly.cons_with_basis_offset(
			this.get_center(),
			this.rotation(),
			new Vector2(40,130),
			new Vector2(1,1),
			1,
			new Vector2(-40,0)
		);
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
