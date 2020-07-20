using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerExe
{
	public class ServerInfo
	{
		private Socket m_Socket;

		public void CreateServer()
		{
			int port = 6000;
			string ips = "127.0.0.1";
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPAddress ip = IPAddress.Parse(ips);
			IPEndPoint point = new IPEndPoint(ip, port);
			socket.Bind(point);

			socket.Listen(10);
			Thread th = new Thread(Listen);
			th.IsBackground = true;
			th.Start(socket);
		}

		/// <summary>
		/// 线程接收连接
		/// </summary>
		/// <param name="o"></param>
		private void Listen(object o)
		{
			try
			{
				Socket s = o as Socket;
				while (true)
				{
					m_Socket = s.Accept();
					Console.WriteLine("连接进来了" + m_Socket.LocalEndPoint);
					if (m_Socket != null)
					{
						Thread thread = new Thread(Received);
						thread.IsBackground = true;
						thread.Start(m_Socket);
					}
				}
			}
			catch
			{

			}
		}

		/// <summary>
		/// 线程接收消息
		/// </summary>
		/// <param name="o"></param>
		private void Received(object o)
		{
			try
			{
				Socket s = o as Socket;
				while (true)
				{
					byte[] buffer = new byte[1024 * 1024 * 3];
					int leng = s.Receive(buffer);
					if (leng == 0)
					{
						break;
					}

					MessageHead head = new MessageHead();
					head.m_MessageID = System.BitConverter.ToInt32(buffer, 0);
					head.m_MessageType = buffer[4];
					head.m_MessageLength = System.BitConverter.ToInt32(buffer, 5);
					if (head.m_MessageLength != leng)
					{
						Console.WriteLine("发送错误");
					}
					else
					{
						ClientRecvMessageBase clientRecvMessageBase = new ClientRecvMessageBase();
						byte[] data = new byte[head.m_MessageLength - 9];
						Array.Copy(buffer, 9, data, 0, data.Length);
						clientRecvMessageBase.AnalyseMessage(head, data);
						Console.WriteLine(clientRecvMessageBase);
					}
				}
			}
			catch
			{

			}
		}
	}
}
