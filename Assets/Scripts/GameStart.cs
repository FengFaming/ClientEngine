/*
 * Creator:ffm
 * Desc:项目启动类
 * Time:2019/11/11 15:25:55
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using System.IO;

public class GameStart : ObjectBase
{
	public string m_IDCard = "44022219910116033X";
	public List<string> m_UINames;
	public UILayer m_Layer;

	private void Start()
	{
#if UNITY_EDITOR || !NEWWINDOW
		SystemDebugManager d = this.gameObject.AddComponent<SystemDebugManager>();
		d.StartWindow();
#endif

#if !UNITY_EDITOR && NEWWINDOW
		Debug.ShowInConsole = false;
		ExeWindowManager.Instance.SetWindows(Vector2Int.zero, new Vector2Int(1024, 768),true);
#endif

#if !UNITY_EDITOR || TEST_AB
		//CopyFile(Application.streamingAssetsPath, Application.persistentDataPath);
		ResObjectManager.Instance.IsUseAB = true;
#endif

#if !DEBUG_LOG
		Debug.ShowDebug = false;
#endif

		GameNetManager.Instance.AddAgreement(EngineMessageHead.NET_CLIENT_TIME_RESPONSE, "GetServerTimeResponse");
		GameNetManager.Instance.CreateClient(1, "127.0.0.1", 6001, 1024 * 1024 * 10, SuccessConnect);

		Debug.Log(Application.persistentDataPath);
	}

	/// <summary>
	/// 链接成功
	/// </summary>
	/// <param name="success"></param>
	private void SuccessConnect(bool success)
	{
		if (success)
		{
			StartCoroutine("StartConnectEnd");
		}
	}

	/// <summary>
	/// 开始版本对比
	/// </summary>
	/// <returns></returns>
	private IEnumerator StartConnectEnd()
	{
		yield return null;
		VersionManager.Instance.StartVersion(VersionSuccess);
	}

	/// <summary>
	/// 版本对比成功
	/// </summary>
	private void VersionSuccess()
	{
		ResObjectManager.Instance.InitResManager("AB");
		MessageManger.Instance.AddMessageListener(EngineMessageHead.CHANGE_SCENE_MESSAGE,
						this.gameObject, OpenChangeScene);

		StartCoroutine("StartGame");
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
	/// 打开跳转界面
	/// </summary>
	/// <param name="arms"></param>
	private void OpenChangeScene(params object[] arms)
	{
		if ((bool)arms[0])
		{
			UIManager.Instance.OpenUI("UIPnlFirstPanle", UILayer.Blk, true);
		}
		else
		{
			UIManager.Instance.RecoveryUIModel("UIPnlFirstPanle", UILayer.Blk);
		}
	}

	/// <summary>
	/// 开始游戏
	/// </summary>
	/// <returns></returns>
	private IEnumerator StartGame()
	{
		GameNetManager.Instance.CloseClient(1);
		yield return null;
		UIManager.Instance.OpenUI("UIPnlFirstPanle", UILayer.Pnl);
		yield return new WaitForSeconds(0.5f);

		GameNetManager.Instance.CreateClient(1, "127.0.0.1", 6003, 1024 * 1024 * 10, DataServer);
	}

	private void DataServer(bool isSuccess)
	{
		if (isSuccess)
		{
			StartCoroutine("DataServerStart");
		}
	}

	private IEnumerator DataServerStart()
	{
		Debug.Log("逻辑服务器启动成功");
		yield return null;
		UIManager.Instance.OpenUI("UIPnlGameStart", UILayer.Pnl);
		StartGameRequest pack = new StartGameRequest();
		GameNetManager.Instance.SendMessage<StartGameRequest>(pack, 1);
	}

	private void OnDestroy()
	{
		GameNetManager.Instance.CloseClient(1);
		GameThreadManager.Instance.CloseAll();
	}
}
