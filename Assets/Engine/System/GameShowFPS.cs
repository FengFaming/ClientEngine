/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:fps显示
 * Time:2020/4/28 11:21:40
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class GameShowFPS : ObjectBase
	{
		/// <summary>
		/// 每格多久计算一次fps
		/// </summary>
		[Tooltip("每格多久计算一次fps")]
		public float m_UpdateInterval = 0.5f;

		private float m_LastInterval;
		private int m_Frames;

		private float m_FPS;
		private bool m_IsShowFPS;
		public bool IsShowFPS
		{
			get { return m_IsShowFPS; }
			set { m_IsShowFPS = value; }
		}

		protected void Awake()
		{
			m_LastInterval = Time.realtimeSinceStartup;
			m_Frames = 0;
			m_FPS = 0;
		}

		private void OnGUI()
		{
			if (m_IsShowFPS)
			{
				GUI.color = Color.red;
				GUIStyle style = new GUIStyle();
				style.fontSize = 35;
				style.normal.textColor = Color.red;

				GUI.Label(new Rect(0, 0, 400, 400), "FPS:" + m_FPS.ToString("f2"), style);
			}
		}

		protected void Update()
		{
			if (m_IsShowFPS)
			{
				m_Frames++;
				if (Time.realtimeSinceStartup > m_LastInterval + m_UpdateInterval)
				{
					m_FPS = m_Frames / (Time.realtimeSinceStartup - m_LastInterval);
					m_Frames = 0;
					m_LastInterval = Time.realtimeSinceStartup;
				}
			}
		}
	}
}
