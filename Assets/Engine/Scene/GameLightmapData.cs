/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:游戏光照数据
 * Time:2020/7/10 10:00:48
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

namespace Game.Engine
{
	/// <summary>
	/// 渲染对象身上的光照贴图
	/// </summary>
	[System.Serializable]
	public struct RendererInfo
	{
		/// <summary>
		/// 渲染主体
		/// </summary>
		public Renderer m_Renderer;

		/// <summary>
		/// 光照贴图标号
		/// </summary>
		public int m_LightmapIndex;

		/// <summary>
		/// 四维矩阵
		///		用来描述贴图位置矩形的
		/// </summary>
		public Vector4 m_LightmapOffsetScale;
	}

	/// <summary>
	/// 光照贴图信息
	/// </summary>
	public struct DyncRenderInfo
	{
		/// <summary>
		/// 光照贴图标号
		/// </summary>
		public int m_LightIndex;

		/// <summary>
		/// 大小矩形
		/// </summary>
		public Vector4 m_LightOffsetScale;

		/// <summary>
		/// 对象hash值
		/// </summary>
		public int m_Hash;

		/// <summary>
		/// 对象位置
		/// </summary>
		public Vector3 m_Pos;
	}

	/// <summary>
	/// 场景Fog信息
	/// </summary>
	[System.Serializable]
	public struct FogInfo
	{
		public bool m_Fog;
		public FogMode m_FogModel;
		public Color m_FogColor;
		public float m_FogStartDistance;
		public float m_FogEndDistance;
		public float m_FogDensity;
	}
}