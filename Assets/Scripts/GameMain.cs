using UnityEngine;
using System.Collections;

public class GameMain : MonoBehaviour {

	public static GameMain _context;

	[SerializeField] private Camera _game_camera;
	private SPScene _current_scene;
	[System.NonSerialized] public TextureResource _tex_resc;
	[System.NonSerialized] public FileCache _file_cache;

	public void Start () {
		_context = this;
		_tex_resc = TextureResource.cons();
		_file_cache = FileCache.cons();
	
	
		_current_scene = GameEngineScene.cons();
	}

	public void Update () {
		float dt_scale = (1/60.0f)/(Time.deltaTime);
		_current_scene.i_update(dt_scale);
	}
}
