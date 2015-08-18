using UnityEngine;
using System.Collections.Generic;
using System.Xml;

public class SpriterXMLParser {
	public static TGSpriterConfigNode parse_scml(string filepath) {
		//to reduce load times, could really optimize all of this
		XmlDocument xml_doc = new XmlDocument();
		string xmlData = System.Text.Encoding.Default.GetString(
			SPUtil.streaming_asset_load(
				System.IO.Path.Combine(Application.streamingAssetsPath, filepath+".scml"))
		);
		xml_doc.LoadXml(xmlData);

		XmlNode xml_root = SpriterXMLParser.bfs_trim_to_node(xml_doc,"spriter_data");
		return SpriterXMLParser.r_parse_scml(xml_root, null);
	}

	private static XmlNode bfs_trim_to_node(XmlNode node, string tar) {
		Queue<XmlNode> queue = new Queue<XmlNode>();
		queue.Enqueue(node);
		while (queue.Count > 0) {
			XmlNode itr = queue.Dequeue();
			if (itr.Name == tar) return itr;
			for (int i = 0; i < itr.ChildNodes.Count; i++) {
				queue.Enqueue(itr.ChildNodes[i]);
			}
		}
		return null;
	}

	public static TGSpriterConfigNode r_parse_scml(XmlNode node, TGSpriterConfigNode parent) {
		TGSpriterConfigNode rtv = new TGSpriterConfigNode();
		rtv._parent = parent;
		rtv._name = node.Name;
		rtv._value = node.Value;

		if (node.Attributes != null) {
			for (int i = 0; i < node.Attributes.Count; i++) {
				XmlAttribute itr = node.Attributes[i];
				rtv._properties[itr.Name] = itr.Value;
			}
		}
		for (int i = 0; i < node.ChildNodes.Count; i++) {
			XmlNode itr = node.ChildNodes[i];
			rtv._children.Add(SpriterXMLParser.r_parse_scml(itr,rtv));
		}
		return rtv;
	}
}

public class TGSpriterConfigNode {
	public string _name = "";
	public TGSpriterConfigNode _parent;
	public List<TGSpriterConfigNode> _children = new List<TGSpriterConfigNode>();
	public string _value = "";
	public Dictionary<string,string> _properties = new Dictionary<string, string>();
	
	public float get_val(string key, float default_val = 0) {
		return (_properties.ContainsKey(key))?float.Parse(_properties[key]):default_val;
	}
	public int get_int(string key, int default_val = 0) {
		return (_properties.ContainsKey(key))?int.Parse(_properties[key]):default_val;
	}
	public string get_str(string key, string default_val = "") {
		return (_properties.ContainsKey(key))?_properties[key]:default_val;
	}
	public int get_id() {
		return get_int("id");
	}
}
