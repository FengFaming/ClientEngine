/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:开始游戏请求
 * Time:2020/7/20 10:07:04
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class StartGameRequest : ClientSendMessageBase
{
	public StartGameRequest() : base()
	{
		m_MessageHead.m_MessageID = 10010;
		m_MessageHead.m_MessageType = 3;
		m_MessageHead.m_MessageLength = 9;
	}

	/// <summary>
	/// 设置数据
	/// </summary>
	/// <param name="user"></param>
	/// <param name="password"></param>
	public void SetSendData(string user, string password)
	{
		SetSendString(user);
		SetSendString(password);
	}
}
