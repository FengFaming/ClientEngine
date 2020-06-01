/*
 * Creator:ffm
 * Desc:UI管理器
 * Time:2019/12/27 15:00:50
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

namespace Game.Engine
{
	public enum UILayer
	{
		None = 0,

		/// <summary>
		/// 最底层
		///		一般不会移动
		/// </summary>
		Bot = 1,

		/// <summary>
		/// 面板层
		/// </summary>
		Pnl = 2,

		/// <summary>
		/// 普通弹出框
		/// </summary>
		Dlg = 3,

		/// <summary>
		/// 提示层
		/// </summary>
		Tip = 4,

		/// <summary>
		/// 紧急弹框层
		/// </summary>
		Blk = 5,
	}

	[XLua.LuaCallCSharp]
	public class UIManager : SingletonMonoClass<UIManager>
	{
		/// <summary>
		/// 协同程序详细
		/// </summary>
		internal class CoroutineFun
		{
			public IUIModelControl m_Control;
			public Action m_Action;
		}

		internal class LoadUIModel : IResObjectCallBack
		{
			private IUIModelControl m_Control;
			private Action<object, IUIModelControl, List<string>, IUIModelControl> m_CallBack;

			public List<string> m_Others;
			public IUIModelControl m_OtherControl;

			public LoadUIModel(IUIModelControl control,
				Action<object, IUIModelControl, List<string>, IUIModelControl> cb)
			{
				m_Control = control;
				m_CallBack = cb;
				m_Others = new List<string>();
				m_Others.Clear();
				m_OtherControl = null;
			}

			/// <summary>
			/// 资源加载回调
			/// </summary>
			/// <param name="t"></param>
			public override void HandleLoadCallBack(object t)
			{
				if (m_CallBack != null)
				{
					m_CallBack(t, m_Control, m_Others, m_OtherControl);
				}
			}

			/// <summary>
			/// 加载回调优先级
			/// </summary>
			/// <returns></returns>
			public override int LoadCallbackPriority()
			{
				return 0;
			}
		}

		/// <summary>
		/// 需要加载的队列
		/// </summary>
		internal class OpenUIData
		{
			public string m_Name;
			public UILayer m_Layer;
			public List<object> m_Arms;

			public OpenUIData(string name, UILayer layer, params object[] arms)
			{
				m_Name = name;
				m_Layer = layer;
				m_Arms = new List<object>();
				if (arms != null)
				{
					m_Arms.AddRange(arms);
				}
			}

			public override string ToString()
			{
				string str = m_Name + " " + m_Layer;
				return str;
			}
		}

		/// <summary>
		/// 所有正在显示的UI内容
		/// </summary>
		private Dictionary<UILayer, List<IUIModelControl>> m_AllShowUIModels;

		/// <summary>
		/// 显示的序列
		/// </summary>
		private List<KeyValuePair<UILayer, int>> m_ShowSequence;

		/// <summary>
		/// 父节点
		/// </summary>
		private Dictionary<UILayer, Transform> m_ParentDic;

		/// <summary>
		/// 需要打开的界面队列
		/// </summary>
		private List<OpenUIData> m_NeedOpenUIs;

		/// <summary>
		/// 需要执行Update的界面
		/// </summary>
		private List<IUIModelControl> m_NeedUpdateUIs;

		/// <summary>
		/// 已经有正在打开的界面
		/// </summary>
		private bool m_HasOpen;

		/// <summary>
		/// 是否需要清理
		/// </summary>
		private bool m_IsClear;

		/// <summary>
		/// 所有的需要协同程序执行的方法
		/// </summary>
		private List<CoroutineFun> m_AllCoroutineActions;

		/// <summary>
		/// 是否有协同程序运行
		/// </summary>
		private bool m_IsCoroutine;

		protected override void Awake()
		{
			base.Awake();
			m_AllShowUIModels = new Dictionary<UILayer, List<IUIModelControl>>();
			m_AllShowUIModels.Clear();

			m_ShowSequence = new List<KeyValuePair<UILayer, int>>();
			m_ShowSequence.Clear();

			m_ParentDic = new Dictionary<UILayer, Transform>();
			for (int index = 1; index <= this.gameObject.transform.childCount; index++)
			{
				int id = index - 1;
				Transform tf = this.gameObject.transform.GetChild(id);
				if (((UILayer)index).ToString() == tf.name)
				{
					m_ParentDic.Add((UILayer)index, tf);
				}
			}

			m_NeedUpdateUIs = new List<IUIModelControl>();
			m_NeedUpdateUIs.Clear();

			m_NeedOpenUIs = new List<OpenUIData>();
			m_NeedOpenUIs.Clear();
			m_HasOpen = false;
			m_IsClear = false;

			m_AllCoroutineActions = new List<CoroutineFun>();
			m_AllCoroutineActions.Clear();

			m_IsCoroutine = false;
		}

		/// <summary>
		/// 数据更新
		/// </summary>
		private void Update()
		{
			if (m_NeedUpdateUIs.Count > 0 && !m_IsClear)
			{
				for (int index = 0; index < m_NeedUpdateUIs.Count;)
				{
					if (m_NeedUpdateUIs[index].ControlTarget != null)
					{
						m_NeedUpdateUIs[index].Update();
						index++;
					}
					else
					{
						m_NeedUpdateUIs.RemoveAt(index);
					}
				}
			}
		}

		/// <summary>
		/// 打开界面
		/// </summary>
		/// <param name="name">类名</param>
		/// <param name="layer">层级</param>
		/// <param name="arms">变长参数</param>
		public void OpenUI(string name, UILayer layer, params object[] arms)
		{
			OpenUIData data = new OpenUIData(name, layer, arms);
			m_NeedOpenUIs.Add(data);
			OpenUI();

			if (!m_IsCoroutine)
			{
				StartCoroutine("UICoroutine");
				m_IsCoroutine = true;
			}
		}

		/// <summary>
		/// 获取界面
		/// </summary>
		/// <param name="name"></param>
		/// <param name="layer"></param>
		/// <returns></returns>
		public IUIModelControl GetShowUI(string name, UILayer layer = UILayer.None)
		{
			IUIModelControl ui = null;

			layer = layer == UILayer.None ? UILayer.Pnl : layer;
			List<IUIModelControl> cs = m_AllShowUIModels[layer];
			for (int index = 0; index < cs.Count; index++)
			{
				if (cs[index].GetType().Name == name)
				{
					ui = cs[index];
					break;
				}
			}

			return ui;
		}

		/// <summary>
		/// 回收UI对象
		/// </summary>
		/// <param name="name"></param>
		/// <param name="layer"></param>
		public void RecoveryUIModel(string name, UILayer layer)
		{
			IUIModelControl ui = GetShowUI(name, layer);
			if (ui != null)
			{
				RecoveryUIModel(ui, false);
			}

			for (int index = 0; index < m_NeedOpenUIs.Count;)
			{
				if (m_NeedOpenUIs[index].m_Name == name &&
					m_NeedOpenUIs[index].m_Layer == layer)
				{
					m_NeedOpenUIs.RemoveAt(index);
				}
				else
				{
					index++;
				}
			}
		}

		/// <summary>
		/// 回收ui对象
		/// </summary>
		/// <param name="ui"></param>
		public void RecoveryUIModel(IUIModelControl ui, bool manager)
		{
			if (ui.ControlTarget != null)
			{
				ui.ControlTarget.SetActive(false);
				GameObject.Destroy(ui.ControlTarget);
			}

			UILayer layer = ui.Layer;
			int id = ui.m_IsOnlyID;

			///移除回退界面记录
			KeyValuePair<UILayer, int> value = new KeyValuePair<UILayer, int>(layer, id);
			m_ShowSequence.Remove(value);
			//for (int index = 0; index < m_ShowSequence.Count; index++)
			//{
			//	if (m_ShowSequence.Equals(value))
			//	{
			//		m_ShowSequence.RemoveAt(index);
			//		break;
			//	}
			//}

			///移除整体界面记录
			List<IUIModelControl> cs = m_AllShowUIModels[layer];
			for (int index = 0; index < cs.Count; index++)
			{
				if (cs[index].m_IsOnlyID == id)
				{
					cs.RemoveAt(index);
					break;
				}
			}

			for (int index = 0; index < m_NeedUpdateUIs.Count; index++)
			{
				if (m_NeedUpdateUIs[index].Layer == ui.Layer &&
					m_NeedUpdateUIs[index].m_IsOnlyID == ui.m_IsOnlyID)
				{
					m_NeedUpdateUIs.RemoveAt(index);
					break;
				}
			}

			if (!manager)
			{
				GoBackWithClose(ui);
			}
		}

		/// <summary>
		/// 添加监听
		/// </summary>
		/// <param name="control"></param>
		public void AddUpdate(IUIModelControl control)
		{
			if (control.ControlTarget != null)
			{
				m_NeedUpdateUIs.Add(control);
			}
		}

		/// <summary>
		/// 添加一个协同程序
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="action"></param>
		public void AddCoroutine(IUIModelControl ui, Action action)
		{
			CoroutineFun fun = new CoroutineFun();
			fun.m_Control = ui;
			fun.m_Action = action;
			m_AllCoroutineActions.Add(fun);
		}

		/// <summary>
		/// 移除协同程序
		/// </summary>
		/// <param name="ui"></param>
		public void RemoveCoroutine(IUIModelControl ui)
		{
			List<CoroutineFun> funs = new List<CoroutineFun>();
			funs.Clear();
			for (int index = 0; index < m_AllCoroutineActions.Count; index++)
			{
				if (m_AllCoroutineActions[index].m_Control == ui)
				{
					funs.Add(m_AllCoroutineActions[index]);
				}
			}

			for (int index = 0; index < funs.Count; index++)
			{
				RemoveCoroutine(funs[index]);
			}
		}

		/// <summary>
		/// 移除一个协同程序
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="action"></param>
		public void RemoveCoroutine(IUIModelControl ui, Action action)
		{
			List<CoroutineFun> funs = new List<CoroutineFun>();
			funs.Clear();
			for (int index = 0; index < m_AllCoroutineActions.Count; index++)
			{
				if (m_AllCoroutineActions[index].m_Control == ui &&
					m_AllCoroutineActions[index].m_Action == action)
				{
					funs.Add(m_AllCoroutineActions[index]);
				}
			}

			for (int index = 0; index < funs.Count; index++)
			{
				RemoveCoroutine(funs[index]);
			}
		}

		public void ClearAllUI()
		{
			///存在打开界面
			if (m_HasOpen)
			{
				m_IsClear = true;
			}

			m_HasOpen = false;
			m_ShowSequence.Clear();
			m_NeedOpenUIs.Clear();
			m_NeedUpdateUIs.Clear();
			m_AllCoroutineActions.Clear();
			foreach (KeyValuePair<UILayer, List<IUIModelControl>> uis in m_AllShowUIModels)
			{
				for (int index = 0; index < uis.Value.Count; index++)
				{
					uis.Value[index].ControlTarget.SetActive(false);
					GameObject.Destroy(uis.Value[index].ControlTarget);
				}

				uis.Value.Clear();
			}
		}

		#region 打开相关
		/// <summary>
		/// 内部打开数据
		/// </summary>
		private void OpenUI()
		{
			if (!m_HasOpen && m_NeedOpenUIs.Count > 0)
			{
				m_HasOpen = true;
				OpenUIData data = m_NeedOpenUIs[0];
				string name = data.m_Name;
				UILayer layer = data.m_Layer;
				IUIModelControl control = ReflexManager.Instance.CreateClass(name) as IUIModelControl;
				if (control != null)
				{
					if (control.m_IsOnlyOne)
					{
						if (m_AllShowUIModels.ContainsKey(layer))
						{
							for (int index = 0; index < m_AllShowUIModels[layer].Count; index++)
							{
								if (m_AllShowUIModels[layer][index].GetType().Name == name)
								{
									control = m_AllShowUIModels[layer][index];
									m_AllShowUIModels[layer].RemoveAt(index);
									break;
								}
							}
						}
					}

					control.InitUIData(layer, data.m_Arms.ToArray());
					OpenSelfUI(control);
				}
			}
		}

		/// <summary>
		/// 设置父亲节点
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="go"></param>
		private void SetParent(UILayer layer, GameObject go)
		{
			go.transform.SetParent(m_ParentDic[layer]);
			go.transform.SetAsLastSibling();

			RectTransform tf = go.GetComponent<RectTransform>();
			tf.localPosition = Vector3.zero;
			tf.localEulerAngles = Vector3.zero;
			tf.localScale = Vector3.one;
			tf.sizeDelta = Vector2.zero;
			tf.anchorMin = Vector2.zero;
			tf.anchorMax = Vector2.one;
			tf.pivot = Vector2.one * 0.5f;
		}

		/// <summary>
		/// 加载回调
		/// </summary>
		/// <param name="t"></param>
		/// <param name="control"></param>
		private void LoadCalBack(object t, IUIModelControl control,
			List<string> others, IUIModelControl other)
		{
			if (m_IsClear && m_NeedOpenUIs.Count == 0)
			{
				m_IsClear = false;
				m_HasOpen = false;
				return;
			}

			GameObject target = t as GameObject;
			SetParent(control.Layer, target);

			if (m_AllShowUIModels.ContainsKey(control.Layer))
			{
				m_AllShowUIModels[control.Layer].Sort((IUIModelControl c1, IUIModelControl c2) =>
				{
					return c1.m_IsOnlyID - c2.m_IsOnlyID;
				});

				for (int index = 0; index < m_AllShowUIModels[control.Layer].Count; index++)
				{
					if (m_AllShowUIModels[control.Layer][index].m_IsOnlyID != index)
					{
						control.m_IsOnlyID = index;
						break;
					}
				}

				m_AllShowUIModels[control.Layer].Add(control);
			}
			else
			{
				control.m_IsOnlyID = 0;
				m_AllShowUIModels.Add(control.Layer, new List<IUIModelControl>() { control });
			}

			if (control.IsGoBack())
			{
				m_ShowSequence.Insert(0, new KeyValuePair<UILayer, int>(control.Layer, control.m_IsOnlyID));
			}

			control.OpenSelf(target);
			OpenOtherUI(others, control.Layer, other);
		}

		/// <summary>
		/// 打开其他关联数据
		/// </summary>
		/// <param name="others"></param>
		private void OpenOtherUI(List<string> others, UILayer layer, IUIModelControl self)
		{
			if (others != null && others.Count > 0)
			{
				string name = others[0];
				others.RemoveAt(0);
				IUIModelControl control = ReflexManager.Instance.CreateClass(name) as IUIModelControl;
				if (control == null)
				{
					OpenOtherUI(others, layer, self);
				}
				else
				{
					if (control.m_IsOnlyOne)
					{
						if (m_AllShowUIModels.ContainsKey(layer))
						{
							for (int index = 0; index < m_AllShowUIModels[layer].Count; index++)
							{
								if (m_AllShowUIModels[layer][index].GetType().Name == name)
								{
									control = m_AllShowUIModels[layer][index];
									m_AllShowUIModels[layer].RemoveAt(index);
									break;
								}
							}
						}
					}

					if (control.m_IsOnlyID > -1)
					{
						UILayer cl = control.Layer;
						int id = control.m_IsOnlyID;
						KeyValuePair<UILayer, int> cv = new KeyValuePair<UILayer, int>(cl, id);
						m_ShowSequence.Remove(cv);
					}

					control.InitUIData(layer);
					if (control.ControlTarget == null)
					{
						LoadUIModel lu = new LoadUIModel(control, LoadCalBack);
						lu.m_OtherControl = self;
						lu.m_Others = others;
						ResObjectManager.Instance.LoadObject(control.m_ModelObjectPath,
							ResObjectType.UIPrefab, lu);
					}
					else
					{
						LoadCalBack(control.ControlTarget, control, others, self);
					}
				}
			}
			else
			{
				if (self != null)
				{
					if (self.ControlTarget == null)
					{
						LoadUIModel lu = new LoadUIModel(self, LoadCalBack);
						ResObjectManager.Instance.LoadObject(self.m_ModelObjectPath,
							ResObjectType.UIPrefab, lu);
					}
					else
					{
						LoadCalBack(self.ControlTarget, self, null, null);
					}
				}
				else
				{
					OpenUIData data = m_NeedOpenUIs[0];
					//打开完毕，进行其他的操作
					Debug.Log("open end." + data);
					m_NeedOpenUIs.RemoveAt(0);
					CloseOther(data);

					m_HasOpen = false;
					if (m_NeedOpenUIs.Count > 0)
					{
						OpenUI();
					}
				}
			}
		}

		/// <summary>
		/// 对象数据
		/// </summary>
		/// <param name="control"></param>
		private void OpenSelfUI(IUIModelControl control)
		{
			List<string> parents = control.GetLinksUI();
			if (parents.Count > 0)
			{
				OpenOtherUI(parents, control.Layer, control);
			}
			else
			{
				if (control.ControlTarget != null)
				{
					LoadCalBack(control.ControlTarget, control, null, null);
				}
				else
				{
					LoadUIModel lu = new LoadUIModel(control, LoadCalBack);
					ResObjectManager.Instance.LoadObject(control.m_ModelObjectPath,
						ResObjectType.UIPrefab, lu);
				}
			}
		}
		#endregion

		#region 关闭相关
		/// <summary>
		/// 因为打开界面引起的关闭和隐藏界面
		/// </summary>
		/// <param name="last"></param>
		private void CloseOther(OpenUIData last)
		{
			IUIModelControl control = null;
			List<IUIModelControl> cs = m_AllShowUIModels[last.m_Layer];
			for (int index = 0; index < cs.Count; index++)
			{
				if (cs[index].GetType().Name == last.m_Name)
				{
					control = cs[index];
					break;
				}
			}

			if (control != null)
			{
				List<string> closes = new List<string>();
				closes.Add(last.m_Name);
				closes.AddRange(control.GetLinksUI());
				bool n = control.GetCloseOther(ref closes);
				if (n)
				{
					List<IUIModelControl> cls = new List<IUIModelControl>();
					for (int index = 0; index < cs.Count; index++)
					{
						if (!closes.Contains(cs[index].GetType().Name))
						{
							if (!cs[index].IsClose())
							{
								cls.Add(cs[index]);
							}
						}
					}

					for (int index = 0; index < cls.Count; index++)
					{
						cls[index].CloseSelf(true);
					}
				}
			}
		}

		/// <summary>
		/// 因为关闭导致的回退界面
		/// </summary>
		private void GoBackWithClose(IUIModelControl ui)
		{
			List<IUIModelControl> cs = m_AllShowUIModels[ui.Layer];
			List<IUIModelControl> nc = new List<IUIModelControl>();
			string uiparten = ui.GetType().Name;
			for (int index = 0; index < cs.Count; index++)
			{
				if (cs[index].GetLinksUI().Contains(uiparten))
				{
					nc.Add(cs[index]);
				}
			}

			for (int index = 0; index < nc.Count; index++)
			{
				nc[index].CloseSelf(true);
			}

			if (m_ShowSequence.Count > 0)
			{
				KeyValuePair<UILayer, int> value = m_ShowSequence[0];
				m_ShowSequence.RemoveAt(0);

				List<IUIModelControl> ccs = m_AllShowUIModels[value.Key];
				for (int index = 0; index < ccs.Count; index++)
				{
					if (ccs[index].m_IsOnlyID == value.Value)
					{
						OpenUI(ccs[index].GetType().Name, ccs[index].Layer);
						break;
					}
				}
			}
		}
		#endregion

		#region 协同程序相关
		/// <summary>
		/// 移除
		/// </summary>
		/// <param name="fun"></param>
		private void RemoveCoroutine(CoroutineFun fun)
		{
			m_AllCoroutineActions.Remove(fun);
		}

		private IEnumerator UICoroutine()
		{
			yield return null;
			while (true)
			{
				while (m_AllCoroutineActions.Count > 0)
				{
					for (int index = 0; index < m_AllCoroutineActions.Count; index++)
					{
						m_AllCoroutineActions[index].m_Action();
						yield return null;
					}
				}

				yield return null;
			}
		}
		#endregion
	}
}
