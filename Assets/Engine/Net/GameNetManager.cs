/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:游戏网络管理控制类
 * Time:2020/7/16 8:47:50
* */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Engine
{
	public class GameNetManager : Singleton<GameNetManager>
	{
		/// <summary>
		/// 所有的协议数据
		/// </summary>
		private Dictionary<int, string> m_AllAgreementDic;

		/// <summary>
		/// 所有客户端
		/// </summary>
		private Dictionary<byte, GameNetClient> m_AllClient;

		public GameNetManager()
		{
			m_AllAgreementDic = new Dictionary<int, string>();
			m_AllAgreementDic.Clear();

			m_AllClient = new Dictionary<byte, GameNetClient>();
			m_AllClient.Clear();
		}

		/// <summary>
		/// 添加协议
		/// </summary>
		/// <param name="id"></param>
		/// <param name="control"></param>
		public void AddAgreement(int id, string control)
		{
			if (!m_AllAgreementDic.ContainsKey(id))
			{
				m_AllAgreementDic.Add(id, control);
			}
			else
			{
				Debug.LogWarningFormat("agreement:{0} has two or mor", id);
			}
		}

		/// <summary>
		/// 获取协议号对应的字段
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string GetAgreement(int id)
		{
			if (m_AllAgreementDic.ContainsKey(id))
			{
				return m_AllAgreementDic[id];
			}

			return null;
		}

		/// <summary>
		/// 创建客户端
		/// </summary>
		/// <param name="client"></param>
		/// <param name="ip"></param>
		/// <param name="port"></param>
		/// <param name="maxLength"></param>
		public void CreateClient(byte client, string ip, int port, int maxLength)
		{
			if (!m_AllClient.ContainsKey(client))
			{
				GameNetClient net = new GameNetClient(ip, port, maxLength);
				net.ConnectSocket();
				m_AllClient.Add(client, net);
			}
		}

		/// <summary>
		/// 发送消息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pack"></param>
		/// <param name="client"></param>
		public void SendMessage<T>(T pack, byte client) where T : ClientSendMessageBase
		{
			if (m_AllClient.ContainsKey(client))
			{
				m_AllClient[client].SendMessage<T>(pack);
			}
		}
	}
}
