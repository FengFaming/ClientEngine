/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:角色动画控制器
 * Time:2020/7/31 8:52:05
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	/// <summary>
	/// 角色动画管理器
	/// </summary>
	public class RoleAnimationManager
	{
		/// <summary>
		/// 从属角色
		/// </summary>
		protected IRole m_Owner;

		/// <summary>
		/// 角色所有动画
		/// </summary>
		protected Dictionary<string, IRoleAnimation> m_AllAnimation;

		/// <summary>
		/// 角色所有动画事件
		/// </summary>
		protected Dictionary<string, List<AnimationFramActionEventInfo>> m_AllAnimationActions;

		/// <summary>
		/// 角色身上的动画控制器
		/// </summary>
		protected Animation m_RoleAnimation;

		/// <summary>
		/// 所有的动画
		/// </summary>
		protected Dictionary<string, AnimationState> m_AllClips;

		/// <summary>
		/// 当前动画
		/// </summary>
		protected IRoleAnimation m_Current;
		public IRoleAnimation Current { get { return m_Current; } }

		public RoleAnimationManager(IRole role)
		{
			m_Owner = role;
			m_AllAnimation = new Dictionary<string, IRoleAnimation>();
			m_AllAnimation.Clear();

			m_AllAnimationActions = new Dictionary<string, List<AnimationFramActionEventInfo>>();
			m_AllAnimationActions.Clear();

			GetAnimationControl();

			m_Current = null;
		}

		/// <summary>
		/// 添加一个角色动画对象
		/// </summary>
		/// <param name="roleAnimation"></param>
		public virtual bool AddRoleAnimation(IRoleAnimation roleAnimation)
		{
			GetAnimationControl();
			if (m_AllClips.ContainsKey(roleAnimation.Name))
			{
				m_AllAnimation.Add(roleAnimation.Name, roleAnimation);
				return true;
			}

			Debug.LogWarning("the animation has not animationClip:" + roleAnimation.Name);
			return false;
		}

		/// <summary>
		/// 移除一个动画对象
		/// </summary>
		/// <param name="name"></param>
		public virtual void RemoveRoleAnimation(string name)
		{
			if (m_AllAnimation.ContainsKey(name))
			{
				m_AllAnimation.Remove(name);
			}
		}

		/// <summary>
		/// 添加一个角色动画事件帧
		/// </summary>
		/// <param name="name"></param>
		/// <param name="info"></param>
		public virtual void AddRoleAnimationAction(string name, AnimationFramActionEventInfo info)
		{
			if (m_AllAnimationActions.ContainsKey(name))
			{
				if (!m_AllAnimationActions[name].Contains(info))
				{
					m_AllAnimationActions[name].Add(info);
					m_AllAnimationActions[name].Sort((AnimationFramActionEventInfo i1, AnimationFramActionEventInfo i2) =>
														{
															return (int)(i1.m_FramTime - i2.m_FramTime);
														});
				}
			}
			else
			{
				m_AllAnimationActions.Add(name, new List<AnimationFramActionEventInfo>() { info });
			}
		}

		/// <summary>
		/// 播放动画
		/// </summary>
		/// <param name="name"></param>
		/// <param name="speed"></param>
		/// <param name="loop"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public virtual bool Play(string name, float speed = 1, bool loop = false, float time = 0.3f)
		{
			if (!m_AllAnimation.ContainsKey(name))
			{
				return false;
			}

			if (!m_AllClips.ContainsKey(name))
			{
				return false;
			}

			//循环动画不具有事件
			if (m_Current != null && !m_Current.IsLoop)
			{
				if (m_AllAnimationActions.ContainsKey(m_Current.Name))
				{
					//将未完成的帧事件完成
					for (int index = 0; index < m_AllAnimationActions[m_Current.Name].Count; index++)
					{
						if (!m_AllAnimationActions[m_Current.Name][index].m_IsAction)
						{
							m_AllAnimationActions[m_Current.Name][index].HanldAction();
						}

						m_AllAnimationActions[m_Current.Name][index].m_IsAction = false;
					}
				}
			}

			m_AllAnimation[name].Play(m_Owner, speed, loop);
			m_Current = m_AllAnimation[name];
			///动画播放模式
			m_AllClips[name].wrapMode = loop ? WrapMode.Loop : WrapMode.Once;
			m_AllClips[name].speed = speed;
			m_RoleAnimation.CrossFade(name, time);
			return true;
		}

		/// <summary>
		/// 修改动画播放速度，一般只有循环动画才执行
		/// </summary>
		/// <param name="sp"></param>
		public virtual void ChangeAnimationSpeed(float sp)
		{
			if (m_Current == null)
			{
				return;
			}

			m_AllClips[m_Current.Name].speed = sp;
		}

		/// <summary>
		/// 查找对应的动画
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public virtual IRoleAnimation GetRoleAnimation(string name)
		{
			if (m_AllAnimation.ContainsKey(name))
			{
				return m_AllAnimation[name];
			}

			return null;
		}

		/// <summary>
		/// 更新
		/// </summary>
		public virtual void Update()
		{
			if (m_Current != null)
			{
				if (!m_Current.IsExit)
				{
					m_Current.Update();
					if (!m_Current.IsLoop)
					{
						if (m_AllAnimationActions.ContainsKey(m_Current.Name))
						{
							for (int index = 0; index < m_AllAnimationActions[m_Current.Name].Count; index++)
							{
								if (m_AllAnimationActions[m_Current.Name][index].m_FramTime <= m_Current.PlayTime)
								{
									if (!m_AllAnimationActions[m_Current.Name][index].m_IsAction)
									{
										m_AllAnimationActions[m_Current.Name][index].HanldAction();
									}
								}
								else
								{
									break;
								}
							}
						}

						if (m_Current.PlayTime >= m_AllClips[m_Current.Name].length)
						{
							m_Current.Exit();
						}
					}
					else
					{
						if (!m_RoleAnimation.IsPlaying(m_Current.Name))
						{
							Play(m_Current.Name, m_Current.PlaySpeed, true);
						}
					}
				}
			}
		}

		/// <summary>
		/// 清除数据
		/// </summary>
		public virtual void ClearData()
		{
			m_RoleAnimation.Stop();
			m_Current = null;
			m_Owner = null;
			m_RoleAnimation = null;

			foreach (var item in m_AllAnimation)
			{
				item.Value.ClearData();
			}

			m_AllAnimation.Clear();
			m_AllAnimation = null;

			foreach (var item in m_AllAnimationActions)
			{
				for (int index = 0; index < item.Value.Count; index++)
				{
					item.Value[index].ClearData();
				}

				item.Value.Clear();
			}

			m_AllAnimationActions.Clear();
			m_AllAnimationActions = null;

			m_AllClips.Clear();
			m_AllClips = null;
		}

		/// <summary>
		/// 获取角色动画控制器
		/// </summary>
		protected virtual void GetAnimationControl()
		{
			if (m_RoleAnimation == null)
			{
				m_RoleAnimation = m_Owner.gameObject.GetComponentInChildren<Animation>();
				m_AllClips = new Dictionary<string, AnimationState>();
				m_AllClips.Clear();

				if (m_RoleAnimation != null)
				{
					foreach (AnimationState state in m_RoleAnimation)
					{
						m_AllClips.Add(state.name, state);
					}
				}
			}
		}
	}
}
