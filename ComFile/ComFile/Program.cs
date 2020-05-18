using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ComFile
{
	class Program
	{
		static void Main(string[] args)
		{
			HashAlgorithm hash = HashAlgorithm.Create();
			Console.WriteLine(hash == null);

			string path = Environment.CurrentDirectory;
			Console.WriteLine(path);

			string f1 = "AB0";
			string f2 = "AB1";

			List<string> fs = GetAllFiles(path + "/" + f1);
			for (int index = 0; index < fs.Count; index++)
			{
				bool b = ComFileWithHash(path + "/" + f1 + "/" + fs[index],
										 path + "/" + f2 + "/" + fs[index],
										 hash);
				Console.WriteLine(b);
				Console.ReadLine();
			}

			Console.ReadLine();
		}

		/// <summary>
		/// 获取路径下所有的文件
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static List<string> GetAllFiles(string path)
		{
			DirectoryInfo fdir = new DirectoryInfo(path);
			FileInfo[] file = fdir.GetFiles();
			List<string> paths = new List<string>();
			for (int index = 0; index < file.Length; index++)
			{
				paths.Add(file[index].Name);
			}

			return paths;
		}

		/// <summary>
		/// 对比两个文件是不是一样
		/// </summary>
		/// <param name="f1"></param>
		/// <param name="f2"></param>
		/// <param name="hash"></param>
		/// <returns></returns>
		public static bool ComFileWithHash(string f1, string f2, HashAlgorithm hash)
		{
			Console.WriteLine(f1 + "\n" + f2);
			FileStream fs1 = new FileStream(f1, FileMode.Open);
			FileStream fs2 = new FileStream(f2, FileMode.Open);

			byte[] hx1 = hash.ComputeHash(fs1);
			byte[] hx2 = hash.ComputeHash(fs2);
			string hs1 = BitConverter.ToString(hx1);
			string hs2 = BitConverter.ToString(hx2);
			fs1.Close();
			fs2.Close();

			return hs1 == hs2;
		}
	}
}
