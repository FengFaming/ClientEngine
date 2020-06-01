/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:lua文件控制的场景基类
 * Time:2020/6/1 14:00:09
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Game.Engine
{
	/// <summary>
	/// 使用lua文件控制的场景
	/// </summary>
	public class IsceneWithLua : IScene
	{
		public class LoadObjectEnd : IResObjectCallBack
		{
			private ResObjectType m_LoadType;
			private string m_LoadName;
			private Action<string> m_LuaFunInfo;
			private Action<object> m_LuaFunObject;
			private Action m_CoutFun;

			public LoadObjectEnd(ResObjectType type, string name, Action<string> luaInfo, Action<object> luaObject, Action fun)
			{
				m_LoadType = type;
				m_LoadName = name;
				m_LuaFunInfo = luaInfo;
				m_LuaFunObject = luaObject;
				m_CoutFun = fun;
			}

			public override void HandleLoadCallBack(object t)
			{
				//throw new NotImplementedException();
				m_CoutFun();
				if (m_LuaFunInfo != null)
				{
					string str = m_LoadType.ToString() + "," + m_LoadName;
					m_LuaFunInfo(str);
				}

				if (m_LuaFunObject != null)
				{
					m_LuaFunObject(t);
				}
			}

			public override int LoadCallbackPriority()
			{
				//throw new NotImplementedException();
				return 0;
			}
		}

		/// <summary>
		/// 启动协程内容的
		/// </summary>
		public class SceneWithLuaMono : MonoBehaviour
		{
			/// <summary>
			/// 开始回调
			/// </summary>
			private Action<float> m_StartSceneFun;

			/// <summary>
			/// 控制对象
			/// </summary>
			public LuaTable m_LuaTable;

			private Dictionary<ResObjectType, List<string>> m_AllNeeds;
			private int m_Cout;
			private int m_TempCout;

			/// <summary>
			/// 开始场景
			/// </summary>
			public void StartScene(Action<float> action)
			{
				m_StartSceneFun = action;
				if (m_LuaTable != null)
				{
					if (m_AllNeeds == null)
					{
						m_AllNeeds = new Dictionary<ResObjectType, List<string>>();
					}

					m_AllNeeds.Clear();
					m_Cout = 0;
					m_TempCout = 0;
					StartCoroutine("StartLoadScene");
				}
				else
				{
					m_StartSceneFun(100);
				}
			}

			/// <summary>
			/// 结束场景
			/// </summary>
			public void EndScene()
			{
				if (m_LuaTable != null)
				{
					Action end = m_LuaTable.Get<Action>("endscene");
					if (end != null)
					{
						end();
					}
				}
			}

			/// <summary>
			/// 加载回调
			/// </summary>
			private void LoadOneObject()
			{
				m_TempCout++;
				float value = (float)m_TempCout / (float)m_Cout;
				value = value * 100f;
				m_StartSceneFun(value);
				if (m_TempCout >= m_Cout)
				{
					Action action = m_LuaTable.Get<Action>("loadend");
					action();
				}
			}

			private IEnumerator StartLoadScene()
			{
				yield return null;

				XLua.LuaFunction luaFunction = m_LuaTable.Get<XLua.LuaFunction>("needloads");
				if (luaFunction != null)
				{
					System.Object[] vs = luaFunction.Call();
					foreach (System.Object o in vs)
					{
						string s = (string)o;
						string[] ss = s.Split(':');
						string[] sss = ss[1].Split(',');
						ResObjectType type = EngineTools.Instance.StringToEnum<ResObjectType>(ss[0]);
						if (m_AllNeeds.ContainsKey(type))
						{
							m_AllNeeds[type].AddRange(sss);
						}
						else
						{
							List<string> n = new List<string>();
							n.AddRange(sss);
							m_AllNeeds.Add(type, n);
						}

						m_Cout += sss.Length;
					}
				}

				yield return null;
				if (m_Cout > 0)
				{
					Action<string> load = m_LuaTable.Get<Action<string>>("loadinfo");
					Action<object> lo = m_LuaTable.Get<Action<object>>("loadobject");
					foreach (KeyValuePair<ResObjectType, List<string>> item in m_AllNeeds)
					{
						for (int index = 0; index < item.Value.Count; index++)
						{
							LoadObjectEnd loe = new LoadObjectEnd(item.Key, item.Value[index], load, lo, LoadOneObject);
							ResObjectManager.Instance.LoadObject(item.Value[index], item.Key, loe);
							yield return null;
						}
					}
				}
			}  
		}

		private LuaTable m_LuaControl;
		private SceneWithLuaMono m_MonoControl;

		public IsceneWithLua(string name) : base(name)
		{

		}

		/// <summary>
		/// 初始化场景
		///		在场景的最开始
		/// </summary>
		/// <returns></returns>
		public override bool InitScene()
		{
			if (!base.InitScene())
				return false;

			m_LuaControl = LuaManager.Instance.CreateTable(m_SceneName);
			if (m_LuaControl == null)
			{
				return false;
			}

			///为什么要在这里创建，因为场景切换的过程当中会删除掉一部分
			GameObject scene = new GameObject();
			scene.name = m_SceneName;
			scene.gameObject.transform.position = Vector3.zero;
			scene.gameObject.transform.eulerAngles = Vector3.zero;
			scene.gameObject.transform.localScale = Vector3.one;
			m_MonoControl = scene.gameObject.AddComponent<SceneWithLuaMono>();
			m_MonoControl.m_LuaTable = m_LuaControl;

			return true;
		}

		/// <summary>
		/// 加载场景
		/// </summary>
		/// <param name="action"></param>
		public override void LoadScene(Action<float> action)
		{
			//base.LoadScene(action);
			if (m_MonoControl != null)
			{
				m_MonoControl.StartScene(action);
			}
		}

		public override void ClearSceneData()
		{
			base.ClearSceneData();
			if (m_MonoControl != null)
			{
				m_MonoControl.EndScene();
			}

			m_MonoControl = null;

			if (m_LuaControl != null)
			{
				m_LuaControl.Dispose();
			}

			m_LuaControl = null;
		}
	}
}
