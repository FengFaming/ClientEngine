/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:游戏主界面
 * Time:2020/6/11 10:23:08
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;

public class UIPnlGameMain : UIModelLuaControl
{
	/// <summary>
	/// 敌人父节点
	/// </summary>
	private Transform m_DiRen;

	/// <summary>
	/// 子弹父节点
	/// </summary>
	private Transform m_ZiDan;

	/// <summary>
	/// 显示总分
	/// </summary>
	private Text m_ShowFenZhi;
	public void SetAllFZ(int f)
	{
		m_ShowFenZhi.text = f.ToString();
	}

	/// <summary>
	/// 自己
	/// </summary>
	private GameObject m_Self;
	private RectTransform m_SelfRect;

	/// <summary>
	/// 背景内容
	/// </summary>
	private RectTransform[] m_Bgs;
	private bool m_IsRun;

	/// <summary>
	/// 下限
	/// </summary>
	private float m_Limit;

	/// <summary>
	/// 上限
	/// </summary>
	private float m_MaxLimit;

	/// <summary>
	/// 变化长度
	/// </summary>
	private float m_TempY;

	private string m_SelfName;
	public int GetZiDanCout()
	{
		if (m_SelfName == "s2")
		{
			return 2;
		}

		return 1;
	}

	public UIPnlGameMain() : base()
	{
		m_ModelObjectPath = "UIPnlGameMain";
		m_IsOnlyOne = true;
		m_LuaName = "UIPnlGameMain";
		m_IsRun = false;
	}

	public override void InitUIData(UILayer layer, params object[] arms)
	{
		base.InitUIData(layer, arms);
		MessageManger.Instance.AddMessageListener("SET_DIREN_PARENT", SetDiRenParent);

		string self = (string)arms[0];
		m_SelfName = self;
	}

	private void LoadEnd(object t)
	{
		SetSelf(t as Sprite);
	}

	/// <summary>
	/// 设置敌人
	/// </summary>
	/// <param name="arms"></param>
	private void SetDiRenParent(params object[] vs)
	{
		List<System.Object> arms = vs[0] as List<System.Object>;

		GameObject go = arms[0] as GameObject;
		m_DiRen = go.transform;

		GameObject g = arms[1] as GameObject;
		m_ZiDan = g.transform;

		m_Self = arms[2] as GameObject;
		int cout = int.Parse(arms[3] as string);
		m_Bgs = new RectTransform[cout];
		int index = 4;
		for (int i = 0; i < cout; i++)
		{
			m_Bgs[i] = (arms[index++] as GameObject).GetComponent<RectTransform>();
		}

		m_Limit = float.Parse(arms[index++] as string);
		m_MaxLimit = float.Parse(arms[index++] as string);
		m_TempY = float.Parse(arms[index++] as string);
		GameObject fen = arms[index] as GameObject;
		m_ShowFenZhi = fen.GetComponent<Text>();
		m_ShowFenZhi.text = string.Empty;
		m_IsRun = true;

		ResObjectCallBackBase cb = new ResObjectCallBackBase();
		cb.m_LoadType = ResObjectType.Icon;
		cb.m_FinshFunction = LoadEnd;
		ResObjectManager.Instance.LoadObject(m_SelfName, ResObjectType.Icon, cb);

		MessageManger.Instance.AddMessageListener(EngineMessageHead.LISTEN_KEY_EVENT_FOR_INPUT_MANAGER + "-" + (int)KeyCode.A, AddKey);
		MessageManger.Instance.AddMessageListener(EngineMessageHead.LISTEN_KEY_EVENT_FOR_INPUT_MANAGER + "-" + (int)KeyCode.D, AddKey);
	}

	private void AddKey(params object[] arms)
	{
		GameMouseInputManager.KeyInfo info = (GameMouseInputManager.KeyInfo)arms[0];
		if (info.m_KeyState == GameMouseInputManager.KeyState.KeyStay)
		{
			int fx = 1;

			if (info.m_KeyCode == KeyCode.A)
			{
				fx = -1;
			}

			float delt = fx * Time.deltaTime * 100;
			if (m_SelfRect == null)
			{
				m_SelfRect = m_Self.GetComponent<RectTransform>();
			}

			Vector3 position = m_SelfRect.localPosition;
			position.x += delt;
			m_SelfRect.localPosition = position;
		}
	}

	public override void OpenSelf(GameObject target)
	{
		base.OpenSelf(target);
		UIManager.Instance.AddUpdate(this);
		GameStart.Instance.UIControl = this;
	}

	/// <summary>
	/// 设置自己
	/// </summary>
	/// <param name="sp"></param>
	public void SetSelf(Sprite sp)
	{
		m_Self.gameObject.GetComponent<Image>().sprite = sp;
		m_Self.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, -350, 0);
	}

	/// <summary>
	/// 设置敌人
	/// </summary>
	/// <param name="diren"></param>
	/// <param name="position"></param>
	public void SetDiRen(GameObject diren, Vector3 position)
	{
		RectTransform rect = diren.GetComponent<RectTransform>();
		rect.SetParent(m_DiRen);
		rect.localPosition = position;
		rect.localEulerAngles = Vector3.zero;
		rect.localScale = Vector3.one;
		diren.SetActive(true);
	}

	/// <summary>
	/// 设置子弹
	/// </summary>
	/// <param name="zidans"></param>
	public void SetZiDan(List<GameObject> zidans)
	{
		if (m_SelfRect == null)
		{
			m_SelfRect = m_Self.GetComponent<RectTransform>();
		}

		int x = zidans.Count == 2 ? -15 : 0;
		for (int index = 0; index < zidans.Count; index++)
		{
			RectTransform rect = zidans[index].GetComponent<RectTransform>();
			Vector3 position = new Vector3(x + index * 30, 42, 0);
			position = position + m_SelfRect.localPosition;
			rect.SetParent(m_ZiDan);
			rect.localPosition = position;
			zidans[index].SetActive(true);
			rect.SetParent(m_ZiDan);
			rect.localScale = Vector3.one;
		}
	}

	public override bool Update()
	{
		if (!base.Update())
			return false;

		if (m_Bgs != null && m_IsRun)
		{
			for (int index = 0; index < m_Bgs.Length; index++)
			{
				float y = m_Bgs[index].localPosition.y;
				y -= m_TempY;
				if (y < m_Limit)
				{
					y = m_MaxLimit;
				}

				Vector3 vector3 = new Vector3(0, y, 0);
				m_Bgs[index].localPosition = vector3;
			}
		}

		return true;
	}
}
