/*
 * Creator:ffm
 * Desc:角色动作机控制类
 * Time:2020/4/13 16:58:54
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class GameCharacterAnimatorBase
	{
		/// <summary>
		/// 对象内容
		/// </summary>
		protected Animator m_OwnerAnimator;

		public GameCharacterAnimatorBase(Animator ator)
		{
			m_OwnerAnimator = ator;
			if (m_OwnerAnimator == null)
			{
				//Debug.LogError("the ch is error.");
			}
		}

		/// <summary>
		/// 修改控制数据
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		public virtual void ChangeParameter(string name, object value, AnimatorControllerParameterType type = AnimatorControllerParameterType.Bool)
		{
			switch (type)
			{
				case AnimatorControllerParameterType.Bool:
					m_OwnerAnimator.SetBool(name, (bool)value);
					break;
				case AnimatorControllerParameterType.Trigger:
					m_OwnerAnimator.SetBool(name, (bool)value);
					break;
				case AnimatorControllerParameterType.Float:
					m_OwnerAnimator.SetFloat(name, (float)value);
					break;
				case AnimatorControllerParameterType.Int:
					m_OwnerAnimator.SetInteger(name, (int)value);
					break;
			}
		}

		/// <summary>
		/// Update更新
		/// </summary>
		public virtual void Update()
		{

		}
	}
}
