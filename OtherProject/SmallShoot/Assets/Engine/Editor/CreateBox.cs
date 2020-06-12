/*
 * Creator:ffm
 * Desc:动态生成包围盒
 * Time:2020/4/13 14:03:47
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEditor;

public class CreateBox : ObjectBase
{
	[MenuItem("Tools/CreateBox")]
	private static void CreateBoxCollider()
	{
		GameObject select = Selection.activeGameObject;
		if (select == null)
		{
			Debug.LogError("please select gameobject.");
			return;
		}

		Vector3 position = select.transform.position;
		Quaternion rotation = select.transform.rotation;
		Vector3 scale = select.transform.localScale;

		select.transform.position = Vector3.zero;
		select.transform.rotation = Quaternion.Euler(Vector3.zero);
		select.transform.localScale = Vector3.one;

		Collider[] colliders = select.GetComponentsInChildren<Collider>();
		foreach (Collider child in colliders)
		{
			DestroyImmediate(child);
		}

		Vector3 center = Vector3.zero;
		Renderer[] renders = select.GetComponentsInChildren<Renderer>();
		foreach (Renderer child in renders)
		{
			center += child.bounds.center;
		}

		center /= renders.Length;
		Bounds bounds = new Bounds(center, Vector3.zero);
		foreach (Renderer child in renders)
		{
			bounds.Encapsulate(child.bounds);
		}

		BoxCollider box = select.AddComponent<BoxCollider>();
		box.center = center;
		box.size = bounds.size;

		select.transform.position = position;
		select.transform.rotation = rotation;
		select.transform.localScale = scale;
	}
}
