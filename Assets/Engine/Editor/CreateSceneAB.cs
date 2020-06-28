/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:创建场景AB
 * Time:2020/6/28 11:42:55
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateSceneAB : EditorWindow
{
	/// <summary>
	/// 打包平台
	/// </summary>
	private BuildTarget m_BuildTarget;
	private bool m_IsAllBuildTarget;

	/// <summary>
	/// 打包场景文件路径
	/// </summary>
	private string m_BuildScenePath = "/Scenes/OtherScene/";
	private List<string> m_Scenes;

	private string m_SavePath;

	private List<bool> m_NeedBuilds = new List<bool>();

	[MenuItem("Tools/CreateSceneAB")]
	private static void Create()
	{
		EditorWindow.GetWindow(typeof(CreateSceneAB));
	}

	private void OnGUI()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.LabelField("是否全平台打包");
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

		m_BuildScenePath = EditorGUILayout.TextField("路径:", m_BuildScenePath);
		if (m_Scenes == null)
		{
			m_Scenes = new List<string>();
		}

		m_Scenes.Clear();
		m_Scenes.AddRange(GetAllFiles(Application.dataPath + m_BuildScenePath));
		for (int index = 0; index < m_Scenes.Count; index++)
		{
			m_Scenes[index] = m_Scenes[index].Substring(m_Scenes[index].LastIndexOf("Assets"));
			if (m_NeedBuilds.Count <= index)
			{
				m_NeedBuilds.Add(false);
			}
		}

		for (int index = 0; index < m_Scenes.Count; index++)
		{
			if (GUILayout.Button(m_Scenes[index]))
			{
				SelectScene(m_Scenes[index]);
			}
			//m_NeedBuilds[index] = EditorGUILayout.Toggle(m_Scenes[index], m_NeedBuilds[index]);
			//GUILayout.Label(m_Scenes[index]);
		}

		GUILayout.Label("需要打包的场景:");
		float curAll = 0;
		for (int index = 0; index < m_Scenes.Count; index++)
		{
			if (m_NeedBuilds[index])
			{
				curAll++;
				GUILayout.Label(m_Scenes[index]);
			}
		}

		if (curAll > 0)
		{
			if (GUILayout.Button("打包场景", GUILayout.Height(20)))
			{
				string ab = Application.dataPath;
				ab = ab.Substring(0, ab.Length - ".assets".Length);
				m_SavePath = ab + "/SceneAB/";
				if (!Directory.Exists(m_SavePath))
				{
					DirectoryInfo f = new DirectoryInfo(m_SavePath);
					f.Create();
				}

				float curProgress = 0;
				for (int index = 0; index < m_Scenes.Count; index++)
				{
					if (m_NeedBuilds[index])
					{
						string name = m_Scenes[index];
						string curRootAsset = "正在打包【" + name + "】场景";
						curProgress = (float)(index + 1) / (float)curAll;
						EditorUtility.DisplayProgressBar(curRootAsset, curRootAsset, curProgress);
						BuildPlayerOptions options = new BuildPlayerOptions();
						options.scenes = new string[] { name };
						string localname = name.Substring(0, name.LastIndexOf("."));
						localname = localname.Substring(localname.LastIndexOf("/") + 1);
						options.locationPathName = m_SavePath + localname + ".unity3d";
						options.target = m_BuildTarget;
						options.options = BuildOptions.BuildAdditionalStreamedScenes;
						BuildPipeline.BuildPlayer(options);
					}
				}

				EditorUtility.ClearProgressBar();
				CopyFile();
			}
		}
	}

	/// <summary>
	/// 选择的场景
	/// </summary>
	/// <param name="name"></param>
	private void SelectScene(string name)
	{
		Debug.Log(name);
		for (int index = 0; index < m_Scenes.Count; index++)
		{
			if (m_Scenes[index] == name)
			{
				m_NeedBuilds[index] = !m_NeedBuilds[index];
				break;
			}
		}
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

		string[] files = Directory.GetFiles(m_SavePath);
		foreach (string f in files)
		{
			FileInfo i = new FileInfo(f);
			string save = path + i.Name;
			File.Copy(f, save, true);
		}

		EditorUtility.OpenWithDefaultApp(path);
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
		if (!Directory.Exists(path))
		{
			Debug.Log("the file has not.");
			return fs;
		}

		var files = Directory.GetFiles(path, "*.unity", SearchOption.AllDirectories);
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
