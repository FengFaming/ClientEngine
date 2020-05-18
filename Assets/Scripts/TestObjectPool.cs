/*
 * Creator:ffm
 * Desc:测试对象池
 * Time:2020/1/19 16:35:37
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class TestObjectPool : IObjectPool
{
	public TestObjectPool(string name) : base(name)
	{

	}

	protected override ObjectPoolControl CloneObject(ObjectPoolControl oc)
	{
		return oc;
	}

	protected override void DestroyObject(ObjectPoolControl oc)
	{

	}

	protected override void InitlizeObject(ObjectPoolControl oc)
	{

	}
}
