namespace NPiculet.DataObject {
	/// <summary>
	/// 数据类型。
	/// </summary>
	public enum DataType {
		/// <summary>
		/// 布尔型。
		/// </summary>
		Boolean = 0,

		/// <summary>
		/// 字节型。
		/// </summary>
		Byte = 1,

		/// <summary>
		/// 日期型。
		/// </summary>
		DateTime = 2,

		/// <summary>
		/// Guid 型。
		/// </summary>
		Guid = 3,

		/// <summary>
		/// Int16 型。
		/// </summary>
		Int16 = 4,

		/// <summary>
		/// Int32 型。
		/// </summary>
		Int32 = 5,

		/// <summary>
		/// Int64 型。
		/// </summary>
		Int64 = 6,

		/// <summary>
		/// 数值型。
		/// </summary>
		Numeric = 7,

		/// <summary>
		/// 字符串。
		/// </summary>
		String = 8,

		/// <summary>
		/// 长文本。
		/// </summary>
		Text = 9,

		/// <summary>
		/// Xml 型。
		/// </summary>
		Xml = 10,

		/// <summary>
		/// 图像。
		/// </summary>
		Image = 11,

		/// <summary>
		/// 对象。
		/// </summary>
		Object = 12,

		/// <summary>
		/// 不可知类型，默认使用字符串型处理。
		/// </summary>
		None = -1
	}
}