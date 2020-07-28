/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:游戏客户端
 * Time:2020/7/16 8:53:09
* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Game.Engine
{
	/// <summary>
	/// 服务器ID
	/// </summary>
	public class ServerID
	{
		/// <summary>
		/// IP地址:192.168.0.1
		/// </summary>
		public string m_Host;

		/// <summary>
		/// 端口号:80
		/// </summary>
		public int m_Port;
	}

	/// <summary>
	/// 协议客户端详细
	/// </summary>
	public class ClientNetInfo
	{
		/// <summary>
		/// 服务器
		/// </summary>
		public ServerID m_ServerID;

		/// <summary>
		/// 链接
		/// </summary>
		public Socket m_Socket;

		public ClientNetInfo(ServerID id)
		{
			m_ServerID = id;
			m_Socket = null;
		}

		/// <summary>
		/// 启动连接
		/// </summary>
		public void ConnectSocket()
		{
			IPAddress address = IPAddress.Parse(m_ServerID.m_Host);
			EndPoint point = new IPEndPoint(address, m_ServerID.m_Port);
			m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			m_Socket.NoDelay = true;
			m_Socket.ReceiveTimeout = Timeout.Infinite;
			m_Socket.SendTimeout = Timeout.Infinite;
			m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
			m_Socket.Connect(point);
		}

		public void CloseSocket()
		{
			if (m_Socket != null)
			{
				try
				{
					m_Socket.Close();
				}
				catch (Exception e)
				{
					Debug.Log("socket close exception:" + e);
				}
			}

			m_Socket = null;
		}
	}

	/// <summary>
	/// 游戏协议客户端
	/// </summary>
	public class GameNetClient
	{
		/// <summary>
		/// 客户端唯一标志
		/// </summary>
		private ClientNetInfo m_ClientInfo;

		/// <summary>
		/// 是否连接成功
		/// </summary>
		private bool m_IsSuccess;

		/// <summary>
		/// 线程ID号
		/// </summary>
		private int m_ThreadID;

		/// <summary>
		/// 一次接收协议数据
		/// </summary>
		private byte[] m_OneMessage;

		/// <summary>
		/// 缓冲数据
		/// </summary>
		private byte[] m_CarshMessage;

		/// <summary>
		/// 一个消息的最大长度
		/// </summary>
		private int m_OneMaxMessageLength;

		/// <summary>
		/// 解析开头
		/// </summary>
		private int m_StartPoint;

		/// <summary>
		/// 解析结束
		/// </summary>
		private int m_EndPoint;

		/// <summary>
		/// 解析的协议头
		/// </summary>
		private MessageHead m_CarshHead;

		/// <summary>
		/// 创建一个客户端
		/// </summary>
		/// <param name="ip"></param>
		/// <param name="port"></param>
		/// <param name="maxLength">一条消息最大是多少</param>
		public GameNetClient(string ip, int port, int maxLength)
		{
			ServerID id = new ServerID();
			id.m_Host = ip;
			id.m_Port = port;
			m_ClientInfo = new ClientNetInfo(id);
			m_IsSuccess = false;
			m_ThreadID = -1;

			m_OneMaxMessageLength = maxLength;
			m_OneMessage = new byte[m_OneMaxMessageLength];
			m_CarshMessage = new byte[m_OneMaxMessageLength * 3];
			m_StartPoint = m_EndPoint = 0;
		}

		/// <summary>
		/// 连接服务器
		/// </summary>
		public void ConnectSocket(Action<bool> action)
		{
			try
			{
				m_ClientInfo.ConnectSocket();
				m_IsSuccess = true;
				m_ThreadID = GameThreadManager.Instance.CreateThread(RecvMessage, Close);
				if (action != null)
				{
					action(true);
				}
			}
			catch (Exception e)
			{
				if (action != null)
				{
					action(false);
				}

				Debug.LogError("connect socket:" + e);
			}
		}

		/// <summary>
		/// 关闭连接
		/// </summary>
		public void Close()
		{
			m_IsSuccess = false;

			//先停止线程
			if (m_ThreadID > 0)
			{
				GameThreadManager.Instance.CloseOne(m_ThreadID);
			}

			if (m_ClientInfo != null)
			{
				m_ClientInfo.CloseSocket();
			}

			m_OneMessage = null;
			m_CarshMessage = null;
			m_StartPoint = m_EndPoint = 0;
		}

		/// <summary>
		/// 发送消息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pack"></param>
		public void SendMessage<T>(T pack) where T : ClientSendMessageBase
		{
			if (m_ClientInfo.m_Socket != null && m_IsSuccess)
			{
				pack.SetOver();
				m_ClientInfo.m_Socket.Send(pack.SendData);
			}
		}

		#region 接收数据
		/// <summary>
		/// 检查缓冲数据
		/// </summary>
		private void JCCarsh()
		{
			if (m_EndPoint > m_OneMaxMessageLength * 2 - 1)
			{
				int length = m_EndPoint - m_StartPoint;
				for (int index = 0; index <= length; index++)
				{
					m_CarshMessage[index] = m_CarshMessage[m_StartPoint + index];
				}

				m_StartPoint = 0;
				m_EndPoint = length;
			}
		}

		/// <summary>
		/// 解析数据
		/// </summary>
		private void FXCarsh()
		{
			int leng = m_EndPoint - m_StartPoint;
			if (leng >= 9)
			{
				if (m_CarshHead == null)
				{
					m_CarshHead = new MessageHead();
				}

				m_CarshHead.ClearData();
				m_CarshHead.m_MessageID = System.BitConverter.ToInt32(m_CarshMessage, m_StartPoint);
				m_CarshHead.m_MessageType = m_CarshMessage[m_StartPoint + 4];
				m_CarshHead.m_MessageLength = System.BitConverter.ToInt32(m_CarshMessage, m_StartPoint + 5);
				if (leng >= m_CarshHead.m_MessageLength &&
					m_CarshHead.m_MessageID > 0 &&
					m_CarshHead.m_MessageLength >= 9)
				{
					int start = m_StartPoint + 9;
					m_StartPoint = m_StartPoint + m_CarshHead.m_MessageLength;
					string control = GameNetManager.Instance.GetAgreement(m_CarshHead.m_MessageID);
					if (control != null)
					{
						ClientRecvMessageBase ms = ReflexManager.Instance.CreateClass(control) as ClientRecvMessageBase;
						if (ms != null)
						{
							ms.AnalyseMessage(start, m_CarshHead, this);
							ms.SendToMainThread();
						}
					}
				}
			}
		}

		/// <summary>
		/// 线程接收消息
		/// </summary>
		private void RecvMessage()
		{
			while (m_IsSuccess)
			{
				try
				{
					JCCarsh();
					int leng = m_ClientInfo.m_Socket.Receive(m_OneMessage);
					if (leng <= 0)
					{
						continue;
					}
					else
					{
						for (int index = 0; index < leng; index++)
						{
							m_CarshMessage[m_EndPoint++] = m_OneMessage[index];
						}
					}

					FXCarsh();
				}
				catch (Exception e)
				{
					m_IsSuccess = false;
					Debug.LogWarning(e);
				}
			}
		}
		#endregion

		#region 放开给外部解析的方法
		/// <summary>
		/// 获取一个字节
		/// </summary>
		/// <param name="start"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool GetMessageWithByte(int start, ref byte data)
		{
			if (start > m_StartPoint)
			{
				return false;
			}

			data = m_CarshMessage[start];
			return true;
		}

		/// <summary>
		/// 获取一个短整型的数值
		/// </summary>
		/// <param name="start"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool GetMessageWithInt16(int start, ref int data)
		{
			if (start + 1 > m_StartPoint)
			{
				return false;
			}

			data = System.BitConverter.ToInt16(m_CarshMessage, start);
			return true;
		}

		/// <summary>
		/// 解析一个整形数值
		/// </summary>
		/// <param name="start"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool GetMessageWithInt32(int start, ref int data)
		{
			if (start + 3 > m_StartPoint)
			{
				return false;
			}

			data = System.BitConverter.ToInt32(m_CarshMessage, start);
			return true;
		}

		/// <summary>
		/// 获取一个单精度的数值
		/// </summary>
		/// <param name="start"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool GetMessageWithFloat(int start, ref float data)
		{
			if (start + 3 > m_StartPoint)
			{
				return false;
			}

			data = System.BitConverter.ToSingle(m_CarshMessage, start);
			return true;
		}

		/// <summary>
		/// 解析一个双精度数值
		/// </summary>
		/// <param name="start"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool GetMessageWithDouble(int start, ref double data)
		{
			if (start + 7 > m_StartPoint)
			{
				return false;
			}

			data = System.BitConverter.ToDouble(m_CarshMessage, start);
			return true;
		}

		/// <summary>
		/// 获取一个长整型
		/// </summary>
		/// <param name="start"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool GetMessageWithLong(int start, ref long data)
		{
			if (start + 7 > m_StartPoint)
			{
				return false;
			}

			data = System.BitConverter.ToInt64(m_CarshMessage, start);
			return true;
		}

		/// <summary>
		/// 获取一个字符串
		/// </summary>
		/// <param name="start"></param>
		/// <param name="leng"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool GetMessageWithString(int start, int leng, ref string data)
		{
			if (start + leng > m_StartPoint)
			{
				return false;
			}

			data = System.Text.Encoding.UTF8.GetString(m_CarshMessage, start, leng);
			return true;
		}

		/// <summary>
		/// 获取所有的字节
		/// </summary>
		/// <param name="start"></param>
		/// <param name="leng"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool GetMessageWithBytes(int start, int leng, ref byte[] data)
		{
			if (start + leng > m_StartPoint)
			{
				return false;
			}

			Array.Copy(m_CarshMessage, start, data, 0, leng);
			return true;
		}
		#endregion
	}
}
