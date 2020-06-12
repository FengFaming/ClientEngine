/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:游戏对象池
 * Time:2020/6/11 14:51:06
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class ShootGameObjectControl : ObjectPoolControl
{
	public GameObject m_Target;
}

public class GamePool : IObjectPool
{
	private Transform m_Parent;

	public GamePool(string name) : base(name)
	{

	}

	protected override void AddObject(object t, ObjectPoolControl noumenon)
	{
		base.AddObject(t, noumenon);
		(noumenon as ShootGameObjectControl).m_Target.SetActive(false);

		if (m_Parent == null)
		{
			m_Parent = new GameObject().transform;
			m_Parent.name = m_PoolName;
			m_Parent.position = Vector3.zero;
			m_Parent.rotation = Quaternion.Euler(Vector3.zero);
			m_Parent.localScale = Vector3.one;
		}

		(noumenon as ShootGameObjectControl).m_Target.GetComponent<RectTransform>().SetParent(m_Parent);
	}

	protected override void InitlizeObject(ObjectPoolControl oc)
	{
		if (m_Parent == null)
		{
			m_Parent = new GameObject().transform;
			m_Parent.name = m_PoolName;
			m_Parent.position = Vector3.zero;
			m_Parent.rotation = Quaternion.Euler(Vector3.zero);
			m_Parent.localScale = Vector3.one;
		}

		ShootGameObjectControl sc = oc as ShootGameObjectControl;
		GameObject go = sc.m_Target;
		go.GetComponent<RectTransform>().SetParent(m_Parent);
		go.gameObject.transform.position = Vector3.zero;
		go.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
		go.gameObject.transform.localScale = Vector3.one;
		BoxCollider2D boxCollider2D = go.GetComponent<BoxCollider2D>();
		boxCollider2D.enabled = false;

		go.SetActive(false);
	}

	protected override ObjectPoolControl CloneObject(ObjectPoolControl oc)
	{
		ShootGameObjectControl soc = oc as ShootGameObjectControl;
		ShootGameObjectControl clone = new ShootGameObjectControl();
		clone.OneObjectData = soc.OneObjectData;
		clone.m_Target = GameObject.Instantiate(soc.m_Target);
		clone.SaveCrashTime = GameTimeManager.Instance.GameNowTime;
		return clone;
	}

	protected override void DestroyObject(ObjectPoolControl oc)
	{
		ShootGameObjectControl soc = oc as ShootGameObjectControl;
		GameObject.Destroy(soc.m_Target);
	}
}
