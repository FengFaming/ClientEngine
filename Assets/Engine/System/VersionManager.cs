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

namespace Game.Engine
{
	/// <summary>
	/// 接收版本差异
	/// </summary>
	public class VersionResponse : ClientRecvMessageBase
	{
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
			int big = 0;
			client.GetMessageWithInt32(start, ref big);
			start += 4;

			int small = 0;
			client.GetMessageWithInt32(start, ref small);

			if (big == small && small == 0)
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

	public class VersionManager : SingletonMonoClass<VersionManager>
	{
		/// <summary>
		/// 下载文件结束回调
		/// </summary>
		private Action m_DownLoadFileEnd;

		/// <summary>
		/// 开始版本号检查
		/// </summary>
		/// <param name="end"></param>
		public void StartVersion(Action end)
		{
			m_DownLoadFileEnd = end;
			GameNetManager.Instance.AddAgreement(EngineMessageHead.NET_CLIENT_VERSION_RESPONSE, "VersionResponse");
			string key = string.Format(EngineMessageHead.NET_CLIENT_MESSAGE_HEAD, EngineMessageHead.NET_CLIENT_VERSION_RESPONSE);
			MessageManger.Instance.AddMessageListener(key, this.gameObject, GetVersionFiles);

			VersionRequest version = new VersionRequest();
			version.SetVersion();
			GameNetManager.Instance.SendMessage<VersionRequest>(version, 1);
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
				}
			}
		}
	}
}
