using UnityEngine;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

[ProtoContract]
public class SpriterData  {
	[ProtoMember(1)] public Dictionary<int,TGSpriterFolder> _folders = new Dictionary<int, TGSpriterFolder>();
	[ProtoMember(2)] public Dictionary<string,TGSpriterAnimation> _animations = new Dictionary<string, TGSpriterAnimation>();
	[ProtoMember(3)] public Dictionary<int,SpriterJSONParser> _atlas = new Dictionary<int, SpriterJSONParser>();
	[ProtoMember(4)] public string _scmlpath;

	public Dictionary<int,TGSpriterFolder> folders() { return _folders; }
	public Dictionary<string,TGSpriterAnimation> animations() { return _animations; }

	public TGSpriterAnimation anim_of_name(string name) { return _animations.ContainsKey(name)?_animations[name]:null; }
	public TGSpriterFile file_for_folderid(int folderid, int fileid) {
		TGSpriterFolder folder = _folders[folderid];
		return folder._files[fileid];
	}

	public static SpriterData cons_data_from_spritesheetreaders(List<SpriterJSONParser> sheetreaders, string scmlpath) {
		return FileCache.inst().get_spriter_data(sheetreaders,scmlpath);
	}

	public static SpriterData _cons_data_from_spritesheetreaders(List<SpriterJSONParser> sheetreaders, string scmlpath) {
		return (new SpriterData()).i_cons_data_from_spritesheetreaders(sheetreaders,scmlpath);
	}

	public SpriterData i_cons_data_from_spritesheetreaders(List<SpriterJSONParser> sheetreaders, string scmlpath) {
		Debug.LogWarning("spriter data from streaming:"+scmlpath);
		_scmlpath = scmlpath;
		TGSpriterConfigNode root = SpriterXMLParser.parse_scml(scmlpath);

		_folders = new Dictionary<int, TGSpriterFolder>();
		_animations = new Dictionary<string, TGSpriterAnimation>();
		_atlas = new Dictionary<int, SpriterJSONParser>();

		for (int i_base = 0; i_base < root._children.Count; i_base++) {
			TGSpriterConfigNode itr_base = root._children[i_base];
			if (itr_base._name == "atlas") {
				this.handle_atlas(itr_base,sheetreaders);

			} else if (itr_base._name == "folder") {
				this.handle_folder(itr_base);

			} else if (itr_base._name == "entity") {
				for (int i_entity_child = 0; i_entity_child < itr_base._children.Count; i_entity_child++) {
					TGSpriterConfigNode itr_entity_child = itr_base._children[i_entity_child];
					if (itr_entity_child._name == "animation") {
						this.handle_animation(itr_entity_child);
					}
				}
			}
		}
		return this;
	}

	public void replace_atlas_index(int index, SpriterJSONParser tar) {
		_atlas[index] = tar;
		foreach (int folder_id in _folders.Keys) {
			TGSpriterFolder itr_folder = _folders[folder_id];
			if (itr_folder._atlas == index) {
				foreach (int file_id in itr_folder._files.Keys) {
					TGSpriterFile itr_file = itr_folder._files[file_id];
					itr_file._texkey = tar.texkey();
					itr_file._rect = tar.rect_for_frame(itr_file._name);
				}
			}
		}
	}

	private void handle_atlas(TGSpriterConfigNode itr_base, List<SpriterJSONParser> sheetreaders) {
		for (int i = 0; i < itr_base._children.Count; i++) {
			TGSpriterConfigNode itr_atlas_element = itr_base._children[i];
			string itr_atlas_element_name = itr_atlas_element.get_str("name");
			for (int i_sheetreaders = 0; i_sheetreaders < sheetreaders.Count && i < sheetreaders.Count; i_sheetreaders++) {
				SpriterJSONParser itr_sheetreaders = sheetreaders[i_sheetreaders];
				if ((itr_sheetreaders.filepath()+".json").Contains(itr_atlas_element_name)) {
					_atlas[i] = itr_sheetreaders;
					break;
				}
			}
		}
	}

	private void handle_folder(TGSpriterConfigNode itr_base) {
		TGSpriterFolder neu_folder = new TGSpriterFolder();
		neu_folder._id = itr_base.get_id();
		neu_folder._atlas = itr_base.get_int("atlas");

		if (!_atlas.ContainsKey(neu_folder._atlas)) return;
		SpriterJSONParser atlas_element = _atlas[neu_folder._atlas];
		for (int i_files = 0; i_files < itr_base._children.Count; i_files++) {
			TGSpriterConfigNode itr_files = itr_base._children[i_files];
			TGSpriterFile neu_file = new TGSpriterFile();
			neu_file._id = itr_files.get_id();
			neu_file._name = itr_files.get_str("name");
			neu_file._pivot = new Vector2(itr_files.get_val("pivot_x"),itr_files.get_val("pivot_y"));
			
			neu_file._rect = atlas_element.rect_for_frame(itr_files.get_str("name")); //Oldman had a phantom folder
			neu_file._texkey = atlas_element.texkey();

			neu_folder._files[neu_file._id] = neu_file;
		}

		_folders[neu_folder._id] = neu_folder;
	}

	private void handle_animation(TGSpriterConfigNode itr_entity_child) {
		TGSpriterAnimation spriter_animation = new TGSpriterAnimation();
		spriter_animation._name = itr_entity_child.get_str("name");
		spriter_animation._duration = itr_entity_child.get_int("length");

		for (int i_animation_child = 0; i_animation_child < itr_entity_child._children.Count; i_animation_child++) {
			TGSpriterConfigNode itr_animation_child = itr_entity_child._children[i_animation_child];
			if (itr_animation_child._name == "mainline") {
				for (int i_key = 0; i_key < itr_animation_child._children.Count; i_key++) {
					TGSpriterConfigNode itr_key = itr_animation_child._children[i_key];
					this.handle_mainline_key(spriter_animation,itr_key);
				}
			
			} else if (itr_animation_child._name == "timeline") {
				this.handle_timeline(spriter_animation,itr_animation_child);

			}
		}
		_animations[spriter_animation._name] = spriter_animation;
	}

	private void handle_mainline_key(TGSpriterAnimation spriter_animation, TGSpriterConfigNode itr_key) {
		TGSpriterMainlineKey mainline_key = new TGSpriterMainlineKey();
		StringBuilder hash = new StringBuilder("-");

		for (int i_key_child = 0; i_key_child < itr_key._children.Count; i_key_child++) {
			TGSpriterConfigNode itr_key_child = itr_key._children[i_key_child];
			TGSpriterObjectRef object_ref = new TGSpriterObjectRef();
			object_ref._id = itr_key_child.get_id();
			object_ref._timeline_id = itr_key_child.get_int("timeline");
			if (!itr_key_child._properties.ContainsKey("parent")) {
				object_ref._is_root = true;
			} else {
				object_ref._is_root = false;
				object_ref._parent_bone_id = itr_key_child.get_int("parent");
			}

			if (itr_key_child._name == "bone_ref") {
				mainline_key._bone_refs.Add(object_ref);
				hash.Append(SPUtil.sprintf("(b_%d-%d)",object_ref._id,object_ref._parent_bone_id));

			} else if (itr_key_child._name == "object_ref") {
				object_ref._zindex = itr_key_child.get_int("z_index"); //I think this was a hack
				mainline_key._object_refs.Add(object_ref);
				hash.Append(SPUtil.sprintf("(o_%d)",object_ref._parent_bone_id));

			}
		}
		mainline_key._hash = SPUtil.md5(hash.ToString());
		mainline_key._start_time = itr_key.get_int("time");
		mainline_key._curve_type = itr_key.get_str("curve_type","quintic") == "instant" ? TGSpriterMainlineKey.CurveType.INSTANT : TGSpriterMainlineKey.CurveType.QUINTIC;
		spriter_animation._mainline_keys.Add(mainline_key);
	}

	private void handle_timeline(TGSpriterAnimation spriter_animation, TGSpriterConfigNode itr_animation_child) {
		TGSpriterTimeLine timeline = new TGSpriterTimeLine();
		timeline._id = itr_animation_child.get_id();
		timeline._name = itr_animation_child.get_str("name");

		for (int i_key = 0; i_key < itr_animation_child._children.Count; i_key++) {
			TGSpriterConfigNode key = itr_animation_child._children[i_key];
			for (int i_object = 0; i_object < key._children.Count; i_object++) {
				TGSpriterConfigNode itr_object = key._children[i_object];
				TGSpriterTimelineKey timeline_key = new TGSpriterTimelineKey();

				timeline_key._folder = itr_object.get_int("folder");
				timeline_key._file = itr_object.get_int("file");
				timeline_key._position = new Vector2(
					itr_object.get_val("x"), itr_object.get_val("y")
				);

				timeline_key._anchorPoint = new Vector2(
					itr_object.get_val("pivot_x",0.0f), itr_object.get_val("pivot_y",1.0f)
				);

				timeline_key._scaleX = itr_object.get_val("scale_x",1.0f);
				timeline_key._scaleY = itr_object.get_val("scale_y",1.0f);
				timeline_key._startsAt = key.get_int("time",0);
				timeline_key._rotation = itr_object.get_val("angle");
				timeline_key._spin = itr_object.get_int("spin",1);
				timeline_key._alpha = itr_object.get_val("a",1.0f);

				timeline.add_key_frame(timeline_key);
			}
		}
		spriter_animation._timelines[timeline._id] = timeline;
	}
}