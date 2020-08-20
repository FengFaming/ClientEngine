using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 数据库管理
 *	主要是控制数据库的读取和数据库的保存
 *		127.0.0.1 6002
 * */

namespace MySqlDataServer
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("打开数据库......");

			string ip = string.Empty;
			int port = 0;
			int type = 0;
			int max = 0;
			int length = 0;

			string db = string.Empty;
			try
			{
				string file = Environment.CurrentDirectory + "/mysqldataserver.txt";
				FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
				StreamReader sr = new StreamReader(fs);
				ip = sr.ReadLine();
				port = int.Parse(sr.ReadLine());
				type = int.Parse(sr.ReadLine());
				max = int.Parse(sr.ReadLine());
				length = int.Parse(sr.ReadLine());
				db = sr.ReadLine();
				sr.Close();
				fs.Close();
			}
			catch
			{

			}


			MysqlDatabaseManager sq = new MysqlDatabaseManager(db);
			object o = sq.ConnectMysql();
			if (o == null)
			{
				Console.WriteLine("数据库开启");
			}

			ServerSocket server = new ServerSocket(ip, port, (byte)type, max * 1024, length, GetSocket);
			Console.WriteLine("数据库打开成功......");
			Console.ReadLine();
		}

		/// <summary>
		/// 获取内容
		/// </summary>
		/// <param name="messageHead"></param>
		/// <returns></returns>
		public static SocketMessageBase GetSocket(MessageHead messageHead)
		{
			SocketMessageBase socketMessageBase = null;
			return socketMessageBase;
		}
	}
}
