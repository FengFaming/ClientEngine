using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// 获取对应消息类的回调头
/// </summary>
/// <param name="head"></param>
/// <returns></returns>
public delegate SocketMessageBase GetMessageWithHead(MessageHead head);

/// <summary>
/// 服务器连接
/// </summary>
public class ServerSocket
{
	/// <summary>
	/// 服务器类型
	///		唯一标识一个服务器连接
	/// </summary>
	private byte m_ServerType;

	/// <summary>
	/// 所有客户端连接
	/// </summary>
	private List<ClientInfo> m_AllClientInfos;

	/// <summary>
	/// 获取客户端的线程
	/// </summary>
	private Thread m_GetClient;

	/// <summary>
	/// 获取客户端消息的线程
	/// </summary>
	private Thread m_GetClientMessage;

	/// <summary>
	/// 发送客户端消息的线程
	/// </summary>
	private Thread m_SendClientMessage;

	/// <summary>
	/// 服务器连接器
	/// </summary>
	private Socket m_ServerSocket;

	/// <summary>
	/// 数据的长度
	/// </summary>
	private int m_MaxLength;

	/// <summary>
	/// 获取对应的消息类
	/// </summary>
	public GetMessageWithHead m_GetMessageWithHead;

	public ServerSocket(string ip, int port, byte type, int length, int maxListen, GetMessageWithHead getMessageWithHead)
	{
		m_ServerType = type;
		m_AllClientInfos = new List<ClientInfo>();
		m_AllClientInfos.Clear();
		m_GetMessageWithHead = getMessageWithHead;
		m_MaxLength = length;

		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		IPAddress Ip = IPAddress.Parse(ip);
		IPEndPoint point = new IPEndPoint(Ip, port);
		socket.Bind(point);
		m_ServerSocket = socket;
		m_ServerSocket.Listen(maxListen);

		m_GetClient = new Thread(GetClient);
		m_GetClient.IsBackground = true;
		m_GetClient.Start();

		m_GetClientMessage = new Thread(GetClientMessage);
		m_GetClientMessage.IsBackground = true;

		m_SendClientMessage = new Thread(SendClientMessage);
		m_SendClientMessage.IsBackground = true;

		m_GetClientMessage.Start();
		m_SendClientMessage.Start();
	}

	/// <summary>
	/// 广播一条消息
	/// </summary>
	/// <param name="socketMessageBase"></param>
	public void BroadcastMessage(SocketMessageBase socketMessageBase)
	{
		Monitor.Enter("Broadcast");
		for (int index = 0; index < m_AllClientInfos.Count;)
		{
			if (m_AllClientInfos[index].m_ClientSocket.Connected)
			{
				m_AllClientInfos[index].AddSendQueue(socketMessageBase);
				index++;
			}
			else
			{
				m_AllClientInfos.RemoveAt(index);
			}
		}
		Monitor.Exit("Broadcast");
	}

	/// <summary>
	/// 获取客户端
	/// </summary>
	private void GetClient()
	{
		while (true)
		{
			try
			{
				Socket socket = m_ServerSocket.Accept();
				Console.WriteLine("一个连接进来：" + socket.LocalEndPoint);
				ClientInfo info = new ClientInfo(socket, m_MaxLength, m_GetMessageWithHead);
				Monitor.Enter("Get");
				m_AllClientInfos.Add(info);
				Monitor.Exit("Get");
			}
			catch
			{

			}

			Thread.Sleep(10);
		}
	}

	/// <summary>
	/// 获取客户端消息
	/// </summary>
	private void GetClientMessage()
	{
		while (true)
		{
			try
			{
				Monitor.Enter("Message");
				if (m_AllClientInfos.Count > 0)
				{
					for (int index = 0; index < m_AllClientInfos.Count;)
					{
						ClientInfo info = m_AllClientInfos[index];
						if (info.m_ClientSocket.Connected)
						{
							info.GetMessage();
							index++;
						}
						else
						{
							m_AllClientInfos.RemoveAt(index);
						}
					}
				}
				Monitor.Exit("Message");
			}
			catch
			{

			}

			Thread.Sleep(10);
		}
	}

	/// <summary>
	/// 发送客户端消息
	/// </summary>
	private void SendClientMessage()
	{
		while (true)
		{
			try
			{
				Monitor.Enter("Send");
				if (m_AllClientInfos.Count > 0)
				{
					for (int index = 0; index < m_AllClientInfos.Count;)
					{
						ClientInfo info = m_AllClientInfos[index];
						if (info.m_ClientSocket.Connected)
						{
							info.SendMessage();
							index++;
						}
						else
						{
							m_AllClientInfos.RemoveAt(index);
						}
					}
				}
				Monitor.Exit("Send");
			}
			catch
			{

			}

			Thread.Sleep(10);
		}
	}
}
