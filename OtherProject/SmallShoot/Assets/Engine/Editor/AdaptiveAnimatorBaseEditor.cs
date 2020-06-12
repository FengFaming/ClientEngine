/*
 * Creator:ffm
 * Desc:自定义动画机类显示内容
 *			主要的作用是更好的用于调试动画状态机
 * Time:2020/1/8 16:17:33
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Engine;

[CustomEditor(typeof(AnimatorBase), true)]
public class AdaptiveAnimatorBaseEditor : Editor
{
	private AnimatorBase m_Target;

	private void OnEnable()
	{
		m_Target = target as AnimatorBase;
	}

	public override void OnInspectorGUI()
	{
		///设置为垂直布局
		EditorGUILayout.BeginVertical();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("States:");
		if (m_Target.m_AllStateDic != null)
		{
			EditorGUILayout.IntField("Size:", m_Target.m_AllStateDic.Count);
			foreach (KeyValuePair<string, AnimationStateBase> item in m_Target.m_AllStateDic)
			{
				EditorGUILayout.BeginHorizontal();
				AnimationStateBase ab = item.Value;
				EditorGUILayout.LabelField("StateName:", ab.StateName);
				EditorGUILayout.EndHorizontal();
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("StateParameters");
		if (m_Target.m_AllParameters != null)
		{
			EditorGUILayout.IntField("Size:", m_Target.m_AllParameters.Count);
			for (int index = 0; index < m_Target.m_AllParameters.Count; index++)
			{
				EditorGUILayout.BeginHorizontal();
				MyAnimatorParameters mcp = m_Target.m_AllParameters[index];
				EditorGUILayout.EnumFlagsField(mcp.type, GUILayout.Width(80));
				EditorGUILayout.LabelField(mcp.name, GUILayout.Width(80));
				switch (mcp.type)
				{
					case AnimatorControllerParameterType.Bool:
						EditorGUILayout.Toggle(mcp.defaultBool, GUILayout.Width(50));
						break;
					case AnimatorControllerParameterType.Float:
						EditorGUILayout.FloatField(mcp.defaultFloat, GUILayout.Width(50));
						break;
					case AnimatorControllerParameterType.Int:
						EditorGUILayout.IntField(mcp.defaultInt, GUILayout.Width(50));
						break;
					case AnimatorControllerParameterType.Trigger:
						EditorGUILayout.Toggle(mcp.defaultBool, GUILayout.Width(50));
						break;
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		else
		{
			EditorGUILayout.IntField("Size:", 0);
		}

		EditorGUILayout.EndVertical();
	}
}
