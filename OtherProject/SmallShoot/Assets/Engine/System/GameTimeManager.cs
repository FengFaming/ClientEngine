/*
 * Creator:ffm
 * Desc:游戏时间管理
 * Time:2020/1/19 15:39:33
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class GameTimeManager : SingletonMonoClass<GameTimeManager>
	{
		/// <summary>
		/// 游戏运行时长
		/// </summary>
		private int m_GameNowTime;
		public int GameNowTime
		{
			get { return m_GameNowTime; }
		}

		private float m_CalNowTime;

		protected override void Awake()
		{
			base.Awake();
			m_GameNowTime = 0;
			m_CalNowTime = 0;
		}

		private void Update()
		{
			m_CalNowTime += Time.deltaTime;
			m_GameNowTime += (int)(m_CalNowTime / 1);
			m_CalNowTime = m_CalNowTime % 1;
		}
	}
}
