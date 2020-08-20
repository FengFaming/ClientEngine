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
	}

	private void OnClickGameStart()
	{
		StartGameRequest pack = new StartGameRequest();
		GameNetManager.Instance.SendMessage<StartGameRequest>(pack, 1);
		GameSceneManager.Instance.ChangeScene(new GameMainScene());
	}
}
