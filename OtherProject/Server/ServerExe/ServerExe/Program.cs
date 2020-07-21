using System;
using System.Threading;

class Program
{
	private static ServerSocket m_Server;

	public static void Main(string[] args)
	{
		Console.WriteLine("Hello World!");
		try
		{
			//ServerInfo info = new ServerInfo();
			//info.CreateServer();

			ServerSocket serverSocket = new ServerSocket("127.0.0.1", 6000, 1, 1024 * 1024, 100, GetSocket);
			Console.WriteLine("服务器开启成功:" + serverSocket);

			m_Server = serverSocket;
			Thread time = new Thread(TimeSend);
			time.IsBackground = true;
			time.Start();
		}
		catch
		{

		}

		Console.Read();
	}

	private static void TimeSend()
	{
		while (true)
		{
			TimeSendMessage timeSendMessage = new TimeSendMessage();
			timeSendMessage.SetTime(DateTime.Now.Ticks);
			m_Server.BroadcastMessage(timeSendMessage);
			Thread.Sleep(5 * 1000);
		}
	}

	/// <summary>
	/// 获取内容
	/// </summary>
	/// <param name="messageHead"></param>
	/// <returns></returns>
	public static SocketMessageBase GetSocket(MessageHead messageHead)
	{
		return new SocketMessageBase(messageHead);
	}
}
