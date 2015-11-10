using UnityEngine;
using System.Collections.Generic;

public class SPText : SPNode {

	public static SPText cons_text(string texkey, string fntkey) {
		return SPNode.generic_cons<SPText>().i_cons_text(texkey,fntkey);
	}
	
	public new static SPNode cons_node() { throw new System.Exception("SPText::cons_node"); }
	
	public override void repool() {
		SPNode.generic_repool<SPText>(this);
	}
	
	private SPText i_cons_text(string texkey, string fntkey) {
	
		FileCache.inst().get_fntfile(RFnt.DIALOGUE_FONT);
	
		return this;
	}

}