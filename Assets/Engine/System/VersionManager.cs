/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:版本管理
 * Time:2020/7/27 15:08:27
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using System.IO;

namespace Game.Engine
{
	/// <summary>
	/// 差异文件详细
	/// </summary>
	public class CombineFileInfo
	{
		/// <summary>
		/// 文件夹名字
		/// </summary>
		public string m_FileName;

		/// <summary>
		/// 文件名字
		/// </summary>
		public string m_Name;

		/// <summary>
		/// 文件大小
		/// </summary>
		public int m_Length;
	}

	/// <summary>
	/// 接收版本差异
	/// </summary>
	public class VersionResponse : ClientRecvMessageBase
	{
		/// <summary>
		/// 版本号
		/// </summary>
		public int m_VersionNumber;

		/// <summary>
		/// 总长度
		/// </summary>
		public int m_AllLength;

		/// <summary>
		/// 所有文件
		/// </summary>
		public List<CombineFileInfo> m_AllFiles;

		public VersionResponse() : base()
		{
			m_AllFiles = new List<CombineFileInfo>();
			m_AllFiles.Clear();
			m_AllLength = 0;
		}

		public override void AnalyseMessage(int start, MessageHead head, GameNetClient client)
		{
			base.AnalyseMessage(start, head, client);
			m_VersionNumber = 0;
			client.GetMessageWithInt32(start, ref m_VersionNumber);

			if (m_VersionNumber == 0)
			{

			}
			else
			{
				start += 4;
				int cout = 0;
				client.GetMessageWithInt32(start, ref cout);

				m_AllLength = 0;
				for (int index = 0; index < cout; index++)
				{
					CombineFileInfo info = new CombineFileInfo();

					///文件夹名字
					start += 4;
					int length = 0;
					client.GetMessageWithInt32(start, ref length);
					start += 4;
					client.GetMessageWithString(start, length, ref info.m_FileName);

					///文件名字
					start += length;
					length = 0;
					client.GetMessageWithInt32(start, ref length);
					start += 4;
					client.GetMessageWithString(start, length, ref info.m_Name);

					///文件长度
					start += length;
					client.GetMessageWithInt32(start, ref info.m_Length);

					m_AllLength += info.m_Length;
					m_AllFiles.Add(info);
				}
			}
		}
	}

	/// <summary>
	/// 申请版本差异
	/// </summary>
	public class VersionRequest : ClientSendMessageBase
	{
		public VersionRequest() : base()
		{
			m_MessageHead.m_MessageID = EngineMessageHead.NET_CLIENT_VERSION_REQUEST;
			m_MessageHead.m_MessageType = 1;
		}

		/// <summary>
		/// 设置当前版本号
		/// </summary>
		public void SetVersion()
		{
			int version = 0;
			if (PlayerPrefs.HasKey(EngineMessageHead.CLIENT_VERSION_NUMBER))
			{
				version = PlayerPrefs.GetInt(EngineMessageHead.CLIENT_VERSION_NUMBER);
			}

			m_SendData.AddRange(BitConverter.GetBytes(version));
		}
	}

	/// <summary>
	/// 下载一个文件
	/// </summary>
	public class DownLoadFileResponse : ClientRecvMessageBase
	{
		/// <summary>
		/// 版本号
		/// </summary>
		public int m_Version;

		/// <summary>
		/// 文件内容
		/// </summary>
		public byte[] m_File;

		/// <summary>
		/// 文件详情
		/// </summary>
		public CombineFileInfo m_FineInfo;

		public DownLoadFileResponse() : base()
		{

		}

		public override void AnalyseMessage(int start, MessageHead head, GameNetClient client)
		{
			base.AnalyseMessage(start, head, client);

			int si = start;

			m_Version = 0;
			client.GetMessageWithInt32(start, ref m_Version);

			///文件夹名字
			m_FineInfo = new CombineFileInfo();
			start += 4;
			int cout = 0;
			client.GetMessageWithInt32(start, ref cout);
			start += 4;
			client.GetMessageWithString(start, cout, ref m_FineInfo.m_FileName);

			///文件名字
			start += cout;
			cout = 0;
			client.GetMessageWithInt32(start, ref cout);
			start += 4;
			client.GetMessageWithString(start, cout, ref m_FineInfo.m_Name);

			///文件大小
			start += cout;
			client.GetMessageWithInt32(start, ref m_FineInfo.m_Length);

			start += 4;
			int length = start - si;
			int leng = head.m_MessageLength - 9 - length;
			m_File = new byte[leng];
			client.GetMessageWithBytes(start, leng, ref m_File);
		}
	}

	/// <summary>
	/// 申请下载文件
	/// </summary>
	public class DownLoadFileRequest : ClientSendMessageBase
	{
		public DownLoadFileRequest() : base()
		{
			m_MessageHead.m_MessageID = EngineMessageHead.NET_CLIENT_DOWNLOAD_FILE_REQUEST;
			m_MessageHead.m_MessageType = 1;
		}

		public void SetFileName(CombineFileInfo info, int version)
		{
			m_SendData.AddRange(BitConverter.GetBytes(version));

			if (!info.m_FileName.Equals("N"))
			{
				string dir = Application.persistentDataPath + info.m_FileName;
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
			}

			List<byte> vs = new List<byte>();
			vs.Clear();
			vs.AddRange(System.Text.Encoding.Default.GetBytes(info.m_FileName));
			m_SendData.AddRange(BitConverter.GetBytes(vs.Count));
			m_SendData.AddRange(vs);
			vs.Clear();

			vs.AddRange(System.Text.Encoding.Default.GetBytes(info.m_Name));
			m_SendData.AddRange(BitConverter.GetBytes(vs.Count));
			m_SendData.AddRange(vs);
			vs.Clear();

			m_SendData.AddRange(BitConverter.GetBytes(info.m_Length));
		}
	}

	public class VersionManager : SingletonMonoClass<VersionManager>
	{
		/// <summary>
		/// 下载文件结束回调
		/// </summary>
		private Action m_DownLoadFileEnd;

		/// <summary>
		/// 服务器版本号
		/// </summary>
		private int m_VersionNumber;

		/// <summary>
		/// 需要下载的文件
		/// </summary>
		private Queue<CombineFileInfo> m_NeedDownLoadFiles;

		protected override void Awake()
		{
			base.Awake();
			m_DownLoadFileEnd = null;
			m_NeedDownLoadFiles = new Queue<CombineFileInfo>();
			m_NeedDownLoadFiles.Clear();
		}

		/// <summary>
		/// 开始版本号检查
		/// </summary>
		/// <param name="end"></param>
		public void StartVersion(Action end)
		{
			m_DownLoadFileEnd = end;
			GameNetManager.Instance.AddAgreement(EngineMessageHead.NET_CLIENT_VERSION_RESPONSE, "VersionResponse");
			GameNetManager.Instance.AddAgreement(EngineMessageHead.NET_CLIENT_DOWNLOAD_FILE_RESPONSE, "DownLoadFileResponse");
			string key = string.Format(EngineMessageHead.NET_CLIENT_MESSAGE_HEAD, EngineMessageHead.NET_CLIENT_VERSION_RESPONSE);
			MessageManger.Instance.AddMessageListener(key, this.gameObject, GetVersionFiles);

			string down = string.Format(EngineMessageHead.NET_CLIENT_MESSAGE_HEAD, EngineMessageHead.NET_CLIENT_DOWNLOAD_FILE_RESPONSE);
			MessageManger.Instance.AddMessageListener(down, this.gameObject, DownFile);

			VersionRequest version = new VersionRequest();
			version.SetVersion();
			GameNetManager.Instance.SendMessage<VersionRequest>(version, 1);
		}

		/// <summary>
		/// 下载文件
		/// </summary>
		/// <param name="arms"></param>
		private void DownFile(params object[] arms)
		{
			if (arms.Length > 0)
			{
				List<object> list = arms[0] as List<object>;
				if (list[0] is DownLoadFileResponse)
				{
					DownLoadFileResponse df = list[0] as DownLoadFileResponse;
					string path = Application.persistentDataPath;
					if (!df.m_FineInfo.m_FileName.Equals("N"))
					{
						path = path + "/" + df.m_FineInfo.m_FileName;
					}

					if (Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}

					string file = path + "/" + df.m_FineInfo.m_Name;
					if (File.Exists(file))
					{
						File.Delete(file);
					}

					FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);
					BinaryWriter binaryWriter = new BinaryWriter(fs);
					for (int index = 0; index < df.m_File.Length; index++)
					{
						binaryWriter.Write(df.m_File[index]);
					}

					binaryWriter.Close();
					fs.Close();

					///文件长度
					MessageManger.Instance.SendMessage(EngineMessageHead.DOWN_LOAD_END_FILE_LENGTH, df.m_FineInfo.m_Length);
					SendDownLoadFile();
				}
			}
		}

		/// <summary>
		/// 获取版本差异文件
		/// </summary>
		/// <param name="amrs"></param>
		private void GetVersionFiles(params object[] arms)
		{
			if (arms.Length > 0)
			{
				List<object> list = arms[0] as List<object>;
				if (list[0] is VersionResponse)
				{
					VersionResponse vr = list[0] as VersionResponse;
					m_VersionNumber = vr.m_VersionNumber;
					if (vr.m_AllLength == 0 && vr.m_AllFiles.Count == 0)
					{
						if (m_DownLoadFileEnd != null)
						{
							m_DownLoadFileEnd();
						}
					}
					else
					{
						for (int index = 0; index < vr.m_AllFiles.Count; index++)
						{
							m_NeedDownLoadFiles.Enqueue(vr.m_AllFiles[index]);
						}

						///文件大小和数量
						MessageManger.Instance.SendMessage(EngineMessageHead.DOWN_LOAD_FILE_LENGTH_AND_COUT, vr.m_AllLength, vr.m_AllFiles.Count);
						SendDownLoadFile();
					}
				}
			}
		}

		/// <summary>
		/// 申请下载文件
		/// </summary>
		private void SendDownLoadFile()
		{
			if (m_NeedDownLoadFiles.Count > 0)
			{
				CombineFileInfo info = m_NeedDownLoadFiles.Dequeue();
				DownLoadFileRequest downLoadFileRequest = new DownLoadFileRequest();
				downLoadFileRequest.SetFileName(info, m_VersionNumber);
				GameNetManager.Instance.SendMessage<DownLoadFileRequest>(downLoadFileRequest, 1);
			}
			else
			{
				if (m_DownLoadFileEnd != null)
				{
					m_DownLoadFileEnd();
				}
			}
		}
	}
}
