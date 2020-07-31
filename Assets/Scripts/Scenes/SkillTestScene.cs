/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:技能测试场景
 * Time:2020/7/30 15:40:39
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class SkillTestScene : ISceneWithLightmap
{
	public SkillTestScene(string name) : base(name)
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
		ReloadingPlayer ch = go.AddComponent<ReloadingPlayer>();
		CharacterXmlControl xml = new CharacterXmlControl("2312003");
		ConfigurationManager.Instance.LoadXml(ref xml);
		ch.LoadInfos = xml.m_LoadInfos;
		ch.ShowMaterial = "Legacy Shaders/Diffuse";
		ch.StartInitCharacter("ch_pc_hou", GetCharacter);
	}

	private void GetCharacter(object t)
	{
		m_LoadAction(100);
		UIManager.Instance.OpenUI("UIPnlBackGameMain", UILayer.Pnl);
		ReloadingPlayer ch = t as ReloadingPlayer;
		CharacterXmlControl xml = new CharacterXmlControl("2312003");
		ConfigurationManager.Instance.LoadXml(ref xml);
		GameCharacterStateManager stateManager = new GameCharacterStateManager(ch);
		foreach (var info in xml.m_StateInfos)
		{
			CharacterStateBase state = ReflexManager.Instance.CreateClass(info.Value.m_Control, info.Key) as CharacterStateBase;
			if (state != null)
			{
				for (int index = 0; index < info.Value.m_Paramters.Count; index++)
				{
					state.AddChangeStateParameter(info.Value.m_Paramters[index]);
				}
			}

			stateManager.AddManagerState(state);
		}

		CharacterMountControl mount = new CharacterMountControl(ch, ch.transform.GetChild(0).gameObject);
		List<MountInfo> infos = new List<MountInfo>();
		infos.AddRange(xml.m_MountInfos.Values);
		mount.AddMountInfo(infos);
		ch.InitCharacter(null, null, null, stateManager, mount);
		ch.SetCameraTra(new Vector3(0, 2, -10), Vector3.zero, Vector3.one);

		UIManager.Instance.OpenUI("UIPnlReloadingControl", UILayer.Pnl, ch, "2312003");
	}
}
