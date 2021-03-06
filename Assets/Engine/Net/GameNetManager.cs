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
using UnityEngine;

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
		/// <param name="client">客户端标记</param>
		/// <param name="ip">IP地址</param>
		/// <param name="port">端口号</param>
		/// <param name="maxLength">最大长度</param>
		/// <param name="success">成功失败回调</param>
		public void CreateClient(byte client, string ip, int port, int maxLength, Action<bool> success)
		{
			if (!m_AllClient.ContainsKey(client))
			{
				GameNetClient net = new GameNetClient(ip, port, maxLength);
				net.ConnectSocket(success);
				m_AllClient.Add(client, net);
			}
		}

		/// <summary>
		/// 关闭客户端
		/// </summary>
		/// <param name="client"></param>
		public void CloseClient(byte client)
		{
			if (m_AllClient.ContainsKey(client))
			{
				m_AllClient[client].Close();
				m_AllClient.Remove(client);
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
