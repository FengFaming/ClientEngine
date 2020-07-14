/*需要屏蔽的警告*/
#pragma warning disable 0618
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
		[Tooltip("每隔多久计算一次fps")]
		[SerializeField]
		private float m_UpdateInterval = 0.5f;

		private float m_LastInterval;
		private int m_Frames;

		private float m_FPS;

		[Tooltip("是否显示")]
		[SerializeField]
		private bool m_IsShowFPS = false;

		[Tooltip("屏幕位置")]
		private float m_UISize = 1f;

		[Tooltip("字体大小")]
		[SerializeField]
		private int m_FontSize = 14;

		[Tooltip("显示风格")]
		private GUIStyle m_LogStyle;

		[Tooltip("显示颜色")]
		[SerializeField]
		private Color m_LogColor = new Color(0.6f, 0, 0);

		/// <summary>
		/// 总内存
		/// </summary>
		private uint m_Tam;

		/// <summary>
		/// 总保留内存
		/// </summary>
		private uint m_Trm;

		/// <summary>
		/// 系统显存
		/// </summary>
		private int m_Gms;

		/// <summary>
		/// 系统内存
		/// </summary>
		private int m_Sms;

		/// <summary>
		/// 系统核心数
		/// </summary>
		private int m_PC;

		/// <summary>
		/// 显示字符串
		/// </summary>
		private string m_ShowStr;

		protected void Awake()
		{
			m_LastInterval = Time.realtimeSinceStartup;
			m_Frames = 0;
			m_FPS = 0;
			m_ShowStr = string.Empty;
		}

		private void Start()
		{
			m_Gms = SystemInfo.graphicsMemorySize;
			m_Sms = SystemInfo.systemMemorySize;
			m_PC = SystemInfo.processorCount;

			if (Application.isMobilePlatform)
			{
				m_UISize = Screen.dpi / 295;
			}

			m_LogStyle = new GUIStyle();
			m_FontSize = (int)(m_FontSize * m_UISize);
			m_LogStyle.normal.textColor = m_LogColor;
			m_LogStyle.fontSize = m_FontSize;
		}

		private void OnGUI()
		{
			if (m_IsShowFPS)
			{
				GUI.skin.textField.fontSize = m_FontSize;
				GUI.skin.button.fontSize = m_FontSize;

				m_ShowStr = string.Empty;
				m_ShowStr = string.Format("系统显存:{0} 系统内存:{1} 核心数:{2}\n总内存:{3} 总保留内存:{4}\nFPS:{5}",
											m_Gms, m_Sms, m_PC, m_Tam, m_Trm, m_FPS);
				GUI.TextField(new Rect(0, 0, 300 * m_UISize, 70 * m_UISize), m_ShowStr);
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

					m_Tam = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory() / 1024 / 1024;
					m_Trm = UnityEngine.Profiling.Profiler.GetTotalReservedMemory() / 1024 / 1024;
				}
			}
		}
	}
}
