/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:获取服务器时间消息返回
 * Time:2020/7/21 11:14:00
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class GetServerTimeResponse : ClientRecvMessageBase
{
	public bool m_IsSuccess;
	public long m_ServetTime;

	public GetServerTimeResponse() : base()
	{
		m_IsSuccess = false;
		m_ServetTime = 0;
	}

	public override void AnalyseMessage(int start, MessageHead head, GameNetClient client)
	{
		base.AnalyseMessage(start, head, client);
		if (client.GetMessageWithLong(m_StartPosition, ref m_ServetTime))
		{
			m_IsSuccess = true;
		}
	}

	public override string ToString()
	{
		return m_IsSuccess.ToString() + " " + m_ServetTime.ToString();
	}
}
