using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Linq.Mapping;
using System.Reflection;

namespace NPiculet.DataObject
{
	/// <summary>
	/// 数据库的通用操作辅助类。
	/// </summary>
	public abstract class DbHelper
	{
		#region 获取配置

		/// <summary>
		/// 获取数据库连接字符串的索引键名称。
		/// </summary>
		public static string DefaultConnectionKey
		{
			get { return AbstractDbHelper.DefaultConnectionKey; }
		}

		/// <summary>
		/// 获取数据库类型。
		/// </summary>
		/// <returns></returns>
		public static ServerType DefaultConnectionType
		{
			get { return AbstractDbHelper.DefaultConnectionType; }
		}

		/// <summary>
		/// 获取数据库连接字符串
		/// </summary>
		/// <returns></returns>
		public static string DefaultConnectionString
		{
			get { return AbstractDbHelper.DefaultConnectionString; }
		}

		/// <summary>
		/// 获取数据库服务类型
		/// </summary>
		/// <param name="connKey"></param>
		/// <returns></returns>
		private static ServerType GetServerType(string connKey)
		{
			return string.IsNullOrEmpty(connKey) ? AbstractDbHelper.DefaultConnectionType : AbstractDbHelper.GetConnectionType(connKey);
		}

		#endregion

		#region 缓存

		private static ConcurrentDictionary<string, IDbHelper> _helperList = new ConcurrentDictionary<string, IDbHelper>();

		private static ConcurrentDictionary<ServerType, IQueryObject> _queryList = new ConcurrentDictionary<ServerType, IQueryObject>();
		private static ConcurrentDictionary<ServerType, IExecuteObject> _executeList = new ConcurrentDictionary<ServerType, IExecuteObject>();

		/// <summary>
		/// 固定命名空间
		/// </summary>
		private const string namespaceString = "NPiculet.Data.";

		/// <summary>
		/// 数据类型的前缀
		/// </summary>
		private static Dictionary<ServerType, string> _prefixDict = new Dictionary<ServerType, string>()
		{
			{ ServerType.SqlClient, "SqlClient" },
			{ ServerType.OleDb, "MsAccess" },
			{ ServerType.OracleClient, "OracleClient" },
			{ ServerType.OracleDataAccess, "Oracle" },
			{ ServerType.MySql, "MySql" },
			{ ServerType.PgSql, "PgSql" },
			{ ServerType.Sqlite, "Sqlite" }
		};

		/// <summary>
		/// 根据数据库类型获取类型名称
		/// </summary>
		/// <param name="serverType"></param>
		/// <returns></returns>
		private static string GetInstanceClassName(ServerType serverType)
		{
			return namespaceString + _prefixDict[serverType];
		}

		#endregion

		#region 创建数据操作对象

		/// <summary>
		/// 创建数据库实例。
		/// </summary>
		/// <param name="serverType">数据服务器类型</param>
		/// <param name="connString">数据连接字符串</param>
		/// <returns></returns>
		public static IDbHelper Create(ServerType serverType, string connString)
		{
			//检查缓存
			string key = serverType.ToString() + "|" + connString;
			if (_helperList.ContainsKey(key)) {
				return _helperList[key].CloneNew();
			}

			//反射实例化对象
			string className = GetInstanceClassName(serverType) + "Helper";
			Assembly ass = Assembly.GetExecutingAssembly();
			IDbHelper helper = ass.CreateInstance(className, false, BindingFlags.Default, null, new object[] { serverType, connString }, null, null) as IDbHelper;
			if (helper == null) {
				throw new DataObjectException("未能实例化数据库 Helper 对象！");
			}
			if (!_helperList.ContainsKey(key))
				_helperList[key] = helper;
			return helper.CloneNew();
			//throw new DataObjectException("配置文件 ConnectionString 节的 ProviderName 信息不正确！没有实现关于此数据连接类型的 DbHelper 插件！");
		}

		/// <summary>
		/// 创建数据库实例。
		/// </summary>
		/// <param name="connKey">连接配置名称</param>
		/// <returns></returns>
		public static IDbHelper Create(string connKey)
		{
			return Create(AbstractDbHelper.GetConnectionType(connKey), AbstractDbHelper.GetConnectionString(connKey));
		}

		/// <summary>
		/// 创建数据库实例。
		/// </summary>
		/// <returns></returns>
		public static IDbHelper Create()
		{
			return Create(DefaultConnectionType, DefaultConnectionString);
		}

		#endregion

		#region 创建执行对象

		/// <summary>
		/// 创建一个空的执行对象。
		/// </summary>
		/// <returns></returns>
		public static IExecuteObject CreateEmptyExecuteObject(ExecuteType type, string connKey = null)
		{
			ServerType serverType = GetServerType(connKey);
			//检查缓存
			if (_executeList.ContainsKey(serverType)) {
				var empty = _executeList[serverType].CloneEmpty();
				empty.ExecuteType = type;
				return empty;
			}

			//反射实例化对象
			string className = GetInstanceClassName(serverType) + "ExecuteObject";
			Assembly ass = Assembly.GetExecutingAssembly();
			object obj = ass.CreateInstance(className);
			IExecuteObject instance = obj as IExecuteObject;
			if (instance == null) {
				throw new DataObjectException("未能实例化数据库 ExecuteObject 对象！");
			}
			instance.ExecuteType = type;
			if (!_executeList.ContainsKey(serverType))
				_executeList[serverType] = instance;
			return instance;
		}

		/// <summary>
		/// 根据数据模型创建执行对象。
		/// </summary>
		/// <param name="type">执行对象类型</param>
		/// <param name="model">数据模型</param>
		/// <returns></returns>
		public static IExecuteObject CreateExecuteObject(ExecuteType type, object model = null)
		{
			if (model == null) return CreateEmptyExecuteObject(type);

			PropertyInfo[] ps = model.GetType().GetProperties();

			IExecuteObject e = CreateEmptyExecuteObject(type);
			if (e == null) return null;

			e.ExecuteType = type;
			e.TableName = model.ToString();
			int i = e.TableName.LastIndexOf('.');
			if (i > -1)
				e.TableName = e.TableName.Substring(i + 1, e.TableName.Length - i - 1);

			//遍历实体类的所有 PropertyInfo
			foreach (var info in ps) {
				object val = info.GetValue(model, null);
				bool chk = true;
				//检测初始化值是否超出范围
				if (info.PropertyType == typeof(DateTime)) {
					chk = (Convert.ToDateTime(val) >= new DateTime(1753, 1, 1, 12, 0, 0) && Convert.ToDateTime(val) <= new DateTime(9999, 12, 31, 11, 59, 59));
				}
				if (chk) {
					//解析LinQ对象
					//遍历 PropertyInfo 的所有 ColumnAttribute
					foreach (ColumnAttribute attr in info.GetCustomAttributes(typeof(ColumnAttribute), false)) {
						//是主键
						if (attr.IsPrimaryKey) {
							e.PrimaryKey = info.Name;
							e.PrimaryValue = val;
						}
						//由数据库自动生成
						if (attr.IsDbGenerated) {
							e.AutoColumns.Add(info.Name);
						}
						//可写
						if (info.CanWrite) {
							if (val != null) {
								e.Add(info.Name, val);
							}
						}
					}
				}
			}
			return e;
		}

		#endregion

		#region 创建查询对象

		/// <summary>
		/// 创建一个空的执行对象。
		/// </summary>
		/// <returns></returns>
		public static IQueryObject CreateEmptyQueryObject(string connKey = null)
		{
			ServerType serverType = GetServerType(connKey);
			//检查缓存
			if (_queryList.ContainsKey(serverType)) return _queryList[serverType].CloneEmpty();

			//反射实例化对象
			string className = GetInstanceClassName(serverType) + "QueryObject";
			Assembly ass = Assembly.GetExecutingAssembly();
			object obj = ass.CreateInstance(className);
			IQueryObject instance = obj as IQueryObject;
			if (instance == null) {
				throw new DataObjectException("未能实例化数据库 QueryObject 对象！");
			}
			if (!_queryList.ContainsKey(serverType))
				_queryList[serverType] = instance;
			return instance;
		}

		/// <summary>
		/// 根据数据模型创建执行对象。
		/// </summary>
		/// <param name="model">数据模型</param>
		/// <returns></returns>
		public static IQueryObject CreateQueryObject(object model = null)
		{
			if (model == null) return CreateEmptyQueryObject();

			PropertyInfo[] ps = model.GetType().GetProperties();

			IQueryObject q = CreateEmptyQueryObject();
			if (q == null) return null;

			q.TableName = model.ToString();
			int i = q.TableName.LastIndexOf('.');
			if (i > -1)
				q.TableName = q.TableName.Substring(i + 1, q.TableName.Length - i - 1);

			//解析LinQ对象
			foreach (var info in ps) {
				foreach (ColumnAttribute attr in info.GetCustomAttributes(typeof(ColumnAttribute), false)) {
					if (attr.IsPrimaryKey) {
						q.PrimaryKey = info.Name;
					}
					if (info.CanRead) {
						q.Add(info.Name);
					}
				}
			}
			return q;
		}

		/// <summary>
		/// 创建查询参数
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public static IDbDataParameter CreateParameter(string name, object val) {
			using (IDbHelper db = DbHelper.Create()) {
				return db.CreateParameter(name, val);
			}
		}

		#endregion

		#region 查询数据

		/// <summary>
		/// 查询并返回 DataSet 对象。
		/// </summary>
		/// <param name="sql">SQL 语句</param>
		/// <returns></returns>
		public static DataSet QueryDataSet(string sql)
		{
			using (IDbHelper db = DbHelper.Create()) {
				DataSet ds = db.GetDataSet(sql);
				return ds;
			}
		}

		/// <summary>
		/// 查询并返回 DataTable 对象。
		/// </summary>
		/// <param name="sql">SQL 语句</param>
		/// <returns></returns>
		public static DataTable Query(string sql)
		{
			DataSet ds = QueryDataSet(sql);
			return (ds.Tables.Count > 0) ? ds.Tables[0] : null;
		}

		/// <summary>
		/// 查询并返回数据表第一行、第一列的值。
		/// </summary>
		/// <param name="sql">SQL 语句</param>
		/// <returns></returns>
		public static object QueryValue(string sql)
		{
			using (IDbHelper db = DbHelper.Create()) {
				object obj = db.ExecuteScalar(sql);
				return obj;
			}
		}

		#endregion

		#region 执行操作

		/// <summary>
		/// 执行数据库操作，并返回影响行数。
		/// </summary>
		/// <param name="sql">SQL 语句</param>
		/// <param name="type">指令类型</param>
		/// <param name="connKey">数据库连接配置名称</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public static int Execute(string sql, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms)
		{
			using (IDbHelper db = DbHelper.Create(connKey)) {
				int i = db.ExecuteNonQuery(sql, type, connKey, parms);
				return i;
			}
		}

		/// <summary>
		/// 执行数据库操作，并返回影响行数。
		/// </summary>
		/// <param name="sql">SQL 语句</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public static int Execute(string sql, params IDbDataParameter[] parms)
		{
			return Execute(sql, CommandType.Text, null, parms);
		}

		#endregion

		#region 执行队列操作（一次连接）

		/// <summary>
		/// 批量执行数据库操作，如果出错会回滚操作。
		/// </summary>
		/// <param name="queue">SQL 语句队列</param>
		/// <param name="connKey">数据库连接字符串</param>
		/// <param name="type">指令类型</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public static void Execute(Queue<string> queue, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms)
		{
			using (IDbHelper db = DbHelper.Create(connKey)) {
				IDbTransaction tran = db.BeginTransaction();
				try {
					foreach (string sql in queue) {
						db.ExecuteNonQuery(sql, parms);
					}
					tran.Commit();
				} catch (Exception ex) {
					tran.Rollback();
					throw ex;
				}
			}
		}

		/// <summary>
		/// 执行数据库操作，并返回影响行数。
		/// </summary>
		/// <param name="queue">队列语句</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		public static void Execute(Queue<string> queue, params IDbDataParameter[] parms)
		{
			Execute(queue, CommandType.Text, null, parms);
		}

		#endregion

		#region 转换对象

		/// <summary>
		/// 转换 SQLServer 中字段类型。
		/// </summary>
		/// <param name="type">类型名称</param>
		/// <returns></returns>
		public static DataType ConvertSqlServerType(string type)
		{
			DataType result = DataType.None;
			switch (type) {
				case "bit":
					result = DataType.Boolean;
					break;
				case "smallint":
					result = DataType.Int16;
					break;
				case "int":
					result = DataType.Int32;
					break;
				case "bigint":
					result = DataType.Int64;
					break;
				case "tinyint":
					result = DataType.Byte;
					break;
				case "char":
					result = DataType.String;
					break;
				case "nchar":
					result = DataType.String;
					break;
				case "varchar":
					result = DataType.String;
					break;
				case "nvarchar":
					result = DataType.String;
					break;
				case "text":
					result = DataType.Text;
					break;
				case "ntext":
					result = DataType.Text;
					break;
				case "uniqueidentifier":
					result = DataType.Guid;
					break;
				case "datetime":
					result = DataType.DateTime;
					break;
				case "decimal":
					result = DataType.Numeric;
					break;
				case "money":
					result = DataType.Numeric;
					break;
				case "float":
					result = DataType.Numeric;
					break;
			}
			return result;
		}

		#endregion
	}
}
