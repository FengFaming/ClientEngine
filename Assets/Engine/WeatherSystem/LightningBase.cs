/*需要屏蔽的警告*/
#pragma warning disable 0618
/*
 * Creator:ffm
 * Desc:闪电控制
 * Time:2020/6/28 10:24:47
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	[RequireComponent(typeof(LineRenderer))]
	public class LightningBase : ObjectBase
	{
		/// <summary>
		/// 线性差值Y偏移量
		///		这个偏移量是一定的
		/// </summary>
		[Range(0, 2)]
		public float m_DisplacementY = 1;

		/// <summary>
		/// 线性差值X偏移量
		///		这个偏移量是随机的
		/// </summary>
		[Range(-1, 1)]
		public float m_DisplacementX = 1;

		/// <summary>
		/// 开始位置
		/// </summary>
		public Vector3 m_StartPosition;

		/// <summary>
		/// 结束位置
		/// </summary>
		public Vector3 m_EndPositioin;

		/// <summary>
		/// 左右偏移量
		/// </summary>
		[Range(0, 10)]
		public float m_Xoffset = 0;

		/// <summary>
		/// 闪烁时间
		/// </summary>
		public float m_DeltTime = 1;

		private LineRenderer m_LineRender;
		private List<Vector3> m_LinePositions;
		private float m_NowTime;

		private void Awake()
		{
			m_LineRender = this.gameObject.GetComponent<LineRenderer>();
			m_LinePositions = new List<Vector3>();
			m_NowTime = m_DeltTime;
		}

		private void Update()
		{
			m_NowTime -= Time.deltaTime;
			if (m_NowTime < 0)
			{
				m_NowTime = m_DeltTime;
				Vector3 start = m_StartPosition + Vector3.right * m_Xoffset * UnityEngine.Random.Range(-1.0f, 1.0f);
				Vector3 end = m_EndPositioin + Vector3.right * m_Xoffset * UnityEngine.Random.Range(-1.0f, 1.0f);
				CollectLinPosition(start, end);
				m_LineRender.SetVertexCount(m_LinePositions.Count);
				m_LineRender.SetPositions(m_LinePositions.ToArray());
			}
		}

		/// <summary>
		/// 线性差值求算线性点
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		private void CollectLinPosition(Vector3 start, Vector3 end)
		{
			float dy = start.y - end.y;
			float dx = (start.x + end.x) / 2;
			float dz = (start.z + end.z) / 2;
			int cout = Math.Abs((int)Math.Truncate(dy / m_DisplacementY));
			m_LinePositions.Clear();
			m_LinePositions.Add(end);
			for (int index = 0; index < cout; index++)
			{
				float y = end.y + index * m_DisplacementY;
				float x = dx + UnityEngine.Random.Range(-1.0f, 1.0f) * m_DisplacementX;
				float z = dz + UnityEngine.Random.Range(-1.0f, 1.0f) * m_DisplacementX;
				Vector3 p = new Vector3(x, y, z);
				m_LinePositions.Add(p);
			}

			m_LinePositions.Add(start);
		}
	}
}
