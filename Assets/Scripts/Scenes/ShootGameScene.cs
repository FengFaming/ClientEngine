/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:射击游戏场景
 * Time:2020/4/27 13:49:19
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public partial class ShootGameScene : IScene
{
	private ShootGameControl m_LoadControl;

	public ShootGameScene() : base("ShootGame")
	{
	}

	/// <summary>
	/// 初始化场景数据
	/// </summary>
	public override bool InitScene()
	{
		if (!base.InitScene())
			return false;

		GameObject scene = new GameObject();
		scene.name = m_SceneName;
		scene.transform.position = Vector3.zero;
		scene.transform.rotation = Quaternion.Euler(Vector3.zero);
		scene.transform.localScale = Vector3.one;
		m_LoadControl = scene.AddComponent<ShootGameControl>();
		return true;
	}

	/// <summary>
	/// 清理场景数据
	/// </summary>
	public override void ClearSceneData()
	{
		base.ClearSceneData();
		if (m_LoadControl != null)
		{
			m_LoadControl.ClearSceneData();
		}
	}

	/// <summary>
	/// 读取场景内容
	/// </summary>
	/// <param name="action"></param>
	public override void LoadScene(Action<float> action)
	{
		if (m_LoadControl != null)
		{
			m_LoadControl.LoadScene(action);
		}
	}

	/// <summary>
	/// 清空场景
	/// </summary>
	/// <param name="action"></param>
	public override void DestroyScene(Action<float> action)
	{
		m_LoadControl.EndScene(action);
	}
}
