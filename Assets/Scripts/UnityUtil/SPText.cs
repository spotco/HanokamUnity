using UnityEngine;
using System.Collections.Generic;

public class SPText : SPNode, SPMainUpdateable {

	public class SPTextCharacter : SPNodeHierarchyElement, GenericPooledObject, SPMainUpdateable {
		public static SPTextCharacter cons_texkey_rect(string texkey, Rect rect) {
			return ObjectPool.inst().generic_depool<SPTextCharacter>().i_cons_texkey_rect(texkey,rect);
		}
		public SPSprite _img;
		public void add_to_parent(SPNode parent) { parent.add_child(_img); }
		public void depool() {
			_img = SPSprite.cons_sprite_texkey_texrect(RTex.BLANK,new Rect(0,0,0,0));
		}
		public void repool() {
			_img.repool();
			_img = null;
		}
		private SPTextCharacter i_cons_texkey_rect(string texkey, Rect rect) {
			_img.set_texkey(texkey);
			_img.set_tex_rect(rect);
			return this;
		}
		public void i_update() {}
		public void set_u_pos(float x, float y) { _img.set_u_pos(x,y); }
		public void set_char_name(char c) { _img.set_name(SPUtil.sprintf("SPTextCharacter(%c)",c)); }
		public void set_manual_sort_z_order(int zord) { _img.set_manual_sort_z_order(zord); }
		public void cleanup() {
			ObjectPool.inst().generic_repool<SPTextCharacter>(this);
		}
	}
	
	public class SPTextStyle {
		public Vector4 _stroke;
		public Vector4 _fill;
		public Vector4 _shadow;
		public float _amplitude;
		public float _time_incr;
		public static SPTextStyle cons(Vector4 stroke, Vector4 fill, Vector4 shadow, float amplitude, float time_incr) {
			SPTextStyle rtv = new SPTextStyle();
			rtv._stroke = stroke;
			rtv._fill = fill;
			rtv._shadow = shadow;
			rtv._amplitude = amplitude;
			rtv._time_incr = time_incr;
			return rtv;
		}
	}

	public static SPText cons_text(string texkey, string fntkey, SPTextStyle default_style) {
		return SPNode.generic_cons<SPText>().i_cons_text(texkey,fntkey, default_style);
	}
	public new static SPNode cons_node() { throw new System.Exception("SPText::cons_node"); }
	
	public override void repool() {
		this.cleanup_existing_characters();
		_pivot_node.repool();
		_pivot_node = null;
		SPNode.generic_repool<SPText>(this);
	}
	
	private SPNode _pivot_node;
	private Vector2 _text_anchor;
	
	private string _texkey;
	private FntFile _bmfont_cfg;
	private List<SPTextCharacter> _characters;
	private Vector2 _rendered_size;
	
	private string _cached_string;
	private Dictionary<string,SPTextStyle> _name_to_styles;
	
	private float _time;
	
	private SPText i_cons_text(string texkey, string fntkey, SPTextStyle default_style) {
		this.set_u_pos(0,0);
		this.set_u_z(0);
		this.set_rotation(0);
		this.set_scale(1);
	
		_pivot_node = SPNode.cons_node();
		_pivot_node.set_name("_pivot_node");
		this.add_child(_pivot_node);
		
		_texkey = texkey;
		_bmfont_cfg = FileCache.inst().get_fntfile(fntkey);
		_characters = new List<SPTextCharacter>();
		_cached_string = "";
		_name_to_styles = new Dictionary<string, SPTextStyle>();
		_time = 0;
		
		_text_anchor = Vector2.zero;
		
		return this;
	}
	
	public void add_style(string name, SPTextStyle style) {
		_name_to_styles[name] = style;
	}
	
	public void i_update() {
	}
	
	private void cleanup_existing_characters() {
		for (int i = 0; i < _characters.Count; i++) {
			_characters[i].cleanup();
		}
		_characters.Clear();
	}
	
	public SPText set_markup_text(string text) {
		if (_cached_string == text) return this;
		_cached_string = text;
		this.cleanup_existing_characters();
		
		//todo -- style map
		string display_string = _cached_string;
		
		if (display_string.Length == 0) return this;
		
		int quantityOfLines = 1;
		for (int i = 0; i < display_string.Length-1; i++) {
			if (display_string[i] == '\n') quantityOfLines++;
		}
		float totalHeight = _bmfont_cfg.common.lineHeight * quantityOfLines;
		float nextFontPositionX = 0;
		float nextFontPositionY = -(_bmfont_cfg.common.lineHeight - totalHeight);
		float longestLine = 0;
		
		FntFile.CharInfo fontDef = null;
		
		for (int i = 0; i < display_string.Length; i++) {
			char c = display_string[i];
			if (c == '\n') {
				nextFontPositionX = 0;
				nextFontPositionY -= _bmfont_cfg.common.lineHeight;
				continue;
			}
			if (!_bmfont_cfg.contains_char(c)) {
				Debug.LogError(SPUtil.sprintf("attempted to use character not defined in this bitmap:(%c)",c));
				continue;
			}
			
			fontDef = _bmfont_cfg.charinfo_for_char(c);
			
			Rect rect = new Rect(fontDef.x,fontDef.y,fontDef.width,fontDef.height);
			SPTextCharacter neu_char = SPTextCharacter.cons_texkey_rect(_texkey, rect);
			neu_char.set_char_name(c);
			neu_char.set_manual_sort_z_order(GameAnchorZ.HUD_BASE);
			neu_char.add_to_parent(_pivot_node);
			_characters.Add(neu_char);
			
			//todo -- apply style
			
			float yoffset = _bmfont_cfg.common.lineHeight - fontDef.yoffset;
			Vector2 fontPos = new Vector2(
				nextFontPositionX + fontDef.xoffset + fontDef.width*0.5f,
				nextFontPositionY + yoffset - rect.size.y*0.5f
			);
			neu_char.set_u_pos(fontPos.x,fontPos.y);
			nextFontPositionX += fontDef.xadvance;
			
			if (longestLine < nextFontPositionX) longestLine = nextFontPositionX;
		}
		
		Vector2 tmpSize = Vector2.zero;
		if (fontDef != null && fontDef.xadvance < fontDef.width) {
			tmpSize.x = longestLine + fontDef.width - fontDef.xadvance;
		} else {
			tmpSize.x = longestLine;
		}
		tmpSize.y = totalHeight;
		_rendered_size = tmpSize;
		this.update_pivot_text_anchor();
		return this;
	}
	
	public void set_text_anchor(float x, float y) {
		_text_anchor = new Vector2(x,y);
		this.update_pivot_text_anchor();
	}
	
	private void update_pivot_text_anchor() {
		_pivot_node.set_u_pos(-_text_anchor.x * _rendered_size.x, -_text_anchor.y * _rendered_size.y);
	}

}