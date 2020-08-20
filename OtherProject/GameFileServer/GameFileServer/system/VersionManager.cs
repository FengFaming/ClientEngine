using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// 版本管理
/// </summary>
public class VersionManager
{
	/// <summary>
	/// 文件信息
	/// </summary>
	public class VersionFileCombine
	{
		/// <summary>
		/// 文件夹名字
		/// </summary>
		public string m_WFileName;

		/// <summary>
		/// 文件名字
		/// </summary>
		public string m_FileName;

		/// <summary>
		/// 文件长度
		/// </summary>
		public int m_FileLength;

		public override bool Equals(object obj)
		{
			if (obj is VersionFileCombine)
			{
				VersionFileCombine other = obj as VersionFileCombine;
				return other.m_WFileName == m_WFileName &&
						other.m_FileName == m_FileName;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	/*
	 * 版本号组成规则
	 *	版本号由大版本号和小版本号组成
	 *		其中大版本号差异的话，那么就整体更新
	 *		小版本号差异，就部分更新
	 *		也就是说，大版本更新的时候，服务器文件夹当中是关联着全部文件的
	 *		小版本号文件夹当中只存在差异文件
	 * */

	private static VersionManager m_Version = new VersionManager();
	public static VersionManager Instance { get { return m_Version; } }

	/// <summary>
	/// 服务器版本号
	/// </summary>
	private int m_NowVersionNumber;

	public VersionManager()
	{
		m_NowVersionNumber = 0;
	}

	/// <summary>
	/// 设置版本号
	///		V0
	/// </summary>
	/// <param name="v1"></param>
	public void SetNowVersion(int v1)
	{
		m_NowVersionNumber = v1;
	}

	/// <summary>
	/// 对比客户端上传的版本号和当前的版本号
	/// </summary>
	/// <param name="version"></param>
	/// <param name="combine"></param>
	public bool CombineVersion(int version, ref int combine)
	{
		combine = m_NowVersionNumber;

		if (version == m_NowVersionNumber)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// 获取差异文件
	/// </summary>
	/// <param name="version">客户端上传的版本号</param>
	/// <param name="vs"></param>
	/// <returns></returns>
	public bool GetCombingVersionFiles(int version, out List<byte> vs)
	{
		vs = new List<byte>();
		vs.Clear();

		string path = Environment.CurrentDirectory;
		path = Path.Combine(path, "Data");
		if (!Directory.Exists(path))
		{
			Console.WriteLine("the Data is null.");
			return false;
		}

		List<VersionFileCombine> vfc = new List<VersionFileCombine>();
		vfc.Clear();
		for (int index = m_NowVersionNumber; index > version; index--)
		{
			string file = Path.Combine(path, "Hash" + index);
			if (!File.Exists(file))
			{
				Console.WriteLine("the file:" + file + " is null.");
				return false;
			}

			FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
			BinaryReader br = new BinaryReader(fs);
			int cout = 0;
			cout = br.ReadInt32();

			for (int i = 0; i < cout; i++)
			{
				VersionFileCombine v = new VersionFileCombine();
				v.m_WFileName = br.ReadString();
				v.m_FileName = br.ReadString();
				v.m_FileLength = br.ReadInt32();

				if (!vfc.Contains(v))
				{
					vfc.Add(v);
				}
			}
		}

		vs.AddRange(BitConverter.GetBytes(vfc.Count));
		List<byte> fn = new List<byte>();
		for (int index = 0; index < vfc.Count; index++)
		{
			fn.Clear();
			fn.AddRange(System.Text.Encoding.Default.GetBytes(vfc[index].m_WFileName));
			int length = fn.Count;
			vs.AddRange(BitConverter.GetBytes(length));
			vs.AddRange(fn);

			fn.Clear();
			fn.AddRange(System.Text.Encoding.Default.GetBytes(vfc[index].m_FileName));
			length = fn.Count;
			vs.AddRange(BitConverter.GetBytes(length));
			vs.AddRange(fn);

			vs.AddRange(BitConverter.GetBytes(vfc[index].m_FileLength));
		}

		return true;
	}


	/// <summary>
	/// 获取文件
	/// </summary>
	/// <param name="file"></param>
	/// <returns></returns>
	public byte[] GetFile(VersionFileCombine file)
	{
		string path = Environment.CurrentDirectory + "/Data";
		if (!file.m_WFileName.Equals("N"))
		{
			path = path + file.m_WFileName;
		}

		path = path + "/" + file.m_FileName;
		if (!File.Exists(path))
		{
			Console.WriteLine("错误，没有该文件" + path);
			return null;
		}

		FileInfo fileInfo = new FileInfo(path);
		byte[] vs = new byte[fileInfo.Length];
		FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
		BinaryReader br = new BinaryReader(fs);
		for (int index = 0; index < vs.Length; index++)
		{
			vs[index] = br.ReadByte();
		}

		br.Close();
		fs.Close();
		return vs;
	}
}