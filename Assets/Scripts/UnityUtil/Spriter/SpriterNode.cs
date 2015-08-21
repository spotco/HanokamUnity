//#define SPRITER_DEBUG
using UnityEngine;
using System.Collections.Generic;

public class SPNode_Bone : SPNode {
	public int _timeline_id;
	public static SPNode_Bone cons_bone() { return SPNode.generic_cons<SPNode_Bone>(); }
	public override SPNode repool() { return SPNode.generic_repool<SPNode_Bone>(this); }
}

public class SPSprite_Object : SPSprite {
	public int _timeline_id, _zindex;
	public static SPSprite_Object cons_object() { 
		SPSprite_Object rtv = SPNode.generic_cons<SPSprite_Object>();
		rtv.i_cons_sprite_texkey_texrect(RTex.BLANK,new Rect(0,0,1,1));
		return rtv;
	}
	public override SPNode repool() { return SPNode.generic_repool<SPSprite_Object>(this); }
}

public class SpriterNode : SPNode {
	private SpriterData _data;
	private Dictionary<int,SPNode_Bone> _bones = new Dictionary<int, SPNode_Bone>();
	private Dictionary<int,SPSprite_Object> _objs = new Dictionary<int, SPSprite_Object>();

	private SPNode_Bone _root_bone;
	private SPNode _root_bone_holder;

	private string _current_playing_anim;
	private float _current_anim_time;
	private int _anim_duration;
	private bool _repeat_anim, _anim_finished;

	private string _on_finish_play_anim;
	private long _last_bone_structure_hash;

	public bool current_anim_repeating() { return _repeat_anim; }
	public bool current_anim_finished() { return _anim_finished; }

	public static SpriterNode cons_spriternode_from_data(SpriterData data) {
		return SPNode.generic_cons<SpriterNode>().i_cons_spriternode_from_data(data);
	}
	public override SPNode repool() {
		this.reset_fields();
		return SPNode.generic_repool<SpriterNode>(this);
	}

	private void reset_fields() {
		_data = null;
		_bones.Clear();
		_objs.Clear();
		_root_bone = null;
		_root_bone_holder = null;
		_current_playing_anim = null;
		_current_anim_time = 0;
		_anim_duration = 0;
		_repeat_anim = false;
		_anim_finished = true;
		_on_finish_play_anim = null;
		_last_bone_structure_hash = 0;
	}

	private SpriterNode i_cons_spriternode_from_data(SpriterData data) {
		this.reset_fields();
		SpriterUtil.calc_table_scubic_point_for_t();

		_data = data;
		_root_bone_holder = SPNode.cons_node().set_name("_root_bone_holder");
		this.add_child(_root_bone_holder);

		return this;
	}

	public void p_play_anim(string anim, bool repeat) {
		_on_finish_play_anim = null;
		if (_current_playing_anim != anim) {
			this.i_play_anim(anim,repeat);
		}
	}

	public void p_play_anim_on_finish(string anim1, string anim2) {
		_on_finish_play_anim = anim2;
		this.i_play_anim(anim1,false);
	}

	private void i_play_anim(string anim_name, bool repeat) {
		if (_data.anim_of_name(anim_name) == null) { Debug.LogError("SpriterNode does not contain animation named "+anim_name); return; }

		_current_anim_time = 0;
		_current_playing_anim = anim_name;
		_anim_duration = (int)_data.anim_of_name(anim_name)._duration;
		_repeat_anim = repeat;
		_anim_finished = false;

		this.update_mainline_keyframes();
		this.update_timeline_keyframes();
	}

	public override void Update() {
		_current_anim_time += Time.deltaTime * 1000;

		if (_current_anim_time > _anim_duration) {
			if (_repeat_anim) {
				_current_anim_time = _current_anim_time-_anim_duration;
			} else {
				if (_on_finish_play_anim != null) {
					this.i_play_anim(_on_finish_play_anim,true);
					_on_finish_play_anim = null;
				} else {
					_current_anim_time = _anim_duration;
					_anim_finished = true;
				}
			}
		}
		this.update_mainline_keyframes();
		this.update_timeline_keyframes();
	}

	private static float get_t_for_keyframes(TGSpriterTimelineKey keyframe_current, TGSpriterTimelineKey keyframe_next, float _current_anim_time, float _anim_duration, bool _repeat_anim) {
		float t;
		if (keyframe_current._startsAt > keyframe_next._startsAt) {
			if (_repeat_anim) {
				t = Mathf.Clamp((_current_anim_time-keyframe_current._startsAt)/(_anim_duration-keyframe_current._startsAt)  + keyframe_next._startsAt, 0, 1);
			} else {
				t = 0;
			}
		} else {
			t = Mathf.Clamp((_current_anim_time-keyframe_current._startsAt)/(keyframe_next._startsAt-keyframe_current._startsAt),0,1);
		}
		return t;
	}

	private void update_timeline_keyframes() {
		foreach (int itr in _bones.Keys) {
			SPNode_Bone itr_bone = _bones[itr];
			TGSpriterTimeLine timeline = _data.anim_of_name(_current_playing_anim).timeline_key_of_id(itr_bone._timeline_id);
			TGSpriterTimelineKey keyframe_current = timeline.key_for_time(_current_anim_time);
			TGSpriterTimelineKey keyframe_next = timeline.next_key_for_time(_current_anim_time);

			float t = get_t_for_keyframes(keyframe_current, keyframe_next, _current_anim_time, _anim_duration, _repeat_anim);

			this.interpolate(itr_bone,keyframe_current,keyframe_next,t);
		}
		foreach (int itr in _objs.Keys) {
			SPSprite_Object itr_obj = _objs[itr];
			TGSpriterTimeLine timeline = _data.anim_of_name(_current_playing_anim).timeline_key_of_id(itr_obj._timeline_id);

			TGSpriterTimelineKey keyframe_current = timeline.key_for_time(_current_anim_time);
			TGSpriterTimelineKey keyframe_next = timeline.next_key_for_time(_current_anim_time);

			float t = get_t_for_keyframes(keyframe_current, keyframe_next, _current_anim_time, _anim_duration, _repeat_anim);
			this.interpolate(itr_obj,keyframe_current,keyframe_next,t);

			TGSpriterFile file = _data.file_for_folderid(keyframe_current._folder,keyframe_current._file);
			itr_obj.set_texkey(file._texkey);
			itr_obj.set_tex_rect(file._rect);
		}
	}

	private Vector2 get_root_chain_scale(SPNode tar) {
		float scfx = 1, scfy = 1;
		while (tar._parent != null) {
			tar = tar._parent;
			if (tar.GetType() == typeof(SPNode_Bone)) {
				scfx *= tar.scale_x();
				scfy *= tar.scale_y();
			} else {
				break;
			}
		}
		return new Vector2(scfx,scfy);
	}

	private void interpolate(SPNode node, TGSpriterTimelineKey from, TGSpriterTimelineKey to, float t) {
		Vector2 rcs = this.get_root_chain_scale(node);

		node.set_u_pos(SpriterUtil.scubic_interp(from._position.x/rcs.x,to._position.x/rcs.x,t),SpriterUtil.scubic_interp(from._position.y/rcs.y,to._position.y/rcs.y,t));
		node.set_rotation(SpriterUtil.scubic_angular_interp(from._rotation,to._rotation,t));
		node.set_scale_x(SpriterUtil.scubic_interp(from._scaleX,to._scaleX,t));
		node.set_scale_y(SpriterUtil.scubic_interp(from._scaleY,to._scaleY,t));
		node.set_opacity(SpriterUtil.scubic_interp(from._alpha,to._alpha,t));
		node.set_anchor_point(SpriterUtil.scubic_interp(from._anchorPoint.x,to._anchorPoint.x,t),SpriterUtil.scubic_interp(from._anchorPoint.y,to._anchorPoint.y,t));
	}

	private void update_mainline_keyframes() {
		TGSpriterAnimation anim = _data.anim_of_name(_current_playing_anim);
		TGSpriterMainlineKey mainline_key = null;
		for (int i = 0; i < anim._mainline_keys.Count; i++) {
			if (anim.nth_mainline_key(i)._start_time > _current_anim_time) {
				break;
			}
			mainline_key = anim.nth_mainline_key(i);
		}
		if (mainline_key == null) return;
		if (_last_bone_structure_hash != mainline_key._hash) {
			this.make_bone_hierarchy(mainline_key);
			this.attach_objects_to_bone_hierarchy(mainline_key);
			_last_bone_structure_hash = mainline_key._hash;
		}
	}

	private HashSet<int> __unadded_bones = new HashSet<int>();
	private void make_bone_hierarchy(TGSpriterMainlineKey mainline_key) {
		__unadded_bones.Clear();
		HashSet<int> unadded_bones = __unadded_bones;
		foreach (int key in _bones.Keys) unadded_bones.Add(key);

		for (int i = 0; i < mainline_key._bone_refs.Count; i++) {
			TGSpriterObjectRef bone_ref = mainline_key.nth_bone_ref(i);
			int bone_ref_id = bone_ref._id;
			if (!_bones.ContainsKey(bone_ref_id)) {
				_bones[bone_ref_id] = SPNode_Bone.cons_bone();

			} else {
				unadded_bones.Remove(bone_ref_id);

			}
			SPNode_Bone itr_bone = _bones[bone_ref_id];
#if SPRITER_DEBUG
			itr_bone.set_name("bone:"+bone_ref_id.ToString());
#endif
			itr_bone._timeline_id = bone_ref._timeline_id;
			itr_bone.set_should_autosort_children(false);
		}

		for (int i = 0; i < mainline_key._bone_refs.Count; i++) {
			TGSpriterObjectRef bone_ref = mainline_key.nth_bone_ref(i);
			int bone_ref_id = bone_ref._id;
			SPNode_Bone itr_bone = _bones[bone_ref_id];

			itr_bone.remove_from_parent();
			if (bone_ref._is_root) {
				_root_bone = itr_bone;
				_root_bone_holder.add_child(_root_bone);

			} else {
				SPNode_Bone itr_bone_parent = _bones[bone_ref._parent_bone_id];
				itr_bone_parent.add_child(itr_bone);
			}
		}

		foreach (int itr in unadded_bones) {
			SPNode_Bone itr_bone = _bones[itr];
			_bones.Remove(itr);
			itr_bone.remove_all_children();
			itr_bone.remove_from_parent(true);
		}
		unadded_bones.Clear();
	}

	private HashSet<int> __unadded_objects = new HashSet<int>();
	private void attach_objects_to_bone_hierarchy(TGSpriterMainlineKey mainline_key) {
		__unadded_objects.Clear();
		HashSet<int> unadded_objects = __unadded_objects;
		foreach (int key in _objs.Keys) unadded_objects.Add(key);

		for (int i = 0; i < mainline_key._object_refs.Count; i++) {
			TGSpriterObjectRef obj_ref = mainline_key.nth_object_ref(i);
			int obj_ref_id = obj_ref._id;
			if (!_objs.ContainsKey(obj_ref_id)) {
				_objs[obj_ref_id] = SPSprite_Object.cons_object();
			
			} else {
				unadded_objects.Remove(obj_ref_id);

			}
			SPSprite_Object itr_obj = _objs[obj_ref_id];
			itr_obj.remove_from_parent();
			itr_obj._timeline_id = obj_ref._timeline_id;
			itr_obj._zindex = obj_ref._zindex;
			itr_obj.set_manual_child_sort_z_offset(itr_obj._zindex*0.1f);
#if SPRITER_DEBUG
			itr_obj.set_name("obj:"+obj_ref_id);
#endif

			SPNode_Bone itr_bone_parent = _bones[obj_ref._parent_bone_id];
			itr_bone_parent.add_child(itr_obj);

		}

		foreach (int itr in unadded_objects) {
			SPSprite_Object itr_objs = _objs[itr];
			_objs.Remove(itr);
			itr_objs.remove_from_parent(true);
		}
		unadded_objects.Clear();
	}

}
