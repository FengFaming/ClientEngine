/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:保存场景的光照贴图数据
 * Time:2020/7/15 11:16:23
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEditor;
using System.IO;

public class SaveLightmapDatas : EditorWindow
{
	[MenuItem("Tools/Build/SaveSceneLightmapData")]
	private static void Create()
	{
		EditorWindow.GetWindow(typeof(SaveLightmapDatas));
	}

	/// <summary>
	/// 场景名字
	/// </summary>
	private string m_SceneName;

	/// <summary>
	/// 保存节点名字
	/// </summary>
	private string m_CopyFileName;

	/// <summary>
	/// 保存的文件名字
	/// </summary>
	private string m_SaveFileName;

	/// <summary>
	/// 所有数据
	/// </summary>
	private SceneLightInfo m_AllInfo;

	/// <summary>
	/// 是否自检完成
	/// </summary>
	private bool m_JCSurress = false;

	private List<string> m_AllInfoNames;

	private void OnGUI()
	{
		GUILayout.Label("Save Lightmap Data");
		m_SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		m_CopyFileName = EditorGUILayout.TextField("节点名字:", m_CopyFileName);
		if (GUILayout.Button("检测"))
		{
			JC();
		}

		if (m_JCSurress)
		{
			GUILayout.BeginVertical();
			GUILayout.Label("所有节点，点击可删除节点:");
			for (int index = 0; index < m_AllInfoNames.Count; index++)
			{
				if (GUILayout.Button(m_AllInfoNames[index]))
				{
					string name = m_AllInfoNames[index];
					m_AllInfo.m_AllInfoDic.Remove(name);
					m_AllInfoNames.Remove(name);
				}
			}
			GUILayout.EndVertical();
		}

		if (GUILayout.Button("新增"))
		{
			AddData();
		}

		if (GUILayout.Button("保存"))
		{
			SaveData();
		}
	}

	private void SaveData()
	{
		FileStream fs = new FileStream(m_SaveFileName, FileMode.OpenOrCreate, FileAccess.Write);
		BinaryWriter writer = new BinaryWriter(fs);
		List<string> assetsNames = new List<string>();
		string record = m_SaveFileName.Substring(m_SaveFileName.LastIndexOf("Assets/"));
		assetsNames.Add(record);

		writer.Write(m_AllInfo.m_LightmapCout);
		foreach (var item in m_AllInfo.m_AllInfoDic)
		{
			LightmapInfo info = item.Value;
			writer.Write(info.m_OnlyName);
			writer.Write(info.m_TextureCout);
			for (int index = 0; index < info.m_TextureCout; index++)
			{
				writer.Write(info.m_TextureInfo[index].Key);
				writer.Write(info.m_TextureInfo[index].Value);
				if (!string.IsNullOrEmpty(info.m_TextureInfo[index].Key))
				{
					string p = "Assets/Art/Lightmaps/" + m_SceneName + "/" + info.m_OnlyName + "/" + info.m_TextureInfo[index].Key + ".exr";
					Texture2D obj = AssetDatabase.LoadAssetAtPath<Texture2D>(p);
					Debug.Log(p);
					assetsNames.Add(AssetDatabase.GetAssetPath(obj));
				}

				if (!string.IsNullOrEmpty(info.m_TextureInfo[index].Value))
				{
					string p = "Assets/Art/Lightmaps/" + m_SceneName + "/" + info.m_OnlyName + "/" + info.m_TextureInfo[index].Value + ".png";
					Texture2D obj = AssetDatabase.LoadAssetAtPath<Texture2D>(p);
					Debug.Log(p);
					assetsNames.Add(AssetDatabase.GetAssetPath(obj));
				}
			}
		}

		writer.Flush();
		writer.Close();
		fs.Close();
		AssetDatabase.ImportAsset(record);

		string ab = Application.dataPath;
		ab = ab.Substring(0, ab.Length - ".assets".Length);
		//m_SaveFilePath = ab + "/AB" + m_BuildVersion + "/";
		string exportTargetPath = ab + "/SceneAB/";
		if (!Directory.Exists(exportTargetPath))
		{
			Directory.CreateDirectory(exportTargetPath);
		}

		EditorUtility.DisplayProgressBar("save lightmap", "正在生成lightmap的相关配置", 0.4f);
		List<AssetBundleBuild> list = new List<AssetBundleBuild>();
		AssetBundleBuild build = new AssetBundleBuild();
		string abname = string.Format(EngineMessageHead.SCENE_LIGHTMAP_COMBINE_NAME, m_SceneName);
		build.assetBundleName = abname + ".unity3d";
		build.assetNames = assetsNames.ToArray();
		list.Add(build);
		BuildPipeline.BuildAssetBundles(exportTargetPath, list.ToArray(),
			BuildAssetBundleOptions.UncompressedAssetBundle,
			EditorUserBuildSettings.activeBuildTarget);

		EditorUtility.DisplayProgressBar("save lightmap", "正在生成lightmap的相关配置", 0.9f);
		Debug.Log("save exit.");
		EditorUtility.ClearProgressBar();
		AssetDatabase.Refresh();
		EditorUtility.OpenWithDefaultApp(exportTargetPath);
	}

	private void AddData()
	{
		if (string.IsNullOrEmpty(m_CopyFileName))
		{
			Debug.Log("请输入节点名字");
			return;
		}

		if (m_AllInfo != null)
		{
			if (m_AllInfo.m_AllInfoDic.ContainsKey(m_CopyFileName))
			{
				Debug.Log("节点名字重复");
				return;
			}
		}

		string path = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
		path = path.Substring(0, path.LastIndexOf("/"));
		path = path + "/" + m_SceneName;
		if (!Directory.Exists(path))
		{
			Debug.Log("请先烘焙场景");
			return;
		}

		if (m_AllInfo == null)
		{
			m_AllInfo = new SceneLightInfo();
			m_AllInfo.m_LightmapCout = 0;
			m_AllInfo.m_AllInfoDic = new Dictionary<string, LightmapInfo>();
			m_AllInfo.m_AllInfoDic.Clear();
			m_AllInfoNames = new List<string>();
			m_AllInfoNames.Clear();
		}

		string copyPath = Application.dataPath + "/Art/Lightmaps/" + m_SceneName + "/" + m_CopyFileName;
		if (Directory.Exists(copyPath))
		{
			Directory.Delete(copyPath, true);
		}

		Directory.CreateDirectory(copyPath);
		CopyFile(path, copyPath);

		LightmapInfo map = new LightmapInfo();
		map.m_OnlyName = m_CopyFileName;
		map.m_TextureCout = LightmapSettings.lightmaps.Length;
		map.m_TextureInfo = new List<KeyValuePair<string, string>>();
		map.m_TextureInfo.Clear();
		for (int index = 0; index < map.m_TextureCout; index++)
		{
			string color = LightmapSettings.lightmaps[index].lightmapColor == null ? "" : LightmapSettings.lightmaps[index].lightmapColor.name;
			string dir = LightmapSettings.lightmaps[index].lightmapDir == null ? "" : LightmapSettings.lightmaps[index].lightmapDir.name;
			if (color != "")
			{
				color = m_CopyFileName + color;
			}

			if (dir != "")
			{
				dir = m_CopyFileName + dir;
			}

			KeyValuePair<string, string> kv = new KeyValuePair<string, string>(color, dir);
			map.m_TextureInfo.Add(kv);
		}

		m_AllInfo.m_LightmapCout++;
		m_AllInfoNames.Add(m_CopyFileName);
		m_AllInfo.m_AllInfoDic.Add(map.m_OnlyName, map);
		m_JCSurress = true;
		AssetDatabase.Refresh();
	}

	/// <summary>
	/// 复制文件
	/// </summary>
	private void CopyFile(string scene, string target)
	{
		string[] files = Directory.GetFiles(scene);
		foreach (string f in files)
		{
			FileInfo i = new FileInfo(f);
			string fullName = i.FullName;
			fullName = fullName.Substring(fullName.LastIndexOf(".") + 1);
			if (fullName == "png" || fullName == "exr")
			{
				string save = target + "/" + m_CopyFileName + i.Name;
				File.Copy(f, save, true);
			}
		}
	}

	/// <summary>
	/// 检测场景数据
	/// </summary>
	private void JC()
	{
		m_SaveFileName = Application.dataPath;
		if (!Directory.Exists(m_SaveFileName))
		{
			Directory.CreateDirectory(m_SaveFileName);
		}

		m_SaveFileName = m_SaveFileName + "/Art/Lightmaps/" + m_SceneName;
		if (!Directory.Exists(m_SaveFileName))
		{
			Directory.CreateDirectory(m_SaveFileName);
		}

		m_SaveFileName = m_SaveFileName + "/" + m_SceneName + ".bytes";
		if (File.Exists(m_SaveFileName))
		{
			FileStream fs = new FileStream(m_SaveFileName, FileMode.Open);
			BinaryReader reader = new BinaryReader(fs);
			int cout = reader.ReadInt32();
			m_AllInfo = new SceneLightInfo();
			m_AllInfoNames = new List<string>();
			m_AllInfo.m_LightmapCout = cout;
			m_AllInfo.m_AllInfoDic = new Dictionary<string, LightmapInfo>();
			m_AllInfo.m_AllInfoDic.Clear();
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

				m_AllInfoNames.Add(map.m_OnlyName);
				m_AllInfo.m_AllInfoDic.Add(map.m_OnlyName, map);
			}

			reader.Close();
			fs.Close();
			File.Delete(m_SaveFileName);

			m_JCSurress = true;
		}
	}
}
