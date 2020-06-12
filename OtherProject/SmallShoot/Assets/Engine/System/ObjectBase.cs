/*
 * Creator:ffm
 * Desc:mono文件的再次封装
 * Time:2019/11/6 16:02:01
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class MouseListen : MonoBehaviour
	{
		/// <summary>
		/// 是谁监听的
		/// </summary>
		private ObjectBase m_ListenTarget;

		/// <summary>
		/// 开启监听
		/// </summary>
		/// <param name="ob"></param>
		public virtual void InitListen(ObjectBase ob)
		{
			m_ListenTarget = ob;
		}

		/// <summary>
		/// 鼠标按下
		/// </summary>
		protected virtual void OnMouseDown()
		{
		}

		/// <summary>
		/// 鼠标弹起
		/// </summary>
		protected virtual void OnMouseUp()
		{
		}

		/// <summary>
		/// 鼠标进入
		/// </summary>
		protected virtual void OnMouseEnter()
		{
		}

		/// <summary>
		/// 鼠标离开
		/// </summary>
		protected virtual void OnMouseExit()
		{
		}

		/// <summary>
		/// 鼠标维持时销毁对象
		/// 比如鼠标在对象身上按下，然后弹起就会触发
		/// </summary>
		protected virtual void OnMouseUpAsButton()
		{
		}
	}

	public class ObjectBase : MonoBehaviour
	{
		/// <summary>
		/// 开启鼠标事件监听
		/// </summary>
		public virtual void StartListenMouse()
		{
			MouseListen listen = this.gameObject.GetComponent<MouseListen>();
			if (listen == null)
			{
				listen = this.gameObject.AddComponent<MouseListen>();
			}

			listen.InitListen(this);
		}

		/// <summary>
		/// 停止鼠标事件监听
		/// </summary>
		public virtual void StopListenMouse()
		{
			MouseListen listen = this.gameObject.GetComponent<MouseListen>();
			if (listen != null)
			{
				GameObject.DestroyImmediate(listen);
			}
		}

		/// <summary>
		/// 监听鼠标进入
		/// </summary>
		protected virtual void ListenMouseEnter()
		{

		}

		/// <summary>
		/// 监听鼠标离开
		/// </summary>
		protected virtual void ListenMouseExit()
		{

		}

		/// <summary>
		/// 监听鼠标按下
		/// </summary>
		protected virtual void ListenMouseDown()
		{

		}

		/// <summary>
		/// 监听弹起
		/// </summary>
		protected virtual void ListenMouseUp()
		{

		}

		/// <summary>
		/// 监听鼠标在身上弹起
		/// </summary>
		protected virtual void ListenMouseUpAsButton()
		{

		}
	}
}
