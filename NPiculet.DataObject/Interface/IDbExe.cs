using NPiculet.DataObject;

namespace NPiculet.DataObject
{
	public interface IDbExe
	{
		/// <summary>
		/// ����һ���յ�ִ�ж���
		/// </summary>
		/// <returns></returns>
		IExecuteObject CreateEmptyExecuteObject();

		/// <summary>
		/// ��������ģ�ʹ���ִ�ж���
		/// </summary>
		/// <param name="model">����ģ��</param>
		/// <returns></returns>
		IExecuteObject CreateExecuteObject(object model);

		/// <summary>
		/// �ύ���ݡ�
		/// </summary>
		/// <param name="model">����ģ��</param>
		int Submit(object model);

		/// <summary>
		/// ɾ�����ݡ�
		/// </summary>
		/// <param name="model">����ģ��</param>
		/// <returns></returns>
		int Delete(object model);
	}
}