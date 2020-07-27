/*
 * Creator:ffm
 * Desc:游戏时间管理
 * Time:2020/1/19 15:39:33
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class GameTimeManager : SingletonMonoClass<GameTimeManager>
	{
		/// <summary>
		/// 客户端时间上传
		/// </summary>
		internal class ServetTimeRequest : ClientSendMessageBase
		{
			public ServetTimeRequest() : base()
			{
				m_MessageHead.m_MessageID = EngineMessageHead.NET_CLIENT_TIME_REQUEST;
				m_MessageHead.m_MessageType = 1;
			}

			public void SetTime(long time)
			{
				m_SendData.AddRange(BitConverter.GetBytes(time));
			}
		}

		/// <summary>
		/// 游戏运行时长
		/// </summary>
		private int m_GameNowTime;
		public int GameNowTime
		{
			get { return m_GameNowTime; }
		}

		private float m_CalNowTime;

		/// <summary>
		/// 服务器时间
		/// </summary>
		private long m_ServerTimeTicks;

		protected override void Awake()
		{
			base.Awake();
			m_GameNowTime = 0;
			m_CalNowTime = 0;
		}

		private void Start()
		{
			string key = string.Format(EngineMessageHead.NET_CLIENT_MESSAGE_HEAD, EngineMessageHead.NET_CLIENT_TIME_RESPONSE);
			MessageManger.Instance.AddMessageListener(key, this.gameObject, GetServerTime);
		}

		/// <summary>
		/// 服务器下发的时间
		/// </summary>
		/// <param name="arms"></param>
		private void GetServerTime(params object[] arms)
		{
			if (arms.Length > 0)
			{
				List<object> list = arms[0] as List<object>;
				if (list[0] is GetServerTimeResponse)
				{
					GetServerTimeResponse getServerTimeResponse = list[0] as GetServerTimeResponse;
					if (getServerTimeResponse.m_IsSuccess)
					{
						m_ServerTimeTicks = getServerTimeResponse.m_ServetTime;

						ServetTimeRequest request = new ServetTimeRequest();
						request.SetTime(System.DateTime.Now.Ticks);
						GameNetManager.Instance.SendMessage<ServetTimeRequest>(request, 1);
					}
				}
			}
		}

		private void Update()
		{
			m_CalNowTime += Time.deltaTime;
			m_GameNowTime += (int)(m_CalNowTime / 1);
			m_CalNowTime = m_CalNowTime % 1;
		}
	}
}
