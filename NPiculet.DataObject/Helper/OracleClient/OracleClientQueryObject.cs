﻿using System;

namespace NPiculet.DataObject
{
	public class OracleClientQueryObject : AbstractQueryObject
	{
		/// <summary>
		/// 参数标识符。
		/// </summary>
		public override char ParmToken
		{
			get { return ':'; }
		}

		public override string Wrap(string field)
		{
			return (field.IndexOf("\"") > -1) ? field : "\"" + field + "\"";
		}

		/// <summary>
		/// 获取统计字符串。
		/// </summary>
		/// <returns></returns>
		public override string GetCountString()
		{
			string sql = "SELECT COUNT(*) FROM " + this.TableName;
			if (this.Where.Length > 0) { sql += " WHERE " + this.Where; }
			return sql;
		}

		/// <summary>
		/// 获取最大值字符串。
		/// </summary>
		/// <returns></returns>
		public override string GetMaxString()
		{
			if (this.Fields.Count > 0) {
				string sql = "SELECT MAX(" + this.Fields[0].Key + ") FROM " + this.TableName;
				if (this.Where.Length > 0) { sql += " WHERE " + this.Where; }
				return sql;
			}
			return String.Empty;
		}

		/// <summary>
		/// 获取最小值字符串。
		/// </summary>
		/// <returns></returns>
		public override string GetMinString()
		{
			if (this.Fields.Count > 0) {
				string sql = "SELECT MIN(" + this.Fields[0].Key + ") FROM " + this.TableName;
				if (this.Where.Length > 0) { sql += " WHERE " + this.Where; }
				return sql;
			}
			return String.Empty;
		}

		/// <summary>
		/// 获取统计字符串。
		/// </summary>
		/// <returns></returns>
		public override string GetSumString()
		{
			//组合查询字段
			if (this.Fields.Count > 0) {
				string sql = "SELECT NVL(SUM(" + this.Fields[0].Key + "), 0) FROM " + this.TableName;
				if (this.Where.Length > 0) { sql += " WHERE " + this.Where; }
				return sql;
			}
			return String.Empty;
		}

		/// <summary>
		/// 获取查询字符串,没有定义主键时,必须指定排序字段。
		/// </summary>
		/// <returns></returns>
		public override string GetQueryString()
		{
			//组合查询字段
			string fields = String.Empty;
			if (this.Fields.Count == 0) {
				fields = "*";
			} else {
				//组合
				foreach (Field field in this.Fields) {
					if (fields.Length != 0) fields += ",";
					fields += Wrap(field.Key);
					if (!String.IsNullOrEmpty(field.Alias))
						fields += " AS " + Wrap(field.Alias);
				}
			}
			//初始化
			string sql = "SELECT " + fields + " FROM ";
			//判断分页
			if (this.PageSize == 0) {

				#region 不需要分页

				//不需分页
				if (string.IsNullOrWhiteSpace(this.OrderBy)) {
					//不需排序
					if (string.IsNullOrWhiteSpace(this.PrimaryKey)) {
						//无主键
						sql += this.TableName;
						if (string.IsNullOrWhiteSpace(this.Where)) {
							//无过滤
						} else {
							//有过滤
							sql += " WHERE " + this.Where;
						}
					} else {
						//有主键
						sql += this.TableName;
						if (string.IsNullOrWhiteSpace(this.Where)) {
							//无过滤
						} else {
							//有过滤
							sql += " WHERE " + this.Where;
						}
						sql += " ORDER BY " + this.PrimaryKey;
					}
				} else {
					//需排序
					if (string.IsNullOrWhiteSpace(this.PrimaryKey)) {
						//无主键
						sql += this.TableName;
						if (string.IsNullOrWhiteSpace(this.Where)) {
							//无过滤
						} else {
							//有过滤
							sql += " WHERE " + this.Where;
						}
						sql += " ORDER BY " + this.OrderBy;
					} else {
						//有主键
						sql += this.TableName;
						if (string.IsNullOrWhiteSpace(this.Where)) {
							//无过滤
						} else {
							//有过滤
							sql += " WHERE " + this.Where;
						}
						sql += " ORDER BY " + this.OrderBy + ", " + this.PrimaryKey;
					}
				}

				#endregion

			} else {

				#region 需要分页

				//需分页
				int i = (this.CurrentPage - 1) * this.PageSize, s = i + 1, e = i + this.PageSize;

				if (string.IsNullOrWhiteSpace(this.OrderBy)) {
					//不需排序
					if (string.IsNullOrWhiteSpace(this.PrimaryKey)) {
						//无主键
						sql += string.Format(@"(SELECT T.*, ROWNUM RN FROM (SELECT * FROM {0}{1}) T WHERE ROWNUM <= {3}) WHERE RN >= {2}"
							, this.TableName
							, string.IsNullOrWhiteSpace(this.Where) ? "" : " WHERE " + this.Where
							, s
							, e
						);
					} else {
						//有主键
						sql += string.Format(@"(SELECT T.*, ROWNUM RN FROM (SELECT * FROM {0}{1} ORDER BY {4}) T WHERE ROWNUM <= {3}) WHERE RN >= {2} ORDER BY {4}"
							, this.TableName
							, string.IsNullOrWhiteSpace(this.Where) ? "" : " WHERE " + this.Where
							, s
							, e
							, this.PrimaryKey
						);
					}
				} else {
					//需排序
					if (string.IsNullOrWhiteSpace(this.PrimaryKey)) {
						//无主键
						sql += string.Format(@"(SELECT T.*, ROWNUM RN FROM (SELECT * FROM {0}{1} ORDER BY {4}) T WHERE ROWNUM <= {3}) WHERE RN >= {2} ORDER BY {4}"
							, this.TableName
							, string.IsNullOrWhiteSpace(this.Where) ? "" : " WHERE " + this.Where
							, s
							, e
							, this.OrderBy
						);
					} else {
						//有主键
						sql += string.Format(@"(SELECT T.*, ROWNUM RN FROM (SELECT * FROM {0}{1} ORDER BY {4}, {5}) T WHERE ROWNUM <= {3}) WHERE RN >= {2} ORDER BY {4}, {5}"
							, this.TableName
							, string.IsNullOrWhiteSpace(this.Where) ? "" : " WHERE " + this.Where
							, s
							, e
							, this.OrderBy
							, this.PrimaryKey
						);
					}
				}

				#endregion

			}
			return sql;
		}

		public override IQueryObject CloneEmpty()
		{
			return new OracleClientQueryObject();
		}
	}
}