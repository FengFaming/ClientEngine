/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:系统显示打印日志
 * Time:2020/6/3 14:19:10
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Game.Engine
{
	public class ConsoleWindow
	{
		/// <summary>
		/// 关闭输出
		/// </summary>
		private TextWriter m_OldOutput;

		/// <summary>
		/// 初始化
		/// </summary>
		public void Initialize()
		{
			if (!AttachConsole(0x0ffffffff))
			{
				AllocConsole();
			}

			m_OldOutput = Console.Out;

			try
			{
				IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
				Microsoft.Win32.SafeHandles.SafeFileHandle safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, true);
				FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
				System.Text.Encoding encoding = System.Text.Encoding.ASCII;
				StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
				standardOutput.AutoFlush = true;
				Console.SetOut(standardOutput);
			}
			catch (System.Exception e)
			{
				Debug.Log("Couldn't redirect output: " + e.Message);
			}
		}

		/// <summary>
		/// 关闭
		/// </summary>
		public void Shutdown()
		{
			Console.SetOut(m_OldOutput);
			FreeConsole();
		}

		/// <summary>
		/// 设置标题
		/// </summary>
		/// <param name="strName"></param>
		public void SetTitle(string strName)
		{
			SetConsoleTitle(strName);
		}

		private const int STD_OUTPUT_HANDLE = -11;

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AttachConsole(uint dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool FreeConsole();

		[DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll")]
		static extern bool SetConsoleTitle(string lpConsoleTitle);
	}

	public class SystemDebugManager : MonoBehaviour
	{
		private ConsoleWindow m_PutWindows;

		public void StartWindow()
		{
			m_PutWindows = new ConsoleWindow();
			m_PutWindows.Initialize();
			m_PutWindows.SetTitle("Debug Log");
		}

		public void OnDestroy()
		{
			if (m_PutWindows != null)
			{
				m_PutWindows.Shutdown();
			}

			m_PutWindows = null;
		}
	}
}
