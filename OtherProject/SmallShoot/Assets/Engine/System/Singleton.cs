/*
 * Creator:ffm
 * Desc:单例基类
 * Time:2019/11/11 10:55:37
* */

using System;

namespace Game.Engine
{
	public class Singleton<T>
	{
		protected Singleton()
		{

		}

		private static T m_Instance;
		public static T Instance
		{
			get
			{
				if (m_Instance == null)
				{
					m_Instance = System.Activator.CreateInstance<T>();
				}

				return m_Instance;
			}
		}
	}
}
