/*
 * Creator:ffm
 * Desc:动作场景玩家
 * Time:2020/4/14 8:45:50
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class AnimationPlayer : GameCharacterBase
{
	private Rigidbody m_ControlRigidbody;

	private bool m_IsWalk;

	public override void InitCharacter(GameCharacterCameraBase gameCharacterCameraBase = null,
										GameCharacterAttributeBase gameCharacterAttributeBase = null,
										GameCharacterAnimatorBase animatorBase = null,
										GameCharacterStateManager gameCharacterStateManager = null,
										CharacterMountControl characterMountControl = null)
	{
		base.InitCharacter(gameCharacterCameraBase, gameCharacterAttributeBase, animatorBase, gameCharacterStateManager, characterMountControl);
		BoxCollider bc = this.gameObject.GetComponentInChildren<BoxCollider>();
		BoxCollider bvc = this.gameObject.AddComponent<BoxCollider>();
		bvc.center = bc.center;
		bvc.size = bc.size;
		GameObject.DestroyImmediate(bc);

		Rigidbody rb = this.gameObject.GetComponentInChildren<Rigidbody>();
		Rigidbody n = this.gameObject.AddComponent<Rigidbody>();
		EngineTools.Instance.CopyRigidbody(rb, ref n);
		GameObject.DestroyImmediate(rb);

		m_ControlRigidbody = n;

		this.gameObject.AddComponent<AnimatorBase>();
		GameMouseInputManager.Instance.SetMouseListen(EngineMessageHead.LISTEN_MOUSE_EVENT_FOR_INPUT_MANAGER, 5);
		MessageManger.Instance.AddMessageListener(EngineMessageHead.LISTEN_MOUSE_EVENT_FOR_INPUT_MANAGER,
			new IMessageBase(this.gameObject, false, ListenMouse));

		MessageManger.Instance.AddMessageListener(EngineMessageHead.LISTEN_KEY_EVENT_FOR_INPUT_MANAGER + "-" + (int)KeyCode.A,
			new IMessageBase(this.gameObject, false, ListenKey));

		MessageManger.Instance.AddMessageListener(EngineMessageHead.LISTEN_KEY_EVENT_FOR_INPUT_MANAGER + "-" + (int)KeyCode.D,
			new IMessageBase(this.gameObject, false, ListenKey));

		MessageManger.Instance.AddMessageListener(EngineMessageHead.LISTEN_KEY_EVENT_FOR_INPUT_MANAGER + "-" + (int)KeyCode.W,
			new IMessageBase(this.gameObject, false, ListenKey));

		MessageManger.Instance.AddMessageListener(EngineMessageHead.LISTEN_KEY_EVENT_FOR_INPUT_MANAGER + "-" + (int)KeyCode.S,
			new IMessageBase(this.gameObject, false, ListenKey));

		m_CharacterStateManager.TryGotoState(0);
		m_IsWalk = true;
	}

	private void ListenKey(params object[] arms)
	{
		GameMouseInputManager.KeyInfo info = (GameMouseInputManager.KeyInfo)arms[0];
		Vector3 focale = Vector3.zero;
		if (info.m_KeyState == GameMouseInputManager.KeyState.KeyStay)
		{
			if (info.m_KeyCode == KeyCode.A)
			{
				focale.x = -1;
			}

			if (info.m_KeyCode == KeyCode.D)
			{
				focale.x = 1;
			}

			if (info.m_KeyCode == KeyCode.W)
			{
				focale.z = 1;
			}

			if (info.m_KeyCode == KeyCode.S)
			{
				focale.z = -1;
			}

			MoveControl(focale);
		}

		if (info.m_KeyState == GameMouseInputManager.KeyState.KeyUp)
		{
			if (focale == Vector3.zero)
			{
				if (m_ControlRigidbody != null)
				{
					m_ControlRigidbody.velocity = focale;
				}

				m_IsWalk = true;
				m_CharacterStateManager.TryGotoState(0);
			}
		}
	}

	private void MoveControl(Vector3 sp)
	{
		GameObjectMoveControl move = this.gameObject.GetComponent<GameObjectMoveControl>();
		if (move == null)
		{
			move = this.gameObject.AddComponent<GameObjectMoveControl>();
		}

		Quaternion q = Quaternion.LookRotation(sp);
		move.SetRotation(q, 0.1f);

		if (m_ControlRigidbody != null)
		{
			if (m_IsWalk)
			{
				m_CharacterStateManager.TryGotoState(1);
			}
			else
			{
				sp *= 2;
				m_CharacterStateManager.TryGotoState(2);
			}

			m_ControlRigidbody.velocity = sp;
		}
	}

	private void ListenMouse(params object[] arms)
	{
		GameMouseInputManager.ListenEvent le = (GameMouseInputManager.ListenEvent)arms[0];

		if (le.m_MouseType == GameMouseInputManager.MouseEventType.Mouse_0_Down)
		{
			m_IsWalk = true;
		}
		else if (le.m_MouseType == GameMouseInputManager.MouseEventType.Mouse_1_Down)
		{
			m_IsWalk = false;
		}
	}

	public override void Clear()
	{
		base.Clear();
		GameMouseInputManager.Instance.SetMouseListen("", 0);
		MessageManger.Instance.RemoveMessageListener(this.gameObject);
	}
}
