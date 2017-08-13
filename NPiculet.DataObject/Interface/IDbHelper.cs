/*
名称：DbHelper 类的接口
日期：2011-01-12
作者：李萨
备注：
*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;

namespace NPiculet.DataObject
{
	/// <summary>
	/// 数据库操作接口。这是一个基类，用于初始化数据连接，包含了通用的数据库操作方法。
	/// </summary>
	public partial interface IDbHelper : IDisposable
	{
		#region 初始化参数

		/// <summary>
		/// DbCommand 对象。
		/// </summary>
		IDbCommand Command { get; set; }

		/// <summary>
		/// DbCommection 对象。
		/// </summary>
		IDbConnection Connection { get; set; }

		/// <summary>
		/// DbDataAapter 对象。
		/// </summary>
		IDbDataAdapter Adapter { get; set; }

		#endregion

		#region 通用数据库连接方法

		/// <summary>
		/// 打开数据库连接。
		/// </summary>
		void Open(string connString = null);

		/// <summary>
		/// 关闭数据库连接。
		/// </summary>
		void Close();

		/// <summary>
		/// 销毁数据库连接。
		/// </summary>
		new void Dispose();

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
		object ExecuteScalar(string sql, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms);

		/// <summary>
		/// 查询数据库并返回第一行第一列的值。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="parms">参数</param>
		/// <returns></returns>
		object ExecuteScalar(string sql, params IDbDataParameter[] parms);

		/// <summary>
		/// 查询数据库并返回 IDataReader 对象，只读向前，会自动打开数据库连接，但使用后连接不会自动关闭！
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="type">指令类型</param>
		/// <param name="connKey">连接配置名称</param>
		/// <param name="parms">参数</param>
		/// <returns>IDataReader 对象</returns>
		IDataReader GetDataReader(string sql, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms);

		/// <summary>
		/// 查询数据库并返回 IDataReader 对象，只读向前，会自动打开数据库连接，但使用后连接不会自动关闭！
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="parms">参数</param>
		/// <returns>IDataReader 对象</returns>
		IDataReader GetDataReader(string sql, params IDbDataParameter[] parms);

		/// <summary>
		/// 查询数据库并返回 DataSet 对象。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="type">指令类型</param>
		/// <param name="connKey">连接配置名称</param>
		/// <param name="parms">参数</param>
		/// <returns>DataSet 对象</returns>
		DataSet GetDataSet(string sql, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms);

		/// <summary>
		/// 查询数据库并返回 DataSet 对象。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="parms">参数</param>
		/// <returns>DataSet 对象</returns>
		DataSet GetDataSet(string sql, params IDbDataParameter[] parms);

		/// <summary>
		/// 查询数据库并返回 DataTable 对象。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="type">指令类型</param>
		/// <param name="connKey">连接配置名称</param>
		/// <param name="parms">参数</param>
		/// <returns>DataTable 对象</returns>
		DataTable GetDataTable(string sql, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms);

		/// <summary>
		/// 查询数据库并返回 DataTable 对象。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="parms">参数</param>
		/// <returns>DataTable 对象</returns>
		DataTable GetDataTable(string sql, params IDbDataParameter[] parms);

		#endregion

		#region 执行语句

		/// <summary>
		/// 执行执行语句，并返回受影响的行数。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="type">指令类型</param>
		/// <param name="connKey">连接配置名称</param>
		/// <param name="parms">参数</param>
		/// <returns>受影响的行数</returns>
		int ExecuteNonQuery(string sql, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms);

		/// <summary>
		/// 执行执行语句，并返回受影响的行数。
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <param name="parms">参数</param>
		/// <returns>受影响的行数</returns>
		int ExecuteNonQuery(string sql, params IDbDataParameter[] parms);

		/// <summary>
		/// 执行新增语句，并返回自增ID
		/// </summary>
		/// <param name="execute">执行对象</param>
		/// <param name="type">指令类型</param>
		/// <param name="connKey">连接配置名称</param>
		/// <param name="parms">参数</param>
		/// <returns>自增ID</returns>
		int ExecuteInsertIdentity(IExecuteObject execute, CommandType type = CommandType.Text, string connKey = null, params IDbDataParameter[] parms);

		/// <summary>
		/// 执行执行语句，并返回受影响的行数。
		/// </summary>
		/// <param name="execute">执行对象</param>
		/// <param name="parms">参数</param>
		/// <returns>自增ID</returns>
		int ExecuteInsertIdentity(IExecuteObject execute, params IDbDataParameter[] parms);

		/// <summary>
		/// 批量插入数据。
		/// </summary>
		/// <param name="dataTable">数据集</param>
		/// <param name="tableName">要插入的数据表名称</param>
		/// <param name="connKey">连接配置名称</param>
		void BatchInsert(DataTable dataTable, string tableName, string connKey = null);

		/// <summary>
		/// 开启事务模式并返回事务对象。
		/// </summary>
		/// <returns></returns>
		IDbTransaction BeginTransaction();

		#endregion

		#region 克隆新对象

		/// <summary>
		/// 克隆一个新对象。
		/// </summary>
		/// <returns></returns>
		IDbHelper CloneNew();

		#endregion

		#region 常用方法

		/// <summary>
		/// 获取数据值，并预处理
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		object GetDataValue(object val);

		/// <summary>
		/// 创建参数
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IDbDataParameter CreateParameter(string name, object val);

		#endregion
	}
}
