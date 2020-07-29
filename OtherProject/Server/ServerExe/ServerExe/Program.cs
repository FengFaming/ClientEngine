﻿using System;
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
			ServerSocket serverSocket = new ServerSocket("127.0.0.1", 6000, 1, 1024 * 1024 * 10, 100, GetSocket);
			Console.WriteLine("服务器开启成功:" + serverSocket);

			VersionManager.Instance.SetNowVersion(1);
			//m_Server = serverSocket;
			//Thread time = new Thread(TimeSend);
			//time.IsBackground = true;
			//time.Start();
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
		SocketMessageBase socketMessageBase = null;

		switch (messageHead.m_MessageID)
		{
			///版本号申请
			case 10:
				socketMessageBase = new VersionDifferenceFile(messageHead);
				break;
			///下发一个文件
			case 12:
				socketMessageBase = new DownLoadFileMessage(messageHead);
				break;
			///测试协议
			case 100010:
				socketMessageBase = new SocketMessageBase(messageHead);
				break;
			///时间比对协议
			case 100002:
				socketMessageBase = new TimeSendMessage();
				break;
		}

		return socketMessageBase;
	}
}
