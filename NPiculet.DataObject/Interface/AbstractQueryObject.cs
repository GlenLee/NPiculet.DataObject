using System;
using System.Collections.Generic;

namespace NPiculet.DataObject
{
	/// <summary>
	/// 查询对象接口。
	/// </summary>
	public abstract partial class AbstractQueryObject : IQueryObject
	{
		/// <summary>
		/// 参数标识符
		/// </summary>
		public abstract char ParmToken { get; }

		/// <summary>
		/// 包装字段名称
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public abstract string Wrap(string field);

		protected int _CurrentPage = 1;
		/// <summary>
		/// 获取或设置当前页码。
		/// </summary>
		public virtual int CurrentPage
		{
			get { return _CurrentPage > 0 ? _CurrentPage : 1; }
			set { _CurrentPage = value; }
		}

		protected int _PageSize = 0;
		/// <summary>
		/// 获取或设置分页大小。
		/// </summary>
		public virtual int PageSize
		{
			get { return _PageSize; }
			set { _PageSize = value; }
		}

		protected string _TableName = String.Empty;
		/// <summary>
		/// 获取或设置需要操作的数据表名称。
		/// </summary>
		public virtual string TableName
		{
			get { return Wrap(_TableName); }
			set { _TableName = value; }
		}

		protected string _PrimaryKey = String.Empty;
		/// <summary>
		/// 获取或设置数据表主键名称，在分页查询中将作为第二排序字段。
		/// </summary>
		public virtual string PrimaryKey
		{
			get { return _PrimaryKey; }
			set { _PrimaryKey = value; }
		}

		protected List<Field> _Fields = new List<Field>();
		/// <summary>
		/// 获取或设置查询字段。
		/// </summary>
		public virtual List<Field> Fields
		{
			get { return _Fields; }
			set { _Fields = value; }
		}

		protected string _Where = String.Empty;
		/// <summary>
		/// 获取或设置查询条件。
		/// </summary>
		public virtual string Where
		{
			get { return _Where; }
			set { _Where = value; }
		}

		protected string _OrderBy = String.Empty;
		/// <summary>
		/// 获取或设置排序字段。
		/// </summary>
		public virtual string OrderBy
		{
			get { return _OrderBy; }
			set { _OrderBy = value; }
		}

		/// <summary>
		/// 增加查询过程的字段对象。
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="size">大小</param>
		/// <param name="type">数据类型</param>
		public virtual void Add(string key, int size, DataType type)
		{
			Fields.Add(new Field(key, null, size, type));
		}

		/// <summary>
		/// 增加查询过程的字段对象。
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="type">数据类型</param>
		public virtual void Add(string key, DataType type)
		{
			Fields.Add(new Field(key, null, 0, type));
		}

		/// <summary>
		/// 增加查询过程的字段对象。
		/// </summary>
		/// <param name="key">键名</param>
		public virtual void Add(string key)
		{
			Add(key, DataType.None);
		}

		/// <summary>
		/// 增加查询过程的字段对象。
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="alias">别名</param>
		public virtual void Add(string key, string alias)
		{
			Fields.Add(new Field(key, null, 0, DataType.None, alias));
		}

		/// <summary>
		/// 移除查询过程的值对象。
		/// </summary>
		/// <param name="key"></param>
		public virtual void Remove(string key)
		{
			foreach (Field field in this.Fields) {
				if (field.Key == key) {
					Fields.Remove(field);
					return;
				}
			}
		}

		/// <summary>
		/// 清理查询对象中的所有字段对象。
		/// </summary>
		public virtual void Clear()
		{
			this.Fields.Clear();
		}

		/// <summary>
		/// 获取数据统计语句字符串。
		/// </summary>
		/// <returns></returns>
		public abstract string GetCountString();

		/// <summary>
		/// 获取字段中最大数字值字符串。
		/// </summary>
		/// <returns></returns>
		public abstract string GetMaxString();

		/// <summary>
		/// 获取字段中最小数字值字符串。
		/// </summary>
		/// <returns></returns>
		public abstract string GetMinString();

		/// <summary>
		/// 获取字段值的统计字符串。
		/// </summary>
		/// <returns></returns>
		public abstract string GetSumString();

		/// <summary>
		/// 获取数据查询语句字符串。
		/// </summary>
		/// <returns></returns>
		public abstract string GetQueryString();

		/// <summary>
		/// 克隆一个空的新对象。
		/// </summary>
		/// <returns></returns>
		public abstract IQueryObject CloneEmpty();
	}
}