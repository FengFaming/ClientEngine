/*
 * Creator:ffm
 * Desc:动画状态机基类
 * Time:2020/1/8 15:02:18
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

namespace Game.Engine
{
#if UNITY_EDITOR
	using UnityEditor;

	public class MyAnimatorParameters
	{
		public string name { get; set; }
		public AnimatorControllerParameterType type { get; set; }
		public float defaultFloat { get; set; }
		public int defaultInt { get; set; }
		public bool defaultBool { get; set; }

		public MyAnimatorParameters() : this(default(AnimatorControllerParameter))
		{

		}

		public MyAnimatorParameters(AnimatorControllerParameter other)
		{
			name = other.name;
			type = other.type;
			defaultFloat = other.defaultFloat;
			defaultInt = other.defaultInt;
			defaultBool = other.defaultBool;
		}
	}
#endif
	/// <summary>
	/// 动画状态机基类
	/// </summary>
	public class AnimatorBase : ObjectBase
	{
		/// <summary>
		/// 动画帧事件
		/// </summary>
		public class AnimationEventInfo
		{
			/// <summary>
			/// 对应回调的名字
			/// </summary>
			public string m_EventName;

			/// <summary>
			/// 对应帧事件
			/// </summary>
			public float m_EventTime;

			/// <summary>
			/// 回调的参数
			/// </summary>
			public string m_EventValue;
		}

		/// <summary>
		/// 控制的对象
		/// </summary>
		protected Animator m_ControlTarget;

		/// <summary>
		/// 所有的状态内容
		/// </summary>
		public Dictionary<string, AnimationStateBase> m_AllStateDic;

#if UNITY_EDITOR
		/// <summary>
		/// 所有的控制条件
		/// </summary>
		public List<MyAnimatorParameters> m_AllParameters;

		/// <summary>
		/// 更新节点时间
		/// </summary>
		private float m_UpdateTime;
#endif

#if UNITY_EDITOR
		private void Awake()
		{
			m_UpdateTime = 0f;

			Animator at = this.gameObject.GetComponentInChildren<Animator>();
			if (at != null)
			{
				m_ControlTarget = at;
				InitAnimator(at);
			}
		}

		/// <summary>
		/// 初始化内容
		/// </summary>
		/// <param name="owner"></param>
		public void InitAnimator(Animator owner)
		{
			m_AllParameters = new List<MyAnimatorParameters>();
			m_AllParameters.Clear();
			AnimatorControllerParameter[] acps = m_ControlTarget.parameters;
			for (int index = 0; index < acps.Length; index++)
			{
				m_AllParameters.Add(new MyAnimatorParameters(acps[index]));
			}
		}

		private void Update()
		{
			if (m_UpdateTime <= 0)
			{
				ChangeView();
			}
			else
			{
				m_UpdateTime -= Time.deltaTime;
				if (m_UpdateTime <= 0)
				{
					m_UpdateTime = 1f;
				}
			}
		}

		private void ChangeView()
		{
			for (int index = 0; index < m_AllParameters.Count; index++)
			{
				switch (m_AllParameters[index].type)
				{
					case AnimatorControllerParameterType.Bool:
						m_AllParameters[index].defaultBool = m_ControlTarget.GetBool(m_AllParameters[index].name);
						break;
					case AnimatorControllerParameterType.Trigger:
						m_AllParameters[index].defaultBool = m_ControlTarget.GetBool(m_AllParameters[index].name);
						break;
					case AnimatorControllerParameterType.Float:
						m_AllParameters[index].defaultFloat = m_ControlTarget.GetFloat(m_AllParameters[index].name);
						break;
					case AnimatorControllerParameterType.Int:
						m_AllParameters[index].defaultInt = m_ControlTarget.GetInteger(m_AllParameters[index].name);
						break;
				}
			}

			if (Selection.activeGameObject != null)
			{
				///主动刷新选中物体
				EditorUtility.SetDirty(Selection.activeGameObject);
			}
		}
#endif

		/// <summary>
		/// 修改控制数据
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		public virtual void ChangeParameter(string name, object value, AnimatorControllerParameterType type = AnimatorControllerParameterType.Bool)
		{
#if UNITY_EDITOR
			ChangeView();
#endif
		}

		/// <summary>
		/// 添加动画帧事件
		/// </summary>
		/// <param name="events"></param>
		public virtual void InitAnimationEvent(Dictionary<string, List<AnimationEventInfo>> events)
		{
			if (m_ControlTarget != null && events != null)
			{
				AnimationClip[] clips = m_ControlTarget.runtimeAnimatorController.animationClips;
				for (int index = 0; index < clips.Length; index++)
				{
					//clips[index].events = default(AnimationEvent[]);
					//string name = clips[index].name;
					//AnimationEvent start = new AnimationEvent();
					//start.time = 0;
					//start.functionName = "StartAnimation";
					//start.stringParameter = name;
					//clips[index].AddEvent(start);

					//AnimationEvent end = new AnimationEvent();
					//end.time = clips[index].length;
					//end.functionName = "EndAnimation";
					//end.stringParameter = name;
					//clips[index].AddEvent(end);

					if (events.ContainsKey(name))
					{
						for (int i = 0; i < events[name].Count; i++)
						{
							AnimationEvent aevent = new AnimationEvent();
							aevent.time = events[name][i].m_EventTime * clips[index].length;
							aevent.functionName = events[name][i].m_EventName;
							aevent.stringParameter = events[name][i].m_EventValue;
							clips[index].AddEvent(aevent);
						}
					}
				}
			}
		}
	}
}
