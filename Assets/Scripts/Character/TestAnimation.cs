/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:测试动画
 * Time:2020/8/3 8:52:31
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class TestAnimation : IRoleAnimation
{
	public string m_Last;

	public TestAnimation(string name) : base(name)
	{

	}

	public override void Exit()
	{
		base.Exit();
		m_Owner.PlayAnimation(m_Last);
	}
}
