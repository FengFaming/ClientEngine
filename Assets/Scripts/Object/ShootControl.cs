/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:子弹控制
 * Time:2020/4/28 10:57:49
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class ShootControl : ObjectBase
{
	public ShootGameObjectControl m_Target;
	public string m_PoolName;

	public void InitMove(Vector3 start, Vector3 end, float time)
	{
		this.gameObject.transform.parent = null;
		GameObjectMoveControl control = this.gameObject.AddComponent<GameObjectMoveControl>();
		this.gameObject.transform.position = start;
		this.gameObject.SetActive(true);
		control.SetMove(end, Vector3.zero, time, MoveEnd);
	}

	private void MoveEnd(bool end)
	{
		GameObjectMoveControl control = this.gameObject.GetComponent<GameObjectMoveControl>();
		GameObject.DestroyImmediate(control);

		ObjectPoolManager.Instance.RecoveryObject(m_PoolName, m_Target.OneObjectData, m_Target);
	}
}
