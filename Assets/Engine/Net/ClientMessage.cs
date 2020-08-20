/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:消息内容
 * Time:2020/7/16 10:34:03
* */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Engine
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
	/// 接收协议数据的基础类型
	/// </summary>
	public class ClientRecvMessageBase
	{
		/// <summary>
		/// 消息头
		/// </summary>
		protected MessageHead m_MessageHead;

		/// <summary>
		/// 解析开始位置
		///		不包含协议头的位置
		/// </summary>
		protected int m_StartPosition;

		/// <summary>
		/// 解析一个协议数据
		///		为什么要把客户端拿过来
		///			因为这样可以少开辟空间进行数据存储
		/// </summary>
		/// <param name="start">开始位置</param>
		/// <param name="head">消息头</param>
		/// <param name="client">客户端</param>
		public virtual void AnalyseMessage(int start, MessageHead head, GameNetClient client)
		{
			m_MessageHead = new MessageHead();
			m_MessageHead.m_MessageID = head.m_MessageID;
			m_MessageHead.m_MessageLength = head.m_MessageLength;
			m_MessageHead.m_MessageType = head.m_MessageType;
			m_StartPosition = start;
		}

		/// <summary>
		/// 通过消息机制返回给主进程
		/// </summary>
		public virtual void SendToMainThread()
		{
			string key = string.Format(EngineMessageHead.NET_CLIENT_MESSAGE_HEAD, m_MessageHead.m_MessageID);
			MessageManger.Instance.SendMessageThread(key, this);
		}
	}

	/// <summary>
	/// 发送消息的协议数据基础数据
	/// </summary>
	public class ClientSendMessageBase
	{
		/// <summary>
		/// 协议头
		/// </summary>
		protected MessageHead m_MessageHead;

		/// <summary>
		/// 发送的数据
		/// </summary>
		protected List<byte> m_SendData;
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

		public ClientSendMessageBase()
		{
			m_SendData = new List<byte>();
			m_SendData.Clear();

			m_MessageHead = new MessageHead();
			m_MessageHead.ClearData();
		}

		/// <summary>
		/// 数据填充完成的最后一步
		/// </summary>
		public virtual void SetOver()
		{
			List<byte> datas = new List<byte>();
			m_MessageHead.m_MessageLength = m_SendData.Count + 9;
			m_SendData.InsertRange(0, m_MessageHead.GetByteData());
		}

		/// <summary>
		/// 设置字符串
		/// </summary>
		/// <param name="data"></param>
		protected virtual void SetSendString(string data)
		{
			int cout = m_SendData.Count;
			m_SendData.AddRange(System.Text.Encoding.Default.GetBytes(data));

			int dt = m_SendData.Count - cout;
			m_SendData.InsertRange(cout, BitConverter.GetBytes(dt));
		}

		/// <summary>
		/// 设置整形数值
		/// </summary>
		/// <param name="data"></param>
		protected virtual void SetSendInt(int data)
		{
			m_SendData.AddRange(BitConverter.GetBytes(data));
		}

		/// <summary>
		/// 这是一个浮点值
		/// </summary>
		/// <param name="data"></param>
		protected virtual void SetSendFloat(float data)
		{
			m_SendData.AddRange(BitConverter.GetBytes(data));
		}

		/// <summary>
		/// 设置一个双精度值
		/// </summary>
		/// <param name="data"></param>
		protected virtual void SetSendDouble(double data)
		{
			m_SendData.AddRange(BitConverter.GetBytes(data));
		}
	}
}