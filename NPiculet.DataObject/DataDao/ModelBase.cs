using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NPiculet.DataObject
{
	[Serializable]
	public abstract class ModelBase : IModel
	{
		#region 处理实体类属性改变事件。

		public event PropertyChangingEventHandler PropertyChanging;

		public event PropertyChangedEventHandler PropertyChanged;

		private readonly List<string> _propertyChangedList = new List<string>();
		public List<string> PropertyChangedList { get { return _propertyChangedList; } }

		protected void OnPropertyChanging(string propertyName)
		{
			//属性改变前事件
			var changingHandler = PropertyChanging;
			if (changingHandler != null) changingHandler(this, new PropertyChangingEventArgs(propertyName));
		}

		protected void OnPropertyChanged(string propertyName)
		{
			//记录改变属性
			if (!PropertyChangedList.Contains(propertyName)) PropertyChangedList.Add(propertyName);
			//属性改变后事件
			var changedHandler = PropertyChanged;
			if (changedHandler != null) changedHandler(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// 清除属性变化记录
		/// </summary>
		public void ClearPropertyChange()
		{
			_propertyChangedList.Clear();
		}

		#endregion

		/// <summary>
		/// 实体类数据表名称。
		/// </summary>
		public abstract string TableName { get; }

		/// <summary>
		/// 实体类主键。
		/// </summary>
		public abstract string PrimeKey { get; }
	}
}