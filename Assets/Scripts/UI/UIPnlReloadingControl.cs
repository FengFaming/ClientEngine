/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:换装查看
 * Time:2020/5/12 10:24:35
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;

public class UIPnlReloadingControl : UIModelLuaControl
{
	private List<int> m_Animations;
	private Dictionary<string, List<CharacterXmlControl.LoadInfo>> m_ReloadingInfos;
	private ReloadingPlayer m_Control;

	public UIPnlReloadingControl() : base()
	{
		m_ModelObjectPath = "UIPnlReloadingControl";
		m_IsOnlyOne = true;
		m_LuaName = "UIPnlReloadingControl";

		m_Animations = new List<int>();
		m_Animations.Clear();

		m_ReloadingInfos = new Dictionary<string, List<CharacterXmlControl.LoadInfo>>();
		m_ReloadingInfos.Clear();
		m_Control = null;
	}

	/// <summary>
	/// 初始化数据
	/// </summary>
	/// <param name="layer"></param>
	/// <param name="arms"></param>
	public override void InitUIData(UILayer layer, params object[] arms)
	{
		base.InitUIData(layer, arms);
		if (arms.Length == 2)
		{
			m_Control = arms[0] as ReloadingPlayer;
			string xml = (string)arms[1];
			CharacterXmlControl config = new CharacterXmlControl(xml);
			ConfigurationManager.Instance.LoadXml(ref config);
			foreach (var item in config.m_StateInfos)
			{
				m_Animations.Add(item.Key);
			}
		}
	}

	public override void OpenSelf(GameObject target)
	{
		base.OpenSelf(target);

		XLua.LuaFunction luaFunction = m_UILuaTable.Get<XLua.LuaFunction>("getreloading");
		if (luaFunction != null)
		{
			System.Object[] vs = luaFunction.Call();
			foreach (System.Object o in vs)
			{
				string s = (string)o;
				string[] ss = s.Split(':');
				string[] sss = ss[1].Split(',');

				List<CharacterXmlControl.LoadInfo> infos = new List<CharacterXmlControl.LoadInfo>();
				foreach (string ssss in sss)
				{
					CharacterXmlControl.LoadInfo info = new CharacterXmlControl.LoadInfo();
					info.m_Key = ss[0];
					info.m_Model = ssss;
					info.m_Position = Vector3.zero;
					info.m_Rotation = Vector3.zero;
					info.m_Scale = Vector3.one;
					infos.Add(info);
				}

				m_ReloadingInfos.Add(ss[0], infos);
			}
		}

		GameObject bt = m_ControlTarget.transform.Find("animation/Button").gameObject;
		for (int index = 0; index < m_Animations.Count - 1; index++)
		{
			GameObject go = GameObject.Instantiate(bt);
			RectTransform rt = go.GetComponent<RectTransform>();
			rt.SetParent(bt.transform.parent);
			rt.SetAsLastSibling();
			rt.localPosition = Vector3.zero;
			rt.localRotation = Quaternion.identity;
			rt.localScale = Vector3.one;
			ButtonData bd = go.AddComponent<ButtonData>();
			bd.Data = m_Animations[index];
			Button dbt = go.GetComponent<Button>();
			go.GetComponentInChildren<Text>().text = m_Animations[index].ToString();
			dbt.onClick.AddListener(() =>
			{
				OnClickAnimation(bd);
			});
		}

		bt.GetComponentInChildren<Text>().text = m_Animations[m_Animations.Count - 1].ToString();
		RectTransform brt = bt.GetComponent<RectTransform>();
		brt.SetAsLastSibling();
		ButtonData bbd = bt.AddComponent<ButtonData>();
		bbd.Data = m_Animations[m_Animations.Count - 1];
		Button ddbt = bt.GetComponent<Button>();
		ddbt.onClick.AddListener(() =>
		{
			OnClickAnimation(bbd);
		});

		CalReloading();
	}

	private void OnClickAnimation(ButtonData name)
	{
		Debug.Log(name.Data);
		m_Control.StateManager.TryGotoState((int)name.Data);
	}

	private void OnClickChange(ButtonData info)
	{
		CharacterXmlControl.LoadInfo loadInfo = (CharacterXmlControl.LoadInfo)info.Data;
		m_Control.ChangeReloading(loadInfo);
	}

	private void CalReloading(List<CharacterXmlControl.LoadInfo> infos, GameObject gameObject)
	{
		GameObject bt = gameObject.transform.Find("Button").gameObject;
		for (int index = 0; index < infos.Count - 1; index++)
		{
			GameObject go = GameObject.Instantiate(bt);
			RectTransform rt = go.GetComponent<RectTransform>();
			rt.SetParent(gameObject.transform);
			rt.SetAsLastSibling();
			rt.localPosition = Vector3.zero;
			rt.localRotation = Quaternion.identity;
			rt.localScale = Vector3.one;
			go.GetComponentInChildren<Text>().text = infos[index].m_Model;
			ButtonData data = go.AddComponent<ButtonData>();
			data.Data = infos[index];
			go.GetComponent<Button>().onClick.AddListener(() =>
			{
				OnClickChange(data);
			});
		}

		bt.transform.SetAsLastSibling();
		ButtonData bd = bt.AddComponent<ButtonData>();
		bd.Data = infos[infos.Count - 1];
		bt.GetComponentInChildren<Text>().text = infos[infos.Count - 1].m_Model;
		bt.GetComponent<Button>().onClick.AddListener(() =>
		{
			OnClickChange(bd);
		});
	}

	private void CalReloading()
	{
		GameObject gameObject = m_ControlTarget.transform.Find("reloadings").gameObject;
		RectTransform gt = gameObject.GetComponent<RectTransform>();
		int cout = 0;
		foreach (var item in m_ReloadingInfos)
		{
			GameObject go = GameObject.Instantiate(gameObject);
			RectTransform rt = go.GetComponent<RectTransform>();
			rt.SetParent(gameObject.transform.parent);
			rt.anchoredPosition = new Vector2(-50 - (cout * 100), gt.anchoredPosition.y);
			rt.anchoredPosition3D = new Vector3(rt.anchoredPosition3D.x, rt.anchoredPosition3D.y, 0);
			rt.localRotation = Quaternion.identity;
			rt.localScale = Vector3.one;
			CalReloading(item.Value, go);
			cout++;
		}

		gameObject.SetActive(false);
	}
}
