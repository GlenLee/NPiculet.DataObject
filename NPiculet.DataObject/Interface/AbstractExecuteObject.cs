/*
名称：DbHelper 类的执行对象接口
日期：2011-01-12
作者：李萨
备注：
*/
using System;
using System.Collections.Generic;

namespace NPiculet.DataObject
{
	/// <summary>
	/// 执行对象接口。
	/// </summary>
	public abstract partial class AbstractExecuteObject : IExecuteObject
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

		private string _TableName = String.Empty;
		/// <summary>
		/// 获取或设置需要操作的数据表名称。
		/// </summary>
		public virtual string TableName { get { return Wrap(_TableName); } set { _TableName = value; } }

		private string _PrimaryKey = String.Empty;
		/// <summary>
		/// 获取或设置数据表主键名称。
		/// </summary>
		public virtual string PrimaryKey { get { return _PrimaryKey; } set { _PrimaryKey = value; } }

		private object _PrimaryValue;
		/// <summary>
		/// 获取或设置数据表主键值。
		/// </summary>
		public virtual object PrimaryValue { get { return _PrimaryValue; } set { _PrimaryValue = value; } }

		private List<string> _AutoColumns = new List<string>();
		/// <summary>
		/// 标识哪些字段的值是自动生成的。
		/// </summary>
		public virtual List<string> AutoColumns { get { return _AutoColumns; } set { _AutoColumns = value; } }

		private string _Where = String.Empty;
		/// <summary>
		/// 获取或设置执行条件。
		/// </summary>
		public virtual string Where { get { return _Where; } set { _Where = value; } }

		private ExecuteType _ExecuteType = ExecuteType.None;
		/// <summary>
		/// 获取或设置执行类型。
		/// </summary>
		public virtual ExecuteType ExecuteType { get { return _ExecuteType; } set { _ExecuteType = value; } }

		private List<Field> _Fields = new List<Field>();
		/// <summary>
		/// 获取值对象列表。
		/// </summary>
		public virtual List<Field> Fields { get { return _Fields; } }

		/// <summary>
		/// 增加执行过程的值对象。
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="val">值</param>
		/// <param name="size">大小</param>
		/// <param name="type">数据类型</param>
		public virtual void Add(string key, object val, int size, DataType type)
		{
			Fields.Add(new Field(key, val, size, type));
		}

		/// <summary>
		/// 增加执行过程的值对象。
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="val">值</param>
		/// <param name="type">数据类型</param>
		public virtual void Add(string key, object val, DataType type)
		{
			Fields.Add(new Field(key, val, 0, type));
		}

		/// <summary>
		/// 增加执行过程的值对象。
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="val">值</param>
		public virtual void Add(string key, object val)
		{
			Fields.Add(new Field(key, val, 0, DataType.None));
		}

		/// <summary>
		/// 移除执行过程的值对象。
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
		/// 清理执行对象中的所有值对象。
		/// </summary>
		public virtual void Clear()
		{
			this.Fields.Clear();
		}

		/// <summary>
		/// 获取执行字符串。
		/// </summary>
		/// <returns>执行字符串</returns>
		public virtual string GetExecuteSQL()
		{
			switch (this.ExecuteType) {
				case ExecuteType.Insert: return CreateInsertSQL();
				case ExecuteType.Update: return CreateUpdateSQL();
				case ExecuteType.Delete: return CreateDeleteSQL();
				default:
					throw new DataObjectException("没有产生任何执行语句字符串，这是因为执行状态为：ExecuteType.None");
			}
		}

		/// <summary>
		/// 生成新增数据的 Sql 语句。
		/// </summary>
		/// <returns></returns>
		public abstract string CreateInsertSQL();

		/// <summary>
		/// 生成更新数据的 Sql 语句。
		/// </summary>
		/// <returns></returns>
		public abstract string CreateUpdateSQL();

		/// <summary>
		/// 生成删除数据的 Sql 语句。
		/// </summary>
		/// <returns></returns>
		public abstract string CreateDeleteSQL();

		/// <summary>
		/// 生成获取自动增长ID的 Sql 语句。
		/// </summary>
		/// <returns></returns>
		public abstract string CreateIdentitySQL();

		/// <summary>
		/// 克隆一个新的对象。
		/// </summary>
		/// <returns></returns>
		public abstract IExecuteObject CloneEmpty();
	}
}
