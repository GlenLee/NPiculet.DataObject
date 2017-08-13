using System;
using System.Configuration;
using System.Data;
using System.Threading;

namespace NPiculet.DataObject
{
	public abstract partial class AbstractDbHelper : IDbHelper
	{
		#region 获取配置，通用静态类

		/// <summary>
		/// 获取数据库连接字符串的索引键名称。
		/// </summary>
		public static string DefaultConnectionKey
		{
			get { return ConfigurationManager.AppSettings["DefaultConnectionKey"]; }
		}

		/// <summary>
		/// 获取数据库类型。
		/// </summary>
		/// <returns></returns>
		public static ServerType DefaultConnectionType
		{
			get { return GetConnectionType(DefaultConnectionKey); }
		}

		/// <summary>
		/// 获取指定的数据连接类型
		/// </summary>
		/// <param name="connKey"></param>
		/// <returns></returns>
		public static ServerType GetConnectionType(string connKey)
		{
			string key = string.IsNullOrEmpty(connKey) ? DefaultConnectionKey : connKey;
			string type = ConfigurationManager.ConnectionStrings[key].ProviderName;
			switch (type) {
				// Microsoft SQL Server 数据库的连接方式
				case "System.Data.SqlClient":
					return ServerType.SqlClient;
				// Microsoft Access 数据库的连接方式
				case "System.Data.OleDb":
					return ServerType.OleDb;
				// Oracle 数据库的连接方式
				case "System.Data.OracleClient":
					return ServerType.OracleClient;
				case "Oracle.DataAccess":
					return ServerType.OracleDataAccess;
				// ODBC 数据库的连接方式
				case "System.Data.Odbc":
					return ServerType.Odbc;
				// MySql 数据库的连接方式
				case "MySql.Data.MySqlClient":
					return ServerType.MySql;
				// PostgreSQL 数据库的连接方式
				case "Npgsql":
					return ServerType.PgSql;
				default:
					return ServerType.SqlClient;
			}
		}

		/// <summary>
		/// 获取数据库连接字符串
		/// </summary>
		/// <returns></returns>
		public static string DefaultConnectionString
		{
			get { return GetConnectionString(DefaultConnectionKey); }
		}

		/// <summary>
		/// 获取指定的连接字符串
		/// </summary>
		/// <param name="connKey"></param>
		/// <returns></returns>
		public static string GetConnectionString(string connKey)
		{
			try {
				string key = string.IsNullOrEmpty(connKey) ? DefaultConnectionKey : connKey;
				ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[key];
				return ConfigurationManager.ConnectionStrings[key].ConnectionString;
			} catch (Exception ex) {
				throw new DataObjectException("数据库连接键名称 " + connKey + " 没有找到对应的配置，请检查 web.config 中 connectionStrings 节是否有 name=" + connKey + " 的配置节。", ex);
			}
		}

		private ServerType _currentConnectionType = ServerType.None;
		private string _currentConnectionString = null;

		/// <summary>
		/// 当前连接字符串
		/// </summary>
		public virtual ServerType CurrentConnectionType
		{
			get { return _currentConnectionType == ServerType.None ? DefaultConnectionType : _currentConnectionType; }
		}

		/// <summary>
		/// 当前连接字符串
		/// </summary>
		public virtual string CurrentConnectionString
		{
			get { return string.IsNullOrEmpty(_currentConnectionString) ? DefaultConnectionString : _currentConnectionString; }
		}

		/// <summary>
		/// 初始化连接字符串
		/// </summary>
		/// <param name="type"></param>
		/// <param name="connString"></param>
		protected AbstractDbHelper(ServerType type, string connString)
		{
			_currentConnectionType = type;
			_currentConnectionString = connString;
		}

		#endregion

		#region 初始化参数

		private IDbConnection _connection;
		private IDbCommand _command;
		private IDbDataAdapter _adapter;

		/// <summary>
		/// DbCommection 对象。
		/// </summary>
		public IDbConnection Connection { get { return _connection; } set { _connection = value; } }

		/// <summary>
		/// DbCommand 对象。
		/// </summary>
		public IDbCommand Command { get { return _command; } set { _command = value; } }

		/// <summary>
		/// DbDataAapter 对象。
		/// </summary>
		public IDbDataAdapter Adapter { get { return _adapter; } set { _adapter = value; } }

		#endregion

		#region 创建对象，推迟到实际应用类中实现

		/// <summary>
		/// 创建数据库连接对象。
		/// </summary>
		/// <param name="connString">连接字符串</param>
		/// <returns>数据库连接对象</returns>
		protected abstract IDbConnection CreateConnection(string connString);

		/// <summary>
		/// 创建数据适配器。
		/// </summary>
		/// <returns>数据适配器</returns>
		protected abstract IDbDataAdapter CreateDataAdapter();

		#endregion

		#region 通用数据库连接方法

		/// <summary>
		/// 打开数据库连接。
		/// </summary>
		/// <param name="connKey">连接配置名称</param>
		public virtual void Open(string connKey = null)
		{
			//初始化连接对象
			if (this.Connection == null) {
				string connString = string.IsNullOrEmpty(connKey) ? (string.IsNullOrEmpty(CurrentConnectionString) ? DefaultConnectionString : CurrentConnectionString) : GetConnectionString(connKey);
				if (!string.IsNullOrEmpty(connString)) {
					this._connection = CreateConnection(connString);
					//检查状态
					if (this.Connection == null)
						throw new DataObjectException("连接对象 Connection 未能初始化，可能是没有配置连接字符串或数据库当前不可用。");
				}
			}
			//初始化指令对象
			if (this.Command == null) {
				this._command = this.Connection.CreateCommand();
			}
			//判断是否已经打开
			if (this.Connection.State == ConnectionState.Open)
				return;
			//执行开启操作
			if (this.Connection.State == ConnectionState.Closed) {
				this.Connection.Open();
			} else if (this.Connection.State == ConnectionState.Broken) {
				this.Connection.Close();
				this.Connection.Open();
			}
		}

		/// <summary>
		/// 关闭数据库连接。
		/// </summary>
		public virtual void Close()
		{
			if (this.Connection != null)
				this.Connection.Close();
		}

		private object _disposeSync = new object();

		/// <summary>
		/// 销毁数据库连接。
		/// </summary>
		public virtual void Dispose()
		{
			lock (_disposeSync) {
				if (this.Connection != null) {
					this.Connection.Close();
					this.Connection.Dispose();
					this.Connection = null;
				}
				if (this.Command != null) {
					this.Command.Dispose();
					this.Command = null;
				}
			}
		}

		#endregion

		#region 查询数据

		/// <summary>
		/// 查询数据库并返回第一行第一列的值。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="type">指令类型</param>
		/// <param name="connKey">连接配置名称</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public virtual object ExecuteScalar(string sql, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms)
		{
			try {
				this.Open(connKey);
				this.Command.CommandType = type;
				this.Command.CommandText = ProcessSqlString(sql);

				JoinParameters(parms, true);
				return this.Command.ExecuteScalar();
			} catch (Exception ex) {
				throw ShowError(ex, sql);
			}
		}

		/// <summary>
		/// 查询数据库并返回第一行第一列的值。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public virtual object ExecuteScalar(string sql, params IDbDataParameter[] parms)
		{
			return ExecuteScalar(sql, CommandType.Text, null, parms);
		}

		/// <summary>
		/// 查询数据库并返回 IDataReader 对象，只读向前，会自动打开数据库连接，但使用后连接不会自动关闭！
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="type">指令类型</param>
		/// <param name="connKey">连接配置名称</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public virtual IDataReader GetDataReader(string sql, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms)
		{
			try {
				this.Open(connKey);
				this.Command.CommandText = ProcessSqlString(sql);
				this.Command.CommandType = type;
				JoinParameters(parms, true);
				IDataReader dr = this.Command.ExecuteReader(CommandBehavior.CloseConnection);
				return dr;
			} catch (Exception ex) {
				throw ShowError(ex, sql);
			}
		}

		/// <summary>
		/// 查询数据库并返回 IDataReader 对象，只读向前，会自动打开数据库连接，但使用后连接不会自动关闭！
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public virtual IDataReader GetDataReader(string sql, params IDbDataParameter[] parms)
		{
			return GetDataReader(sql, CommandType.Text, null, parms);
		}

		/// <summary>
		/// 查询数据库并返回 DataSet 对象。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="type">指令类型</param>
		/// <param name="connKey">连接配置名称</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public virtual DataSet GetDataSet(string sql, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms)
		{
			DataSet ds = new DataSet();
			try {
				this.Open(connKey);
				this.Command.CommandText = ProcessSqlString(sql);
				this.Command.CommandType = type;
				this.Command.CommandTimeout = 60;
				JoinParameters(parms, true);
				this.Adapter = CreateDataAdapter();
				this.Adapter.SelectCommand = this.Command;
				this.Adapter.Fill(ds);
			} catch (Exception ex) {
				throw ShowError(ex, sql);
			} finally {
				this.Close();
			}
			return ds;
		}

		/// <summary>
		/// 查询数据库并返回 DataSet 对象。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public virtual DataSet GetDataSet(string sql, params IDbDataParameter[] parms)
		{
			return GetDataSet(sql, CommandType.Text, null, parms);
		}

		/// <summary>
		/// 查询数据库并返回 DataTable 对象。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="type">指令类型</param>
		/// <param name="connKey">连接配置名称</param>
		/// <param name="parms">参数</param>
		/// <returns>DataTable 对象</returns>
		public virtual DataTable GetDataTable(string sql, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms)
		{
			DataSet ds = GetDataSet(sql, type, connKey, parms);
			return (ds.Tables.Count > 0) ? ds.Tables[0] : null;
		}

		/// <summary>
		/// 查询数据库并返回 DataTable 对象。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="parms">参数</param>
		/// <returns>DataTable 对象</returns>
		public virtual DataTable GetDataTable(string sql, params IDbDataParameter[] parms)
		{
			return GetDataTable(sql, CommandType.Text, null, parms);
		}

		#endregion

		#region 执行语句

		/// <summary>
		/// 执行执行语句，并返回受影响的行数。
		/// </summary>
		/// <param name="connKey">连接配置名称</param>
		/// <param name="type">执行类型</param>
		/// <param name="sql">执行语句</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public virtual int ExecuteNonQuery(string sql, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms)
		{
			try {
				this.Open(connKey);
				this.Command.CommandType = type;
				this.Command.CommandText = ProcessSqlString(sql);
				JoinParameters(parms, true);
				return (int)this.Command.ExecuteNonQuery();
			} catch (Exception ex) {
				throw ShowError(ex, sql);
			}
		}

		/// <summary>
		/// 执行执行语句，并返回受影响的行数。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public virtual int ExecuteNonQuery(string sql, params IDbDataParameter[] parms)
		{
			return ExecuteNonQuery(sql, CommandType.Text, null, parms);
		}

		/// <summary>
		/// 执行新增语句，并返回最新的自增ID值。
		/// </summary>
		/// <param name="connKey">连接配置名称</param>
		/// <param name="type">执行类型</param>
		/// <param name="execute">执行对象</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public int ExecuteInsertIdentity(IExecuteObject execute, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms)
		{
			try {
				this.Open(connKey);
				this.Command.Transaction = this.Connection.BeginTransaction();
				this.Command.CommandType = type;
				this.Command.CommandText = ProcessSqlString(execute.CreateInsertSQL());
				JoinParameters(parms, true);
				this.Command.ExecuteNonQuery();

				this.Command.CommandType = CommandType.Text;
				this.Command.CommandText = execute.CreateIdentitySQL();
				object val = this.Command.ExecuteScalar();
				int dataId = Convert.ToInt32(val);

				this.Command.Transaction.Commit();

				return dataId;
			} catch (Exception ex) {
				this.Command.Transaction.Rollback();
				throw ShowError(ex, execute.ExecuteType + ":" + execute.GetExecuteSQL());
			}
		}

		/// <summary>
		/// 执行新增语句，并返回最新的自增ID值。
		/// </summary>
		/// <param name="execute">执行对象</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public int ExecuteInsertIdentity(IExecuteObject execute, params IDbDataParameter[] parms)
		{
			return ExecuteInsertIdentity(execute, CommandType.Text, null, parms);
		}

		/// <summary>
		/// 批量插入数据。
		/// </summary>
		/// <param name="dataTable">数据表</param>
		/// <param name="tableName">要插入的数据表名称</param>
		/// <param name="connKey">连接配置名称</param>
		public abstract void BatchInsert(DataTable dataTable, string tableName, string connKey = null);

		/// <summary>
		/// 启动一个事务
		/// </summary>
		/// <returns></returns>
		public virtual IDbTransaction BeginTransaction()
		{
			Open();
			this.Command.Transaction = this.Connection.BeginTransaction();
			return this.Command.Transaction;
		}

		#endregion

		#region 克隆新对象

		/// <summary>
		/// 克隆一个新对象
		/// </summary>
		/// <returns></returns>
		public abstract IDbHelper CloneNew();

		#endregion

		#region 常用方法

		/// <summary>
		/// 获取数据值，并预处理
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public abstract object GetDataValue(object val);

		/// <summary>
		/// 创建参数
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public abstract IDbDataParameter CreateParameter(string name, object val);

		#endregion

		#region IDisposable 成员

		void IDisposable.Dispose()
		{
			this.Dispose();
		}

		#endregion

		#region 私有函数

		/// <summary>
		/// 显示出错信息。
		/// </summary>
		/// <param name="ex">异常</param>
		/// <param name="msg">附加信息</param>
		/// <returns></returns>
		private Exception ShowError(Exception ex, string msg)
		{
			return new Exception(ex.Message + "\r\n" + msg, ex);
		}

		/// <summary>
		/// 连接参数
		/// </summary>
		/// <param name="parms">参数集合</param>
		/// <param name="clean">首先清理参数</param>
		private void JoinParameters(IDbDataParameter[] parms, bool clean)
		{
			if (clean) this.Command.Parameters.Clear();
			if (parms != null && parms.Length > 0) {
				foreach (var parm in parms) {
					this.Command.Parameters.Add(parm);
				}
			}
		}

		/// <summary>
		/// 处理传入的字符串。
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		protected virtual string ProcessSqlString(string sql)
		{
			return sql;
		}

		#endregion

	}
}