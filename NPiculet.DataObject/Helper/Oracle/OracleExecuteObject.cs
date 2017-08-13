using System;
using System.Text;

namespace NPiculet.DataObject
{
	public class OracleExecuteObject : AbstractExecuteObject
	{
		/// <summary>
		/// ������ʶ����
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
		/// ���������ַ�����
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
		/// ���������ַ�����
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
					throw new DataObjectException("��ֹ����ȫ�����ݣ����������������������ȷʵ��Ҫ����ȫ����ʹ�� 1=1 ��Ϊ������");
				}
				sqlBuilder.AppendFormat(" WHERE {0}={1}{0}", this.PrimaryKey, this.ParmToken);
			}
			return sqlBuilder.ToString();
		}

		/// <summary>
		/// ����ɾ���ַ�����
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
					throw new DataObjectException("��ֹɾ��ȫ�����ݣ����������������������ȷʵ��Ҫɾ��ȫ����ʹ�� 1=1 ��Ϊ������");
				}
				sqlBuilder.AppendFormat(" WHERE {0}={1}{0}", this.PrimaryKey, this.ParmToken);
			}
			return sqlBuilder.ToString();
		}

		/// <summary>
		/// ���»�ȡ�Զ�ID�ַ�����
		/// </summary>
		/// <returns></returns>
		public override string CreateIdentitySQL()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return this.TableName + ",Fields:" + this.Fields.Count;
		}

		public override IExecuteObject CloneEmpty() {
			return new OracleExecuteObject();
		}
	}
}
