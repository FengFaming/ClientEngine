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
using System.Threading;
using UnityEngine;

namespace Game.Engine
{
	public partial class PaiLieZuHeControl : MonoBehaviour
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

		/// <summary>
		/// 线程
		/// </summary>
		private Thread m_Thread;
		private SynchronizationContext m_MainThreadSynContext;

		private delegate void SendMessageWithThread();

		private void Awake()
		{
			m_IsCaling = false;
		}

		private void OnDestroy()
		{
			if (m_Thread != null)
			{
				m_Thread.Abort();
			}
		}

		/// <summary>
		/// 由子线程返回到主线程
		/// </summary>
		/// <param name="state"></param>
		private void GoBaMainThread(object state)
		{
			Debug.Log(m_ReturnData.Count);
			Debug.Log("end:" + Time.realtimeSinceStartup);
			if (m_Thread != null)
			{
				m_Thread.Abort();
			}
		}

		/// <summary>
		/// 通过信号量的方法返回租进程
		/// </summary>
		private void SendMessage()
		{
			m_MainThreadSynContext.Post(new SendOrPostCallback(GoBaMainThread), null);
		}

		/// <summary>
		/// 加入时不加不需要的内容
		/// </summary>
		/// <param name="data"></param>
		/// <param name="temp"></param>
		private void DelOne(ref List<int[]> data, int[] temp)
		{
			//for (int index = 0; index < data.Count; index++)
			//{
			//	if (EngineTools.Instance.CalDB<int>(data[index], temp))
			//	{
			//		return;
			//	}
			//}

			data.Add(temp);
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

		/// <summary>
		/// 使用协程计算
		/// </summary>
		/// <returns></returns>
		private IEnumerator StartCalZuHe()
		{
			Debug.Log("start:" + Time.time);
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
			Debug.Log("end:" + Time.time);
		}

		/// <summary>
		/// 使用线程计算
		/// </summary>
		private void StartThread(object action)
		{
			m_IsCaling = true;
			int sw = -1;
			List<int[]> rtData = new List<int[]>();
			int[] rt;
			do
			{
				int[] temp = new int[m_Cout];
				m_ToZuHeData.CopyTo(temp, 0);
				DelOne(ref rtData, temp);

				if (sw >= 0)
				{
					rt = EngineTools.Instance.Reverse<int>(m_ToZuHeData, sw, m_Cout - 1);
					int[] tmp = new int[m_Cout];
					rt.CopyTo(tmp, 0);
					DelOne(ref rtData, tmp);
				}
			} while (EngineTools.Instance.Permutation(ref m_ToZuHeData, 0, m_Cout, ref sw));

			int start = 0;
			do
			{
				EngineTools.Instance.DelCF<int>(ref rtData, rtData[start], start + 1);
				start = start + 1;
			} while (start < rtData.Count);

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
			}

			m_IsCaling = false;
			m_IsSuccess = true;
			SendMessageWithThread callback = action as SendMessageWithThread;
			callback();
		}

		/// <summary>
		/// 使用线程计算
		/// </summary>
		/// <param name="data"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public void UserThreadZuHe(object[] data, int start, int end)
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
				m_Thread = new Thread(StartThread);
				m_Thread.IsBackground = true;
				SendMessageWithThread callback = new SendMessageWithThread(SendMessage);
				m_Thread.Start(callback);
				Debug.Log("start:" + Time.time);
			}
		}

		/// <summary>
		/// 使用协程计算
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
	}
}
