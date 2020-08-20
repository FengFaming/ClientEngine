using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 版本文件差异
/// </summary>
public class VersionDifferenceFile : SocketMessageBase
{
	private List<byte> m_SendClientDatas;

	public VersionDifferenceFile(MessageHead messageHead) : base(messageHead)
	{
		m_MessageHead.m_MessageID = 11;
		m_MessageHead.m_MessageType = 2;

		m_SendClientDatas = new List<byte>();
		m_SendClientDatas.Clear();
	}

	public override bool AnaysizeMessage(byte[] data, ClientInfo client)
	{
		if (!base.AnaysizeMessage(data, client))
			return false;

		int cv = BitConverter.ToInt32(data, 0);
		int ccv = 0;
		bool has = VersionManager.Instance.CombineVersion(cv, ref ccv);
		m_SendClientDatas.AddRange(BitConverter.GetBytes(ccv));

		if (has)
		{

		}
		else
		{
			List<byte> vs;
			if (VersionManager.Instance.GetCombingVersionFiles(cv, out vs))
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
