using SNT2_WPF.Models.DataConEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Communication.Repositories;
internal class UserRepositoriesDB : IRepositoriesDB
{
	public string? GetCounterId(string? numberCounter)
	{
		var counterId = string.Empty;
		using var contex = new DataContext();
		{
			counterId = contex.ListValues
				.Where(id => id.Value == numberCounter)
				.Select(id => id.CounterId)
				.ToList()
				.First();
		}
		return counterId;
	}

	public string? GetDescriptionCounter(string? counterId)
	{
		var descriptionCounter = string.Empty;
		using var contex = new DataContext();
		{
			descriptionCounter = contex.Counters	
				.Where(id => id.CounterId == counterId)
				.Select(desc => desc.NameCounter)
				.ToList()
				.First();
		}
		return descriptionCounter;
	}

	public string? GetValueChanel(int hash)
	{
		var value = string.Empty;
		using var contex = new DataContext();
		{
			value = contex.ListValues
				.Where(h => h.Hash == hash)
				.Select(v => v.Value)
				.ToList()
				.First();
		}
		return value;
	}

	public bool GetStatusChanel(int hash)
	{
		string? value = "0";
		using var contex = new DataContext();
		{
			value = contex.ListValues
				.Where(h => h.Hash == hash)
				.Select(v => v.Value)
				.ToList()
				.First();
		}
		return !value.Equals("0");
	}

	public List<Tuple<DateTime, double>> GetHistoryData(string hash)
	{
		var hashInt = Convert.ToInt32(hash);

		var historyData = new List<Tuple<DateTime, double>>();
		using var context = new DataContext();
		{
			var result = from historyH in context.History
						 where historyH.HashId == hashInt &&
						 historyH.DateTime > DateTime.Now.AddMinutes(-30) &&
						 historyH.DateTime <= DateTime.Now
						 orderby historyH.DateTime
						 select new
						 {
							 historyH.DateTime,
							 historyH.Value
						 };

			foreach (var item in result)
				historyData.Add(new (item.DateTime, Convert.ToDouble(item.Value)));
		}
		return historyData;
	}

	public (DateTime, double) GetLastHistoryData(string hash)
	{
		var hashInt = Convert.ToInt32(hash);

		(DateTime, double) historyData = new();
		using var context = new DataContext();
		{
			var result = context.History
				.Where(d => d.HashId == hashInt && d.DateTime > DateTime.Now.AddMinutes(-30))
				.OrderBy(date => date.DateTime)
				.Select(val => new
				{
					val.DateTime,
					val.Value
				})
				.ToList()
				.Last();
			historyData = (result.DateTime, Convert.ToDouble(result.Value));
		}
		return historyData;
	}
}
