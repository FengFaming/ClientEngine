/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:创建雨滴
 * Time:2020/6/20 14:43:21
* */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 创建成片的雨滴
/// </summary>
public class CreateRainMesh : EditorWindow
{
	/// <summary>
	/// 面片大小
	/// </summary>
	public int m_NumberOfParticles = 200;

	/// <summary>
	/// 位置X最大值
	/// </summary>
	public float m_AreaSize = 40.0f;

	/// <summary>
	/// 位置Z最大值
	/// </summary>
	public float m_AreaHeight = 15.0f;

	/// <summary>
	/// 左右偏差
	/// </summary>
	public float m_ParticleSize = 0.2f;

	/// <summary>
	/// 上下偏差
	/// </summary>
	public float m_FlakeRandom = 0.1f;

	/// <summary>
	/// 保存名字
	/// </summary>
	public string m_SaveName;

	[MenuItem("Tools/CreateRain")]
	private static void Create()
	{
		EditorWindow.GetWindow(typeof(CreateRainMesh));
	}

	private void OnGUI()
	{
		m_SaveName = EditorGUILayout.TextField("Save Name:", m_SaveName);
		EditorGUILayout.LabelField("Save Path:", "Assets/Art/Mesh/" + m_SaveName + ".asset");
		if (GUILayout.Button("Create"))
		{
			Rain();
		}
	}

	private void Rain()
	{
		Mesh mesh = CreateMesh();
		AssetDatabase.CreateAsset(mesh, "Assets/Art/Mesh/" + m_SaveName + ".asset");
	}

	private Mesh CreateMesh()
	{
		var mesh = new Mesh();

		Vector3 cameraRight = Camera.main.transform.right;
		Vector3 cameraUp = (Vector3.up);

		int particleNum = m_NumberOfParticles;
		Vector3[] verts = new Vector3[4 * particleNum];
		Vector2[] uvs = new Vector2[4 * particleNum];
		Vector2[] uvs2 = new Vector2[4 * particleNum];
		Vector3[] normals = new Vector3[4 * particleNum];

		int[] tris = new int[2 * 3 * particleNum];
		Vector3 position;
		for (int i = 0; i < particleNum; i++)
		{
			int i4 = i * 4;
			int i6 = i * 6;

			position.x = m_AreaSize * (Random.value - 0.5f);
			position.y = m_AreaHeight * Random.value;
			position.z = m_AreaSize * (Random.value - 0.5f);

			float rand = Random.value;
			float widthWithRandom = m_ParticleSize * 0.215f;// + rand * m_FlakeRandom;
			float heightWithRandom = m_ParticleSize + rand * m_FlakeRandom;

			verts[i4 + 0] = position - cameraRight * widthWithRandom - cameraUp * heightWithRandom;
			verts[i4 + 1] = position + cameraRight * widthWithRandom - cameraUp * heightWithRandom;
			verts[i4 + 2] = position + cameraRight * widthWithRandom + cameraUp * heightWithRandom;
			verts[i4 + 3] = position - cameraRight * widthWithRandom + cameraUp * heightWithRandom;

			normals[i4 + 0] = -Camera.main.transform.forward;
			normals[i4 + 1] = -Camera.main.transform.forward;
			normals[i4 + 2] = -Camera.main.transform.forward;
			normals[i4 + 3] = -Camera.main.transform.forward;

			uvs[i4 + 0] = new Vector2(0.0f, 0.0f);
			uvs[i4 + 1] = new Vector2(1.0f, 0.0f);
			uvs[i4 + 2] = new Vector2(1.0f, 1.0f);
			uvs[i4 + 3] = new Vector2(0.0f, 1.0f);

			uvs2[i4 + 0] = new Vector2(Random.Range(-2, 2) * 4.0f, Random.Range(-1, 1) * 1.0f);
			uvs2[i4 + 1] = new Vector2(uvs2[i4 + 0].x, uvs2[i4 + 0].y);
			uvs2[i4 + 2] = new Vector2(uvs2[i4 + 0].x, uvs2[i4 + 0].y);
			uvs2[i4 + 3] = new Vector2(uvs2[i4 + 0].x, uvs2[i4 + 0].y);

			tris[i6 + 0] = i4 + 0;
			tris[i6 + 1] = i4 + 1;
			tris[i6 + 2] = i4 + 2;
			tris[i6 + 3] = i4 + 0;
			tris[i6 + 4] = i4 + 2;
			tris[i6 + 5] = i4 + 3;
		}

		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.uv2 = uvs2;
		mesh.RecalculateBounds();

		return mesh;
	}
}
