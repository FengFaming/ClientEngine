/*需要屏蔽的警告*/
//#pragma warning disable
/*
 * Creator:ffm
 * Desc:下载显示界面
 * Time:2020/7/29 13:56:44
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;
using UnityEngine.UI;

public class UIPnlDownLoad : ObjectBase
{
	/// <summary>
	/// 显示内容
	/// </summary>
	public Text m_ShowTex;

	/// <summary>
	/// 显示进度
	/// </summary>
	public Slider m_ShowProgess;

	/// <summary>
	/// 总大小
	/// </summary>
	private int m_AllFileLength;

	/// <summary>
	/// 总数量
	/// </summary>
	private int m_AllFileCout;

	/// <summary>
	/// 当前大小
	/// </summary>
	private int m_CurrentLength;

	/// <summary>
	/// 当前数量
	/// </summary>
	private int m_CurrentCout;

	private void Awake()
	{
		m_AllFileCout = 0;
		m_AllFileLength = 0;
		m_CurrentCout = 0;
		m_CurrentLength = 0;
	}

	private void Start()
	{
		MessageManger.Instance.AddMessageListener(EngineMessageHead.DOWN_LOAD_FILE_LENGTH_AND_COUT, this.gameObject, LengthAndCout);
		MessageManger.Instance.AddMessageListener(EngineMessageHead.DOWN_LOAD_END_FILE_LENGTH, this.gameObject, OneFile);
		m_ShowProgess.value = 0;
		m_ShowTex.text = "正在校验版本";
	}

	private void OnDestroy()
	{
		MessageManger.Instance.RemoveMessageListener(this.gameObject);
	}

	/// <summary>
	/// 数量和长度
	/// </summary>
	/// <param name="arms"></param>
	private void LengthAndCout(params object[] arms)
	{
		m_AllFileLength = (int)arms[0];
		m_AllFileCout = (int)arms[1];
		m_ShowTex.text = string.Format("版本校验成功，总数量:{0}/总大小:{1}", m_AllFileCout, m_AllFileLength);
		if (m_AllFileCout == m_AllFileLength && m_AllFileLength == 0)
		{
			GameObject.Destroy(this.gameObject);
		}
	}

	private void OneFile(params object[] arms)
	{
		m_CurrentCout++;
		m_CurrentLength += (int)arms[0];
		m_ShowTex.text = string.Format("数量:{0}/{1} 大小:{2}/{3}", m_CurrentCout, m_AllFileCout, m_CurrentLength, m_AllFileLength);

		if (m_CurrentCout >= m_AllFileCout)
		{
			GameObject.Destroy(this.gameObject);
		}
	}
}
