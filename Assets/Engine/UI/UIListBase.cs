/*
 * Creator:ffm
 * Desc:列表基类
 * Time:3/5/2019 9:50:50 AM
* */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Game.Engine
{
	public class UIListBase : MonoBehaviour, IDragHandler, IEndDragHandler
	{
		public enum VectorPosition
		{
			Left,
			Right,
			Up,
			Down
		}

		/// <summary>
		/// list 布局信息
		/// </summary>
		public GridLayoutGroup m_Layout;

		/// <summary>
		/// list Item Data.
		/// </summary>
		public GameObject m_ItemData;

		/// <summary>
		/// list max item count.
		/// </summary>
		public int m_MaxItemCount = 30;

		/// <summary>
		/// list show data.
		/// </summary>
		protected List<GameObject> m_ItemPools;

		/// <summary>
		/// list all data.
		/// </summary>
		protected List<object> m_AllItemData;

		/// <summary>
		/// 上下 & 左右 计数
		/// </summary>
		private int m_TopIndex;
		private int m_DownIndex;

		/// <summary>
		/// 记录是否需要移动计算
		/// </summary>
		private bool m_IsDragging;

		/// <summary>
		/// 记录是什么滑动方式
		/// </summary>
		private bool m_HOV;

		/// <summary>
		/// 最大的板块数量
		/// 主要用来计算Y轴移动位置是否可以刷新内容
		/// 相当于屏的概念
		/// </summary>
		private const int M_PLATE_COUNT = 3;

		/// <summary>
		/// 对数据的初始化
		/// </summary>
		private void Awake()
		{
			ResetData();

			this.gameObject.GetComponent<ScrollRect>().onValueChanged.AddListener(OnValueChange);
			m_HOV = this.gameObject.GetComponent<ScrollRect>().horizontal;
			m_ItemData.gameObject.SetActive(false);
		}

		/// <summary>
		/// 重置所有数据
		/// </summary>
		private void ResetData()
		{
			if (m_ItemPools == null)
				m_ItemPools = new List<GameObject>();
			else
			{
				for (int index = 0; index < m_ItemPools.Count; index++)
					GameObject.Destroy(m_ItemPools[index]);
			}

			if (null == m_AllItemData)
				m_AllItemData = new List<object>();

			m_ItemPools.Clear();
			m_AllItemData.Clear();

			m_IsDragging = true;

			m_TopIndex = 0;
			m_DownIndex = 0;

			m_Layout.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}

		/// <summary>
		/// 添加数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		public virtual void AddItem(object item)
		{
			if (m_MaxItemCount <= 0 || m_ItemPools.Count < m_MaxItemCount)
			{
				GameObject itemGo = GameObject.Instantiate(m_ItemData);
				if (itemGo == null)
					return;

				UIListItemBase itemT = itemGo.GetComponent<UIListItemBase>();
				if (itemT == null)
					return;

				//先设置Index，因为可能在SetData里面会使用到Index.
				itemT.ItemIndex = m_AllItemData.Count;
				itemT.SetData(item);
				m_AllItemData.Add(item);

				itemGo.transform.SetParent(m_Layout.transform);
				itemGo.GetComponent<RectTransform>().localRotation = Quaternion.identity;
				itemGo.GetComponent<RectTransform>().localScale = Vector3.one;
				itemGo.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(itemGo.GetComponent<RectTransform>().anchoredPosition3D.x, itemGo.GetComponent<RectTransform>().anchoredPosition3D.y, 0);

				m_DownIndex = m_ItemPools.Count;

				itemGo.SetActive(true);
				m_ItemPools.Add(itemGo);
			}
			else
			{
				m_AllItemData.Add(item);
			}
		}

		/// <summary>
		/// 移除数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		public virtual void RemoveItem(object item)
		{
			if (m_AllItemData.Contains(item))
				m_AllItemData.Remove(item);

			for (int index = 0; index < m_ItemPools.Count; ++index)
			{
				if (m_ItemPools[index].GetComponent<UIListItemBase>().ComparedData(item))
				{
					GameObject go = m_ItemPools[index];
					m_ItemPools.Remove(m_ItemPools[index]);
					GameObject.Destroy(go);
					break;
				}
			}
		}

		/// <summary>
		/// Clear list all data.
		/// </summary>
		public virtual void ClearData()
		{
			ResetData();
		}

		/// <summary>
		/// 获取当前一共显示着多少个Item
		/// </summary>
		/// <returns></returns>
		private int GetShowCount()
		{
			return m_ItemPools.Count;
		}

		/// <summary>
		/// 获取全部数据个数
		/// </summary>
		/// <returns></returns>
		private int GetAllCount()
		{
			return m_AllItemData.Count;
		}

		/// <summary>
		/// 区域滑动回调事件
		/// </summary>
		/// <param name="pos"></param>
		private void OnValueChange(Vector2 pos)
		{
			ChangeItem();
		}

		/// <summary>
		/// 回调分开只是为了区分系统调用与自身调用
		/// </summary>
		private void ChangeItem()
		{
			if (!m_IsDragging && m_AllItemData.Count > m_MaxItemCount)
			{
				if (!m_HOV)
				{
					//向上
					while (m_Layout.GetComponent<RectTransform>().anchoredPosition.y > GetLineNumber(VectorPosition.Up) &&
						m_DownIndex < m_AllItemData.Count - 1)
					{
						m_TopIndex++;
						m_DownIndex++;

						Vector2 anchoredPosition = m_Layout.GetComponent<RectTransform>().anchoredPosition;
						GameObject go = m_ItemPools[0];
						UIListItemBase item = go.GetComponent<UIListItemBase>();
						m_ItemPools[0].transform.SetAsLastSibling();
						item.SetData(m_AllItemData[m_DownIndex]);

						anchoredPosition.y -= (m_ItemData.GetComponent<UIListItemBase>().GetItemElement().preferredHeight + m_Layout.spacing.y);
						m_Layout.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

						m_ItemPools.RemoveAt(0);
						m_ItemPools.Add(go);
					}

					//向下
					while ((m_Layout.GetComponent<RectTransform>().anchoredPosition.y < GetLineNumber(VectorPosition.Down) && m_TopIndex > 0))
					{
						m_TopIndex--;
						m_DownIndex--;

						Vector2 anchoredPosition = m_Layout.GetComponent<RectTransform>().anchoredPosition;
						GameObject go = m_ItemPools[m_ItemPools.Count - 1];
						UIListItemBase item = go.GetComponent<UIListItemBase>();
						m_ItemPools[m_ItemPools.Count - 1].transform.SetAsFirstSibling();
						item.SetData(m_AllItemData[m_TopIndex]);

						anchoredPosition.y += (m_ItemData.GetComponent<UIListItemBase>().GetItemElement().preferredHeight + m_Layout.spacing.y);
						m_Layout.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

						m_ItemPools.RemoveAt(m_ItemPools.Count - 1);
						m_ItemPools.Insert(0, go);
					}
				}
				else
				{
					//向左
					while (m_Layout.GetComponent<RectTransform>().anchoredPosition.x < GetLineNumber(VectorPosition.Right) &&
						m_DownIndex < m_AllItemData.Count - 1)
					{
						m_TopIndex++;
						m_DownIndex++;

						Vector2 anchoredPosition = m_Layout.GetComponent<RectTransform>().anchoredPosition;
						GameObject go = m_ItemPools[0];
						UIListItemBase item = go.GetComponent<UIListItemBase>();
						m_ItemPools[0].transform.SetAsLastSibling();
						item.SetData(m_AllItemData[m_DownIndex]);

						anchoredPosition.x += (m_ItemData.GetComponent<UIListItemBase>().GetItemElement().preferredWidth + m_Layout.spacing.x);
						anchoredPosition.y = 0;
						m_Layout.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

						m_ItemPools.RemoveAt(0);
						m_ItemPools.Add(go);
					}

					//向右
					while (m_Layout.GetComponent<RectTransform>().anchoredPosition.x > GetLineNumber(VectorPosition.Left) && m_TopIndex > 0)
					{
						m_TopIndex--;
						m_DownIndex--;

						Vector2 anchoredPosition = m_Layout.GetComponent<RectTransform>().anchoredPosition;
						GameObject go = m_ItemPools[m_ItemPools.Count - 1];
						UIListItemBase item = go.GetComponent<UIListItemBase>();
						m_ItemPools[m_ItemPools.Count - 1].transform.SetAsFirstSibling();
						item.SetData(m_AllItemData[m_TopIndex]);

						anchoredPosition.x -= (m_ItemData.GetComponent<UIListItemBase>().GetItemElement().preferredWidth + m_Layout.spacing.x);
						anchoredPosition.y = 0;
						m_Layout.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

						m_ItemPools.RemoveAt(m_ItemPools.Count - 1);
						m_ItemPools.Insert(0, go);
					}
				}
			}
		}

		/// <summary>
		/// 获取需要变换的线方程
		/// </summary>
		/// <returns></returns>
		private float GetLineNumber(VectorPosition vp)
		{
			float number = 0;

			switch (vp)
			{
				//左边 & 右边
				case VectorPosition.Left:
				case VectorPosition.Right:
					float lineCount = m_MaxItemCount / M_PLATE_COUNT > 1 ?
									m_MaxItemCount / M_PLATE_COUNT : 2;
					float spacingCount = lineCount - 1;
					float line = m_ItemData.GetComponent<UIListItemBase>().GetItemElement().preferredWidth * lineCount + m_Layout.spacing.x * spacingCount;
					number = 0 - line;
					break;
				//上边 & 下边
				case VectorPosition.Up:
				case VectorPosition.Down:
					float lineCountY = m_MaxItemCount / M_PLATE_COUNT > 1 ?
									m_MaxItemCount / M_PLATE_COUNT : 2;
					float spcingCountY = lineCountY - 1;
					float lineY = m_ItemData.GetComponent<UIListItemBase>().GetItemElement().preferredHeight * lineCountY + m_Layout.spacing.y * spcingCountY;
					number = lineY;
					break;
			}

			return number;
		}

		/// <summary>
		/// 点击滑动事件
		/// 继承于 IDragHandler
		/// </summary>
		/// <param name="eventData"></param>
		public void OnDrag(PointerEventData eventData)
		{
			m_IsDragging = true;
		}

		/// <summary>
		/// 点击结束事件
		/// 继承于 IEndDragHandler
		/// </summary>
		/// <param name="eventData"></param>
		public void OnEndDrag(PointerEventData eventData)
		{
			m_IsDragging = false;
			ChangeItem();
		}
	}
}
