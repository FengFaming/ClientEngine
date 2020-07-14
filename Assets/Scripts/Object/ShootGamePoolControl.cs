/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:射击游戏对象池
 * Time:2020/4/28 9:30:10
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

public class ShootGamePoolControl : IObjectPool
{
	public ShootGamePoolControl(string name) : base(name)
	{

	}

	public string PoolName { get { return m_PoolName; } }

	private Transform m_Parent;

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

		(noumenon as ShootGameObjectControl).m_Target.transform.parent = m_Parent;
	}

	/// <summary>
	/// 初始化一个实例对象
	///		注意，并不是实例化
	/// </summary>
	/// <param name="oc"></param>
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
		go.transform.parent = m_Parent;
		go.gameObject.transform.position = Vector3.zero;
		go.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
		go.gameObject.transform.localScale = Vector3.one;
		go.SetActive(false);
	}

	/// <summary>
	/// 克隆一个对象
	///		也就是实例化一个对象
	/// </summary>
	/// <param name="oc"></param>
	/// <returns></returns>
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
