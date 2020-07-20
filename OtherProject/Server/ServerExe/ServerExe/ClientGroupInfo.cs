using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerExe
{
	/// <summary>
	/// 一个客户端群
	/// </summary>
	public class ClientGroupInfo
	{
		/// <summary>
		/// 客户端群唯一ID
		/// </summary>
		private int m_OnlyID;

		/// <summary>
		/// 所有包含的客户端
		/// </summary>
		private List<ClientInfo> m_AllClientInfoDic;

		/// <summary>
		/// 消息监听线程
		/// </summary>
		private Thread m_RecvSendThread;

		/// <summary>
		/// 发送的消息队列
		/// </summary>
		private Queue<ClientSendMessageBase> m_MessageQueue;

		public ClientGroupInfo(int id)
		{
			m_AllClientInfoDic = new List<ClientInfo>();
			m_AllClientInfoDic.Clear();
			m_OnlyID = id;
		}

		/// <summary>
		/// 添加一个客户端
		/// </summary>
		/// <param name="info"></param>
		public void AddClientInfo(ClientInfo info)
		{
			m_AllClientInfoDic.Add(info);
		}

		/// <summary>
		/// 移除监听
		/// </summary>
		/// <param name="id"></param>
		public void RemoveClient(int id)
		{
			//if (m_AllClientInfoDic.ContainsKey(id))
			//{
			//	m_AllClientInfoDic[id].m_IsSuccess = false;
			//}
		}

		/// <summary>
		/// 开始线程监听内容
		/// </summary>
		public void StartThreadListen()
		{
			m_RecvSendThread = new Thread(RecvMessage);
			m_RecvSendThread.IsBackground = true;
			m_RecvSendThread.Start();
		}

		private void RecvMessage()
		{
			while (true)
			{
				///分析数据
				for (int index = 0; index < m_AllClientInfoDic.Count; index++)
				{
					if (m_AllClientInfoDic[index].m_IsSuccess)
					{
						int length = 0;
						byte[] datas = null;
						if (m_AllClientInfoDic[index].GetRecvMessage(out datas, ref length))
						{
							ClientSendMessageBase clientRecvMessageBase = new ClientSendMessageBase(m_AllClientInfoDic[index]);
							clientRecvMessageBase.SetRecvMessage(datas, length);
							m_MessageQueue.Enqueue(clientRecvMessageBase);
						}
					}
				}

				///发送数据
				while (m_MessageQueue.Count > 0)
				{
					ClientSendMessageBase message = m_MessageQueue.Dequeue();
					message.Send();
				}
			}
		}
	}
}
