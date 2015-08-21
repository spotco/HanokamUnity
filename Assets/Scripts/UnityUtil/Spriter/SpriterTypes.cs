using UnityEngine;
using System.Collections.Generic;

public class TGSpriterFolder {
	public int _id, _atlas;
	public Dictionary<int,TGSpriterFile> _files = new Dictionary<int, TGSpriterFile>();
}

public class TGSpriterFile {
	public int _id;
	public string _name = "";
	public Rect _rect = new Rect();
	public Vector2 _pivot = new Vector2();
	public string _texkey;
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
	public float _scaleX, _scaleY, _alpha;
}

public class TGSpriterTimeLine {
	public List<TGSpriterTimelineKey> _keys = new List<TGSpriterTimelineKey>();
	public string _name = "";
	public int _id;

	public TGSpriterTimelineKey key_for_time(float val) {
		return _keys[this.index_of_key_for_time(val)];
	}

	public TGSpriterTimelineKey next_key_for_time(float val) {
		return _keys[this.index_of_next_key_for_time(val)];
	}

	public int index_of_key_for_time(float val) {
		if (_keys.Count > 0) {
			TGSpriterTimelineKey min_keyframe = _keys[0];
			for (int i = 1; i < _keys.Count; i++) {
				TGSpriterTimelineKey itr = _keys[i];
				if (itr._startsAt < min_keyframe._startsAt) min_keyframe = itr;
			}
			if (val < min_keyframe._startsAt) {
				return ((int)_keys.Count-1);
			}
		}
		int rtv = 0;
		for (int i = 0; i < _keys.Count; i++) {
			TGSpriterTimelineKey keyframe = _keys[i];
			if (keyframe._startsAt >= val) {
				break;
			} else {
				rtv = i;
			}
		}
		return rtv;
	}

	public int index_of_next_key_for_time(float val) {
		return (this.index_of_key_for_time(val)+1)%_keys.Count;
	}
	public void add_key_frame(TGSpriterTimelineKey frame) {
		_keys.Add(frame);
	}
}

public class TGSpriterAnimation {
	public string _name = "";
	public List<TGSpriterMainlineKey> _mainline_keys = new List<TGSpriterMainlineKey>();
	public Dictionary<int,TGSpriterTimeLine> _timelines = new Dictionary<int, TGSpriterTimeLine>();
	public long _duration;
	public TGSpriterMainlineKey nth_mainline_key(int i) {
		return _mainline_keys[i];
	}
	public TGSpriterTimeLine timeline_key_of_id(int id) {
		return _timelines[id];
	}
}