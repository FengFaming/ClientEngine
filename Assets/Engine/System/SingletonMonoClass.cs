/*
 * Creator:ffm
 * Desc:mono单例类
 * Time:2019/11/11 11:14:45
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class SingletonMonoClass<T> : ObjectBase where T : ObjectBase
	{
		private static T m_Instance;
		public static T Instance
		{
			get
			{
				if (m_Instance == null)
				{
					GameObject go = new GameObject();
					go.name = typeof(T).ToString();
					go.transform.position = Vector3.zero;
					go.transform.eulerAngles = Vector3.zero;
					go.transform.localScale = Vector3.one;

					m_Instance = go.gameObject.AddComponent<T>();
					GameObject.DontDestroyOnLoad(go);
				}

				return m_Instance;
			}
		}

		protected virtual void Awake()
		{
			if (m_Instance == null)
			{
				m_Instance = this.gameObject.GetComponent<T>();
				GameObject.DontDestroyOnLoad(m_Instance.gameObject);
			}
		}
	}
}
