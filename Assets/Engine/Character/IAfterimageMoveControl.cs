/*
 * Creator:ffm
 * Desc:残影变化控制基类
 * Time:2020/4/9 11:21:34
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	/// <summary>
	/// 残影的变化规则
	/// </summary>
	public class IAfterimageMoveControl : ObjectBase
	{
		protected float m_DieTime;

		protected float m_LastTime;
		protected MeshRenderer[] m_AllSMR;

		protected virtual void Awake()
		{
			m_AllSMR = this.gameObject.GetComponentsInChildren<MeshRenderer>();
			if (m_AllSMR == null || m_AllSMR.Length < 1)
			{
				Debug.Log("the target is not mesh.");
				return;
			}
		}

		public virtual void StartMove(float dieTime)
		{
			m_DieTime = dieTime;
			m_LastTime = Time.time;
		}

		protected virtual void Update()
		{
			if ((Time.time - m_LastTime) > m_DieTime)
			{
				GameObject.DestroyImmediate(this.gameObject);
			}
		}
	}
}
