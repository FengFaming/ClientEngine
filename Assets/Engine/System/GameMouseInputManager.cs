/*
 * Creator:ffm
 * Desc:鼠标输入管理
 * Time:2020/4/14 8:48:36
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class GameMouseInputManager : SingletonMonoClass<GameMouseInputManager>
	{
		public enum ListenType
		{
			/// <summary>
			/// 都不监听
			/// </summary>
			None,

			/// <summary>
			/// 场景
			/// </summary>
			Scence,

			/// <summary>
			/// UI
			/// </summary>
			UI,

			/// <summary>
			/// 所有
			/// </summary>
			All
		}

		/// <summary>
		/// 鼠标当时状态
		/// </summary>
		public enum MouseEventType
		{
			/// <summary>
			/// 左键按下
			/// </summary>
			Mouse_0_Down,

			/// <summary>
			/// 左键弹起
			/// </summary>
			Mouse_0_Up,

			/// <summary>
			/// 左键维持
			/// </summary>
			Mouse_0_Stay,

			/// <summary>
			/// 右键按下
			/// </summary>
			Mouse_1_Down,

			/// <summary>
			/// 右键弹起
			/// </summary>
			Mouse_1_Up,

			/// <summary>
			/// 右键维持
			/// </summary>
			Mouse_1_Stay
		}

		/// <summary>
		/// 鼠标发送事件结构
		/// </summary>
		public struct ListenEvent
		{
			/// <summary>
			/// 鼠标位置
			/// </summary>
			public Vector2 m_MousePosition;

			/// <summary>
			/// 鼠标所指的场景位置
			/// </summary>
			public Vector3 m_ScenePosition;

			/// <summary>
			/// 鼠标当时状态
			/// </summary>
			public MouseEventType m_MouseType;

			/// <summary>
			/// 鼠标射线检测的所有物体
			/// </summary>
			public Dictionary<int, List<KeyValuePair<int, GameObject>>> m_RayHitGameObjects;
		}

		/// <summary>
		/// 按键状态
		/// </summary>
		public enum KeyState
		{
			/// <summary>
			/// 按键按下
			/// </summary>
			KeyDown,

			/// <summary>
			/// 按下维持
			/// </summary>
			KeyStay,

			/// <summary>
			/// 按键弹起
			/// </summary>
			KeyUp,
		}

		/// <summary>
		/// 按键的事件结合
		/// </summary>
		public struct KeyInfo
		{
			/// <summary>
			/// 按键
			/// </summary>
			public KeyCode m_KeyCode;

			/// <summary>
			/// 按键状态
			/// </summary>
			public KeyState m_KeyState;

			/// <summary>
			/// 按键按下的时间
			/// </summary>
			public float m_DownTime;
		}

		/// <summary>
		/// 是否开启鼠标监听
		/// </summary>
		private bool m_ListenMouse;

		/// <summary>
		/// 鼠标监听层面
		/// </summary>
		private ListenType m_ListenType;

		/// <summary>
		/// 场景深度
		/// </summary>
		private float m_SceneDistance;

		/// <summary>
		/// 鼠标监听事件返回
		/// </summary>
		private string m_ListenHead;

		/// <summary>
		/// 鼠标监听维持事件
		/// </summary>
		private float m_StayDeltTime;

		/// <summary>
		/// 鼠标监听当前时间
		/// </summary>
		private float m_StayTime;

		/// <summary>
		/// 是否维持
		/// </summary>
		private bool m_HasStay;

		/// <summary>
		/// 射线检测深度
		/// </summary>
		private int m_RayDistance;

		/// <summary>
		/// 射线检测层级
		/// </summary>
		private int m_RayLayer;

		/// <summary>
		/// 射线主要摄像机
		/// </summary>
		private Camera m_ControlCamera;

		protected override void Awake()
		{
			base.Awake();
			m_ListenHead = string.Empty;
			m_ListenMouse = false;
			m_ControlCamera = Camera.main;
			m_ListenType = ListenType.None;
			m_RayDistance = 0;
			m_RayLayer = 0;
			m_SceneDistance = 0;
			m_StayDeltTime = 0;
			m_HasStay = false;
		}

		/// <summary>
		/// 设置监听内容
		/// </summary>
		/// <param name="head"></param>
		/// <param name="distance"></param>
		/// <param name="listenType"></param>
		/// <param name="stayTime"></param>
		/// <param name="rayDistance"></param>
		/// <param name="layer"></param>
		public void SetMouseListen(string head, float distance, ListenType listenType = ListenType.All, float stayTime = 0.1f,
									Camera rayCamera = null, int rayDistance = int.MaxValue, int layer = int.MaxValue)
		{
			m_ListenHead = head;
			if (!string.IsNullOrEmpty(m_ListenHead))
			{
				m_ListenMouse = true;
				m_SceneDistance = distance;
				m_ListenType = listenType;
				m_StayDeltTime = stayTime;
				m_ControlCamera = rayCamera == null ? Camera.main : rayCamera;
				m_RayDistance = rayDistance;
				m_RayLayer = layer;
			}
			else
			{
				m_ListenHead = string.Empty;
				m_ListenMouse = false;
				m_ControlCamera = Camera.main;
				m_ListenType = ListenType.None;
				m_RayDistance = 0;
				m_RayLayer = 0;
				m_SceneDistance = 0;
				m_StayDeltTime = 0;
			}

			m_HasStay = false;
		}

		/// <summary>
		/// 数据发送
		/// </summary>
		/// <param name="type"></param>
		private void SetListenEvent(MouseEventType type, RaycastHit[] hit)
		{
			ListenEvent listen = new ListenEvent();
			listen.m_MouseType = type;
			listen.m_MousePosition = Input.mousePosition;
			Vector3 position = new Vector3(listen.m_MousePosition.x, listen.m_MousePosition.y, m_SceneDistance);
			listen.m_ScenePosition = m_ControlCamera.ScreenToWorldPoint(position);
			listen.m_RayHitGameObjects = new Dictionary<int, List<KeyValuePair<int, GameObject>>>();
			listen.m_RayHitGameObjects.Clear();
			if (hit.Length > 0)
			{
				for (int index = 0; index < hit.Length; index++)
				{
					GameObject hitcollider = hit[index].collider.gameObject;
					KeyValuePair<int, GameObject> value = new KeyValuePair<int, GameObject>(index, hitcollider);
					int layer = hitcollider.layer;
					if (listen.m_RayHitGameObjects.ContainsKey(layer))
					{
						listen.m_RayHitGameObjects[layer].Add(value);
					}
					else
					{
						List<KeyValuePair<int, GameObject>> ls = new List<KeyValuePair<int, GameObject>>();
						ls.Clear();
						ls.Add(value);
						listen.m_RayHitGameObjects.Add(layer, ls);
					}
				}
			}

			MessageManger.Instance.SendMessage(m_ListenHead, listen);
		}

		private void Update()
		{
			ListenMouse();
			ListenKey();
		}

		/// <summary>
		/// 监听按键
		/// </summary>
		private void ListenKey()
		{
			///f1 - f15
			for (int index = 282; index < 297; index++)
			{
				KeyInput(index);
			}

			for (int index = 97; index < 122; index++)
			{
				KeyInput(index);
			}

			///ESC
			KeyInput(27);

			//Tab
			KeyInput((int)(KeyCode.Tab));

			//Enter
			KeyInput((int)(KeyCode.Return));
			KeyInput((int)(KeyCode.KeypadEnter));

			//Space
			KeyInput((int)KeyCode.Space);
		}

		/// <summary>
		/// 设置按键监听状态
		/// </summary>
		private void SetKeyCodeListenEvent(KeyState state, int key)
		{
			KeyInfo info = new KeyInfo();
			info.m_KeyState = state;
			info.m_KeyCode = (KeyCode)key;
			info.m_DownTime = Time.time;
			string head = EngineMessageHead.LISTEN_KEY_EVENT_FOR_INPUT_MANAGER + "-" + key;
			MessageManger.Instance.SendMessage(head, info);
		}

		private void KeyInput(int index)
		{
			if (Input.GetKeyDown((KeyCode)index))
			{
				SetKeyCodeListenEvent(KeyState.KeyDown, index);
			}

			if (Input.GetKey((KeyCode)index))
			{
				SetKeyCodeListenEvent(KeyState.KeyStay, index);
			}

			if (Input.GetKeyUp((KeyCode)index))
			{
				SetKeyCodeListenEvent(KeyState.KeyUp, index);
			}
		}

		/// <summary>
		/// 监听鼠标
		/// </summary>
		private void ListenMouse()
		{
			if (m_ListenMouse)
			{
				Ray ray = m_ControlCamera.ScreenPointToRay(Input.mousePosition);
				Debug.DrawRay(ray.origin, ray.direction, Color.red);
				RaycastHit[] hit = Physics.RaycastAll(ray, m_RayDistance, m_RayLayer);

				if (m_HasStay)
				{
					if ((GameTimeManager.Instance.GameNowTime - m_StayTime) > m_StayDeltTime)
					{
						if (Input.GetMouseButton(0))
						{
							SetListenEvent(MouseEventType.Mouse_0_Stay, hit);
						}
						else
						{
							SetListenEvent(MouseEventType.Mouse_1_Stay, hit);
						}
					}
				}

				if (Input.GetMouseButtonDown(0))
				{
					m_StayTime = GameTimeManager.Instance.GameNowTime;
					m_HasStay = true;
					SetListenEvent(MouseEventType.Mouse_0_Down, hit);
				}

				if (Input.GetMouseButtonDown(1))
				{
					m_StayTime = GameTimeManager.Instance.GameNowTime;
					m_HasStay = true;
					SetListenEvent(MouseEventType.Mouse_1_Down, hit);
				}

				if (Input.GetMouseButtonUp(0))
				{
					m_StayTime = GameTimeManager.Instance.GameNowTime;
					m_HasStay = false;
					SetListenEvent(MouseEventType.Mouse_0_Up, hit);
				}

				if (Input.GetMouseButtonUp(1))
				{
					m_StayTime = GameTimeManager.Instance.GameNowTime;
					m_HasStay = false;
					SetListenEvent(MouseEventType.Mouse_1_Up, hit);
				}
			}
		}
	}
}