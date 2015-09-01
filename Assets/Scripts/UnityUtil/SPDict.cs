using UnityEngine;
using System.Collections.Generic;

public class SPDict<TKey,TValue> {

	private Dictionary<TKey,TValue> _dict;
	private List<TKey> _list;

	public SPDict() {
		_dict = new Dictionary<TKey, TValue>();
		_list = new List<TKey>();
	}

	public object this[TKey i]
	{
		get { 
			return _dict[i]; 
		}
		set { 
			_dict[i] = (TValue)value;
			if (!_list.Contains(i)) {
				_list.Remove(i);
			}
		}
	}

	public void remove(TKey i) {
		_dict.Remove(i);
		_list.Remove(i);
	}

	public void clear() {
		_dict.Clear();
		_list.Clear();
	}

	public List<TKey> key_itr() {
		return _list;
	}

}
