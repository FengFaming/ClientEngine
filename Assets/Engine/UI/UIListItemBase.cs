/*
 * Creator:ffm
 * Desc:Item基类
 * Time:3/5/2019 9:53:57 AM
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Engine
{
	public class UIListItemBase : MonoBehaviour
	{
		/// <summary>
		/// Item Index.
		/// </summary>
		[HideInInspector]
		private int m_ItemIndex;
		public int ItemIndex
		{
			get { return m_ItemIndex; }
			set { m_ItemIndex = value; }
		}

		/// <summary>
		/// Item Data.
		/// </summary>
		[HideInInspector]
		private object m_ItemData;
		protected object ItemData
		{
			get { return m_ItemData; }
		}

		/// <summary>
		/// Clear Item Data.
		/// </summary>
		public virtual void ClearData()
		{
			m_ItemData = null;
		}

		/// <summary>
		/// Set Item Data.
		/// </summary>
		/// <param name="data"></param>
		public virtual void SetData(object data)
		{
			this.m_ItemData = data;
		}

		/// <summary>
		/// Compared Data.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public virtual bool ComparedData(object data)
		{
			return true;
		}

		/// <summary>
		/// Get Item Element Data.
		/// </summary>
		/// <returns></returns>
		public LayoutElement GetItemElement()
		{
			LayoutElement elemt = this.gameObject.GetComponent<LayoutElement>();
			if (elemt == null)
				return default(LayoutElement);

			return elemt;
		}

		/// <summary>
		/// Get Item Data As T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetItemData<T>()
		{
			if (m_ItemData != null)
				return ((T)m_ItemData);

			return default(T);
		}
	}
}