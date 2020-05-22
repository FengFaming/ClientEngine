/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:排列组合控制
 * Time:2020/5/22 11:04:23
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class PaiLieZuHeControl : MonoBehaviour
	{
		/// <summary>
		/// 排列组合内容
		/// </summary>
		private static PaiLieZuHeControl m_Instance;

		public static PaiLieZuHeControl GetInstance()
		{
			if (m_Instance == null)
			{
				GameObject go = new GameObject();
				go.name = typeof(PaiLieZuHeControl).Name;
				m_Instance = go.AddComponent<PaiLieZuHeControl>();
			}

			return m_Instance;
		}

		/// <summary>
		/// 元数据
		/// </summary>
		private object[] m_SourceData;

		/// <summary>
		/// 开始坐标
		/// </summary>
		private int m_StartIndex;

		/// <summary>
		/// 结束坐标
		/// </summary>
		private int m_EndIndex;

		/// <summary>
		/// 给去组合的数据
		/// </summary>
		private int[] m_ToZuHeData;

		/// <summary>
		/// 给去组合的数据长度
		/// </summary>
		private int m_Cout;

		/// <summary>
		/// 数据是否已经能得到
		/// </summary>
		private bool m_IsSuccess;

		/// <summary>
		/// 是否在计算当中
		/// </summary>
		private bool m_IsCaling;

		/// <summary>
		/// 返回数据
		/// </summary>
		private List<object[]> m_ReturnData;

		private void Awake()
		{
			m_IsCaling = false;
		}

		/// <summary>
		/// 开始计算数据
		/// </summary>
		/// <param name="data"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public void CalZuHe(object[] data, int start, int end)
		{
			if (!m_IsCaling)
			{
				ClearData();
				m_SourceData = data;
				m_StartIndex = start;
				m_EndIndex = end;
				m_ReturnData = new List<object[]>();

				if (m_StartIndex > m_EndIndex)
				{
					EngineTools.Instance.Swap<int>(ref m_StartIndex, ref m_EndIndex);
				}

				m_Cout = m_EndIndex - m_StartIndex;
				m_ToZuHeData = new int[m_Cout];
				for (int index = 0; index < m_Cout; index++)
				{
					m_ToZuHeData[index] = m_StartIndex + index;
				}

				StartCoroutine("StartCalZuHe");
			}
		}

		private void ClearData()
		{
			StopCoroutine("StartCalZuHe");
			StopAllCoroutines();
			m_SourceData = null;
			m_ToZuHeData = null;
			m_IsCaling = false;
			m_IsSuccess = false;
			m_StartIndex = m_EndIndex = 0;
			m_ReturnData = null;
		}

		private IEnumerator StartCalZuHe()
		{
			m_IsCaling = true;
			yield return null;
			int sw = -1;
			List<int[]> rtData = new List<int[]>();
			int[] rt;
			do
			{
				int[] temp = new int[m_Cout];
				m_ToZuHeData.CopyTo(temp, 0);
				rtData.Add(temp);

				if (sw >= 0)
				{
					rt = EngineTools.Instance.Reverse<int>(m_ToZuHeData, sw, m_Cout - 1);
					int[] tmp = new int[m_Cout];
					rt.CopyTo(tmp, 0);
					rtData.Add(tmp);
				}

				yield return null;
			} while (EngineTools.Instance.Permutation(ref m_ToZuHeData, 0, m_Cout, ref sw));

			int start = 0;
			do
			{
				EngineTools.Instance.DelCF<int>(ref rtData, rtData[start], start + 1);
				start = start + 1;
				yield return null;
			} while (start < rtData.Count);

			yield return null;

			m_ReturnData = new List<object[]>();
			for (int index = 0; index < rtData.Count; index++)
			{
				object[] vs = new object[m_SourceData.Length];
				for (int i = 0; i < m_SourceData.Length; i++)
				{
					if (i < m_StartIndex)
					{
						vs[i] = m_SourceData[i];
					}
					else
					{
						if (i < m_EndIndex)
						{
							int c = 0;
							c = rtData[index][i - m_StartIndex];
							vs[i] = m_SourceData[c];
						}
						else
						{
							vs[i] = m_SourceData[i];
						}
					}
				}

				m_ReturnData.Add(vs);
				yield return null;
			}

			yield return null;
			m_IsCaling = false;
			m_IsSuccess = true;
		}
	}
}
