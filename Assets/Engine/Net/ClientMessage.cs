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
		/// 清除数据
		/// </summary>
		public void ClearData()
		{
			m_MessageType = 0;
			m_MessageLength = 0;
			m_MessageID = 0;
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
		/// </summary>
		/// <param name="start"></param>
		/// <param name="head"></param>
		public virtual void AnalyseMessage(int start, MessageHead head)
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
		/// 发送的数据
		/// </summary>
		public byte[] m_SendData;

		public virtual void SetSendData()
		{

		}
	}
}