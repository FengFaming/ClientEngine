/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:叠加场景测试
 * Time:2020/6/30 16:11:57
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class AdditiveSceneTest : ISceneWithAdditive
{
	public AdditiveSceneTest(string name) : base(name)
	{

	}

	protected override void LoadEnd(string name)
	{
		base.LoadEnd(name);
		if (m_Cout >= m_AllLoadScene)
		{
			UIManager.Instance.OpenUI("UIPnlBackGameMain", UILayer.Pnl);
		}
	}

	public override void LoadScene(Action<float> action)
	{
		//base.LoadScene(action);
		m_StartAction = action;
		List<string> vs = new List<string>();
		vs.Add(m_SceneName);
		vs.Add("t1");
		m_Cout = 0;
		m_AllLoadScene = 2;
		for (int index = 0; index < vs.Count; index++)
		{
			LoadEndScene les = new LoadEndScene();
			les.m_End = LoadEnd;
			les.m_Name = vs[index];
			ResObjectManager.Instance.LoadObject(les.m_Name, ResObjectType.Scene, les);
		}
	}
}
