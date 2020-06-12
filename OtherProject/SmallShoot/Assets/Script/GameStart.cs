/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:游戏开始
 * Time:2020/6/11 10:22:01
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using XLua;
using System.IO;

public class GameStart : SingletonMonoClass<GameStart>
{
	private LuaTable m_ControlTable;
	private string m_ZiDanPool;
	private string m_DiRenPool;

	private List<PoolMoveControl> m_ZiDans;
	private List<PoolMoveControl> m_DiRens;

	private bool m_IsShoot;

	private UIPnlGameMain m_UIControl;
	public UIPnlGameMain UIControl { set { m_UIControl = value; } }

	private float m_MoveSpeed;
	private float m_TimeSpeed;
	private float m_DeltTime;

	private float m_DiRenTime;
	private float m_DiRenTempTime;
	private float m_DiRenLimit;
	private float m_DiRenMaxLimit;

	private int m_FenLimit;
	private int m_FenMaxLimit;
	private float m_DiRenSpeed;
	private LuaFunction m_DiRenNumber;
	private bool m_StartDiRen;

	private int m_AllFenZhi;
	private LuaFunction m_GuoGuan;

	protected override void Awake()
	{
		base.Awake();
		m_ControlTable = null;
		m_ZiDanPool = "ZIDANPOOL";
		m_DiRenPool = "DIRENPOOL";

		m_ZiDans = new List<PoolMoveControl>();
		m_ZiDans.Clear();

		m_DiRens = new List<PoolMoveControl>();
		m_DiRens.Clear();

		m_IsShoot = false;
		m_DeltTime = 0f;
		m_DiRenTempTime = 0f;
		m_StartDiRen = false;

		m_AllFenZhi = 0;
	}

	private void Start()
	{
		string str = string.Empty;
		string[] vs = Environment.GetCommandLineArgs();
		if (vs != null && vs.Length > 1)
		{
			string s = vs[1];
			string[] ss = s.Split(',');
			int with = int.Parse(ss[0]);
			int height = int.Parse(ss[1]);
			ExeWindowManager.Instance.SetWindows(Vector2Int.zero, new Vector2Int(with, height), true);

			str = with + " " + height;
		}

#if !UNITY_EDITOR || TEST_AB
		CopyFile(Application.streamingAssetsPath, Application.persistentDataPath);
		ResObjectManager.Instance.IsUseAB = true;
#endif
		Debug.Log(Application.persistentDataPath);
		ResObjectManager.Instance.InitResManager("AB");

		GameShowFPS fps = this.gameObject.AddComponent<GameShowFPS>();
		fps.IsShowFPS = true;
		fps.m_ShowOther = str;

		StartCoroutine("StartGame");
	}

	private IEnumerator StartGame()
	{
		yield return null;
		yield return new WaitForSeconds(1f);
		UIManager.Instance.OpenUI("UIPnlGameStart", UILayer.Pnl);
	}

	/// <summary>
	/// 开始初始化游戏
	/// </summary>
	/// <param name="self"></param>
	/// <param name="table"></param>
	public void StartGameWithData(string self, XLua.LuaTable table)
	{
		m_ControlTable = table;
		LoadPool p = new LoadPool();
		p.m_IsUpDown = true;
		ResObjectManager.Instance.LoadObject("zidan", ResObjectType.GameObject, p);

		LoadPool p1 = new LoadPool();
		p1.m_IsUpDown = false;
		ResObjectManager.Instance.LoadObject("diren", ResObjectType.GameObject, p1);

		UIManager.Instance.OpenUI("UIPnlGameMain", UILayer.Pnl, self);
		MessageManger.Instance.AddMessageListener(EngineMessageHead.LISTEN_KEY_EVENT_FOR_INPUT_MANAGER + "-" + (int)KeyCode.Space, Shoot);

		LuaFunction luaFunction = m_ControlTable.Get<LuaFunction>("st");
		System.Object[] vs = luaFunction.Call();
		m_TimeSpeed = float.Parse((string)vs[1]);
		m_MoveSpeed = float.Parse((string)vs[0]);

		DiRenData();
	}

	/// <summary>
	/// 升级
	/// </summary>
	private void SengJi()
	{
		DiRenData();
		m_StartDiRen = true;
	}

	/// <summary>
	/// 敌人数据
	/// </summary>
	private void DiRenData()
	{
		if (m_DiRenNumber == null)
		{
			m_DiRenNumber = m_ControlTable.Get<LuaFunction>("diren");
		}

		System.Object[] vsd = m_DiRenNumber.Call();
		m_DiRenTime = float.Parse((string)vsd[0]);
		m_DiRenLimit = float.Parse((string)vsd[1]);
		m_DiRenMaxLimit = float.Parse((string)vsd[2]);
		m_DiRenSpeed = float.Parse((string)vsd[3]);
		m_FenLimit = int.Parse((string)vsd[4]);
		m_FenMaxLimit = int.Parse((string)vsd[5]);
	}

	private void Shoot(params object[] vs)
	{
		GameMouseInputManager.KeyInfo info = (GameMouseInputManager.KeyInfo)vs[0];
		if (info.m_KeyState == GameMouseInputManager.KeyState.KeyUp)
		{
			m_IsShoot = !m_IsShoot;
		}
	}

	/// <summary>
	/// 初始化对象池
	/// </summary>
	/// <param name="t"></param>
	/// <param name="upDown"></param>
	public void InitPool(object t, bool upDown)
	{
		string pool = upDown ? m_ZiDanPool : m_DiRenPool;
		IObjectPool p = ObjectPoolManager.Instance.InitPool(pool, "GamePool");
		GameObject d = t as GameObject;
		ShootGameObjectControl shootGameObjectControl = new ShootGameObjectControl();
		shootGameObjectControl.m_Target = d;
		ObjectPoolManager.Instance.AddObject(pool, pool, shootGameObjectControl);
		ShootGameObjectControl s = ObjectPoolManager.Instance.GetCloneObject(pool, pool) as ShootGameObjectControl;
		if (s != null)
		{
			ObjectPoolManager.Instance.RecoveryObject(pool, pool, s);
		}

		if (!upDown)
		{
			m_StartDiRen = true;
		}
	}

	/// <summary>
	/// 回收
	/// </summary>
	/// <param name="go"></param>
	/// <param name="upDown"></param>
	public void Recv(GameObject go, bool upDown)
	{
		PoolMoveControl control = go.GetComponent<PoolMoveControl>();
		if (upDown)
		{
			m_ZiDans.Remove(control);
		}
		else
		{
			m_DiRens.Remove(control);
		}

		GameObject.Destroy(control);

		ShootGameObjectControl c = new ShootGameObjectControl();
		c.m_Target = go;
		string pool = upDown ? m_ZiDanPool : m_DiRenPool;
		ObjectPoolManager.Instance.RecoveryObject(pool, pool, c);
	}

	/// <summary>
	/// 预加载
	/// </summary>
	public class LoadPool : IResObjectCallBack
	{
		public bool m_IsUpDown;

		public override void HandleLoadCallBack(object t)
		{
			GameStart.Instance.InitPool(t, m_IsUpDown);
		}

		public override int LoadCallbackPriority()
		{
			return 0;
		}
	}

	private void Update()
	{
		if (m_StartDiRen)
		{
			if (m_DiRenTempTime <= 0 && m_UIControl != null)
			{
				ShootGameObjectControl control = ObjectPoolManager.Instance.GetCloneObject(m_DiRenPool, m_DiRenPool) as ShootGameObjectControl;
				GameObject go = control.m_Target;
				control = null;

				float x = UnityEngine.Random.RandomRange(-200f, 200f);
				float y = UnityEngine.Random.RandomRange(m_DiRenLimit, m_DiRenMaxLimit);
				int f = UnityEngine.Random.RandomRange(m_FenLimit, m_FenMaxLimit);
				m_UIControl.SetDiRen(go, new Vector3(x, y, 0));

				PoolMoveControl c = go.AddComponent<PoolMoveControl>();
				c.StartMove(false, m_DiRenSpeed, f, 0);
				m_DiRenTempTime = m_DiRenTime;
			}
			else
			{
				m_DiRenTempTime -= Time.deltaTime;
			}
		}

		if (m_IsShoot)
		{
			if (m_DeltTime <= 0 && m_UIControl != null)
			{
				List<GameObject> games = new List<GameObject>();
				List<PoolMoveControl> pms = new List<PoolMoveControl>();
				games.Clear();
				pms.Clear();
				for (int index = 0; index < m_UIControl.GetZiDanCout(); index++)
				{
					ShootGameObjectControl control = ObjectPoolManager.Instance.GetCloneObject(m_ZiDanPool, m_ZiDanPool) as ShootGameObjectControl;
					GameObject go = control.m_Target;
					control = null;

					PoolMoveControl c = go.AddComponent<PoolMoveControl>();
					games.Add(go);
					pms.Add(c);
				}

				m_UIControl.SetZiDan(games);
				for (int index = 0; index < pms.Count; index++)
				{
					pms[index].StartMove(true, m_MoveSpeed, 0, 1);
				}

				m_DeltTime = m_TimeSpeed;
			}
			else
			{
				m_DeltTime -= Time.deltaTime;
			}
		}
	}

	public void CalGuoGuan(int f)
	{
		m_AllFenZhi += f;
		m_UIControl.SetAllFZ(m_AllFenZhi);
		if (m_GuoGuan == null)
		{
			m_GuoGuan = m_ControlTable.Get<LuaFunction>("guoguan");
		}

		System.Object[] vs = m_GuoGuan.Call(m_AllFenZhi);
		bool g = bool.Parse((string)vs[0]);
		if (g)
		{
			m_AllFenZhi = 0;
			SengJi();
		}
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
}
