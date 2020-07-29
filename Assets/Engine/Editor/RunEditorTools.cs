/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:运行的部分辅助工具
 * Time:2020/7/29 14:29:04
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEditor;

public class RunEditorTools : ObjectBase
{
	[MenuItem("Tools/ClearAllSaveData")]
	private static void ClearAllSaveData()
	{
		PlayerPrefs.DeleteAll();
	}

}
