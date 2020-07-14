/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:输入界面
 * Time:2020/5/8 10:38:34
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class UIPnlInputCout : UIModelLuaControl
{
	public UIPnlInputCout() : base()
	{
		m_ModelObjectPath = "UIPnlInputCout";
		m_IsOnlyOne = true;
		m_LuaName = "UIPnlInputCout";
	}
}
