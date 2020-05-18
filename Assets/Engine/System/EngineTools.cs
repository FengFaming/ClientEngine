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
	}
}
