using System;
using System.Collections.Generic;

public class Debug
{
	private static bool m_ShowInConsole = true;
	public static bool ShowInConsole { set { m_ShowInConsole = value; } }

	private static bool m_ShowDebug = true;
	public static bool ShowDebug { set { m_ShowDebug = value; } }

	public static void DrawLine(UnityEngine.Vector3 start, UnityEngine.Vector3 end, UnityEngine.Color color)
	{
		if (m_ShowDebug)
		{
			UnityEngine.Debug.DrawLine(start, end, color);
		}
	}

	public static void DrawRay(UnityEngine.Vector3 start, UnityEngine.Vector3 end, UnityEngine.Color color)
	{
		if (m_ShowDebug)
		{
			UnityEngine.Debug.DrawRay(start, end, color);
		}
	}

	public static void Log(object message)
	{
		if (m_ShowDebug)
		{
			UnityEngine.Debug.Log(message);
		}

		if (m_ShowInConsole)
		{
			Console.WriteLine(message);
		}
	}

	public static void Log(object message, UnityEngine.Object context)
	{
		Log(message.ToString() + " " + context.ToString());
	}

	public static void LogFormat(string format, params object[] args)
	{
		Log(string.Format(format, args));
	}

	public static void LogWarning(object message)
	{
		if (m_ShowDebug)
		{
			UnityEngine.Debug.LogWarning(message);
		}

		if (m_ShowInConsole)
		{
			Console.WriteLine(message);
		}
	}

	public static void LogWarning(object message, UnityEngine.Object context)
	{
		LogWarning(message.ToString() + " " + context.ToString());
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		LogWarning(string.Format(format, args));
	}

	public static void LogError(object message)
	{
		if (m_ShowDebug)
		{
			UnityEngine.Debug.LogError(message);
		}

		if (m_ShowInConsole)
		{
			Console.WriteLine(message);
		}
	}

	public static void LogError(object message, UnityEngine.Object context)
	{
		LogError(message.ToString() + " " + context.ToString());
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		LogError(string.Format(format, args));
	}

	public static void LogException(Exception exception)
	{
		if (m_ShowDebug)
		{
			UnityEngine.Debug.LogException(exception);
		}

		if (m_ShowInConsole)
		{
			Console.WriteLine("Log Exception: " + exception.ToString());
		}
	}
}