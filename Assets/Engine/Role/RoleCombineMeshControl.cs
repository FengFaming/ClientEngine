/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:角色合并mesh控制
 * Time:2020/7/31 15:03:56
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class RoleCombineMeshControl
	{
		/// <summary>
		/// 合并mesh
		/// </summary>
		/// <param name="skinneds">需要合并的内容</param>
		/// <param name="parent">合并返回的对象</param>
		public void CombineMesh(List<GameObject> skinneds, ref GameObject parent)
		{
			List<SkinnedMeshRenderer> renderers = new List<SkinnedMeshRenderer>();
			for (int index = 0; index < skinneds.Count; index++)
			{
				renderers.AddRange(skinneds[index].GetComponentsInChildren<SkinnedMeshRenderer>());
				skinneds[index].SetActive(false);
			}

			CombineObject(ref parent, renderers.ToArray(), true);
		}

		/// <summary>
		/// 合并mesh
		/// </summary>
		/// <param name="skeleton"></param>
		/// <param name="meshes"></param>
		/// <param name="combine"></param>
		private void CombineObject(ref GameObject skeleton, SkinnedMeshRenderer[] meshes, bool combine = false)
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
				newMaterial = new Material(Shader.Find("Legacy Shaders/Diffuse"));
				oldUV = new List<Vector2[]>();
				List<Texture2D> Textures = new List<Texture2D>();
				for (int i = 0; i < materials.Count; i++)
				{
					Textures.Add(materials[i].GetTexture("_MainTex") as Texture2D);
				}

				newDiffuseTex = new Texture2D(512, 512, TextureFormat.RGBA32, true);
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
			///重新拷贝生成mesh
			r.sharedMesh.CombineMeshes(combineInstances.ToArray(), combine, false);

			///重新赋予骨骼
			r.bones = bones.ToArray();
			if (combine)
			{
				///如果材质修改了，需要重新计算一个uv
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
}
