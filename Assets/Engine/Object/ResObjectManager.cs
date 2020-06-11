#pragma warning disable 0414
#pragma warning disable 0618
/*
 * Creator:ffm
 * Desc:游戏对象加载管理类
 * Time:2019/11/6 16:08:09
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Engine
{
	public enum ResObjectType
	{
		/// <summary>
		/// 被依赖包
		/// </summary>
		AssetBundle,

		/// <summary>
		/// 场景预制内容
		/// </summary>
		GameObject,

		/// <summary>
		/// 材质
		/// </summary>
		Material,

		/// <summary>
		/// 3D图片资源
		/// </summary>
		Texture,

		/// <summary>
		/// ui预制体
		/// </summary>
		UIPrefab,

		/// <summary>
		/// 2D图片资源
		/// </summary>
		Icon,

		/// <summary>
		/// 音效
		/// </summary>
		Aduio,

		/// <summary>
		/// 特效
		/// </summary>
		Effect,

		/// <summary>
		/// 影片
		/// </summary>
		Move,

		/// <summary>
		/// 配置文件
		/// </summary>
		Configuration,

		/// <summary>
		/// lua配置文件
		/// </summary>
		Lua
	}

	public partial class ResObjectManager : SingletonMonoClass<ResObjectManager>
	{
		internal class ABInfo
		{
			/// <summary>
			/// 是不是常驻内存
			/// </summary>
			public bool m_IsDontClear;

			/// <summary>
			/// AB名字
			/// </summary>
			public string m_ABName;

			/// <summary>
			/// AB对象
			/// </summary>
			public AssetBundle m_TargetAB;

			/// <summary>
			/// 上一次使用的时间
			/// </summary>
			public long m_LastUseTime;

			/// <summary>
			/// 被关联的次数
			/// </summary>
			public int m_UseCount;

			public ABInfo()
			{
				m_IsDontClear = false;
				m_UseCount = 0;
				m_LastUseTime = 0;
				m_TargetAB = null;
				m_ABName = string.Empty;
			}
		}

		/// <summary>
		/// 资源加载内容
		/// </summary>
		internal class LoadResObjectInfo
		{
			/// <summary>
			/// 加载类型
			/// </summary>
			public ResObjectType m_LoadType;

			/// <summary>
			/// 加载的名字
			/// </summary>
			public string m_LoadName;

			/// <summary>
			/// 加载完成回调
			/// </summary>
			public List<IResObjectCallBack> m_LoadCB;

			public LoadResObjectInfo() : this(ResObjectType.GameObject, string.Empty)
			{

			}

			public LoadResObjectInfo(ResObjectType type, string name, IResObjectCallBack cb = null)
			{
				m_LoadType = type;
				m_LoadName = name;
				m_LoadCB = new List<IResObjectCallBack>();
				m_LoadCB.Clear();

				if (cb != null)
				{
					m_LoadCB.Add(cb);
				}
			}

			public override bool Equals(object obj)
			{
				LoadResObjectInfo info = obj as LoadResObjectInfo;
				if (m_LoadType == info.m_LoadType)
				{
					return info.m_LoadName == m_LoadName;
				}
				else
				{
					return false;
				}
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override string ToString()
			{
				return string.Format("type:{0}|name:{1}", m_LoadType, m_LoadName);
			}
		}

		/// <summary>
		/// 所有已经加载好了的AB包
		/// </summary>
		private Dictionary<string, ABInfo> m_AllABInfoDic;

		/// <summary>
		/// 记录所有资源包依赖关系的包
		/// </summary>
		private string m_ManifestABName;

		/// <summary>
		/// 资源跟内容
		/// </summary>
		private AssetBundleManifest m_Manifest;

		/// <summary>
		/// 是否初始化成功
		/// </summary>
		private bool m_IsInitSuccess;

		/// <summary>
		/// 资源是否在加载中
		/// </summary>
		private bool m_IsLoad;

		/// <summary>
		/// 需要加载的资源
		/// </summary>
		private List<LoadResObjectInfo> m_NeedLoadInfos;

		/// <summary>
		/// 是否使用AB
		/// 默认不使用AB
		/// </summary>
		private bool m_IsUseAB;
		public bool IsUseAB
		{
			get { return m_IsUseAB; }
			set { m_IsUseAB = value; }
		}

		protected override void Awake()
		{
			base.Awake();
			m_AllABInfoDic = new Dictionary<string, ABInfo>();
			m_AllABInfoDic.Clear();

			m_ManifestABName = string.Empty;
			m_IsInitSuccess = false;

			m_NeedLoadInfos = new List<LoadResObjectInfo>();
			m_NeedLoadInfos.Clear();

			m_IsLoad = false;
			m_IsUseAB = false;
		}

		/// <summary>
		/// 获取AB包的路径
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private string GetABPath(string name)
		{
			return Application.persistentDataPath + "/AB/" + name;
		}

		/// <summary>
		/// 获取AB包的名字
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		private string GetABPath(LoadResObjectInfo info)
		{
			if (info.m_LoadType == ResObjectType.AssetBundle)
			{
				return info.m_LoadName;
			}

			string str = info.m_LoadType.ToString();
			switch (info.m_LoadType)
			{
				case ResObjectType.Configuration:
					str = str + "." + info.m_LoadName + ".xml";
					break;
				default:
					str = str + "." + info.m_LoadName + ".prefab";
					break;
			}

			return str;
		}

		private IEnumerator LoadYiedFunction()
		{
			yield return null;

			m_IsLoad = true;

			while (m_NeedLoadInfos.Count > 0)
			{
				LoadResObjectInfo info = m_NeedLoadInfos[0];
				Debug.Log(info);
				m_NeedLoadInfos.RemoveAt(0);

				string path = GetABPath(info);
				string[] depndencies = m_Manifest.GetAllDependencies(path);
				List<string> s = new List<string>();
				s.Clear();
				for (int index = 0; index < depndencies.Length; index++)
				{
					if (!m_AllABInfoDic.ContainsKey(depndencies[index]))
					{
						s.Add(depndencies[index]);
					}
				}

				if (s.Count > 0)
				{
					for (int index = 0; index < s.Count; index++)
					{
						LoadResObjectInfo i = new LoadResObjectInfo();
						i.m_LoadType = ResObjectType.AssetBundle;
						i.m_LoadName = s[index];
						for (int j = 0; j < m_NeedLoadInfos.Count;)
						{
							if (m_NeedLoadInfos[j].m_LoadType == ResObjectType.AssetBundle &&
								m_NeedLoadInfos[j].m_LoadName == i.m_LoadName)
							{
								m_NeedLoadInfos.RemoveAt(j);
							}
							else
							{
								j++;
							}
						}

						m_NeedLoadInfos.Insert(0, i);
					}

					m_NeedLoadInfos.Add(info);
				}
				else
				{
					if (m_AllABInfoDic.ContainsKey(info.m_LoadName))
					{
						InitGameObject(info);
					}
					else
					{
						string lp = GetABPath(path);
						WWW www = new WWW(lp);
						if (www.assetBundle != null)
						{
							ABInfo ab = new ABInfo();
							ab.m_ABName = info.m_LoadName;
							ab.m_IsDontClear = false;
							ab.m_LastUseTime = (long)Time.time;
							ab.m_TargetAB = www.assetBundle;
							ab.m_UseCount = 0;
							m_AllABInfoDic.Add(info.m_LoadName, ab);
						}
						else
						{
							ABInfo ab = new ABInfo();
							ab.m_ABName = info.m_LoadName;
							ab.m_IsDontClear = true;
							ab.m_LastUseTime = (long)Time.time;
							ab.m_TargetAB = null;
							ab.m_UseCount = 0;
							m_AllABInfoDic.Add(info.m_LoadName, ab);
						}

						www.Dispose();

						if (m_AllABInfoDic.ContainsKey(info.m_LoadName))
						{
							InitGameObject(info);
						}
					}
				}

				yield return null;
			}

			m_IsLoad = false;
		}

		/// <summary>
		/// 初始化对象内容
		/// </summary>
		/// <param name="info"></param>
		private void InitGameObject(LoadResObjectInfo info)
		{
			if (info.m_LoadCB != null && info.m_LoadCB.Count > 0)
			{
				info.m_LoadCB.Sort((IResObjectCallBack a, IResObjectCallBack b) =>
				{
					return a.LoadCallbackPriority() - b.LoadCallbackPriority();
				});

				for (int index = 0; index < info.m_LoadCB.Count; index++)
				{
					object oj = null;
					switch (info.m_LoadType)
					{
						case ResObjectType.Aduio:
							oj = m_AllABInfoDic[info.m_LoadName].m_TargetAB.LoadAsset<AudioClip>(info.m_LoadName);
							break;
						case ResObjectType.Effect:
							GameObject ef = m_AllABInfoDic[info.m_LoadName].m_TargetAB.LoadAsset<GameObject>(info.m_LoadName);
							oj = GameObject.Instantiate(ef);
							break;
						case ResObjectType.GameObject:
							GameObject go = m_AllABInfoDic[info.m_LoadName].m_TargetAB.LoadAsset<GameObject>(info.m_LoadName);
							oj = GameObject.Instantiate(go);
							break;
						case ResObjectType.Icon:
							GameObject ic = m_AllABInfoDic[info.m_LoadName].m_TargetAB.LoadAsset<GameObject>(info.m_LoadName);
							oj = ic.GetComponent<Image>().sprite;
							break;
						case ResObjectType.Material:
							GameObject mt = m_AllABInfoDic[info.m_LoadName].m_TargetAB.LoadAsset<GameObject>(info.m_LoadName);
							oj = mt.GetComponent<MeshRenderer>().material;
							break;
						case ResObjectType.Texture:
							GameObject tt = m_AllABInfoDic[info.m_LoadName].m_TargetAB.LoadAsset<GameObject>(info.m_LoadName);
							oj = tt.GetComponent<MeshRenderer>().material.mainTexture;
							break;
						case ResObjectType.UIPrefab:
							GameObject up = m_AllABInfoDic[info.m_LoadName].m_TargetAB.LoadAsset<GameObject>(info.m_LoadName);
							oj = GameObject.Instantiate(up);
							break;
						case ResObjectType.Move:
							UnityEngine.Object mv = m_AllABInfoDic[info.m_LoadName].m_TargetAB.LoadAsset<UnityEngine.Object>(info.m_LoadName);
							oj = mv;
							break;
						case ResObjectType.Configuration:
							UnityEngine.Object cf = m_AllABInfoDic[info.m_LoadName].m_TargetAB.LoadAsset<UnityEngine.Object>(info.m_LoadName);
							oj = cf;
							break;
						case ResObjectType.Lua:
							TextAsset ta = m_AllABInfoDic[info.m_LoadName].m_TargetAB.LoadAsset<TextAsset>(info.m_LoadName);
							oj = ta;
							break;
					}

					info.m_LoadCB[index].HandleLoadCallBack(oj);
				}
			}
		}
	}
}
