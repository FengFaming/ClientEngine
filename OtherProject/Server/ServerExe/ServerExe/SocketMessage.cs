using System;
using System.Collections.Generic;
using System.Text;

namespace ServerExe
{
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
			datas.AddRange(BitConverter.GetBytes(m_MessageType));
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
	/// 接收协议数据的基础类型
	/// </summary>
	public class ClientRecvMessageBase
	{
		/// <summary>
		/// 消息头
		/// </summary>
		protected MessageHead m_MessageHead;

		public ClientRecvMessageBase()
		{
			m_MessageHead = new MessageHead();
			m_MessageHead.ClearData();
		}

		/// <summary>
		/// 解析数据
		/// </summary>
		/// <param name="head">头</param>
		/// <param name="data">剩余数据</param>
		public virtual void AnalyseMessage(MessageHead head, byte[] data)
		{
			m_MessageHead.m_MessageType = head.m_MessageType;
			m_MessageHead.m_MessageID = head.m_MessageID;
			m_MessageHead.m_MessageLength = head.m_MessageLength;
		}

		public override string ToString()
		{
			return m_MessageHead.ToString();
		}
	}

	/// <summary>
	/// 发送消息的协议数据基础数据
	/// </summary>
	public class ClientSendMessageBase
	{
		/// <summary>
		/// 消息对应客户端
		/// </summary>
		protected ClientInfo m_ClientInfo;

		/// <summary>
		/// 协议头
		/// </summary>
		protected MessageHead m_MessageHead;

		/// <summary>
		/// 发送的数据
		/// </summary>
		private List<byte> m_SendData;
		public virtual byte[] SendData
		{
			get
			{
				if (m_SendData != null && m_SendData.Count > 0)
				{
					return m_SendData.ToArray();
				}

				return null;
			}
		}

		/// <summary>
		/// 设置客户端数据
		/// </summary>
		/// <param name="data"></param>
		/// <param name="length"></param>
		public virtual void SetRecvMessage(byte[] data, int length)
		{

		}

		public ClientSendMessageBase(ClientInfo clientInfo)
		{
			m_ClientInfo = clientInfo;
			m_SendData = new List<byte>();
			m_SendData.Clear();

			m_MessageHead = new MessageHead();
			m_MessageHead.ClearData();
		}

		/// <summary>
		/// 数据填充完成的最后一步
		/// </summary>
		protected virtual void SetOver()
		{
			List<byte> datas = new List<byte>();
			m_MessageHead.m_MessageLength = m_SendData.Count + 9;
			m_SendData.InsertRange(0, m_MessageHead.GetByteData());
		}

		/// <summary>
		/// 发送消息
		/// </summary>
		public virtual void Send()
		{
			if (m_ClientInfo != null)
			{
				if (m_ClientInfo.m_IsSuccess)
				{
					SetOver();
					m_ClientInfo.Send(SendData);
				}
			}
		}
	}
}
