/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:角色动画接口
 * Time:2020/7/31 8:52:35
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	/// <summary>
	/// 角色一个动画
	/// </summary>
	public class IRoleAnimation
	{
		/// <summary>
		/// 动画名字
		/// </summary>
		protected string m_AnimationName;
		public string Name { get { return m_AnimationName; } }

		/// <summary>
		/// 动画播放模式
		/// </summary>
		protected bool m_IsLoop;
		public bool IsLoop { get { return m_IsLoop; } }

		/// <summary>
		/// 动画播放速度
		/// </summary>
		protected float m_PlaySpeed;
		public float PlaySpeed { get { return m_PlaySpeed; } }

		/// <summary>
		/// 动画是否已经开始
		/// </summary>
		protected bool m_IsUpdate;
		public bool IsPlay { get { return m_IsUpdate; } }

		/// <summary>
		/// 动画播放了多久
		/// </summary>
		protected float m_PlayTime;
		public float PlayTime { get { return m_PlayTime; } }

		/// <summary>
		/// 是否已经退出
		/// </summary>
		protected bool m_IsExit;
		public bool IsExit { get { return m_IsExit; } }

		/// <summary>
		/// 从属者
		/// </summary>
		protected IRole m_Owner;

		/// <summary>
		/// 退出的函数监听
		/// </summary>
		protected Action<IRoleAnimation> m_ExitAction;
		public Action<IRoleAnimation> ExitAction { set { m_ExitAction = value; } }

		public IRoleAnimation(string name)
		{
			m_AnimationName = name;
			m_PlaySpeed = 1f;
			m_IsUpdate = false;
			m_IsLoop = false;

			m_IsExit = false;
		}

		/// <summary>
		/// 播放动画
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="sp"></param>
		/// <param name="loop"></param>
		/// <param name="exit"></param>
		public virtual void Play(IRole owner, float sp, bool loop = false, Action<IRoleAnimation> exit = null)
		{
			m_Owner = owner;
			m_PlaySpeed = sp;
			m_PlayTime = 0f;
			m_IsLoop = loop;
			m_IsExit = false;
			m_IsUpdate = !m_IsLoop;
			m_ExitAction = exit;
		}

		/// <summary>
		/// 动画退出
		/// </summary>
		public virtual void Exit()
		{
			m_IsExit = true;
			m_IsUpdate = false;
			m_PlaySpeed = 1f;
			m_PlayTime = 0f;

			if (m_ExitAction != null)
			{
				m_ExitAction(this);
			}
		}

		/// <summary>
		/// 清除数据
		/// </summary>
		public virtual void ClearData()
		{
			m_IsUpdate = false;
			m_PlaySpeed = 1f;
			m_PlayTime = 0f;
		}

		/// <summary>
		/// 动画更新
		/// </summary>
		public virtual void Update()
		{
			if (m_IsUpdate)
			{
				m_PlayTime += Time.deltaTime * m_PlaySpeed;
			}
		}
	}
}
