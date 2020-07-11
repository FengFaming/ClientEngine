/*
 * Creator:ffm
 * Desc:场景基类
 * Time:2020/4/11 17:31:07
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

namespace Game.Engine
{
	public class IScene
	{
		/// <summary>
		/// 场景名字
		/// </summary>
		protected string m_SceneName;
		public string SceneName { get { return m_SceneName; } }

		public IScene(string name)
		{
			m_SceneName = name;
		}

		/// <summary>
		/// 初始化一些数据
		/// </summary>
		public virtual bool InitScene()
		{
			return true;
		}

		/// <summary>
		/// 加载场景
		/// </summary>
		/// <param name="action">返回加载长度</param>
		public virtual void LoadScene(Action<float> action)
		{
			action(100);
		}

		/// <summary>
		/// 清除数据
		///		比如监听内容等等
		/// </summary>
		public virtual void ClearSceneData()
		{

		}

		/// <summary>
		/// 移除场景内容
		/// </summary>
		/// <param name="action">返回加载长度</param>
		public virtual void DestroyScene(Action<float> action)
		{
			action(100);
		}
	}
}
