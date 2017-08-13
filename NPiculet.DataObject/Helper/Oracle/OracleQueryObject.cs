using System;

namespace NPiculet.DataObject
{
	public class OracleQueryObject : AbstractQueryObject
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
		/// ��ȡͳ���ַ�����
		/// </summary>
		/// <returns></returns>
		public override string GetCountString()
		{
			string sql = "SELECT COUNT(*) FROM " + this.TableName;
			if (this.Where.Length > 0) { sql += " WHERE " + this.Where; }
			return sql;
		}

		/// <summary>
		/// ��ȡ���ֵ�ַ�����
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
		/// ��ȡ��Сֵ�ַ�����
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
		/// ��ȡͳ���ַ�����
		/// </summary>
		/// <returns></returns>
		public override string GetSumString()
		{
			//��ϲ�ѯ�ֶ�
			if (this.Fields.Count > 0) {
				string sql = "SELECT NVL(SUM(" + this.Fields[0].Key + "), 0) FROM " + this.TableName;
				if (this.Where.Length > 0) { sql += " WHERE " + this.Where; }
				return sql;
			}
			return String.Empty;
		}

		/// <summary>
		/// ��ȡ��ѯ�ַ���,û�ж�������ʱ,����ָ�������ֶΡ�
		/// </summary>
		/// <returns></returns>
		public override string GetQueryString()
		{
			//��ϲ�ѯ�ֶ�
			string fields = String.Empty;
			if (this.Fields.Count == 0) {
				fields = "*";
			} else {
				//���
				foreach (Field field in this.Fields) {
					if (fields.Length != 0) fields += ",";
					fields += Wrap(field.Key);
					if (!String.IsNullOrEmpty(field.Alias))
						fields += " AS " + Wrap(field.Alias);
				}
			}
			//��ʼ��
			string sql = "SELECT " + fields + " FROM ";
			//�жϷ�ҳ
			if (this.PageSize == 0) {

				#region ����Ҫ��ҳ

				//�����ҳ
				if (string.IsNullOrWhiteSpace(this.OrderBy)) {
					//��������
					if (string.IsNullOrWhiteSpace(this.PrimaryKey)) {
						//������
						sql += this.TableName;
						if (string.IsNullOrWhiteSpace(this.Where)) {
							//�޹���
						} else {
							//�й���
							sql += " WHERE " + this.Where;
						}
					} else {
						//������
						sql += this.TableName;
						if (string.IsNullOrWhiteSpace(this.Where)) {
							//�޹���
						} else {
							//�й���
							sql += " WHERE " + this.Where;
						}
						sql += " ORDER BY " + this.PrimaryKey;
					}
				} else {
					//������
					if (string.IsNullOrWhiteSpace(this.PrimaryKey)) {
						//������
						sql += this.TableName;
						if (string.IsNullOrWhiteSpace(this.Where)) {
							//�޹���
						} else {
							//�й���
							sql += " WHERE " + this.Where;
						}
						sql += " ORDER BY " + this.OrderBy;
					} else {
						//������
						sql += this.TableName;
						if (string.IsNullOrWhiteSpace(this.Where)) {
							//�޹���
						} else {
							//�й���
							sql += " WHERE " + this.Where;
						}
						sql += " ORDER BY " + this.OrderBy + ", " + this.PrimaryKey;
					}
				}

				#endregion

			} else {

				#region ��Ҫ��ҳ

				//���ҳ
				int i = (this.CurrentPage - 1) * this.PageSize, s = i + 1, e = i + this.PageSize;

				if (string.IsNullOrWhiteSpace(this.OrderBy)) {
					//��������
					if (string.IsNullOrWhiteSpace(this.PrimaryKey)) {
						//������
						sql += string.Format(@"(SELECT T.*, ROWNUM RN FROM (SELECT * FROM {0}{1}) T WHERE ROWNUM <= {3}) WHERE RN >= {2}"
							, this.TableName
							, string.IsNullOrWhiteSpace(this.Where) ? "" : " WHERE " + this.Where
							, s
							, e
						);
					} else {
						//������
						sql += string.Format(@"(SELECT T.*, ROWNUM RN FROM (SELECT * FROM {0}{1} ORDER BY {4}) T WHERE ROWNUM <= {3}) WHERE RN >= {2} ORDER BY {4}"
							, this.TableName
							, string.IsNullOrWhiteSpace(this.Where) ? "" : " WHERE " + this.Where
							, s
							, e
							, this.PrimaryKey
						);
					}
				} else {
					//������
					if (string.IsNullOrWhiteSpace(this.PrimaryKey)) {
						//������
						sql += string.Format(@"(SELECT T.*, ROWNUM RN FROM (SELECT * FROM {0}{1} ORDER BY {4}) T WHERE ROWNUM <= {3}) WHERE RN >= {2} ORDER BY {4}"
							, this.TableName
							, string.IsNullOrWhiteSpace(this.Where) ? "" : " WHERE " + this.Where
							, s
							, e
							, this.OrderBy
						);
					} else {
						//������
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

		public override IQueryObject CloneEmpty() {
			return new OracleQueryObject();
		}
	}
}