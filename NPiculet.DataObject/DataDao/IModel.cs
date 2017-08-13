using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NPiculet.DataObject
{
	public interface IModel
	{
		#region 属性事件

		/// <summary>
		/// 属性正在改变事件。
		/// </summary>
		event PropertyChangingEventHandler PropertyChanging;

		/// <summary>
		/// 属性已改变事件。
		/// </summary>
		event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// 实体类属性改变记录列表。
		/// </summary>
		List<string> PropertyChangedList { get; }

		/// <summary>
		/// 清除实体类属性改变记录。
		/// </summary>
		void ClearPropertyChange();

		#endregion

		/// <summary>
		/// 实体类数据表名称。
		/// </summary>
		string TableName { get; }

		/// <summary>
		/// 实体类主键。
		/// </summary>
		string PrimeKey { get; }
	}
}