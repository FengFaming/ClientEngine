using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 消息头
/// </summary>
public class MessageHead
{
	/// <summary>
	/// 消息类型 1
	/// </summary>
	public byte m_MessageType;

	/// <summary>
	/// 消息协议号 4
	/// </summary>
	public int m_MessageID;

	/// <summary>
	/// 消息长度，带上消息头的长度 4
	/// </summary>
	public int m_MessageLength;

	public MessageHead()
	{
		m_MessageType = 0;
		m_MessageLength = 0;
		m_MessageID = 0;
	}

	/// <summary>
	/// 获取协议头数据
	/// </summary>
	/// <returns></returns>
	public List<byte> GetByteData()
	{
		List<byte> datas = new List<byte>();
		datas.AddRange(BitConverter.GetBytes(m_MessageID));
		datas.Add(m_MessageType);
		datas.AddRange(BitConverter.GetBytes(m_MessageLength));
		return datas;
	}

	/// <summary>
	/// 清除数据
	/// </summary>
	public void ClearData()
	{
		m_MessageType = 0;
		m_MessageLength = 0;
		m_MessageID = 0;
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("ID:{0};Type:{1};Length:{2}", m_MessageID, m_MessageType, m_MessageLength);
	}
}

/// <summary>
/// 消息基类
/// </summary>
public class SocketMessageBase
{
	/// <summary>
	/// 消息头
	/// </summary>
	protected MessageHead m_MessageHead;

	protected SocketMessageBase()
	{

	}

	public SocketMessageBase(MessageHead head)
	{
		m_MessageHead = new MessageHead();
		m_MessageHead.m_MessageType = head.m_MessageType;
		m_MessageHead.m_MessageID = head.m_MessageID;
		m_MessageHead.m_MessageLength = head.m_MessageLength;
	}

	/// <summary>
	/// 分析数据
	/// </summary>
	/// <param name="data">其中不包含数据协议头</param>
	/// <returns></returns>
	public virtual bool AnaysizeMessage(byte[] data, ClientInfo client)
	{
		return true;
	}

	/// <summary>
	/// 获取发送消息的内容
	/// </summary>
	/// <returns></returns>
	public virtual List<byte> GetSendMessage()
	{
		List<byte> vs = new List<byte>();
		vs.Clear();
		vs.AddRange(m_MessageHead.GetByteData());
		return vs;
	}
}
