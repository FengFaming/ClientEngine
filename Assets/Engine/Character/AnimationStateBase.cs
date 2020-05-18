/*
 * Creator:ffm
 * Desc:动画状态基类
 * Time:2020/1/9 15:01:49
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

namespace Game.Engine
{
	[System.Serializable]
	public class AnimationStateBase
	{
		/// <summary>
		/// 状态名字
		/// </summary>
		public string StateName;

		/// <summary>
		/// 相互关联的状态
		/// </summary>
		protected AnimationStateBase m_LinkState;

		/// <summary>
		/// 进入状态
		/// </summary>
		public virtual void EnterState()
		{

		}

		/// <summary>
		/// 离开状态
		/// </summary>
		public virtual void ExitState()
		{

		}
	}
}
