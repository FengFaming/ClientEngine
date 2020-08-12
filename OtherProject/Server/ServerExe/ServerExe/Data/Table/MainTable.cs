using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

public class MainTable
{
	public class MainTableData
	{
		public int m_UID;
		public string m_Name;
		public int m_Level;

		public override string ToString()
		{
			return string.Format("ID:{0};Name:{1};Level:{2}", m_UID, m_Name, m_Level);
		}
	}

	private Dictionary<int, MainTableData> m_AllUser;
	private bool m_IsOne;
	private bool m_IsSuccess;

	public MainTable()
	{
		m_AllUser = new Dictionary<int, MainTableData>();
		m_AllUser.Clear();

		m_IsOne = false;
		m_IsSuccess = false;
	}

	public bool LoadData(bool one = false)
	{
		if (one)
		{
			ClearData();
		}

		if (!m_IsOne)
		{
			m_IsOne = true;
			MysqlDatabaseManager.Instance.AddLoadQueue("SELECT * FROM main;", LoadEnd);
		}

		return m_IsSuccess;
	}

	public MainTableData GetData(int uid)
	{
		if (m_AllUser.ContainsKey(uid))
		{
			return m_AllUser[uid];
		}

		return null;
	}

	private void LoadEnd(object o)
	{
		MySqlDataReader read = o as MySqlDataReader;
		while (read.Read())
		{
			int uid = read.GetInt32("ID");
			MainTableData data = new MainTableData();
			data.m_UID = uid;
			data.m_Name = read.GetString("Name");
			data.m_Level = read.GetInt32("Level");
			if (!m_AllUser.ContainsKey(uid))
			{
				m_AllUser.Add(uid, data);
			}
		}

		m_IsSuccess = true;
	}

	private void ClearData()
	{
		m_AllUser.Clear();
		m_IsOne = false;
		m_IsSuccess = false;
	}
}