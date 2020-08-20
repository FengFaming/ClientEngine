using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 文件管理
 *	主要作用是启动连接，接受客户端的文件下载
 *		127.0.0.1 6001
 * */

namespace GameFileServer
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("打开文件服务器......");

			string ip = string.Empty;
			int port = 0;
			int type = 0;
			int max = 0;
			int length = 0;
			int version = 0;
			try
			{
				string file = Environment.CurrentDirectory + "/gamefileserver.txt";
				FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
				StreamReader sr = new StreamReader(fs);
				ip = sr.ReadLine();
				port = int.Parse(sr.ReadLine());
				type = int.Parse(sr.ReadLine());
				max = int.Parse(sr.ReadLine());
				length = int.Parse(sr.ReadLine());
				version = int.Parse(sr.ReadLine());
				sr.Close();
				fs.Close();
			}
			catch
			{

			}

			ServerSocket server = new ServerSocket(ip, port, (byte)type, max * 1024, length, GetSocket);
			VersionManager.Instance.SetNowVersion(version);
			Console.WriteLine("文件服务器启动成功......");
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
				///版本号申请
				case 10:
					socketMessageBase = new VersionDifferenceFile(messageHead);
					break;
				///下发一个文件
				case 12:
					socketMessageBase = new DownLoadFileMessage(messageHead);
					break;
			}

			return socketMessageBase;
		}
	}
}
