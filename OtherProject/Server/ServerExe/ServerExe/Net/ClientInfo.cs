using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// 一个客户端
/// </summary>
public class ClientInfo
{
	/// <summary>
	/// 客户端连接
	/// </summary>
	public Socket m_ClientSocket;

	/// <summary>
	/// 发送消息队列
	/// </summary>
	private Queue<SocketMessageBase> m_SendQueue;

	/// <summary>
	/// 数据长度
	/// </summary>
	private byte[] m_MessageData;

	/// <summary>
	/// 一次接收数据
	/// </summary>
	private byte[] m_OneMessageData;

	/// <summary>
	/// 开始位置
	/// </summary>
	private int m_StartPosition;

	/// <summary>
	/// 结束位置
	/// </summary>
	private int m_EndPosition;

	/// <summary>
	/// 获取对应的消息类
	/// </summary>
	public GetMessageWithHead m_GetMessageWithHead;

	public ClientInfo(Socket socket, int maxLength, GetMessageWithHead withHead)
	{
		m_SendQueue = new Queue<SocketMessageBase>();
		m_SendQueue.Clear();

		m_ClientSocket = socket;

		m_MessageData = new byte[maxLength * 3];
		m_OneMessageData = new byte[maxLength];
		m_StartPosition = m_EndPosition = 0;

		m_GetMessageWithHead = withHead;
	}

	/// <summary>
	/// 获取消息
	/// </summary>
	public void GetMessage()
	{
		int length = m_ClientSocket.Receive(m_OneMessageData);
		Array.Copy(m_OneMessageData, 0, m_MessageData, m_EndPosition, length);
		m_EndPosition += length;
		length = m_EndPosition - m_StartPosition;
		while (length >= 9)
		{
			MessageHead head = new MessageHead();
			head.m_MessageID = System.BitConverter.ToInt32(m_MessageData, m_StartPosition);
			head.m_MessageType = m_MessageData[4];
			head.m_MessageLength = BitConverter.ToInt32(m_MessageData, m_StartPosition + 5);
			Console.WriteLine("收到消息:" + head.ToString());

			if (length >= head.m_MessageLength)
			{
				if (m_GetMessageWithHead != null)
				{
					SocketMessageBase socketMessageBase = m_GetMessageWithHead(head);
					if (socketMessageBase != null)
					{
						byte[] data = new byte[head.m_MessageLength - 9];
						Array.Copy(m_MessageData, m_StartPosition + 9, data, 0, data.Length);
						socketMessageBase.AnaysizeMessage(data, this);
					}
				}

				//不管解析是否成功，起点位置均移动
				m_StartPosition += head.m_MessageLength;
				if (m_EndPosition > m_MessageData.Length * 0.7)
				{
					int leng = m_EndPosition - m_StartPosition;
					for (int index = 0; index < leng; index++)
					{
						m_MessageData[index] = m_MessageData[m_StartPosition + index];
					}

					m_StartPosition = 0;
					m_EndPosition = leng;
				}

				length = m_EndPosition - m_StartPosition;
			}
			else
			{
				break;
			}
		}
	}

	/// <summary>
	/// 添加发送队列
	/// </summary>
	/// <param name="message"></param>
	public void AddSendQueue(SocketMessageBase message)
	{
		Monitor.Enter("AddSend");
		m_SendQueue.Enqueue(message);
		Monitor.Exit("AddSend");
	}

	/// <summary>
	/// 一次性发送消息
	/// </summary>
	public void SendMessage()
	{
		Monitor.Enter("Send");

		if (m_SendQueue.Count > 0)
		{
			while (m_SendQueue.Count > 0)
			{
				SocketMessageBase socketMessageBase = m_SendQueue.Dequeue();
				List<byte> vs = socketMessageBase.GetSendMessage();
				if (m_ClientSocket.Connected)
				{
					m_ClientSocket.Send(vs.ToArray());
				}
			}
		}

		Monitor.Exit("Send");
	}
}
