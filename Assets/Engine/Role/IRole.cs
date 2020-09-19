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
		/// 设置UID,xml数据
		///		读取配置数据等
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="xmlID"></param>
		public virtual void SetUID(int uid, int xmlID = 0)
		{
			m_RoleUID = uid;
		}

		/// <summary>
		/// 初始化角色控制
		/// </summary>
		/// <param name="animation"></param>
		/// <param name="state"></param>
		/// <param name="skillManager"></param>
		protected virtual void InitRole(RoleAnimationManager animation,
									IRoleStateManager state,
									IRoleSkillManager skillManager)
		{
			m_RoleAnimationManager = animation;
			m_StateManager = state;
			m_SkillManager = skillManager;
		}

		/// <summary>
		/// 控制更新
		/// </summary>
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
	}
}