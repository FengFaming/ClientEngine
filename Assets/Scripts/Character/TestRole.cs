/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:测试角色
 * Time:2020/7/31 15:19:24
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

#region Editor
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(TestRole))]
public class TestRoleEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (GUI.changed)
		{
			TestRole testRole = target as TestRole;
			testRole.RoleAnimationManager.ChangeAnimationSpeed(testRole.m_Sp);
		}
	}
}

#endif
#endregion

public class TestRole : IRole
{
	public GameObject m_Prant;

	public List<GameObject> m_Skinneds;

	[Range(0, 10)]
	public float m_Sp;

#if UNITY_EDITOR
	public RoleAnimationManager RoleAnimationManager { get { return m_RoleAnimationManager; } }
#endif

	protected override void Awake()
	{
		base.Awake();
		m_RoleUID = 1;

		RoleCombineMeshControl role = new RoleCombineMeshControl();
		role.CombineMesh(m_Skinneds, ref m_Prant);

		RoleAnimationManager animation = new RoleAnimationManager(this);
		Animation a = m_Prant.GetComponent<Animation>();
		foreach (AnimationState state in a)
		{
			IRoleAnimation ira = new IRoleAnimation(state.name);
			animation.AddRoleAnimation(ira);
			Debug.Log(state.clip.length);
		}

		InitRole(1, animation);
		m_RoleAnimationManager.Play("run", m_Sp, true);
	}
}
