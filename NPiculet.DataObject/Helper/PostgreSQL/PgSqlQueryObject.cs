using System;
using System.Text.RegularExpressions;

namespace NPiculet.DataObject
{
	public class PgSqlQueryObject : AbstractQueryObject
	{
		/// <summary>
		/// 参数标识符。
		/// </summary>
		public override char ParmToken
		{
			get { return '@'; }
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
			ProcessParameterString();
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
			ProcessParameterString();
			if (this.Fields.Count > 0) {
				string sql = "SELECT MAX(" + Wrap(this.Fields[0].Key) + ") FROM " + this.TableName;
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
			ProcessParameterString();
			if (this.Fields.Count > 0) {
				string sql = "SELECT MIN(" + Wrap(this.Fields[0].Key) + ") FROM " + this.TableName;
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
			ProcessParameterString();
			//组合查询字段
			if (this.Fields.Count > 0) {
				string sql = "SELECT IfNull(SUM(" + Wrap(this.Fields[0].Key) + "), 0) FROM " + this.TableName;
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
			ProcessParameterString();
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
						sql += " ORDER BY " + Wrap(this.PrimaryKey);
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
						sql += " ORDER BY " + this.OrderBy + ", " + Wrap(this.PrimaryKey);
					}
				}

				#endregion

			} else {

				#region 需要分页

				//需分页
				if (string.IsNullOrWhiteSpace(this.OrderBy)) {
					//不需排序
					sql += this.TableName;
					//过滤条件
					if (string.IsNullOrWhiteSpace(this.Where)) {
						//无过滤
					} else {
						//有过滤
						sql += " WHERE " + this.Where;
					}
					if (string.IsNullOrWhiteSpace(this.PrimaryKey)) {
						//无主键
					} else {
						//有主键
						sql += " ORDER BY " + Wrap(this.PrimaryKey);
					}
					//分页
					sql += string.Format(@" LIMIT {1} OFFSET {0}"
						, ((this.CurrentPage - 1) * this.PageSize)
						, this.PageSize
					);
				} else {
					//需排序
					sql += this.TableName;
					//过滤条件
					if (string.IsNullOrWhiteSpace(this.Where)) {
						//无过滤
					} else {
						//有过滤
						sql += " WHERE " + this.Where;
					}
					if (string.IsNullOrWhiteSpace(this.PrimaryKey)) {
						//无主键
						sql += " ORDER BY " + this.OrderBy;
					} else {
						//有主键
						sql += " ORDER BY " + this.OrderBy + ", " + Wrap(this.PrimaryKey);
					}
					//分页
					sql += string.Format(@" LIMIT {1} OFFSET {0}"
						, ((this.CurrentPage - 1) * this.PageSize)
						, this.PageSize
					);
				}

				#endregion

			}
			return sql;
		}

		public override IQueryObject CloneEmpty()
		{
			return new PgSqlQueryObject();
		}

		/// <summary>
		/// 针对 PostgreSQL 大小写敏感问题，处理 Where 字符串中字段名称。
		/// </summary>
		private void ProcessParameterString()
		{
			//处理 WhereString 中的字段
			if (!string.IsNullOrWhiteSpace(this.Where)) {
				this.Where = this.Where.Replace("`", "\"");
				//注意首字母必须是英文
				Regex wx = new Regex(@"([a-zA-Z][a-zA-Z0-9_]*)( *)(=|>|<| LIKE | NOT | IS )", RegexOptions.IgnoreCase);
				this.Where = wx.Replace(this.Where, "\"$1\"$2$3");
			}
			//处理 OrderBy 中的字段
			if (!string.IsNullOrWhiteSpace(this.OrderBy)) {
				this.OrderBy = this.OrderBy.Replace("`", "\"");
				//获取每个排序字段
				string[] orders = this.OrderBy.Split(',');

				string orderString = "";
				for (int i = 0; i < orders.Length; i++) {
					Regex ox = new Regex(" +");
					string orderField = ox.Replace(orders[i].Trim(), " "); //处理字段空格
					string[] os = orderField.Split(' '); //分离字段及排序关键字
					if (!string.IsNullOrEmpty(orderString)) orderString += ","; //组合字段分隔符
					orderString += Wrap(os[0]); //组合字段名
					if (os.Length > 1) orderString += " " + os[1]; //组合排序关键字
				}

				this.OrderBy = orderString;
			}
		}
	}
}