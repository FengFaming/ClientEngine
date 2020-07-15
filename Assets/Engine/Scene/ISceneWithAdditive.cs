/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:动态添加场景
 * Time:2020/6/28 15:18:17
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	/// <summary>
	/// 因为解决了动态加载光照贴图的问题，所以这个内容可以弃用
	/// </summary>
	public class ISceneWithAdditive : IScene
	{
		public class LoadEndScene : IResObjectCallBack
		{
			public Action<string> m_End;
			public string m_Name;

			public override void HandleLoadCallBack(object t)
			{
				m_End(m_Name);
			}

			public override int LoadCallbackPriority()
			{
				return 0;
			}
		}

		protected int m_AllLoadScene;
		protected int m_Cout;
		protected Action<float> m_StartAction;

		public ISceneWithAdditive(string name) : base(name)
		{
			m_AllLoadScene = 0;
			m_Cout = 0;
			m_StartAction = null;
		}

		/// <summary>
		/// 读取所有场景数据完成
		/// </summary>
		protected virtual void LoadSceneEnd()
		{
			//new WaitForEndOfFrame();
			//new WaitForSeconds(2f);
			//UnityEngine.SceneManagement.Scene target =
			//	UnityEngine.SceneManagement.SceneManager.GetSceneByName(m_SceneName);
			//while (!target.isLoaded)
			//{
			//	new WaitForEndOfFrame();
			//}

			//UnityEngine.SceneManagement.SceneManager.SetActiveScene(target);
		}

		/// <summary>
		/// 读取场景数据
		/// </summary>
		/// <param name="name"></param>
		protected virtual void LoadEnd(string name)
		{
			//Application.LoadLevelAdditive(name);
			UnityEngine.SceneManagement.SceneManager.LoadScene(name, UnityEngine.SceneManagement.LoadSceneMode.Additive);
			m_Cout++;
			if (m_Cout >= m_AllLoadScene)
			{
				m_StartAction(100);
				LoadSceneEnd();
			}
			else
			{
				float c = (float)m_Cout / (float)m_AllLoadScene;
				c *= 100;
				m_StartAction(c);
			}
		}

		/// <summary>
		/// 加载场景
		/// </summary>
		/// <param name="action"></param>
		public override void LoadScene(Action<float> action)
		{
			m_StartAction = action;
			List<string> vs = new List<string>();
			vs.Add(m_SceneName);

			m_Cout = 0;
			m_AllLoadScene = 1;
			for (int index = 0; index < vs.Count; index++)
			{
				LoadEndScene les = new LoadEndScene();
				les.m_End = LoadEnd;
				les.m_Name = vs[index];
				ResObjectManager.Instance.LoadObject(les.m_Name, ResObjectType.Scene, les);
			}
		}
	}
}
