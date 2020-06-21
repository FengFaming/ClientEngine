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
		protected MeshFilter m_MeshFilter;

		/// <summary>
		/// mesh坐标
		/// </summary>
		protected int m_MeshIndex;

		/// <summary>
		/// 初始位置
		/// </summary>
		protected Vector3 m_StartPosition;

		/// <summary>
		/// 是否启动雨滴运动
		/// </summary>
#if UNITY_EDITOR
		[SerializeField]
#endif
		protected bool m_IsUpdate;

		protected virtual void Awake()
		{
			m_MeshIndex = 0;
			m_MeshFilter = this.gameObject.GetComponent<MeshFilter>();
			m_StartPosition = this.gameObject.transform.localPosition;
			ChangeMesh();
		}

		/// <summary>
		/// 修改mesh数据
		/// </summary>
		protected virtual void ChangeMesh()
		{
			m_MeshFilter.sharedMesh = GetMesh();
			m_MeshFilter.transform.localScale = Vector3.one * m_RainSparsity;
			m_MeshFilter.transform.localPosition = m_StartPosition;
		}

		/// <summary>
		/// 得到对应mesh
		/// </summary>
		/// <returns></returns>
		protected virtual Mesh GetMesh()
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

		/// <summary>
		/// 持续修改雨势大小
		///		size越大，雨势越小
		/// </summary>
		/// <param name="size"></param>
		public void ChangeRainMaxMin(float size)
		{
			m_RainSparsity = size;
			m_MeshFilter.transform.localScale = Vector3.one * m_RainSparsity;
		}

		/// <summary>
		/// 持续修改雨滴速度
		/// </summary>
		/// <param name="speed"></param>
		public void ChangeRainSpeed(float speed)
		{
			m_RainSpeed = speed;
		}

		/// <summary>
		/// mesh运动
		/// </summary>
		protected virtual void Update()
		{
			if (m_IsUpdate)
			{
				this.gameObject.transform.position -= Vector3.up * Time.deltaTime * m_RainSpeed;
				if (m_StartPosition.y - this.gameObject.transform.position.y > m_DeltY)
				{
					ChangeMesh();
				}
			}
		}
	}
}
