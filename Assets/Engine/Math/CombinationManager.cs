/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:组合计算管理类
 * Time:2020/6/4 8:43:28
* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Engine
{
	public class CombinationManager
	{
		#region 公有数据
		/// <summary>
		/// 数据源
		/// </summary>
		private object[] m_SourceData;

		/// <summary>
		/// 数据开始和结束
		/// </summary>
		private Vector2Int m_DataStartEnd;

		/// <summary>
		/// 是否在计算当中
		/// </summary>
		private bool m_HasCal;

		/// <summary>
		/// 数据返回
		/// </summary>
		private Action<CombinationManager> m_DataReturn;

		/// <summary>
		/// 返回数据
		/// </summary>
		private List<object[]> m_ReturnData;

		public CombinationManager()
		{
			m_DataReturn = null;
			m_HasCal = false;
			m_SourceData = null;
			m_DataStartEnd = Vector2Int.zero;
		}
		#endregion

		#region 使用两个数组进行标志位计算
		/// <summary>
		/// 保存文件的名字
		/// </summary>
		private string m_SaveFileName;

		/// <summary>
		/// 最大长度
		/// </summary>
		private int m_MaxLength;

		/// <summary>
		/// 间隔数据
		/// </summary>
		private int[] m_DataTemp;

		/// <summary>
		/// 书籍数据
		/// </summary>
		private int[] m_DataBook;

		/// <summary>
		/// 开始计算
		/// </summary>
		/// <param name="source"></param>
		/// <param name="se"></param>
		public void Combination(object[] source, Vector2Int se, Action<CombinationManager> action)
		{
			if (!m_HasCal)
			{
				m_HasCal = true;
				m_SourceData = source;
				m_DataStartEnd = se;
				m_DataReturn = action;
				if (m_DataStartEnd.x > m_DataStartEnd.y)
				{
					int temp = m_DataStartEnd.y;
					m_DataStartEnd.y = m_DataStartEnd.x;
					m_DataStartEnd.x = temp;
				}

				m_SaveFileName = Application.persistentDataPath + "/" +
					Time.realtimeSinceStartup.ToString().Replace('.', '-') + ".txt";
				Debug.Log(m_SaveFileName);

				Debug.Log("start:" + Time.realtimeSinceStartup);
				GameThreadManager.Instance.CreateThread(ThreadMain, GoBack);
			}
		}

		/// <summary>
		/// 计算返回数据
		/// </summary>
		private void CalReturnData()
		{
			FileStream fs = new FileStream(m_SaveFileName, FileMode.Open);
			StreamReader sr = new StreamReader(fs);
			string line = sr.ReadLine();
			while (line != null)
			{
				if (line == "" ||
					line == "\n" ||
					line == "")
				{
					continue;
				}

				string[] vs = line.Split(',');
				if (vs.Length != m_MaxLength)
				{
					continue;
				}

				int[] data = new int[vs.Length];
				for (int index = 0; index < vs.Length; index++)
				{
					data[index] = int.Parse(vs[index]) - 1;
				}

				object[] rt = new object[m_SourceData.Length];
				for (int index = 0; index < rt.Length; index++)
				{
					if (index < m_DataStartEnd.x)
					{
						rt[index] = m_SourceData[index];
					}
					else if (index < m_DataStartEnd.y)
					{
						rt[index] = m_SourceData[data[index - m_DataStartEnd.x]];
					}
					else
					{
						rt[index] = m_SourceData[index];
					}
				}

				if (m_ReturnData == null)
				{
					m_ReturnData = new List<object[]>();
					m_ReturnData.Clear();
				}

				m_ReturnData.Add(rt);
				line = sr.ReadLine();
			}

			sr.Close();
			fs.Close();
			//File.Delete(m_SaveFileName);
		}

		private void ThreadMain()
		{
			m_MaxLength = m_DataStartEnd.y - m_DataStartEnd.x;
			m_DataTemp = new int[m_MaxLength + 1];
			m_DataBook = new int[m_MaxLength + 1];
			dfs(1);
			CalReturnData();
		}

		private void SaveFile()
		{
			FileStream fs;
			if (!File.Exists(m_SaveFileName))
			{
				fs = new FileStream(m_SaveFileName, FileMode.Create, FileAccess.Write);
			}
			else
			{
				fs = new FileStream(m_SaveFileName, FileMode.Append, FileAccess.Write);
			}

			StreamWriter sw = new StreamWriter(fs);
			for (int i = 1; i < m_DataTemp.Length; i++)
			{
				sw.Write(m_DataTemp[i]);
				if (i < m_DataTemp.Length - 1)
				{
					sw.Write(",");
				}
			}

			sw.WriteLine();
			sw.Flush();
			sw.Close();
			fs.Close();
		}

		private void dfs(int step)
		{
			if (step == m_MaxLength + 1)
			{
				SaveFile();
				return;
			}

			for (int i = 1; i <= m_MaxLength; i++)
			{
				if (m_DataBook[i] == 0)
				{
					m_DataTemp[step] = i;
					m_DataBook[i] = i;
					dfs(step + 1);
					m_DataBook[i] = 0;
				}
			}
		}

		private void GoBack()
		{
			Debug.Log("end:" + Time.realtimeSinceStartup);
			m_HasCal = false;
			if (m_DataReturn != null)
			{
				m_DataReturn(this);
			}
		}
		#endregion

		#region 回溯非回归方法



		private void CalPaiLie()
		{
			//int sw = -1;
			//List<int[]> rtData = new List<int[]>();
			//int[] rt;
			//do
			//{
			//	/*
			//	 * 保存一次数据
			//	 * */

			//	if (sw >= 0)
			//	{
			//		rt = EngineTools.Instance.Reverse<int>(m_ToZuHeData, sw, m_Cout - 1);
			//		/*
			//		 * 保存一次数据
			//		 * */
			//	}

			//} while (EngineTools.Instance.Permutation(ref m_ToZuHeData, 0, m_Cout, ref sw));
		}
		#endregion

		#region 使用树进行计算的方法

		/// <summary>
		/// 开始计算
		/// </summary>
		/// <param name="source"></param>
		/// <param name="se"></param>
		public void Combination2(object[] source, Vector2Int se, Action<CombinationManager> action)
		{
			if (!m_HasCal)
			{
				m_HasCal = true;
				m_SourceData = source;
				m_DataStartEnd = se;
				m_DataReturn = action;
				if (m_DataStartEnd.x > m_DataStartEnd.y)
				{
					int temp = m_DataStartEnd.y;
					m_DataStartEnd.y = m_DataStartEnd.x;
					m_DataStartEnd.x = temp;
				}

				Debug.Log("start:" + Time.realtimeSinceStartup);
				GameThreadManager.Instance.CreateThread(ThreadMain2, GoBack2);
			}
		}

		/// <summary>
		/// 树状结构点
		/// </summary>
		internal class TreeDataInfo
		{
			/// <summary>
			/// 在父亲节点的顺序
			/// </summary>
			public int m_InParentIndex;

			/// <summary>
			/// 最大顺序节点
			/// </summary>
			public int m_MaxIndex;

			/// <summary>
			/// 元数据
			/// </summary>
			public int m_SourData;

			/// <summary>
			/// 父亲
			/// </summary>
			public TreeDataInfo m_Parent;

			/// <summary>
			/// 儿子们
			/// </summary>
			public List<TreeDataInfo> m_Suns;
		}

		private void ThreadMain2()
		{
			int leng = m_DataStartEnd.y - m_DataStartEnd.x;
			TreeDataInfo[] infos = new TreeDataInfo[leng];
			List<int> source = new List<int>();

			for (int index = 0; index < leng; index++)
			{
				infos[index] = new TreeDataInfo();
				infos[index].m_InParentIndex = -1;
				infos[index].m_MaxIndex = 0;
				infos[index].m_Parent = null;
				infos[index].m_SourData = index;
				infos[index].m_Suns = new List<TreeDataInfo>();
				infos[index].m_Suns.Clear();

				source.Add(index);
			}

			for (int index = 0; index < leng; index++)
			{
				List<int> temp = new List<int>();
				temp.Clear();
				temp.AddRange(source);
				temp.Remove(infos[index].m_SourData);
				CalTree(ref infos[index], temp);
			}
		}

		/// <summary>
		/// 递归计算树
		/// </summary>
		/// <param name="info">树节点</param>
		/// <param name="sxData">剩下的数据</param>
		private void CalTree(ref TreeDataInfo info, List<int> sxData)
		{
			if (sxData.Count == 1)
			{
				TreeDataInfo data = new TreeDataInfo();
				data.m_InParentIndex = info.m_MaxIndex;
				data.m_MaxIndex = 0;
				data.m_SourData = sxData[0];
				data.m_Suns = new List<TreeDataInfo>();
				data.m_Suns.Clear();

				data.m_Parent = info;
				info.m_MaxIndex += 1;
				info.m_Suns.Add(data);
			}
			else
			{
				for (int index = 0; index < sxData.Count; index++)
				{
					TreeDataInfo data = new TreeDataInfo();
					data.m_InParentIndex = info.m_MaxIndex;
					data.m_MaxIndex = 0;
					data.m_SourData = sxData[index];
					data.m_Suns = new List<TreeDataInfo>();
					data.m_Suns.Clear();

					data.m_Parent = info;
					info.m_MaxIndex += 1;
					info.m_Suns.Add(data);
				}

				for (int index = 0; index < info.m_Suns.Count; index++)
				{
					List<int> l = new List<int>();
					l.Clear();
					l.AddRange(sxData);
					l.Remove(info.m_Suns[index].m_SourData);
					TreeDataInfo next = info.m_Suns[index];
					CalTree(ref next, l);
				}
			}
		}

		private void GoBack2()
		{
			Debug.Log("end:" + Time.realtimeSinceStartup);
			if (m_DataReturn != null)
			{
				m_DataReturn(this);
			}
		}
		#endregion
	}
}