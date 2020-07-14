/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:ui内容包含lua数据
 * Time:2020/5/6 15:16:20
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using XLua;
using System.IO;

namespace Game.Engine
{
	public class UIModelLuaControl : IUIModelControl
	{
		/// <summary>
		/// lua控制内容
		/// </summary>
		protected LuaTable m_UILuaTable;

		/// <summary>
		/// lua名字
		/// </summary>
		protected string m_LuaName;

		/// <summary>
		/// lua.Update
		/// </summary>
		protected Action m_LuaUpdate;

		/// <summary>
		/// lua.Destroy
		/// </summary>
		protected Action m_LuaDestroy;

		public UIModelLuaControl() : base()
		{
			m_LuaName = string.Empty;
		}

		public override void CloseSelf(bool manager = false)
		{
			base.CloseSelf(manager);

			if (m_LuaDestroy != null)
			{
				m_LuaDestroy();
			}

			if (m_UILuaTable != null)
			{
				m_UILuaTable.Dispose();
			}

			m_UILuaTable = null;
			m_LuaUpdate = null;
			m_LuaDestroy = null;
		}

		/// <summary>
		/// 打开自己
		/// </summary>
		/// <param name="target"></param>
		public override void OpenSelf(GameObject target)
		{
			base.OpenSelf(target);

			///为什么要再这里进行创建，因为再这里方便设置自身
			m_UILuaTable = LuaManager.Instance.CreateTable(m_LuaName, this);
			Action<GameObject> luaAwake = m_UILuaTable.Get<Action<GameObject>>("init");
			m_UILuaTable.Get("update", out m_LuaUpdate);
			m_UILuaTable.Get("destroy", out m_LuaDestroy);

			if (luaAwake != null)
			{
				luaAwake(m_ControlTarget);
			}
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <returns></returns>
		public override bool Update()
		{
			if (!base.Update())
				return false;

			if (m_LuaUpdate != null)
			{
				m_LuaUpdate();
			}

			return true;
		}
	}
}
