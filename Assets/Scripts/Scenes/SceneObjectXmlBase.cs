/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:场景配置文件
 * Time:2020/7/14 17:03:14
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using System.IO;

public class SceneObjectXmlBase : XmlBase
{
	public class MRInfo
	{
		public string m_NodeName;
		public int m_Index;
		public Vector4 m_OffectScale;
	}

	public class PrefabMRInfo
	{
		public string m_PrefabName;
		public List<MRInfo> m_Childs;
	}

	private Dictionary<string, PrefabMRInfo> m_AllSceneNames;

	public SceneObjectXmlBase(string name) : base(name)
	{
		m_AllSceneNames = new Dictionary<string, PrefabMRInfo>();
		m_AllSceneNames.Clear();
	}

	public override string GetXmlPath()
	{
		if (ResObjectManager.Instance.IsUseAB)
		{
			return Application.persistentDataPath + "/Scene/" + m_XmlName + ".bytes";
		}

		return Application.streamingAssetsPath + "/Scene/" + m_XmlName + ".bytes";
	}

	public override bool LoadBinary(BinaryReader reader)
	{
		if (!base.LoadBinary(reader))
			return false;

		try
		{
			string name = reader.ReadString();
			PrefabMRInfo prefabMRInfo = new PrefabMRInfo();
			prefabMRInfo.m_PrefabName = name;
			prefabMRInfo.m_Childs = new List<MRInfo>();
			int cout = reader.ReadInt32();
			for (int index = 0; index < cout; index++)
			{
				MRInfo info = new MRInfo();
				info.m_NodeName = reader.ReadString();
				info.m_Index = reader.ReadInt32();
				info.m_OffectScale = new Vector4(0, 0, 0, 0);
				info.m_OffectScale.x = reader.ReadSingle();
				info.m_OffectScale.y = reader.ReadSingle();
				info.m_OffectScale.z = reader.ReadSingle();
				info.m_OffectScale.w = reader.ReadSingle();
				prefabMRInfo.m_Childs.Add(info);
			}

			m_AllSceneNames.Add(name, prefabMRInfo);
		}
		catch
		{
			return true;
		}

		return true;
	}

	public PrefabMRInfo GetInfoData(string name)
	{
		if (m_AllSceneNames.ContainsKey(name))
		{
			return m_AllSceneNames[name];
		}

		return null;
	}
}
