/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:游戏开始界面
 * Time:2020/6/11 14:02:46
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;

public class UIPnlGameStart : IUIModelControl
{
	public UIPnlGameStart() : base()
	{
		m_ModelObjectPath = "UIPnlGameStart";
		m_IsOnlyOne = true;
	}

	public override void OpenSelf(GameObject target)
	{
		base.OpenSelf(target);

		Button bt = m_ControlTarget.gameObject.transform.Find("Button").gameObject.GetComponent<Button>();
		bt.onClick.AddListener(OnClick);
	}

	public void OnClick()
	{
		string lua = "GameStartLua";
		XLua.LuaTable table = LuaManager.Instance.CreateTable(lua);
		XLua.LuaFunction luaFunction = table.Get<XLua.LuaFunction>("zijishuliang");
		System.Object[] vs = luaFunction.Call();
		GameStart.Instance.StartGameWithData((string)vs[0], table);
		CloseSelf();
	}
}
