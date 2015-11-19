using UnityEngine;
using System.Collections;

public class SPParallaxScrollSprite : SPGameUpdateable {

	public static SPParallaxScrollSprite cons(string tex_key, Rect rect, Vector2 scale, Vector3 lpos) {
		return (new SPParallaxScrollSprite()).i_cons(tex_key,rect, scale,lpos);
	}

	public SPSprite _img;
	public float _bg_width,_bg_height;
	private float _uv_y_size = 1;
	private Rect _tex_rect;
	private float _x_offset,_y_offset;

	private SPParallaxScrollSprite i_cons(string tex_key, Rect rect, Vector2 scale, Vector3 lpos) {
		_tex_rect = rect;
		_img = SPSprite.cons_sprite_texkey_texrect(tex_key,_tex_rect);
		_img.set_u_pos(new Vector2(lpos.x,lpos.y));
		_img.set_u_z(lpos.z);
		_img.set_scale_x(scale.x);
		_img.set_scale_y(scale.y);
		_img.set_anchor_point(0.5f,0.5f);

		_uv_y_size = 1;
		_x_offset = 0;
		_y_offset = 0;
		
		_bg_width = Mathf.Abs(_img.u_pos_of_vertex(SPSprite.VTX_1_1).x-_img.u_pos_of_vertex(SPSprite.VTX_0_0).x);
		_bg_height = Mathf.Abs(_img.u_pos_of_vertex(SPSprite.VTX_1_1).y-_img.u_pos_of_vertex(SPSprite.VTX_0_0).y);
		
		return this;
	}

	public void set_uv_y_size(float sc) {
		_uv_y_size = sc;
	}

	public void i_update(GameEngineScene g) {
		_img.set_u_pos(_img._u_x,GameMain._context._game_camera.transform.localPosition.y);
		
		float normalized_uv_y = (GameMain._context._game_camera.transform.localPosition.y  + _y_offset)/ _bg_height;
		float tex_size_scaled_uv_y = (normalized_uv_y%1.0f) * _tex_rect.size.y;

		_img.set_tex_rect(new Rect(
			_tex_rect.position.x + _x_offset,
			-tex_size_scaled_uv_y,
			_tex_rect.size.x,
			_tex_rect.size.y * _uv_y_size
		));
	}
	
	public void set_x_offset(float val) {
		_x_offset = (val % _bg_width);
	}
	
	public void set_y_offset(float val) {
		_y_offset = (val % _bg_height);
	}

	public SPParallaxScrollSprite set_enabled(bool val) {
		_img.set_enabled(val);
		return this;
	}

}
