/*
 * Creator:ffm
 * Desc:UI面板控制类
 * Time:2019/12/27 15:50:45
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public abstract class IUIModelControl
	{
		#region 类数据控制
		/// <summary>
		/// ui模型路径
		/// </summary>
		public string m_ModelObjectPath;

		/// <summary>
		/// 控制是否同一时刻只能存在一个
		/// </summary>
		public bool m_IsOnlyOne;

		/// <summary>
		/// 唯一的ID号，由控制类分配
		/// </summary>
		public int m_IsOnlyID;
		#endregion

		/// <summary>
		/// 管理的模型实体
		/// </summary>
		protected GameObject m_ControlTarget;
		public GameObject ControlTarget { get { return m_ControlTarget; } }

		/// <summary>
		/// ui显示层级
		/// </summary>
		protected UILayer m_Layer;
		public UILayer Layer { get { return m_Layer; } }

		/// <summary>
		/// 是否显示当中
		/// </summary>
		protected bool m_IsShow;

		/// <summary>
		/// 构建函数
		/// </summary>
		public IUIModelControl()
		{
			m_ControlTarget = null;
			m_IsShow = false;
			m_Layer = UILayer.None;

			m_IsOnlyOne = false;
			m_ModelObjectPath = string.Empty;
			m_IsOnlyID = -1;
		}

		/// <summary>
		/// 是否参与回退界面
		/// 主要是因为别的界面关闭会回退一个界面
		/// </summary>
		/// <returns></returns>
		public virtual bool IsGoBack()
		{
			return false;
		}

		/// <summary>
		/// 是否允许别人关闭
		/// </summary>
		/// <returns></returns>
		public virtual bool IsClose()
		{
			return false;
		}

		/// <summary>
		/// 获取相互关联的界面
		/// </summary>
		/// <returns></returns>
		public virtual List<string> GetLinksUI()
		{
			return new List<string>();
		}

		/// <summary>
		/// 获取关闭排除界面
		/// 返回是否要关闭别人的操作
		/// </summary>
		/// <param name="others"></param>
		/// <returns></returns>
		public virtual bool GetCloseOther(ref List<string> others)
		{
			return false;
		}

		/// <summary>
		/// 初始化自身数据
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="arms"></param>
		public virtual void InitUIData(UILayer layer, params object[] arms)
		{
			m_Layer = layer;
		}

		/// <summary>
		/// 打开自己
		/// </summary>
		/// <param name="target"></param> 
		public virtual void OpenSelf(GameObject target)
		{
			m_ControlTarget = target;
			SetUIDisable(true);
		}

		/// <summary>
		/// 关闭自己
		/// </summary>
		public virtual void CloseSelf(bool manager = false)
		{
			MessageManger.Instance.RemoveMessageListener(m_ControlTarget);
			UIManager.Instance.RecoveryUIModel(this, manager);
			m_ControlTarget = null;
			m_Layer = UILayer.None;
		}

		/// <summary>
		/// 将界面设置在本层的最上层
		/// </summary>
		public virtual void SetLayerTop()
		{
			m_ControlTarget.transform.SetAsLastSibling();
		}

		/// <summary>
		/// 设置界面显示隐藏
		/// </summary>
		/// <param name="show"></param>
		public virtual void SetUIDisable(bool show)
		{
			m_IsShow = show;
			m_ControlTarget.SetActive(m_IsShow);
		}

		/// <summary>
		/// 进行数据更新
		/// </summary>
		public virtual bool Update()
		{
			if (m_IsShow)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
