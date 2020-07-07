/*
 * Creator:ffm
 * Desc:角色属性基类
 * Time:2020/4/13 16:27:35
* */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Engine
{
	public class GameCharacterAttributeBase
	{
		/// <summary>
		/// 所有的属性内容
		/// </summary>
		protected Dictionary<int, double> m_AttrDic;

		/// <summary>
		/// 需要进行监听的属性列
		/// </summary>
		protected List<int> m_NeedListenAttris;

		/// <summary>
		/// 所属角色的唯一ID
		/// </summary>
		protected int m_GCUID;

		public GameCharacterAttributeBase(int uid)
		{
			m_GCUID = uid;

			m_AttrDic = new Dictionary<int, double>();
			m_AttrDic.Clear();

			m_NeedListenAttris = new List<int>();
			m_NeedListenAttris.Clear();
		}

		/// <summary>
		/// 添加或者修改属性内容
		/// </summary>
		/// <param name="id">属性ID</param>
		/// <param name="value">属性值</param>
		/// <param name="isListen">是否需要给外界监听</param>
		public void AddAttribute(int id, double value, bool isListen = false)
		{
			if (isListen)
			{
				if (!m_NeedListenAttris.Contains(id))
				{
					m_NeedListenAttris.Add(id);
				}
			}

			if (m_AttrDic.ContainsKey(id))
			{
				if (m_AttrDic[id] != value)
				{
					double temp = m_AttrDic[id];
					m_AttrDic[id] = value;

					if (m_NeedListenAttris.Contains(id))
					{
						string head = string.Format(EngineMessageHead.CHANGE_CHARACTER_ATTRIBUTE_VALUE, m_GCUID, id);
						MessageManger.Instance.SendMessage(head, temp, value);
					}
				}
			}
			else
			{
				m_AttrDic.Add(id, value);
				if (m_NeedListenAttris.Contains(id))
				{
					string head = string.Format(EngineMessageHead.CHANGE_CHARACTER_ATTRIBUTE_VALUE, m_GCUID, id);
					MessageManger.Instance.SendMessage(head, temp, value);
				}
			}
		}

		/// <summary>
		/// 判断一下是否存在对应ID
		/// </summary>
		/// <param name="id">属性ID</param>
		/// <param name="value">属性值</param>
		/// <returns>是否存在</returns>
		public bool HasAttributeID(int id, ref double value)
		{
			if (m_AttrDic.ContainsKey(id))
			{
				value = m_AttrDic[id];
				return true;
			}

			return false;
		}

		/// <summary>
		/// 移除某一个属性
		/// </summary>
		/// <param name="id"></param>
		public void RemoveArrtibute(int id)
		{
			if (m_AttrDic.ContainsKey(id))
			{
				m_AttrDic.Remove(id);
			}

			if (m_NeedListenAttris.Contains(id))
			{
				m_NeedListenAttris.Remove(id);
			}
		}

		/// <summary>
		/// 清除所有
		/// </summary>
		public void Clear()
		{
			m_NeedListenAttris.Clear();
			m_AttrDic.Clear();
		}
	}
}
