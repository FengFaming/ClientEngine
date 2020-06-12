/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:窗口运行管理
 * Time:2020/6/3 16:23:41
* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Game.Engine
{
	public class ExeWindowManager : SingletonMonoClass<ExeWindowManager>
	{
		[DllImport("user32.dll")]
		static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);

		[DllImport("user32.dll")]
		static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

		/// <summary>
		/// 获得窗口句柄
		/// </summary>
		/// <returns></returns>
		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();

		private const uint SWP_SHOWWINDOW = 0x0040;

		/// <summary>
		/// 边框大小
		/// </summary>
		private const int GWL_STYLE = -16;
		private const int WS_BORDER = 1;
		private const int WS_POPUP = 0x800000;

		private int m_PosX;
		private int m_PosY;

		//宽高
		private int m_TxtWith;
		private int m_TxtHeight;

		/// <summary>
		/// 设置窗口大小
		/// 位置
		/// 是否系统计算
		/// 如果选择系统计算，那么前面的位置就失效
		/// </summary>
		/// <param name="pos">位置信息</param>
		/// <param name="size">窗口大小</param>
		/// <param name="cal">是否系统计算</param>
		public void SetWindows(Vector2Int pos, Vector2Int size, bool cal = false)
		{
			m_PosX = pos.x;
			m_PosY = pos.y;

			m_TxtWith = size.x;
			m_TxtHeight = size.y;

			if (cal)
			{
				//显示器支持的所有分辨率  
				int i = Screen.resolutions.Length;

				int resWidth = Screen.resolutions[i - 1].width;
				int resHeight = Screen.resolutions[i - 1].height;

				m_PosX = resWidth / 2 - m_TxtWith / 2;
				m_PosY = resHeight / 2 - m_TxtHeight / 2;
			}

			StopAllCoroutines();
			StartCoroutine("SetWindowState");
		}

		/// <summary>
		/// 协程修改缓存数据
		/// </summary>
		/// <returns></returns>
		private IEnumerator SetWindowState()
		{
			/*
			 * 为什么要使用协程进行修改，
			 *	是因为缓存里面还有数据，我们必须等缓存数据加载之后才能进行新的设置
			 *		否则设置就不起作用了
			 * */
			yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(0.1f);
			if (Application.platform != RuntimePlatform.WindowsEditor)
			{
				/*
				 * 方法解释
				 *	更改窗口属性
				 *	参数一：获得窗口句柄
				 *	参数二：有效范围是零到额外的串口内存字节数减去整数的大小，其中有下面几个值说明
				 *		GWL_EXSTYLE：-20 设置新的扩展窗口样式
				 *		GWL_HINSTANCE：-6 设置新的应用程序实例句柄
				 *		GWL_ID：-12 设置子窗口的新标识符
				 *		GWL_STYLE:-16 设置新的窗口样式
				 *		GWL_USERDATA:-21 设置与窗口关联的用户数据
				 *		GWL_WNDPROC:-4 设置窗口过程的新地址
				 *	参数三：长
				 *	函数说明：https://docs.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-setwindowlonga?redirectedfrom=MSDN
				 * */
				SetWindowLong(GetForegroundWindow(), GWL_STYLE, WS_POPUP);

				/*
				 * https://docs.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-setwindowpos
				 * */
				SetWindowPos(GetForegroundWindow(), 0, m_PosX, m_PosY, m_TxtWith, m_TxtHeight, SWP_SHOWWINDOW);
			}
		}
	}
}
