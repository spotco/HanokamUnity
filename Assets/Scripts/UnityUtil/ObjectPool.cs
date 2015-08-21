using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : SPBaseBehavior {

	public static ObjectPool inst() { return GameMain._context._objpool; }

	public static ObjectPool cons() {
		GameObject neu_obj = new GameObject("ObjectPool");
		return neu_obj.AddComponent<ObjectPool>().i_cons();	
	}

	private Dictionary<string,List<Object>> _typekey_to_objlist = new Dictionary<string,List<Object>>();

	public ObjectPool i_cons() {
		return this;
	}

	public T depool<T>() where T : SPBaseBehavior {
		string key = typeof(T).ToString();
		if (!_typekey_to_objlist.ContainsKey(key)) _typekey_to_objlist[key] = new List<Object>();
		List<Object> tar_list = _typekey_to_objlist[key];
		if (tar_list.Count == 0) {
			GameObject neu_obj = new GameObject(key);
			neu_obj.AddComponent<T>();
			neu_obj.SetActive(true);
			return neu_obj.GetComponent<T>();

		} else {
			T rtv = (T)tar_list[0];
			tar_list.RemoveRange(0,1);
			rtv.gameObject.SetActive(true);
			return (T)rtv;
		}
	}

	public void repool<T>(T obj) where T : SPBaseBehavior {
		string key = typeof(T).ToString();
		if (!_typekey_to_objlist.ContainsKey(key)) _typekey_to_objlist[key] = new List<Object>();
		obj.gameObject.SetActive(false);
		obj.gameObject.transform.parent = this.transform;
		_typekey_to_objlist[key].Add(obj);
	}

}
