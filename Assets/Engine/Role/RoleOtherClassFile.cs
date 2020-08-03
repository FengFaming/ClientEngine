/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:角色其他相关类内容
 * Time:2020/7/31 10:08:11
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	#region 角色动画相关内容
	/// <summary>
	/// 一个帧事件
	/// </summary>
	public class AnimationActionEvent
	{
		/// <summary>
		/// 事件内容
		/// </summary>
		public Action<object[]> m_ActionEvent;

		/// <summary>
		/// 事件参数
		/// </summary>
		public List<object> m_ActionArms;

		public AnimationActionEvent() : this(null)
		{

		}

		public AnimationActionEvent(Action<object[]> action) : this(null, null)
		{

		}

		public AnimationActionEvent(Action<object[]> action, List<object> list)
		{
			m_ActionEvent = action;
			m_ActionArms = new List<object>();
			m_ActionArms.Clear();
			if (list != null)
			{
				m_ActionArms.AddRange(list);
			}
		}

		/// <summary>
		/// 启动回调
		/// </summary>
		public virtual void EventAction()
		{
			if (m_ActionEvent != null)
			{
				m_ActionEvent(m_ActionArms.ToArray());
			}
		}

		/// <summary>
		/// 清除数据
		/// </summary>
		public virtual void ClearData()
		{
			m_ActionArms.Clear();
			m_ActionArms = null;
			m_ActionEvent = null;
		}
	}

	/// <summary>
	/// 帧事件详情
	/// </summary>
	public class AnimationFramActionEventInfo
	{
		/// <summary>
		/// 时间长度
		/// </summary>
		public float m_FramTime;

		/// <summary>
		/// 是否已经调用过
		/// </summary>
		public bool m_IsAction;

		/// <summary>
		/// 所有的回调
		/// </summary>
		public List<AnimationActionEvent> m_AllActions;

		public AnimationFramActionEventInfo(float time)
		{
			m_FramTime = time;
			m_IsAction = false;
			m_AllActions = new List<AnimationActionEvent>();
			m_AllActions.Clear();
		}

		/// <summary>
		/// 添加回调
		/// </summary>
		/// <param name="action"></param>
		public virtual void AddActionEvent(AnimationActionEvent action)
		{
			m_AllActions.Add(action);
		}

		/// <summary>
		/// 添加回调
		/// </summary>
		/// <param name="action"></param>
		public virtual void AddActionEvent(Action<object[]> action)
		{
			AddActionEvent(new AnimationActionEvent(action));
		}

		/// <summary>
		/// 添加回调
		/// </summary>
		/// <param name="action"></param>
		/// <param name="list"></param>
		public virtual void AddActionEvent(Action<object[]> action, List<object> list)
		{
			AddActionEvent(new AnimationActionEvent(action, list));
		}

		/// <summary>
		/// 启动回调
		/// </summary>
		public virtual void HanldAction()
		{
			if (m_IsAction)
			{
				return;
			}

			m_IsAction = true;
			for (int index = 0; index < m_AllActions.Count; index++)
			{
				m_AllActions[index].EventAction();
			}
		}

		/// <summary>
		/// 恢复回调
		/// </summary>
		public virtual void ReaseAction()
		{
			m_IsAction = false;
		}

		/// <summary>
		/// 清除数据
		/// </summary>
		public virtual void ClearData()
		{
			m_IsAction = true;
			m_FramTime = 999f;
			for (int index = 0; index < m_AllActions.Count; index++)
			{
				m_AllActions[index].ClearData();
			}

			m_AllActions.Clear();
			m_AllActions = null;
		}

		public override bool Equals(object obj)
		{
			if (obj is AnimationFramActionEventInfo)
			{
				AnimationFramActionEventInfo other = obj as AnimationFramActionEventInfo;
				return other.m_FramTime == this.m_FramTime;
			}

			if (obj is float)
			{
				return (float)obj == this.m_FramTime;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
	#endregion
}
