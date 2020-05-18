/*
 * Creator:ffm
 * Desc:配置文件管理器
 * Time:2020/4/6 10:01:59
* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Game.Engine
{
	/// <summary>
	/// xml配置文件管理器
	/// 为了让配置文件第一时间加载并且无差别的读取
	///		配置文件不走资源包，直接加载
	/// </summary>
	public class ConfigurationManager : Singleton<ConfigurationManager>
	{
		/// <summary>
		/// 所有的配置文件
		/// </summary>
		private Dictionary<string, XmlBase> m_AllXml;

		/// <summary>
		/// 所有不会被清理的文件
		/// </summary>
		private List<string> m_DontClearXml;

		public ConfigurationManager()
		{
			m_AllXml = new Dictionary<string, XmlBase>();
			m_AllXml.Clear();

			m_DontClearXml = new List<string>();
			m_DontClearXml.Clear();
		}

		/// <summary>
		/// 读取配置文件
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="isForceLoad"></param>
		/// <param name="isSave"></param>
		public void LoadXml<T>(ref T xml, bool isForceLoad = false, bool isSave = false) where T : XmlBase
		{
			if (isForceLoad)
			{
				if (m_AllXml.ContainsKey(xml.XmlName))
				{
					m_AllXml.Remove(xml.XmlName);
				}
			}

			if (m_AllXml.ContainsKey(xml.XmlName))
			{
				xml = m_AllXml[xml.XmlName] as T;
				return;
			}
			else
			{
				XmlDocument doc = new XmlDocument();
				string path = xml.GetXmlPath();
				doc.Load(File.OpenText(path));

				if (doc != null)
				{
					XmlElement element = doc.DocumentElement;
					xml.LoadXml(element);
				}
			}

			if (isSave)
			{
				m_AllXml.Add(xml.XmlName, xml);
			}
		}

		/// <summary>
		/// 获取文件
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public XmlBase GetXml(string name)
		{
			if (m_AllXml.ContainsKey(name))
			{
				return m_AllXml[name];
			}

			return null;
		}

		/// <summary>
		/// 添加不清理的文件内容
		/// </summary>
		/// <param name="xml"></param>
		public void AddDontClearXml(XmlBase xml)
		{
			if (m_AllXml.ContainsKey(xml.XmlName))
			{
				if (!m_DontClearXml.Contains(xml.XmlName))
				{
					m_DontClearXml.Add(xml.XmlName);
				}
			}
		}

		/// <summary>
		/// 清理所有的配置文件
		/// </summary>
		public void ClearXml()
		{
			List<string> cn = new List<string>();
			cn.Clear();

			foreach (KeyValuePair<string, XmlBase> item in m_AllXml)
			{
				if (!m_DontClearXml.Contains(item.Key))
				{
					cn.Add(item.Key);
				}
			}

			for (int index = 0; index < cn.Count; index++)
			{
				m_AllXml.Remove(cn[index]);
			}
		}

		/// <summary>
		/// 清理单一的配置文件
		/// </summary>
		/// <param name="xml"></param>
		public void ClearOneXml(XmlBase xml)
		{
			if (m_AllXml.ContainsKey(xml.XmlName))
			{
				m_AllXml.Remove(xml.XmlName);
			}

			if (m_DontClearXml.Contains(xml.XmlName))
			{
				m_DontClearXml.Remove(xml.XmlName);
			}
		}
	}
}
