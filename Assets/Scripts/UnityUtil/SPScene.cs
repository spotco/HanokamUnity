using UnityEngine;
using System.Collections;

public class SPScene : MonoBehaviour {
	public virtual void i_update(float dt_scale){}
	public virtual bool should_remove(){ return false; }
	public virtual void do_remove() { }
}
