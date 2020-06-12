/*
 * Creator:ffm
 * Desc:角色摄像机控制
 * Time:2020/4/13 17:10:50
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class GameCharacterCameraBase
	{
		/// <summary>
		/// 摄像机
		/// </summary>
		protected Camera m_ControlCamera;

		/// <summary>
		/// 主体位置
		/// </summary>
		protected GameObject m_ControlTarget;

		/// <summary>
		/// 是否是从属关系
		/// </summary>
		protected bool m_IsSun;

		/// <summary>
		/// 位置偏差
		/// </summary>
		protected Vector3 m_DeltPosition;

		/// <summary>
		/// 是否跟随
		/// </summary>
		protected bool m_IsFollow;

		/// <summary>
		/// 初始化摄像机管理
		/// </summary>
		/// <param name="target">目标对象</param>
		/// <param name="sun">是否作为子节点</param>
		/// <param name="c">摄像机对象</param>
		public GameCharacterCameraBase(GameObject target, bool sun = false, Camera c = null, bool follow = true)
		{
			m_ControlTarget = target;

			if (c == null)
			{
				c = Camera.main;
			}

			m_ControlCamera = c;
			m_IsSun = sun;
			if (m_IsSun)
			{
				m_ControlCamera.gameObject.transform.parent = m_ControlTarget.transform;
			}

			m_DeltPosition = Vector3.zero;

			m_IsFollow = follow;
		}

		/// <summary>
		/// 设置位置对象
		/// </summary>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		/// <param name="scale"></param>
		public void SetTransform(Vector3 position, Vector3 rotation, Vector3 scale)
		{
			if (m_IsSun)
			{
				m_ControlCamera.gameObject.transform.localPosition = position;
				m_ControlCamera.gameObject.transform.localRotation = Quaternion.Euler(rotation);
				m_ControlCamera.gameObject.transform.localScale = scale;
			}
			else
			{
				m_ControlCamera.gameObject.transform.position = position;
				m_ControlCamera.gameObject.transform.rotation = Quaternion.Euler(rotation);
				m_ControlCamera.gameObject.transform.localScale = scale;

				m_DeltPosition = m_ControlTarget.gameObject.transform.position -
								 m_ControlCamera.gameObject.transform.position;
			}
		}

		/// <summary>
		/// Update更新
		/// </summary>
		/// <param name="time"></param>
		public virtual void Update(float time)
		{

		}

		/// <summary>
		/// Late Update更新
		/// </summary>
		/// <param name="time"></param>
		public virtual void LateUpdate(float time)
		{
			if (m_IsFollow)
			{
				if (!m_IsSun)
				{
					m_ControlCamera.gameObject.transform.position =
						Vector3.Lerp(m_ControlCamera.gameObject.transform.position,
						m_ControlTarget.gameObject.transform.position - m_DeltPosition,
						time);
				}
			}
		}
	}
}
