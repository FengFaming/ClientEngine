/*
 * Creator:ffm
 * Desc:反射控制类
 * Time:2019/12/27 15:38:10
* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.Engine
{
	public class ReflexManager : SingletonMonoClass<ReflexManager>
	{
		/// <summary>
		/// 创建一个实体类
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object CreateClass(string name)
		{
			System.Type t = System.Type.GetType(name);
			if (t == null)
			{
				string value = "Game.Engine." + name;
				t = System.Type.GetType(value);
			}

			if (t != null)
			{
				return System.Activator.CreateInstance(t);
			}
			else
			{
				Debug.LogErrorFormat("create class:{0} error.", name);
			}

			return null;
		}

		/// <summary>
		/// 带一个参数创建
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public object CreateClass(string name, object value)
		{
			System.Type t = System.Type.GetType(name);
			if (t == null)
			{
				string v = "Game.Engine." + name;
				t = System.Type.GetType(v);
			}

			if (t != null)
			{
				return System.Activator.CreateInstance(t, value);
			}
			else
			{
				Debug.LogErrorFormat("create class:{0} error.", name);
			}

			return null;
		}

		/// <summary>
		/// 创建一个对象类
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public T CreateClass<T>(string name, object value)
		{
			if (name == string.Empty || name == " ")
			{
				Debug.LogWarningFormat("reflex error.because name:{0} is null.", name);
			}
			else
			{
				try
				{
					System.Type t = System.Type.GetType(name);
					if (t == null)
					{
						string v = "SGGame.Engine." + name;
						t = System.Type.GetType(v);
					}

					return CreateClass<T>(t, value);
				}
				catch (Exception e)
				{
					Debug.LogWarning("create t:" + name + " error " + e);
				}
			}

			return default(T);
		}

		/// <summary>
		/// 创建类
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public T CreateClass<T>(System.Type type, object value)
		{
			object obj = System.Activator.CreateInstance(type, value);
			if (obj != null)
			{
				return (T)obj;
			}

			return default(T);
		}

		/// <summary>
		/// 获取某一个类的某一个方法
		/// </summary>
		/// <param name="type"></param>
		/// <param name="methodName"></param>
		/// <returns></returns>
		public MethodInfo GetMethodInfo(Type type, string methodName)
		{
			Type t = System.Type.GetType(type.FullName);
			MethodInfo info = t.GetMethod(methodName);
			if (info != null)
			{
				return info;
			}

			return default(MethodInfo);
		}

		/// <summary>
		/// 获取私有方法
		/// </summary>
		/// <param name="type"></param>
		/// <param name="methodName"></param>
		/// <returns></returns>
		public MethodInfo GetMethodInfoNonPublic(Type type, string methodName)
		{
			Type t = System.Type.GetType(type.FullName);
			MethodInfo info = t.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
			if (info != null)
			{
				return info;
			}

			return default(MethodInfo);
		}

		/// <summary>
		/// 调用方法
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="methodName"></param>
		/// <param name="arms"></param>
		public void InvkMethod<T>(object obj, string methodName, params object[] arms)
		{
			MethodInfo info = GetMethodInfo(typeof(T), methodName);
			InvkMethod(obj, info, arms);
		}

		/// <summary>
		/// 调用方法
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="info"></param>
		/// <param name="arms"></param>
		public void InvkMethod(object obj, MethodInfo info, params object[] arms)
		{
			if (info != null)
			{
				info.Invoke(obj, arms);
			}
		}

		/// <summary>
		/// 带返回值的调用方法
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="info"></param>
		/// <param name="arms"></param>
		/// <returns></returns>
		public object InvkMethodReturn(object obj, MethodInfo info, params object[] arms)
		{
			if (info != null)
			{
				return info.Invoke(obj, arms);
			}

			return null;
		}

		/// <summary>
		/// 调用方法
		/// </summary>
		/// <param name="type"></param>
		/// <param name="methodName"></param>
		/// <param name="arms"></param>
		public void InvkMethod<T>(Type type, string methodName, params object[] arms)
		{
			MethodInfo info = GetMethodInfo(type, methodName);
			if (info != null)
			{
				InvkMethod<T>(type, info, arms);
			}
		}

		/// <summary>
		/// 调用方法
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="info"></param>
		/// <param name="arms"></param>
		public void InvkMethod<T>(Type type, MethodInfo info, params object[] arms)
		{
			T obj = CreateClass<T>(type, null);
			if (obj != null)
			{
				InvkMethod(obj, info, arms);
			}
		}
	}
}
