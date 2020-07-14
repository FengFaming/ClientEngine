/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:扑克牌主界面
 * Time:2020/5/18 10:51:54
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;

public class UIPnlPuKeMain : IUIModelControl
{
	private Dictionary<int, List<PuKePai>> m_AllPuKe;

	private GameObject m_FaPaiButton;
	private Image m_Left;
	private Image m_Right;
	private Image m_Down;

	//玩家牌
	private PuKeWanJia m_WanJia1;
	private PuKeWanJia m_WanJia2;
	private PuKeWanJia m_Self;
	private PuKePai[] m_Dizhu;

	/// <summary>
	/// 所有牌
	/// </summary>
	private List<GameObject> m_AllTarget;

	/// <summary>
	/// 发牌数量
	/// </summary>
	private int m_FaPaiCout;
	private int m_FaPaiID;

	public UIPnlPuKeMain() : base()
	{
		m_ModelObjectPath = "UIPnlPuKeMain";
		m_IsOnlyOne = true;
	}

	public override void InitUIData(UILayer layer, params object[] arms)
	{
		base.InitUIData(layer, arms);
		m_AllPuKe = arms[0] as Dictionary<int, List<PuKePai>>;
	}

	public override void OpenSelf(GameObject target)
	{
		base.OpenSelf(target);
		Button fapai = m_ControlTarget.gameObject.transform.Find("Button").gameObject.GetComponent<Button>();
		fapai.onClick.AddListener(OnClickFaPai);

		m_FaPaiButton = fapai.gameObject;

		m_Left = m_ControlTarget.gameObject.transform.Find("left/pai").gameObject.GetComponent<Image>();
		m_Right = m_ControlTarget.gameObject.transform.Find("right/pai").gameObject.GetComponent<Image>();
		m_Down = m_ControlTarget.gameObject.transform.Find("down/pai").gameObject.GetComponent<Image>();
		m_Left.gameObject.SetActive(false);
		m_Right.gameObject.SetActive(false);
		m_Down.gameObject.SetActive(false);

		Button jisuan = m_ControlTarget.gameObject.transform.Find("jisuan").gameObject.GetComponent<Button>();
		jisuan.onClick.AddListener(() =>
		{
			DouDiZhuMathManager.GetDouDiZhuManager().StartCal(m_Self,
				null,
				null,
				null);
		});
	}

	/// <summary>
	/// 发牌
	/// </summary>
	private void OnClickFaPai()
	{
		m_FaPaiButton.SetActive(false);
		UIManager.Instance.AddCoroutine(this, AnimationAll);
		//FaPaiLuoJi();
	}

	/// <summary>
	/// 发牌逻辑
	/// </summary>
	private void FaPaiLuoJi()
	{
		m_WanJia1 = new PuKeWanJia();
		m_WanJia1.m_PuKePais = new List<PuKePai>();
		m_WanJia2 = new PuKeWanJia();
		m_WanJia2.m_PuKePais = new List<PuKePai>();
		m_Self = new PuKeWanJia();
		m_Self.m_PuKePais = new List<PuKePai>();
		m_Dizhu = new PuKePai[3];
		List<PuKePai> ids = new List<PuKePai>();
		m_AllTarget = new List<GameObject>();
		for (int index = 0; index < 54; index++)
		{
			PuKePai p = new PuKePai();
			p.m_PuKeColor = index / 13 + 1;
			p.m_PuKeDianShu = index % 13 + 1;
			if (p.GetPuKeTexture(m_AllPuKe))
			{
				ids.Add(p);
			}

			GameObject go = GameObject.Instantiate(m_Left.gameObject);
			RectTransform rect = go.GetComponent<RectTransform>();
			rect.SetParent(m_Left.gameObject.transform.parent.parent);
			rect.localPosition = Vector3.zero;
			rect.localRotation = Quaternion.identity;
			rect.localScale = Vector3.one;
			rect.gameObject.SetActive(true);
			m_AllTarget.Add(go);
		}

		for (int index = 0; index < 3; index++)
		{
			int start = 0;
			int end = ids.Count - 1;
			int id = UnityEngine.Random.Range(start, end);
			m_Dizhu[index] = ids[id];
			ids.RemoveAt(id);
		}

		int cout = 1;
		while (ids.Count > 0)
		{
			int start = 0;
			int end = ids.Count - 1;
			int id = UnityEngine.Random.Range(start, end);
			PuKePai p = ids[id];
			ids.RemoveAt(id);

			switch (cout)
			{
				case 1:
					m_Self.m_PuKePais.Add(p);
					break;
				case 2:
					m_WanJia1.m_PuKePais.Add(p);
					break;
				case 3:
					m_WanJia2.m_PuKePais.Add(p);
					break;
			}

			cout++;
			if (cout > 3)
			{
				cout = 1;
			}
		}

		UIManager.Instance.RemoveCoroutine(this);
		m_FaPaiCout = 0;
		m_FaPaiID = 0;

		m_Self.m_PuKePais.Sort((PuKePai p1, PuKePai p2) =>
		{
			return p1.SwithID() - p2.SwithID();
		});

		m_WanJia1.m_PuKePais.Sort((PuKePai p1, PuKePai p2) =>
		{
			return p1.SwithID() - p2.SwithID();
		});

		m_WanJia2.m_PuKePais.Sort((PuKePai p1, PuKePai p2) =>
		{
			return p1.SwithID() - p2.SwithID();
		});

		UIManager.Instance.AddCoroutine(this, AnimationLeft);
		UIManager.Instance.AddCoroutine(this, AnimationRight);
		UIManager.Instance.AddCoroutine(this, AnimationSelf);
	}

	/// <summary>
	/// 所有牌
	/// </summary>
	private void AnimationAll()
	{
		FaPaiLuoJi();
	}

	/// <summary>
	/// 左边动画
	/// </summary>
	private void AnimationLeft()
	{
		if (m_FaPaiCout == 1)
		{
			if (m_FaPaiID > 17)
			{
				//UIManager.Instance.RemoveCoroutine(this);
			}
			else
			{
				PuKePai p = m_WanJia1.m_PuKePais[m_FaPaiID];
				int id = m_FaPaiID * 3 + 1;
				GameObject go = m_AllTarget[id];
				go.GetComponent<Image>().sprite = p.m_PuKeTexture;
				RectTransform rect = go.GetComponent<RectTransform>();
				rect.SetParent(m_Left.transform.parent);
				float x = 136 - m_FaPaiID * 20;
				rect.localPosition = new Vector3(0, x, 0);
				rect.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
				m_FaPaiCout = 2;
			}
		}
	}

	/// <summary>
	/// 右边动画
	/// </summary>
	private void AnimationRight()
	{
		if (m_FaPaiCout == 2)
		{
			if (m_FaPaiID > 17)
			{
				//UIManager.Instance.RemoveCoroutine(this);
			}
			else
			{
				PuKePai p = m_WanJia2.m_PuKePais[m_FaPaiID];
				int id = m_FaPaiID * 3 + 2;
				GameObject go = m_AllTarget[id];
				go.GetComponent<Image>().sprite = p.m_PuKeTexture;
				RectTransform rect = go.GetComponent<RectTransform>();
				rect.SetParent(m_Right.transform.parent);
				float x = 136 - m_FaPaiID * 20;
				rect.localPosition = new Vector3(0, x, 0);
				rect.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
				m_FaPaiCout = 0;
				m_FaPaiID++;
			}
		}
	}

	/// <summary>
	/// 自己动画
	/// </summary>
	private void AnimationSelf()
	{
		if (m_FaPaiCout == 0)
		{
			if (m_FaPaiID >= 17)
			{
				UIManager.Instance.RemoveCoroutine(this);
				UIManager.Instance.AddCoroutine(this, DiZhuPai);
			}
			else
			{
				PuKePai p = m_Self.m_PuKePais[m_FaPaiID];
				int id = m_FaPaiID * 3 + 0;
				GameObject go = m_AllTarget[id];
				go.GetComponent<Image>().sprite = p.m_PuKeTexture;
				RectTransform rect = go.GetComponent<RectTransform>();
				rect.SetParent(m_Down.transform.parent);
				float x = -160 + m_FaPaiID * 20;
				rect.localPosition = new Vector3(x, 0, 0);
				m_FaPaiCout = 1;
			}
		}
	}

	/// <summary>
	/// 地主牌
	/// </summary>
	private void DiZhuPai()
	{
		int id = m_FaPaiID * 3;
		for (int index = 0; index < 3; index++)
		{
			PuKePai p = m_Dizhu[index];
			GameObject go = m_AllTarget[id + index];
			go.GetComponent<Image>().sprite = p.m_PuKeTexture;
			RectTransform rect = go.GetComponent<RectTransform>();
			float x = -20 + index * 20;
			rect.localPosition = new Vector3(x, 0, 0);
		}

		UIManager.Instance.RemoveCoroutine(this);
	}
}
