using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Communication.Repositories;
internal interface IRepositoriesDB
{
	public string? GetCounterId(string? numberCounter);
	public string? GetDescriptionCounter(string? counterId);
	public string? GetValueChanel(int hash);
	public bool GetStatusChanel(int hash);
	public List<Tuple<DateTime,double>> GetHistoryData(string hash);

}
