/*
 * Creator:ffm
 * Desc:对象池虚基类
 * Time:2020/1/19 15:30:25
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public abstract class IObjectPool
	{
		/// <summary>
		/// 本体内容
		/// </summary>
		private Dictionary<object, ObjectPoolControl> m_NoumenonDic;

		/// <summary>
		/// 克隆体
		/// </summary>
		private Dictionary<object, List<ObjectPoolControl>> m_Clones;

		/// <summary>
		/// 对象池的唯一名字
		/// </summary>
		protected string m_PoolName;

		public IObjectPool(string name)
		{
			m_PoolName = name;
			m_NoumenonDic = new Dictionary<object, ObjectPoolControl>();
			m_NoumenonDic.Clear();

			m_Clones = new Dictionary<object, List<ObjectPoolControl>>();
			m_Clones.Clear();
		}

		/// <summary>
		/// 添加一个本体内容
		/// </summary>
		/// <param name="t"></param>
		/// <param name="noumenon"></param>
		protected virtual void AddObject(object t, ObjectPoolControl noumenon)
		{
			noumenon.OneObjectData = t;
			noumenon.SaveCrashTime = GameTimeManager.Instance.GameNowTime;
			if (!m_NoumenonDic.ContainsKey(t))
			{
				m_NoumenonDic.Add(t, noumenon);
			}
		}

		/// <summary>
		/// 移除本体
		/// </summary>
		/// <param name="t"></param>
		/// <param name="isAll"></param>
		protected virtual void RemoveObject(object t, bool isAll)
		{
			if (isAll)
			{
				foreach (KeyValuePair<object, ObjectPoolControl> item in m_NoumenonDic)
				{
					DestroyObject(item.Value);
				}

				m_NoumenonDic.Clear();

				foreach (KeyValuePair<object, List<ObjectPoolControl>> item in m_Clones)
				{
					for (int index = 0; index < item.Value.Count; index++)
					{
						DestroyObject(item.Value[index]);
					}
				}

				m_Clones.Clear();
			}
			else
			{
				if (m_NoumenonDic.ContainsKey(t))
				{
					DestroyObject(m_NoumenonDic[t]);
					m_NoumenonDic.Remove(t);
				}

				if (m_Clones.ContainsKey(t))
				{
					for (int index = 0; index < m_Clones[t].Count; index++)
					{
						DestroyObject(m_Clones[t][index]);
					}

					m_Clones.Remove(t);
				}
			}
		}

		/// <summary>
		/// 得到一个克隆体内容
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		protected virtual ObjectPoolControl GetClones(object t)
		{
			if (!m_NoumenonDic.ContainsKey(t))
			{
				return null;
			}

			ObjectPoolControl rc;

			if (m_Clones.ContainsKey(t))
			{
				if (m_Clones[t].Count > 0)
				{
					rc = m_Clones[t][0];
					m_Clones[t].RemoveAt(0);
				}
				else
				{
					rc = CloneObject(m_NoumenonDic[t]);
				}
			}
			else
			{
				rc = CloneObject(m_NoumenonDic[t]);
			}

			InitlizeObject(rc);
			return rc;
		}

		/// <summary>
		/// 回收一个克隆体
		/// </summary>
		/// <param name="t"></param>
		/// <param name="clones"></param>
		protected virtual void RecoveryObject(object t, ObjectPoolControl clones)
		{
			if (!m_NoumenonDic.ContainsKey(t))
			{
				return;
			}

			InitlizeObject(clones);
			clones.SaveCrashTime = GameTimeManager.Instance.GameNowTime;
			if (m_Clones.ContainsKey(t))
			{
				m_Clones[t].Add(clones);
			}
			else
			{
				List<ObjectPoolControl> cs = new List<ObjectPoolControl>();
				cs.Clear();
				cs.Add(clones);
				m_Clones.Add(t, cs);
			}
		}

		/// <summary>
		/// 初始化一个对象
		/// </summary>
		/// <param name="oc"></param>
		protected abstract void InitlizeObject(ObjectPoolControl oc);

		/// <summary>
		/// 克隆一个对象
		/// </summary>
		/// <param name="oc"></param>
		/// <returns></returns>
		protected abstract ObjectPoolControl CloneObject(ObjectPoolControl oc);

		/// <summary>
		/// 销毁一个对象
		/// </summary>
		/// <param name="oc"></param>
		protected abstract void DestroyObject(ObjectPoolControl oc);
	}
}
