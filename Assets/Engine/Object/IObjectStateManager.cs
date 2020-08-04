/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:对象状态管理器
 * Time:2020/8/4 10:26:08
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	/// <summary>
	/// 对象状态管理类
	///		有一点比较特殊，状态一旦进入，就不会退出导致不存在状态的概念
	///			所以控制接口当中没有涉及主动退出状态的接口
	/// </summary>
	public class IObjectStateManager
	{
		/// <summary>
		/// 所有状态
		/// </summary>
		protected Dictionary<int, IObjectState> m_AllStateDic;

		/// <summary>
		/// 当前状态
		/// </summary>
		protected IObjectState m_CurrenState;
		public IObjectState Current { get { return m_CurrenState; } }

		public IObjectStateManager()
		{
			m_AllStateDic = new Dictionary<int, IObjectState>();
			m_AllStateDic.Clear();

			m_CurrenState = null;
		}

		/// <summary>
		/// 添加状态
		/// </summary>
		/// <param name="state"></param>
		public virtual void AddState(IObjectState state)
		{
			if (m_AllStateDic.ContainsKey(state.StateID))
			{
				m_AllStateDic.Remove(state.StateID);
			}

			m_AllStateDic.Add(state.StateID, state);
		}

		/// <summary>
		/// 移除状态
		/// </summary>
		/// <param name="id"></param>
		public virtual void RemoveState(int id)
		{
			if (m_AllStateDic.ContainsKey(id))
			{
				m_AllStateDic.Remove(id);
			}

			if (m_CurrenState != null && m_CurrenState.StateID == id)
			{
				m_CurrenState = null;
			}
		}

		/// <summary>
		/// 获取状态
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual IObjectState GetControlState(int id)
		{
			if (m_AllStateDic.ContainsKey(id))
			{
				return m_AllStateDic[id];
			}

			return null;
		}

		/// <summary>
		/// 开始状态
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual bool StartState(int id, float speed = 1f)
		{
			IObjectState state = GetControlState(id);
			if (state == null)
			{
				return false;
			}

			if (!state.IsSelfStarting())
			{
				return false;
			}

			if (m_CurrenState != null)
			{
				///新状态能否打断旧状态
				if (!m_CurrenState.IsInterrupted(state))
				{
					return false;
				}

				m_CurrenState.ExitState(true);
				m_CurrenState = null;
			}

			state.StartState(speed);
			m_CurrenState = state;
			return true;
		}

		/// <summary>
		/// 状态更新
		/// </summary>
		public virtual void Update()
		{
			if (m_CurrenState != null)
			{
				m_CurrenState.StayState();
			}
		}
	}
}
