using UnityEngine;
using System.Collections.Generic;

public class SPDebugRender : CameraRenderHookDelegate {
	
	public struct DebugRenderQuad {
		public Vector3 _v0, _v1, _v2, _v3;
		public Color _color;
	}

	public static SPDebugRender cons() {
		return (new SPDebugRender()).i_cons();
	}
	
	private Material _mat; 
	public SPDebugRender i_cons() {
		_mat = new Material(ShaderResource.get_shader(RShader.DEFAULT));
		return this;
	}
	
	private List<DebugRenderQuad> _quads = new List<DebugRenderQuad>();
	
	public void on_pre_render() {}
	public void on_post_render() {
		for (int i = 0; i < _quads.Count; i++) {
			DebugRenderQuad itr = _quads[i];
			
			GL.PushMatrix();
			GL.MultMatrix(GameMain._context.transform.localToWorldMatrix);
			GL.Begin(GL.TRIANGLES);
			_mat.SetPass(0);
			GL.Color(itr._color);
			GL.Vertex(itr._v0);
			GL.Vertex(itr._v1);
			GL.Vertex(itr._v2);
			GL.Vertex(itr._v0);
			GL.Vertex(itr._v2);
			GL.Vertex(itr._v3);
			GL.End();
			
			GL.PopMatrix();
		}
	}
	
	public void clear_draw_queue() {
		_quads.Clear();
	}
	
	public void draw_hit_rect(SPHitRect rect, Color color) {
		_quads.Add(new DebugRenderQuad() {
			_v0 = new Vector2(rect._x1, rect._y1),
			_v1 = new Vector2(rect._x2, rect._y1),
			_v2 = new Vector2(rect._x2, rect._y2),
			_v3 = new Vector2(rect._x1, rect._y2),
			_color = color
		});
	}
	
	public void draw_hit_poly(SPHitPoly poly, Color color) {
		_quads.Add(new DebugRenderQuad() {
			_v0 = poly.pts(0),
			_v1 = poly.pts(1),
			_v2 = poly.pts(2),
			_v3 = poly.pts(3),
			_color = color
		});
	}
	
	public void draw_hitpoly_owner(SPHitPolyOwner owner, Color rect_color, Color poly_color) {
		this.draw_hit_rect(owner.get_hit_rect(), rect_color);
		this.draw_hit_poly(owner.get_hit_poly(), poly_color);
	}
	
}
