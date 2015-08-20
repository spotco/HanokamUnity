using UnityEngine;
using System.Collections.Generic;

public class GameMain : SPBaseBehavior {

	public static GameMain _context;

	[SerializeField] public Camera _game_camera;
	[System.NonSerialized] public ObjectPool _objpool;

	private SPScene _current_scene;
	[System.NonSerialized] public TextureResource _tex_resc;
	[System.NonSerialized] public FileCache _file_cache;

	private const float ROOT_SCF = 0.001f;

	public override void Start () {
		this.transform.localScale = SPUtil.valv(ROOT_SCF);

		_context = this;
		_objpool = ObjectPool.cons();
		_tex_resc = TextureResource.cons();
		_file_cache = FileCache.cons();

		_current_scene = GameEngineScene.cons();
	}

	public override void Update () {
		float dt_scale = (1/60.0f)/(Time.deltaTime);
		_current_scene.i_update(dt_scale);
	}
}
