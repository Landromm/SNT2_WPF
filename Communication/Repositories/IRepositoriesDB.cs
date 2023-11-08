using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Communication.Repositories;
internal interface IRepositoriesDB
{
	/// <summary>
	/// Возвращает Id счетчика по его номеру.
	/// </summary>
	/// <param name="numberCounter"></param>
	/// <returns></returns>
	public string? GetCounterId(string? numberCounter);
	/// <summary>
	/// Возвращает наименование счетчика.
	/// </summary>
	/// <param name="counterId"></param>
	/// <returns></returns>
	public string? GetDescriptionCounter(string? counterId);
	/// <summary>
	/// Возвращает значение по хэшу параметра.
	/// </summary>
	/// <param name="hash"></param>
	/// <returns></returns>
	public string? GetValueChanel(int hash);
	/// <summary>
	/// Возвращает статус активности счетчика (false - нет связи, true - связь есть).
	/// </summary>
	/// <param name="hash"></param>
	/// <returns></returns>
	public bool GetStatusChanel(int hash);
	/// <summary>
	/// Возвращает список кортежей содержащих архивные значения по соответсвующему хэшу.
	/// </summary>
	/// <param name="hash"></param>
	/// <returns></returns>
	public List<Tuple<DateTime,double>> GetHistoryData(string? hash);
	/// <summary>
	/// Возвращает кортеж последних данных из онлайн таблицы по соответствующему хэшу.
	/// </summary>
	/// <param name="hash"></param>
	/// <returns></returns>
	public (DateTime,double) GetLastHistoryData(string? hash);
}
