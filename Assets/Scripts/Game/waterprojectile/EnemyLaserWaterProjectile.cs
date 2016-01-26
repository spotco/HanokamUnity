using UnityEngine;
using System.Collections;

public class EnemyLaserWaterProjectile : WaterProjectileBase, GenericPooledObject {
	public static EnemyLaserWaterProjectile cons(GameEngineScene g, DiveGameState state, Vector2 start, Vector2 dir) {
		return (ObjectPool.inst().generic_depool<EnemyLaserWaterProjectile>()).i_cons(g, state, start, dir);
	}
	
	private SPNode _root;
	private EnemyLaserBaseSprite _base;
	private SPSprite _body;
	public override void add_to_parent(SPNode parent) { parent.add_child(_root); }
	
	public void depool() {
		_root = SPNode.cons_node();
		_root.set_name("EnemyLaserWaterProjectile");
		
		_base = EnemyLaserBaseSprite.cons();
		_base.add_to_parent(_root);
	
		_body = SPSprite.cons_sprite_texkey_texrect(RTex.ENEMY_LASER_BODY,FileCache.inst().get_texrect(RTex.ENEMY_LASER_BODY,"laser_body_0.png"));
		_body.set_anchor_point(0.5f,0);
		_root.add_child(_body);
		
		_base._sprite.set_shader(RShader.LASER_COLOR_SHADER);
		_body.set_shader(RShader.LASER_COLOR_SHADER);
		
		Color fill_color = new Color(0,1,1,1);
		Color stroke_color = new Color(1,0,1,1);
		_base._sprite.set_opacity(0.85f);
		_body.set_opacity(0.85f);
		_base._sprite.add_material_color_param("_fill_color",fill_color);
		_base._sprite.add_material_color_param("_stroke_color",stroke_color);
		_body.add_material_color_param("_fill_color",fill_color);
		_body.add_material_color_param("_stroke_color",stroke_color);
	}
	
	public void repool() {
		ObjectPool.inst().generic_repool<EnemyLaserBaseSprite>(_base);
		_base = null;
		_root.repool();
		_root = null;
	}
	
	public enum Mode {
		AnimStart,
		Hold,
		AnimOut
	}
	
	public Mode _mode;
	private float _body_height;
	private SPSpriteAnimator _body_anim;
	
	private EnemyLaserWaterProjectile i_cons(GameEngineScene g, DiveGameState state, Vector2 start, Vector2 dir) {
		_base_params._pos = start;
		
		_base._root.set_rotation(SPUtil.dir_ang_deg(dir.x,dir.y)-90);
		_base._sprite.set_manual_sort_z_order(GameAnchorZ.BGWater_FX+1);
		_body.set_rotation(SPUtil.dir_ang_deg(dir.x,dir.y)-90);
		_body.set_manual_sort_z_order(GameAnchorZ.BGWater_FX);
		
		_mode = Mode.AnimStart;
		_base._animator.play_anim(EnemyLaserBaseSprite.ANIM_START);
		
		_body_height = 0;
		_body_anim = SPSpriteAnimator.cons(_body)
			.add_anim("play",
				SPUtil.list_reverse(FileCache.inst().get_rects_list(RTex.ENEMY_LASER_BODY,"laser_body_%d.png",0,5)),5)
			.play_anim("play");
		
		return this;
	}
	
	public override void i_update(GameEngineScene g, DiveGameState state) {
		_base.i_update(g);
		
		switch (_mode) {
		case Mode.AnimStart: {
			if (_base._animator.is_finished()) {
				_mode = Mode.Hold;
				_base._animator.play_anim(EnemyLaserBaseSprite.ANIM_POINT);
				_base._sprite.set_scale(0.25f);
			}
		} break;
		case Mode.Hold: {
			if (SPUtil.get_horiz_world_bounds().extend(100).contains(this.pos_of_laser_tip().x)) {
				_body_height += 20 * SPUtil.dt_scale_get();
			}
			_body_anim.i_update();
			
		
		} break;
		case Mode.AnimOut: {
		
		} break;
		}
		
		this.apply_body_height();
		this.update_base_position();
	}
	
	private void apply_body_height() {
		Rect tex_rect = _body.texrect();
		tex_rect.height = _body_height;
		_body.set_tex_rect(tex_rect);
		_body.manual_set_mesh_size(tex_rect.width,tex_rect.height);
	}
	
	private Vector2 pos_of_laser_tip() {
		Vector2 center_05_0 = _root.get_u_pos();
		Vector2 basis_right = SPUtil.ang_deg_dir(_body.rotation());
		Vector2 basis_up = SPUtil.vec_cross(SPUtil.vec_z,basis_right);
		return SPUtil.vec_add(center_05_0, SPUtil.vec_scale(basis_up,_body.texrect().height));
	}
	
	private void update_base_position() {
		if (_mode == Mode.AnimStart) {
			_base._root.set_u_pos(0,0);
		} else {
			Vector2 dir = SPUtil.vec_sub(this.pos_of_laser_tip(),_root.get_u_pos()).normalized;
			_base._root.set_u_pos(SPUtil.vec_scale(dir,15));
			_base._sprite.set_scale(SPUtil.drpt(_base._sprite.scale_x(),1,1/15.0f));
		}
	}
	
	public override bool should_remove(GameEngineScene g, DiveGameState state) { return false; }
	public override void do_remove() {
		ObjectPool.inst().generic_repool<EnemyLaserWaterProjectile>(this);
	}
	
	protected override void apply_offset_to_position() {
		_root.set_u_pos(_base_params._pos.x,_base_params._pos.y - _env_offset);
	}
	
	public override SPHitRect get_hit_rect() {
		return SPHitPoly.hitpoly_to_bounding_hitrect(
			this.get_hit_poly(),
			new Vector2(-10,-10),
			new Vector2(10,10)
		);
	}
	public override SPHitPoly get_hit_poly() { 
		Vector2 center_05_0 = _root.get_u_pos();
		Vector2 basis_right = SPUtil.ang_deg_dir(_body.rotation());
		Vector2 basis_up = SPUtil.vec_cross(SPUtil.vec_z,basis_right);
		SPHitPoly rtv = new SPHitPoly() {
			_pts0 = SPUtil.vec_add(center_05_0, SPUtil.vec_add(SPUtil.vec_scale(basis_right,50),SPUtil.vec_scale(basis_up,-50))),
			_pts1 = SPUtil.vec_add(center_05_0, SPUtil.vec_add(SPUtil.vec_scale(basis_right,-50),SPUtil.vec_scale(basis_up,-50))),
			_pts2 = SPUtil.vec_add(center_05_0, SPUtil.vec_add(SPUtil.vec_scale(basis_right,50),SPUtil.vec_scale(basis_up,_body.texrect().height))),
			_pts3 = SPUtil.vec_add(center_05_0, SPUtil.vec_add(SPUtil.vec_scale(basis_right,-50),SPUtil.vec_scale(basis_up,_body.texrect().height)))
		};
		return rtv;
	}
	
}
