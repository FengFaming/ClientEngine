/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:查看叠加场景
 * Time:2020/7/8 10:44:20
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class LookAdditiveScene : ISceneWithAdditive
{
	public LookAdditiveScene(string name) : base(name)
	{

	}

	protected override void LoadSceneEnd()
	{
		base.LoadSceneEnd();
		CreateCharacter();
	}

	private void CreateCharacter()
	{
		GameObject go = new GameObject();
		go.name = "Player";
		go.transform.position = Vector3.zero;
		go.transform.rotation = Quaternion.Euler(Vector3.zero);
		go.transform.localScale = Vector3.one;
		AnimationPlayer ch = go.AddComponent<AnimationPlayer>();
		ch.StartInitCharacter("elephant", GetCharacter);
	}

	private void GetCharacter(object t)
	{
		AnimationPlayer ch = t as AnimationPlayer;

		CharacterXmlControl xml = new CharacterXmlControl("2312001");
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

		ch.InitCharacter(null, null, null, stateManager);
		ch.SetCameraTra(new Vector3(0, 2, -10), Vector3.zero, Vector3.one);
		UIManager.Instance.OpenUI("UIPnlBackGameMain", UILayer.Pnl, new Vector3(0, 240, 0));
	}
}
