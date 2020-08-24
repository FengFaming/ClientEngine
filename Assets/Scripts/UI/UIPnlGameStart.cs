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
	private Button m_Check;

	public UIPnlGameStart()
	{
		m_ModelObjectPath = "UIPnlGameStart";
		m_IsOnlyOne = true;

		m_Check = null;
	}

	public override void OpenSelf(GameObject target)
	{
		base.OpenSelf(target);
		Button bt = m_ControlTarget.transform.Find("Button").GetComponent<Button>();
		bt.onClick.AddListener(OnClickGameStart);
		m_Check = bt;

		string key = string.Format(EngineMessageHead.NET_CLIENT_MESSAGE_HEAD, 10011);
		MessageManger.Instance.AddMessageListener(key, this.m_ControlTarget, ChangeScene);
	}

	private void OnClickGameStart()
	{
		m_Check.interactable = false;

		Transform user = m_ControlTarget.transform.Find("user");
		Text u = user.Find("Text").gameObject.GetComponent<Text>();
		string su = u.text;

		Transform password = m_ControlTarget.transform.Find("password");
		Text p = password.Find("Text").gameObject.GetComponent<Text>();
		string sp = p.text;

		if (string.IsNullOrEmpty(su))
		{
			Debug.LogWarning("user is null.");
			m_Check.interactable = true;
			return;
		}

		if (string.IsNullOrEmpty(sp))
		{
			Debug.LogWarning("password is null.");
			m_Check.interactable = true;
			return;
		}

		StartGameRequest pack = new StartGameRequest();
		pack.SetSendData(su, sp);
		GameNetManager.Instance.SendMessage<StartGameRequest>(pack, 1);

	}

	/// <summary>
	/// 服务器验证
	/// </summary>
	/// <param name="arms"></param>
	private void ChangeScene(params object[] arms)
	{
		if (arms.Length > 0)
		{
			List<object> list = arms[0] as List<object>;
			if (list[0] is CheckUserPasswordResponse)
			{
				CheckUserPasswordResponse getServerTimeResponse = list[0] as CheckUserPasswordResponse;
				if (getServerTimeResponse.m_IsSuccess)
				{
					GameSceneManager.Instance.ChangeScene(new GameMainScene());
				}
				else
				{
					Debug.Log("账号异常！");
					m_Check.interactable = true;
				}
			}
		}
	}
}
