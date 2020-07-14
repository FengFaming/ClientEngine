/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:shootgame游戏主界面
 * Time:2020/4/27 13:54:06
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;
using TMPro;

public class UIPnlShootGameMain : IUIModelControl
{
	private TextMeshProUGUI m_ShowText;

	public UIPnlShootGameMain() : base()
	{
		m_ModelObjectPath = "UIPnlShootGameMain";
		m_IsOnlyOne = true;
	}

	public override void OpenSelf(GameObject target)
	{
		base.OpenSelf(target);
		m_ShowText = m_ControlTarget.gameObject.transform.Find("fenshu").gameObject.GetComponent<TextMeshProUGUI>();
		m_ShowText.text = "";

		MessageManger.Instance.AddMessageListener(GameMessageHeadFiled.M_SHOOT_GAME_FENSHU, m_ControlTarget, ChangeShowText);
	}

	private void ChangeShowText(params object[] arms)
	{
		int f = (int)arms[0];
		m_ShowText.text = f.ToString();
	}
}
