/*
 * Creator:ffm
 * Desc:角色共有方法
 * Time:2020/4/13 16:52:02
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

namespace Game.Engine
{
	public partial class GameCharacterBase : ObjectBase
	{
		/// <summary>
		/// 加载回调
		/// </summary>
		public class LoadCharacter : IResObjectCallBack
		{
			private Action<object, Action<object>> m_LoadAction;

			private Action<object> m_Other;

			public LoadCharacter(Action<object, Action<object>> load, Action<object> end) : base()
			{
				m_LoadAction = load;
				m_Other = end;
			}

			public override void HandleLoadCallBack(object t)
			{
				if (m_LoadAction != null)
				{
					m_LoadAction(t, m_Other);
				}
			}

			public override int LoadCallbackPriority()
			{
				return 0;
			}
		}

		/// <summary>
		/// 加载角色预制体
		/// </summary>
		/// <param name="name">加载名字</param>
		/// <param name="end">加载结束后的方法</param>
		public virtual void StartInitCharacter(string name, Action<object> end)
		{
			LoadCharacter cb = new LoadCharacter(LoadObject, end);
			ResObjectManager.Instance.LoadObject(name, ResObjectType.GameObject, cb);
		}

		/// <summary>
		/// 完成初始化的最后一步
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="gameCharacterAttributeBase"></param>
		/// <param name="animatorBase"></param>
		public virtual void InitCharacter(
			GameCharacterCameraBase gameCharacterCameraBase = null,
			GameCharacterAttributeBase gameCharacterAttributeBase = null,
			GameCharacterAnimatorBase animatorBase = null,
			GameCharacterStateManager gameCharacterStateManager = null,
			CharacterMountControl characterMountControl = null)
		{
			if (gameCharacterCameraBase == null)
			{
				gameCharacterCameraBase = new GameCharacterCameraBase(this.gameObject);
			}

			if (gameCharacterAttributeBase == null)
			{
				gameCharacterAttributeBase = new GameCharacterAttributeBase(m_GCUID);
			}

			if (animatorBase == null)
			{
				animatorBase = new GameCharacterAnimatorBase(this.gameObject.GetComponentInChildren<Animator>());
			}

			if (gameCharacterStateManager == null)
			{
				gameCharacterStateManager = new GameCharacterStateManager(this);
			}

			if (characterMountControl == null)
			{
				characterMountControl = new CharacterMountControl(this);
			}

			m_CharacterCamera = gameCharacterCameraBase;
			m_AttriControl = gameCharacterAttributeBase;
			m_CharacterStateManager = gameCharacterStateManager;
			m_CharacterStateManager.ControlAnimator = animatorBase;
			m_CharacterMountControl = characterMountControl;
			m_HasInit = true;
		}

		/// <summary>
		/// 设置摄像机参数
		/// </summary>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		/// <param name="scale"></param>
		public virtual void SetCameraTra(Vector3 position, Vector3 rotation, Vector3 scale)
		{
			m_CharacterCamera.SetTransform(position, rotation, scale);
		}

		/// <summary>
		/// 清理角色
		/// </summary>
		public virtual void Clear()
		{
			CreateNull();
		}
	}
}
