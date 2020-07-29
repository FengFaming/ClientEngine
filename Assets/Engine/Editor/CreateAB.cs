/*
 * Creator:ffm
 * Desc:创建AB包
 * Time:2019/11/6 16:17:23
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;

public class CreateAB : EditorWindow
{
	/// <summary>
	/// 打包平台
	/// </summary>
	private BuildTarget m_BuildTarget;
	private bool m_IsAllBuildTarget;
	private Dictionary<string, AssetInfo> m_AssetInfoDict;

	private float curProgress = 0;
	private string curRootAsset = "";

	/// <summary>
	/// 路径保存
	/// </summary>
	private string m_SaveFilePath;

	/// <summary>
	/// 设置打包力度
	/// </summary>
	private int m_BuildPiece = 1;

	/// <summary>
	/// 是否打包完成
	/// </summary>
	private bool m_IsBuinded = false;

	/// <summary>
	/// 是否网络测试
	/// </summary>
	private bool m_IsSocketTest = false;

	[MenuItem("Tools/Build/CreateAB")]
	private static void Create()
	{
		EditorWindow.GetWindow(typeof(CreateAB));
	}

	private void OnGUI()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.LabelField("是否全平台打包");
		m_IsAllBuildTarget = EditorGUILayout.ToggleLeft("是", m_IsAllBuildTarget);
		EditorGUI.EndChangeCheck();

		if (!m_IsAllBuildTarget)
		{
			if (GUILayout.Button("Android", GUILayout.Height(20)))
			{
				m_BuildTarget = BuildTarget.Android;
			}

			if (GUILayout.Button("iOS", GUILayout.Height(20)))
			{
				m_BuildTarget = BuildTarget.iOS;
			}

			if (GUILayout.Button("Windows", GUILayout.Height(20)))
			{
				m_BuildTarget = BuildTarget.StandaloneWindows64;
			}
		}

		GUILayout.Label("选择的平台是：");
		string str = m_IsAllBuildTarget ? "全平台" : m_BuildTarget.ToString();
		GUILayout.Label(str);

		m_BuildPiece = int.Parse(EditorGUILayout.TextField("打包粒度:", m_BuildPiece.ToString()));
		CalSavePath();

		if (GUILayout.Button("清理依赖关系", GUILayout.Height(20)))
		{
			FileUtil.DeleteFileOrDirectory(m_SaveFilePath);
			m_IsBuinded = false;
			string[] abnames = AssetDatabase.GetAllAssetBundleNames();
			foreach (string a in abnames)
			{
				AssetDatabase.RemoveAssetBundleName(a, true);
			}
		}

		if (GUILayout.Button("分析依赖关系", GUILayout.Height(30)))
		{
			m_IsBuinded = false;
			SetABNames();
		}

		if (GUILayout.Button("开始打包", GUILayout.Height(50)))
		{
			if (m_IsAllBuildTarget)
			{
				BuildAB(BuildTarget.Android);
				BuildAB(BuildTarget.iOS);
				BuildAB(BuildTarget.StandaloneWindows64);
			}
			else
			{
				if (m_BuildTarget == 0)
				{
					m_BuildTarget =
#if UNITY_EDITOR_IOS
						BuildTarget.iOS;
#elif UNITY_EDITOR_ANDROID
						BuildTarget.Android;
#else
						BuildTarget.StandaloneWindows64;
#endif
				}

				BuildAB(m_BuildTarget);
				m_IsBuinded = true;
			}
		}

		if (m_IsBuinded)
		{
			m_IsSocketTest = EditorGUILayout.ToggleLeft("是否网络测试", m_IsSocketTest);
			if (GUILayout.Button("文件复制测试", GUILayout.Height(30)))
			{
				CopyFile();
			}
		}
	}

	/// <summary>
	/// 计算保存路径
	/// </summary>
	private void CalSavePath()
	{
		string ab = Application.dataPath;
		ab = ab.Substring(0, ab.Length - ".assets".Length);
		m_SaveFilePath = ab + "/AB/";
	}

	/// <summary>
	/// 复制文件
	/// </summary>
	private void CopyFile()
	{
		string path = Application.persistentDataPath;

		string ab = Application.dataPath;
		ab = ab.Substring(0, ab.Length - ".assets".Length);
		if (m_IsSocketTest)
		{
			string last = ab + "/OtherProject/ComFile/ComFile/bin/Debug/Last";
			if (Directory.Exists(last))
			{
				Directory.Move(last, last + "999");
			}

			string now = ab + "/OtherProject/ComFile/ComFile/bin/Debug/Now";
			if (Directory.Exists(now))
			{
				Directory.Move(now, last);
			}

			path = now;
		}

		if (!Directory.Exists(path))
		{
			DirectoryInfo f = new DirectoryInfo(path);
			f.Create();
		}

		///复制AB
		string[] files = Directory.GetFiles(m_SaveFilePath);
		string abPath = path + "/AB/";
		if (!Directory.Exists(abPath))
		{
			DirectoryInfo f = new DirectoryInfo(abPath);
			f.Create();
		}
		foreach (string f in files)
		{
			FileInfo i = new FileInfo(f);
			string save = abPath + i.Name;
			if (i.Name == "HashTab")
			{
				continue;
			}

			File.Copy(f, save, true);
		}

		///复制场景
		string[] scenes = Directory.GetFiles(ab + "/SceneAB/");
		foreach (string f in scenes)
		{
			FileInfo i = new FileInfo(f);
			string save = abPath + i.Name;
			if (i.Name == "HashTab")
			{
				continue;
			}

			File.Copy(f, save, true);
		}

		CopyFile(Application.streamingAssetsPath, path);

		EditorUtility.OpenWithDefaultApp(path);
	}

	/// <summary>
	/// 拷贝文件
	/// </summary>
	/// <param name="srcPath">源目录</param>
	/// <param name="desPath">目标目录</param>
	private void CopyFile(string srcPath, string desPath)
	{
		List<string> files = GetAllFiles(srcPath);
		for (int index = 0; index < files.Count; index++)
		{
			string filename = files[index].Substring(srcPath.Length);
			string save = desPath + filename;
			filename = save.Substring(0, save.LastIndexOf("\\"));
			if (!Directory.Exists(filename))
			{
				DirectoryInfo f = new DirectoryInfo(filename);
				f.Create();
			}

			File.Copy(files[index], save, true);
		}
	}

	/// <summary>
	/// 获取所有文件
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	private List<string> GetAllFiles(string path)
	{
		List<string> fs = new List<string>();
		fs.Clear();

		if (!Directory.Exists(path))
		{
			return fs;
		}

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

	/// <summary>
	/// 编辑打包
	/// </summary>
	/// <param name="target"></param>
	private void BuildAB(BuildTarget target)
	{
		if (!Directory.Exists(m_SaveFilePath))
		{
			DirectoryInfo f = new DirectoryInfo(m_SaveFilePath);
			f.Create();
		}

		BuildPipeline.BuildAssetBundles(m_SaveFilePath,
									BuildAssetBundleOptions.UncompressedAssetBundle |
									BuildAssetBundleOptions.DeterministicAssetBundle,
									target);

		AssetDatabase.Refresh();
		EditorUtility.OpenWithDefaultApp(m_SaveFilePath);
	}

	/// <summary>
	/// 设置AB名字
	/// </summary>
	private void SetABNames()
	{
		string path = GetSelectedAssetPath();
		if (path == null)
		{
			Debug.LogError("你是不是删除了一些东西.");
			return;
		}

		GetAllAssets(path);
	}

	/// <summary>
	/// 获取所有得as
	/// </summary>
	/// <param name="path"></param>
	private void GetAllAssets(string path)
	{
		if (m_AssetInfoDict == null)
		{
			m_AssetInfoDict = new Dictionary<string, AssetInfo>();
		}

		m_AssetInfoDict.Clear();

		DirectoryInfo dirInfo = new DirectoryInfo(path);
		FileInfo[] fs = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
		int index = 0;
		foreach (FileInfo f in fs)
		{
			curProgress = (float)index / (float)fs.Length;
			curRootAsset = "正在分析依赖：" + f.Name;
			EditorUtility.DisplayProgressBar(curRootAsset, curRootAsset, curProgress);
			index++;

			int i = f.FullName.IndexOf("Assets");
			if (i != -1)
			{
				string assetpath = f.FullName.Substring(i);
				UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(assetpath);
				string upath = AssetDatabase.GetAssetPath(asset);
				if (!m_AssetInfoDict.ContainsKey(upath) &&
					assetpath.StartsWith("Assets") &&
					!(asset is MonoScript) &&
					!(asset is LightingDataAsset) &&
					asset != null)
				{
					AssetInfo info = new AssetInfo(upath, true);
					CreateDeps(info);
				}

				EditorUtility.UnloadUnusedAssetsImmediate();
			}

			EditorUtility.UnloadUnusedAssetsImmediate();
		}

		EditorUtility.ClearProgressBar();

		int setIndex = 0;
		foreach (KeyValuePair<string, AssetInfo> kv in m_AssetInfoDict)
		{
			EditorUtility.DisplayProgressBar("正在设置AB名字", kv.Key, (float)setIndex / (float)m_AssetInfoDict.Count);
			setIndex++;
			AssetInfo a = kv.Value;
			a.SetAssetBundleName(m_BuildPiece);
		}

		EditorUtility.ClearProgressBar();
		EditorUtility.UnloadUnusedAssetsImmediate();
		AssetDatabase.SaveAssets();
	}

	/// <summary>
	/// 递归分析每一个依赖关系
	/// </summary>
	/// <param name="self"></param>
	/// <param name="parent"></param>
	private void CreateDeps(AssetInfo self, AssetInfo parent = null)
	{
		if (self.HasParent(parent))
		{
			return;
		}

		if (!m_AssetInfoDict.ContainsKey(self.AssetPath))
		{
			m_AssetInfoDict.Add(self.AssetPath, self);
		}

		self.AddParent(parent);

		UnityEngine.Object[] deps = EditorUtility.CollectDependencies(new UnityEngine.Object[] { self.GetAsset() });
		for (int i = 0; i < deps.Length; i++)
		{
			UnityEngine.Object o = deps[i];
			if (o is MonoScript || o is LightingDataAsset)
			{
				continue;
			}

			string path = AssetDatabase.GetAssetPath(o);
			if (path == self.AssetPath)
			{
				continue;
			}

			if (path.StartsWith("Assets") == false)
			{
				continue;
			}

			AssetInfo info = null;
			if (m_AssetInfoDict.ContainsKey(path))
			{
				info = m_AssetInfoDict[path];
			}
			else
			{
				info = new AssetInfo(path, false);
				m_AssetInfoDict.Add(path, info);
			}

			EditorUtility.DisplayProgressBar(curRootAsset, path, curProgress);
			CreateDeps(info, self);
		}

		EditorUtility.UnloadUnusedAssetsImmediate();
	}

	/// <summary>
	/// 获取路径
	/// </summary>
	/// <returns></returns>
	private string GetSelectedAssetPath()
	{
		return Application.dataPath + "/UseAB";
	}
}
