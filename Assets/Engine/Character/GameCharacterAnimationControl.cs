/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:角色动作管理器
 * Time:2020/5/12 9:53:54
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	/// <summary>
	/// 角色动画控制器
	/// </summary>
	public class GameCharacterAnimationControl : GameCharacterAnimatorBase
	{
		private Animation m_AnimationControl;
		private GameObject m_Owner;

		public GameCharacterAnimationControl(GameObject owner) : base(null)
		{
			m_Owner = owner;
			m_AnimationControl = m_Owner.GetComponentInChildren<Animation>();
		}

		public override void ChangeParameter(string name, object value, AnimatorControllerParameterType type = AnimatorControllerParameterType.Bool)
		{
			//base.ChangeParameter(name, value, type);
			m_AnimationControl.Play(name);
		}
	}
}
