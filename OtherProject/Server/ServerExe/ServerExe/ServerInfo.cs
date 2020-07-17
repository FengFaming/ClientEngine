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

					string str = Encoding.UTF8.GetString(buffer, 0, leng);
					Console.WriteLine(str);
				}
			}
			catch
			{

			}
		}
	}
}
