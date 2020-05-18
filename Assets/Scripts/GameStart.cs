/*
 * Creator:ffm
 * Desc:项目启动类
 * Time:2019/11/11 15:25:55
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class GameStart : ObjectBase
{
	public string m_IDCard = "44022219910116033X";
	public List<string> m_UINames;
	public UILayer m_Layer;

	private void Start()
	{
		ResObjectManager.Instance.InitResManager("AB");
		MessageManger.Instance.AddMessageListener(EngineMessageHead.CHANGE_SCENE_MESSAGE,
						this.gameObject, OpenChangeScene);

		GameShowFPS fps = this.gameObject.AddComponent<GameShowFPS>();
		fps.IsShowFPS = true;

		StartCoroutine("StartGame");
	}

	private void OpenChangeScene(params object[] arms)
	{
		if ((bool)arms[0])
		{
			UIManager.Instance.OpenUI("UIPnlFirstPanle", UILayer.Blk, true);
		}
		else
		{
			UIManager.Instance.RecoveryUIModel("UIPnlFirstPanle", UILayer.Blk);
		}
	}

	private IEnumerator StartGame()
	{
		yield return null;
		UIManager.Instance.OpenUI("UIPnlFirstPanle", UILayer.Pnl);
		yield return new WaitForSeconds(0.5f);

		UIManager.Instance.OpenUI("UIPnlGameStart", UILayer.Pnl);
	}
}
