using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 * 游戏逻辑服务器
 *	控制游戏各项进程
 *		负责和数据库通信等等内容
 *		127.0.0.1 6000
 * */
namespace GameServer
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("打开系统服务器中……");

			string ip = string.Empty;
			int port = 0;
			int type = 0;
			int max = 0;
			int length = 0;
			try
			{
				string file = Environment.CurrentDirectory + "/gameserver.txt";
				FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
				StreamReader sr = new StreamReader(fs);
				ip = sr.ReadLine();
				port = int.Parse(sr.ReadLine());
				type = int.Parse(sr.ReadLine());
				max = int.Parse(sr.ReadLine());
				length = int.Parse(sr.ReadLine());
				sr.Close();
				fs.Close();
			}
			catch
			{

			}

			ServerSocket server = new ServerSocket(ip, port, (byte)type, max * 1024, length, GetSocket);
			Console.WriteLine("打开系统服务器成功");

			Thread time = new Thread(TimeSend);
			time.IsBackground = true;
			time.Start(server);
			Console.ReadLine();
		}

		/// <summary>
		/// 协议内容
		/// </summary>
		/// <param name="messageHead"></param>
		/// <returns></returns>
		static SocketMessageBase GetSocket(MessageHead messageHead)
		{
			SocketMessageBase socketMessageBase = null;

			switch (messageHead.m_MessageID)
			{
				///测试协议
				case 100010:
					socketMessageBase = new SocketMessageBase(messageHead);
					break;
			}

			return socketMessageBase;
		}

		private static void TimeSend(object server)
		{
			ServerSocket m_Server = server as ServerSocket;
			while (true)
			{
				TimeSendMessage timeSendMessage = new TimeSendMessage();
				timeSendMessage.SetTime(DateTime.Now.Ticks);
				m_Server.BroadcastMessage(timeSendMessage);
				Thread.Sleep(5 * 1000);
			}
		}
	}
}
