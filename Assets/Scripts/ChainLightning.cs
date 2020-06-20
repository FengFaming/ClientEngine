/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:闪电
 * Time:2020/6/17 17:50:00
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

[RequireComponent(typeof(LineRenderer))]
//[ExecuteInEditMode]  //普通的类，加上ExecuteInEditMode， 就可以在编辑器模式中运行
public class ChainLightning : ObjectBase
{
	public float m_Detail = 1;//增加后，线条数量会减少，每个线条会更长。  
	public float m_Displacement = 15;//位移量，也就是线条数值方向偏移的最大值  
	public Transform m_EndPostion;//链接目标  
	public Transform m_StartPosition;
	public float m_Yoffset = 0;
	public float m_DeltTime = 1;

	private LineRenderer m_LineRender;
	private List<Vector3> m_LinePosList;
	private float m_NowTime;

	private void Awake()
	{
		m_LineRender = GetComponent<LineRenderer>();
		m_LinePosList = new List<Vector3>();
		m_NowTime = m_DeltTime;
	}

	private void Update()
	{
		m_NowTime -= Time.deltaTime;
		if (m_NowTime < 0)
		{
			m_NowTime = m_DeltTime;
			if (Time.timeScale != 0)
			{
				m_LinePosList.Clear();
				Vector3 startPos = Vector3.zero;
				Vector3 endPos = Vector3.zero;
				if (m_EndPostion != null)
				{
					endPos = m_EndPostion.position + Vector3.up * m_Yoffset;
				}

				if (m_StartPosition != null)
				{
					startPos = m_StartPosition.position + Vector3.up * m_Yoffset;
				}

				CollectLinPos(startPos, endPos, m_Displacement);
				m_LinePosList.Add(endPos);
				m_LineRender.SetVertexCount(m_LinePosList.Count);
				for (int i = 0, n = m_LinePosList.Count; i < n; i++)
				{
					m_LineRender.SetPosition(i, m_LinePosList[i]);
				}
			}
		}
	}

	/// <summary>
	/// 递归求线的点
	/// </summary>
	/// <param name="startPos"></param>
	/// <param name="destPos"></param>
	/// <param name="displace"></param>
	private void CollectLinPos(Vector3 startPos, Vector3 destPos, float displace)
	{
		if (displace < m_Detail)
		{
			m_LinePosList.Add(startPos);
		}
		else
		{
			float midX = (startPos.x + destPos.x) / 2;
			float midY = (startPos.y + destPos.y) / 2;
			float midZ = (startPos.z + destPos.z) / 2;

			/*
			 * UnityEngine.Random.value
			 * 返回一个介于0.0 ~~ 1.0之间的浮点数
			 * */
			midX += (float)(UnityEngine.Random.value - 0.5) * displace;
			midY += (float)(UnityEngine.Random.value - 0.5) * displace;
			midZ += (float)(UnityEngine.Random.value - 0.5) * displace;
			Vector3 midPos = new Vector3(midX, midY, midZ);

			///二分差值法
			CollectLinPos(startPos, midPos, displace / 2);
			CollectLinPos(midPos, destPos, displace / 2);
		}
	}
}
