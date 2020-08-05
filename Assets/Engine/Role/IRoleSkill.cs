/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:角色技能
 * Time:2020/8/5 9:41:53
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class IRoleSkill
	{
		/// <summary>
		/// 技能唯一ID
		/// </summary>
		protected int m_SkillID;
		public int SkillID { get { return m_SkillID; } }

		/// <summary>
		/// 状态ID
		/// </summary>
		protected int m_StateID;
		public int StateID
		{
			get { return m_StateID; }
			set { m_StateID = value; }
		}

		/// <summary>
		/// 技能自然结束的回调
		/// </summary>
		protected Action<IRoleSkill> m_ExitAction;
		public Action<IRoleSkill> ExitAction { set { m_ExitAction = value; } }

		/// <summary>
		/// 所有事件集合
		/// </summary>
		protected Dictionary<float, List<AnimationFramActionEventInfo>> m_AllActionDic;

		/// <summary>
		/// 技能时间
		/// </summary>
		protected float m_PlayTime;

		/// <summary>
		/// 技能播放速度
		/// </summary>
		protected float m_PlaySpeed;

		/// <summary>
		/// 控制对象
		/// </summary>
		protected IRole m_Owner;

		public IRoleSkill(int id)
		{
			m_SkillID = id;
			m_AllActionDic = new Dictionary<float, List<AnimationFramActionEventInfo>>();
			m_AllActionDic.Clear();

			m_PlayTime = 0;
			m_PlaySpeed = 0;
			m_Owner = null;
		}

		/// <summary>
		/// 重置数据
		/// </summary>
		protected virtual void ResetData()
		{
			foreach (KeyValuePair<float, List<AnimationFramActionEventInfo>> item in m_AllActionDic)
			{
				for (int index = 0; index < item.Value.Count; index++)
				{
					item.Value[index].m_IsAction = false;
				}
			}
		}

		/// <summary>
		/// 状态退出
		/// </summary>
		/// <param name="state"></param>
		protected virtual void StateExit(IObjectState state)
		{
			ExitSkill(false);
		}

		/// <summary>
		/// 添加事件
		/// </summary>
		/// <param name="info"></param>
		public virtual void AddEventInfo(AnimationFramActionEventInfo info)
		{
			if (m_AllActionDic.ContainsKey(info.m_FramTime))
			{
				m_AllActionDic[info.m_FramTime].Add(info);
			}
			else
			{
				m_AllActionDic.Add(info.m_FramTime, new List<AnimationFramActionEventInfo>() { info });
			}
		}

		/// <summary>
		/// 技能是否可以开始
		/// </summary>
		/// <returns></returns>
		public virtual bool IsStarting(IRole owner)
		{
			m_Owner = owner;
			return true;
		}

		/// <summary>
		/// 技能开始
		/// </summary>
		public virtual void EnterSkill(float sp = 1f)
		{
			m_PlaySpeed = sp;
			m_PlayTime = 0;
			ResetData();
			if (m_Owner != null)
			{
				IRoleState state = m_Owner.StateManager.GetControlState(m_StateID) as IRoleState;
				state.ExitAction = StateExit;
				m_Owner.StateManager.StartState(m_StateID, m_PlaySpeed);
			}
		}

		/// <summary>
		/// 维持技能
		/// </summary>
		public virtual void StaySkill()
		{
			m_PlayTime += Time.deltaTime * m_PlaySpeed;
			foreach (KeyValuePair<float, List<AnimationFramActionEventInfo>> item in m_AllActionDic)
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
			}
		}

		/// <summary>
		/// 技能结束
		/// </summary>
		public virtual void ExitSkill(bool isManager = false)
		{
			m_PlaySpeed = 0;
			m_PlayTime = 0;
			if (!isManager)
			{
				if (m_ExitAction != null)
				{
					m_ExitAction(this);
				}
			}
		}
	}
}
