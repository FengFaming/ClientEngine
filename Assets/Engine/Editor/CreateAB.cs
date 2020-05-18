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
	/// 当前版本号
	/// </summary>
	private string m_BuildVersion = "0.0.0.1";

	/// <summary>
	/// 路径保存
	/// </summary>
	private string m_SaveFilePath;

	/// <summary>
	/// 上一个版本号
	/// </summary>
	private string m_LastVersion;
	private string m_LastSavePath;

	/// <summary>
	/// 设置打包力度
	/// </summary>
	private int m_BuildPiece = 1;

	private bool m_IsBuinded = false;

	[MenuItem("Tools/CreateAB")]
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
		m_BuildVersion = EditorGUILayout.TextField("版本号", m_BuildVersion);
		CalSavePath();
		CalLastVersionPath();

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
			if (GUILayout.Button("去除版本号约束", GUILayout.Height(30)))
			{
				ExitFileVersion();
			}

			if (GUILayout.Button("分析hash数据", GUILayout.Height(30)))
			{
				CalHashData();
			}

			if (!string.IsNullOrEmpty(m_LastVersion) &&
				Directory.Exists(m_SaveFilePath) &&
				Directory.Exists(m_LastSavePath))
			{
				if (GUILayout.Button("与上一版本分析差异文件", GUILayout.Height(30)))
				{
					CheckDiff();
				}
			}

			if (GUILayout.Button("Editor测试", GUILayout.Height(30)))
			{
				CopyFile();
			}
		}
	}

	/// <summary>
	/// 分析差异
	/// </summary>
	private void CheckDiff()
	{
		string hx1 = m_SaveFilePath + "/HashTab";
		string hx2 = m_LastSavePath + "/HashTab";
		Dictionary<string, string> hxs2 = new Dictionary<string, string>();
		List<string> diffs = new List<string>();
		FileStream fs = new FileStream(hx2, FileMode.Open);
		StreamReader sr = new StreamReader(fs);
		string line;
		while ((line = sr.ReadLine()) != null)
		{
			string[] hs = line.Split(' ');
			hxs2.Add(hs[0], hs[1]);
		}

		sr.Close();
		fs.Close();

		fs = new FileStream(hx1, FileMode.Open);
		sr = new StreamReader(fs);
		string line2;
		while ((line2 = sr.ReadLine()) != null)
		{
			string[] hs = line2.Split(' ');
			if (hxs2.ContainsKey(hs[0]))
			{
				if (hxs2[hs[0]] != hs[1])
				{
					diffs.Add(hs[0]);
				}
			}
			else
			{
				diffs.Add(hs[0]);
			}
		}

		sr.Close();
		fs.Close();

		string fileName = m_SaveFilePath.Remove(m_SaveFilePath.Length - 1) + "-" + m_LastVersion;
		FileUtil.DeleteFileOrDirectory(fileName);
		if (!Directory.Exists(fileName))
		{
			DirectoryInfo f = new DirectoryInfo(fileName);
			f.Create();
		}

		string[] files = Directory.GetFiles(m_SaveFilePath);
		foreach (string f in files)
		{
			FileInfo i = new FileInfo(f);
			string save = fileName + "/" + i.Name;
			if (diffs.Contains(i.Name))
			{
				File.Copy(f, save, true);
			}
		}

		EditorUtility.OpenWithDefaultApp(fileName);
	}

	/// <summary>
	/// 部分文件去除版本号
	/// </summary>
	private void ExitFileVersion()
	{
		string ab = m_SaveFilePath + "/AB" + m_BuildVersion;
		string abm = ab + ".manifest";
		string ab1 = m_SaveFilePath + "/AB";
		string abm1 = ab1 + ".manifest";
		File.Move(ab, ab1);
		File.Move(abm, abm1);
	}

	/// <summary>
	/// 分析hash数据
	/// </summary>
	private void CalHashData()
	{
		string sf = m_SaveFilePath + "/HashTab";
		if (File.Exists(sf))
		{
			File.Delete(sf);
		}

		DirectoryInfo fdir = new DirectoryInfo(m_SaveFilePath);
		FileInfo[] file = fdir.GetFiles();

		FileStream sffs = new FileStream(sf, FileMode.CreateNew);
		StreamWriter sw = new StreamWriter(sffs);

		HashAlgorithm hash = HashAlgorithm.Create();
		for (int index = 0; index < file.Length; index++)
		{
			string path = m_SaveFilePath + "/" + file[index].Name;
			FileStream fs = new FileStream(path, FileMode.Open);
			byte[] hx = hash.ComputeHash(fs);
			string hs = BitConverter.ToString(hx);
			fs.Close();
			sw.WriteLine(file[index].Name + " " + hs);
		}

		sw.Close();
		sffs.Close();

		EditorUtility.OpenWithDefaultApp(m_SaveFilePath);
	}

	/// <summary>
	/// 计算上一个版本得路径
	/// </summary>
	private void CalLastVersionPath()
	{
		string[] vs = m_BuildVersion.Split('.');
		int i1 = int.Parse(vs[0]);
		int i2 = int.Parse(vs[1]);
		int i3 = int.Parse(vs[2]);
		int i4 = int.Parse(vs[3]);

		if (i4 > 0)
		{
			i4 = i4 - 1;
		}
		else
		{
			i4 = 99;
			if (i3 > 0)
			{
				i3 = i3 - 1;
			}
			else
			{
				i3 = 99;
				if (i2 > 0)
				{
					i2 = i2 - 1;
				}
				else
				{
					i2 = 99;
					if (i1 > 0)
					{
						i1 = i1 - 1;
					}
					else
					{
						i1 = i2 = i3 = i4 = 0;
					}
				}
			}
		}

		if ((i1 + i2 + i3 + i4) == 0)
		{
			m_LastSavePath = string.Empty;
			m_LastVersion = string.Empty;
		}
		else
		{
			m_LastVersion = string.Format("{0}.{1}.{2}.{3}", i1, i2, i3, i4);
			string ab = Application.dataPath;
			ab = ab.Substring(0, ab.Length - ".assets".Length);
			m_LastSavePath = ab + "/AB" + m_LastVersion + "/";
		}
	}

	/// <summary>
	/// 计算保存路径
	/// </summary>
	private void CalSavePath()
	{
		if (string.IsNullOrEmpty(m_BuildVersion))
		{
			m_BuildVersion = "0.0.0.1";
		}

		string[] vs = m_BuildVersion.Split('.');
		if (vs.Length != 4)
		{
			m_BuildVersion = "0.0.0.1";
			vs = m_BuildVersion.Split('.');
		}

		string ab = Application.dataPath;
		ab = ab.Substring(0, ab.Length - ".assets".Length);
		m_SaveFilePath = ab + "/AB" + m_BuildVersion + "/";
	}

	/// <summary>
	/// 复制文件
	/// </summary>
	private void CopyFile()
	{
		string path = Application.persistentDataPath + "/AB/";
		if (!Directory.Exists(path))
		{
			DirectoryInfo f = new DirectoryInfo(path);
			f.Create();
		}

		string[] files = Directory.GetFiles(m_SaveFilePath);
		foreach (string f in files)
		{
			FileInfo i = new FileInfo(f);
			string save = path + i.Name;
			if (i.Name == "AB" + m_BuildVersion)
			{
				save = path + "AB";
			}

			if (i.Name == "AB" + m_BuildVersion + ".manifest")
			{
				save = path + "AB.manifest";
			}

			if (i.Name == "HashTab")
			{
				continue;
			}

			File.Copy(f, save, true);
		}

		EditorUtility.OpenWithDefaultApp(path);
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

		//ExitFileVersion();
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
