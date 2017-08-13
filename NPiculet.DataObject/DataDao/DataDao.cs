using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using NPiculet.DataObject;

namespace NPiculet.DataObject
{
	public class DataDao
	{
		#region 返回数据集

		/// <summary>
		/// 根据实体类查询并返回数据集
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="whereString"></param>
		/// <param name="orderBy"></param>
		/// <param name="parms"></param>
		/// <returns></returns>
		public DataTable Query<T>(string whereString = null, string orderBy = null, params IDbDataParameter[] parms) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				var model = new T();
				var query = DbHelper.CreateQueryObject();
				query.TableName = model.TableName;
				query.PrimaryKey = model.PrimeKey;
				query.Where = whereString;
				query.OrderBy = orderBy;
				return db.GetDataTable(query.GetQueryString(), parms);
			}
		}

		/// <summary>
		/// 根据实体类查询并返回数据集
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="curPage"></param>
		/// <param name="pageSize"></param>
		/// <param name="whereString"></param>
		/// <param name="orderBy"></param>
		/// <param name="parms"></param>
		/// <returns></returns>
		public DataTable Query<T>(int curPage, int pageSize = 10, string whereString = null, string orderBy = null, params IDbDataParameter[] parms) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				var model = new T();
				var query = DbHelper.CreateQueryObject();
				query.TableName = model.TableName;
				query.PrimaryKey = model.PrimeKey;
				query.CurrentPage = curPage;
				query.PageSize = pageSize;
				query.Where = whereString;
				query.OrderBy = orderBy;
				return db.GetDataTable(query.GetQueryString(), parms);
			}
		}

		/// <summary>
		/// 查询 SQL 语句
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="parms"></param>
		/// <returns></returns>
		public DataTable QuerySql(string sql, params IDbDataParameter[] parms)
		{
			using (IDbHelper db = DbHelper.Create()) {
				return db.GetDataTable(sql, parms);
			}
		}

		#endregion

		#region 返回实体类

		/// <summary>
		/// 查询单实体类
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="whereString"></param>
		/// <param name="parms"></param>
		/// <returns></returns>
		public T QueryModel<T>(string whereString, params IDbDataParameter[] parms) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				var model = new T();
				var query = DbHelper.CreateQueryObject();
				query.TableName = model.TableName;
				query.Where = whereString;

				using (IDataReader dr = db.GetDataReader(query.GetQueryString(), parms)) {
					if (dr.Read()) {
						Fill(db, model, dr);
						model.ClearPropertyChange();
						return model;
					}
				}
				return default(T);
			}
		}

		/// <summary>
		/// 查询实体集合
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="curPage"></param>
		/// <param name="pageSize"></param>
		/// <param name="whereString"></param>
		/// <param name="orderBy"></param>
		/// <param name="parms"></param>
		/// <returns></returns>
		public List<T> QueryList<T>(int curPage, int pageSize = 10, string whereString = null, string orderBy = null, params IDbDataParameter[] parms) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				List<T> list = new List<T>();
				var query = DbHelper.CreateQueryObject();
				T t = new T();
				query.TableName = t.TableName;
				query.PrimaryKey = t.PrimeKey;
				query.CurrentPage = curPage;
				query.PageSize = pageSize;
				query.Where = whereString;
				query.OrderBy = orderBy;

				using (IDataReader dr = db.GetDataReader(query.GetQueryString(), parms)) {
					while (dr.Read()) {
						var model = new T();
						Fill(db, model, dr);
						model.ClearPropertyChange();
						list.Add(model);
					}
				}

				return list;
			}
		}

		/// <summary>
		/// 查询实体集合
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="whereString"></param>
		/// <param name="orderBy"></param>
		/// <param name="parms"></param>
		/// <returns></returns>
		public List<T> QueryList<T>(string whereString, string orderBy = null, params IDbDataParameter[] parms) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				List<T> list = new List<T>();
				var query = DbHelper.CreateQueryObject();
				T t = new T();
				query.TableName = t.TableName;
				query.PrimaryKey = t.PrimeKey;
				query.Where = whereString;
				query.OrderBy = orderBy;

				using (IDataReader dr = db.GetDataReader(query.GetQueryString(), parms)) {
					while (dr.Read()) {
						var model = new T();
						Fill(db, model, dr);
						model.ClearPropertyChange();
						list.Add(model);
					}
				}

				return list;
			}
		}

		/// <summary>
		/// 根据 SQL 预计查询的结果填充实体集合
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sql"></param>
		/// <param name="parms"></param>
		/// <returns></returns>
		public List<T> QueryListBySql<T>(string sql, params IDbDataParameter[] parms) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				List<T> list = new List<T>();
				using (IDataReader dr = db.GetDataReader(sql, parms)) {
					while (dr.Read()) {
						var model = new T();
						Fill(db, model, dr);
						model.ClearPropertyChange();
						list.Add(model);
					}
				}

				return list;
			}
		}

		/// <summary>
		/// 填充实体类
		/// </summary>
		/// <param name="db"></param>
		/// <param name="model"></param>
		/// <param name="dr"></param>
		private void Fill(IDbHelper db, object model, IDataReader dr)
		{
			if (model != null) {
				Type type = model.GetType();
				PropertyInfo[] infos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

				foreach (PropertyInfo info in infos) {
					//属性可写
					if (info.CanWrite) {
						Debug.Print(info.Name);
						try {
							object val = db.GetDataValue(dr[info.Name]);
							if (val != null && val != DBNull.Value)
								info.SetValue(model, val, null);
						} catch (Exception ex) {
							Debug.Print("DataDao 绑定时出现错误：" + ex.Message);
						}
					}
				}
			}
		}

		/// <summary>
		/// 获取记录统计数字
		/// </summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <param name="whereString">查询语句</param>
		/// <param name="parms">参数值</param>
		/// <returns></returns>
		public int RecordCount<T>(string whereString, params IDbDataParameter[] parms) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				var model = new T();
				var query = DbHelper.CreateQueryObject();
				query.TableName = model.TableName;
				query.Where = whereString;
				return Convert.ToInt32(db.ExecuteScalar(query.GetCountString(), parms));
			}
		}

		#endregion

		#region 删除、增加和修改

		/// <summary>
		/// 解析执行对象。
		/// </summary>
		/// <param name="db">数据库对象</param>
		/// <param name="e">执行对象</param>
		/// <returns></returns>
		private static IDbDataParameter[] ResolveExecuteObject(IDbHelper db, IExecuteObject e)
		{
			var parms = new List<IDbDataParameter>();
			db.Open();
			if (db.Command != null) {
				for (int i = 0; i < e.Fields.Count; i++) {
					Field field = e.Fields[i];
					//判断传入值不为null，否则会导致类型转换出错）
					if (field.Value != null) {
						//传入存储过程变量
						IDbDataParameter parm = db.Command.CreateParameter();
						parm.ParameterName = e.ParmToken + field.Key;
						parm.Value = field.Value;

						parms.Add(parm);
					}
				}
			}
			return parms.ToArray();
		}

		/// <summary>
		/// 删除多个实体对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="model"></param>
		/// <returns></returns>
		public bool Delete<T>(T model) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				var execute = DbHelper.CreateExecuteObject(ExecuteType.Delete);
				execute.TableName = model.TableName;
				execute.PrimaryKey = model.PrimeKey;
				execute.AutoColumns.Add(model.PrimeKey);

				Type type = model.GetType();
				//处理主键
				if (!string.IsNullOrEmpty(model.PrimeKey)) {
					PropertyInfo info = type.GetProperty(model.PrimeKey, BindingFlags.Public | BindingFlags.Instance);
					execute.PrimaryValue = info.GetValue(model, null);
					execute.Add(execute.PrimaryKey, execute.PrimaryValue);
				}
				IDbDataParameter[] parms = ResolveExecuteObject(db, execute);
				string sql = execute.CreateDeleteSQL();
				db.ExecuteNonQuery(sql, parms);
				return true;
			}
		}

		public bool Delete<T>(string whereString, params IDbDataParameter[] parms) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				var model = new T();
				var execute = DbHelper.CreateExecuteObject(ExecuteType.Delete);
				execute.TableName = model.TableName;
				execute.Where = whereString;
				return db.ExecuteNonQuery(execute.CreateDeleteSQL(), parms) > 0;
			}
		}

		public bool Insert<T>(T model) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				var execute = DbHelper.CreateExecuteObject(ExecuteType.Insert);
				execute.TableName = model.TableName;
				execute.PrimaryKey = model.PrimeKey;
				execute.AutoColumns.Add(model.PrimeKey);

				Type type = model.GetType();
				var properties = model.PropertyChangedList;
				foreach (var property in properties) {
					//属性可写
					PropertyInfo info = type.GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
					if (info.CanWrite) {
						var val = info.GetValue(model, null);
						if (val != null) {
							execute.Add(info.Name, val);
						}
					}
				}
				IDbDataParameter[] parms = ResolveExecuteObject(db, execute);
				string sql = execute.CreateInsertSQL();
				return db.ExecuteNonQuery(sql, parms) > 0;
			}
		}

		/// <summary>
		/// 新增数据后获取自增ID
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="model"></param>
		/// <returns></returns>
		public int InsertIdentity<T>(T model) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				var execute = DbHelper.CreateExecuteObject(ExecuteType.Insert);
				execute.TableName = model.TableName;
				execute.PrimaryKey = model.PrimeKey;
				execute.AutoColumns.Add(model.PrimeKey);

				Type type = model.GetType();
				var properties = model.PropertyChangedList;
				foreach (var property in properties) {
					//属性可写
					PropertyInfo info = type.GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
					if (info.CanWrite) {
						var val = info.GetValue(model, null);
						if (val != null) {
							execute.Add(info.Name, val);
						}
					}
				}
				IDbDataParameter[] parms = ResolveExecuteObject(db, execute);
				return db.ExecuteInsertIdentity(execute, parms);
			}
		}

		public bool Update<T>(T model, string whereString = null) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				var execute = DbHelper.CreateExecuteObject(ExecuteType.Update);
				execute.TableName = model.TableName;
				execute.PrimaryKey = model.PrimeKey;
				execute.Where = whereString;
				execute.AutoColumns.Add(model.PrimeKey);
				
				Type type = model.GetType();
				var properties = model.PropertyChangedList;
				//处理主键
				if (!properties.Contains(model.PrimeKey)) {
					PropertyInfo info = type.GetProperty(model.PrimeKey, BindingFlags.Public | BindingFlags.Instance);
					execute.PrimaryValue = info.GetValue(model, null);
					execute.Add(execute.PrimaryKey, execute.PrimaryValue);
				}
				//处理其他属性
				foreach (var property in properties) {
					PropertyInfo info = type.GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
					//属性可写
					if (info.CanWrite) {
						if (info.Name == model.PrimeKey) execute.PrimaryValue = info.GetValue(model, null);
						var val = info.GetValue(model, null);
						if (val != null) {
							execute.Add(info.Name, val);
						}
					}
				}
				IDbDataParameter[] parms = ResolveExecuteObject(db, execute);
				string sql = execute.CreateUpdateSQL();
				return db.ExecuteNonQuery(sql, parms) > 0;
			}
		}

		#endregion

		#region 扩展增删改方法

		/// <summary>
		/// 新增多个实体对象。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="models"></param>
		/// <returns></returns>
		public bool InsertList<T>(List<T> models) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				IDbTransaction tran = db.BeginTransaction();
				try {
					var list = models;
					foreach (T model in list) {
						var execute = DbHelper.CreateExecuteObject(ExecuteType.Insert);
						execute.TableName = model.TableName;
						execute.PrimaryKey = model.PrimeKey;
						execute.AutoColumns.Add(model.PrimeKey);

						Type type = model.GetType();
						var properties = model.PropertyChangedList;
						foreach (var property in properties) {
							//属性可写
							PropertyInfo info = type.GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
							if (info.CanWrite) {
								var val = info.GetValue(model, null);
								if (val != null) {
									execute.Add(info.Name, val);
								}
							}
						}
						IDbDataParameter[] parms = ResolveExecuteObject(db, execute);
						string sql = execute.CreateInsertSQL();
						db.ExecuteNonQuery(sql, parms);
					}
					tran.Commit();
					return true;
				} catch (Exception ex) {
					tran.Rollback();
					throw ex;
				}
			}
		}

		/// <summary>
		/// 更新多个实体对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="models"></param>
		/// <returns></returns>
		public bool UpdateList<T>(List<T> models) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				IDbTransaction tran = db.BeginTransaction();
				try {
					var list = models;
					foreach (T model in list) {
						var execute = DbHelper.CreateExecuteObject(ExecuteType.Update);
						execute.TableName = model.TableName;
						execute.PrimaryKey = model.PrimeKey;
						execute.AutoColumns.Add(model.PrimeKey);

						Type type = model.GetType();
						var properties = model.PropertyChangedList;
						//处理主键
						if (!properties.Contains(model.PrimeKey)) {
							PropertyInfo info = type.GetProperty(model.PrimeKey, BindingFlags.Public | BindingFlags.Instance);
							execute.PrimaryValue = info.GetValue(model, null);
							execute.Add(execute.PrimaryKey, execute.PrimaryValue);
						}
						//处理其他属性
						foreach (var property in properties) {
							PropertyInfo info = type.GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
							//属性可写
							if (info.CanWrite) {
								if (info.Name == model.PrimeKey) execute.PrimaryValue = info.GetValue(model, null);
								var val = info.GetValue(model, null);
								if (val != null) {
									execute.Add(info.Name, val);
								}
							}
						}
						IDbDataParameter[] parms = ResolveExecuteObject(db, execute);
						string sql = execute.CreateUpdateSQL();
						db.ExecuteNonQuery(sql, parms);
					}
					tran.Commit();
					return true;
				} catch (Exception ex) {
					tran.Rollback();
					throw ex;
				}
			}
		}

		/// <summary>
		/// 删除多个实体对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="models"></param>
		/// <returns></returns>
		public bool DeleteList<T>(List<T> models) where T : IModel, new()
		{
			using (IDbHelper db = DbHelper.Create()) {
				IDbTransaction tran = db.BeginTransaction();
				try {
					var list = models;
					foreach (T model in list) {
						var execute = DbHelper.CreateExecuteObject(ExecuteType.Delete);
						execute.TableName = model.TableName;
						execute.PrimaryKey = model.PrimeKey;
						execute.AutoColumns.Add(model.PrimeKey);

						Type type = model.GetType();
						//处理主键
						if (!string.IsNullOrEmpty(model.PrimeKey)) {
							PropertyInfo info = type.GetProperty(model.PrimeKey, BindingFlags.Public | BindingFlags.Instance);
							execute.PrimaryValue = info.GetValue(model, null);
							execute.Add(execute.PrimaryKey, execute.PrimaryValue);
						}
						IDbDataParameter[] parms = ResolveExecuteObject(db, execute);
						string sql = execute.CreateDeleteSQL();
						db.ExecuteNonQuery(sql, parms);
					}
					return true;
				} catch (Exception ex) {
					tran.Rollback();
					throw ex;
				}
			}
		}

		#endregion
	}
}
