/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:lua管理器
 * Time:2020/5/8 9:41:23
* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

namespace Game.Engine
{
	[LuaCallCSharp]
	public class LuaManager : SingletonMonoClass<LuaManager>
	{
		private static readonly LuaEnv M_LUA_ENV = new LuaEnv();
		public LuaEnv MyLuaEnv { get { return M_LUA_ENV; } }

		/// <summary>
		/// 创建一个table
		/// </summary>
		/// <param name="fileName">lua名字</param>
		/// <returns></returns>
		public LuaTable CreateTable(string fileName)
		{
			LuaTable target = M_LUA_ENV.NewTable();

			LuaTable temp = M_LUA_ENV.NewTable();
			temp.Set("__index", M_LUA_ENV.Global);
			target.SetMetaTable(temp);
			temp.Dispose();

			string path = Application.dataPath + "/UseAB/Lua/" + fileName + ".lua.txt";
			if (File.Exists(path))
			{
				string ta = File.ReadAllText(path);
				M_LUA_ENV.DoString(ta, fileName, target);
			}
			else
			{
				Debug.LogError(string.Format("the file[{0}]is null.", path));
				target.Dispose();
			}

			return target;
		}

		/// <summary>
		/// 创建一个Table并且设置自己
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		public LuaTable CreateTable(string fileName, object self)
		{
			LuaTable target = CreateTable(fileName);
			if (self != null)
			{
				target.Set("self", self);
				return target;
			}

			target.Dispose();
			return null;
		}

		/// <summary>
		/// 查看内容
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public GameObject GetGameObject(GameObject parent, string name)
		{
			Transform tf = parent.transform.Find(name);
			if (tf != null)
			{
				return tf.gameObject;
			}

			return null;
		}

		/// <summary>
		/// 获取对应的component
		/// </summary>
		/// <param name="target"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public Component GetComp(GameObject target, string name)
		{
			Component t = target.GetComponent(name);
			return t;
		}

		#region UI相关
		/// <summary>
		/// 打开相关
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="layer"></param>
		/// <param name="arms"></param>
		public void OpenUI(string ui, string layer, params object[] arms)
		{
			UILayer l = EngineTools.Instance.StringToEnum<UILayer>(layer);
			UIManager.Instance.OpenUI(ui, l, arms);
		}

		/// <summary>
		/// 获取相关
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="layer"></param>
		/// <returns></returns>
		public IUIModelControl GetUI(string ui, string layer)
		{
			UILayer l = EngineTools.Instance.StringToEnum<UILayer>(layer);
			return UIManager.Instance.GetShowUI(ui, l);
		}

		/// <summary>
		/// 关闭界面
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="layer"></param>
		public void CloseUI(string ui, string layer)
		{
			UILayer l = EngineTools.Instance.StringToEnum<UILayer>(layer);
			UIManager.Instance.RecoveryUIModel(ui, l);
		}
		#endregion

		#region 场景相关
		public void ChangeScene(string name)
		{

		}
		#endregion

		private void Update()
		{
			if (M_LUA_ENV != null)
			{
				M_LUA_ENV.Tick();
			}
		}
	}
}
