using System;
using System.Collections.Generic;
using System.Text;

public class TimeSendMessage : SocketMessageBase
{
	/// <summary>
	/// 服务器发送时间
	/// </summary>
	private long m_SendTime;

	/// <summary>
	/// 客户端发送时间
	/// </summary>
	private long m_ClientTime;

	public TimeSendMessage() : base()
	{
		m_MessageHead = new MessageHead();
		m_MessageHead.m_MessageID = 100001;
		m_MessageHead.m_MessageType = 1;
	}

	/// <summary>
	/// 解析客户端消息
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	public override bool AnaysizeMessage(byte[] data, ClientInfo client)
	{
		if (!base.AnaysizeMessage(data, client))
			return false;

		m_ClientTime = BitConverter.ToInt64(data, 0);
		Console.WriteLine(m_ClientTime);
		return true;
	}

	/// <summary>
	/// 设置发送时间
	/// </summary>
	/// <param name="time"></param>
	public void SetTime(long time)
	{
		m_SendTime = time;
	}

	/// <summary>
	/// 获取发送内容
	/// </summary>
	/// <returns></returns>
	public override List<byte> GetSendMessage()
	{
		List<byte> vs = new List<byte>();
		vs.AddRange(System.BitConverter.GetBytes(m_SendTime));
		m_MessageHead.m_MessageLength = vs.Count + 9;
		List<byte> head = base.GetSendMessage();
		head.AddRange(vs);
		return head;
	}
}
