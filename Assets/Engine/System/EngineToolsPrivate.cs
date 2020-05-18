/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:private tools
 * Time:2020/4/22 9:43:46
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public partial class EngineTools : Singleton<EngineTools>
	{
		private bool CheckIDCard18(string id)
		{
			long n = 0;
			if (long.TryParse(id.Remove(17), out n) == false ||
				n < Math.Pow(10, 16) ||
				long.TryParse(id.Replace('x', '0').Replace('X', '0'), out n) == false)
			{
				///里面包含非数字或者首位不大于零
				return false;
			}

			string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
			if (address.IndexOf(id.Remove(2)) == -1)
			{
				///省份验证不通过
				return false;
			}

			string birth = id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
			DateTime time = new DateTime();
			if (DateTime.TryParse(birth, out time) == false)
			{
				///生日验证不通过
				return false;
			}

			string[] arrvarifycode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
			string[] wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
			char[] ai = id.Remove(17).ToCharArray();
			int sum = 0;
			for (int index = 0; index < 17; index++)
			{
				sum += int.Parse(wi[index]) * int.Parse(ai[index].ToString());
			}

			int y = -1;
			Math.DivRem(sum, 11, out y);
			if (arrvarifycode[y] != id.Substring(17, 1).ToLower())
			{
				///校验码不正确
				return false;
			}

			///身份证符合GB1643-1999标准
			return true;
		}

		/// <summary>
		/// 克隆单个对象
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="go"></param>
		/// <param name="position"></param>
		private void SetGameObject(Transform parent, GameObject go, Vector3 position)
		{
			GameObject clone = GameObject.Instantiate(go);
			clone.transform.SetParent(parent);
			clone.transform.position = position;
			clone.transform.rotation = Quaternion.Euler(Vector3.zero);
			clone.transform.localScale = Vector3.one;
		}
	}
}
