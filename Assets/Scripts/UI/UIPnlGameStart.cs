/*
 * Creator:ffm
 * Desc:游戏开始界面
 * Time:2020/4/9 8:43:18
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;

public class UIPnlGameStart : IUIModelControl
{
	public UIPnlGameStart()
	{
		m_ModelObjectPath = "UIPnlGameStart";
		m_IsOnlyOne = true;
	}

	public override void OpenSelf(GameObject target)
	{
		base.OpenSelf(target);
		Button bt = m_ControlTarget.transform.Find("Button").GetComponent<Button>();
		bt.onClick.AddListener(OnClickGameStart);

		string key = string.Format(EngineMessageHead.NET_CLIENT_MESSAGE_HEAD, 100001);
		MessageManger.Instance.AddMessageListener(key, m_ControlTarget, ListenResponse);
	}

	private void ListenResponse(params object[] arms)
	{
		if (arms.Length > 0)
		{
			List<object> list = arms[0] as List<object>;
			if (list[0] is GetServerTimeResponse)
			{
				Debug.Log((list[0] as GetServerTimeResponse).ToString());
			}
		}
	}

	private void OnClickGameStart()
	{
		//StartGameRequest pack = new StartGameRequest();
		////pack.SetSendString("test");
		//GameNetManager.Instance.SendMessage<StartGameRequest>(pack, 1);
		GameSceneManager.Instance.ChangeScene(new GameMainScene());
	}
}
