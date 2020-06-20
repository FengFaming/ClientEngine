/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:下雨天气控制
 * Time:2020/6/20 11:22:49
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

namespace Game.Engine
{
	/// <summary>
	/// 下雨控制器
	/// </summary>
	public class RainWeatherControl : ObjectBase
	{
		/// <summary>
		/// 开始下雨内容
		/// </summary>
		public void StartRain()
		{
			StartCoroutine("Raining");
		}

		private IEnumerator Raining()
		{
			yield return null;
		}
	}
}
