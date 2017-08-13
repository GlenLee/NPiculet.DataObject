using System;
using System.Text;
using NPiculet.DataObject;

namespace NPiculet.DataObject
{
	public class MySqlExecuteObject : AbstractExecuteObject
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
			return field;
		}

		/// <summary>
		/// 创建新增字符串。
		/// </summary>
		/// <returns></returns>
		public override string CreateInsertSQL()
		{
			StringBuilder sqlBuilder = new StringBuilder();
			sqlBuilder.AppendFormat("INSERT INTO {0} (", this.TableName);
			bool isFirstRow = true;
			foreach (Field field in this.Fields) {
				if (!this.AutoColumns.Contains(field.Key)) {
					if (isFirstRow) {
						sqlBuilder.AppendFormat("{0}", Wrap(field.Key));
						isFirstRow = false;
					} else {
						sqlBuilder.AppendFormat(",{0}", Wrap(field.Key));
					}
				}
			}
			sqlBuilder.Append(")VALUES(");
			isFirstRow = true;
			foreach (Field field in this.Fields) {
				if (!this.AutoColumns.Contains(field.Key)) {
					if (isFirstRow) {
						sqlBuilder.AppendFormat("{0}{1}", ParmToken, field.Key);
						isFirstRow = false;
					} else {
						sqlBuilder.AppendFormat(",{0}{1}", ParmToken, field.Key);
					}
				}
			}
			sqlBuilder.Append(")");
			return sqlBuilder.ToString();
		}

		/// <summary>
		/// 创建更新字符串。
		/// </summary>
		/// <returns></returns>
		public override string CreateUpdateSQL()
		{
			StringBuilder sqlBuilder = new StringBuilder();
			sqlBuilder.AppendFormat("UPDATE {0} SET ", this.TableName);
			bool isFirstRow = true;
			foreach (Field field in this.Fields) {
				if (!this.AutoColumns.Contains(field.Key)) {
					if (isFirstRow) {
						sqlBuilder.AppendFormat("{0}={1}", Wrap(field.Key), ParmToken + field.Key);
						isFirstRow = false;
					} else {
						sqlBuilder.AppendFormat(",{0}={1}", Wrap(field.Key), ParmToken + field.Key);
					}
				}
			}
			if (!string.IsNullOrWhiteSpace(this.Where)) {
				sqlBuilder.AppendFormat(" WHERE {0}", this.Where);
			} else {
				if (String.IsNullOrEmpty(this.PrimaryKey) || this.PrimaryValue == null) {
					throw new DataObjectException("禁止更新全表数据！请检查主键或过滤条件，如确实需要更新全表，可使用 1=1 作为条件。");
				}
				sqlBuilder.AppendFormat(" WHERE {0}={1}{0}", this.PrimaryKey, this.ParmToken);
			}
			return sqlBuilder.ToString();
		}

		/// <summary>
		/// 创建删除字符串。
		/// </summary>
		/// <returns></returns>
		public override string CreateDeleteSQL()
		{
			StringBuilder sqlBuilder = new StringBuilder();
			sqlBuilder.AppendFormat("DELETE FROM {0} ", this.TableName);
			if (!string.IsNullOrWhiteSpace(this.Where)) {
				sqlBuilder.AppendFormat(" WHERE {0}", this.Where);
			} else {
				if (String.IsNullOrEmpty(this.PrimaryKey) || this.PrimaryValue == null) {
					throw new DataObjectException("禁止删除全表数据！请检查主键或过滤条件，如确实需要删除全表，可使用 1=1 作为条件。");
				}
				sqlBuilder.AppendFormat(" WHERE {0}={1}{0}", this.PrimaryKey, this.ParmToken);
			}
			return sqlBuilder.ToString();
		}

		/// <summary>
		/// 创新获取自动ID字符串。
		/// </summary>
		/// <returns></returns>
		public override string CreateIdentitySQL()
		{
			return "SELECT @@IDENTITY";
		}

		/// <summary>
		/// 克隆一个新对象。
		/// </summary>
		/// <returns></returns>
		public override IExecuteObject CloneEmpty()
		{
			return new MySqlExecuteObject();
		}

		public override string ToString()
		{
			return this.TableName + ",Fields:" + this.Fields.Count;
		}
	}
}
