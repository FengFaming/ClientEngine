#pragma warning disable 0618
/*
 * Creator:ffm
 * Desc:游戏对象加载管理类对外接口
 * Time:2019/11/11 14:21:40
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Engine
{
	public partial class ResObjectManager : SingletonMonoClass<ResObjectManager>
	{
		/// <summary>
		/// 初始化或者说启用这个管理类
		/// </summary>
		/// <param name="manifest"></param>
		public void InitResManager(string manifest)
		{
			m_ManifestABName = manifest;
			WWW mani = new WWW(GetABPath(m_ManifestABName));
			if (mani != null && string.IsNullOrEmpty(mani.error))
			{
				m_Manifest = mani.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

				ABInfo info = new ABInfo();
				info.m_ABName = m_ManifestABName;
				info.m_IsDontClear = true;
				info.m_LastUseTime = (long)Time.time;
				info.m_TargetAB = mani.assetBundle;
				info.m_UseCount = 0;

				m_AllABInfoDic.Add(info.m_ABName, info);
				m_IsInitSuccess = true;
			}

			mani.Dispose();
		}

		/// <summary>
		/// 移除某一个正在加载的数据
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="cb"></param>
		public void RemoveLoadObject(string name, ResObjectType type, IResObjectCallBack cb)
		{
			LoadResObjectInfo info = new LoadResObjectInfo();
			info.m_LoadName = name;
			info.m_LoadType = type;
			info.m_LoadCB.Add(cb);

			if (m_NeedLoadInfos.Contains(info))
			{
				for (int index = 0; index < m_NeedLoadInfos.Count; index++)
				{
					if (m_NeedLoadInfos[index].Equals(info))
					{
						m_NeedLoadInfos[index].m_LoadCB.Remove(cb);
						if (m_NeedLoadInfos[index].m_LoadCB.Count <= 0)
						{
							m_NeedLoadInfos.RemoveAt(index);
						}

						break;
					}
				}
			}
		}

		/// <summary>
		/// 清理内容
		/// </summary>
		public void ClearAll()
		{
			StopAllCoroutines();
			m_NeedLoadInfos.Clear();
			List<string> cs = new List<string>();
			cs.Clear();
			foreach (var info in m_AllABInfoDic)
			{
				if (!info.Value.m_IsDontClear)
				{
					cs.Add(info.Key);
				}
			}

			if (cs.Count > 0)
			{
				for (int index = 0; index < cs.Count; index++)
				{
					ABInfo info = m_AllABInfoDic[cs[index]];
					m_AllABInfoDic.Remove(cs[index]);
					info.m_TargetAB.Unload(false);
				}
			}

			m_IsLoad = false;
		}

		/// <summary>
		/// 加载资源
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="cb"></param>
		public void LoadObject(string name, ResObjectType type, IResObjectCallBack cb)
		{
			if (!m_IsUseAB)
			{
#if UNITY_EDITOR
				LoadResObjectInfo info = new LoadResObjectInfo();
				info.m_LoadCB.Add(cb);
				info.m_LoadName = name;
				info.m_LoadType = type;
				string str = "Assets/UseAB/" + info.m_LoadType.ToString();
				switch (info.m_LoadType)
				{
					case ResObjectType.Configuration:
						str = str + "/" + info.m_LoadName + ".xml";
						break;
					case ResObjectType.Lua:
						str = str + "/" + info.m_LoadName + ".lua.txt";
						break;
					default:
						str = str + "/" + info.m_LoadName + ".prefab";
						break;
				}

				UnityEngine.Object oj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(str);
				if (oj == null)
				{
					Debug.Log("the res is null." + info);
					return;
				}

				if (cb != null)
				{
					if (info.m_LoadType != ResObjectType.Lua)
					{
						if (info.m_LoadType == ResObjectType.Icon)
						{
							Sprite s = (oj as GameObject).gameObject.GetComponent<Image>().sprite;
							cb.HandleLoadCallBack(s);
						}
						else
						{
							UnityEngine.Object target = UnityEngine.Object.Instantiate(oj);
							cb.HandleLoadCallBack(target);
						}
					}
					else
					{
						cb.HandleLoadCallBack(oj);
					}
				}
#endif
			}
			else
			{
				if (m_IsInitSuccess)
				{
					LoadResObjectInfo info = new LoadResObjectInfo();
					info.m_LoadName = name;
					info.m_LoadType = type;
					info.m_LoadCB.Add(cb);

					if (m_NeedLoadInfos.Contains(info))
					{
						for (int index = 0; index < m_NeedLoadInfos.Count; index++)
						{
							if (m_NeedLoadInfos[index].Equals(info))
							{
								m_NeedLoadInfos[index].m_LoadCB.Add(cb);
								break;
							}
						}
					}
					else
					{
						m_NeedLoadInfos.Add(info);
					}

					if (!m_IsLoad)
					{
						StartCoroutine("LoadYiedFunction");
					}
				}
			}
		}
	}
}
