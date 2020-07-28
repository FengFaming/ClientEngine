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
	/// 接收版本差异
	/// </summary>
	public class VersionResponse : ClientRecvMessageBase
	{
		public int m_Big;
		public int m_Small;

		/// <summary>
		/// 总长度
		/// </summary>
		public int m_AllLength;

		/// <summary>
		/// 所有文件
		/// </summary>
		public List<KeyValuePair<string, int>> m_AllFiles;

		public VersionResponse() : base()
		{
			m_AllFiles = new List<KeyValuePair<string, int>>();
			m_AllFiles.Clear();
			m_AllLength = 0;
		}

		public override void AnalyseMessage(int start, MessageHead head, GameNetClient client)
		{
			base.AnalyseMessage(start, head, client);
			m_Big = 0;
			client.GetMessageWithInt32(start, ref m_Big);
			start += 4;

			m_Small = 0;
			client.GetMessageWithInt32(start, ref m_Small);

			if (m_Big == m_Small && m_Small == 0)
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
					start += 4;
					int length = 0;
					client.GetMessageWithInt32(start, ref length);

					start += 4;
					string name = "";
					client.GetMessageWithString(start, length, ref name);

					start += length;
					int size = 0;
					client.GetMessageWithInt32(start, ref size);

					m_AllLength += size;
					KeyValuePair<string, int> kv = new KeyValuePair<string, int>(name, size);
					m_AllFiles.Add(kv);
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
			int big = 0;
			int small = 0;
			if (PlayerPrefs.HasKey(EngineMessageHead.CLIENT_VERSION_BIG_VALUE))
			{
				big = PlayerPrefs.GetInt(EngineMessageHead.CLIENT_VERSION_BIG_VALUE);
			}

			if (PlayerPrefs.HasKey(EngineMessageHead.CLIENT_VERSION_SMALL_VALUE))
			{
				small = PlayerPrefs.GetInt(EngineMessageHead.CLIENT_VERSION_SMALL_VALUE);
			}

			m_SendData.AddRange(BitConverter.GetBytes(big));
			m_SendData.AddRange(BitConverter.GetBytes(small));
		}
	}

	public class DownLoadFileResponse : ClientRecvMessageBase
	{
		public int m_Big;
		public int m_Small;
		public byte[] m_File;
		public string m_FileName;

		public DownLoadFileResponse() : base()
		{

		}

		public override void AnalyseMessage(int start, MessageHead head, GameNetClient client)
		{
			base.AnalyseMessage(start, head, client);

			m_Big = 0;
			client.GetMessageWithInt32(start, ref m_Big);

			start += 4;
			m_Small = 0;
			client.GetMessageWithInt32(start, ref m_Small);

			start += 4;
			int cout = 0;
			client.GetMessageWithInt32(start, ref cout);

			start += 4;
			m_FileName = "";
			client.GetMessageWithString(start, cout, ref m_FileName);

			start = start + cout;
			int leng = head.m_MessageLength - 9 - 12 - cout;
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

		public void SetFileName(string name, int big, int small)
		{
			List<byte> vs = new List<byte>();
			vs.Clear();
			vs.AddRange(System.Text.Encoding.Default.GetBytes(name));

			m_SendData.AddRange(BitConverter.GetBytes(big));
			m_SendData.AddRange(BitConverter.GetBytes(small));
			m_SendData.AddRange(BitConverter.GetBytes(vs.Count));
			m_SendData.AddRange(vs);
		}
	}

	public class VersionManager : SingletonMonoClass<VersionManager>
	{
		/// <summary>
		/// 下载文件结束回调
		/// </summary>
		private Action m_DownLoadFileEnd;

		/// <summary>
		/// 需要下载的文件
		/// </summary>
		private Queue<KeyValuePair<string, KeyValuePair<int, int>>> m_NeedDownLoadFiles;

		protected override void Awake()
		{
			base.Awake();
			m_DownLoadFileEnd = null;
			m_NeedDownLoadFiles = new Queue<KeyValuePair<string, KeyValuePair<int, int>>>();
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
					DownLoadFileResponse downLoadFileResponse = list[0] as DownLoadFileResponse;
					string path = Application.persistentDataPath + "/AB/";
					if (Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}

					string file = path + downLoadFileResponse.m_FileName;
					if (File.Exists(file))
					{
						File.Delete(file);
					}

					FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);
					BinaryWriter binaryWriter = new BinaryWriter(fs);
					for (int index = 0; index < downLoadFileResponse.m_File.Length; index++)
					{
						binaryWriter.Write(downLoadFileResponse.m_File[index]);
					}

					binaryWriter.Close();
					fs.Close();

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
					VersionResponse getServerTimeResponse = list[0] as VersionResponse;
					if (getServerTimeResponse.m_AllLength == 0 && getServerTimeResponse.m_AllFiles.Count == 0)
					{
						if (m_DownLoadFileEnd != null)
						{
							m_DownLoadFileEnd();
						}
					}
					else
					{
						for (int index = 0; index < getServerTimeResponse.m_AllFiles.Count; index++)
						{
							KeyValuePair<int, int> keyValuePair = new KeyValuePair<int, int>(getServerTimeResponse.m_Big, getServerTimeResponse.m_Small);
							KeyValuePair<string, KeyValuePair<int, int>> kp = new KeyValuePair<string, KeyValuePair<int, int>>(getServerTimeResponse.m_AllFiles[index].Key, keyValuePair);
							m_NeedDownLoadFiles.Enqueue(kp);
						}

						SendDownLoadFile();
					}
				}
			}
		}

		private void SendDownLoadFile()
		{
			if (m_NeedDownLoadFiles.Count > 0)
			{
				KeyValuePair<string, KeyValuePair<int, int>> kp = m_NeedDownLoadFiles.Dequeue();
				DownLoadFileRequest downLoadFileRequest = new DownLoadFileRequest();
				downLoadFileRequest.SetFileName(kp.Key, kp.Value.Key, kp.Value.Value);
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
