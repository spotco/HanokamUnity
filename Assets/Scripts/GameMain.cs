using UnityEngine;
using System.Collections.Generic;

public class GameMain : SPBaseBehavior {

	/**
	TODO--
	dive mode
	bounding box debug render
	air mode
	gameui

	gameui
	dialogui
	**/

	public static GameMain _context;

	[SerializeField] public Camera _game_camera;
	[SerializeField] public Camera _ui_camera;

	private RenderTexture _game_camera_out;
	private RenderTexture _ui_camera_out;

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
		Rect game_cam_rect = new Rect((1-norm_game_width)/2.0f,0,norm_game_width,1.0f);

		_game_camera_out = new RenderTexture(
			Screen.width,
			Screen.height,
			16,
			RenderTextureFormat.RGB565);
		_game_camera_out.Create();

		_ui_camera_out = new RenderTexture(
			Screen.width,
			Screen.height,
			16,
			RenderTextureFormat.ARGB32);
		_ui_camera_out.Create();

		_ui_camera.targetTexture = _ui_camera_out;
		_game_camera.targetTexture = _game_camera_out;

		SPSprite game_camera_out = SPSprite.cons_sprite_texkey_texrect(RTex.BLANK,new Rect(0,0,1,1));
		game_camera_out.set_u_pos(0,0);
		game_camera_out.manual_set_texture(_game_camera_out);
		game_camera_out.manual_set_mesh_size(
			Screen.width * game_cam_rect.width,
			Screen.height * game_cam_rect.height
		);
		game_camera_out.gameObject.layer = RLayer.get_layer(RLayer.OUTPUT);
		game_camera_out.set_name("game_camera_out");

		SPSprite ui_camera_out = SPSprite.cons_sprite_texkey_texrect(RTex.BLANK,new Rect(0,0,1,1));
		ui_camera_out.set_u_pos(0,0);
		ui_camera_out.manual_set_texture(_ui_camera_out);
		ui_camera_out.manual_set_mesh_size(
			Screen.width * game_cam_rect.width,
			Screen.height * game_cam_rect.height
		);
		ui_camera_out.gameObject.layer = RLayer.get_layer(RLayer.OUTPUT);
		ui_camera_out.set_name("ui_camera_out");

		game_camera_out.set_u_pos(0,0);
		ui_camera_out.set_u_pos(0,0);
		ui_camera_out.set_u_z(-1);
		game_camera_out.transform.parent = _overlay_camera.transform.parent;
		ui_camera_out.transform.parent = _overlay_camera.transform.parent;
		game_camera_out.set_scale(1);
		ui_camera_out.set_scale(1);
		_overlay_camera.transform.parent.localPosition = new Vector3(15000,0,0);

		Vector3 vtx_0_0 = _overlay_camera.WorldToScreenPoint(game_camera_out.w_pos_of_vertex(SPSprite.VTX_0_0));
		Vector3 vtx_0_1 = _overlay_camera.WorldToScreenPoint(game_camera_out.w_pos_of_vertex(SPSprite.VTX_0_1));

		_overlay_camera.transform.localPosition = new Vector3(0,0,-2);
		game_camera_out.set_scale(((float)Screen.height)/Mathf.Abs(vtx_0_0.y - vtx_0_1.y));
		ui_camera_out.set_scale(((float)Screen.height)/Mathf.Abs(vtx_0_0.y - vtx_0_1.y));

		_game_camera.rect = game_cam_rect;
		_ui_camera.rect = game_cam_rect;

		_game_camera.cullingMask = int.MaxValue 
			& (~(1 << RLayer.get_layer(RLayer.SPRITER_NODE)))
			& (~(1 << RLayer.get_layer(RLayer.SURFACEREFLECTION_ONLY)))
			& (~(1 << RLayer.get_layer(RLayer.UI)))
			& (~(1 << RLayer.get_layer(RLayer.OUTPUT)));

		_ui_camera.cullingMask = (1 << RLayer.get_layer(RLayer.UI));
		_overlay_camera.cullingMask = (1 << RLayer.get_layer(RLayer.OUTPUT));
		_current_scene = GameEngineScene.cons();
	}

	public override void Update () {
		float dt_scale = (Time.deltaTime)/(1/60.0f);
		_current_scene.i_update(dt_scale);
	}
}
