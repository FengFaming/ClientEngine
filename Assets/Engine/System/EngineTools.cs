/*
 * Creator:ffm
 * Desc:框架内置工具
 * Time:2019/12/23 17:08:12
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public partial class EngineTools : Singleton<EngineTools>
	{
		/// <summary>
		/// 检查身份证号码是否符合
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool CheckIDCard(string id)
		{
			if (id.Length == 18)
			{
				return CheckIDCard18(id);
			}

			return false;
		}

		/// <summary>
		/// 提起身份证上的性别
		///  1：男；-1：女；0：身份证有误
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public int CheckIDCardSex(string id)
		{
			if (CheckIDCard(id))
			{
				if (id.Length == 18)
				{
					int sex = int.Parse(id.Substring(16, 1));
					return sex % 2 == 0 ? -1 : 1;
				}
			}

			return 0;
		}

		/// <summary>
		/// 获取朝向旋转角度
		/// </summary>
		/// <param name="target"></param>
		/// <param name="start"></param>
		/// <returns></returns>
		public Quaternion GetLookAtQuatrnion(Vector3 target, Vector3 start)
		{
			Vector3 f = target - start;
			return Quaternion.LookRotation(f);
		}

		/// <summary>
		/// 创建矩阵内容
		/// </summary>
		/// <param name="parent">父亲节点</param>
		/// <param name="clone">克隆体</param>
		/// <param name="length">距离</param>
		/// <param name="c">多上层</param>
		public void CreateRect(Transform parent, GameObject clone, float length, int c)
		{
			Vector3 leftPosition = Vector3.zero;
			leftPosition.x = 0 - length * c;
			leftPosition.z = length * c;
			clone.gameObject.transform.parent = parent;
			clone.gameObject.transform.localPosition = leftPosition;
			clone.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
			clone.gameObject.transform.localScale = Vector3.one;

			for (int width = 0; width < c * 2; width++)
			{
				for (int height = 0; height < c * 2; height++)
				{
					if (width == 0 && height == 0)
					{
						continue;
					}
					else
					{
						Vector3 position = new Vector3(length * height, 0, -length * width);
						position += leftPosition;
						SetGameObject(parent, clone, position);
					}
				}
			}
		}

		/// <summary>
		/// 拷贝rigidbody
		/// </summary>
		/// <param name="target"></param>
		/// <param name="n"></param>
		public void CopyRigidbody(Rigidbody target, ref Rigidbody n)
		{
			n.mass = target.mass;
			n.drag = target.drag;
			n.angularDrag = target.angularDrag;
			n.useGravity = target.useGravity;
			n.isKinematic = target.isKinematic;
			n.interpolation = target.interpolation;
			n.collisionDetectionMode = target.collisionDetectionMode;
			n.constraints = target.constraints;
		}

		/// <summary>
		/// 将字符串转成枚举
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public T StringToEnum<T>(string value)
		{
			try
			{
				return (T)(Enum.Parse(typeof(T), value));
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}

			return default(T);
		}

		/// <summary>
		/// 字符串转向量
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public Vector3 StringToVector3(string s)
		{
			Vector3 rt = Vector3.zero;
			if (s == null || s.Equals(" "))
			{
				return rt;
			}

			string[] point = s.Split(',');
			if (point.Length != 3)
			{
				return rt;
			}

			rt.x = float.Parse(point[0]);
			rt.y = float.Parse(point[1]);
			rt.z = float.Parse(point[2]);
			return rt;
		}

		/// <summary>
		/// 求数组中n个元素的组合
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public List<T[]> GetCombination<T>(T[] t, int n)
		{
			if (t.Length < n)
			{
				return null;
			}

			int[] temp = new int[n];
			List<T[]> list = new List<T[]>();
			GetCombination<T>(ref list, t, t.Length, n, temp, n);
			return list;
		}

		/// <summary>
		/// 求数组中n个元素的排列
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t">原始数组</param>
		/// <param name="n">排列个数</param>
		/// <returns></returns>
		public List<T[]> GetPermutation<T>(T[] t, int n)
		{
			if (n > t.Length)
			{
				return null;
			}

			List<T[]> ts = new List<T[]>();
			List<T[]> c = GetCombination<T>(t, n);
			for (int index = 0; index < c.Count; index++)
			{
				List<T[]> l = new List<T[]>();
				GetPermutation<T>(ref l, c[index], 0, n - 1);
				ts.AddRange(l);
			}

			return ts;
		}

		/// <summary>
		/// 返回全排列
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <returns></returns>
		public List<T[]> GetPermutation<T>(T[] t)
		{
			return GetPermutation<T>(t, 0, t.Length);
		}

		/// <summary>
		/// 返回起始标号到结束标号的排列
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <returns></returns>
		public List<T[]> GetPermutation<T>(T[] t, int startIndex, int endIndex)
		{
			if (startIndex < 0 || endIndex > t.Length)
			{
				return null;
			}

			if (startIndex > endIndex)
			{
				Swap<int>(ref startIndex, ref endIndex);
			}

			List<T[]> list = new List<T[]>();
			GetPermutation<T>(ref list, t, startIndex, endIndex);
			return list;
		}

		/// <summary>
		/// 交换
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public void Swap<T>(ref T a, ref T b)
		{
			T temp = a;
			a = b;
			b = temp;
		}

		/// <summary>
		/// 不修改数组的反转
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <param name="s"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public T[] Reverse<T>(T[] t, int s, int e)
		{
			T[] rt = new T[t.Length];
			t.CopyTo(rt, 0);
			if (s > e)
			{
				return null;
			}

			if (s < 0 || e > t.Length)
			{
				return null;
			}

			for (; s < e;)
			{
				Swap<T>(ref rt[s], ref rt[e]);
				s++;
				e--;
			}

			return rt;
		}

		/// <summary>
		/// 将数组反转
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t">目标数组</param>
		/// <param name="s">反转起点</param>
		/// <param name="e">反转终点</param>
		public void Reverse<T>(ref T[] t, int s, int e)
		{
			if (s > e)
			{
				return;
			}

			if (s < 0 || e > t.Length)
			{
				return;
			}

			for (; s < e;)
			{
				Swap<T>(ref t[s], ref t[e]);
				s++;
				e--;
			}
		}

		/// <summary>
		/// 求一个排列
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public bool Permutation(ref int[] t, int start, int end, ref int swIndex)
		{
			int i = end - 2;
			while (i >= 0 && t[i] >= t[i + 1])
			{
				i--;
			}

			if (i == -1)
			{
				return false;
			}

			int j = end - 1;
			while (t[j] <= t[i])
			{
				--j;
			}

			Swap<int>(ref t[i], ref t[j]);
			swIndex = i;
			Reverse<int>(ref t, i + 1, end - 1);
			return true;
		}

		/// <summary>
		/// 计算数组是否一致
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public bool CalDB<T>(T[] t1, T[] t2)
		{
			if (t1.Length != t2.Length)
			{
				return false;
			}

			for (int index = 0; index < t1.Length; index++)
			{
				if (!t1[index].Equals(t2[index]))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 删除重复数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ts"></param>
		/// <param name="t"></param>
		public void DelCF<T>(ref List<T[]> ts, T[] t, int start)
		{
			while (start < ts.Count)
			{
				T[] temp = ts[start];
				if (CalDB<T>(temp, t))
				{
					ts.RemoveAt(start);
				}
				else
				{
					start++;
				}
			}
		}
	}
}
