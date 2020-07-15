/*需要屏蔽的警告*/
//#pragma warning disable
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
	/// 一套光照贴图的数据
	/// </summary>
	public class LightmapInfo
	{
		/// <summary>
		/// 唯一名字
		/// </summary>
		public string m_OnlyName;

		/// <summary>
		/// 有多少张贴图
		/// </summary>
		public int m_TextureCout;

		/// <summary>
		/// 各贴图的详细内容
		/// </summary>
		public List<KeyValuePair<string, string>> m_TextureInfo;
	}

	/// <summary>
	/// 场景光照贴图数据
	/// </summary>
	public class SceneLightInfo
	{
		/// <summary>
		/// 基准数据
		/// </summary>
		public AssetBundle m_ABData;

		/// <summary>
		/// 光照贴图有多少套
		/// </summary>
		public int m_LightmapCout;

		/// <summary>
		/// 所有的光照贴图内容
		/// </summary>
		public Dictionary<string, LightmapInfo> m_AllInfoDic;
	}
}