/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:排列组合问题
 * Time:2020/5/25 9:56:00
* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game.Engine
{
	public partial class PaiLieZuHeControl : MonoBehaviour
	{
		private void ThreadStartT(object action)
		{
			m_IsCaling = true;

			List<int[]> rt = EngineTools.Instance.GetPermutation<int>(m_ToZuHeData);
			m_ReturnData = new List<object[]>();
			for (int index = 0; index < rt.Count; index++)
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
							c = rt[index][i - m_StartIndex];
							vs[i] = m_SourceData[c];
						}
						else
						{
							vs[i] = m_SourceData[i];
						}
					}
				}

				m_ReturnData.Add(vs);
			}

			m_IsCaling = false;
			m_IsSuccess = true;
			SendMessageWithThread callback = action as SendMessageWithThread;
			callback();
		}

		/// <summary>
		/// 使用线程和递归进行计算
		/// </summary>
		/// <param name="data"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public void UseThreadT(object[] data, int start, int end)
		{
			if (!m_IsCaling)
			{
				if (m_Thread != null)
				{
					m_Thread.Abort();
				}

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

				//StartCoroutine("StartCalZuHe");
				m_MainThreadSynContext = SynchronizationContext.Current;
				m_Thread = null;
				m_Thread = new Thread(ThreadStartT);
				m_Thread.IsBackground = true;
				SendMessageWithThread callback = new SendMessageWithThread(SendMessage);
				m_Thread.Start(callback);
				Debug.Log("start:" + Time.realtimeSinceStartup);
			}
		}
	}
}
