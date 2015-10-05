using UnityEngine;
using System.Collections;

public class SPDebugRender : CameraRenderHookDelegate {
	public static SPDebugRender cons() {
		return (new SPDebugRender()).i_cons();
	}
	
	private Material _mat; 
	public SPDebugRender i_cons() {
		_mat = new Material(ShaderResource.get_shader(RSha.DEFAULT));
		return this;
	}

	public void on_pre_render() {}
	public void on_post_render() {
		GL.Begin(GL.TRIANGLES);
		_mat.SetPass(0);
		GL.Color(new Color(1,0,0,0.25f));
		GL.Vertex3(10f,10f,0);
		GL.Vertex3(0f,0f,0);
		GL.Vertex3(10f,0f,0);
		GL.End();
	}
	
	public void draw_hit_rect(SPHitRect rect, Color color) {
	
	}
	
	public void draw_hit_poly(SPHitPoly poly, Color color) {
	
	}
	
}
