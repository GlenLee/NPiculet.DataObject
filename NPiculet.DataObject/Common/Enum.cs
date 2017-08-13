/*
名称：DbHelper 类的枚举类型
日期：2011-01-12
作者：李萨
备注：
*/
namespace NPiculet.DataObject
{
	/// <summary>
	/// 数据库类型。
	/// </summary>
	public enum ServerType
	{
		/// <summary>
		/// 未设置。
		/// </summary>
		None,
		/// <summary>
		/// Microsoft SQL Server 数据库的连接方式。
		/// </summary>
		SqlClient,
		/// <summary>
		/// Microsoft Access 数据库的连接方式。
		/// </summary>
		OleDb,
		/// <summary>
		/// Oracle 数据库的连接方式。
		/// </summary>
		OracleClient,
		OracleDataAccess,
		/// <summary>
		/// ODBC 数据库的连接方式。
		/// </summary>
		Odbc,
		/// <summary>
		/// MySQL 数据库的连接方式。
		/// </summary>
		MySql,
		/// <summary>
		/// SQLite 数据库的连接方式。
		/// </summary>
		Sqlite,
		/// <summary>
		/// PostgreSQL 数据库的连接方式
		/// </summary>
		PgSql
	}

	/// <summary>
	/// 执行类型。
	/// </summary>
	public enum ExecuteType
	{
		/// <summary>
		/// 新增。
		/// </summary>
		Insert,
		/// <summary>
		/// 更新。
		/// </summary>
		Update,
		/// <summary>
		/// 删除。
		/// </summary>
		Delete,
		/// <summary>
		/// 不做操作。
		/// </summary>
		None
	}

	/// <summary>
	/// 删除模式。
	/// </summary>
	public enum DeleteMode
	{
		/// <summary>
		/// 物理删除。
		/// </summary>
		Physical,
		/// <summary>
		/// 标记删除。
		/// </summary>
		Mark
	}
}
