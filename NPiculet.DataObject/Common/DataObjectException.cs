using System;

namespace NPiculet.DataObject {
	/// <summary>
	/// 数据对象异常
	/// </summary>
	public class DataObjectException : Exception {

		public DataObjectException() : base()
		{
		}

		public DataObjectException(string msg) : base(msg)
		{
		}

		public DataObjectException(string msg, Exception ex) : base(msg, ex)
		{
		}

	}
}