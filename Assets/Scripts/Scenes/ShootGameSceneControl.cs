/*需要屏蔽的警告*/
#pragma warning disable
/*
 * Creator:ffm
 * Desc:场景控制
 * Time:2020/4/27 14:10:52
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Engine;

public partial class ShootGameScene : IScene
{
	public class ShootGameControl : ObjectBase
	{
		private Action<float> m_LoadFun;
		private Action<float> m_EndFun;

		private ShootGamePlayer m_TargetPlayer;

		private int m_Cout;

		private ShootGamePoolControl m_PoolControl;

		public void ClearSceneData()
		{
			if (m_TargetPlayer != null)
			{
				m_TargetPlayer.Clear();
			}

			if (m_PoolControl != null)
			{
				ObjectPoolManager.Instance.RemoveObject(m_PoolControl.PoolName, "Sphere");
			}
		}

		public void LoadScene(Action<float> action)
		{
			m_LoadFun = action;
			m_Cout = 0;
			StartCoroutine("StartScene");
			m_PoolControl = ObjectPoolManager.Instance.InitPool("ShootGamePoolControl", "ShootGamePoolControl") as ShootGamePoolControl;
		}

		public void EndScene(Action<float> action)
		{
			m_EndFun = action;
			m_EndFun(100);
		}

		private void LoadBack(object t)
		{
			GameObject plane = t as GameObject;
			plane.transform.position = new Vector3(0, 0, 0);
			plane.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
			plane.transform.localScale = new Vector3(0.8f, 0.85f, 1);

			m_Cout++;
			m_LoadFun(m_Cout / 2 * 80 + 20);
		}

		private void LoadPlayer(object t)
		{
			m_TargetPlayer = (t as GameObject).gameObject.AddComponent<ShootGamePlayer>();
			m_TargetPlayer.gameObject.name = "ShootGamePlayer";
			m_TargetPlayer.gameObject.transform.position = new Vector3(0, -4, 0);
			m_TargetPlayer.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
			m_TargetPlayer.gameObject.transform.localScale = Vector3.one;

			CharacterXmlControl xml = new CharacterXmlControl("2312002");
			ConfigurationManager.Instance.LoadXml(ref xml);
			GameCharacterStateManager stateManager = new GameCharacterStateManager(m_TargetPlayer);
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

			GameCharacterCameraBase gameCharacterCameraBase = new GameCharacterCameraBase(m_TargetPlayer.gameObject, false, null, false);
			m_TargetPlayer.InitCharacter(gameCharacterCameraBase, null, null, stateManager);
			m_TargetPlayer.SetCameraTra(new Vector3(0, 0, -10), Vector3.zero, Vector3.one);

			if (m_TargetPlayer != null)
			{
				m_Cout++;
				m_LoadFun(m_Cout / 2 * 80 + 20);
			}
		}

		private IEnumerator StartScene()
		{
			yield return null;
			if (Camera.main != null)
			{
				Camera.main.orthographic = true;
				Camera.main.orthographicSize = 5;
				Camera.main.nearClipPlane = 0.3f;
				Camera.main.farClipPlane = 100;

				Camera.main.gameObject.transform.position = new Vector3(0, 0, -10);
				Camera.main.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
				Camera.main.gameObject.transform.localScale = Vector3.one;
			}

			Light l = GameObject.Find("Directional Light").GetComponent<Light>();
			l.shadows = LightShadows.None;

			m_LoadFun(20);
			ResObjectCallBackBase cb = new ResObjectCallBackBase();
			cb.m_FinshFunction = LoadPlayer;
			ResObjectManager.Instance.LoadObject("lion", ResObjectType.GameObject, cb);

			yield return null;

			ResObjectCallBackBase bc = new ResObjectCallBackBase();
			bc.m_FinshFunction = LoadBack;
			ResObjectManager.Instance.LoadObject("Plane", ResObjectType.GameObject, bc);
			UIManager.Instance.OpenUI("UIPnlShootGameMain", UILayer.Bot);
			UIManager.Instance.OpenUI("UIPnlBackGameMain", UILayer.Pnl);

			ResObjectCallBackBase dc = new ResObjectCallBackBase();
			dc.m_FinshFunction = LoadShoot;
			ResObjectManager.Instance.LoadObject("Sphere", ResObjectType.GameObject, dc);
		}

		private void LoadShoot(object t)
		{
			ShootGameObjectControl oc = new ShootGameObjectControl();
			oc.m_Target = t as GameObject;
			ObjectPoolManager.Instance.AddObject(m_PoolControl.PoolName, "Sphere", oc);
			List<ObjectPoolControl> ocs = new List<ObjectPoolControl>();
			for (int index = 0; index < 1; index++)
			{
				ObjectPoolControl objectPoolControl = ObjectPoolManager.Instance.GetCloneObject(m_PoolControl.PoolName, "Sphere");
				ocs.Add(objectPoolControl);
			}

			for (int index = 0; index < ocs.Count; index++)
			{
				ObjectPoolManager.Instance.RecoveryObject(m_PoolControl.PoolName, "Sphere", ocs[index]);
			}
		}
	}
}
