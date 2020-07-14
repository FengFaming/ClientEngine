/*
 * Creator:ffm
 * Desc:配置文件木本
 * Time:2020/4/6 10:03:45
* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Xml.Serialization;

namespace Game.Engine
{
	public class XmlBase
	{
		/// <summary>
		/// xml文件头
		/// </summary>
		protected XmlElement m_DocNode;

		/// <summary>
		/// 文件名字
		/// </summary>
		protected string m_XmlName;
		public string XmlName { get { return m_XmlName; } }

		public XmlBase(string name)
		{
			m_XmlName = name;
		}

		public virtual string GetXmlPath()
		{
			if (ResObjectManager.Instance.IsUseAB)
			{
				return Application.persistentDataPath + "/" + m_XmlName + ".xml";
			}

			return Application.streamingAssetsPath + "/" + m_XmlName + ".xml";
		}

		/// <summary>
		/// 读取配置文件
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public virtual bool LoadXml(XmlElement node)
		{
			if (node == null)
			{
				return false;
			}

			m_DocNode = node;
			return true;
		}

		/// <summary>
		/// 读取二进制文件
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public virtual bool LoadBinary(BinaryReader reader)
		{
			if (reader == null)
			{
				return false;
			}

			return true;
		}

		public override string ToString()
		{
			string str = base.ToString();
			str += "\n" + m_XmlName;
			return str;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
