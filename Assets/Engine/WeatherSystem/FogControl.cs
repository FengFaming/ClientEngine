/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:雾控制
 * Time:2020/6/22 18:34:25
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

namespace Game.Engine
{
	public class FogControl : ObjectBase
	{
		/// <summary>
		/// 所有控制的摄像机
		/// </summary>
		private List<Camera> m_ControlCameras;

		/// <summary>
		/// 开始深度
		/// </summary>
		private float m_StartFogDistance;

		/// <summary>
		/// 结束深度
		/// </summary>
		private float m_EndFogDistance;

		/// <summary>
		/// 雾浓度
		/// 只有当模式是指数或者指数平方的时候起作用
		/// </summary>
		private float m_FogDensity;

		private void Awake()
		{
			m_ControlCameras = new List<Camera>();
			m_ControlCameras.Clear();

			m_StartFogDistance = 0f;
			m_EndFogDistance = 0f;
			m_EndFogDistance = 1f;

			AddCamera(Camera.main);
			StartFog(FogMode.Linear, Camera.main.nearClipPlane, Camera.main.farClipPlane, 0);
		}

		/// <summary>
		/// 添加摄像机
		/// </summary>
		/// <param name="camera"></param>
		public void AddCamera(Camera camera)
		{
			if (!m_ControlCameras.Contains(camera))
			{
				m_ControlCameras.Add(camera);
			}
		}

		/// <summary>
		/// 启用雾效果
		/// </summary>
		/// <param name="fogMode"></param>
		/// <param name="size"></param>
		/// <param name="density"></param>
		public void StartFog(FogMode fogMode, float start, float end, float density)
		{
			RenderSettings.fog = true;
			RenderSettings.fogMode = fogMode;
			m_StartFogDistance = start;
			m_EndFogDistance = end;
			m_FogDensity = density;
			if (RenderSettings.fogMode == FogMode.Linear)
			{
				RenderSettings.fogStartDistance = m_StartFogDistance;
				RenderSettings.fogEndDistance = m_EndFogDistance;

				for (int index = 0; index < m_ControlCameras.Count; index++)
				{
					m_ControlCameras[index].farClipPlane = m_EndFogDistance;
				}
			}
			else
			{
				RenderSettings.fogDensity = m_FogDensity;
			}
		}
	}
}
