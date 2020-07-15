/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:动态添加光照贴图的场景
 * Time:2020/7/15 9:44:51
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using System.IO;

namespace Game.Engine
{
	public class ISceneWithLightmap : IScene
	{
		/// <summary>
		/// 加载数据
		/// </summary>
		public class LoadSceneLightmapData : IResObjectCallBack
		{
			public string m_LightName;
			public Action<string, object> m_LoadEnd;

			public override void HandleLoadCallBack(object t)
			{
				m_LoadEnd(m_LightName, t);
			}

			public override int LoadCallbackPriority()
			{
				return 0;
			}
		}

		/// <summary>
		/// 光照贴图的名称
		/// </summary>
		protected string m_LightmapName;
		public string LightmapName { set { m_LightmapName = value; } }

		/// <summary>
		/// 加载回调
		/// </summary>
		protected Action<float> m_LoadAction;

		/// <summary>
		/// 场景当中存在多少套光照贴图
		/// </summary>
		protected SceneLightInfo m_CurrentInfo;
		public SceneLightInfo CurrentInfo { get { return m_CurrentInfo; } }

		/// <summary>
		/// 当前的光照贴图
		/// </summary>
		protected LightmapInfo m_CurrentLight;

		public ISceneWithLightmap(string name) : base(name)
		{
			m_LightmapName = string.Format(EngineMessageHead.SCENE_LIGHTMAP_COMBINE_NAME, name);
		}

		/// <summary>
		/// 外部修改内容
		/// </summary>
		/// <param name="name"></param>
		public virtual void ChangeLightmapWithName(string name)
		{
			AssetBundle ab = m_CurrentInfo.m_ABData;
			LightmapInfo lightmapInfo = null;
			if (m_CurrentInfo.m_AllInfoDic.ContainsKey(name))
			{
				lightmapInfo = m_CurrentInfo.m_AllInfoDic[name];
			}

			if (lightmapInfo == null)
			{
				return;
			}

			LightmapData[] datas = new LightmapData[lightmapInfo.m_TextureCout];
			for (int index = 0; index < lightmapInfo.m_TextureCout; index++)
			{
				LightmapData data = new LightmapData();
				KeyValuePair<string, string> keyValuePair = lightmapInfo.m_TextureInfo[index];
				if (!string.IsNullOrEmpty(keyValuePair.Key))
				{
					data.lightmapColor = ab.LoadAsset<Texture2D>(keyValuePair.Key);
				}

				if (!string.IsNullOrEmpty(keyValuePair.Value))
				{
					data.lightmapDir = ab.LoadAsset<Texture2D>(keyValuePair.Value);
				}

				datas[index] = data;
			}

			LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
			LightmapSettings.lightmaps = datas;
		}

		/// <summary>
		/// 修改光照贴图内容
		/// </summary>
		/// <param name="name"></param>
		protected virtual void ChangeLightmapName(string name)
		{
			LoadSceneLightmapData loadSceneLightmapData = new LoadSceneLightmapData();
			loadSceneLightmapData.m_LightName = name;
			loadSceneLightmapData.m_LoadEnd = LoadLightmapEnd;
			m_LightmapName = string.Format(EngineMessageHead.SCENE_LIGHTMAP_COMBINE_NAME, name);
			ResObjectManager.Instance.LoadObject(m_LightmapName, ResObjectType.Scene, loadSceneLightmapData);
		}

		/// <summary>
		/// 加载完成
		/// </summary>
		/// <param name="name"></param>
		/// <param name="t"></param>
		protected virtual void LoadLightmapEnd(string name, object t)
		{
			AssetBundle ab = t as AssetBundle;
			TextAsset text = ab.LoadAsset<TextAsset>(name);
			MemoryStream ms = new MemoryStream(text.bytes);
			ms.Position = 0;
			BinaryReader reader = new BinaryReader(ms);

			m_CurrentInfo = new SceneLightInfo();
			m_CurrentInfo.m_ABData = ab;
			int cout = reader.ReadInt32();
			m_CurrentInfo.m_LightmapCout = cout;
			m_CurrentInfo.m_AllInfoDic = new Dictionary<string, LightmapInfo>();
			m_CurrentInfo.m_AllInfoDic.Clear();
			LightmapInfo lightmapInfo = null;
			for (int index = 0; index < cout; index++)
			{
				LightmapInfo map = new LightmapInfo();
				map.m_OnlyName = reader.ReadString();
				map.m_TextureCout = reader.ReadInt32();
				map.m_TextureInfo = new List<KeyValuePair<string, string>>();
				map.m_TextureInfo.Clear();
				for (int i = 0; i < map.m_TextureCout; i++)
				{
					string color = reader.ReadString();
					string dir = reader.ReadString();
					map.m_TextureInfo.Add(new KeyValuePair<string, string>(color, dir));
				}

				m_CurrentInfo.m_AllInfoDic.Add(map.m_OnlyName, map);
				if (lightmapInfo == null)
				{
					lightmapInfo = map;
				}
			}

			LightmapData[] datas = new LightmapData[lightmapInfo.m_TextureCout];
			for (int index = 0; index < lightmapInfo.m_TextureCout; index++)
			{
				LightmapData data = new LightmapData();
				KeyValuePair<string, string> keyValuePair = lightmapInfo.m_TextureInfo[index];
				if (!string.IsNullOrEmpty(keyValuePair.Key))
				{
					data.lightmapColor = ab.LoadAsset<Texture2D>(keyValuePair.Key);
				}

				if (!string.IsNullOrEmpty(keyValuePair.Value))
				{
					data.lightmapDir = ab.LoadAsset<Texture2D>(keyValuePair.Value);
				}

				datas[index] = data;
			}

			//int cnt = reader.ReadInt32();
			//string[] lmcolors = new string[cnt];
			//string[] lmdirs = new string[cnt];
			//LightmapData[] datas = new LightmapData[cnt];
			//for (int i = 0; i < cnt; i++)
			//{
			//	lmcolors[i] = reader.ReadString();
			//	lmdirs[i] = reader.ReadString();
			//	LightmapData data = new LightmapData();
			//	if (!string.IsNullOrEmpty(lmcolors[i]))
			//	{
			//		data.lightmapColor = ab.LoadAsset<Texture2D>(lmcolors[i]);
			//	}

			//	if (!string.IsNullOrEmpty(lmdirs[i]))
			//	{
			//		data.lightmapDir = ab.LoadAsset<Texture2D>(lmdirs[i]);
			//	}

			//	datas[i] = data;
			//}

			reader.Close();
			ms.Close();
			LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
			LightmapSettings.lightmaps = datas;

			if (m_LoadAction != null)
			{
				base.LoadScene(m_LoadAction);
			}
		}

		/// <summary>
		/// 清除数据
		///		清理第一步
		/// </summary>
		public override void ClearSceneData()
		{
			base.ClearSceneData();

			//为什么不需要清理了，是因为场景跳转的时候会自动清理
			//LightmapSettings.lightmaps = null;
		}

		/// <summary>
		/// 卸载场景
		///		清理第二步
		/// </summary>
		/// <param name="action"></param>
		public override void DestroyScene(Action<float> action)
		{
			base.DestroyScene(action);
		}

		/// <summary>
		/// 初始化数据
		///		加载第一步
		/// </summary>
		/// <returns></returns>
		public override bool InitScene()
		{
			return base.InitScene();
		}

		/// <summary>
		/// 加载场景
		///		加载第二步
		/// </summary>
		/// <param name="action"></param>
		public override void LoadScene(Action<float> action)
		{
			m_LoadAction = action;
			ChangeLightmapName(SceneName);
			//base.LoadScene(action);
		}
	}
}
