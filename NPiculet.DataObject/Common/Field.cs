/*
名称：DbHelper 类的执行对象的值对象
日期：2011-01-12
作者：李萨
备注：
*/
namespace NPiculet.DataObject
{
	/// <summary>
	/// 字段对象，用于新增及更新操作中数据的传入。
	/// </summary>
	public class Field
	{
		/// <summary>
		/// 字段对象
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="val">值</param>
		/// <param name="size">大小</param>
		/// <param name="type">数据类型</param>
		/// <param name="alias">别名</param>
		public Field(string key, object val, int size, DataType type, string alias = "")
		{
			this._Key = key;
			this._Value = val;
			this._Size = size;
			this._Type = type;
			this._Alias = alias;
		}

		/// <summary>
		/// 字段对象
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="alias">别名</param>
		public Field(string key, string alias = "")
		{
			this._Key = key;
			this._Alias = alias;
		}

		private string _Key;
		/// <summary>
		/// 键名。
		/// </summary>
		public string Key
		{
			get { return _Key; }
		}

		private object _Value;
		/// <summary>
		/// 值
		/// </summary>
		public object Value
		{
			get { return _Value; }
		}

		private int _Size;
		/// <summary>
		/// 大小
		/// </summary>
		public int Size
		{
			get { return _Size; }
		}

		private DataType _Type = DataType.None;
		/// <summary>
		/// 数据类型
		/// </summary>
		public DataType Type
		{
			get { return _Type; }
		}

		private string _Alias;
		/// <summary>
		/// 别名
		/// </summary>
		public string Alias
		{
			get { return _Alias; }
			set { _Alias = value; }
		}

		public override string ToString()
		{
			return this.Key + ":" + this.Value;
		}
	}
}
