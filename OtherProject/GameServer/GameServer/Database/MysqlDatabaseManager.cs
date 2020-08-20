using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

/// <summary>
/// 读取数据详细
/// </summary>
public class LoadDataInfo
{
	/// <summary>
	/// 数据索引
	/// </summary>
	public string m_SqlStr;

	/// <summary>
	/// 回调
	/// </summary>
	public Action<object> m_RetrunAction;
}

/// <summary>
/// 数据库管理类
/// </summary>
public class MysqlDatabaseManager
{
	/// <summary>
	/// 自身数据
	/// </summary>
	private static MysqlDatabaseManager m_Mysql;
	public static MysqlDatabaseManager Instance
	{
		get { return m_Mysql; }
	}

	/// <summary>
	/// 连接内容
	/// </summary>
	private MySqlConnection m_MySqlCon;

	/// <summary>
	/// 读取线程
	/// </summary>
	private Thread m_SqlThead;

	/// <summary>
	/// 需要读取的队列
	/// </summary>
	private Queue<LoadDataInfo> m_NeedLoadQue;

	/// <summary>
	/// 构建内容
	/// </summary>
	/// <param name="user"></param>
	/// <param name="password"></param>
	/// <param name="database"></param>
	/// <param name="server"></param>
	/// <param name="port"></param>
	public MysqlDatabaseManager(string user, string password, string database, string server = "127.0.0.1", string port = "3306")
	{
		string connetStr = string.Format("server={0};port={1};user id={2};password={3};database={4}",
										server, port, user, password, database);

		m_MySqlCon = new MySqlConnection(connetStr);

		m_SqlThead = new Thread(LoadData);
		m_SqlThead.IsBackground = true;

		m_NeedLoadQue = new Queue<LoadDataInfo>();
		m_NeedLoadQue.Clear();

		m_Mysql = this;
	}

	/// <summary>
	/// 构建数据库
	/// </summary>
	/// <param name="str"></param>
	public MysqlDatabaseManager(string str)
	{
		if (m_MySqlCon != null)
		{
			m_MySqlCon = null;
			m_SqlThead = null;
			m_NeedLoadQue.Clear();
			m_NeedLoadQue = null;
			m_Mysql = null;
		}

		m_MySqlCon = new MySqlConnection(str);
		m_SqlThead = new Thread(LoadData);
		m_SqlThead.IsBackground = true;
		m_NeedLoadQue = new Queue<LoadDataInfo>();
		m_NeedLoadQue.Clear();

		m_Mysql = this;
	}

	/// <summary>
	/// 连接数据库
	/// </summary>
	/// <returns></returns>
	public MySqlException ConnectMysql()
	{
		try
		{
			m_MySqlCon.Open();
			Console.WriteLine("connect successed.");
			m_SqlThead.Start();
		}
		catch (MySqlException ex)
		{
			Console.WriteLine(ex);
			return ex;
		}

		return null;
	}

	/// <summary>
	/// 添加读取内容
	/// </summary>
	/// <param name="str"></param>
	/// <param name="action"></param>
	public void AddLoadQueue(string str, Action<object> action)
	{
		LoadDataInfo info = new LoadDataInfo();
		info.m_SqlStr = str;
		info.m_RetrunAction = action;
		m_NeedLoadQue.Enqueue(info);
	}

	/// <summary>
	/// 线程读取数据库
	/// </summary>
	private void LoadData()
	{
		while (true)
		{
			if (m_NeedLoadQue.Count > 0)
			{
				LoadDataInfo info = null;
				Monitor.Enter("LKDataBase");
				info = m_NeedLoadQue.Dequeue();
				Monitor.Exit("LKDataBase");

				if (info != null)
				{
					if (info.m_RetrunAction != null)
					{
						info.m_RetrunAction(ReadData(info.m_SqlStr));
					}
				}
			}

			Thread.Sleep(100);
		}
	}

	/// <summary>
	/// 读取数据
	/// </summary>
	/// <param name="sql"></param>
	/// <returns></returns>
	private MySqlDataReader ReadData(string sql)
	{
		MySqlCommand cmd = new MySqlCommand(sql, m_MySqlCon);
		MySqlDataReader read = cmd.ExecuteReader();
		return read;
	}
}
