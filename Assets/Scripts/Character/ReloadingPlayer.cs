/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:换装主角
 * Time:2020/5/11 14:25:55
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class ReloadingPlayer : GameCharacterBase
{
	/// <summary>
	/// 加载回调
	/// </summary>
	public class LoadReloadingCharacter : IResObjectCallBack
	{
		private Action<object, Transform, Vector3, Vector3, Vector3, string, bool> m_LoadAction;
		public Vector3 m_Position, m_Rotation, m_Scale;
		public Transform m_Parent;
		public string m_Key;
		public bool m_IsEnd;

		public LoadReloadingCharacter(Action<object, Transform, Vector3, Vector3, Vector3, string, bool> load) : base()
		{
			m_LoadAction = load;
		}

		public override void HandleLoadCallBack(object t)
		{
			if (m_LoadAction != null)
			{
				m_LoadAction(t, m_Parent, m_Position, m_Rotation, m_Scale, m_Key, m_IsEnd);
			}
		}

		public override int LoadCallbackPriority()
		{
			return 0;
		}
	}

	/// <summary>
	/// shader名称
	/// </summary>
	private string m_ShowMaterial;
	public string ShowMaterial { set { m_ShowMaterial = value; } }

	private List<CharacterXmlControl.LoadInfo> m_LoadInfos;
	public List<CharacterXmlControl.LoadInfo> LoadInfos { set { m_LoadInfos = value; } }

	private Action<object> m_LoadEnd;
	private int m_Cout;

	/// <summary>
	/// 角色第一节点
	/// </summary>
	private Transform m_PlayerFirst;
	private Dictionary<string, GameObject> m_LoadGameObjects;

	private void LoadEnd(object t)
	{
		Transform p = this.gameObject.transform.GetChild(0);
		m_PlayerFirst = p;
		m_Cout = 0;
		m_LoadGameObjects = new Dictionary<string, GameObject>();
		m_LoadGameObjects.Clear();
		for (int index = 0; index < m_LoadInfos.Count; index++)
		{
			LoadReloadingCharacter load = new LoadReloadingCharacter(LoadObject);
			load.m_Position = m_LoadInfos[index].m_Position;
			load.m_Rotation = m_LoadInfos[index].m_Rotation;
			load.m_Scale = m_LoadInfos[index].m_Scale;
			load.m_Parent = m_PlayerFirst;
			load.m_Key = m_LoadInfos[index].m_Key;
			load.m_IsEnd = false;
			ResObjectManager.Instance.LoadObject(m_LoadInfos[index].m_Model, ResObjectType.GameObject, load);
		}
	}

	private void LoadObject(object target, Transform parent, Vector3 p, Vector3 r, Vector3 s, string key, bool isEnd)
	{
		GameObject go = target as GameObject;
		go.transform.parent = parent;
		go.transform.localPosition = p;
		go.transform.localRotation = Quaternion.Euler(r);
		go.transform.localScale = s;

		if (m_LoadGameObjects.ContainsKey(key))
		{
			GameObject t = m_LoadGameObjects[key];
			m_LoadGameObjects.Remove(key);
			GameObject.DestroyImmediate(t);
		}

		m_LoadGameObjects.Add(key, go);

		if (!isEnd)
		{
			m_Cout++;
			if (m_Cout >= m_LoadInfos.Count)
			{
				m_LoadEnd(this);
				CombineObject();
			}
		}
		else
		{
			CombineObject();
		}
	}

	protected override void LoadObject(object target, Action<object> end)
	{
		GameObject t = target as GameObject;
		t.transform.parent = this.gameObject.transform;
		t.transform.localPosition = Vector3.zero;
		t.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
		t.transform.localScale = Vector3.one;

		if (end != null)
		{
			end(this);
		}
	}

	public override void StartInitCharacter(string name, Action<object> end)
	{
		m_LoadEnd = end;
		base.StartInitCharacter(name, LoadEnd);
	}

	public override void InitCharacter(GameCharacterCameraBase gameCharacterCameraBase = null,
										GameCharacterAttributeBase gameCharacterAttributeBase = null,
										GameCharacterAnimatorBase animatorBase = null,
										GameCharacterStateManager gameCharacterStateManager = null,
										CharacterMountControl characterMountControl = null)
	{
		animatorBase = new GameCharacterAnimationControl(m_PlayerFirst.gameObject);
		base.InitCharacter(gameCharacterCameraBase, gameCharacterAttributeBase, animatorBase, gameCharacterStateManager, characterMountControl);

		m_CharacterStateManager.TryGotoState(0);
	}

	/// <summary>
	/// 更换模型
	/// </summary>
	/// <param name="info"></param>
	public void ChangeReloading(CharacterXmlControl.LoadInfo info)
	{
		LoadReloadingCharacter load = new LoadReloadingCharacter(LoadObject);
		load.m_Position = info.m_Position;
		load.m_Rotation = info.m_Rotation;
		load.m_Scale = info.m_Scale;
		load.m_Parent = m_PlayerFirst;
		load.m_Key = info.m_Key;
		load.m_IsEnd = true;
		ResObjectManager.Instance.LoadObject(info.m_Model, ResObjectType.GameObject, load);
	}

	private const int COMBINE_TEXTURE_MAX = 512;
	private const string COMBINE_DIFFUSE_TEXTURE = "_MainTex";

	private void CombineObject()
	{
		List<SkinnedMeshRenderer> rends = new List<SkinnedMeshRenderer>();
		foreach (KeyValuePair<string, GameObject> go in m_LoadGameObjects)
		{
			rends.Add(go.Value.GetComponentInChildren<SkinnedMeshRenderer>());
			go.Value.gameObject.SetActive(false);
		}

		CombineObject(m_PlayerFirst.gameObject, rends.ToArray(), true);
	}

	/// <summary>
	/// 合并mesh
	/// </summary>
	/// <param name="skeleton"></param>
	/// <param name="meshes"></param>
	/// <param name="combine"></param>
	public void CombineObject(GameObject skeleton, SkinnedMeshRenderer[] meshes, bool combine = false)
	{
		List<Transform> transforms = new List<Transform>();
		transforms.AddRange(skeleton.GetComponentsInChildren<Transform>(true));

		List<Material> materials = new List<Material>();
		List<CombineInstance> combineInstances = new List<CombineInstance>();//the list of meshes
		List<Transform> bones = new List<Transform>();

		List<Vector2[]> oldUV = null;
		Material newMaterial = null;
		Texture2D newDiffuseTex = null;

		for (int i = 0; i < meshes.Length; i++)
		{
			SkinnedMeshRenderer smr = meshes[i];
			materials.AddRange(smr.materials);
			for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
			{
				CombineInstance ci = new CombineInstance();
				ci.mesh = smr.sharedMesh;
				ci.subMeshIndex = sub;
				combineInstances.Add(ci);
			}

			for (int j = 0; j < smr.bones.Length; j++)
			{
				int tBase = 0;
				for (tBase = 0; tBase < transforms.Count; tBase++)
				{
					if (smr.bones[j].name.Equals(transforms[tBase].name))
					{
						bones.Add(transforms[tBase]);
						break;
					}
				}
			}
		}

		if (combine)
		{
			/*
			 * 特别注意的地方
			 * newMaterial = new Material(Shader.Find("Mobile/Diffuse"));
			 * 使用这个代码，除非再编辑器上和移动平台上运行，
			 * 否则就会出现问题，导致异常而无法往下运行
			 * */
			newMaterial = new Material(Shader.Find(m_ShowMaterial));
			oldUV = new List<Vector2[]>();
			List<Texture2D> Textures = new List<Texture2D>();
			for (int i = 0; i < materials.Count; i++)
			{
				Textures.Add(materials[i].GetTexture(COMBINE_DIFFUSE_TEXTURE) as Texture2D);
			}

			newDiffuseTex = new Texture2D(COMBINE_TEXTURE_MAX, COMBINE_TEXTURE_MAX, TextureFormat.RGBA32, true);
			Rect[] uvs = newDiffuseTex.PackTextures(Textures.ToArray(), 0);
			newMaterial.mainTexture = newDiffuseTex;

			Vector2[] uva, uvb;
			for (int j = 0; j < combineInstances.Count; j++)
			{
				uva = (Vector2[])(combineInstances[j].mesh.uv);
				uvb = new Vector2[uva.Length];
				for (int k = 0; k < uva.Length; k++)
				{
					uvb[k] = new Vector2((uva[k].x * uvs[j].width) + uvs[j].x, (uva[k].y * uvs[j].height) + uvs[j].y);
				}

				oldUV.Add(combineInstances[j].mesh.uv);
				combineInstances[j].mesh.uv = uvb;
			}
		}

		SkinnedMeshRenderer oldSKinned = skeleton.GetComponent<SkinnedMeshRenderer>();
		if (oldSKinned != null)
		{
			GameObject.DestroyImmediate(oldSKinned);
		}

		SkinnedMeshRenderer r = skeleton.AddComponent<SkinnedMeshRenderer>();
		r.sharedMesh = new Mesh();
		r.sharedMesh.CombineMeshes(combineInstances.ToArray(), combine, false);
		r.bones = bones.ToArray();
		if (combine)
		{
			r.material = newMaterial;
			for (int i = 0; i < combineInstances.Count; i++)
			{
				combineInstances[i].mesh.uv = oldUV[i];
			}
		}
		else
		{
			r.materials = materials.ToArray();
		}
	}
}
