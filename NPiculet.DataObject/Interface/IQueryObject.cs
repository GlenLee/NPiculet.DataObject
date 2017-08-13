/*
Name: DbHelper's Query Object
Date: 2009-8-29
Author: iSLeeCN
Description: Create DbHelper's Query Object.
*/
using System;
using System.Collections.Generic;

namespace NPiculet.DataObject
{
	/// <summary>
	/// 查询对象接口。
	/// </summary>
	public partial interface IQueryObject
	{
		/// <summary>
		/// 参数标识符
		/// </summary>
		char ParmToken { get; }

		/// <summary>
		/// 获取或设置当前页码。
		/// </summary>
		int CurrentPage { get; set; }

		/// <summary>
		/// 获取或设置分页大小。
		/// </summary>
		int PageSize { get; set; }

		/// <summary>
		/// 获取或设置需要操作的数据表名称。
		/// </summary>
		string TableName { get; set; }

		/// <summary>
		/// 获取或设置数据表主键名称，在分页查询中将作为第二排序字段。
		/// </summary>
		string PrimaryKey { get; set; }

		/// <summary>
		/// 获取或设置查询字段。
		/// </summary>
		List<Field> Fields { get; set; }

		/// <summary>
		/// 获取或设置查询条件。
		/// </summary>
		string Where { get; set; }

		/// <summary>
		/// 获取或设置排序字段。
		/// </summary>
		string OrderBy { get; set; }

		/// <summary>
		/// 增加查询过程的字段对象。
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="size">大小</param>
		/// <param name="type">数据类型</param>
		void Add(string key, int size, DataType type);

		/// <summary>
		/// 增加查询过程的字段对象。
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="type">数据类型</param>
		void Add(string key, DataType type);

		/// <summary>
		/// 增加查询过程的字段对象。
		/// </summary>
		/// <param name="key">键名</param>
		void Add(string key);

		/// <summary>
		/// 增加查询过程的字段对象。
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="alias">别名</param>
		void Add(string key, string alias);

		/// <summary>
		/// 移除查询过程的值对象。
		/// </summary>
		/// <param name="key"></param>
		void Remove(string key);

		/// <summary>
		/// 清理查询对象中的所有字段对象。
		/// </summary>
		void Clear();

		/// <summary>
		/// 获取数据统计语句字符串。
		/// </summary>
		/// <returns></returns>
		string GetCountString();

		/// <summary>
		/// 获取字段中最大数字值字符串。
		/// </summary>
		/// <returns></returns>
		string GetMaxString();

		/// <summary>
		/// 获取字段中最小数字值字符串。
		/// </summary>
		/// <returns></returns>
		string GetMinString();

		/// <summary>
		/// 获取字段值的统计字符串。
		/// </summary>
		/// <returns></returns>
		string GetSumString();

		/// <summary>
		/// 获取数据查询语句字符串。
		/// </summary>
		/// <returns></returns>
		string GetQueryString();

		/// <summary>
		/// 克隆一个空的新对象。
		/// </summary>
		/// <returns></returns>
		IQueryObject CloneEmpty();
	}
}
