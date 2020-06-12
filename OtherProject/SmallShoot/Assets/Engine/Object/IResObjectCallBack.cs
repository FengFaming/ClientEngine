/*
 * Creator:ffm
 * Desc:资源加载回调
 * Time:2019/11/11 15:51:22
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

namespace Game.Engine
{
	/// <summary>
	/// 资源加载虚基类
	/// </summary>
	public abstract class IResObjectCallBack
	{
		/// <summary>
		/// 资源加载回调
		/// </summary>
		/// <param name="t"></param>
		public abstract void HandleLoadCallBack(object t);

		/// <summary>
		/// 加载回调优先级
		/// </summary>
		/// <returns></returns>
		public abstract int LoadCallbackPriority();
	}

	/// <summary>
	/// 测试基类
	/// </summary>
	public class ResObjectCallBackBase : IResObjectCallBack
	{
		public ResObjectType m_LoadType;
		public Action<object> m_FinshFunction;

		public override void HandleLoadCallBack(object t)
		{
			if (m_FinshFunction != null)
			{
				m_FinshFunction(t);
			}
		}

		public override int LoadCallbackPriority()
		{
			return 0;
		}
	}
}
