/*
 * Creator:ffm
 * Desc:大象动画控制
 * Time:2020/4/13 13:56:47
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class ElephentControl : AnimatorBase
{
	private void Awake()
	{
		Animator at = this.gameObject.GetComponentInChildren<Animator>();
		if (at != null)
		{
			m_ControlTarget = at;
		}
	}
}
