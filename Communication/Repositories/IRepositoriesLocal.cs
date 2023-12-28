using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Communication.Repositories;
internal interface IRepositoriesLocal
{
	/// <summary>
	/// Возвращает наименования счетчиков, записаные в настройках (config-файле).
	/// И записывает общее количество счетчиков,
	/// в соответветствующий тег в настройках программы.
	/// </summary>
	/// <returns>Список номеров счетчиков.</returns>
	List<string> InitializeCounters();
}
