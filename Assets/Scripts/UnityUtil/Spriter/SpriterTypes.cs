using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;

[ProtoContract]
public class TGSpriterFolder {
	[ProtoMember(1)] public int _id;
	[ProtoMember(2)] public int _atlas;
	[ProtoMember(3)] public Dictionary<int,TGSpriterFile> _files = new Dictionary<int, TGSpriterFile>();
}

[ProtoContract]
public class TGSpriterFile {
	[ProtoMember(1)] public int _id;
	[ProtoMember(2)] public string _name = "";
	[ProtoMember(3)] public Rect _rect = new Rect();
	[ProtoMember(4)] public Vector2 _pivot = new Vector2();
	[ProtoMember(5)] public string _texkey;
}

[ProtoContract]
public class TGSpriterObjectRef {
	[ProtoMember(1)] public int _id;
	[ProtoMember(2)] public int _parent_bone_id;
	[ProtoMember(3)] public int _timeline_id;
	[ProtoMember(4)] public int _zindex;
	[ProtoMember(5)] public bool _is_root;
}

[ProtoContract]
public class TGSpriterMainlineKey {

	public enum CurveType {
		QUINTIC = 0,
		INSTANT = 1
	}

	[ProtoMember(1)] public int _start_time;
	[ProtoMember(2)] public long _hash;
	[ProtoMember(3)] public string _hashtest = "";
	[ProtoMember(4)] public List<TGSpriterObjectRef> _bone_refs = new List<TGSpriterObjectRef>();
	[ProtoMember(5)] public List<TGSpriterObjectRef> _object_refs = new List<TGSpriterObjectRef>();
	[ProtoMember(6)] public CurveType _curve_type;

	public TGSpriterObjectRef nth_bone_ref(int i) {
		return _bone_refs[i];
	}
	public TGSpriterObjectRef nth_object_ref(int i) {
		return _object_refs[i];
	}
}

[ProtoContract]
public class TGSpriterTimelineKey {
	[ProtoMember(1)] public int _file;
	[ProtoMember(2)] public int _folder;
	[ProtoMember(3)] public float _startsAt;
	[ProtoMember(4)] public Vector2 _position = new Vector2();
	[ProtoMember(5)] public Vector2 _anchorPoint = new Vector2();
	[ProtoMember(6)] public float _rotation;
	[ProtoMember(7)] public int _spin;
	[ProtoMember(8)] public float _scaleX;
	[ProtoMember(9)] public float _scaleY;
	[ProtoMember(10)] public float _alpha;
}

[ProtoContract]
public class TGSpriterTimeLine {
	[ProtoMember(1)] public List<TGSpriterTimelineKey> _keys = new List<TGSpriterTimelineKey>();
	[ProtoMember(2)] public string _name = "";
	[ProtoMember(3)] public int _id;

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

[ProtoContract]
public class TGSpriterAnimation {
	[ProtoMember(1)] public string _name = "";
	[ProtoMember(2)] public List<TGSpriterMainlineKey> _mainline_keys = new List<TGSpriterMainlineKey>();
	[ProtoMember(3)] public Dictionary<int,TGSpriterTimeLine> _timelines = new Dictionary<int, TGSpriterTimeLine>();
	[ProtoMember(4)] public long _duration;

	public TGSpriterMainlineKey nth_mainline_key(int i) {
		return _mainline_keys[i];
	}
	public TGSpriterTimeLine timeline_key_of_id(int id) {
		return _timelines[id];
	}
}