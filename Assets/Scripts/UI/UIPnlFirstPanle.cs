/*
 * Creator:ffm
 * Desc:第一个界面
 * Time:2020/4/6 16:00:51
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;

public class UIPnlFirstPanle : IUIModelControl
{
	private Slider m_ShowProgress;
	private bool m_ShowLoding;

	public UIPnlFirstPanle() : base()
	{
		m_ModelObjectPath = "UIPnlFirstPanle";
		m_IsOnlyOne = true;
		m_ShowLoding = false;
	}

	public override void InitUIData(UILayer layer, params object[] arms)
	{
		base.InitUIData(layer, arms);
		if (arms.Length > 0)
		{
			m_ShowLoding = (bool)arms[0];
		}
	}

	public override void OpenSelf(GameObject target)
	{
		base.OpenSelf(target);

		m_ShowProgress = m_ControlTarget.gameObject.transform.Find("Slider").gameObject.GetComponent<Slider>();
		m_ShowProgress.value = 0;

		m_ShowProgress.gameObject.SetActive(m_ShowLoding);
		if (m_ShowLoding)
		{
			MessageManger.Instance.AddMessageListener(EngineMessageHead.CHANGE_SCENE_PRESS_VALUE, new IMessageBase(
			 m_ControlTarget, false, Listen));
		}
	}

	private void Listen(params object[] arms)
	{
		m_ShowProgress.value = (float)arms[0];
	}

	public override void CloseSelf(bool manager = false)
	{
		base.CloseSelf(manager);
		MessageManger.Instance.RemoveMessageListener(EngineMessageHead.CHANGE_SCENE_PRESS_VALUE, m_ControlTarget);
	}
}
