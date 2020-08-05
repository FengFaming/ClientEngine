/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:技能管理
 * Time:2020/8/5 9:42:18
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class IRoleSkillManager
	{
		/// <summary>
		/// 技能拥有者
		/// </summary>
		protected IRole m_Owner;

		/// <summary>
		/// 所有技能
		/// </summary>
		protected Dictionary<int, IRoleSkill> m_AllSkillDic;

		/// <summary>
		/// 当前的技能
		/// </summary>
		protected IRoleSkill m_CurrentSkill;
		public IRoleSkill Current { get { return m_CurrentSkill; } }

		/// <summary>
		/// 退出技能
		/// </summary>
		protected Action m_ExitAction;
		public Action ExitAction { set { m_ExitAction = value; } }

		public IRoleSkillManager(IRole role)
		{
			m_AllSkillDic = new Dictionary<int, IRoleSkill>();
			m_AllSkillDic.Clear();

			m_Owner = role;
		}

		/// <summary>
		/// 添加技能
		/// </summary>
		/// <param name="skill"></param>
		public virtual void AddSkill(IRoleSkill skill)
		{
			if (m_AllSkillDic.ContainsKey(skill.SkillID))
			{
				m_AllSkillDic.Remove(skill.SkillID);
			}

			m_AllSkillDic.Add(skill.SkillID, skill);
		}

		/// <summary>
		/// 移除技能
		/// </summary>
		/// <param name="id"></param>
		public virtual void RemoveSkill(int id)
		{
			if (m_AllSkillDic.ContainsKey(id))
			{
				m_AllSkillDic.Remove(id);
			}
		}

		/// <summary>
		/// 尝试使用技能
		/// </summary>
		/// <param name="id"></param>
		public virtual void TrySkill(int id, float sp = 1f)
		{
			if (!m_AllSkillDic.ContainsKey(id))
			{
				return;
			}

			IRoleSkill skill = m_AllSkillDic[id];
			if (!skill.IsStarting(m_Owner))
			{
				return;
			}

			if (m_CurrentSkill != null)
			{
				m_CurrentSkill.ExitSkill(true);
			}

			m_CurrentSkill = null;
			skill.ExitAction = SkillExitAction;
			skill.EnterSkill(sp);
			m_CurrentSkill = skill;
		}

		/// <summary>
		/// 技能数据
		/// </summary>
		public virtual void Update()
		{
			if (m_CurrentSkill != null)
			{
				m_CurrentSkill.StaySkill();
			}
		}

		/// <summary>
		/// 技能退出
		/// </summary>
		/// <param name="skill"></param>
		protected virtual void SkillExitAction(IRoleSkill skill)
		{
			if (m_CurrentSkill.SkillID == skill.SkillID)
			{
				m_CurrentSkill = null;
				if (m_ExitAction != null)
				{
					m_ExitAction();
				}
			}
		}
	}
}