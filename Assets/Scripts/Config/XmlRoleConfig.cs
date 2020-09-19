/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:xml控制角色配置文件
 * Time:2020/8/27 9:13:28
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using System.Xml;

public class XmlRoleConfig : XmlBase
{
	/// <summary>
	/// 加载节点
	/// </summary>
	public class NodeInfo
	{
		public string m_NodePrefab;
		public Vector3 m_Position;
		public Vector3 m_Rotation;
		public Vector3 m_Scale;
	}

	/// <summary>
	/// 状态详细
	/// </summary>
	public class StateInfo
	{
		public List<string> m_Animations;
		public float m_SpValue;
		public bool m_IsLoop;
		public string m_ExitName;
		public string m_ControlName;
	}

	/// <summary>
	/// 技能详细
	/// </summary>
	public class SkillInfo
	{
		/// <summary>
		/// 技能控制
		/// </summary>
		public string m_SkillControl;

		/// <summary>
		/// 技能状态
		/// </summary>
		public int m_StateID;

		/// <summary>
		/// 技能退出
		/// </summary>
		public string m_SkillExit;
	}

	public XmlRoleConfig(string name) : base(name)
	{
		m_AllNodeDic = new Dictionary<string, NodeInfo>();
		m_AllNodeDic.Clear();

		m_AllAnimation = new Dictionary<string, string>();
		m_AllAnimation.Clear();

		m_StateDic = new Dictionary<int, StateInfo>();
		m_StateDic.Clear();

		m_SkillDic = new Dictionary<int, SkillInfo>();
		m_SkillDic.Clear();
	}

	/// <summary>
	/// 所有需要加载的地步节点
	/// </summary>
	public Dictionary<string, NodeInfo> m_AllNodeDic;

	/// <summary>
	/// 所有的动作
	/// </summary>
	public Dictionary<string, string> m_AllAnimation;

	/// <summary>
	/// 所有状态
	/// </summary>
	public Dictionary<int, StateInfo> m_StateDic;

	/// <summary>
	/// 所有技能
	/// </summary>
	public Dictionary<int, SkillInfo> m_SkillDic;

	/// <summary>
	/// 初始化后的状态
	/// </summary>
	public int m_InitState;

	public override bool LoadXml(XmlElement node)
	{
		if (!base.LoadXml(node))
			return false;

		m_InitState = int.Parse(node.GetAttribute("Idle"));

		foreach (XmlElement item in node.ChildNodes)
		{
			////加载节点
			if (item.Name == "Loads")
			{
				foreach (XmlElement l in item.ChildNodes)
				{
					string key = l.GetAttribute("Key");
					if (!m_AllNodeDic.ContainsKey(key))
					{
						NodeInfo info = new NodeInfo();
						info.m_NodePrefab = l.GetAttribute("Name");
						info.m_Position = EngineTools.Instance.StringToVector3(l.GetAttribute("Position"));
						info.m_Rotation = EngineTools.Instance.StringToVector3(l.GetAttribute("Rotation"));
						info.m_Scale = EngineTools.Instance.StringToVector3(l.GetAttribute("Scale"));
						m_AllNodeDic.Add(key, info);
					}
				}
			}

			///加载动画
			if (item.Name == "Animations")
			{
				foreach (XmlElement l in item.ChildNodes)
				{
					string key = l.GetAttribute("Name");
					if (!m_AllAnimation.ContainsKey(key))
					{
						string control = l.GetAttribute("Control");
						m_AllAnimation.Add(key, control);
					}
				}
			}

			///加载状态
			if (item.Name == "States")
			{
				foreach (XmlElement l in item.ChildNodes)
				{
					int id = int.Parse(l.GetAttribute("ID"));
					if (!m_StateDic.ContainsKey(id))
					{
						StateInfo info = new StateInfo();
						info.m_Animations = new List<string>();
						string ans = l.GetAttribute("Ans");
						string[] a = ans.Split(',');
						foreach (string s in a)
						{
							info.m_Animations.Add(s);
						}

						info.m_IsLoop = bool.Parse(l.GetAttribute("Loop"));
						info.m_SpValue = float.Parse(l.GetAttribute("Speed"));
						info.m_ExitName = l.GetAttribute("Exit");
						info.m_ControlName = l.GetAttribute("Control");
						m_StateDic.Add(id, info);
					}
				}
			}

			if (item.Name == "Skills")
			{
				foreach (XmlElement l in item.ChildNodes)
				{
					int id = int.Parse(l.GetAttribute("ID"));
					if (!m_SkillDic.ContainsKey(id))
					{
						SkillInfo info = new SkillInfo();
						info.m_SkillControl = l.GetAttribute("Control");
						info.m_StateID = int.Parse(l.GetAttribute("State"));
						info.m_SkillExit = l.GetAttribute("Exit");
						m_SkillDic.Add(id, info);
					}
				}
			}
		}

		return true;
	}
}
