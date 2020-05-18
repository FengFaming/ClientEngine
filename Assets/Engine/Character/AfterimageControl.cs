/*
 * Creator:ffm
 * Desc:残影控制
 * Time:2020/4/9 10:47:33
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class AfterimageControl
	{
		/// <summary>
		/// 运动主体
		/// </summary>
		private GameObject m_ControlTarget;

		/// <summary>
		/// 上一个生成时间
		/// </summary>
		private float m_LastTime;

		/// <summary>
		/// 生成时间间隔
		/// </summary>
		private float m_ControlTime;

		/// <summary>
		/// 死亡时间
		/// </summary>
		private float m_DieTime;

		/// <summary>
		/// 是否自动生成
		/// </summary>
		private bool m_IsCopy;

		/// <summary>
		/// 残影控制
		/// </summary>
		private Type m_ControlFun;

		/// <summary>
		/// 残影的材质球
		/// </summary>
		private Shader m_ControlShader;

		/// <summary>
		/// 残影生成
		/// </summary>
		/// <param name="target">残影主体</param>
		/// <param name="imc">残影的变化规则</param>
		/// <param name="shader">残影的变化规则</param>
		/// <param name="deltTime">残影生成的间隔时间</param>
		/// <param name="dieTime">残影存活时间</param>
		public AfterimageControl(GameObject target,
								Type imc = null, Shader shader = null,
								float deltTime = 0, float dieTime = 0)
		{
			m_ControlTarget = target;
			m_ControlTime = deltTime;
			m_DieTime = dieTime;
			m_ControlFun = imc;
			m_ControlShader = shader;

			if (m_ControlTime > 0)
			{
				m_IsCopy = true;
				m_LastTime = Time.time;
				CopyAfterimage(m_ControlFun, m_ControlShader, m_DieTime);
			}
		}

		/// <summary>
		/// 停止复制
		/// </summary>
		public void StopCopy()
		{
			m_IsCopy = false;
			m_ControlTarget = null;
		}

		/// <summary>
		/// 生成一个残影
		/// </summary>
		/// <param name="control">残影控制</param>
		public void CopyAfterimage(Type control, Shader shader, float dieTime)
		{
			if (m_ControlTarget == null)
			{
				Debug.LogError("the target is null.");
				return;
			}

			SkinnedMeshRenderer[] mr = m_ControlTarget.GetComponentsInChildren<SkinnedMeshRenderer>();
			if (mr == null || mr.Length < 1)
			{
				Debug.LogError("the target is not mesh.");
				return;
			}

			GameObject go = new GameObject();
			go.name = m_ControlTarget.name + "afterimage";
			go.transform.position = m_ControlTarget.transform.position;
			go.transform.eulerAngles = m_ControlTarget.transform.eulerAngles;
			go.transform.localScale = m_ControlTarget.transform.localScale;

			for (int index = 0; index < mr.Length; index++)
			{
				Mesh mesh = new Mesh();
				mr[index].BakeMesh(mesh);
				GameObject g = new GameObject();
				g.name = mr[index].name + "afterimage";
				g.transform.parent = go.transform;
				//g.hideFlags = HideFlags.HideAndDontSave;
				MeshFilter mf = g.AddComponent<MeshFilter>();
				mf.mesh = mesh;
				MeshRenderer msr = g.AddComponent<MeshRenderer>();
				msr.material = mr[index].material;
				msr.material.shader = shader;
				g.transform.localPosition = mr[index].transform.localPosition;
				g.transform.localEulerAngles = mr[index].transform.localEulerAngles;
				g.transform.localScale = mr[index].transform.localScale;
			}

			IAfterimageMoveControl ia = go.AddComponent(control) as IAfterimageMoveControl;
			if (ia != null)
			{
				ia.StartMove(dieTime);
			}
		}

		/// <summary>
		/// 计算生成残影
		/// </summary>
		public void LateUpdate()
		{
			if (m_IsCopy)
			{
				if (Time.time - m_LastTime > m_ControlTime)
				{
					m_LastTime = Time.time;
					CopyAfterimage(m_ControlFun, m_ControlShader, m_DieTime);
				}
			}
		}
	}
}
