#pragma warning disable 0618
/*
 * Creator:ffm
 * Desc:创建草体
 * Time:2020/4/18 16:36:46
* */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateGrass : EditorWindow
{
	[MenuItem("Tools/CreateGrass")]
	private static void Create()
	{
		EditorWindow.GetWindow(typeof(CreateGrass));
	}

	private int m_Width = 1;
	private int m_Len = 1;
	private string m_Padding = "3";
	private GameObject m_Obj;
	private List<Vector3> m_Vertices1;
	private List<int> m_Triangles1;
	private List<Vector2> m_UV1;
	private List<Color> m_Colors1;
	private string m_Times = "50";
	private string m_GrassUnitWidth = "4";
	private string m_GrassUnitHeight = "40";
	private string m_Segment = "5";
	private UnityEngine.Object m_Material = null;

	private void OnDestroy()
	{
		if (m_Obj != null)
		{
			GameObject.DestroyImmediate(m_Obj);
		}
	}

	private void OnGUI()
	{
		GUILayout.Label("Grass Setting", EditorStyles.boldLabel);
		m_Times = EditorGUILayout.TextField("Num", m_Times);
		m_Padding = EditorGUILayout.TextField("Range", m_Padding);
		m_GrassUnitWidth = EditorGUILayout.TextField("grassunitywidth", m_GrassUnitWidth);
		m_Segment = EditorGUILayout.TextField("segment", m_Segment);
		if (m_Material == null)
		{
			m_Material = AssetDatabase.LoadAssetAtPath("Assets/Engine/Shader/grass.mat", typeof(Material));
		}

		m_Material = EditorGUILayout.ObjectField("material", m_Material, typeof(Material));
		if (GUILayout.Button("Create"))
		{
			Grass();
		}

		if (GUILayout.Button("save"))
		{
			if (m_Obj != null)
			{
				if (!Directory.Exists("Assets/Temp/Grass"))
				{
					DirectoryInfo f = new DirectoryInfo("Assets/Temp/Grass");
					f.Create();
				}

				AssetDatabase.CreateAsset(m_Obj.GetComponent<MeshFilter>().sharedMesh, "Assets/Temp/Grass/gmesh.asset");
				PrefabUtility.SaveAsPrefabAsset(m_Obj, "Assets/Temp/Grass/grass.prefab");
			}
		}
	}

	private void Grass()
	{
		if (m_Obj != null)
		{
			if (m_Obj.GetComponent<MeshFilter>() != null)
			{
				GameObject.DestroyImmediate(m_Obj.GetComponent<MeshFilter>().sharedMesh);
			}

			GameObject.DestroyImmediate(m_Obj);
		}

		m_Obj = new GameObject();
		m_Obj.transform.position = Vector3.zero;
		m_Obj.name = "grass";
		Mesh mesh = new Mesh();
		mesh.Clear();
		float paddingF = float.Parse(m_Padding);
		int seg = int.Parse(m_Segment);
		int wNum = m_Width * seg;
		int lNum = m_Len;
		int timesNum = int.Parse(m_Times);
		int vertNum = (wNum + 1) * (lNum + 1) * timesNum * timesNum;
		if (vertNum > 65535)
		{
			Debug.LogError("Vert num can not be more than 65536");
			return;
		}

		m_Vertices1 = new List<Vector3>();
		m_UV1 = new List<Vector2>();
		m_Colors1 = new List<Color>();
		m_Triangles1 = new List<int>();
		int startIndex = 0;
		for (int t = 0; t < timesNum; t++)
		{
			for (int k = 0; k < timesNum; k++)
			{
				Vector3[] vertices = new Vector3[(wNum + 1) * (lNum + 1)];
				Color[] colors = new Color[(wNum + 1) * (lNum + 1)];
				Vector2[] uv = new Vector2[(wNum + 1) * (lNum + 1)];
				int[] triangles = new int[wNum * lNum * 6];
				float padding1 = paddingF / 2;
				Vector3 off = new Vector3(-padding1, 0, -padding1);
				off += new Vector3(paddingF / timesNum * t, 0f, paddingF / timesNum * k);
				Vector3 basePos = off;
				float xr = Random.Range(-paddingF / timesNum, paddingF / timesNum);
				float zr = Random.Range(-paddingF / timesNum, paddingF / timesNum);
				basePos += new Vector3(xr, 0f, zr);
				int index = 0;
				for (int i = 0; i < wNum; i++)
				{
					for (int j = 0; j < lNum; j++)
					{
						int line = lNum + 1;
						int self = j + (i * line) + startIndex;

						triangles[index] = self;
						triangles[index + 1] = self + line + 1;
						triangles[index + 2] = self + line;
						triangles[index + 3] = self;
						triangles[index + 4] = self + 1;
						triangles[index + 5] = self + 1 + line;
						index += 6;
					}
				}

				float anglez = Random.Range(-10f, 10f);
				Vector3 normalDir = Quaternion.Euler(0f, 0f, anglez) * Vector3.up;
				Vector3 rightDir = Quaternion.Euler(0f, 0f, anglez) * Vector3.right;
				float weightR = Random.Range(-float.Parse(m_GrassUnitWidth) / 2f, 0f);
				float lr = Random.Range(0f, 1f);
				Color color = Color.Lerp(new Color(0.6f, 0.9f, 0f, 1f), new Color(1f, 0.6f, 0.4f, 1f), lr);
				float heightR = Random.Range(0.7f, 1.1f);
				for (int j = 0; j < wNum + 1; j++)
				{
					for (int i = 0; i < lNum + 1; i++)
					{
						int line = lNum + 1;
						float w = (weightR + float.Parse(m_GrassUnitWidth)) * i / timesNum;
						if (i == 0)
						{
							w += j * w * 0.9f / seg;
						}
						else
						{
							w -= j * w * 0.9f / seg;
						}
						int index1 = j * line + i;
						vertices[index1] = basePos + (w) * rightDir + (heightR * float.Parse(m_GrassUnitHeight) * j / seg / 70f) * normalDir;
						//vertices[i * line + j] = basePos + Vector3.right * float.Parse(m_GrassUnitWidth) *i / timesNum + float.Parse(m_GrassUnitHeight) * j / timesNum * Vector3.up;
						uv[index1] = new Vector2(1f * i / lNum, 1f * j / wNum);
						colors[index1] = color;
					}
				}

				m_Vertices1.AddRange(vertices);
				m_Colors1.AddRange(colors);
				m_UV1.AddRange(uv);
				m_Triangles1.AddRange(triangles);
				startIndex += vertices.Length;
			}
		}

		mesh.vertices = m_Vertices1.ToArray();
		mesh.triangles = m_Triangles1.ToArray();
		mesh.uv = m_UV1.ToArray();
		mesh.colors = m_Colors1.ToArray();
		Vector3[] normals = new Vector3[mesh.vertices.Length];
		mesh.normals = normals;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		m_Obj.AddComponent<MeshFilter>().sharedMesh = mesh;
		MeshRenderer render = m_Obj.AddComponent<MeshRenderer>();
		//render.sharedMaterial = AssetDatabase.LoadAssetAtPath("Assets/MobileGrass/Material/grass.mat", typeof(Material)) as Material;
		render.sharedMaterial = m_Material as Material;
		render.sharedMaterial.SetFloat("_GrassSeg", seg);
		render.sharedMaterial.SetFloat("_GrassNum", float.Parse(m_Times));
		render.sharedMaterial.SetFloat("_GrassRange", float.Parse(m_Padding));
		GameObject newObj = GameObject.Instantiate(m_Obj);
		newObj.transform.parent = m_Obj.transform;
		newObj.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
		newObj.transform.localPosition = Vector3.zero;
		newObj.transform.localScale = Vector3.one;
	}
}
