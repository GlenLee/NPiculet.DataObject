/*
Name: DbHelper's SqlClient Class
Date: 2009-8-17
Author: iSLeeCN
Description: DbHelper's SqlClient Objects.
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace NPiculet.DataObject
{
	/// <summary>
	/// SQL Server 的数据库通用操作辅助类。
	/// </summary>
	public class SqlClientHelper : AbstractDbHelper
	{
		#region 初始化

		public SqlClientHelper(ServerType type, string connString) : base(type, connString)
		{ }

		/// <summary>
		/// 创建数据库连接对象。
		/// </summary>
		/// <param name="connString">连接字符串</param>
		/// <returns>数据库连接对象</returns>
		protected override IDbConnection CreateConnection(string connString)
		{
			return new SqlConnection(connString);
		}

		/// <summary>
		/// 创建数据适配器。
		/// </summary>
		/// <returns>数据适配器</returns>
		protected override IDbDataAdapter CreateDataAdapter()
		{
			return new SqlDataAdapter();
		}

		#endregion

		#region 批量增加数据

		/// <summary>
		/// 批量插入数据。
		/// </summary>
		/// <param name="dataTable">数据表</param>
		/// <param name="tableName">要插入的数据表名称</param>
		/// <param name="connKey">连接配置名称</param>
		public override void BatchInsert(DataTable dataTable, string tableName, string connKey = null)
		{
			if (dataTable.Rows.Count == 0) {
				return;
			}
			try {
				this.Open(connKey);
				//给表名加上前后导符
				using (var bulk = new SqlBulkCopy((SqlConnection)this.Connection, SqlBulkCopyOptions.KeepIdentity, null) {
					DestinationTableName = tableName,
					BatchSize = 10000
				}) {
					bulk.WriteToServer(dataTable);
					bulk.Close();
				}
			} catch (Exception ex) {
				throw;
			}
		}

		#endregion

		#region 特有数据处理方法

		protected override string ProcessSqlString(string sql)
		{
			return sql.Replace("`", "");
		}

		#endregion

		#region 克隆新对象

		/// <summary>
		/// 克隆一个新对象
		/// </summary>
		/// <returns></returns>
		public override IDbHelper CloneNew()
		{
			return new SqlClientHelper(base.CurrentConnectionType, base.CurrentConnectionString);
		}

		#endregion

		#region 常用方法

		/// <summary>
		/// 获取数据值，并预处理
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public override object GetDataValue(object val)
		{
			return val;
		}

		/// <summary>
		/// 创建参数
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public override IDbDataParameter CreateParameter(string name, object val)
		{
			SqlParameter param = new SqlParameter();
			param.ParameterName = name;
			param.Value = val;
			return param;
		}

		#endregion

	}
}
