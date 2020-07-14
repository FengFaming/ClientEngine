/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:角色配置文件
 * Time:2020/4/26 13:51:10
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using System.Xml;

public class CharacterXmlControl : XmlBase
{
	public struct StateInfo
	{
		public string m_Control;
		public List<KeyValuePair<string, bool>> m_Paramters;
	}

	public struct LoadInfo
	{
		public string m_Key;
		public string m_Model;
		public Vector3 m_Position;
		public Vector3 m_Rotation;
		public Vector3 m_Scale;
	}

	/// <summary>
	/// 状态
	/// </summary>
	public Dictionary<int, StateInfo> m_StateInfos;

	/// <summary>
	/// 挂载点
	/// </summary>
	public Dictionary<int, MountInfo> m_MountInfos;

	/// <summary>
	/// 加载列表
	/// </summary>
	public List<LoadInfo> m_LoadInfos;

	public override string ToString()
	{
		string str = base.ToString();
		str += string.Format("State:{0} Mount:{1} Load:{2}", m_StateInfos.Count, m_MountInfos.Count, m_LoadInfos.Count);
		return str;
	}

	public CharacterXmlControl(string name) : base(name)
	{
		m_StateInfos = new Dictionary<int, StateInfo>();
		m_StateInfos.Clear();

		m_MountInfos = new Dictionary<int, MountInfo>();
		m_MountInfos.Clear();

		m_LoadInfos = new List<LoadInfo>();
		m_LoadInfos.Clear();
	}

	public override bool LoadXml(XmlElement node)
	{
		if (!base.LoadXml(node))
			return false;

		foreach (XmlElement item in node.ChildNodes)
		{
			if (item.Name == "Loads")
			{
				foreach (XmlElement s in item.ChildNodes)
				{
					LoadInfo info = new LoadInfo();
					info.m_Key = s.GetAttribute("Key");
					info.m_Model = s.GetAttribute("Name");
					info.m_Position = EngineTools.Instance.StringToVector3(s.GetAttribute("Position"));
					info.m_Rotation = EngineTools.Instance.StringToVector3(s.GetAttribute("Rotation"));
					info.m_Scale = EngineTools.Instance.StringToVector3(s.GetAttribute("Scale"));
					m_LoadInfos.Add(info);
				}
			}

			if (item.Name == "States")
			{
				foreach (XmlElement s in item.ChildNodes)
				{
					int id = int.Parse(s.GetAttribute("ID"));
					string control = s.GetAttribute("Control");
					StateInfo info = new StateInfo();
					info.m_Control = control;
					info.m_Paramters = new List<KeyValuePair<string, bool>>();
					foreach (XmlElement p in s.ChildNodes)
					{
						string key = p.GetAttribute("Key");
						bool value = bool.Parse(p.GetAttribute("Value"));
						info.m_Paramters.Add(new KeyValuePair<string, bool>(key, value));
					}

					m_StateInfos.Add(id, info);
				}
			}

			if (item.Name == "Mount")
			{
				foreach (XmlElement s in item.ChildNodes)
				{
					MountInfo info = new MountInfo();
					info.m_MountIndex = int.Parse(s.GetAttribute("ID"));
					info.m_MountName = s.GetAttribute("Name");
					info.m_MountPosition = EngineTools.Instance.StringToVector3(s.GetAttribute("Position"));
					info.m_MountRotation = EngineTools.Instance.StringToVector3(s.GetAttribute("Rotation"));
					info.m_MountScale = EngineTools.Instance.StringToVector3(s.GetAttribute("Scale"));
					m_MountInfos.Add(info.m_MountIndex, info);
				}
			}
		}

		return true;
	}
}
