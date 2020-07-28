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
		/// 文件名字
		/// </summary>
		public string m_FileName;

		/// <summary>
		/// 文件长度
		/// </summary>
		public int m_FileLength;
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
	/// 大版本号
	/// </summary>
	private int m_NowVersionWithBig;

	/// <summary>
	/// 小版本号
	/// </summary>
	private int m_NowVersionWithSmall;

	public VersionManager()
	{
		m_NowVersionWithBig = 0;
		m_NowVersionWithSmall = 0;
	}

	/// <summary>
	/// 设置版本号
	///		V0.0
	/// </summary>
	/// <param name="v1"></param>
	/// <param name="v2"></param> 
	public void SetNowVersion(int v1, int v2)
	{
		m_NowVersionWithBig = v1;
		m_NowVersionWithSmall = v2;
	}

	/// <summary>
	/// 对比两个版本号
	/// </summary>
	/// <param name="big"></param>
	/// <param name="small"></param>
	/// <param name="combineBig"></param>
	/// <param name="combingSmall"></param>
	public void CombineVersion(int big, int small, ref int combineBig, ref int combingSmall)
	{
		combineBig = m_NowVersionWithBig - big;
		combingSmall = m_NowVersionWithSmall - small;
		if (combineBig != 0)
		{
			combineBig = big + 1;
			combingSmall = 0;
		}
		else if (combingSmall != 0)
		{
			combineBig = big;
			combingSmall = small + 1;
		}
		else
		{
			combineBig = 0;
			combingSmall = 0;
		}
	}

	/// <summary>
	/// 获取所有差异文件的名字
	/// </summary>
	/// <param name="big"></param>
	/// <param name="small"></param>
	/// <returns></returns>
	public bool GetCombingVersionFiles(int big, int small, out List<byte> vs)
	{
		vs = new List<byte>();
		vs.Clear();

		string path = Environment.CurrentDirectory;
		path = Path.Combine(path, "V" + big + "." + small);
		if (!Directory.Exists(path))
		{
			Console.WriteLine("the version:V" + big + "." + small + " is null.");
			return false;
		}

		string file = Path.Combine(path, "Hash");
		if (!File.Exists(file))
		{
			Console.WriteLine("the file:" + file + " is null.");
			return false;
		}

		List<VersionFileCombine> vfc = new List<VersionFileCombine>();
		vfc.Clear();
		int cout = 0;
		FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
		BinaryReader br = new BinaryReader(fs);
		cout = br.ReadInt32();
		for (int index = 0; index < cout; index++)
		{
			VersionFileCombine v = new VersionFileCombine();
			v.m_FileName = br.ReadString();
			v.m_FileLength = br.ReadInt32();
			vfc.Add(v);
		}

		br.Close();
		fs.Close();

		vs.AddRange(BitConverter.GetBytes(vfc.Count));
		List<byte> fn = new List<byte>();
		for (int index = 0; index < vfc.Count; index++)
		{
			fn.Clear();
			fn.AddRange(System.Text.Encoding.Default.GetBytes(vfc[index].m_FileName));
			int length = fn.Count;
			vs.AddRange(BitConverter.GetBytes(length));
			vs.AddRange(fn);
			vs.AddRange(BitConverter.GetBytes(vfc[index].m_FileLength));
		}

		return true;
	}

	/// <summary>
	/// 获取文件
	/// </summary>
	/// <param name="name"></param>
	/// <param name="big"></param>
	/// <param name="small"></param>
	/// <returns></returns>
	public byte[] GetFile(string name, int big, int small)
	{
		string path = Environment.CurrentDirectory;
		path = Path.Combine(path, "V" + big + "." + small);
		path = path + "/" + name;
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