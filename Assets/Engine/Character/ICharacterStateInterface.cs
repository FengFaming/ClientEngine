/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:角色状态接口
 * Time:2020/4/26 11:11:47
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

namespace Game.Engine
{
	public abstract class ICharacterStateInterface
	{
		public ICharacterStateInterface(int id)
		{
			m_StateID = id;
		}

		/// <summary>
		/// 状态的唯一ID
		/// </summary>
		protected int m_StateID;
		public int StateID { get { return m_StateID; } }

		/// <summary>
		/// 从属于那个管理
		/// </summary>
		protected GameCharacterStateManager m_OwnerManager;
		public GameCharacterStateManager OwnerManager { set { m_OwnerManager = value; } }

		/// <summary>
		/// 状态是否可以开始
		/// </summary>
		/// <param name="last">上一个状态</param>
		/// <returns></returns>
		public abstract bool IsStartState(ICharacterStateInterface last);

		/// <summary>
		/// 进入状态
		/// </summary>
		/// <param name="arms">状态进入的一些参数</param>
		/// <returns></returns>
		public abstract bool EnterState(params object[] arms);

		/// <summary>
		/// 离开状态
		/// </summary>
		/// <param name="isManager">是否是自身退出</param>
		public abstract void ExitState(bool isManager);

		/// <summary>
		/// 状态维持
		/// </summary>
		public abstract void StayState();

		/// <summary>
		/// 获取退出状态需要切换到的状态
		/// </summary>
		/// <returns></returns>
		public abstract int GetExitState();
	}
}
