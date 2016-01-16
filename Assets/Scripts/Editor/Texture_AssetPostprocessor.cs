using UnityEngine;
using UnityEditor;
using System.Collections;

public class Texture_AssetPostprocessor : AssetPostprocessor {
	void OnPreprocessTexture () {
		TextureImporter textureImporter  = (TextureImporter) assetImporter;
		textureImporter.mipmapEnabled = false;
		textureImporter.filterMode = FilterMode.Point;
	}
}
