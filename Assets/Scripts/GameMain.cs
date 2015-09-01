using UnityEngine;
using System.Collections.Generic;

public class GameMain : SPBaseBehavior {

	//game scene implementation
	//village gameplay
	//dive gameplay
	//air gameplay
	//figure out UI
	//letterbox area

	public static GameMain _context;

	[SerializeField] public Camera _game_camera;
	[System.NonSerialized] public ObjectPool _objpool;

	private SPScene _current_scene;
	[System.NonSerialized] public TextureResource _tex_resc;
	[System.NonSerialized] public FileCache _file_cache;

	private const float ROOT_SCF = 0.001f;

	public override void Start () {
		Application.targetFrameRate = 30;
		this.transform.localScale = new Vector3(ROOT_SCF,ROOT_SCF,1.0f);

		_context = this;
		_objpool = ObjectPool.cons();
		_tex_resc = TextureResource.cons();
		_file_cache = FileCache.cons();

		_game_camera.cullingMask = int.MaxValue & (~(1 << LayerMask.NameToLayer("SpriterNode")));

		_current_scene = GameEngineScene.cons();
	}

	public override void Update () {
		float dt_scale = (1/60.0f)/(Time.deltaTime);
		_current_scene.i_update(dt_scale);
	}
}
