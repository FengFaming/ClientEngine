using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerExe
{
	/// <summary>
	/// 一个客户端
	/// </summary>
	public class ClientInfo
	{
		/// <summary>
		/// 服务器唯一标识
		/// </summary>
		public int m_OnlyID;

		/// <summary>
		/// 是否有效
		/// </summary>
		public bool m_IsSuccess;

		/// <summary>
		/// 连接端
		/// </summary>
		private Socket m_Socket;

		public ClientInfo(int id, Socket socket)
		{
			m_Socket = socket;
			m_OnlyID = id;
			m_IsSuccess = true;
		}

		/// <summary>
		/// 获取消息
		/// </summary>
		/// <param name="datas"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public bool GetRecvMessage(out byte[] datas, ref int length)
		{
			if (m_Socket.Connected && m_IsSuccess)
			{
				datas = new byte[1024 * 1024 * 3];
				length = m_Socket.Receive(datas);
				return true;
			}

			datas = null;
			return false;
		}

		/// <summary>
		/// 发送消息
		/// </summary>
		/// <param name="datas"></param>
		public void Send(byte[] datas)
		{
			if (m_Socket.Connected && m_IsSuccess)
			{
				m_Socket.Send(datas);
			}
		}
	}
}
