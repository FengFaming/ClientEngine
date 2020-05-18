/*
 * Creator:ffm
 * Desc:love界面
 * Time:2019/12/28 14:59:28
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using XLua;
using UnityEngine.UI;

public class UIPnlLove : UIModelLuaControl
{
	public UIPnlLove() : base()
	{
		m_ModelObjectPath = "UIPnlLove";
		m_IsOnlyOne = true;
		m_LuaName = "MyTextLua";
	}

	public override void OpenSelf(GameObject target)
	{
		base.OpenSelf(target);
		Button t = target.transform.Find("Image").gameObject.GetComponent<Button>();
		if (m_UILuaTable != null)
		{
			m_UILuaTable.Set("lightCpnt", t);
		}

		Action<GameObject> action = m_UILuaTable.Get<Action<GameObject>>("click");
		t.onClick.AddListener(() => { action(t.gameObject); });

		//UIManager.Instance.AddUpdate(this);
	}

	public override bool Update()
	{
		if (!base.Update())
			return false;

		if (m_LuaUpdate != null)
		{
			m_LuaUpdate();
		}

		return true;
	}
}
