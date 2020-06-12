/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:角色挂载点管理
 * Time:2020/5/11 13:44:00
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

namespace Game.Engine
{
	/// <summary>
	/// 挂载点详细信息
	/// </summary>
	public class MountInfo
	{
		/// <summary>
		/// 挂载点ID
		/// </summary>
		public int m_MountIndex;

		/// <summary>
		/// 挂载点名字
		/// </summary>
		public string m_MountName;

		/// <summary>
		/// 位置偏移
		/// </summary>
		public Vector3 m_MountPosition;

		/// <summary>
		/// 旋转偏移
		/// </summary>
		public Vector3 m_MountRotation;

		/// <summary>
		/// 大小偏移
		/// </summary>
		public Vector3 m_MountScale;

		/// <summary>
		/// 挂载点对象
		/// </summary>
		public GameObject m_MountTarget;

		/// <summary>
		/// 挂载点是否初始化
		/// </summary>
		public bool m_IsInit;

		public MountInfo()
		{
			m_IsInit = false;
			m_MountName = string.Empty;
			m_MountTarget = null;
			m_MountIndex = -1;
			m_MountPosition = Vector3.zero;
			m_MountRotation = Vector3.zero;
			m_MountScale = Vector3.one;
		}

		/// <summary>
		/// 初始化内容
		/// </summary>
		/// <param name="parent"></param>
		public void InitMount(GameObject parent, bool reset = false)
		{
			if (reset)
			{
				m_IsInit = false;
			}

			if (m_IsInit)
			{
				return;
			}

			Transform go = parent.transform.Find(m_MountName);
			if (go == null)
			{
				return;
			}

			m_MountTarget = go.gameObject;
			m_IsInit = true;
		}
	}

	public class CharacterMountControl
	{
		/// <summary>
		/// 管理对象
		/// </summary>
		private GameCharacterBase m_ControlTarget;

		/// <summary>
		/// 母体
		/// </summary>
		private GameObject m_Parent;

		/// <summary>
		/// 所有的对象
		/// </summary>
		private Dictionary<int, MountInfo> m_AllMountInfoDic;

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="ch">控制类</param>
		/// <param name="parent">控制对象</param>
		public CharacterMountControl(GameCharacterBase ch, GameObject parent = null)
		{
			if (parent == null)
			{
				parent = ch.gameObject;
			}

			m_ControlTarget = ch;
			m_Parent = parent;
			m_AllMountInfoDic = new Dictionary<int, MountInfo>();
			m_AllMountInfoDic.Clear();
		}

		/// <summary>
		/// 添加节点
		/// </summary>
		/// <param name="info"></param>
		public void AddMountInfo(MountInfo info)
		{
			if (!m_AllMountInfoDic.ContainsKey(info.m_MountIndex))
			{
				m_AllMountInfoDic.Add(info.m_MountIndex, info);
			}
		}

		/// <summary>
		/// 添加节点
		/// </summary>
		/// <param name="infos"></param>
		public void AddMountInfo(List<MountInfo> infos)
		{
			for (int index = 0; index < infos.Count; index++)
			{
				AddMountInfo(infos[index]);
			}
		}

		/// <summary>
		/// 获取一个节点
		/// </summary>
		/// <param name="id"></param>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject GetMount(int id, ref Vector3 position, ref Vector3 rotation, ref Vector3 scale)
		{
			if (m_AllMountInfoDic.ContainsKey(id))
			{
				if (m_AllMountInfoDic[id].m_IsInit)
				{
					position = m_AllMountInfoDic[id].m_MountPosition;
					rotation = m_AllMountInfoDic[id].m_MountRotation;
					scale = m_AllMountInfoDic[id].m_MountScale;
					return m_AllMountInfoDic[id].m_MountTarget;
				}
				else
				{
					m_AllMountInfoDic[id].InitMount(m_Parent, true);
					return GetMount(id, ref position, ref rotation, ref scale);
				}
			}

			return null;
		}
	}
}
