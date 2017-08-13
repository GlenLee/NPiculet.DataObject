using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NPiculet.DataObject
{
	public interface IModel
	{
		#region �����¼�

		/// <summary>
		/// �������ڸı��¼���
		/// </summary>
		event PropertyChangingEventHandler PropertyChanging;

		/// <summary>
		/// �����Ѹı��¼���
		/// </summary>
		event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// ʵ�������Ըı��¼�б�
		/// </summary>
		List<string> PropertyChangedList { get; }

		/// <summary>
		/// ���ʵ�������Ըı��¼��
		/// </summary>
		void ClearPropertyChange();

		#endregion

		/// <summary>
		/// ʵ�������ݱ����ơ�
		/// </summary>
		string TableName { get; }

		/// <summary>
		/// ʵ����������
		/// </summary>
		string PrimeKey { get; }
	}
}