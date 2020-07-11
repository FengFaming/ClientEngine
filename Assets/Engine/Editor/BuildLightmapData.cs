/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:创建光照贴图ab包
 * Time:2020/7/10 10:11:30
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;

public class BuildLightmapData : ObjectBase
{
	[MenuItem("Tools/Build LightMapData")]
	private static void CreateLightMatData()
	{
		RecordLightmapOffsetScale(null);
		return;

		string scene = SceneManager.GetActiveScene().name;
		string asset_path = "Assets/" + scene + ".bytes";
		string record = Path.Combine(Application.dataPath, scene + ".bytes");

		///需要打包到同一个AB的内容
		List<string> assetNames = new List<string>();
		assetNames.Add(asset_path);

		EditorUtility.DisplayProgressBar("创建光照贴图信息", "正在生成相关配置", 0.1f);
		if (File.Exists(record))
		{
			File.Delete(record);
		}

		///获取写出节点
		FileStream fs = new FileStream(record, FileMode.OpenOrCreate, FileAccess.Write);
		BinaryWriter writer = new BinaryWriter(fs);

		///写入光照贴图数量
		int cnt = LightmapSettings.lightmaps.Length;
		writer.Write(cnt);

		//所有光照贴图
		Texture2D[] lmColors = new Texture2D[cnt];

		///所有的方向贴图
		Texture2D[] lmDirs = new Texture2D[cnt];

		for (int i = 0; i < cnt; i++)
		{
			lmColors[i] = LightmapSettings.lightmaps[i].lightmapColor;
			lmDirs[i] = LightmapSettings.lightmaps[i].lightmapDir;
			//写出两张贴图的名字
			writer.Write(lmColors[i] == null ? "" : lmColors[i].name);
			writer.Write(lmDirs[i] == null ? "" : lmDirs[i].name);
			if (lmColors[i] != null)
			{
				assetNames.Add(AssetDatabase.GetAssetPath(lmColors[i]));
			}

			if (lmDirs[i] != null)
			{
				assetNames.Add(AssetDatabase.GetAssetPath(lmDirs[i]));
			}
		}
	}

	/// <summary>
	/// 写出场景当中预制内容
	/// </summary>
	/// <param name="writer"></param>
	private static void RecordLightmapOffsetScale(BinaryWriter writer)
	{
		GameObject go = GameObject.Find("AllPrefab");
		if (go == null)
		{
			Debug.Log("the scene is not target scene.");
			return;
		}

		MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer>();
		List<DyncRenderInfo> dyncRenderInfos = new List<DyncRenderInfo>();
		for (int index = 0; index < meshRenderers.Length; index++)
		{
			if (meshRenderers[index].lightmapIndex != -1)
			{
				DyncRenderInfo info = new DyncRenderInfo();
				info.m_LightIndex = meshRenderers[index].lightmapIndex;
				info.m_LightOffsetScale = meshRenderers[index].lightmapScaleOffset;
				UnityEngine.Object parentObject = PrefabUtility.GetCorrespondingObjectFromSource(meshRenderers[index].gameObject);
				info.m_Hash = AssetDatabase.GetAssetPath(parentObject).GetHashCode();
				info.m_Pos = meshRenderers[index].transform.position;
				dyncRenderInfos.Add(info);
			}
		}
	}
}
