using System;

namespace ServerExe
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			try
			{
				ServerInfo info = new ServerInfo();
				info.CreateServer();
				Console.Read();
			}
			catch
			{

			}
		}


	}
}
