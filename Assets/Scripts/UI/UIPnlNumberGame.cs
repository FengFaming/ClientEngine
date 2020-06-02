/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:2048游戏界面
 * Time:2020/6/2 10:07:34
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;

public class UIPnlNumberGame : IUIModelControl
{
	private List<GameObject> m_AllGameObjects;

	private NumberGameControl m_Control;

	private NumberGameControl.MoveType m_Type;

	public UIPnlNumberGame() : base()
	{
		m_ModelObjectPath = "UIPnlNumberGame";
		m_IsOnlyOne = true;

		m_AllGameObjects = new List<GameObject>();
		m_Control = new NumberGameControl(new Vector2Int(5, 5));
		m_Type = NumberGameControl.MoveType.Down;
	}

	public override void OpenSelf(GameObject target)
	{
		base.OpenSelf(target);

		GameObject go = m_ControlTarget.transform.Find("data/Image").gameObject;
		go.SetActive(false);
		m_AllGameObjects.Add(go);
		for (int i = 0; i < 24; i++)
		{
			GameObject t = GameObject.Instantiate(go);
			RectTransform rect = t.GetComponent<RectTransform>();
			rect.SetParent(go.transform.parent);
			Vector3 position = new Vector3(-200, 200, 0);
			int l = (i + 1) / 5;
			int w = (i + 1) % 5;
			position.x = position.x + w * 100;
			position.y = position.y - l * 100;
			rect.localPosition = position;
			rect.localEulerAngles = Vector3.zero;
			rect.localScale = Vector3.one;
			t.SetActive(false);
			m_AllGameObjects.Add(t);
		}

		ShowData(NumberGameControl.MoveType.Down);
		//m_Control.NextNumber(NumberGameControl.MoveType.Down);

		MessageManger.Instance.AddMessageListener(EngineMessageHead.LISTEN_KEY_EVENT_FOR_INPUT_MANAGER + "-" + (int)KeyCode.A,
			new IMessageBase(m_ControlTarget, true, AddKey));

		MessageManger.Instance.AddMessageListener(EngineMessageHead.LISTEN_KEY_EVENT_FOR_INPUT_MANAGER + "-" + (int)KeyCode.S,
			new IMessageBase(m_ControlTarget, true, AddKey));

		MessageManger.Instance.AddMessageListener(EngineMessageHead.LISTEN_KEY_EVENT_FOR_INPUT_MANAGER + "-" + (int)KeyCode.D,
			new IMessageBase(m_ControlTarget, true, AddKey));

		MessageManger.Instance.AddMessageListener(EngineMessageHead.LISTEN_KEY_EVENT_FOR_INPUT_MANAGER + "-" + (int)KeyCode.W,
			new IMessageBase(m_ControlTarget, true, AddKey));

		MessageManger.Instance.AddMessageListener(EngineMessageHead.LISTEN_KEY_EVENT_FOR_INPUT_MANAGER + "-" + (int)KeyCode.Q,
			new IMessageBase(m_ControlTarget, true, AddKey));
	}

	public override void ClearData()
	{
		base.ClearData();
		m_AllGameObjects.Clear();
		m_Control = null;
		m_AllGameObjects = null;
	}

	/// <summary>
	/// 按键监听
	/// </summary>
	/// <param name="arms"></param>
	private void AddKey(params object[] arms)
	{
		GameMouseInputManager.KeyInfo info = (GameMouseInputManager.KeyInfo)arms[0];
		if (info.m_KeyState == GameMouseInputManager.KeyState.KeyUp)
		{
			NumberGameControl.MoveType type = NumberGameControl.MoveType.Down;
			switch (info.m_KeyCode)
			{
				case KeyCode.A:
					type = NumberGameControl.MoveType.Up;
					break;
				case KeyCode.W:
					type = NumberGameControl.MoveType.Left;
					break;
				case KeyCode.S:
					type = NumberGameControl.MoveType.Right;
					break;
				case KeyCode.D:
					type = NumberGameControl.MoveType.Down;
					break;
			}

			if (info.m_KeyCode == KeyCode.Q)
			{
				if (!m_Control.NextNumber(m_Type))
				{
					Debug.Log("the game is over.");
					return;
				}

				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 5; j++)
					{
						int data;
						if (m_Control.GetLWData(i, j, out data))
						{
							if (data > 0)
							{
								int id = i * 5 + j;
								m_AllGameObjects[id].transform.Find("Text").gameObject.GetComponent<Text>().text = data.ToString();
								m_AllGameObjects[id].SetActive(true);
							}
						}
					}
				}
			}
			else
			{
				m_Type = type;
				ShowData(type);
			}
		}
	}

	private void ShowData(NumberGameControl.MoveType moveType)
	{
		for (int i = 0; i < m_AllGameObjects.Count; i++)
		{
			m_AllGameObjects[i].SetActive(false);
		}

		m_Control.MoveNumber(moveType, false);
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				int data;
				if (m_Control.GetLWData(i, j, out data))
				{
					if (data > 0)
					{
						int id = i * 5 + j;
						m_AllGameObjects[id].transform.Find("Text").gameObject.GetComponent<Text>().text = data.ToString();
						m_AllGameObjects[id].SetActive(true);
					}
				}
			}
		}
	}
}
