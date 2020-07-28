/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:保存场景光照数据
 * Time:2020/7/14 11:09:59
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;

public class SaveSceneLightmapData : EditorWindow
{
	//[MenuItem("Tools/SaveSceneLightmap")]
	//private static void SaveLightmap()
	//{
	//	string scene = SceneManager.GetActiveScene().name;
	//	string assetsPath = "Assets/" + scene + ".bytes";
	//	string record = Path.Combine(Application.dataPath, scene + ".bytes");
	//	List<string> assetsNames = new List<string>();
	//	assetsNames.Add(assetsPath);

	//	EditorUtility.DisplayProgressBar("save lightmap", "正在生成lightmap的相关配置", 0.1f);
	//	if (File.Exists(record))
	//	{
	//		File.Delete(record);
	//	}

	//	FileStream fs = new FileStream(record, FileMode.OpenOrCreate, FileAccess.Write);
	//	BinaryWriter writer = new BinaryWriter(fs);

	//	int cnt = LightmapSettings.lightmaps.Length;
	//	writer.Write(cnt);
	//	Texture2D[] lmColors = new Texture2D[cnt];
	//	Texture2D[] lmDirs = new Texture2D[cnt];

	//	for (int i = 0; i < cnt; i++)
	//	{
	//		lmColors[i] = LightmapSettings.lightmaps[i].lightmapColor;
	//		lmDirs[i] = LightmapSettings.lightmaps[i].lightmapDir;
	//		writer.Write(lmColors[i] == null ? "" : lmColors[i].name);
	//		writer.Write(lmDirs[i] == null ? "" : lmDirs[i].name);
	//		if (lmColors[i] != null)
	//		{
	//			assetsNames.Add(AssetDatabase.GetAssetPath(lmColors[i]));
	//		}

	//		if (lmDirs[i] != null)
	//		{
	//			assetsNames.Add(AssetDatabase.GetAssetPath(lmDirs[i]));
	//		}
	//	}

	//	writer.Flush();
	//	writer.Close();
	//	fs.Close();
	//	AssetDatabase.ImportAsset(assetsPath);

	//	string ab = Application.dataPath;
	//	ab = ab.Substring(0, ab.Length - ".assets".Length);
	//	//m_SaveFilePath = ab + "/AB" + m_BuildVersion + "/";
	//	string exportTargetPath = ab + "/SceneAB/";
	//	if (!Directory.Exists(exportTargetPath))
	//	{
	//		Directory.CreateDirectory(exportTargetPath);
	//	}

	//	EditorUtility.DisplayProgressBar("save lightmap", "正在生成lightmap的相关配置", 0.4f);
	//	List<AssetBundleBuild> list = new List<AssetBundleBuild>();
	//	AssetBundleBuild build = new AssetBundleBuild();
	//	build.assetBundleName = scene + "_Lightmap.unity3d";
	//	build.assetNames = assetsNames.ToArray();
	//	list.Add(build);
	//	BuildPipeline.BuildAssetBundles(exportTargetPath, list.ToArray(),
	//		BuildAssetBundleOptions.UncompressedAssetBundle,
	//		EditorUserBuildSettings.activeBuildTarget);

	//	EditorUtility.DisplayProgressBar("save lightmap", "正在生成lightmap的相关配置", 0.9f);
	//	Debug.Log("save exit.");
	//	EditorUtility.ClearProgressBar();
	//	AssetDatabase.Refresh();
	//	EditorUtility.OpenWithDefaultApp(exportTargetPath);
	//}

	[MenuItem("Tools/Build/SavePrefabLightmapData")]
	private static void SavePrefab()
	{
		string scene = SceneManager.GetActiveScene().name;
		GameObject go = GameObject.Find(scene);
		if (go == null)
		{
			Debug.LogError("please create not with scene name.");
			return;
		}

		string record = Path.Combine(Application.streamingAssetsPath, "Scene/" + scene + ".bytes");
		if (File.Exists(record))
		{
			File.Delete(record);
		}

		FileStream fs = new FileStream(record, FileMode.OpenOrCreate, FileAccess.Write);
		BinaryWriter writer = new BinaryWriter(fs);

		for (int index = 0; index < go.transform.childCount; index++)
		{
			GameObject child = go.transform.GetChild(index).gameObject;
			UnityEngine.Object oj = PrefabUtility.GetCorrespondingObjectFromSource(child);
			if (oj != null)
			{
				MeshRenderer[] meshRenderer = child.GetComponentsInChildren<MeshRenderer>();
				if (meshRenderer == null || meshRenderer.Length < 1)
				{
					break;
				}

				writer.Write(oj.name);
				writer.Write(meshRenderer.Length);
				for (int i = 0; i < meshRenderer.Length; i++)
				{
					writer.Write(meshRenderer[i].name);
					writer.Write(meshRenderer[i].lightmapIndex);
					writer.Write(meshRenderer[i].lightmapScaleOffset.x);
					writer.Write(meshRenderer[i].lightmapScaleOffset.y);
					writer.Write(meshRenderer[i].lightmapScaleOffset.z);
					writer.Write(meshRenderer[i].lightmapScaleOffset.w);
				}
			}
		}

		writer.Flush();
		writer.Close();
		fs.Close();

		AssetDatabase.Refresh();
	}
}
