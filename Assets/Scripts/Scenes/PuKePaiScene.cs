/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:扑克牌场景
 * Time:2020/5/18 8:45:19
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;

public class PuKePaiScene : IScene
{
	public class PuKePaiControl : MonoBehaviour
	{
		public class PuKeLoadBack : IResObjectCallBack
		{
			public Action<PuKePai> m_LoadEnd;
			public PuKePai m_EndData;

			public override void HandleLoadCallBack(object t)
			{
				m_EndData.m_PuKeTexture = t as Sprite;
				m_LoadEnd(m_EndData);
			}

			public override int LoadCallbackPriority()
			{
				return 0;
			}
		}

		/// <summary>
		/// 所有扑克牌
		/// </summary>
		public Dictionary<int, List<PuKePai>> m_AllPuKeDic;

		private Action<float> m_LoadAction;
		private int m_Cout;

		/// <summary>
		/// 开始场景
		/// </summary>
		/// <param name="action"></param>
		public void StartScene(Action<float> action)
		{
			m_LoadAction = action;
			m_AllPuKeDic = new Dictionary<int, List<PuKePai>>();
			m_AllPuKeDic.Clear();
			m_Cout = 0;
			StartCoroutine("LoadScene");
		}

		/// <summary>
		/// 加载扑克回调
		/// </summary>
		/// <param name="data"></param>
		private void LoadPuKe(PuKePai data)
		{
			if (m_AllPuKeDic.ContainsKey(data.m_PuKeColor))
			{
				m_AllPuKeDic[data.m_PuKeColor].Add(data);
			}
			else
			{
				List<PuKePai> ps = new List<PuKePai>();
				ps.Clear();

				ps.Add(data);
				m_AllPuKeDic.Add(data.m_PuKeColor, ps);
			}

			m_Cout++;
			if (m_Cout >= 54)
			{
				m_LoadAction(100);

				UIManager.Instance.OpenUI("UIPnlPuKeMain", UILayer.Pnl, m_AllPuKeDic);
				UIManager.Instance.OpenUI("UIPnlBackGameMain", UILayer.Pnl, new Vector3(0, 240, 0));
			}
			else
			{
				m_LoadAction(m_Cout * 0.54f);
			}
		}

		private IEnumerator LoadScene()
		{
			yield return null;
			for (int index = 0; index < 54; index++)
			{
				PuKePai p = new PuKePai();
				p.m_PuKeColor = index / 13 + 1;
				p.m_PuKeDianShu = index % 13 + 1;

				PuKeLoadBack plb = new PuKeLoadBack();
				plb.m_EndData = p;
				plb.m_LoadEnd = LoadPuKe;
				ResObjectManager.Instance.LoadObject(p.GetPuKeID().ToString(), ResObjectType.Icon, plb);
				yield return null;
			}
		}

		/// <summary>
		/// 结束场景
		/// </summary>
		/// <param name="action"></param>
		public void EndScene()
		{

		}
	}

	private PuKePaiControl m_PuKePaiControl;
	public PuKePaiScene(string name) : base(name)
	{

	}

	/// <summary>
	/// 清理场景
	/// </summary>
	public override void ClearSceneData()
	{
		base.ClearSceneData();
		m_PuKePaiControl.EndScene();
		m_PuKePaiControl = null;
	}

	/// <summary>
	/// 初始化场景
	/// </summary>
	public override void InitScene()
	{
		base.InitScene();

		///为什么要在这里创建，因为场景切换的过程当中会删除掉一部分
		GameObject scene = new GameObject();
		scene.name = m_SceneName;
		scene.gameObject.transform.position = Vector3.zero;
		scene.gameObject.transform.eulerAngles = Vector3.zero;
		scene.gameObject.transform.localScale = Vector3.one;
		m_PuKePaiControl = scene.gameObject.AddComponent<PuKePaiControl>();
	}

	/// <summary>
	/// 加载场景
	/// </summary>
	/// <param name="action"></param>
	public override void LoadScene(Action<float> action)
	{
		m_PuKePaiControl.StartScene(action);
	}
}
