/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:场景光照贴图管理
 * Time:2020/7/14 10:45:01
* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Engine
{
	/// <summary>
	/// 场景关照贴图管理，
	///		主要是控制贴图数量的
	/// </summary>
	public class SceneLightmapManager : SingletonMonoClass<SceneLightmapManager>
	{
		public class LoadLightmapBack : IResObjectCallBack
		{
			public string m_SceneName;
			public Action<string, object> m_EndAction;

			public override void HandleLoadCallBack(object t)
			{
				m_EndAction(m_SceneName, t);
			}

			public override int LoadCallbackPriority()
			{
				return 0;
			}
		}

		/// <summary>
		/// 设置场景光照贴图
		/// </summary>
		public void SetSceneLightmap()
		{
			string scene = GameSceneManager.Instance.Current.SceneName;
			string name = scene + "_Lightmap";
			LoadLightmapBack resObjectCallBackBase = new LoadLightmapBack();
			resObjectCallBackBase.m_SceneName = scene;
			resObjectCallBackBase.m_EndAction = LoadLightmapAB;
			ResObjectManager.Instance.LoadObject(name, ResObjectType.Scene, resObjectCallBackBase);
		}

		private void LoadLightmapAB(string scene, object t)
		{
			AssetBundle ab = t as AssetBundle;
			TextAsset text = ab.LoadAsset<TextAsset>(scene);
			MemoryStream ms = new MemoryStream(text.bytes);
			ms.Position = 0;
			BinaryReader reader = new BinaryReader(ms);
			int cnt = reader.ReadInt32();
			string[] lmcolors = new string[cnt];
			string[] lmdirs = new string[cnt];
			LightmapData[] datas = new LightmapData[cnt];
			for (int i = 0; i < cnt; i++)
			{
				lmcolors[i] = reader.ReadString();
				lmdirs[i] = reader.ReadString();
				LightmapData data = new LightmapData();
				if (!string.IsNullOrEmpty(lmcolors[i]))
				{
					data.lightmapColor = ab.LoadAsset<Texture2D>(lmcolors[i]);
				}

				if (!string.IsNullOrEmpty(lmdirs[i]))
				{
					data.lightmapDir = ab.LoadAsset<Texture2D>(lmdirs[i]);
				}

				datas[i] = data;
			}

			reader.Close();
			ms.Close();
			LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
			LightmapSettings.lightmaps = datas;
		}
	}
}
