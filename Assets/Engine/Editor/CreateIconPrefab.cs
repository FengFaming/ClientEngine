/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:创建图标预制
 * Time:2020/5/18 9:53:19
* */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class CreateIconPrefab : MonoBehaviour
{
	[MenuItem("Tools/CreateIconPrefab")]
	private static void CreatePrefab()
	{
		Object oj = Selection.activeObject;
		if (oj == null)
		{
			Debug.LogError("please select file.");
			return;
		}

		string path = GetDirection(oj);
		List<string> files = GetAllFiles(path);
		string savePath = "Assets/UseAB/Icon/";
		if (!Directory.Exists(savePath))
		{
			DirectoryInfo di = new DirectoryInfo(savePath);
			di.Create();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		for (int index = 0; index < files.Count; index++)
		{
			Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(files[index]);
			CreateGameObject(sprite);
		}

		AssetDatabase.Refresh();
	}

	private static void CreateGameObject(Sprite s)
	{
		GameObject go = new GameObject();
		go.name = s.name;
		go.transform.position = Vector3.zero;
		go.transform.rotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;

		Image i = go.AddComponent<Image>();
		i.sprite = s;

		string save = "Assets/UseAB/Icon/" + s.name + ".prefab";
		Object oj = PrefabUtility.CreateEmptyPrefab(save);
		oj = PrefabUtility.ReplacePrefab(go, oj);
		DestroyImmediate(go);
	}

	/// <summary>
	/// 获取选中物体的文件夹路径
	/// </summary>
	/// <param name="select"></param>
	/// <returns></returns>
	private static string GetDirection(UnityEngine.Object select)
	{
		string path = AssetDatabase.GetAssetPath(select);
		if (File.Exists(path))
		{
			path = path.Substring(0, path.LastIndexOf("/"));
		}
		return path;
	}

	/// <summary>
	/// 获取文件夹下面的所有内容
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	private static List<string> GetAllFiles(string path)
	{
		List<string> fs = new List<string>();
		fs.Clear();
		var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
		foreach (var file in files)
		{
			if (file.LastIndexOf(".meta") < 0)
			{
				fs.Add(file);
			}
		}

		return fs;
	}
}
