/*
 * Creator:ffm
 * Desc:Text页面重新布局
 * Time:3/6/2019 9:48:47 AM
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Game.Engine;

[CustomEditor(typeof(UText), true)]
[CanEditMultipleObjects]
public class AdaptiveTextEditor : UnityEditor.UI.TextEditor
{
	SerializedProperty m_DescKey;

	protected override void OnEnable()
	{
		base.OnEnable();
		m_DescKey = serializedObject.FindProperty("m_DescKey");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();
		EditorGUILayout.PropertyField(m_DescKey);
		serializedObject.ApplyModifiedProperties();
	}
}
