/*
 * Creator:ffm
 * Desc:use update move
 * Time:2020/4/16 14:07:47
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	/// <summary>
	/// 使用update进行移动控制
	/// </summary>
	public class GameObjectMoveControl : ObjectBase
	{
		/// <summary>
		/// 移动类型
		/// </summary>
		private enum MoveType
		{
			/// <summary>
			/// 以目标点
			/// </summary>
			Target,

			/// <summary>
			/// 以时间
			/// </summary>
			Time,
		}

		/// <summary>
		/// 移动终点
		/// </summary>
		private Vector3 m_MoveTarget;

		/// <summary>
		/// 方向速度
		/// </summary>
		private Vector3 m_ForwardSpeed;

		/// <summary>
		/// 移动时间
		/// </summary>
		private float m_MoveTime;

		/// <summary>
		/// 移动结束回调
		/// </summary>
		private Action<bool> m_MoveEnd;

		/// <summary>
		/// 移动类型
		/// </summary>
		private MoveType m_MoveType;

		/// <summary>
		/// 是否可以开始移动
		/// </summary>
		private bool m_StartMove;

		/// <summary>
		/// 新的旋转内容
		/// </summary>
		private Quaternion m_NewQuaternion;

		/// <summary>
		/// 旋转时间
		/// </summary>
		private float m_RotationTime;

		/// <summary>
		/// 是否开始旋转
		/// </summary>
		private bool m_HasRotation;

		private void Awake()
		{
			m_MoveTarget = this.gameObject.transform.position;
			m_ForwardSpeed = Vector3.zero;
			m_MoveTime = 0f;
			m_MoveEnd = null;
			m_MoveType = MoveType.Target;
			m_StartMove = false;

			m_NewQuaternion = this.gameObject.transform.rotation;
			m_RotationTime = 0f;
			m_HasRotation = false;
		}

		/// <summary>
		/// 设置旋转
		/// </summary>
		/// <param name="rotation"></param>
		/// <param name="time"></param>
		public void SetRotation(Quaternion rotation, float time)
		{
			m_NewQuaternion = rotation;
			m_RotationTime = time;
			m_HasRotation = true;
		}

		/// <summary>
		/// 设置移动
		/// </summary>
		/// <param name="target"></param>
		/// <param name="forwardSpeed"></param>
		/// <param name="moveTime"></param>
		/// <param name="end"></param>
		public void SetMove(Vector3 target, Vector3 forwardSpeed, float moveTime = -1f, Action<bool> end = null)
		{
			m_StartMove = false;
			m_MoveTarget = target;
			m_ForwardSpeed = forwardSpeed;
			m_MoveTime = moveTime;
			m_MoveEnd = end;

			if (forwardSpeed != Vector3.zero && moveTime > 0)
			{
				m_MoveType = MoveType.Time;
			}

			if (m_MoveType == MoveType.Target)
			{
				if (m_MoveTime > 0)
				{
					float distance = Vector3.Distance(this.gameObject.transform.position, m_MoveTarget);
					float s = distance / m_MoveTime;
					Vector3 f = Vector3.Normalize(m_MoveTarget - this.gameObject.transform.position);
					m_ForwardSpeed = f * s;
				}
			}

			m_StartMove = true;
		}

		private void Update()
		{
			if (m_HasRotation)
			{
				this.gameObject.transform.rotation = Quaternion.Lerp(this.gameObject.transform.rotation, m_NewQuaternion, m_RotationTime);
			}

			if (m_StartMove)
			{
				if (m_MoveType == MoveType.Target)
				{
					Vector3 t = m_ForwardSpeed * Time.deltaTime + this.gameObject.transform.position;
					this.gameObject.transform.position = t;

					if (Vector3.Normalize(m_MoveTarget - this.gameObject.transform.position) !=
							Vector3.Normalize(m_ForwardSpeed))
					{
						CalExit(true);
					}
				}
				else if (m_MoveType == MoveType.Time)
				{
					Vector3 t = m_ForwardSpeed * Time.deltaTime + this.gameObject.transform.position;
					this.gameObject.transform.position = t;
					m_MoveTime -= Time.deltaTime;
					if (m_MoveTime < 0)
					{
						CalExit(true);
					}
				}
			}
		}

		private void OnDestroy()
		{
			m_StartMove = false;
			m_ForwardSpeed = Vector3.zero;
			m_MoveTarget = this.gameObject.transform.position;
			m_MoveTime = -1f;
			m_MoveType = MoveType.Target;
			m_MoveEnd = null;
		}

		private void CalExit(bool end)
		{
			m_StartMove = false;
			m_ForwardSpeed = Vector3.zero;
			m_MoveTarget = this.gameObject.transform.position;
			m_MoveTime = -1f;
			m_MoveType = MoveType.Target;
			Action<bool> e = m_MoveEnd;
			m_MoveEnd = null;

			if (e != null)
			{
				e(end);
			}
		}
	}
}
