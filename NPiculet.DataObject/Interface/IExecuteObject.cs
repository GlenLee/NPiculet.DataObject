/*
名称：DbHelper 类的执行对象接口
日期：2011-01-12
作者：李萨
备注：
*/
using System.Collections.Generic;
using System;

namespace NPiculet.DataObject
{
	/// <summary>
	/// 执行对象接口。
	/// </summary>
	public partial interface IExecuteObject
	{
		/// <summary>
		/// 参数标识符
		/// </summary>
		char ParmToken { get; }

		/// <summary>
		/// 获取或设置需要操作的数据表名称。
		/// </summary>
		string TableName { get; set; }

		/// <summary>
		/// 获取或设置数据表主键名称。
		/// </summary>
		string PrimaryKey { get; set; }

		/// <summary>
		/// 获取或设置数据表主键值。
		/// </summary>
		object PrimaryValue { get; set; }

		/// <summary>
		/// 获取或设置主键值是否由数据库自动生成。
		/// </summary>
		List<string> AutoColumns { get; set; }

		/// <summary>
		/// 获取或设置执行条件。
		/// </summary>
		string Where { get; set; }

		/// <summary>
		/// 获取或设置执行类型。
		/// </summary>
		ExecuteType ExecuteType { get; set; }

		/// <summary>
		/// 获取值对象列表。
		/// </summary>
		List<Field> Fields { get; }

		/// <summary>
		/// 增加执行过程的值对象。
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="val">值</param>
		/// <param name="size">大小</param>
		/// <param name="type">数据类型</param>
		void Add(string key, object val, int size, DataType type);

		/// <summary>
		/// 增加执行过程的值对象。
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="val">值</param>
		/// <param name="type">数据类型</param>
		void Add(string key, object val, DataType type);

		/// <summary>
		/// 增加执行过程的值对象。
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="val">值</param>
		void Add(string key, object val);

		/// <summary>
		/// 移除执行过程的值对象。
		/// </summary>
		/// <param name="key"></param>
		void Remove(string key);

		/// <summary>
		/// 清理执行对象中的所有值对象。
		/// </summary>
		void Clear();

		/// <summary>
		/// 获取执行字符串。
		/// </summary>
		/// <returns>执行字符串</returns>
		string GetExecuteSQL();

		/// <summary>
		/// 生成新增数据的 Sql 语句。
		/// </summary>
		/// <returns></returns>
		string CreateInsertSQL();

		/// <summary>
		/// 生成更新数据的 Sql 语句。
		/// </summary>
		/// <returns></returns>
		string CreateUpdateSQL();

		/// <summary>
		/// 生成删除数据的 Sql 语句。
		/// </summary>
		/// <returns></returns>
		string CreateDeleteSQL();

		/// <summary>
		/// 生成获取自动索引的 Sql 语句。
		/// </summary>
		/// <returns></returns>
		string CreateIdentitySQL();

		/// <summary>
		/// 克隆一个空的新的对象
		/// </summary>
		/// <returns></returns>
		IExecuteObject CloneEmpty();
	}
}
