/*
 * Creator:ffm
 * Desc:游戏角色控制
 * Time:2020/4/13 16:16:42
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public partial class GameCharacterBase : ObjectBase
	{
		/// <summary>
		/// 角色唯一ID
		/// </summary>
		protected int m_GCUID;
		public int GCUID { get { return m_GCUID; } }

		/// <summary>
		/// 服务器ID
		/// </summary>
		protected int m_ServerID;
		public int ServerID { get { return m_ServerID; } }

		/// <summary>
		/// 角色名字
		/// </summary>
		protected string m_GCName;
		public string GCName { get { return m_GCName; } }

		/// <summary>
		/// 角色属性存储器
		/// </summary>
		protected GameCharacterAttributeBase m_AttriControl;
		public GameCharacterAttributeBase AttriControl { get { return m_AttriControl; } }

		/// <summary>
		/// 角色状态管理器
		/// </summary>
		protected GameCharacterStateManager m_CharacterStateManager;
		public GameCharacterStateManager StateManager { get { return m_CharacterStateManager; } }

		/// <summary>
		/// 挂载点控制
		/// </summary>
		protected CharacterMountControl m_CharacterMountControl;

		/// <summary>
		/// 角色摄像机
		/// </summary>
		protected GameCharacterCameraBase m_CharacterCamera;

		/// <summary>
		/// 摄像机追随时间
		/// </summary>
		protected float m_LateCameraTime;

		/// <summary>
		/// 是否具有初始化
		/// </summary>
		protected bool m_HasInit;
	}
}
