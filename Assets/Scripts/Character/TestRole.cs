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
		List<AnimationState> state = new List<AnimationState>();
		state.Clear();
		foreach (AnimationState i in a)
		{
			state.Add(i);
		}

		for (int index = 0; index < state.Count; index++)
		{
			TestAnimation test = new TestAnimation(state[index].name);
			//test.ExitAction = PlayerLast;
			int n = index == 0 ? state.Count - 1 : index - 1;
			//test.m_Last = state[n].name;
			animation.AddRoleAnimation(test);

			///生成帧事件
			//AnimationActionEvent aae = new AnimationActionEvent(DebugAnimation, new List<object>() { test.Name, test.Name });
			//AnimationFramActionEventInfo info = new AnimationFramActionEventInfo(state[index].length / 2);
			//info.AddActionEvent(aae);
			//animation.AddRoleAnimationAction(test.Name, info);
		}

		IRoleState rs = new IRoleState(1);
		IRoleState rs1 = new IRoleState(2);
		IRoleState rs2 = new IRoleState(3);
		for (int index = 0; index < state.Count; index++)
		{
			if (index < 3)
			{
				rs.AddAnimation(state[index].name);
			}
			else
			{
				rs1.AddAnimation(state[index].name);
			}
		}

		rs.AnimationManager = animation;
		rs.Loop = false;
		rs.ExitAction = ExitState;
		rs1.AnimationManager = animation;
		rs1.Loop = false;
		rs1.ExitAction = ExitState;
		rs2.Loop = true;
		rs2.AnimationManager = animation;
		rs2.AddAnimation("run");

		IRoleStateManager rsm = new IRoleStateManager();
		rsm.AddState(rs);
		rsm.AddState(rs1);
		rsm.AddState(rs2);
		SetUID(100001);
		IRoleSkill skill = new IRoleSkill(1);
		skill.StateID = 1;

		IRoleSkillManager skillm = new IRoleSkillManager(this);
		skillm.AddSkill(skill);
		skillm.ExitAction = ExitSkillManager;
		InitRole(animation, rsm, skillm);
		m_SkillManager.TrySkill(1);
		//		m_StateManager.StartState(1);
		//m_RoleAnimationManager.Play("run", m_Sp, false);
	}

	private void ExitSkillManager()
	{
		m_StateManager.StartState(3);
	}

	private void ExitState(IObjectState state)
	{
		//if (state.StateID == 1)
		//{
		//	Debug.Log("state 1 -> 2");
		//	m_StateManager.StartState(2);
		//}
		//else
		//{
		//	Debug.Log("state 2 -> 3");
		//	m_StateManager.StartState(3);
		//}
	}

	private void PlayerLast(IRoleAnimation t)
	{
		m_RoleAnimationManager.Play((t as TestAnimation).m_Last);
	}

	private void DebugAnimation(params object[] arms)
	{
		Debug.Log(arms[0]);
		Debug.Log(arms[1]);
	}
}
