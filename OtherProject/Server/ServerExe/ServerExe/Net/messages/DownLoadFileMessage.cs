using System;
using System.Collections.Generic;
using System.Text;

public class DownLoadFileMessage : SocketMessageBase
{
	private List<byte> m_SendClientDatas;

	public DownLoadFileMessage(MessageHead head) : base(head)
	{
		m_MessageHead.m_MessageID = 13;
		m_MessageHead.m_MessageType = 1;

		m_SendClientDatas = new List<byte>();
		m_SendClientDatas.Clear();
	}

	public override bool AnaysizeMessage(byte[] data, ClientInfo client)
	{
		if (!base.AnaysizeMessage(data, client))
			return false;

		int start = 0;
		int big = BitConverter.ToInt32(data, start);

		start += 4;
		int small = BitConverter.ToInt32(data, start);

		start += 4;
		int cout = BitConverter.ToInt32(data, start);

		start += 4;
		string name = System.Text.Encoding.Default.GetString(data, start, cout);
		m_SendClientDatas.AddRange(data);
		m_SendClientDatas.AddRange(VersionManager.Instance.GetFile(name, big, small));

		client.AddSendQueue(this);
		return true;
	}

	public override List<byte> GetSendMessage()
	{
		m_MessageHead.m_MessageLength = m_SendClientDatas.Count + 9;
		m_SendClientDatas.InsertRange(0, base.GetSendMessage());
		return m_SendClientDatas;
	}
}
