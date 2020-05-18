/*
 * Creator:ffm
 * Desc:第一个游戏场景
 * Time:2020/4/11 17:45:38
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public class AnimationScene : IScene
{
	public class AnimationSceneControl : ObjectBase
	{
		public class LoadGrass : IResObjectCallBack
		{
			public Vector3 m_Position;
			public Vector3 m_Rotation;
			public Vector3 m_LocalScale;
			public GameObject m_Parent;

			public Action<object, Vector3, Vector3, Vector3, GameObject> m_LoadEnd;

			public LoadGrass() : base()
			{

			}

			public override void HandleLoadCallBack(object t)
			{
				if (m_LoadEnd != null)
				{
					m_LoadEnd(t, m_Position, m_Rotation, m_LocalScale, m_Parent);
				}
			}

			public override int LoadCallbackPriority()
			{
				return 0;
			}
		}

		private Action<float> m_LoadAction;

		private int m_MaxLoadCont;
		private int m_LoadCont;

		/// <summary>
		/// 场景目标
		/// </summary>
		private AnimationPlayer m_SceneTarget;

		private void Awake()
		{
			m_MaxLoadCont = 101;
			m_LoadCont = 0;
		}

		public void ClearData()
		{
			if (m_SceneTarget != null)
			{
				m_SceneTarget.Clear();
			}

			m_SceneTarget = null;
		}

		public void LoadScene(Action<float> action)
		{
			m_LoadAction = action;
			m_LoadCont = 0;

			StartCoroutine("StartLoadScene");
		}

		private void CreateCharacter()
		{
			GameObject go = new GameObject();
			go.name = "Player";
			go.transform.position = Vector3.zero;
			go.transform.rotation = Quaternion.Euler(Vector3.zero);
			go.transform.localScale = Vector3.one;
			AnimationPlayer ch = go.AddComponent<AnimationPlayer>();
			ch.StartInitCharacter("elephant", GetCharacter);
			m_SceneTarget = ch;
		}

		private void GetCharacter(object t)
		{
			m_LoadCont++;
			m_LoadAction((m_LoadCont / (float)m_MaxLoadCont) * 100);
			AnimationPlayer ch = t as AnimationPlayer;

			CharacterXmlControl xml = new CharacterXmlControl("2312001");
			ConfigurationManager.Instance.LoadXml(ref xml);
			GameCharacterStateManager stateManager = new GameCharacterStateManager(ch);
			foreach (var info in xml.m_StateInfos)
			{
				CharacterStateBase state = ReflexManager.Instance.CreateClass(info.Value.m_Control, info.Key) as CharacterStateBase;
				if (state != null)
				{
					for (int index = 0; index < info.Value.m_Paramters.Count; index++)
					{
						state.AddChangeStateParameter(info.Value.m_Paramters[index]);
					}
				}

				stateManager.AddManagerState(state);
			}

			ch.InitCharacter(null, null, null, stateManager);
			ch.SetCameraTra(new Vector3(0, 2, -10), Vector3.zero, Vector3.one);
		}

		private void LoadGrassEnd(object t, Vector3 p, Vector3 r, Vector3 s, GameObject g)
		{
			m_LoadCont++;
			GameObject go = t as GameObject;
			go.transform.parent = g.transform;
			go.transform.localPosition = p;
			go.transform.localRotation = Quaternion.Euler(r);
			go.transform.localScale = s;
			m_LoadAction((m_LoadCont / (float)m_MaxLoadCont) * 100);
		}

		private IEnumerator StartLoadScene()
		{
			yield return null;
			CreateCharacter();
			yield return null;

			GameObject parent = new GameObject();
			parent.name = "garss";
			parent.gameObject.transform.position = Vector3.zero;
			parent.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
			parent.gameObject.transform.localScale = Vector3.one;
			Vector3 start = new Vector3(-15, 0, 15);
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					LoadGrass loadGrass = new LoadGrass();
					loadGrass.m_LocalScale = Vector3.one;
					loadGrass.m_Rotation = Vector3.zero;
					Vector3 position = start + new Vector3(3 * i, 0, -3 * j);
					loadGrass.m_Position = position;
					loadGrass.m_Parent = parent;
					loadGrass.m_LoadEnd = LoadGrassEnd;
					ResObjectManager.Instance.LoadObject("grass", ResObjectType.GameObject, loadGrass);
					yield return null;
				}
			}

			yield return null;
			UIManager.Instance.OpenUI("UIPnlBackGameMain", UILayer.Pnl);
		}
	}

	private AnimationSceneControl m_LoadControl;

	public AnimationScene(string name) : base(name)
	{

	}

	public override void InitScene()
	{
		///为什么要在这里创建，因为场景切换的过程当中会删除掉一部分
		GameObject scene = new GameObject();
		scene.name = m_SceneName;
		scene.gameObject.transform.position = Vector3.zero;
		scene.gameObject.transform.eulerAngles = Vector3.zero;
		scene.gameObject.transform.localScale = Vector3.one;
		m_LoadControl = scene.gameObject.AddComponent<AnimationSceneControl>();
	}

	public override void ClearSceneData()
	{
		base.ClearSceneData();
		if (m_LoadControl != null)
		{
			m_LoadControl.ClearData();
		}
	}

	public override void LoadScene(Action<float> action)
	{
		if (m_LoadControl != null)
		{
			m_LoadControl.LoadScene(action);
		}
	}
}
