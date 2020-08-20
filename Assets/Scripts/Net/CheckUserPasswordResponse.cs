/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:检查账号密码返回
 * Time:2020/8/20 16:43:06
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class CheckUserPasswordResponse : ClientRecvMessageBase
{
	public bool m_IsSuccess;

	public override void AnalyseMessage(int start, MessageHead head, GameNetClient client)
	{
		base.AnalyseMessage(start, head, client);
		byte d = 0;
		if (client.GetMessageWithByte(start, ref d))
		{
			m_IsSuccess = d == 1 ? true : false;
		}
	}
}
