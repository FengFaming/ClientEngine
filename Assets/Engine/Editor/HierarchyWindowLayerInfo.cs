/*
 * Creator:ffm
 * Desc:再场景当中显示层级内容
 * Time:7/18/2019 11:40:19 AM
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEditor;

[InitializeOnLoad]
public static class HierarchyWindowLayerInfo
{
	private static readonly int IgnoreLayer = LayerMask.NameToLayer("Default");

	private static readonly GUIStyle style = new GUIStyle()
	{
		fontSize = 9,
		alignment = TextAnchor.MiddleRight
	};

	static HierarchyWindowLayerInfo()
	{
#if UNITY_SHOW_LAYER
		EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
#endif
	}

	private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
	{
		var gameobject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
		if (gameobject != null)
		{
			EditorGUI.LabelField(selectionRect, LayerMask.LayerToName(gameobject.layer), style);
		}
	}
}
