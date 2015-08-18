using UnityEngine;
using System.Collections.Generic;

public class GameMain : SPBaseBehavior {

	public static GameMain _context;

	[SerializeField] private Camera _game_camera;
	[System.NonSerialized] public ObjectPool _objpool;

	private SPScene _current_scene;
	[System.NonSerialized] public TextureResource _tex_resc;
	[System.NonSerialized] public FileCache _file_cache;

	public override void Start () {
		_context = this;
		_objpool = ObjectPool.cons();
		_tex_resc = TextureResource.cons();
		_file_cache = FileCache.cons();

		SpriterData.cons_data_from_spritesheetreaders(
			new List<SpriteSheetReader>{ 
				SpriterJSONParser.cons_from_texture_and_file(
					_tex_resc.get_tex(RTex.SPRITER_OLDMAN),
					RSpr.OLDMAN
				) 
			},
			RSpr.OLDMAN
		);

		_current_scene = GameEngineScene.cons();
	}

	public override void Update () {
		float dt_scale = (1/60.0f)/(Time.deltaTime);
		_current_scene.i_update(dt_scale);
	}
}
