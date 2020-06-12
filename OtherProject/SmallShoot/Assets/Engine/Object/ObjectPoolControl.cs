/*
 * Creator:ffm
 * Desc:对象控制
 * Time:2020/1/19 15:33:01
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class ObjectPoolControl
	{
		/// <summary>
		/// 完整标识唯一
		/// </summary>
		private object m_OneObjectData;
		public object OneObjectData
		{
			get { return m_OneObjectData; }
			set { m_OneObjectData = value; }
		}

		/// <summary>
		/// 存入时间
		/// </summary>
		private int m_SaveCrashTime;
		public int SaveCrashTime
		{
			get { return m_SaveCrashTime; }
			set { m_SaveCrashTime = value; }
		}
	}
}
