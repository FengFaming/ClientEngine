/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:xml角色场景
 * Time:2020/8/27 14:21:25
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class XmlRoleScene : ISceneWithLightmap
{
	public XmlRoleScene(string name) : base(name)
	{

	}

	protected override void LoadLightmapEnd(string name, object t)
	{
		Light light = GameObject.FindObjectOfType<Light>();
		light.cullingMask = light.cullingMask & ~(1 << LayerMask.NameToLayer("NoLight"));
		base.LoadLightmapEnd(name, t);

		UIManager.Instance.OpenUI("UIPnlBackGameMain", UILayer.Pnl);

		ResObjectCallBackBase cb = new ResObjectCallBackBase();
		cb.m_LoadType = ResObjectType.GameObject;
		cb.m_FinshFunction = LoadPPEnd;
		ResObjectManager.Instance.LoadObject("dimian", ResObjectType.GameObject, cb);
	}

	private void LoadPPEnd(object t)
	{
		GameObject go = t as GameObject;
		go.AddComponent<TestLightmapData>();
		EngineTools.Instance.SetTargetLayer(go, LayerMask.NameToLayer("NoLight"));
		go.SetActive(true);

		CreateCharacter();
	}

	/// <summary>
	/// 创建主角
	/// </summary>
	private void CreateCharacter()
	{
		GameObject go = new GameObject();
		go.name = "Player";
		go.transform.position = Vector3.zero;
		go.transform.rotation = Quaternion.Euler(Vector3.zero);
		go.transform.localScale = Vector3.one;
		XmlRoleBase xml = go.AddComponent<XmlRoleBase>();
		LoadCharacter load = new LoadCharacter();
		load.m_Owner = xml;
		load.m_Action = (XmlRoleBase xb) =>
		{
			xb.SetUID(10001, 1000001);
		};
		ResObjectManager.Instance.LoadObject("ch_pc_hou", ResObjectType.GameObject, load);
		//ReloadingPlayer ch = go.AddComponent<ReloadingPlayer>();
		//CharacterXmlControl xml = new CharacterXmlControl("2312003");
		//ConfigurationManager.Instance.LoadXml(ref xml);
		//ch.LoadInfos = xml.m_LoadInfos;
		//ch.ShowMaterial = "Legacy Shaders/Diffuse";
		//ch.StartInitCharacter("ch_pc_hou", GetCharacter);
	}

	private class LoadCharacter : IResObjectCallBack
	{
		public XmlRoleBase m_Owner;
		public Action<XmlRoleBase> m_Action;

		public LoadCharacter() : base()
		{

		}

		public override void HandleLoadCallBack(object t)
		{
			GameObject parent = t as GameObject;
			parent.transform.parent = m_Owner.gameObject.transform;
			parent.transform.localPosition = Vector3.zero;
			parent.transform.localEulerAngles = new Vector3(0, -180, 0);
			parent.transform.localScale = Vector3.one;
			m_Owner.ControlParent = parent;
			if (m_Action != null)
			{
				m_Action(m_Owner);
			}
		}

		public override int LoadCallbackPriority()
		{
			return 0;
		}
	}
}
