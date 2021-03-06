/*需要屏蔽的警告*/
//#pragma warning disable
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

		/// <summary>
		/// 雾的变化速度
		/// </summary>
		private float m_MoveSpeed;

		/// <summary>
		/// 之前的颜色
		/// </summary>
		private Color m_OldColor;

		private void Awake()
		{
			m_ControlCameras = new List<Camera>();
			m_ControlCameras.Clear();

			m_StartFogDistance = 0f;
			m_EndFogDistance = 0f;
			m_EndFogDistance = 1f;
			m_MoveSpeed = 0f;
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
		/// 启动雾效果
		/// </summary>
		/// <param name="fogMode"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="density"></param>
		/// <param name="dtTime"></param>
		/// <param name="useColor"></param>
		/// <param name="c"></param>
		public void StartFog(FogMode fogMode, float start, float end, float density, float dtTime, bool useColor = false, Color c = default(Color))
		{
			RenderSettings.fog = true;
			RenderSettings.fogMode = fogMode;
			m_StartFogDistance = start;
			m_EndFogDistance = end;
			m_FogDensity = density;
			if (RenderSettings.fogMode == FogMode.Linear)
			{
				RenderSettings.fogStartDistance = m_EndFogDistance;
				RenderSettings.fogEndDistance = m_EndFogDistance;

				m_MoveSpeed = (m_EndFogDistance - m_StartFogDistance) / dtTime;

				for (int index = 0; index < m_ControlCameras.Count; index++)
				{
					m_ControlCameras[index].farClipPlane = m_EndFogDistance;
				}
			}
			else
			{
				RenderSettings.fogDensity = 0;
				m_MoveSpeed = m_FogDensity / dtTime;
			}

			m_OldColor = RenderSettings.fogColor;
			if (useColor)
			{
				RenderSettings.fogColor = c;
			}

			StartCoroutine("LineChange");
		}

		/// <summary>
		/// 停止雾效果
		/// </summary>
		public void StopFog()
		{
			StopCoroutine("LineChange");
			RenderSettings.fogColor = m_OldColor;
			RenderSettings.fog = false;
		}

		private IEnumerator LineChange()
		{
			yield return null;

			if (RenderSettings.fogMode == FogMode.Linear)
			{
				while (RenderSettings.fogStartDistance > m_StartFogDistance)
				{
					RenderSettings.fogStartDistance -= m_MoveSpeed * Time.deltaTime;
					yield return null;
				}
			}
			else
			{
				while (RenderSettings.fogDensity < m_FogDensity)
				{
					RenderSettings.fogDensity += m_MoveSpeed * Time.deltaTime;
					yield return null;
				}
			}
		}
	}
}
