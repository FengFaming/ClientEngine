/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:配置文件生成角色
 * Time:2020/8/27 9:04:08
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class XmlRoleBase : IRole
{
	/// <summary>
	/// 控制父节点
	/// </summary>
	private GameObject m_ControlParent;
	public GameObject ControlParent { set { m_ControlParent = value; } }

	/// <summary>
	/// 需要合并得内容
	/// </summary>
	private Dictionary<string, GameObject> m_Combines;

	/// <summary>
	/// 需要加载的内容
	/// </summary>
	private int m_NeedsLoadCout;

	protected override void Awake()
	{
		base.Awake();
		m_ControlParent = null;
		m_Combines = new Dictionary<string, GameObject>();
		m_Combines.Clear();
		m_NeedsLoadCout = 0;
	}

	/// <summary>
	/// 设置对应内容
	/// </summary>
	/// <param name="uid"></param>
	/// <param name="xmlID"></param>
	public override void SetUID(int uid, int xmlID)
	{
		base.SetUID(uid, xmlID);
		XmlRoleConfig cf = new XmlRoleConfig(xmlID.ToString());
		ConfigurationManager.Instance.LoadXml<XmlRoleConfig>(ref cf);
		LoadCombineGameObject(cf.m_AllNodeDic);
		RoleAnimationManager animation = new RoleAnimationManager(this);
		foreach (KeyValuePair<string, string> kv in cf.m_AllAnimation)
		{
			IRoleAnimation r = ReflexManager.Instance.CreateClass<IRoleAnimation>(kv.Value, kv.Key);
			if (r != null)
			{
				animation.AddRoleAnimation(r);
			}
		}

		IRoleStateManager state = new IRoleStateManager();
		foreach (KeyValuePair<int, XmlRoleConfig.StateInfo> kv in cf.m_StateDic)
		{
			IRoleState s = ReflexManager.Instance.CreateClass<IRoleState>(kv.Value.m_ControlName, kv.Key);
			if (s != null)
			{
				s.AnimationManager = animation;
				s.Loop = kv.Value.m_IsLoop;

				if (!string.IsNullOrEmpty(kv.Value.m_ExitName))
				{
					s.ExitAction = delegate (IObjectState ios)
					{
						System.Reflection.MethodInfo m = ReflexManager.Instance.GetMethodInfo(s.GetType(), kv.Value.m_ExitName);
						ReflexManager.Instance.InvkMethod(s, m, ios);
					};
				}

				state.AddState(s);
			}
		}

		IRoleSkillManager sk = new IRoleSkillManager(this);
		foreach (KeyValuePair<int, XmlRoleConfig.SkillInfo> kv in cf.m_SkillDic)
		{
			IRoleSkill s = ReflexManager.Instance.CreateClass<IRoleSkill>(kv.Value.m_SkillControl, kv.Key);
			if (s != null)
			{
				s.StateID = kv.Value.m_StateID;
				s.ExitAction = delegate (IRoleSkill skill)
				{
					System.Reflection.MethodInfo m = ReflexManager.Instance.GetMethodInfo(s.GetType(), kv.Value.m_SkillExit);
					ReflexManager.Instance.InvkMethod(s, m, skill);
				};

				sk.AddSkill(s);
			}
		}

		InitRole(animation, state, sk);
		m_StateManager.StartState(cf.m_InitState);
	}

	/// <summary>
	/// 加载所有合并节点
	/// </summary>
	/// <param name="kvs"></param>
	private void LoadCombineGameObject(Dictionary<string, XmlRoleConfig.NodeInfo> kvs)
	{
		m_NeedsLoadCout = kvs.Count;

		foreach (KeyValuePair<string, XmlRoleConfig.NodeInfo> item in kvs)
		{
			LoadCombineClass l = new LoadCombineClass();
			l.m_LoadKey = item.Key;
			l.m_Action = LoadCombineEnd;
			l.m_Position = item.Value.m_Position;
			l.m_Rotation = item.Value.m_Rotation;
			l.m_Scale = item.Value.m_Scale;
			ResObjectManager.Instance.LoadObject(item.Value.m_NodePrefab, ResObjectType.GameObject, l);
		}
	}

	/// <summary>
	/// 加载完成
	/// </summary>
	/// <param name="k"></param>
	/// <param name="p"></param>
	/// <param name="r"></param>
	/// <param name="s"></param>
	/// <param name="g"></param>
	private void LoadCombineEnd(string k, Vector3 p, Vector3 r, Vector3 s, GameObject g)
	{
		if (m_Combines.ContainsKey(k))
		{
			GameObject.Destroy(g);
		}
		else
		{
			g.transform.parent = m_ControlParent.transform;
			g.transform.localPosition = p;
			g.transform.localRotation = Quaternion.Euler(r);
			g.transform.localScale = s;
			m_Combines.Add(k, g);
		}

		m_NeedsLoadCout--;
		if (m_NeedsLoadCout <= 0)
		{
			RoleCombineMeshControl com = new RoleCombineMeshControl();
			List<GameObject> games = new List<GameObject>();
			games.AddRange(m_Combines.Values);
			com.CombineMesh(games, ref m_ControlParent);
		}
	}

	#region 其他内容
	/// <summary>
	/// 加载完毕
	/// </summary>
	public class LoadCombineClass : IResObjectCallBack
	{
		public string m_LoadKey;
		public Vector3 m_Position, m_Rotation, m_Scale;
		public Action<string, Vector3, Vector3, Vector3, GameObject> m_Action;

		public LoadCombineClass() : base()
		{

		}

		public override void HandleLoadCallBack(object t)
		{
			if (m_Action != null && t != null && t is GameObject)
			{
				m_Action(m_LoadKey, m_Position, m_Rotation, m_Scale, t as GameObject);
			}
		}

		public override int LoadCallbackPriority()
		{
			return 0;
		}
	}
	#endregion
}
