/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:换装场景
 * Time:2020/5/11 14:13:43
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class ReloadingScene : IScene
{
	public class AnimationSceneControl : ObjectBase
	{
		private Action<float> m_LoadAction;

		/// <summary>
		/// 场景目标
		/// </summary>
		private ReloadingPlayer m_SceneTarget;

		public void ClearData()
		{
			if (m_SceneTarget != null)
			{
				m_SceneTarget.Clear();
			}

			m_SceneTarget = null;
		}

		public void LoadScene(Action<float> action)
		{
			m_LoadAction = action;
			StartCoroutine("StartLoadScene");
		}

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
			ch.StartInitCharacter("ch_pc_hou", GetCharacter);
			m_SceneTarget = ch;
		}

		private void GetCharacter(object t)
		{
			m_LoadAction(100);
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

		private IEnumerator StartLoadScene()
		{
			yield return null;
			CreateCharacter();
			yield return null;
			UIManager.Instance.OpenUI("UIPnlBackGameMain", UILayer.Pnl);
		}
	}

	private AnimationSceneControl m_LoadControl;

	public ReloadingScene(string name) : base(name)
	{

	}

	public override void ClearSceneData()
	{
		base.ClearSceneData();
		if (m_LoadControl != null)
		{
			m_LoadControl.ClearData();
		}
	}

	public override void LoadScene(Action<float> action)
	{
		if (m_LoadControl != null)
		{
			m_LoadControl.LoadScene(action);
		}
	}

	public override bool InitScene()
	{
		if (!base.InitScene())
			return false;

		///为什么要在这里创建，因为场景切换的过程当中会删除掉一部分
		GameObject scene = new GameObject();
		scene.name = m_SceneName;
		scene.gameObject.transform.position = Vector3.zero;
		scene.gameObject.transform.eulerAngles = Vector3.zero;
		scene.gameObject.transform.localScale = Vector3.one;
		m_LoadControl = scene.gameObject.AddComponent<AnimationSceneControl>();
		return true;
	}
}
