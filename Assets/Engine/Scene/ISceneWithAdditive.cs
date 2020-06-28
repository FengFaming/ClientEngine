/*需要屏蔽的警告*/
#pragma warning disable
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
	public class ISceneWithAdditive : IScene
	{
		public class SceneWithAdditive : ObjectBase
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

			private Action<float> m_StartAction;
			private int m_Cout;
			private int m_AC;

			public virtual void EndScene()
			{
				m_Cout = 0;
				m_AC = 0;
			}

			public virtual void StartScene(Action<float> action, List<string> names)
			{
				m_AC = names.Count;
				m_StartAction = action;
				for (int index = 0; index < m_AC; index++)
				{
					LoadEndScene les = new LoadEndScene();
					les.m_End = LoadEnd;
					les.m_Name = names[index];
					ResObjectManager.Instance.LoadObject(les.m_Name, ResObjectType.Scene, les);
				}
				//LoadEndScene
				//  ResObjectCallBackBase rbs = new ResObjectCallBackBase();
				//
				//rbs.m_LoadType = ResObjectType.Scene;
				//rbs.m_FinshFunction = LoadEnd;
				//ResObjectManager.Instance.LoadObject(name, ResObjectType.Scene, rbs);
			}

			protected virtual void LoadEnd(string name)
			{
				Application.LoadLevelAdditive(name);
				m_Cout++;
				if (m_Cout >= m_AC)
				{
					m_StartAction(100);
				}
				else
				{
					float c = (float)m_Cout / (float)m_AC;
					c *= 100;
					m_StartAction(c);
				}
			}
		}

		private SceneWithAdditive m_SceneControl;
		public ISceneWithAdditive(string name) : base(name)
		{
			m_SceneControl = null;
		}

		/// <summary>
		/// 初始化场景
		/// </summary>
		public override bool InitScene()
		{
			if (!base.InitScene())
				return false;

			///为什么要在这里创建，因为场景切换的过程当中会删除掉一部分
			GameObject scene = new GameObject();
			scene.name = m_SceneName;
			scene.gameObject.transform.position = Vector3.zero;
			scene.gameObject.transform.eulerAngles = Vector3.zero;
			scene.gameObject.transform.localScale = Vector3.one;
			m_SceneControl = scene.gameObject.AddComponent<SceneWithAdditive>();
			return true;
		}

		/// <summary>
		/// 清理场景
		/// </summary>
		public override void ClearSceneData()
		{
			base.ClearSceneData();
			m_SceneControl.EndScene();
			m_SceneControl = null;
		}

		/// <summary>
		/// 加载场景
		/// </summary>
		/// <param name="action"></param>
		public override void LoadScene(Action<float> action)
		{
			List<string> vs = new List<string>();
			vs.Add(m_SceneName);
			vs.Add("t1");
			m_SceneControl.StartScene(action, vs);
		}
	}
}
