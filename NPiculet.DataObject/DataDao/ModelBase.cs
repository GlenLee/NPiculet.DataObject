using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NPiculet.DataObject
{
	[Serializable]
	public abstract class ModelBase : IModel
	{
		#region ����ʵ�������Ըı��¼���

		public event PropertyChangingEventHandler PropertyChanging;

		public event PropertyChangedEventHandler PropertyChanged;

		private readonly List<string> _propertyChangedList = new List<string>();
		public List<string> PropertyChangedList { get { return _propertyChangedList; } }

		protected void OnPropertyChanging(string propertyName)
		{
			//���Ըı�ǰ�¼�
			var changingHandler = PropertyChanging;
			if (changingHandler != null) changingHandler(this, new PropertyChangingEventArgs(propertyName));
		}

		protected void OnPropertyChanged(string propertyName)
		{
			//��¼�ı�����
			if (!PropertyChangedList.Contains(propertyName)) PropertyChangedList.Add(propertyName);
			//���Ըı���¼�
			var changedHandler = PropertyChanged;
			if (changedHandler != null) changedHandler(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// ������Ա仯��¼
		/// </summary>
		public void ClearPropertyChange()
		{
			_propertyChangedList.Clear();
		}

		#endregion

		/// <summary>
		/// ʵ�������ݱ����ơ�
		/// </summary>
		public abstract string TableName { get; }

		/// <summary>
		/// ʵ����������
		/// </summary>
		public abstract string PrimeKey { get; }
	}
}