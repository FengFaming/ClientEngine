/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:测试光照场景
 * Time:2020/7/15 10:37:34
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class TestLightmapScene : ISceneWithLightmap
{
	public TestLightmapScene(string name) : base(name)
	{

	}

	protected override void LoadLightmapEnd(string name, object t)
	{
		Light light = GameObject.FindObjectOfType<Light>();
		light.cullingMask = light.cullingMask & ~(1 << LayerMask.NameToLayer("NoLight"));
		base.LoadLightmapEnd(name, t);

		UIManager.Instance.OpenUI("UIPnlBackGameMain", UILayer.Pnl);
		UIManager.Instance.OpenUI("UIPnlTestLightmap", UILayer.Pnl);

		ResObjectCallBackBase cb = new ResObjectCallBackBase();
		cb.m_LoadType = ResObjectType.GameObject;
		cb.m_FinshFunction = LoadPPEnd;
		ResObjectManager.Instance.LoadObject("tt1", ResObjectType.GameObject, cb);
	}

	private void LoadPPEnd(object t)
	{
		GameObject go = t as GameObject;
		go.AddComponent<TestLightmapData>();
		go.layer = LayerMask.NameToLayer("NoLight");
		go.SetActive(true);
		//SceneLightmapManager.Instance.SetSceneLightmap();
	}
}
