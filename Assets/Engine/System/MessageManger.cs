/*
 * Creator:ffm
 * Desc:消息管理器
 * Time:2019/12/25 17:20:52
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using System.Threading;

namespace Game.Engine
{
	/// <summary>
	/// 消息接口虚基类
	/// </summary>
	public interface IMessageEventListener
	{
		/// <summary>
		/// 是否还有用
		/// 主要用于有些内容已经销毁，但是回调还没有清空的内容
		/// </summary>
		/// <returns></returns>
		bool IsUseful();

		/// <summary>
		/// 是否允许清理
		/// </summary>
		/// <returns></returns>
		bool HasClear();

		/// <summary>
		/// 处理消息
		/// </summary>
		/// <param name="arms"></param>
		void HandleEvent(params object[] arms);

		/// <summary>
		/// 同一个消息头下的消息优先顺序
		/// </summary>
		/// <returns></returns>
		int EventPriority();

		/// <summary>
		/// 是否是同一个对象的内容
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		bool IsOneTarget(object obj);
	}

	/// <summary>
	/// 消息模板
	/// </summary>
	public class IMessageBase : IMessageEventListener
	{
		/// <summary>
		/// 回调函数
		/// </summary>
		private Action<object[]> m_DeleteFunction;

		private GameObject m_Owner;

		private bool m_HasClear;

		public IMessageBase(GameObject owner, bool hasClear, Action<object[]> del)
		{
			m_Owner = owner;
			m_HasClear = hasClear;
			m_DeleteFunction = del;
		}

		/// <summary>
		/// 对象是否已经销毁
		/// </summary>
		/// <returns></returns>
		public bool IsUseful()
		{
			return m_Owner != null;
		}

		/// <summary>
		/// 内容是否可以清理
		/// </summary>
		/// <returns></returns>
		public bool HasClear()
		{
			return m_HasClear;
		}

		/// <summary>
		/// 处理方法
		/// </summary>
		/// <param name="arms"></param>
		public void HandleEvent(params object[] arms)
		{
			if (m_DeleteFunction != null)
			{
				m_DeleteFunction(arms);
			}
		}

		/// <summary>
		/// 排序规则
		/// </summary>
		/// <returns></returns>
		public int EventPriority()
		{
			return 0;
		}

		/// <summary>
		/// 是不是同一个
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool IsOneTarget(object obj)
		{
			return m_Owner.Equals(obj);
		}
	}

	/// <summary>
	/// 消息管理器
	/// </summary>
	[XLua.LuaCallCSharp]
	public class MessageManger : SingletonMonoClass<MessageManger>
	{
		/// <summary>
		/// 消息内容交互
		/// 主要用在多线程交互的时候
		/// </summary>
		internal struct MessageInfo
		{
			/// <summary>
			/// 消息头
			/// </summary>
			public string m_Header;

			/// <summary>
			/// 消息数据
			/// </summary>
			public List<object> m_Arms;

			public MessageInfo(string head, params object[] arms)
			{
				m_Header = head;
				m_Arms = new List<object>();
				m_Arms.Clear();
				if (arms != null && arms.Length > 0)
				{
					m_Arms.AddRange(arms);
				}
			}
		}

		/// <summary>
		/// 一个消息头对应的数据
		/// </summary>
		internal class MessagesInfo
		{
			private string m_MessageKey;
			public string MessageKey { get { return m_MessageKey; } }

			private List<IMessageEventListener> m_AllListeners;

			public MessagesInfo(string key)
			{
				m_MessageKey = key;
				m_AllListeners = new List<IMessageEventListener>();
			}

			/// <summary>
			/// 添加数据
			/// </summary>
			/// <param name="owner"></param>
			/// <param name="haveClear"></param>
			/// <param name="action"></param>
			public void AddListener(GameObject owner, bool haveClear, Action<object[]> action)
			{
				AddListener(new IMessageBase(owner, haveClear, action));
			}

			/// <summary>
			/// 添加数据
			/// </summary>
			/// <param name="listen"></param>
			public void AddListener(IMessageEventListener listen)
			{
				if (!m_AllListeners.Contains(listen))
				{
					m_AllListeners.Add(listen);
					m_AllListeners.Sort((IMessageEventListener l1, IMessageEventListener l2) =>
					{
						return l2.EventPriority() - l1.EventPriority();
					});
				}
			}

			/// <summary>
			/// 发送消息
			/// </summary>
			/// <param name="arms"></param>
			public void SendMessage(params object[] arms)
			{
				List<IMessageEventListener> ibs = new List<IMessageEventListener>();
				ibs.AddRange(m_AllListeners);
				for (int index = 0; index < ibs.Count; index++)
				{
					ibs[index].HandleEvent(arms);
				}
			}

			/// <summary>
			/// 移除数据
			/// </summary>
			/// <param name="listen"></param>
			public void RemoveListener(IMessageEventListener listen)
			{
				if (m_AllListeners.Contains(listen))
				{
					m_AllListeners.Remove(listen);
				}
			}

			/// <summary>
			/// 移除数据
			/// </summary>
			/// <param name="owner"></param>
			public void RemoveListener(GameObject owner)
			{
				for (int index = 0; index < m_AllListeners.Count;)
				{
					if (m_AllListeners[index].IsOneTarget(owner))
					{
						m_AllListeners.RemoveAt(index);
					}
					else
					{
						index++;
					}
				}
			}

			/// <summary>
			/// 清空所有数据
			/// </summary>
			public void ClearData()
			{
				m_AllListeners.Clear();
			}

			/// <summary>
			/// 重新排列
			///		返回的是看看消息头是否有效
			/// </summary>
			/// <returns></returns>
			public bool Rearrange()
			{
				for (int index = 0; index < m_AllListeners.Count;)
				{
					if (m_AllListeners[index].HasClear())
					{
						if (m_AllListeners[index].IsUseful())
						{
							m_AllListeners.RemoveAt(index);
						}
						else
						{
							index++;
						}
					}
					else
					{
						index++;
					}
				}

				return m_AllListeners.Count > 0;
			}
		}

		/// <summary>
		/// 消息头对应的下标数据
		/// </summary>
		private Dictionary<string, int> m_KeyWithIndex;

		/// <summary>
		/// 消息的真实内容
		/// </summary>
		private List<MessagesInfo> m_AllMessageListens;

		/// <summary>
		/// 别的消息队列
		/// </summary>
		private Queue<MessageInfo> m_ThreadQueue;

		/// <summary>
		/// 是否正在重新排列数据
		/// </summary>
		private bool m_IsRearrange;

		/// <summary>
		/// 线程锁
		/// </summary>
		private static readonly object m_Lock = "lock-thread";

		protected override void Awake()
		{
			base.Awake();
			m_KeyWithIndex = new Dictionary<string, int>();
			m_AllMessageListens = new List<MessagesInfo>();
			m_AllMessageListens.Clear();
			m_KeyWithIndex.Clear();

			m_ThreadQueue = new Queue<MessageInfo>();
			m_ThreadQueue.Clear();

			m_IsRearrange = false;
		}

		/// <summary>
		/// 添加消息
		/// </summary>
		/// <param name="head"></param>
		/// <param name="listener"></param>
		public void AddMessageListener(string head, IMessageEventListener listener)
		{
			if (!m_IsRearrange)
			{
				if (m_KeyWithIndex.ContainsKey(head))
				{
					int id = m_KeyWithIndex[head];
					m_AllMessageListens[id].AddListener(listener);
				}
				else
				{
					m_KeyWithIndex.Add(head, m_AllMessageListens.Count);
					MessagesInfo info = new MessagesInfo(head);
					info.AddListener(listener);
					m_AllMessageListens.Add(info);
				}
			}
		}

		/// <summary>
		/// 添加消息
		/// </summary>
		/// <param name="head">消息头</param>
		/// <param name="owner">消息拥有者</param>
		/// <param name="clear">消息是否允许管理类进行清理</param>
		/// <param name="fun">消息回调</param>
		public void AddMessageListener(string head, GameObject owner, bool clear, Action<object[]> fun)
		{
			IMessageBase ib = new IMessageBase(owner, clear, fun);
			AddMessageListener(head, ib);
		}

		/// <summary>
		/// 添加消息
		///		永久不失效
		///		允许管理类进行清理
		/// </summary>
		/// <param name="head"></param>
		/// <param name="fun"></param>
		public void AddMessageListener(string head, Action<object[]> fun)
		{
			AddMessageListener(head, this.gameObject, true, fun);
		}

		/// <summary>
		/// 添加消息
		///		允许管理类进行清理
		/// </summary>
		/// <param name="head"></param>
		/// <param name="owner"></param>
		/// <param name="fun"></param>
		public void AddMessageListener(string head, GameObject owner, Action<object[]> fun)
		{
			AddMessageListener(head, owner, true, fun);
		}

		/// <summary>
		/// 添加消息
		///		永久不失效
		/// </summary>
		/// <param name="head"></param>
		/// <param name="clear"></param>
		/// <param name="fun"></param>
		public void AddMessageListener(string head, bool clear, Action<object[]> fun)
		{
			AddMessageListener(head, this.gameObject, clear, fun);
		}

		public void RemoveMessageListener(string head, IMessageEventListener listen)
		{
			if (!m_IsRearrange)
			{
				if (m_KeyWithIndex.ContainsKey(head))
				{
					m_AllMessageListens[m_KeyWithIndex[head]].RemoveListener(listen);
				}
			}
		}

		/// <summary>
		/// 移除消息
		/// </summary>
		/// <param name="head"></param>
		/// <param name="fun"></param>
		public void RemoveMessageListener(string head, Action<object[]> fun)
		{
			IMessageBase ib = new IMessageBase(this.gameObject, true, fun);
			RemoveMessageListener(head, ib);
		}

		/// <summary>
		/// 移除消息
		/// </summary>
		/// <param name="owner"></param>
		public void RemoveMessageListener(GameObject owner)
		{
			if (!m_IsRearrange)
			{
				for (int index = 0; index < m_AllMessageListens.Count; index++)
				{
					RemoveMessageListener(m_AllMessageListens[index].MessageKey, owner);
				}
			}
		}

		/// <summary>
		/// 移除消息
		/// </summary>
		/// <param name="head"></param>
		/// <param name="owner"></param>
		public void RemoveMessageListener(string head, GameObject owner)
		{
			if (!m_IsRearrange)
			{
				if (m_KeyWithIndex.ContainsKey(head))
				{
					int id = m_KeyWithIndex[head];
					m_AllMessageListens[id].RemoveListener(owner);
				}
			}
		}

		/// <summary>
		/// 移除消息
		/// </summary>
		/// <param name="head"></param>
		/// <param name="owner"></param>
		/// <param name="fun"></param>
		public void RemoveMessageListener(string head, GameObject owner, Action<object[]> fun)
		{
			IMessageBase ib = new IMessageBase(owner, true, fun);
			RemoveMessageListener(head, ib);
		}

		/// <summary>
		/// 发送消息
		/// </summary>
		/// <param name="head"></param>
		/// <param name="arms"></param>
		public void SendMessage(string head, params object[] arms)
		{
			if (!m_IsRearrange)
			{
				if (m_KeyWithIndex.ContainsKey(head))
				{
					m_AllMessageListens[m_KeyWithIndex[head]].SendMessage(arms);
				}
			}
		}

		/// <summary>
		/// 别的线程交互的消息
		/// </summary>
		/// <param name="head"></param>
		/// <param name="arms"></param>
		public void SendMessageThread(string head, params object[] arms)
		{
			Monitor.Enter(m_Lock);
			MessageInfo info = new MessageInfo();
			info.m_Header = head;
			if (arms != null && arms.Length > 0)
			{
				info.m_Arms.AddRange(arms);
			}

			m_ThreadQueue.Enqueue(info);

			Monitor.Exit(m_Lock);
		}

		/// <summary>
		/// 重新排列数据
		/// </summary>
		public void RearrangeMessage()
		{
			m_IsRearrange = true;

			List<MessagesInfo> infos = new List<MessagesInfo>();
			infos.Clear();
			infos.AddRange(m_AllMessageListens);

			m_AllMessageListens.Clear();
			m_KeyWithIndex.Clear();

			for (int index = 0; index < infos.Count;)
			{
				if (!infos[index].Rearrange())
				{
					infos.RemoveAt(index);
				}
				else
				{
					index++;
				}
			}

			for (int index = 0; index < infos.Count; index++)
			{
				m_KeyWithIndex.Add(infos[index].MessageKey, index);
				m_AllMessageListens.Add(infos[index]);
			}

			m_IsRearrange = false;
		}

		private void Update()
		{
			if (!m_IsRearrange)
			{
				if (m_ThreadQueue.Count > 0)
				{
					for (int index = 0; index < 10; index++)
					{
						if (m_ThreadQueue.Count > 0)
						{
							MessageInfo info = m_ThreadQueue.Dequeue();
							SendMessage(info.m_Header, info.m_Arms);
						}
						else
						{
							break;
						}
					}
				}
			}
		}
	}
}