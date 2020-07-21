using System;
using System.Collections.Generic;
using System.Text;

public class TimeSendMessage : SocketMessageBase
{
	private long m_SendTime;

	public TimeSendMessage() : base()
	{
		m_MessageHead = new MessageHead();
		m_MessageHead.m_MessageID = 100001;
		m_MessageHead.m_MessageType = 1;
	}

	public void SetTime(long time)
	{
		m_SendTime = time;
	}

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
