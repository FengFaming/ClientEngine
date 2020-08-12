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
		public IRoleStateManager StateManager { get { return m_StateManager; } }

		protected IRoleSkillManager m_SkillManager;
		public IRoleSkillManager SkillManager { get { return m_SkillManager; } }

		protected virtual void Awake()
		{
			m_RoleAnimationManager = null;
			m_RoleUID = -100000;
		}

		/// <summary>
		/// 设置UID
		///		读取配置数据等
		/// </summary>
		/// <param name="uid"></param>
		public void SetUID(int uid)
		{
			m_RoleUID = uid;
		}

		protected virtual void InitRole(RoleAnimationManager animation,
									IRoleStateManager state,
									IRoleSkillManager skillManager)
		{
			m_RoleAnimationManager = animation;
			m_StateManager = state;
			m_SkillManager = skillManager;
		}

		protected virtual void Update()
		{
			if (m_RoleAnimationManager != null)
			{
				m_RoleAnimationManager.Update();
			}

			if (m_StateManager != null)
			{
				m_StateManager.Update();
			}

			if (m_SkillManager != null)
			{
				m_SkillManager.Update();
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