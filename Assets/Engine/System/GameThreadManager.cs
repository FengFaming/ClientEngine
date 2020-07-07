/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:游戏线程管理
 * Time:2020/6/4 8:21:28
* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game.Engine
{
	public class GameThreadManager : SingletonMonoClass<GameThreadManager>
	{
		internal class MyThread
		{
			/// <summary>
			/// 线程主体
			/// </summary>
			public Thread m_Thread;

			/// <summary>
			/// 线程主执行方法
			/// </summary>
			public Action m_ThradMainFunction;

			/// <summary>
			/// 返回主进程
			/// </summary>
			public Action m_GoToMainFunction;

			/// <summary>
			/// 线程通知返回主进程
			/// </summary>
			public Action<MyThread> m_ToMainThread;

			/// <summary>
			/// 主进程获取
			/// </summary>
			private SynchronizationContext m_MainThreadSynContext;
			private delegate void SendMessageWithThread();

			public void StartThread()
			{
				m_MainThreadSynContext = SynchronizationContext.Current;
				m_Thread = new Thread(MainThreadFunction);
				//m_Thread.IsBackground = true;
				SendMessageWithThread callback = new SendMessageWithThread(SendMessage);
				m_Thread.Start(callback);
			}

			/// <summary>
			/// 线程方法
			/// </summary>
			/// <param name="action"></param>
			private void MainThreadFunction(object action)
			{
				if (m_ThradMainFunction != null)
				{
					m_ThradMainFunction();
				}

				SendMessageWithThread callback = action as SendMessageWithThread;
				callback();
			}

			/// <summary>
			/// 通过信号量的方法返回租进程
			/// </summary>
			private void SendMessage()
			{
				m_MainThreadSynContext.Post(new SendOrPostCallback(GoBaMainThread), null);
			}

			/// <summary>
			/// 由子线程返回到主线程
			/// </summary>
			/// <param name="state"></param>
			private void GoBaMainThread(object state)
			{
				if (m_ToMainThread != null)
				{
					m_ToMainThread(this);
				}

				if (m_Thread != null)
				{
					m_Thread.Abort();
				}
			}
		}

		/// <summary>
		/// 所有管理的线程
		/// </summary>
		private List<MyThread> m_AllThreads;

		protected override void Awake()
		{
			base.Awake();
			m_AllThreads = new List<MyThread>();
			m_AllThreads.Clear();
		}

		/// <summary>
		/// 清除一个
		/// </summary>
		/// <param name="t"></param>
		private void CloseOne(MyThread t)
		{
			//先移除在返回
			m_AllThreads.Remove(t);
			if (t != null)
			{
				t.m_GoToMainFunction();
			}

			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		/// <summary>
		/// 创建一个线程
		/// </summary>
		/// <param name="main">线程主要执行方法</param>
		/// <param name="tomain">返回主进程方法</param>
		public void CreateThread(Action main, Action tomain)
		{
			MyThread t = new MyThread();
			t.m_ThradMainFunction = main;
			t.m_ToMainThread = CloseOne;
			t.m_GoToMainFunction = tomain;
			t.StartThread();
			m_AllThreads.Add(t);
		}

		/// <summary>
		/// 关闭所有
		/// </summary>
		public void CloseAll()
		{
			while (m_AllThreads.Count > 0)
			{
				MyThread t = m_AllThreads[0];
				m_AllThreads.RemoveAt(0);
				t.m_Thread.Abort();
				t.m_ThradMainFunction = null;
				t.m_ToMainThread = null;
				t.m_Thread = null;
				t = null;
				GC.Collect();
				GC.WaitForPendingFinalizers();
				Debug.Log("close");
			}
		}

		private void OnDestroy()
		{
			CloseAll();
		}
	}
}
