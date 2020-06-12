/*
 * Creator:ffm
 * Desc:对象池管理
 * Time:2020/1/19 15:24:46
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Game.Engine
{
	public class ObjectPoolManager : SingletonMonoClass<ObjectPoolManager>
	{
		internal class OPData
		{
			public MethodInfo m_AddO;
			public MethodInfo m_RemoveO;
			public MethodInfo m_GetO;
			public MethodInfo m_RecoveryO;
			public IObjectPool m_O;
		}

		private Dictionary<string, OPData> m_AllPoolDic;

		protected override void Awake()
		{
			base.Awake();
			m_AllPoolDic = new Dictionary<string, OPData>();
			m_AllPoolDic.Clear();
		}

		/// <summary>
		/// 初始化一个对象池
		/// </summary>
		/// <param name="name"></param>
		/// <param name="control"></param>
		public IObjectPool InitPool(string name, string control)
		{
			if (m_AllPoolDic.ContainsKey(name))
			{
				return m_AllPoolDic[name].m_O;
			}

			OPData od = new OPData();
			IObjectPool op = ReflexManager.Instance.CreateClass(control, name) as IObjectPool;
			if (op != null)
			{
				od.m_O = op;
				od.m_AddO = ReflexManager.Instance.GetMethodInfoNonPublic(op.GetType(), "AddObject");
				od.m_RemoveO = ReflexManager.Instance.GetMethodInfoNonPublic(op.GetType(), "RemoveObject");
				od.m_GetO = ReflexManager.Instance.GetMethodInfoNonPublic(op.GetType(), "GetClones");
				od.m_RecoveryO = ReflexManager.Instance.GetMethodInfoNonPublic(op.GetType(), "RecoveryObject");
				m_AllPoolDic.Add(name, od);
				return InitPool(name, control);
			}

			return null;
		}

		/// <summary>
		/// 添加一个本体
		/// </summary>
		/// <param name="name"></param>
		/// <param name="t"></param>
		/// <param name="oc"></param>
		public void AddObject(string name, object t, ObjectPoolControl oc)
		{
			if (!m_AllPoolDic.ContainsKey(name))
			{
				return;
			}

			ReflexManager.Instance.InvkMethod(m_AllPoolDic[name].m_O, m_AllPoolDic[name].m_AddO, t, oc);
		}

		/// <summary>
		/// 移除一个对象
		/// </summary>
		/// <param name="name">对象池名字</param>
		/// <param name="t"></param>
		/// <param name="isAll"></param>
		/// <param name="isClear"></param>
		public void RemoveObject(string name, object t, bool isAll = false, bool isClear = false)
		{
			if (!m_AllPoolDic.ContainsKey(name))
			{
				return;
			}

			ReflexManager.Instance.InvkMethod(m_AllPoolDic[name].m_O, m_AllPoolDic[name].m_RemoveO, t, isAll);
			if (isClear && isAll)
			{
				m_AllPoolDic.Remove(name);
			}
		}

		/// <summary>
		/// 获取一个对象
		/// </summary>
		/// <param name="name"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public ObjectPoolControl GetCloneObject(string name, object t)
		{
			if (!m_AllPoolDic.ContainsKey(name))
			{
				return null;
			}

			object oj = ReflexManager.Instance.InvkMethodReturn(m_AllPoolDic[name].m_O, m_AllPoolDic[name].m_GetO, t);
			if (oj == null)
			{
				return null;
			}

			if (oj is ObjectPoolControl)
			{
				return oj as ObjectPoolControl;
			}

			return null;
		}

		public void RecoveryObject(string name, object t, ObjectPoolControl oc)
		{
			if (!m_AllPoolDic.ContainsKey(name))
			{
				return;
			}

			ReflexManager.Instance.InvkMethod(m_AllPoolDic[name].m_O, m_AllPoolDic[name].m_RecoveryO, t, oc);
		}
	}
}
