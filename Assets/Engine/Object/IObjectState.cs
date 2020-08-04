/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:对象状态
 * Time:2020/8/4 9:19:26
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	/// <summary>
	/// 状态虚基类
	/// </summary>
	public abstract class IObjectState
	{
		/// <summary>
		/// 状态唯一ID
		/// </summary>
		protected int m_OnlyID;
		public int StateID { get { return m_OnlyID; } }

		/// <summary>
		/// 状态是否循环
		/// </summary>
		protected bool m_IsLoop;
		public bool Loop
		{
			set { m_IsLoop = value; }
			get { return m_IsLoop; }
		}

		/// <summary>
		/// 运行的总长度
		///		对于非循环的有用
		/// </summary>
		protected float m_TimeLenght;
		public float TimeLength { set { m_TimeLenght = value; } }

		/// <summary>
		/// 状态运行时间
		/// </summary>
		protected float m_PlayTime;
		public float PlayTime { get { return m_PlayTime; } }

		/// <summary>
		/// 状态运行速度
		/// </summary>
		protected float m_PlaySpeed;
		public float PlaySpeed
		{
			get { return m_PlaySpeed; }
			set { m_PlaySpeed = value; }
		}

		/// <summary>
		/// 退出事件
		/// </summary>
		protected Action<IObjectState> m_ExitAction;
		public Action<IObjectState> ExitAction { set { m_ExitAction = value; } }

		/// <summary>
		/// 运行事件
		/// </summary>
		protected Dictionary<float, List<AnimationFramActionEventInfo>> m_StateEventDic;

		/// <summary>
		/// 是否处于运行中
		/// </summary>
		protected bool m_IsRuning;

		public IObjectState(int id)
		{
			m_OnlyID = id;
			m_IsLoop = false;
			m_PlaySpeed = 1;
			m_PlayTime = 0;
			m_StateEventDic = new Dictionary<float, List<AnimationFramActionEventInfo>>();
			m_StateEventDic.Clear();
			m_IsRuning = false;
			m_TimeLenght = 0f;
			m_ExitAction = null;
		}

		/// <summary>
		/// 重置所有事件
		/// </summary>
		protected virtual void ResetEvent()
		{
			foreach (KeyValuePair<float, List<AnimationFramActionEventInfo>> item in m_StateEventDic)
			{
				for (int index = 0; index < item.Value.Count; index++)
				{
					item.Value[index].m_IsAction = false;
				}
			}
		}

		/// <summary>
		/// 添加一个对象事件
		/// </summary>
		/// <param name="time"></param>
		/// <param name="eventInfo"></param>
		public virtual void AddEvent(float time, AnimationFramActionEventInfo eventInfo)
		{
			if (m_StateEventDic.ContainsKey(time))
			{
				m_StateEventDic[time].Add(eventInfo);
				m_StateEventDic[time].Sort((AnimationFramActionEventInfo i1, AnimationFramActionEventInfo i2) =>
				{
					return (int)(i1.m_FramTime - i2.m_FramTime);
				});
			}
			else
			{
				m_StateEventDic.Add(time, new List<AnimationFramActionEventInfo>() { eventInfo });
			}
		}

		/// <summary>
		/// 状态是否能被别的状态打断
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public abstract bool IsInterrupted(IObjectState other);

		/// <summary>
		/// 自身判断自己是否能启动
		/// </summary>
		/// <returns></returns>
		public abstract bool IsSelfStarting();

		/// <summary>
		/// 开始状态
		/// </summary>
		/// <returns></returns>
		public virtual bool StartState(float speed = 1f)
		{
			ResetEvent();

			m_PlaySpeed = speed;
			m_IsRuning = true;
			m_PlayTime = 0f;
			return true;
		}

		/// <summary>
		/// 状态维持
		/// </summary>
		/// <returns></returns>
		public virtual bool StayState()
		{
			if (m_IsRuning)
			{
				if (!m_IsLoop)
				{
					m_PlayTime += Time.deltaTime * m_PlaySpeed;
					foreach (KeyValuePair<float, List<AnimationFramActionEventInfo>> item in m_StateEventDic)
					{
						if (item.Key <= m_PlayTime)
						{
							for (int index = 0; index < item.Value.Count; index++)
							{
								if (!item.Value[index].m_IsAction)
								{
									item.Value[index].HanldAction();
								}
							}
						}
						else
						{
							break;
						}
					}

					if (m_TimeLenght > 0)
					{
						if (m_TimeLenght <= m_PlayTime)
						{
							ExitState();
						}
					}
				}
			}

			return true;
		}

		/// <summary>
		/// 状态退出
		/// </summary>
		/// <param name="manager"></param>
		/// <returns></returns>
		public virtual bool ExitState(bool manager = false)
		{
			m_IsRuning = false;
			m_PlayTime = 0f;

			if (!manager)
			{
				if (m_ExitAction != null)
				{
					m_ExitAction(this);
				}
			}

			return true;
		}
	}
}