using System;

namespace NPiculet.DataObject
{
	public class MySqlQueryObject : AbstractQueryObject
	{
		/// <summary>
		/// ������ʶ����
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
				string sql = "SELECT MAX(" + Wrap(this.Fields[0].Key) + ") FROM " + this.TableName;
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
				string sql = "SELECT MIN(" + Wrap(this.Fields[0].Key) + ") FROM " + this.TableName;
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
				string sql = "SELECT IfNull(SUM(" + Wrap(this.Fields[0].Key) + "), 0) FROM " + this.TableName;
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
				if (string.IsNullOrWhiteSpace(this.OrderBy)) {
					//��������
					sql += this.TableName;
					//��������
					if (string.IsNullOrWhiteSpace(this.Where)) {
						//�޹���
					} else {
						//�й���
						sql += " WHERE " + this.Where;
					}
					if (string.IsNullOrWhiteSpace(this.PrimaryKey)) {
						//������
					} else {
						//������
						sql += " ORDER BY " + this.PrimaryKey;
					}
					//��ҳ
					sql += string.Format(@" LIMIT {1} OFFSET {0}"
						, ((this.CurrentPage - 1) * this.PageSize)
						, this.PageSize
					);
				} else {
					//������
					sql += this.TableName;
					//��������
					if (string.IsNullOrWhiteSpace(this.Where)) {
						//�޹���
					} else {
						//�й���
						sql += " WHERE " + this.Where;
					}
					if (string.IsNullOrWhiteSpace(this.PrimaryKey)) {
						//������
						sql += " ORDER BY " + this.OrderBy;
					} else {
						//������
						sql += " ORDER BY " + this.OrderBy + ", " + this.PrimaryKey;
					}
					//��ҳ
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
			return new MySqlQueryObject();
		}
	}
}