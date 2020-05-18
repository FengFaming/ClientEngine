/*
 * Creator:ffm
 * Desc:游戏主场景
 * Time:2020/4/13 13:36:35
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class GameMainScene : IScene
{
	public GameMainScene() : base("GameMainScene")
	{

	}

	public override void LoadScene(Action<float> action)
	{
		base.LoadScene(action);
		UIManager.Instance.OpenUI("UIPnlGameMain", UILayer.Pnl);
	}
}
