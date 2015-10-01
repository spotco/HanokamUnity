﻿using UnityEngine;
using System.Collections.Generic;


public class MultiList<TKey, TValue> {
	public Dictionary<TKey,List<TValue>> _key_to_list = new Dictionary<TKey, List<TValue>>();
	public int count_of(TKey key) {
		if (!_key_to_list.ContainsKey(key)) _key_to_list[key] = new List<TValue>();
		return _key_to_list[key].Count;
	}
	public void add(TKey key, TValue val) {
		if (!_key_to_list.ContainsKey(key)) _key_to_list[key] = new List<TValue>();
		_key_to_list[key].Add(val);
	}
	public void clear(TKey key) {
		if (!_key_to_list.ContainsKey(key)) _key_to_list[key] = new List<TValue>();
		_key_to_list[key].Clear();
	}
	public List<TValue> list(TKey key) {
		if (!_key_to_list.ContainsKey(key)) _key_to_list[key] = new List<TValue>();
		return _key_to_list[key];
	}
	public List<TKey> keys() {
		return new List<TKey>(_key_to_list.Keys);
	}
}

public interface GenericPooledObject {
	void depool();
	void repool();
}

public class ObjectPool : SPBaseBehavior {

	public static ObjectPool inst() { return GameMain._context._objpool; }

	public static ObjectPool cons() {
		GameObject neu_obj = new GameObject("ObjectPool");
		return neu_obj.AddComponent<ObjectPool>().i_cons();	
	}

	private MultiList<string,SPBaseBehavior> _spbasebehavior_typekey_to_objlist = new MultiList<string, SPBaseBehavior>();

	public ObjectPool i_cons() {
		return this;
	}

	public T spbasebehavior_depool<T>() where T : SPBaseBehavior {
		string key = typeof(T).ToString();
		List<SPBaseBehavior> tar_list = _spbasebehavior_typekey_to_objlist.list(key);
		if (tar_list.Count == 0) {
			GameObject neu_obj = new GameObject(key);
			neu_obj.AddComponent<T>();
			neu_obj.SetActive(true);
			neu_obj.GetComponent<T>().depool();
			return neu_obj.GetComponent<T>();

		} else {
			T rtv = (T)tar_list[0];
			tar_list.RemoveRange(0,1);
			rtv.gameObject.SetActive(true);
			((T)rtv).depool();
			return (T)rtv;
		}
	}

	public void spbasebehavior_repool<T>(T obj) where T : SPBaseBehavior {
		string key = typeof(T).ToString();
		obj.repool();
		obj.gameObject.SetActive(false);
		obj.gameObject.transform.parent = this.transform;
		_spbasebehavior_typekey_to_objlist.list(key).Add(obj);
	}


	private MultiList<string,GenericPooledObject> _generic_typekey_to_objlist = new MultiList<string, GenericPooledObject>();
	public T generic_depool<T>() where T : GenericPooledObject {
		string key = typeof(T).ToString();
		List<GenericPooledObject> tar_list = _generic_typekey_to_objlist.list(key);
		if (tar_list.Count == 0) {
			T rtv = System.Activator.CreateInstance<T>();
			rtv.depool();
			return rtv;
		} else {
			T rtv = (T)tar_list[0];
			tar_list.RemoveRange(0,1);
			rtv.depool();
			return rtv;
		}
	}

	public void generic_repool<T>(T obj) where T : GenericPooledObject {
		string key = typeof(T).ToString();
		obj.repool();
		_generic_typekey_to_objlist.list(key).Add(obj);
	}
}
