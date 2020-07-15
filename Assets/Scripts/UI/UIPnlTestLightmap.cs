/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:测试切换贴图
 * Time:2020/7/15 14:55:46
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;

public class UIPnlTestLightmap : IUIModelControl
{
	private List<string> m_AllNames;

	public UIPnlTestLightmap() : base()
	{
		m_ModelObjectPath = "UIPnlTestLightmap";
		m_IsOnlyOne = true;
	}

	public override void InitUIData(UILayer layer, params object[] arms)
	{
		base.InitUIData(layer, arms);
		m_AllNames = new List<string>();
		if (GameSceneManager.Instance.Current is ISceneWithLightmap)
		{
			SceneLightInfo sceneLightInfo = (GameSceneManager.Instance.Current as ISceneWithLightmap).CurrentInfo;
			foreach (var info in sceneLightInfo.m_AllInfoDic)
			{
				m_AllNames.Add(info.Key);
			}
		}
	}

	public override void OpenSelf(GameObject target)
	{
		base.OpenSelf(target);
		Button bt = m_ControlTarget.transform.Find("Button").gameObject.GetComponent<Button>();
		int index = 0;
		for (; index < m_AllNames.Count - 1; index++)
		{
			string name = m_AllNames[index];
			Button b = GameObject.Instantiate(bt);
			b.transform.SetParent(bt.transform.parent);
			b.GetComponentInChildren<Text>().text = m_AllNames[index];
			b.onClick.AddListener(() => { OnClick(name); });
			b.GetComponent<RectTransform>().localPosition = new Vector3(0, 230 - 50 * index, 0);
			b.GetComponent<RectTransform>().localScale = Vector3.one;
		}

		bt.GetComponentInChildren<Text>().text = m_AllNames[index];
		bt.onClick.AddListener(() => { OnClick(m_AllNames[index]); });
		bt.GetComponent<RectTransform>().localPosition = new Vector3(0, 230 - 50 * index, 0);
		bt.GetComponent<RectTransform>().localScale = Vector3.one;
	}

	private void OnClick(string name)
	{
		(GameSceneManager.Instance.Current as ISceneWithLightmap).ChangeLightmapWithName(name);
	}
}
