using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacter : SPGameUpdateable, SPHitPolyOwner, SPNodeHierarchyElement {
	
	public static PlayerCharacter cons(GameEngineScene g) {
		return (new PlayerCharacter()).i_cons(g);
	}
	
	private SPNode _root;
	private SpriterNode _img;

	private SPSprite _streak_left, _streak_right, _water_dash;
	private float _streak_tar_alpha;
	private float _waterdash_active_ct;
	private SPSpriteAnimator _streak_left_anim, _streak_right_anim, _water_dash_anim;
	
	private float _trail_tar_alpha;
	private SPSprite _trail;
	
	public PlayerAimReticule _aim_retic;
	
	public struct Params {
		public float _max_health;
	}
	public Params _params;
	
	private PlayerCharacter i_cons(GameEngineScene g) {
		_root = SPNode.cons_node();
		_root.set_name("PlayerCharacter");
		
		_params = new Params() {
			_max_health = 3f
		};
		
		SpriterData data = SpriterData.cons_data_from_spritesheetreaders(new List<SpriterJSONParser> { 
			SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA,RTex.SPRITER_HANOKA),
			SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA_BOW,RTex.SPRITER_HANOKA_BOW),
			SpriterJSONParser.cons_from_texture_and_file(RTex.SPRITER_HANOKA_SWORD,RTex.SPRITER_HANOKA_SWORD)
		}, RTex.SPRITER_HANOKA);

		_img = SpriterNode.cons_spriternode_from_data(data);
		_img.p_play_anim("Idle",true);
		_img.set_manual_sort_z_order(GameAnchorZ.Player_Ground);
		_img.set_name("_img");
		_img.set_layer(RLayer.REFLECTION_SURFACE_CHARACTER);
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
		
		_water_dash = SPSprite.cons_sprite_texkey_texrect(RTex.HANOKA_EFFECTS_WATER, FileCache.inst().get_texrect(RTex.HANOKA_EFFECTS_WATER, "water_dash_0.png"));
		_water_dash.set_u_pos(0,120);
		_water_dash.set_anchor_point(0.5f,0.75f);
		_water_dash.set_manual_sort_z_order(GameAnchorZ.BGWater_FX);
		_root.add_child(_water_dash);
		
		_water_dash_anim = SPSpriteAnimator.cons(_water_dash).add_anim("play",
			FileCache.inst().get_rects_list(
				RTex.HANOKA_EFFECTS_WATER,"water_dash_%d.png",0,4),4).play_anim("play");

		this.set_streak_enabled(false);
		this.set_trail_enabled_and_rotation(false);
		
		_aim_retic = PlayerAimReticule.cons();
		_aim_retic.set_u_pos(this.get_center_offset());
		_aim_retic.add_to_parent(_root);
		_aim_retic.set_enabled(false);
		
		return this;
	}

	public void set_streak_enabled(bool val) {
		_streak_tar_alpha = val?1:0;
	}
	public void show_waterdash_for(float val) {
		_waterdash_active_ct = val;
	}
	public void set_trail_enabled_and_rotation(bool val, float angle=0) {
		_trail_tar_alpha = val?1:0;
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
	public float scale_x() { return _img.img_scale_x(); }
	public float rotation() { return _img._rendered_img.rotation(); }
	public PlayerCharacter set_rotation(float deg) { _img._rendered_img.set_rotation(deg); return this; }
	public Vector2 get_center_offset() { return new Vector2(0,128); }
	public Vector2 get_center() {
		return SPUtil.vec_add(_root.get_u_pos(), this.get_center_offset());
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
	public bool is_anim_finished() {
		return _img.current_anim_finished();
	}
	
	public void i_update(GameEngineScene g) {
		_img.i_update();
		
		_water_dash.set_rotation(this.rotation());
		_water_dash.set_opacity(SPUtil.drpt(_water_dash.get_opacity(),_waterdash_active_ct>0?0.7f:0,1/4.0f));
		_water_dash.set_scale(SPUtil.y_for_point_of_2pt_line(new Vector2(0,1.35f),new Vector2(1,1),_water_dash.get_opacity()));
		_waterdash_active_ct = Mathf.Clamp(_waterdash_active_ct-SPUtil.dt_scale_get(),0,1);
		if (!SPUtil.flt_cmp_delta(_water_dash.get_opacity(),0,0.1f)) {
			_water_dash_anim.i_update();
		}
		
		_streak_left.set_opacity(SPUtil.lmovto(_streak_left.get_opacity(),_streak_tar_alpha,0.2f*SPUtil.dt_scale_get()));
		_streak_right.set_opacity(_streak_left.get_opacity());
		
		if (SPUtil.flt_cmp_delta(_streak_left.get_opacity(),0,0.1f)) {
			_streak_left.set_enabled(false);
			_streak_right.set_enabled(false);
		} else {
			_streak_left.set_enabled(true);
			_streak_right.set_enabled(true);
			_streak_left_anim.i_update();
			_streak_right_anim.i_update();
		}
		
		_trail.set_opacity(SPUtil.lmovto(_trail.get_opacity(),_trail_tar_alpha,0.2f*SPUtil.dt_scale_get()));
		_trail.set_enabled(!SPUtil.flt_cmp_delta(_trail.get_opacity(),0,0.1f));
		
		_aim_retic.i_update(g);
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
	
	public float get_arrow_target_rotation() {
		float line_target_rotation = this.rotation() + (this.scale_x() > 0 ? 120 : -120);
		return line_target_rotation;
	}
	
	public float get_target_rotation_for_aim_direction(Vector2 dir) {
		float ang = SPUtil.dir_ang_deg(dir.x,dir.y);
		return ang + (this.scale_x() > 0 ? 150 : -150 + 180);
	}
}

public class PlayerCharacterAnims {
	public static string IDLE = "Idle";
	public static string RUN = "Run";
	public static string WALK = "Walk";
	
	public static string DIVE_RUN_FORWARD_PREPARE = "Run Forward Prepare";
	public static string DIVE_RUN_FORWARD = "Run Forward";
	public static string DIVE_FLIP_AND_JUMP = "Flip and Jump";
	public static string DIVE_FORWARD_SPIN = "Forward Spin";
	public static string DIVE_TRANSITION = "Dive Transition";
	public static string DIVE_FALL = "Dive";
	
	
	// old
	public static string PREPDIVE = "Prep Dive (old)";
	public static string DIVE = "Dive (old)";
	//
	
	public static string SPIN = "Spin";
	
	public static string SWIM = "Swim";
	public static string SWIM_SLOW = "SwimSlow";
	public static string SWIM_SPIN = "Swim_Spin";
	
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

	public static Vector2 pos_in_bounds(float x, float y) {
		SPRange bounds_ext = SPUtil.get_horiz_world_bounds().extend(-50);
		return new Vector2(Mathf.Clamp(x,bounds_ext._min,bounds_ext._max),y);
	}

	public static void move_in_bounds(PlayerCharacter player, float x, float y) {
		SPRange bounds_ext = SPUtil.get_horiz_world_bounds().extend(-50);
		player.set_u_pos(Mathf.Clamp(x,bounds_ext._min,bounds_ext._max),y);
	}
	public static void move_center_in_bounds(PlayerCharacter player, float x, float y) {
		SPRange bounds_ext = SPUtil.get_horiz_world_bounds().extend(-50);
		player.set_center_u_pos(Mathf.Clamp(x,bounds_ext._min,bounds_ext._max),y);
	}
	public static void rotate_to_rotation_for_vel(PlayerCharacter player, float vx, float vy, float fric, float offset = -90) {
		float tar_rotation = SPUtil.dir_ang_deg(vx,vy) + offset;
		PlayerCharacterUtil.rotate_to_rotation(player,tar_rotation,fric);
	}
	public static void rotate_to_rotation(PlayerCharacter player, float tar_rotation, float fric) {
		player.set_rotation(SPUtil.drpt(player.rotation(), player.rotation() + SPUtil.shortest_angle(player.rotation(),tar_rotation), fric));
	}
	
}
