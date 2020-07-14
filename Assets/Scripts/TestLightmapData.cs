/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:测试光照数据
 * Time:2020/7/14 17:16:03
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class TestLightmapData : ObjectBase
{
	private void Start()
	{
		SceneObjectXmlBase sceneData = new SceneObjectXmlBase("animatortest");
		ConfigurationManager.Instance.LoadBinaryConfig<SceneObjectXmlBase>(ref sceneData, false, true);
		string name = this.gameObject.name;
		if (name.LastIndexOf("Clone") > 0)
		{
			name = name.Substring(0, name.LastIndexOf("Clone") - 1);
		}

		Debug.Log(name);

		SceneObjectXmlBase.PrefabMRInfo info = sceneData.GetInfoData(name);
		if (info != null)
		{
			MeshRenderer[] renderers = this.gameObject.GetComponentsInChildren<MeshRenderer>();
			for (int index = 0; index < renderers.Length; index++)
			{
				string cn = renderers[index].gameObject.name;
				if (cn.LastIndexOf("Clone") > 0)
				{
					cn = cn.Substring(0, cn.LastIndexOf("Clone") - 1);
				}

				for (int i = 0; i < info.m_Childs.Count; i++)
				{
					if (info.m_Childs[i].m_NodeName == cn)
					{
						renderers[index].lightmapIndex = info.m_Childs[i].m_Index;
						renderers[index].lightmapScaleOffset = info.m_Childs[i].m_OffectScale;
						break;
					}
				}
			}
		}
	}
}
