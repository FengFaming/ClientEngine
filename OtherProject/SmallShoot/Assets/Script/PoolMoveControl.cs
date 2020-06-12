/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:子弹和敌人运动
 * Time:2020/6/11 14:42:14
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;

public class PoolMoveControl : ObjectBase
{
	/// <summary>
	/// 控制条件
	/// </summary>
	private RectTransform m_ControlRect;

	/// <summary>
	/// 向上向下
	/// </summary>
	private bool m_IsUpDown;
	public bool IsUpDown { get { return m_IsUpDown; } }

	/// <summary>
	/// 是否开始运动
	/// </summary>
	private bool m_IsMove;

	/// <summary>
	/// 初始位置
	/// </summary>
	private Vector3 m_StartPosition;

	/// <summary>
	/// 分数
	/// </summary>
	private int m_FenZhi;

	/// <summary>
	/// 战力
	/// </summary>
	private int m_ZL;
	public int ZL { get { return m_ZL; } }

	/// <summary>
	/// 运动速度
	/// </summary>
	private float m_MoveSpeed;

	private Text m_ShowFen;

	public void StartMove(bool c, float sp, int fen, int zl)
	{
		if (m_ControlRect == null)
		{
			m_ControlRect = this.gameObject.GetComponent<RectTransform>();
		}

		m_StartPosition = m_ControlRect.localPosition;
		m_IsUpDown = c;
		m_MoveSpeed = sp;
		m_IsMove = true;

		m_MoveSpeed = m_IsUpDown ? m_MoveSpeed : m_MoveSpeed * -1;

		if (m_ControlRect == null)
		{
			m_ControlRect = this.gameObject.GetComponent<RectTransform>();
		}

		m_FenZhi = fen;
		m_ShowFen = null;
		if (!m_IsUpDown)
		{
			m_ShowFen = m_ControlRect.Find("Text").gameObject.GetComponent<Text>();
			m_ShowFen.text = m_FenZhi.ToString();
		}

		m_ZL = zl;
		BoxCollider2D boxCollider2D = this.gameObject.GetComponent<BoxCollider2D>();
		boxCollider2D.enabled = true;
	}

	public void JianFen(int f)
	{
		GameStart.Instance.CalGuoGuan(f);
		m_FenZhi -= f;
		if (m_FenZhi <= 0)
		{
			Recv();
		}
		else
		{
			if (m_ShowFen != null)
			{
				m_ShowFen.text = m_FenZhi.ToString();
			}
		}
	}

	public void Recv()
	{
		GameStart.Instance.Recv(this.gameObject, m_IsUpDown);
		m_IsMove = false;
		m_FenZhi = 0;
		m_ShowFen = null;
		m_IsUpDown = false;
		m_ControlRect = null;
		m_StartPosition = Vector3.zero;
		m_ZL = 0;
		m_MoveSpeed = 0;
	}

	private void Update()
	{
		if (m_IsMove)
		{
			float dt = Time.deltaTime * m_MoveSpeed;
			dt = m_ControlRect.localPosition.y + dt;
			m_ControlRect.localPosition = new Vector3(m_StartPosition.x, dt, m_StartPosition.z);

			if (dt > 400 || dt < -400)
			{
				Recv();
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject != null)
		{
			PoolMoveControl p = collision.gameObject.GetComponent<PoolMoveControl>();
			if (p.IsUpDown != this.IsUpDown)
			{
				if (m_IsUpDown)
				{
					p.JianFen(m_ZL);
					Recv();
				}
				else
				{
					JianFen(p.ZL);
					p.Recv();
				}
			}
		}
	}
}
