/*
 * Creator:ffm
 * Desc:面板文字
 * Time:3/6/2019 9:14:02 AM
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Engine
{
	public class UText : Text
	{
		[Tooltip("显示钥匙")]
		[SerializeField]
		private string m_DescKey;

		/// <summary>
		/// 是否使用Key显示过
		/// </summary>
		private bool m_IsShowKey;

		protected override void Awake()
		{
			base.Awake();
			m_IsShowKey = false;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			ShowKeyDesc();
		}

		private void ShowKeyDesc()
		{
			if (!m_IsShowKey)
			{
				//if (SGGameLanguageManager.Instance != null)
				//{
				//	text = SGGameLanguageManager.Instance.GetDescWithKey(m_DescKey);
				//	m_IsShowKey = true;
				//}
			}
		}
	}
}
