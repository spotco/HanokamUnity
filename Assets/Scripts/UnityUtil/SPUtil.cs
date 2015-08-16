using UnityEngine;
using System.Collections;

public class SPUtil : MonoBehaviour {

	public static string sprintf(string format ,params object[] varargs) {
		return AT.MIN.Tools.sprintf(format,varargs);
	}

	public static void logf(string format ,params object[] varargs) {
		Debug.Log(SPUtil.sprintf(format,varargs));
	}

	public static byte[] streaming_asset_load(string filePath) {
		byte[] result;
		if (filePath.Contains("://")) {
			WWW www = new WWW(filePath);
			while(!www.isDone);
			result = www.bytes;
		} else {
			result = System.IO.File.ReadAllBytes(filePath);
		}
		return result;
	}
}
