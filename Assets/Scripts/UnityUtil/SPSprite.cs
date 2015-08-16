using UnityEngine;
using System.Collections;

public class SPSprite : MonoBehaviour {

	public static SPSprite cons(string texkey, Rect texrect) {
		GameObject neu_obj = (new GameObject("SPSprite"));
		neu_obj.transform.parent = GameMain._context.gameObject.transform;
		return neu_obj.AddComponent<SPSprite>().i_cons(texkey,texrect);
	}

	private SPSprite i_cons(string texkey, Rect texrect) {
		this.gameObject.AddComponent<MeshFilter>().mesh = MeshGen.get_cached_unit_quad_mesh();
		this.gameObject.AddComponent<MeshRenderer>().material = GameMain._context._tex_resc.get_material_default(texkey);
		return this;
	}

	public void Start () {
		
	}


	public void Update () {
	
	}
}
