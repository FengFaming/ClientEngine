/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:雨块运动控制器
 * Time:2020/6/20 15:23:41
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class RainRuningBase : ObjectBase
	{
		/// <summary>
		/// 雨滴的缩放比例
		///		用来调整雨势大小的
		/// </summary>
		[Tooltip("雨滴比例")]
		public float m_RainSparsity = 1f;

		/// <summary>
		/// 雨滴降落多远消失
		/// </summary>
		[Tooltip("雨滴消失距离")]
		public float m_DeltY = 5f;

		/// <summary>
		/// 雨滴下落速度
		/// </summary>
		[Tooltip("雨滴下降的速度")]
		public float m_RainSpeed = 23f;

		/// <summary>
		/// 所有的mesh内容
		/// </summary>
		public Mesh[] m_AllMeshs;

		/// <summary>
		/// mesh对象
		/// </summary>
		private MeshFilter m_MeshFilter;

		/// <summary>
		/// 抢定旋转角度
		/// </summary>
		private Quaternion m_Rotation;

		/// <summary>
		/// mesh坐标
		/// </summary>
		private int m_MeshIndex;

		/// <summary>
		/// 初始位置
		/// </summary>
		private Vector3 m_StartPosition;

		/// <summary>
		/// 是否启动雨滴运动
		/// </summary>
#if UNITY_EDITOR
		[SerializeField]
#endif
		private bool m_IsUpdate;

		private void Awake()
		{
			m_Rotation = Quaternion.identity;
			this.gameObject.transform.rotation = m_Rotation;
			m_MeshIndex = 0;
			m_MeshFilter = this.gameObject.GetComponent<MeshFilter>();
			m_StartPosition = this.gameObject.transform.localPosition;
			ChangeMesh();
		}

		private void ChangeMesh()
		{
			m_MeshFilter.sharedMesh = GetMesh();
			m_MeshFilter.transform.localScale = Vector3.one * m_RainSparsity;
			m_MeshFilter.transform.localPosition = m_StartPosition;
		}

		private Mesh GetMesh()
		{
			if (m_MeshIndex >= m_AllMeshs.Length)
			{
				m_MeshIndex = 0;
			}

			return m_AllMeshs[m_MeshIndex++];
		}

		/// <summary>
		/// 是否启动雨滴运动
		/// </summary>
		public void StartRain(bool runing = true)
		{
			m_IsUpdate = runing;
			if (!m_IsUpdate)
			{
				m_MeshFilter.sharedMesh = null;
				this.gameObject.transform.localPosition = m_StartPosition;
				m_MeshIndex = 0;
			}
		}

		private void Update()
		{
			if (m_IsUpdate)
			{
				this.gameObject.transform.position -= Vector3.up * Time.deltaTime * m_RainSpeed;
				if (m_StartPosition.y - this.gameObject.transform.position.y > m_DeltY)
				{
					ChangeMesh();
				}

				/*
				 * 为什么要修订这个旋转角度
				 *	怕的是父节点进行了旋转
				 * */
				this.gameObject.transform.rotation = m_Rotation;
			}
		}
	}
}
