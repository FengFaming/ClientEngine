/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:编辑器下使用协同程序
 * Time:2020/8/19 14:05:28
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * 使用细则，EditorCoroutineRunner.StartEditorCoroutine即可
 * */

public class EditorCoroutine : IEnumerator
{
	private Stack<IEnumerator> m_ExecutionStack;

	public EditorCoroutine(IEnumerator iterator)
	{
		m_ExecutionStack = new Stack<IEnumerator>();
		m_ExecutionStack.Clear();
		m_ExecutionStack.Push(iterator);
	}

	public bool MoveNext()
	{
		IEnumerator i = this.m_ExecutionStack.Peek();
		if (i.MoveNext())
		{
			object result = i.Current;
			if (result != null && result is IEnumerator)
			{
				this.m_ExecutionStack.Push((IEnumerator)result);
			}

			return true;
		}
		else
		{
			if (this.m_ExecutionStack.Count > 1)
			{
				this.m_ExecutionStack.Pop();
				return true;
			}
		}

		return false;
	}

	public void Reset()
	{

	}

	public object Current { get { return this.m_ExecutionStack.Peek().Current; } }

	public bool Find(IEnumerator iterator)
	{
		return this.m_ExecutionStack.Contains(iterator);
	}
}

public static class EditorCoroutineRunner
{
	private static List<EditorCoroutine> m_EditorCoroutineList;
	private static List<IEnumerator> m_Buffer;

	public static IEnumerator StartEditorCoroutine(IEnumerator iterator)
	{
		if (m_EditorCoroutineList == null)
		{
			m_EditorCoroutineList = new List<EditorCoroutine>();
			m_EditorCoroutineList.Clear();
		}

		if (m_Buffer == null)
		{
			m_Buffer = new List<IEnumerator>();
			m_Buffer.Clear();
		}

		if (m_EditorCoroutineList.Count == 0)
		{
			EditorApplication.update += Update;
		}

		m_Buffer.Add(iterator);
		return iterator;
	}

	private static bool Find(IEnumerator iterator)
	{
		foreach (EditorCoroutine ec in m_EditorCoroutineList)
		{
			if (ec.Find(iterator))
			{
				return true;
			}
		}

		return false;
	}

	private static void Update()
	{
		m_EditorCoroutineList.RemoveAll(coroutine => { return coroutine.MoveNext() == false; });

		if (m_Buffer.Count > 0)
		{
			foreach (IEnumerator it in m_Buffer)
			{
				if (!Find(it))
				{
					m_EditorCoroutineList.Add(new EditorCoroutine(it));
				}
			}

			m_Buffer.Clear();
		}

		if (m_EditorCoroutineList.Count == 0)
		{
			EditorApplication.update -= Update;
		}
	}
}
