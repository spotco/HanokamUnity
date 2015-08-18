using UnityEngine;
using System.Collections;

public class SPUtil {

	public static string sprintf(string format ,params object[] varargs) {
		return AT.MIN.Tools.sprintf(format,varargs);
	}

	public static void logf(string format ,params object[] varargs) {
		Debug.Log(SPUtil.sprintf(format,varargs));
	}

	private static System.Security.Cryptography.MD5 _md5 = System.Security.Cryptography.MD5.Create();
	public static long md5(string input) {
		byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
		byte[] hash = SPUtil._md5.ComputeHash(inputBytes);
		return System.BitConverter.ToInt64(hash,0);
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
