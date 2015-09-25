using UnityEngine;
using System.Collections.Generic;

public class GameMain : SPBaseBehavior {

	/**
	TODO--
	build textures to resources
	http://answers.unity3d.com/questions/984854/is-it-possible-to-excluding-streamingassets-depend.html

	village mode
	dive mode
	bounding box debug render
	air mode
	gameui

	gameui
	dialogui
	**/

	public static GameMain _context;

	[SerializeField] public Camera _game_camera;
	[SerializeField] public Camera _overlay_camera;
	[System.NonSerialized] public ObjectPool _objpool;

	private SPScene _current_scene;
	[System.NonSerialized] public TextureResource _tex_resc;
	public FileCache _file_cache;

	private const float ROOT_SCF = 0.1f;

	public override void Start () {

		Application.targetFrameRate = 30;
		this.transform.localScale = new Vector3(ROOT_SCF,ROOT_SCF,ROOT_SCF);

		_context = this;
		_objpool = ObjectPool.cons();
		_tex_resc = TextureResource.cons();
		_file_cache = FileCache.cons();

		float tar_x_div_y = 414.0f / 736.0f;
		float actual_x_div_y = SPUtil.game_screen().x / SPUtil.game_screen().y;
		float norm_game_width = tar_x_div_y / actual_x_div_y;
		_game_camera.rect = new Rect((1-norm_game_width)/2.0f,0,norm_game_width,1.0f);

		_game_camera.cullingMask = int.MaxValue 
			& (~(1 << RLayer.get_layer(RLayer.SPRITER_NODE)))
			& (~(1 << RLayer.get_layer(RLayer.SURFACEREFLECTION_ONLY)))
		;

		_current_scene = GameEngineScene.cons();
	}

	public override void Update () {
		float dt_scale = (Time.deltaTime)/(1/60.0f);
		_current_scene.i_update(dt_scale);
	}
}
