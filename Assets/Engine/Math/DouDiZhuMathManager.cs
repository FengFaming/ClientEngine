/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:斗地主数学相关内容
 * Time:2020/5/19 14:14:55
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	/// <summary>
	/// 斗地主玩家数据
	/// </summary>
	public class PuKeWanJia
	{
		/// <summary>
		/// 玩家ID
		/// </summary>
		public int m_WanJiaID;

		/// <summary>
		/// 是否是地主
		/// </summary>
		public bool m_IsDiZhu;

		/// <summary>
		/// 座位号
		/// </summary>
		public int m_ZuoWeiHao;

		/// <summary>
		/// 所有的扑克牌
		/// </summary>
		public List<PuKePai> m_PuKePais;

		public PuKeWanJia()
		{
			m_WanJiaID = 0;
			m_IsDiZhu = false;
			m_ZuoWeiHao = 0;
			m_PuKePais = new List<PuKePai>();
			m_PuKePais.Clear();
		}

		/// <summary>
		/// 构建同样数据
		/// </summary>
		/// <param name="other"></param>
		public PuKeWanJia(PuKeWanJia other)
		{
			m_WanJiaID = other.m_WanJiaID;
			m_IsDiZhu = other.m_IsDiZhu;
			m_ZuoWeiHao = other.m_ZuoWeiHao;
			m_PuKePais = new List<PuKePai>();
			m_PuKePais.Clear();
			if (other.m_PuKePais.Count > 0)
			{
				m_PuKePais.AddRange(other.m_PuKePais);
			}
		}
	}

	public class PuKePai
	{
		/// <summary>
		/// 扑克牌的颜色
		/// </summary>
		public int m_PuKeColor;

		/// <summary>
		/// 扑克牌的点数
		/// </summary>
		public int m_PuKeDianShu;

		/// <summary>
		/// 扑克牌实体
		/// </summary>
		public Sprite m_PuKeTexture;

		/// <summary>
		/// 获取扑克牌序号
		/// </summary>
		/// <returns></returns>
		public int GetPuKeID()
		{
			return m_PuKeColor * 100 + m_PuKeDianShu;
		}

		/// <summary>
		/// 获取没有颜色的ID
		/// </summary>
		/// <returns></returns>
		public int GetNotColorID()
		{
			if (m_PuKeColor == 5)
			{
				return 100;
			}
			else
			{
				return m_PuKeDianShu;
			}
		}

		/// <summary>
		/// 交换ID
		/// </summary>
		/// <returns></returns>
		public int SwithID()
		{
			if (m_PuKeColor == 5)
			{
				return m_PuKeColor * 100 + m_PuKeDianShu;
			}

			return m_PuKeDianShu;
		}

		/// <summary>
		/// 设置内容
		/// </summary>
		/// <param name="all"></param>
		public bool GetPuKeTexture(Dictionary<int, List<PuKePai>> all)
		{
			if (!all.ContainsKey(m_PuKeColor))
			{
				return false;
			}

			for (int index = 0; index < all[m_PuKeColor].Count; index++)
			{
				if (all[m_PuKeColor][index].m_PuKeDianShu == m_PuKeDianShu)
				{
					m_PuKeTexture = all[m_PuKeColor][index].m_PuKeTexture;
					return true;
				}
			}

			return false;
		}
	}

	public class DouDiZhuMathManager : MonoBehaviour
	{
		private static DouDiZhuMathManager m_Instance;
		private void Awake()
		{
			m_Instance = this;
			m_IsCaling = false;
		}

		/// <summary>
		/// 玩家数据
		/// </summary>
		private PuKeWanJia m_ControlData;

		/// <summary>
		/// 计算的出牌内容
		/// </summary>
		private Action<PuKeWanJia, List<PuKePai>> m_CalEndAction;

		/// <summary>
		/// 是否是主动出牌
		/// </summary>
		private bool m_IsSelfOrOther;

		/// <summary>
		/// 被动出牌的内容
		/// </summary>
		private List<PuKePai> m_OtherPais;

		/// <summary>
		/// 已经打出的牌
		/// </summary>
		private List<PuKePai> m_YiJingUsePai;

		/// <summary>
		/// 其他玩家的手牌数
		/// </summary>
		private Dictionary<bool, int> m_OtherCout;

		/// <summary>
		/// 是否在计算当中
		/// </summary>
		private bool m_IsCaling;

		/// <summary>
		/// 得到唯一内容
		///		区别于框架内的数据只是因为这个内容使用的时候才需要
		/// </summary>
		/// <returns></returns>
		public static DouDiZhuMathManager GetDouDiZhuManager()
		{
			if (m_Instance == null)
			{
				GameObject go = new GameObject();
				go.name = "DouDiZhuMathManager";
				go.transform.position = Vector3.zero;
				go.transform.rotation = Quaternion.identity;
				go.transform.localScale = Vector3.one;
				m_Instance = go.AddComponent<DouDiZhuMathManager>();
			}

			return m_Instance;
		}

		/// <summary>
		/// 清空自己
		/// </summary>
		public void DestroyThis()
		{
			m_Instance = null;
			GameObject.Destroy(this.gameObject);
		}

		/// <summary>
		/// 开始计算出牌
		/// </summary>
		/// <param name="wanjia">自己牌</param>
		/// <param name="end">结束回调</param>
		/// <param name="used">已经使用了的牌</param>
		/// <param name="oc">其他玩家牌数</param>
		/// <param name="self">是否主动</param>
		/// <param name="other">其他玩家出的牌</param>
		public void StartCal(PuKeWanJia wanjia, Action<PuKeWanJia, List<PuKePai>> end,
								List<PuKePai> used, Dictionary<bool, int> oc,
								bool self = true, List<PuKePai> other = null)
		{
			if (!m_IsCaling)
			{
				m_IsCaling = true;
				m_ControlData = new PuKeWanJia(wanjia);
				m_CalEndAction = end;
				m_YiJingUsePai = used;
				m_OtherCout = oc;
				m_IsSelfOrOther = self;
				m_OtherPais = other;

				StartCoroutine("CalPai");
			}
		}

		/// <summary>
		/// 计算出牌
		/// </summary>
		/// <returns></returns>
		private IEnumerator CalPai()
		{
			yield return null;

			GetShouShu();

			//是否主动出牌
			if (m_IsSelfOrOther)
			{
				///判断自己是不是地主
				if (m_ControlData.m_IsDiZhu)
				{

				}
				else
				{

				}
			}
			else
			{

			}

			m_IsCaling = false;
		}

		/// <summary>
		/// 获取自己出牌的手数
		/// </summary>
		/// <returns></returns>
		private int GetShouShu()
		{
			if (m_ControlData.m_PuKePais.Count == 1)
			{
				return 1;
			}
			else
			{
				///由大到小计算
				m_ControlData.m_PuKePais.Sort((PuKePai p1, PuKePai p2) =>
				{
					return p2.SwithID() - p1.SwithID();
				});

				List<PuKePai> danzhan = new List<PuKePai>();
				List<PuKePai> duizi = new List<PuKePai>();
				List<PuKePai> sanzhang = new List<PuKePai>();
				List<PuKePai> sizhang = new List<PuKePai>();
				List<PuKePai> daxiaowang = new List<PuKePai>();

				for (int index = 0; index < m_ControlData.m_PuKePais.Count;)
				{
					int id = index + 4;
					while (id >= m_ControlData.m_PuKePais.Count)
					{
						id--;
					}

					///如果计算下来差值相等，说明到了最后一张
					if (id == index)
					{
						danzhan.Add(m_ControlData.m_PuKePais[index]);
					}

					while (m_ControlData.m_PuKePais[id].GetNotColorID() !=
						m_ControlData.m_PuKePais[index].GetNotColorID() &&
						id > index)
					{
						id--;
					}

					if (m_ControlData.m_PuKePais[index].m_PuKeColor == 5)
					{
						if (id == index)
						{
							danzhan.Add(m_ControlData.m_PuKePais[index]);
							index++;
						}
						else
						{
							daxiaowang.Add(m_ControlData.m_PuKePais[index]);
							daxiaowang.Add(m_ControlData.m_PuKePais[index + 1]);
							index += 2;
						}
					}
					else
					{
						if (id == index)
						{
							danzhan.Add(m_ControlData.m_PuKePais[index]);
							index++;
						}
						else
						{
							int zs = id - index;
							switch (zs)
							{
								case 1:
									duizi.Add(m_ControlData.m_PuKePais[index]);
									duizi.Add(m_ControlData.m_PuKePais[index + 1]);
									index += 2;
									break;
								case 2:
									sanzhang.Add(m_ControlData.m_PuKePais[index]);
									sanzhang.Add(m_ControlData.m_PuKePais[index + 1]);
									sanzhang.Add(m_ControlData.m_PuKePais[index + 2]);
									index += 3;
									break;
								case 3:
									sizhang.Add(m_ControlData.m_PuKePais[index]);
									sizhang.Add(m_ControlData.m_PuKePais[index + 1]);
									sizhang.Add(m_ControlData.m_PuKePais[index + 2]);
									sizhang.Add(m_ControlData.m_PuKePais[index + 3]);
									index += 4;
									break;
							}
						}
					}
				}
			}

			return 1;
		}
	}
}
