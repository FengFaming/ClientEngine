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

			string f1 = "Now";
			string f2 = "Last";

			List<string> fs = GetAllFiles(path + "/" + f1);
			List<string> combineFiles = new List<string>();
			for (int index = 0; index < fs.Count; index++)
			{
				if (fs[index].Equals("Hash"))
				{
					break;
				}

				string p = path + "/" + f2 + "/" + fs[index];
				if (File.Exists(p))
				{
					bool b = ComFileWithHash(path + "/" + f1 + "/" + fs[index],
											 path + "/" + f2 + "/" + fs[index],
											 hash);

					if (!b)
					{
						combineFiles.Add(fs[index]);
					}
				}
				else
				{
					combineFiles.Add(fs[index]);
				}
			}

			string dic = path + "/Combine/";
			if (Directory.Exists(dic))
			{
				Directory.Delete(dic, true);
			}

			Directory.CreateDirectory(dic);
			string save = path + "/Combine/" + "Hash";
			if (File.Exists(save))
			{
				File.Delete(save);
			}

			FileStream fsl = new FileStream(save, FileMode.Create, FileAccess.Write);
			BinaryWriter bw = new BinaryWriter(fsl);
			bw.Write(combineFiles.Count);
			for (int index = 0; index < combineFiles.Count; index++)
			{
				bw.Write(combineFiles[index]);
				string filePath = path + "/" + f1 + "/" + combineFiles[index];
				FileInfo fileInfo = new FileInfo(filePath);
				int length = (int)fileInfo.Length;
				bw.Write(length);
				string newPath = dic + combineFiles[index];
				File.Copy(filePath, newPath);
				Console.WriteLine("Combine File:" + combineFiles[index] + " " + length);
			}

			bw.Close();
			fsl.Close();

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
