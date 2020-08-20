using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CheckUserPassword : SocketMessageBase
{
	private string m_User;
	private string m_Password;

	private bool m_IsSuccess;

	private ClientInfo m_Target;

	public CheckUserPassword() : base()
	{
		m_MessageHead = new MessageHead();
		m_MessageHead.m_MessageID = 10011;
		m_MessageHead.m_MessageType = 1;

		m_User = m_Password = string.Empty;

		m_IsSuccess = false;
		m_Target = null;
	}

	public override bool AnaysizeMessage(byte[] data, ClientInfo client)
	{
		if (!base.AnaysizeMessage(data, client))
			return false;

		m_Target = client;

		int start = 0;
		int cout = BitConverter.ToInt32(data, start);

		start += 4;
		m_User = System.Text.Encoding.UTF8.GetString(data, start, cout);

		start += cout;
		cout = BitConverter.ToInt32(data, start);

		start += 4;
		m_Password = System.Text.Encoding.UTF8.GetString(data, start, cout);

		try
		{
			string loadstr = string.Format("SELECT PASSWORD FROM main WHERE ID={0};", m_User);
			MysqlDatabaseManager.Instance.AddLoadQueue(loadstr, LoadData);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			m_Target.AddSendQueue(this);
		}

		return true;
	}

	/// <summary>
	/// 发送消息
	/// </summary>
	/// <returns></returns>
	public override List<byte> GetSendMessage()
	{
		List<byte> datas = new List<byte>();
		m_MessageHead.m_MessageLength = 1 + 9;
		datas.AddRange(base.GetSendMessage());
		datas.Add(m_IsSuccess ? (byte)1 : (byte)0);
		return datas;
	}

	private void LoadData(object data)
	{
		if (data != null)
		{
			MySqlDataReader read = data as MySqlDataReader;
			if (read.Read())
			{
				string ps = read.GetString("Password");
				if (ps.Equals(m_Password))
				{
					m_IsSuccess = true;
				}
			}
		}

		m_Target.AddSendQueue(this);
	}
}
