/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:角色
 * Time:2020/7/31 8:57:03
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class IRole : ObjectBase
	{
		/// <summary>
		/// 角色唯一ID
		/// </summary>
		protected int m_RoleUID;
		public int RoleUID { get { return m_RoleUID; } }

		/// <summary>
		/// 动画播放控制器
		/// </summary>
		protected RoleAnimationManager m_RoleAnimationManager;

		/// <summary>
		/// 角色状态管理器
		/// </summary>
		protected IRoleStateManager m_StateManager;

		protected virtual void Awake()
		{
			m_RoleAnimationManager = null;
			m_RoleUID = -100000;
		}

		public virtual void InitRole(int uid,
									RoleAnimationManager animation,
									IRoleStateManager state)
		{
			m_RoleUID = uid;
			m_RoleAnimationManager = animation;
			m_StateManager = state;
		}

		protected virtual void Update()
		{
			if (m_RoleAnimationManager != null)
			{
				m_RoleAnimationManager.Update();
			}
		}

		#region 暂时测试使用
		public void PlayAnimation(string name)
		{
			m_RoleAnimationManager.Play(name);
		}
		#endregion
	}
}