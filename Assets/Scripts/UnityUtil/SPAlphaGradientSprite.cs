using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(SPAlphaGradientSprite), true)]
#endif
public class SPAlphaGradientSprite : SPSprite {

	public new static SPSprite cons_sprite_texkey_texrect(string texkey, Rect texrect) { throw new System.Exception("SPAlphaGradientSprite::cons_sprite_texkey_textrect"); }
	public new static SPNode cons_node() { throw new System.Exception("SPAlphaGradientSprite::cons_node"); }

	public override void repool() {
		_meshrenderer.material = null;
		SPNode.generic_repool<SPAlphaGradientSprite>(this);
	}

	public static SPAlphaGradientSprite cons_alphagradient_sprite(string texkey, Rect texrect, SPRange x_alpha, SPRange y_alpha) {
		return SPNode.generic_cons<SPAlphaGradientSprite>().i_cons_alphagradient_sprite(texkey,texrect,x_alpha,y_alpha);
	}

	private SPAlphaGradientSprite i_cons_alphagradient_sprite(string texkey, Rect texrect, SPRange x_alpha, SPRange y_alpha) {
		this.i_cons_sprite_texkey_texrect(texkey, texrect);

		Color[] vtx_colors = new Color[4];
		vtx_colors[VTX_0_0] = new Color(1,1,1,x_alpha._min * y_alpha._min);
		vtx_colors[VTX_0_1] = new Color(1,1,1,x_alpha._min * y_alpha._max);
		vtx_colors[VTX_1_0] = new Color(1,1,1,x_alpha._max * y_alpha._min);
		vtx_colors[VTX_1_1] = new Color(1,1,1,x_alpha._max * y_alpha._max);
		_meshfilter.mesh.colors = vtx_colors;

		return this;
	}

}
