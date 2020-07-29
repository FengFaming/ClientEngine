using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ComFile
{
	public class CombineFileInfo
	{
		public string m_WFileName;
		public string m_FileName;
		public int m_Length;
	}

	class Program
	{
		static void Main(string[] args)
		{
			HashAlgorithm hash = HashAlgorithm.Create();
			Console.WriteLine(hash == null);

			string path = Environment.CurrentDirectory;
			Console.WriteLine(path);

			List<CombineFileInfo> combineFiles = new List<CombineFileInfo>();
			combineFiles.Clear();
			CombineDirectoryFile(path + "/Now", ref combineFiles, hash);

			DirectoryInfo info = new DirectoryInfo(path + "/Now");
			DirectoryInfo[] directoryInfos = info.GetDirectories();
			foreach (DirectoryInfo wenjianjia in directoryInfos)
			{
				string wpath = wenjianjia.FullName;
				wpath = wpath.Replace("\\", "/");
				CombineDirectoryFile(wpath, ref combineFiles, hash);
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
				bw.Write(combineFiles[index].m_WFileName);
				bw.Write(combineFiles[index].m_FileName);
				bw.Write(combineFiles[index].m_Length);
				Console.WriteLine("Combine File:" + combineFiles[index].m_WFileName + " " +
													combineFiles[index].m_FileName + " " +
													combineFiles[index].m_Length);
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
		/// 对比文件夹下面的内容
		/// </summary>
		/// <param name="path"></param>
		/// <param name="files"></param>
		private static void CombineDirectoryFile(string path, ref List<CombineFileInfo> files, HashAlgorithm hash)
		{
			string wenjianjia = path.Substring(path.LastIndexOf("/"));
			string nw;
			bool isN = true;
			if (wenjianjia.Equals("/Now"))
			{
				nw = path.Substring(0, path.LastIndexOf("/") + 1) + "Last";
			}
			else
			{
				isN = false;
				nw = path.Substring(0, path.LastIndexOf("/"));
				nw = nw.Substring(0, nw.LastIndexOf("/")) + "/Last/" + wenjianjia;
			}

			List<string> vs = GetAllFiles(path);
			for (int index = 0; index < vs.Count; index++)
			{
				if (vs[index].Equals("Hash"))
				{
					break;
				}

				string lastp = nw + "/" + vs[index];
				if (File.Exists(lastp))
				{
					bool b = ComFileWithHash(path + "/" + vs[index],
											   nw + "/" + vs[index],
											   hash);

					if (!b)
					{
						CombineFileInfo info = new CombineFileInfo();
						info.m_WFileName = isN ? "N" : wenjianjia;
						info.m_FileName = vs[index];
						string filePath = path + "/" + vs[index];
						FileInfo fileInfo = new FileInfo(filePath);
						int length = (int)fileInfo.Length;
						info.m_Length = length;
						files.Add(info);
					}
				}
				else
				{
					CombineFileInfo info = new CombineFileInfo();
					info.m_WFileName = isN ? "N" : wenjianjia;
					info.m_FileName = vs[index];
					string filePath = path + "/" + vs[index];
					FileInfo fileInfo = new FileInfo(filePath);
					int length = (int)fileInfo.Length;
					info.m_Length = length;
					files.Add(info);
				}
			}
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
