using NPiculet.DataObject;

namespace NPiculet.DataObject
{
	public interface IDbExe
	{
		/// <summary>
		/// 创建一个空的执行对象。
		/// </summary>
		/// <returns></returns>
		IExecuteObject CreateEmptyExecuteObject();

		/// <summary>
		/// 根据数据模型创建执行对象。
		/// </summary>
		/// <param name="model">数据模型</param>
		/// <returns></returns>
		IExecuteObject CreateExecuteObject(object model);

		/// <summary>
		/// 提交数据。
		/// </summary>
		/// <param name="model">数据模型</param>
		int Submit(object model);

		/// <summary>
		/// 删除数据。
		/// </summary>
		/// <param name="model">数据模型</param>
		/// <returns></returns>
		int Delete(object model);
	}
}