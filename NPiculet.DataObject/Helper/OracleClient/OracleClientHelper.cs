using System;
using System.Data;
using System.Data.OracleClient;

namespace NPiculet.DataObject
{
	public class OracleClientHelper : AbstractDbHelper
	{
		#region 初始化

		public OracleClientHelper(ServerType type, string connString) : base(type, connString)
		{ }

		/// <summary>
		/// 创建数据库连接对象。
		/// </summary>
		/// <param name="connString">连接字符串</param>
		/// <returns>数据库连接对象</returns>
		protected override IDbConnection CreateConnection(string connString)
		{
			return new OracleConnection(connString);
		}

		/// <summary>
		/// 创建数据适配器。
		/// </summary>
		/// <returns>数据适配器</returns>
		protected override IDbDataAdapter CreateDataAdapter()
		{
			return new OracleDataAdapter();
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
			var sql = string.Empty;
			try {
				this.Open(connKey);
				this.Command.CommandType = CommandType.Text;
				this.Command.CommandText = "SELECT * FROM " + tableName;

				var dt = dataTable.Copy();
				foreach (DataRow dr in dt.Rows) {
					if (dr.RowState == DataRowState.Unchanged) dr.SetAdded();
				}
				this.Command.Transaction = this.Connection.BeginTransaction();
				var da = new OracleDataAdapter((OracleCommand)this.Command);
				var cb = new OracleCommandBuilder(da);
				da.InsertCommand = cb.GetInsertCommand();
				da.Update(dt);
				this.Command.Transaction.Commit();
			} catch (Exception ex) {
				throw new DataObjectException("批量插入数据时出现错误：" + ex.Message + "\r\n" + sql, ex);
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
			return new OracleClientHelper(base.CurrentConnectionType, base.CurrentConnectionString);
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
			OracleParameter param = new OracleParameter();
			param.ParameterName = name;
			param.Value = val;
			return param;
		}

		#endregion

	}
}