/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:角色状态管理
 * Time:2020/4/26 10:45:02
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class GameCharacterStateManager
	{
		/// <summary>
		/// 角色所有的状态
		/// </summary>
		private Dictionary<int, ICharacterStateInterface> m_AllStateDic;

		/// <summary>
		/// 当前处于的状态
		/// </summary>
		private ICharacterStateInterface m_CurrentState;
		public ICharacterStateInterface CurrentState { get { return m_CurrentState; } }

		protected GameCharacterBase m_Owner;
		public GameCharacterBase Owner { get { return m_Owner; } }

		/// <summary>
		/// 动画控制器
		/// </summary>
		protected GameCharacterAnimatorBase m_ControlAnimator;
		public GameCharacterAnimatorBase ControlAnimator
		{
			set { m_ControlAnimator = value; }
			get { return m_ControlAnimator; }
		}

		public GameCharacterStateManager(GameCharacterBase gameCharacterBase)
		{
			m_AllStateDic = new Dictionary<int, ICharacterStateInterface>();
			m_AllStateDic.Clear();

			m_CurrentState = null;

			m_Owner = gameCharacterBase;
		}

		/// <summary>
		/// 获取相对应的状态
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ICharacterStateInterface GetManagerState(int id)
		{
			if (m_AllStateDic.ContainsKey(id))
			{
				return m_AllStateDic[id];
			}

			return null;
		}

		/// <summary>
		/// 添加状态
		/// </summary>
		/// <param name="state"></param>
		public void AddManagerState(ICharacterStateInterface state)
		{
			if (m_AllStateDic.ContainsKey(state.StateID))
			{
				m_AllStateDic.Remove(state.StateID);
			}

			state.OwnerManager = this;
			m_AllStateDic.Add(state.StateID, state);
		}

		/// <summary>
		/// 移除某一个状态
		/// </summary>
		/// <param name="id"></param>
		public void RemoveManagerState(int id)
		{
			if (m_CurrentState != null && m_CurrentState.StateID == id)
			{
				m_CurrentState = null;
			}

			if (m_AllStateDic.ContainsKey(id))
			{
				m_AllStateDic.Remove(id);
			}
		}

		/// <summary>
		/// 尝试进入状态
		/// </summary>
		/// <param name="id"></param>
		/// <param name="arms"></param>
		/// <returns></returns>
		public bool TryGotoState(int id, params object[] arms)
		{
			ICharacterStateInterface state = GetManagerState(id);
			if (state != null)
			{
				return TryGotoState(state, arms);
			}

			return false;
		}

		/// <summary>
		/// 尝试进入状态
		/// </summary>
		/// <param name="state"></param>
		/// <param name="arms"></param>
		/// <returns></returns>
		public bool TryGotoState(ICharacterStateInterface state, params object[] arms)
		{
			if (m_CurrentState == null)
			{
				m_CurrentState = state;
				state.EnterState(arms);
				return true;
			}
			else
			{
				//即将进入的状态能否切换当前状态
				if (state.IsStartState(m_CurrentState))
				{
					m_CurrentState.ExitState(true);
					m_CurrentState = state;
					state.EnterState(arms);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 退出某一个状态
		/// </summary>
		/// <param name="state">状态</param>
		/// <param name="isManager">是否是状态自身退出</param>
		public void ExitState(ICharacterStateInterface state, bool isManager = false)
		{
			if (isManager)
			{
				if (state.StateID == m_CurrentState.StateID)
				{
					m_CurrentState = null;
				}
			}
			else
			{
				state.ExitState(false);
			}
		}

		/// <summary>
		/// 更新状态
		/// </summary>
		public void UpdateState()
		{
			if (m_CurrentState != null)
			{
				m_CurrentState.StayState();
			}

			if (m_ControlAnimator != null)
			{
				m_ControlAnimator.Update();
			}
		}
	}
}
