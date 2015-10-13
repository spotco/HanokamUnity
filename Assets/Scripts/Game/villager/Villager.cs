using UnityEngine;
using System.Collections;

// TODO -- refactor for injection of behavior
public class Villager : SPGameUpdateable, SPNodeHierarchyElement {
	
	public enum Mode {
		Idle,
		Chatting
	}
	
	public static Villager cons(SpriterNode img) {
		return (new Villager()).i_cons(img);
	}
	
	private Mode _current_mode;
	private SPNode _root;
	private SpriterNode _img;
	private Villager i_cons(SpriterNode img) {
		_root = SPNode.cons_node();
		_root.name = "Villager";
		
		_current_mode = Mode.Idle;
		
		_img = img;
		_root.add_child(_img);
		_img.set_u_pos(0,0);
		
		this.setup_overhead_icon(new Vector2(0,175));
		return this;
	}
	
	public void i_update(GameEngineScene g) { 
		_img.i_update(); 
		if (this.is_in_chat_range_with_player(g)) {
			_overhead_icon.set_target_status(VillagerOverheadIcon.Status.Large);
		} else {
			_overhead_icon.set_target_status(VillagerOverheadIcon.Status.Small);
		}
		
		_overhead_icon.i_update();
	}
	
	public bool is_in_chat_range_with_player(GameEngineScene g) {
		return Mathf.Abs(g._player._u_x - _root._u_x) < 100;
	}
	
	public void set_mode(Mode mode) {
		_current_mode = mode;
	}
	
	
	public void add_to_parent(SPNode parent) { parent.add_child(_root); }
	public void add_child(SPNode child) { _root.add_child(child); }
	public void set_u_pos(float x, float y) { _root.set_u_pos(x,y); }
	public void set_u_z(float z) { _root.set_u_z(z); }
	public float _u_x { get { return _root._u_x; } }
	public float _u_y { get { return _root._u_y; } }
	
	protected VillagerOverheadIcon _overhead_icon;
	
	protected void setup_overhead_icon(Vector2 pos) {
		_overhead_icon = VillagerOverheadIcon.cons();
		this.add_child(_overhead_icon._root);
		_overhead_icon.set_u_pos(pos.x,pos.y);
	}

}


public class VillagerOverheadIcon : SPNodeHierarchyElement {
	public static VillagerOverheadIcon cons() {
		return (new VillagerOverheadIcon()).i_cons();
	}
	public SPNode _root;
	private SPSprite _back, _icon;
	private DrptVal _scale, _opacity;
	private VillagerOverheadIcon i_cons() {
		_root = SPNode.cons_node();
		_root.set_name("VillagerOverheadIcon");
		
		_back = SPSprite.cons_sprite_texkey_texrect(RTex.DIALOG_UI,FileCache.inst().get_texrect(RTex.DIALOG_UI,"icon_back_overhead.png"));
		_back.set_anchor_point(0.5f,0);
		_back.set_scale(0.85f);
		_back.set_opacity(0.75f);
		_root.add_child(_back);
		
		_icon = SPSprite.cons_sprite_texkey_texrect(RTex.DIALOG_UI,FileCache.inst().get_texrect(RTex.DIALOG_UI,"icon_dialogue.png"));
		_icon.set_scale(0.85f);
		_icon.set_opacity(0.75f);
		_icon.set_u_pos(0,122);
		_back.add_child(_icon);
		
		_scale = new DrptVal() {
			_current = 0.85f, _target = 0.85f, _drptval = 1/10.0f
		};
		_opacity = new DrptVal() {
			_current = 0.75f, _target = 0.75f, _drptval = 1/10.0f
		};
		
		return this;
	}
	
	public enum Status {
		Small,
		Large
	}
	public void set_target_status(Status status) {
		if (status == Status.Small) {
			_scale._target = 0.5f;
			_opacity._target = 0.5f;
		} else {
			_scale._target = 0.85f;
			_opacity._target = 0.75f;
		}
	}
	
	public void i_update() {
		_scale.i_update();
		_opacity.i_update();
		_back.set_scale(_scale._current);
		_icon.set_opacity(_opacity._current);
		_back.set_opacity(_opacity._current);
	}
	
	public void add_to_parent(SPNode parent) {
		parent.add_child(_root);
	}
	public void set_u_pos(float x, float y) {
		_root.set_u_pos(x,y);
	}
}

/*
public class TestVillager : BaseVillager {

}

public class TestBoyVillager : BaseVillager {
	public static TestBoyVillager cons(SpriterNode img) {
		return (new TestBoyVillager()).i_cons(img);
	}

	private enum Mode {
		Left,
		LeftToRight,
		Right,
		RightToLeft
	}

	private SpriterNode _img;
	private Mode _current_mode;
	private float _anim_t = 0;
	public TestBoyVillager i_cons(SpriterNode img) {
		_img = img;
		_current_mode = Mode.Left;
		_img.set_u_pos(-100,0);
		this.setup_overhead_icon(new Vector2(0,0));
		return this;
	}

	public override void i_update(GameEngineScene g) {
		_img.i_update();
		switch (_current_mode) {
		case Mode.Left: {
			_anim_t += SPUtil.sec_to_tick(0.5f);
			_img._u_x = -200;
			_img.p_play_anim("Idle");
			if (_anim_t >= 1) {
				_anim_t = 0;
				_current_mode = Mode.LeftToRight;
			}

		} break;
		case Mode.LeftToRight: {
			_anim_t += SPUtil.sec_to_tick(1.0f);
			_img._u_x = SPUtil.lerp(-200,200,_anim_t);
			_img.p_play_anim("Walk");
			_img.set_img_scale_x(-1);
			if (_anim_t >= 1) {
				_anim_t = 0;
				_current_mode = Mode.Right;
			}
		} break;
		case Mode.Right: {
			_anim_t += SPUtil.sec_to_tick(0.5f);
			_img._u_x = 200;
			_img.p_play_anim("Idle");
			if (_anim_t >= 1) {
				_anim_t = 0;
				_current_mode = Mode.RightToLeft;
			}
		} break;
		case Mode.RightToLeft: {
			_anim_t += SPUtil.sec_to_tick(1.0f);
			_img._u_x = SPUtil.lerp(200,-200,_anim_t);
			_img.p_play_anim("Walk");
			_img.set_img_scale_x(1);
			if (_anim_t >= 1) {
				_current_mode = Mode.Left;
				_anim_t = 0;
			}
		} break;
		}	
	}
}
*/