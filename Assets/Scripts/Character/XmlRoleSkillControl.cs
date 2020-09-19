/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:xml角色技能控制
 * Time:2020/8/27 14:18:26
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class XmlRoleSkillControl : IRoleSkill
{
	public XmlRoleSkillControl(int id) : base(id)
	{

	}

	public void XmlExitSkill(IRoleSkill skill)
	{
		m_Owner.StateManager.StartState(1);
	}
}
