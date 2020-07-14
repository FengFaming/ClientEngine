/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:2048游戏控制器
 * Time:2020/6/2 10:12:42
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Game.Engine
{
	public class NumberGameControl
	{
		/// <summary>
		/// 移动类型
		/// </summary>
		public enum MoveType
		{
			/// <summary>
			/// 左移
			/// </summary>
			Left = 1,

			/// <summary>
			/// 右移
			/// </summary>
			Right,

			/// <summary>
			/// 上移
			/// </summary>
			Up,

			/// <summary>
			/// 下移
			/// </summary>
			Down
		}

		/// <summary>
		/// 所有得数据
		/// </summary>
		private int[,] m_AllDatas;

		/// <summary>
		/// 逻辑table
		/// </summary>
		private XLua.LuaTable m_LuaTable;

		/// <summary>
		/// 数组大小
		/// </summary>
		private Vector2Int m_MaxLW;

		/// <summary>
		/// 是否是第一次
		/// </summary>
		private bool m_IsOne;

		/// <summary>
		/// 下一个数据
		/// </summary>
		private LuaFunction m_NextNumber;

		/// <summary>
		/// 初始化控制内容
		/// </summary>
		/// <param name="lw">行列号</param>
		/// <param name="luaTable">lua逻辑
		///		如果有，那么使用lua进行逻辑内容，如果没有，那么就使用默认得</param>
		public NumberGameControl(Vector2Int lw, XLua.LuaTable luaTable = null)
		{
			m_MaxLW = lw;
			m_AllDatas = new int[m_MaxLW.x, m_MaxLW.y];
			for (int i = 0; i < m_MaxLW.x; i++)
			{
				for (int j = 0; j < m_MaxLW.y; j++)
				{
					m_AllDatas[i, j] = 0;
				}
			}

			m_LuaTable = luaTable;
			m_IsOne = true;
			m_NextNumber = null;

			if (m_LuaTable != null)
			{
				Action<string> action = m_LuaTable.Get<Action<string>>("init");
				if (action != null)
				{
					action(m_MaxLW.x + "," + m_MaxLW.y);
				}
			}
		}

		/// <summary>
		/// 移动数据
		/// </summary>
		/// <param name="moveType"></param>
		/// <returns></returns>
		public bool MoveNumber(MoveType moveType, bool next = true)
		{
			if (m_IsOne)
			{
				if (m_LuaTable == null)
				{
					List<Vector2Int> rangs = new List<Vector2Int>();
					for (int i = 0; i < m_MaxLW.x; i++)
					{
						rangs.Add(new Vector2Int(i, 0));
					}

					for (int i = 0; i < m_MaxLW.y; i++)
					{
						rangs.Add(new Vector2Int(0, i));
					}

					int cout = UnityEngine.Random.Range(2, rangs.Count - 1);
					cout = UnityEngine.Random.Range(2, cout);
					for (int index = 0; index < cout; index++)
					{
						int c = UnityEngine.Random.Range(1, 2);
						c = UnityEngine.Random.Range(1, c);
						int id = UnityEngine.Random.Range(0, rangs.Count - 1);
						m_AllDatas[rangs[id].x, rangs[id].y] = (int)Math.Pow(2, c);
						rangs.RemoveAt(id);
					}
				}
				else
				{
					if (m_NextNumber == null)
					{
						m_NextNumber = m_LuaTable.Get<LuaFunction>("nextNumber");
					}

					if (m_NextNumber == null)
					{
						Debug.LogError("the lua not has [nextNumber] methed.");
						return false;
					}

					System.Object[] lua = m_NextNumber.Call(true, 0);
					foreach (System.Object o in lua)
					{
						Vector3 number = EngineTools.Instance.StringToVector3((string)o);
						int l = (int)number.x;
						int w = (int)number.y;
						int d = (int)number.z;
						m_AllDatas[l, w] = d;
					}
				}

				m_IsOne = false;
				return true;
			}

			switch (moveType)
			{
				case MoveType.Down:
					MoveHeight(-1);
					break;
				case MoveType.Up:
					MoveHeight(1);
					break;
				case MoveType.Left:
					MoveWith(1);
					break;
				case MoveType.Right:
					MoveWith(-1);
					break;
			}

			if (next)
			{
				if (!NextNumber(moveType))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 主动移动下一个数据
		///		返回值判定是否已经失败
		/// </summary>
		/// <param name="moveType"></param>
		public bool NextNumber(MoveType moveType)
		{
			if (m_LuaTable == null)
			{
				List<Vector2Int> rangs = new List<Vector2Int>();
				for (int i = 0; i < m_MaxLW.x; i++)
				{
					for (int j = 0; j < m_MaxLW.y; j++)
					{
						if (m_AllDatas[i, j] == 0)
						{
							rangs.Add(new Vector2Int(i, j));
						}
					}
				}

				if (rangs.Count == 0)
				{
					return false;
				}

				if (rangs.Count > 0)
				{
					int max = rangs.Count - 1;
					max = max > m_MaxLW.x * m_MaxLW.y * 0.25 ? (int)(m_MaxLW.x * m_MaxLW.y * 0.25) : max;
					int s = 2;
					s = s > max ? max : s;
					int cout = UnityEngine.Random.Range(s, max);
					cout = UnityEngine.Random.Range(s, cout);
					if (max == 0)
					{
						int c = UnityEngine.Random.Range(1, 3);
						c = UnityEngine.Random.Range(1, c);
						m_AllDatas[rangs[0].x, rangs[0].y] = (int)Math.Pow(2, c);
						rangs.Clear();
					}
					else
					{
						for (int index = 0; index < cout; index++)
						{
							int c = UnityEngine.Random.Range(1, 3);
							c = UnityEngine.Random.Range(1, c);
							int id = UnityEngine.Random.Range(0, rangs.Count - 1);
							m_AllDatas[rangs[id].x, rangs[id].y] = (int)Math.Pow(2, c);
							rangs.RemoveAt(id);
						}
					}
				}

				return true;
			}
			else
			{
				if (m_NextNumber == null)
				{
					return false;
				}

				System.Object[] lua = m_NextNumber.Call(false, (int)moveType);
				if (lua.Length == 1 && (string)lua[0] == "Max")
				{
					return false;
				}

				foreach (System.Object o in lua)
				{
					Vector3 number = EngineTools.Instance.StringToVector3((string)o);
					int l = (int)number.x;
					int w = (int)number.y;
					int d = (int)number.z;
					m_AllDatas[l, w] = d;
				}

				return true;
			}
		}

		/// <summary>
		/// 获取对应数据
		/// </summary>
		/// <param name="l"></param>
		/// <param name="w"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool GetLWData(int l, int w, out int data)
		{
			data = 0;
			if (l >= m_MaxLW.x || w >= m_MaxLW.y ||
				l < 0 || w < 0)
			{
				return false;
			}

			data = m_AllDatas[l, w];
			return true;
		}

		/// <summary>
		/// 垂直移动
		/// </summary>
		/// <param name="cout"></param>
		private void MoveHeight(int cout)
		{
			for (int index = 0; index < m_MaxLW.x; index++)
			{
				if (cout > 0)
				{
					for (int i = 0; i < m_MaxLW.y; i++)
					{
						int s = i + 1;
						while (s < m_MaxLW.y)
						{
							///只要是s大于零，不管相等不相等，都退出查找
							if (m_AllDatas[index, s] > 0)
							{
								if (m_AllDatas[index, i] == 0)
								{
									m_AllDatas[index, i] = m_AllDatas[index, s];
									m_AllDatas[index, s] = 0;
									i--;
								}
								else if (m_AllDatas[index, i] == m_AllDatas[index, s])
								{
									m_AllDatas[index, i] *= 2;
									m_AllDatas[index, s] = 0;
								}

								break;
							}
							else
							{
								s++;
							}
						}
					}
				}
				else
				{
					for (int i = m_MaxLW.y - 1; i >= 0; i--)
					{
						int s = i - 1;
						while (s >= 0)
						{
							///只要是s大于零，不管相等不相等，都退出查找
							if (m_AllDatas[index, s] > 0)
							{
								if (m_AllDatas[index, i] == 0)
								{
									m_AllDatas[index, i] = m_AllDatas[index, s];
									m_AllDatas[index, s] = 0;
									i++;
								}
								else if (m_AllDatas[index, i] == m_AllDatas[index, s])
								{
									m_AllDatas[index, i] *= 2;
									m_AllDatas[index, s] = 0;
								}

								break;
							}
							else
							{
								s--;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 水平移动
		/// </summary>
		/// <param name="cout"></param>
		private void MoveWith(int cout)
		{
			for (int index = 0; index < m_MaxLW.x; index++)
			{
				if (cout > 0)
				{
					for (int i = 0; i < m_MaxLW.x; i++)
					{
						int s = i + 1;
						while (s < m_MaxLW.x)
						{
							if (m_AllDatas[s, index] > 0)
							{
								if (m_AllDatas[s, index] == m_AllDatas[i, index])
								{
									m_AllDatas[i, index] *= 2;
									m_AllDatas[s, index] = 0;
								}
								else if (m_AllDatas[i, index] == 0)
								{
									m_AllDatas[i, index] = m_AllDatas[s, index];
									m_AllDatas[s, index] = 0;
									i--;
								}

								break;
							}
							else
							{
								s++;
							}
						}
					}
				}
				else
				{
					for (int i = m_MaxLW.x - 1; i >= 0; i--)
					{
						int s = i - 1;
						while (s >= 0)
						{
							if (m_AllDatas[s, index] > 0)
							{
								if (m_AllDatas[i, index] == 0)
								{
									m_AllDatas[i, index] = m_AllDatas[s, index];
									m_AllDatas[s, index] = 0;
									i++;
								}
								else if (m_AllDatas[i, index] == m_AllDatas[s, index])
								{
									m_AllDatas[i, index] *= 2;
									m_AllDatas[s, index] = 0;
								}

								break;
							}
							else
							{
								s--;
							}
						}
					}
				}
			}
		}
	}
}
