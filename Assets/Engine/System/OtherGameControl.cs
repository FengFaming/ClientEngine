/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:其他游戏控制
 * Time:2020/6/11 18:05:05
* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Game.Engine
{
	public class OtherGameControl : SingletonMonoClass<OtherGameControl>
	{
		public void OpenOtherExe(string path, string other)
		{
			Process process = new Process();
			string e = Application.streamingAssetsPath + "/Other/" + path;
#if !UNITY_EDITOR || TEST_AB
			e = Application.persistentDataPath + "/Other/" + path;
#endif

			ProcessStartInfo startinfo = new ProcessStartInfo(e, other);
			process.StartInfo = startinfo;
			process.StartInfo.UseShellExecute = false;
			process.Start();
		}
	}
}
