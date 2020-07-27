using System;
using System.Collections.Generic;
using System.Text;

public class VersionDifferenceFile : SocketMessageBase
{
	private List<byte> m_SendClientDatas;

	public VersionDifferenceFile(MessageHead messageHead) : base(messageHead)
	{
		m_MessageHead.m_MessageID = 11;
		m_MessageHead.m_MessageType = 1;

		m_SendClientDatas = new List<byte>();
		m_SendClientDatas.Clear();
	}

	public override bool AnaysizeMessage(byte[] data, ClientInfo client)
	{
		if (!base.AnaysizeMessage(data, client))
			return false;

		int big = BitConverter.ToInt32(data, 0);
		int small = BitConverter.ToInt32(data, 4);
		int ob = 0, os = 0;
		VersionManager.Instance.CombineVersion(big, small, ref ob, ref os);
		m_SendClientDatas.AddRange(BitConverter.GetBytes(ob));
		m_SendClientDatas.AddRange(BitConverter.GetBytes(os));

		if (ob == os && os == 0)
		{

		}
		else
		{
			List<byte> vs;
			if (VersionManager.Instance.GetCombingVersionFiles(ob, os, out vs))
			{
				m_SendClientDatas.AddRange(vs);
			}
		}


		client.AddSendQueue(this);
		return true;
	}

	/// <summary>
	/// 获取下发内容
	/// </summary>
	/// <returns></returns>
	public override List<byte> GetSendMessage()
	{
		m_MessageHead.m_MessageLength = m_SendClientDatas.Count + 9;
		m_SendClientDatas.InsertRange(0, base.GetSendMessage());
		return m_SendClientDatas;
	}
}
