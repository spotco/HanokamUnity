using UnityEngine;
using System.Collections.Generic;

public class SpriterNode : SPNode {
	private class SPNode_Bone : SPNode {
		public int _timeline_id;
		public new static SPNode_Bone cons() { return SPNode.generic_cons<SPNode_Bone>(); }
		public override void repool() { SPNode.generic_repool<SPNode_Bone>(this); }
	}
	
	private class SPNode_Object : SPNode {
		public int _timeline_id, _zindex;
		public new static SPNode_Object cons() { return SPNode.generic_cons<SPNode_Object>(); }
		public override void repool() { SPNode.generic_repool<SPNode_Object>(this); }
	}

	private SpriterData _data;
	private Dictionary<int,SPNode_Bone> _bones;
	private Dictionary<int,SPNode_Object> _objs;

	private Dictionary<int,SPNode_Bone> _unused_bones;
	private Dictionary<int,SPNode_Object> _unused_objs;

	private SPNode_Bone _root_bone;
	private SPNode _root_bone_holder;

	private string _current_anim_name;
	private int _anim_duration;
	private bool _repeat_anim, _anim_finished;

	private string _on_finish_play_anim, _current_playing;
	private long _last_bone_structure_hash;

}

/*
@implementation SpriterNode {
	SpriterData *_data;
	NSMutableDictionary *_bones;
	NSMutableDictionary *_objs;
	
	NSMutableDictionary *_unused_bones;
	NSMutableDictionary *_unused_objs;
	
	CCNode_Bone *_root_bone;
	CCNode *_root_bone_holder;
	CCRenderTexture *_render_target;
	
	NSString *_current_anim_name;
	
	float _current_anim_time;
	int _anim_duration;
	BOOL _repeat_anim;
	BOOL _anim_finished;
	
	NSString *_on_finish_play_anim, *_current_playing;
	
	long _last_bone_structure_hash;
	
	CGPoint _render_placement;
}

-(BOOL)current_anim_repeating { return _repeat_anim; }
-(BOOL)current_anim_finished { return _anim_finished; }

-(void)set_render_placement:(CGPoint)placement {
	_render_placement = placement;
}

+(SpriterNode*)nodeFromData:(SpriterData*)data render_size:(CGPoint)pt {
	return [[SpriterNode node] initFromData:data render_size:pt];
}
-(SpriterNode*)initFromData:(SpriterData*)data render_size:(CGPoint)pt {
	_data = data;
	_bones = [NSMutableDictionary dictionary];
	_objs = [NSMutableDictionary dictionary];
	_root_bone = NULL;
	
	_unused_bones = [NSMutableDictionary dictionary];
	_unused_objs = [NSMutableDictionary dictionary];
	
	_root_bone_holder = [CCNode node];
	
	_render_placement = ccp(0.5,0.5);
	_render_target = [CCRenderTexture renderTextureWithWidth:pt.x height:pt.y];
	[self addChild:_render_target];
	[self setScale:0.5];
	
	return self;
}

-(void)p_play_anim:(NSString*)anim repeat:(BOOL)repeat {
	_on_finish_play_anim = NULL;
	if(_current_playing != anim) {
		_current_playing = anim;
		[self playAnim:anim repeat:repeat];
	}
}

-(void)p_play_anim:(NSString*)anim1 on_finish_anim:(NSString*)anim2 {
	_current_playing = anim1;
	[self playAnim:anim1 repeat:NO];
	_on_finish_play_anim = anim2;
}

-(void)playAnim:(NSString *)anim_name repeat:(BOOL)repeat {
	if (![_data anim_of_name:anim_name]) {
		NSLog(@"does not contain animation %@",anim_name);
		return;
	}
	_current_anim_time = 0;
	_current_anim_name = anim_name;
	_anim_duration = (int)[_data anim_of_name:anim_name]._duration;
	_repeat_anim = repeat;
	_anim_finished = NO;
	
	[self update_mainline_keyframes];
	[self update_timeline_keyframes];
}

-(void)update:(CCTime)delta {
	_current_anim_time += delta * 1000;
	if (_current_anim_time > _anim_duration) {
		if (_repeat_anim) {
			_current_anim_time = _current_anim_time-_anim_duration;
		} else {
			_current_anim_time = _anim_duration;
			_anim_finished = YES;
		}
	}
	[self update_mainline_keyframes];
	[self update_timeline_keyframes];
	
	//[_render_target clear:255 g:0 b:0 a:255];
	[_render_target clear:0 g:0 b:0 a:0];
	[_render_target begin];
	[_root_bone_holder setPosition:pct_of_obj(_render_target, _render_placement.x, _render_placement.y)];
	[_root_bone_holder visit];
	[_render_target end];
	
}

float get_t_for_keyframes(TGSpriterTimelineKey *keyframe_current, TGSpriterTimelineKey *keyframe_next, float _current_anim_time, float _anim_duration, bool _repeat_anim) {
	float t;
	if (keyframe_current.startsAt > keyframe_next.startsAt) {
		if (_repeat_anim) {
			t = clampf((_current_anim_time-keyframe_current.startsAt)/(_anim_duration-keyframe_current.startsAt)  + keyframe_next.startsAt, 0, 1);
		} else {
			t = 0;
			keyframe_next = keyframe_current;
		}

	} else {
		t = clampf((_current_anim_time-keyframe_current.startsAt)/(keyframe_next.startsAt-keyframe_current.startsAt),0,1);
	}
	return t;
}

-(void)update_timeline_keyframes {
	for (NSNumber *itr in _bones) {
		CCNode_Bone *itr_bone = _bones[itr];
		TGSpriterTimeline *timeline = [[_data anim_of_name:_current_anim_name] timeline_key_of_id:itr_bone._timeline_id];
		TGSpriterTimelineKey *keyframe_current = [timeline keyForTime:_current_anim_time];
		TGSpriterTimelineKey *keyframe_next = [timeline nextKeyForTime:_current_anim_time];
		
		float t = get_t_for_keyframes(keyframe_current, keyframe_next, _current_anim_time, _anim_duration, _repeat_anim);
		
		[self interpolate:itr_bone from:keyframe_current to:keyframe_next t:t];

	}
	for (NSNumber *itr in _objs) {
		CCSprite_Object *itr_obj = _objs[itr];
		TGSpriterTimeline *timeline = [[_data anim_of_name:_current_anim_name] timeline_key_of_id:itr_obj._timeline_id];
		
		TGSpriterTimelineKey *keyframe_current = [timeline keyForTime:_current_anim_time];
		TGSpriterTimelineKey *keyframe_next = [timeline nextKeyForTime:_current_anim_time];
		
		float t = get_t_for_keyframes(keyframe_current, keyframe_next, _current_anim_time, _anim_duration, _repeat_anim);
		[self interpolate:itr_obj from:keyframe_current to:keyframe_next t:t];
		
		TGSpriterFile *file = [_data file_for_folderid:keyframe_current.folder fileid:keyframe_current.file];
		itr_obj.texture = file._texture;
		itr_obj.textureRect = file._rect;
		itr_obj._globalSortOrder = itr_obj._zindex;
	}
}

-(CGPoint)get_root_chain_scale:(CCNode*)tar {
	float scfx = 1, scfy = 1;
	while (tar.parent != NULL) {
		tar = tar.parent;
		if (tar.class == [CCNode_Bone class]) {
			scfx *= tar.scaleX;
			scfy *= tar.scaleY;
		} else {
			break;
		}
	}
	return ccp(scfx,scfy);
}

-(void)interpolate:(CCNode*)node from:(TGSpriterTimelineKey*)from to:(TGSpriterTimelineKey*)to t:(float)t {
	CGPoint rcs = [self get_root_chain_scale:node];

	node.position = ccp(scubic_interp(from.position.x/rcs.x, to.position.x/rcs.x, t),scubic_interp(from.position.y/rcs.y, to.position.y/rcs.y, t));
	node.rotation = scubic_angular_interp(from.rotation, to.rotation, t);
	node.scaleX = scubic_interp(from.scaleX, to.scaleX, t);
	node.scaleY = scubic_interp(from.scaleY, to.scaleY, t);
	node.opacity = scubic_interp(from.alpha, to.alpha, t);
	node.anchorPoint = ccp(scubic_interp(from.anchorPoint.x, to.anchorPoint.x, t),scubic_interp(from.anchorPoint.y, to.anchorPoint.y, t));
}

-(void)update_mainline_keyframes {
	TGSpriterAnimation *anim = [_data anim_of_name:_current_anim_name];
	TGSpriterMainlineKey *mainline_key;
	for (int i = 0; i < [anim._mainline_keys count]; i++) {
		if ([anim nth_mainline_key:i]._start_time > _current_anim_time) {
			break;
		}
		mainline_key = [anim nth_mainline_key:i];
	}
	
	if (_last_bone_structure_hash != mainline_key._hash) {
		[self make_bone_hierarchy:mainline_key];
		[self attach_objects_to_bone_hierarchy:mainline_key];
		
		_last_bone_structure_hash = mainline_key._hash;
	}
}

-(void)make_bone_hierarchy:(TGSpriterMainlineKey*)mainline_key {
	NSMutableSet *unadded_bones = [NSMutableSet setWithSet:[_bones keySet]];
	
	for (int i = 0; i < mainline_key._bone_refs.count; i++) {
		TGSpriterObjectRef *bone_ref = [mainline_key nth_bone_ref:i];
		NSNumber *bone_ref_id = [NSNumber numberWithInt:bone_ref._id];
		if (![_bones objectForKey:bone_ref_id]) {
			if ([_unused_bones objectForKey:bone_ref_id] == NULL) {
				_bones[bone_ref_id] = [CCNode_Bone node];
				
			} else {
				_bones[bone_ref_id] = _unused_bones[bone_ref_id];
				[_unused_bones removeObjectForKey:bone_ref_id];
			}
			
		} else {
			[unadded_bones removeObject:bone_ref_id];
		}
		CCNode_Bone *itr_bone = _bones[bone_ref_id];
		itr_bone._timeline_id = bone_ref._timeline_id;
	}
	
	for (int i = 0; i < mainline_key._bone_refs.count; i++) {
		TGSpriterObjectRef *bone_ref = [mainline_key nth_bone_ref:i];
		NSNumber *bone_ref_id = [NSNumber numberWithInt:bone_ref._id];
		CCNode_Bone *itr_bone = _bones[bone_ref_id];
		
		[itr_bone removeFromParent];
		if (bone_ref._is_root) {
			_root_bone = itr_bone;
			[_root_bone_holder addChild:_root_bone];
		} else {
			CCNode_Bone *itr_bone_parent = _bones[[NSNumber numberWithInt:bone_ref._parent_bone_id]];
			[itr_bone_parent addChild:itr_bone];
		}
		
	}
	
	for (NSNumber *itr in unadded_bones) {
		CCNode_Bone *itr_bone = _bones[itr];
		[itr_bone removeFromParent];
		[_bones removeObjectForKey:itr];
		_unused_bones[itr] = itr_bone;
	}
}

-(void)attach_objects_to_bone_hierarchy:(TGSpriterMainlineKey*)mainline_key {
	NSMutableSet *unadded_objects = [NSMutableSet setWithSet:[_objs keySet]];
	for (int i = 0; i < mainline_key._object_refs.count; i++) {
		TGSpriterObjectRef *obj_ref = [mainline_key nth_object_ref:i];
		NSNumber *obj_ref_id = [NSNumber numberWithInt:obj_ref._id];
		if (![_objs objectForKey:obj_ref_id]) {
			if ([_unused_objs objectForKey:obj_ref_id] == NULL) {
				_objs[obj_ref_id] = [CCSprite_Object node];
			} else {
				_objs[obj_ref_id] = _unused_objs[obj_ref_id];
				[_unused_objs removeObjectForKey:obj_ref_id];
			}
		} else {
			[unadded_objects removeObject:obj_ref_id];
		}
		CCSprite_Object *itr_obj = _objs[obj_ref_id];
		[itr_obj removeFromParent];
		itr_obj._timeline_id = obj_ref._timeline_id;
		itr_obj._zindex = obj_ref._zindex;
		
		CCNode_Bone *itr_bone_parent = _bones[[NSNumber numberWithInt:obj_ref._parent_bone_id]];
		[itr_bone_parent addChild:itr_obj z:itr_obj._zindex];
	}
	
	for (NSNumber *itr in unadded_objects) {
		CCSprite_Object *itr_objs = _objs[itr];
		[itr_objs removeFromParent];
		[_objs removeObjectForKey:itr];
		_unused_objs[itr] = itr_objs;
	}
}

*/
