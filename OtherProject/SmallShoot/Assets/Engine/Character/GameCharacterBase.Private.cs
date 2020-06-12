/*
 * Creator:ffm
 * Desc:私有方法
 * Time:2020/4/13 16:48:19
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public partial class GameCharacterBase : ObjectBase
	{
		protected virtual void Awake()
		{
			CreateNull();
		}

		/// <summary>
		/// 加载对象预制
		/// </summary>
		/// <param name="target"></param>
		protected virtual void LoadObject(object target, Action<object> end)
		{
			GameObject t = target as GameObject;
			t.transform.parent = this.gameObject.transform;
			t.transform.localPosition = Vector3.zero;
			t.transform.localRotation = Quaternion.Euler(Vector3.zero);
			t.transform.localScale = Vector3.one;

			if (end != null)
			{
				end(this);
			}
		}

		/// <summary>
		/// 创建一个空的内容
		/// </summary>
		protected virtual void CreateNull()
		{
			m_HasInit = false;
			m_GCUID = -1;
			m_GCName = "";
			m_ServerID = -1;
			m_LateCameraTime = 1f;
			m_AttriControl = null;
			m_CharacterCamera = null;
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <returns></returns>
		protected virtual bool Update()
		{
			if (m_HasInit)
			{
				if (m_CharacterCamera != null)
				{
					m_CharacterCamera.Update(m_LateCameraTime);
				}

				if (m_CharacterStateManager != null)
				{
					m_CharacterStateManager.UpdateState();
				}

				return true;
			}

			return false;
		}

		protected virtual bool LateUpdate()
		{
			if (m_HasInit)
			{
				if (m_CharacterCamera != null)
				{
					m_CharacterCamera.LateUpdate(m_LateCameraTime);
				}

				return true;
			}

			return false;
		}
	}
}
