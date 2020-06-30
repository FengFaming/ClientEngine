/*
 * Creator:ffm
 * Desc:游戏主界面
 * Time:2020/4/11 9:56:15
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;

public class UIPnlGameMain : IUIModelControl
{
	public UIPnlGameMain() : base()
	{
		m_ModelObjectPath = "UIPnlGameMain";
		m_IsOnlyOne = true;
	}

	private InputField m_InputCout;

	public override void OpenSelf(GameObject target)
	{
		base.OpenSelf(target);

		m_ControlTarget.gameObject.AddComponent<PaiLieZuHeControl>();

		Button animation = m_ControlTarget.gameObject.transform.Find("animation").gameObject.GetComponent<Button>();
		Button shoot = m_ControlTarget.gameObject.transform.Find("shoot").gameObject.GetComponent<Button>();
		Button lua = m_ControlTarget.gameObject.transform.Find("lua").gameObject.GetComponent<Button>();
		Button reloaing = m_ControlTarget.gameObject.transform.Find("reloading").gameObject.GetComponent<Button>();
		Button puke = m_ControlTarget.gameObject.transform.Find("puke").gameObject.GetComponent<Button>();
		Button ce = m_ControlTarget.gameObject.transform.Find("ces").gameObject.GetComponent<Button>();

		animation.onClick.AddListener(new UnityEngine.Events.UnityAction(() => { OnClickAnimation(1); }));
		shoot.onClick.AddListener(new UnityEngine.Events.UnityAction(() => { OnClickAnimation(2); }));
		lua.onClick.AddListener(new UnityEngine.Events.UnityAction(() => { OnClickAnimation(3); }));
		reloaing.onClick.AddListener(new UnityEngine.Events.UnityAction(() => { OnClickAnimation(4); }));
		puke.onClick.AddListener(new UnityEngine.Events.UnityAction(() => { OnClickAnimation(5); }));
		ce.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
		{
			OnClickAnimation(6);
		}));

		m_InputCout = m_ControlTarget.gameObject.transform.Find("cout").gameObject.GetComponent<InputField>();
	}

	private void OnClickAnimation(int tage)
	{
		switch (tage)
		{
			case 1:
				GameSceneManager.Instance.ChangeScene(new AnimationScene("animationscene"));
				break;
			case 2:
				//GameSceneManager.Instance.ChangeScene(new ShootGameScene());
				OtherGameControl.Instance.OpenOtherExe("SmallShoot", "450,800");
				break;
			case 3:
				UIManager.Instance.OpenUI("UIPnlInputCout", UILayer.Blk);
				break;
			case 4:
				GameSceneManager.Instance.ChangeScene(new ReloadingScene("reloadingscene"));
				break;
			case 5:
				GameSceneManager.Instance.ChangeScene(new PuKePaiScene("pukepaiscene"));
				break;
			case 6:
				//object[] d = new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 16, 17, 18, 19, 20, 21 };
				//int cout = int.Parse(m_InputCout.text);
				////PaiLieZuHeControl.GetInstance().UseThreadT(d, 0, cout);
				//CombinationManager cb = new CombinationManager();
				//cb.Combination(d, new Vector2Int(0, cout), CalEnd);
				////PaiLieZuHeControl.GetInstance().UserThreadZuHe(d, 0, cout);
				////Debug.Log(Time.realtimeSinceStartup);
				////List<object[]> r = EngineTools.Instance.GetPermutation<object>(d, 0, cout);
				////Debug.Log(Time.realtimeSinceStartup + " " + r.Count);
				GameSceneManager.Instance.ChangeScene(new AdditiveSceneTest("testload"));
				break;
		}
	}

	private void CalEnd(CombinationManager manager)
	{
		GC.Collect();
		manager = null;
	}

	public override bool GetCloseOther(ref List<string> others)
	{
		return true;
	}
}
