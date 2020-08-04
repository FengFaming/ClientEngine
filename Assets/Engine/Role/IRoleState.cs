/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:角色状态
 * Time:2020/8/4 13:59:58
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class IRoleState : IObjectState
	{
		/// <summary>
		/// 动画队列
		/// </summary>
		protected List<string> m_Animations;

		/// <summary>
		/// 播放序列
		/// </summary>
		protected int m_PlayIndex;

		/// <summary>
		/// 动画管理对象
		/// </summary>
		protected RoleAnimationManager m_AnimationManager;
		public RoleAnimationManager AnimationManager { set { m_AnimationManager = value; } }

		public IRoleState(int id) : base(id)
		{
			m_Animations = new List<string>();
			m_Animations.Clear();
			m_PlayIndex = 0;
		}

		/// <summary>
		/// 添加一个动画
		/// </summary>
		/// <param name="name"></param>
		public virtual void AddAnimation(string name)
		{
			m_Animations.Add(name);
		}

		/// <summary>
		/// 状态能否被别人打断
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public override bool IsInterrupted(IObjectState other)
		{
			return true;
		}

		/// <summary>
		/// 状态自身能否进行
		/// </summary>
		/// <returns></returns>
		public override bool IsSelfStarting()
		{
			return true;
		}

		public override bool StartState(float speed = 1f)
		{
			if (!base.StartState(speed))
				return false;

			m_PlayIndex = 0;
			if (m_Animations.Count <= 0)
			{
				return true;
			}

			IRoleAnimation a = m_AnimationManager.GetRoleAnimation(m_Animations[m_PlayIndex]);
			if (a != null)
			{
				a.ExitAction = AnimationExitFun;
				m_AnimationManager.Play(m_Animations[m_PlayIndex], m_PlaySpeed);
			}

			return true;
		}

		/// <summary>
		/// 动画播放结束
		/// </summary>
		/// <param name="animation"></param>
		protected virtual void AnimationExitFun(IRoleAnimation animation)
		{
			if (animation.Name == m_Animations[m_PlayIndex])
			{
				m_PlayIndex++;
				if (m_Animations.Count <= m_PlayIndex)
				{
					if (m_IsLoop)
					{
						m_PlayIndex = 0;
					}
				}

				if (m_Animations.Count > m_PlayIndex)
				{
					IRoleAnimation a = m_AnimationManager.GetRoleAnimation(m_Animations[m_PlayIndex]);
					if (a != null)
					{
						a.ExitAction = AnimationExitFun;
						m_AnimationManager.Play(m_Animations[m_PlayIndex], m_PlaySpeed);
					}
				}
				else
				{
					ExitState();
				}
			}
		}
	}
}
