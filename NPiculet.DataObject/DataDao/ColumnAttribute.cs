using System;

namespace NPiculet.DataObject
{
	/// <summary>
	/// 数据列属性
	/// </summary>
	public class Column : Attribute
	{
		/// <summary>
		/// 字段名称
		/// </summary>
		public string Field { get; set; }

		/// <summary>
		/// 字段类型
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// 长度
		/// </summary>
		public int Length { get; set; }

		/// <summary>
		/// 刻度（小数位）
		/// </summary>
		public int Scale { get; set; }
	}
}