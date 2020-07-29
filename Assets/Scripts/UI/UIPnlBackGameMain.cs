/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:返回游戏主界面
 * Time:2020/4/27 8:57:36
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;

public class UIPnlBackGameMain : IUIModelControl
{
	private Vector3 m_Position;
	private bool m_IsSetPosition;

	public UIPnlBackGameMain() : base()
	{
		m_ModelObjectPath = "UIPnlBackGameMain";
		m_IsOnlyOne = true;
	}

	public override void InitUIData(UILayer layer, params object[] arms)
	{
		base.InitUIData(layer, arms);
		m_IsSetPosition = false;
		if (arms != null && arms.Length > 0)
		{
			m_Position = (Vector3)arms[0];
			m_IsSetPosition = true;
		}
	}

	public override void OpenSelf(GameObject target)
	{
		base.OpenSelf(target);
		Button bt = m_ControlTarget.gameObject.transform.Find("Button").gameObject.GetComponent<Button>();
		bt.onClick.AddListener(GoBackGameMain);
		if (m_IsSetPosition)
		{
			bt.gameObject.transform.localPosition = m_Position;
		}
	}

	private void GoBackGameMain()
	{
		GameSceneManager.Instance.ChangeScene(new GameMainScene());
	}
}
