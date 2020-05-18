/*
 * Creator:ffm
 * Desc:快速查看场景内容
 * Time:2019/12/4 17:01:38
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEditor;
using System.IO;
using System.Text;
using UnityEditor.SceneManagement;

public class ScenesMenuBuild
{
	private static readonly string m_ScenesMenuPath = "Engine/Editor/ScenesMenu.cs";

	[MenuItem("Tools/Update Scene List")]
	public static void UpdateList()
	{
		string path = Path.Combine(Application.dataPath, m_ScenesMenuPath);
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("using UnityEditor;");
		sb.AppendLine("using UnityEditor.SceneManagement;");
		sb.AppendLine("public static class ScenesMenu");
		sb.AppendLine("{");

		foreach (string sceneGuid in AssetDatabase.FindAssets("t:Scene", new string[] { "Assets" }))
		{
			string sceneFilename = AssetDatabase.GUIDToAssetPath(sceneGuid);
			string sceneName = Path.GetFileNameWithoutExtension(sceneFilename);
			string methodName = sceneFilename.Replace('/', '_').Replace('\\', '_').Replace('.', '_').Replace('-', '_');
			sb.AppendLine(string.Format("[MenuItem(\"Tools/Update Scene List/{0}\", priority = 10)]", sceneName));
			sb.AppendLine(string.Format("public static void {0}() {{ ScenesMenuBuild.OpenScene(\"{1}\"); }}", methodName, sceneFilename));
		}

		sb.AppendLine("}");
		Directory.CreateDirectory(Path.GetDirectoryName(m_ScenesMenuPath));
		File.WriteAllText(path, sb.ToString());
		AssetDatabase.Refresh();
	}

	public static void OpenScene(string filename)
	{
		if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
		{
			EditorSceneManager.OpenScene(filename);
		}
	}
}
