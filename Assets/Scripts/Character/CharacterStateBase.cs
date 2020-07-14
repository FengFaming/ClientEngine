/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:动画播放场景当中的状态基类
 * Time:2020/4/26 11:33:43
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

/// <summary>
/// 状态基类
/// </summary>
public class CharacterStateBase : ICharacterStateInterface
{
	private Dictionary<string, bool> m_ChangeState;

	public CharacterStateBase(int id) : base(id)
	{
		m_ChangeState = new Dictionary<string, bool>();
		m_ChangeState.Clear();
	}

	/// <summary>
	/// 增加控制内容
	/// </summary>
	/// <param name="kv"></param>
	public void AddChangeStateParameter(KeyValuePair<string, bool> kv)
	{
		if (!m_ChangeState.ContainsKey(kv.Key))
		{
			m_ChangeState.Add(kv.Key, kv.Value);
		}
	}

	/// <summary>
	/// 是否可以打断目标状态进入当前状态
	/// </summary>
	/// <param name="last"></param>
	/// <returns></returns>
	public override bool IsStartState(ICharacterStateInterface last)
	{
		return true;
	}

	public override bool EnterState(params object[] arms)
	{
		if (m_OwnerManager == null)
		{
			return false;
		}

		foreach (KeyValuePair<string, bool> keyValuePair in m_ChangeState)
		{
			m_OwnerManager.ControlAnimator.ChangeParameter(keyValuePair.Key, keyValuePair.Value);
		}

		return true;
	}

	public override void ExitState(bool isManager)
	{
		if (isManager)
		{
			m_OwnerManager.ExitState(this, true);
		}
		else
		{
			int id = GetExitState();
			m_OwnerManager.TryGotoState(id);
		}
	}

	public override int GetExitState()
	{
		return 0;
	}

	public override void StayState()
	{
	}
}
