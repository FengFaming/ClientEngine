using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateSprite : EditorWindow
{
	private static string fileName;
	private static string creator;
	private static string describe;
	private static string showText;

	[MenuItem("Tools/CreateSprite")]
	private static void Create()
	{
		fileName = string.Empty;
		showText = string.Empty;
		describe = string.Empty;

		creator = "ffm";

		EditorWindow.GetWindow(typeof(CreateSprite));
	}

	private void OnGUI()
	{
		GUILayout.Label("Create C# file");

		fileName = EditorGUILayout.TextField("C#文件名字:", fileName);
		creator = EditorGUILayout.TextField("文件创建者：", creator);
		describe = EditorGUILayout.TextField("文件描述:", describe);
		if (GUILayout.Button("创建文件", GUILayout.Width(200)))
		{
			Object oj = Selection.activeObject;
			if (oj == null)
			{
				showText = "请先选择文件夹";
				return;
			}

			string path = AssetDatabase.GetAssetPath(oj);
			string datapath = Application.dataPath;
			datapath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
			path = string.Format("{0}/{1}", datapath, path);
			if (System.IO.File.Exists(path))
			{
				path = path.Substring(0, path.LastIndexOf("/"));
			}

			showText = string.Format("{0}/{1}.cs", path, fileName);
			if (File.Exists(showText))
			{
				showText = "文件已存在!";
				return;
			}

			FileStream fs = new FileStream(showText, FileMode.Create, FileAccess.Write);
			StreamWriter sw = new StreamWriter(fs);

			sw.WriteLine("/*需要屏蔽的警告*/");
			sw.WriteLine("#pragma warning disable");
			sw.WriteLine("/*");
			sw.WriteLine(string.Format(" * Creator:{0}", creator));
			sw.WriteLine(string.Format(" * Desc:{0}", describe));
			sw.WriteLine(string.Format(" * Time:{0}", System.DateTime.Now.ToString()));
			sw.WriteLine("* */");
			sw.WriteLine();
			sw.WriteLine("using System;");
			sw.WriteLine("using System.Collections;");
			sw.WriteLine("using System.Collections.Generic;");
			sw.WriteLine("using UnityEngine;");
			sw.WriteLine("using Game.Engine;");
			sw.WriteLine();
			sw.WriteLine(string.Format("public class {0}:ObjectBase", fileName));
			sw.WriteLine("{");
			sw.WriteLine("}");

			sw.Close();
			fs.Close();

			this.Repaint();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			this.Close();
		}

		EditorGUILayout.TextField(showText);
	}
}
