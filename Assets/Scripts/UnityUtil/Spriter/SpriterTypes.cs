using UnityEngine;
using System.Collections.Generic;

public class TGSpriterFolder {
	public int _id, _atlas;
	public Dictionary<string,TGSpriterFile> _files = new Dictionary<string, TGSpriterFile>();
}

public class TGSpriterFile {
	public int _id;
	public string _name = "";
	public Rect _rect = new Rect();
	public Vector2 _pivot = new Vector2();
	public Texture _texture;
}

public class TGSpriterObjectRef {
	public int _id, _parent_bone_id, _timeline_id, _zindex;
	public bool _is_root;
}

public class TGSpriterMainlineKey {
	public int _start_time;
	public long _hash;
	public string _hashtest = "";
	public List<TGSpriterObjectRef> _bone_refs = new List<TGSpriterObjectRef>();
	public List<TGSpriterObjectRef> _object_refs = new List<TGSpriterObjectRef>();
	public TGSpriterObjectRef nth_bone_ref(int i) {
		return _bone_refs[i];
	}
	public TGSpriterObjectRef nth_object_ref(int i) {
		return _object_refs[i];
	}
}

public class TGSpriterTimelineKey {
	public int _file, _folder;
	public float _startsAt;
	public Vector2 _position = new Vector2(), _anchorPoint = new Vector2();
	public float _rotation;
	public int _spin;
	float _scaleX, _scaleY, _alpha;
}

public class TGSpriterTimeLine {
	public List<TGSpriterTimelineKey> _keys = new List<TGSpriterTimelineKey>();
	public string _name = "";
	public int _id;

	public TGSpriterTimelineKey key_for_time(float val) {
		//return keys_[[self indexOfKeyForTime:val]];
	}

	public TGSpriterTimelineKey next_key_for_time(float val) {
		//return keys_[[self indexOfNextKeyForTime:val]];
	}

	public int index_of_key_for_time(float val) {
		/*
	if (keys_.count > 0) { //no keyframe in first frame case
		TGSpriterTimelineKey *min_keyframe = keys_[0];
		for (int i = 1; i < keys_.count; i++) {
			TGSpriterTimelineKey *itr = keys_[i];
			if (itr.startsAt < min_keyframe.startsAt) min_keyframe = itr;
		}
		if (val < min_keyframe.startsAt) {
			return ((int)keys_.count-1);
		}
	}
	int rtv = 0;
	for (int i = 0; i < keys_.count; i++) {
		TGSpriterTimelineKey *keyframe = keys_[i];
		if (keyframe.startsAt >= val) {
			break;
		} else {
			rtv = i;
		}
	}
	return rtv;
		*/
	}

	public int index_of_next_key_for_time(float val) {
		//return ([self indexOfKeyForTime:val]+1)%keys_.count;
	}
	public void add_key_frame(TGSpriterTimelineKey frame) {
		//[keys_ addObject:frame];
	}
}

public class TGSpriterAnimation {
	public string _name = "";
	public List<TGSpriterMainlineKey> _mainline_keys = new List<TGSpriterMainlineKey>();
	public Dictionary<string,TGSpriterTimeLine> _timelines = new Dictionary<string, TGSpriterTimeLine>();
	public long _duration;
	public TGSpriterMainlineKey nth_mainline_key(int i) {
		//return _mainline_keys[i];
	}
	public TGSpriterTimeLine timeline_key_of_id(int id) {
		//return [_timelines objectForKey:[NSNumber numberWithInt:i]];
	}
}