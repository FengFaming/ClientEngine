/*
 * Creator:ffm
 * Desc:游戏场景管理器
 * Time:2020/4/11 10:50:34
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class GameSceneManager : SingletonMonoClass<GameSceneManager>
	{
		public enum ChangeStage
		{
			None,

			/// <summary>
			/// 移除之前场景内容
			/// </summary>
			DestoryLast,

			/// <summary>
			/// 加载当前场景内容
			/// </summary>
			LoadCurent
		}

		/// <summary>
		/// 是否正在切换场景当中
		/// </summary>
		private bool m_IsChangeScene;

		/// <summary>
		/// 场景切换进度
		/// </summary>
		private float m_ChangePress;

		/// <summary>
		/// 开始加载的时间长度
		/// </summary>
		private float m_StartChangeTime;

		/// <summary>
		/// 当前场景
		/// </summary>
		private IScene m_Current;
		public IScene Current { get { return m_Current; } }

		/// <summary>
		/// 上一个场景
		/// </summary>
		private IScene m_LastScene;

		protected override void Awake()
		{
			base.Awake();
			m_IsChangeScene = false;
			m_ChangePress = 0f;
		}

		/// <summary>
		/// 切换场景
		/// </summary>
		/// <param name="id"></param>
		public void ChangeScene(IScene scene)
		{
			if (m_IsChangeScene)
			{
				Debug.LogError("wait...Scene is changing.");
				return;
			}

			m_ChangePress = 0f;
			m_IsChangeScene = true;
			StopAllCoroutines();
			StartCoroutine("ChangeStart", scene);
		}

		private IEnumerator ChangeStart(IScene scene)
		{
			yield return null;
			m_StartChangeTime = Time.time;
			m_LastScene = m_Current;
			m_Current = scene;
			m_ChangePress = 0f;

			UIManager.Instance.ClearAllUI();
			MessageManger.Instance.SendMessage(EngineMessageHead.CHANGE_SCENE_MESSAGE, true);
			yield return new WaitForEndOfFrame();
			yield return new WaitForFixedUpdate();
			ChangePressValue(0.1f);

			if (m_LastScene != null)
			{
				m_LastScene.ClearSceneData();
			}

			AsyncOperation asy = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
			yield return asy;
			ChangePressValue(0.2f);
			yield return null;

			if (m_LastScene == null)
			{
				GetDestroyProcess(100);
			}
			else
			{
				m_LastScene.DestroyScene(GetDestroyProcess);
			}
		}

		/// <summary>
		/// 获取目标场景
		/// </summary>
		/// <returns></returns>
		private IEnumerator LoadTarget()
		{
			yield return null;
			ChangePressValue(0.4f);

			//把这个内容提到这个位置，是因为打开界面的时候不能清空资源
			ResObjectManager.Instance.ClearAll();
			yield return new WaitForEndOfFrame();
			yield return new WaitForFixedUpdate();
			ChangePressValue(0.5f);

			AsyncOperation asyt = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(2);
			yield return asyt;
			yield return new WaitForEndOfFrame();
			yield return new WaitForFixedUpdate();
			ChangePressValue(0.6f);

			if (m_Current.InitScene())
			{
				yield return null;
				yield return new WaitForEndOfFrame();
				yield return new WaitForFixedUpdate();

				m_Current.LoadScene(GetStartProcess);
			}
			else
			{
				m_IsChangeScene = false;
				ChangeScene(m_LastScene);
			}
		}

		/// <summary>
		/// 获取加载进度
		/// </summary>
		/// <param name="process"></param>
		private void GetStartProcess(float process)
		{
			if (process == 100)
			{
				ChangePressValue(1);
				MessageManger.Instance.SendMessage(EngineMessageHead.CHANGE_SCENE_MESSAGE, false);
				m_IsChangeScene = false;
			}
			else
			{
				ChangePressValue(0.6f + 0.004f * process);
			}
		}

		/// <summary>
		/// 获取销毁进度
		/// </summary>
		/// <param name="process"></param>
		private void GetDestroyProcess(float process)
		{
			if (process == 100)
			{
				StartCoroutine("LoadTarget");
			}
			else
			{
				ChangePressValue(0.2f + 0.002f * process);
			}
		}

		/// <summary>
		/// 修改加载进度
		/// </summary>
		/// <param name="value"></param>
		private void ChangePressValue(float value)
		{
			m_ChangePress = value;
			MessageManger.Instance.SendMessage(EngineMessageHead.CHANGE_SCENE_PRESS_VALUE, m_ChangePress);
		}
	}
}
