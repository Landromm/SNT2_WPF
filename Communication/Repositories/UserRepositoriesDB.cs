using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using SNT2_WPF.Communication.Logger;
using SNT2_WPF.Models.DataConEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Communication.Repositories;
internal class UserRepositoriesDB : IRepositoriesDB
{
	private LogWriter logWriter = new();

	public string? GetCounterId(string? numberCounter)
	{
		string? counterId = string.Empty;

		using var contex1 = new DataContext();
		{
			try
			{
				var fullNumberCounter = "00" + numberCounter;
				using var contex2 = new DataContext();
				{
					counterId = contex2.ListValues
						.Where(id => id.Value == fullNumberCounter)
						.Select(id => id.CounterId)
						.ToList()
						.First();
				}
			}
			catch (Exception ex)
			{
				counterId = contex1.ListValues
					.Where(id => id.Value == numberCounter)
					.Select(id => id.CounterId)
					.ToList()
					.First();

				logWriter.WriteError($"UserRepositoriesDB.GetCounterId() - Не вернул Id Счетчика по его номеру. \n\t {ex}");
			}
		}
		return counterId;
	}

	public string? GetDescriptionCounter(string? counterId)
	{
		var descriptionCounter = string.Empty;
		using var contex = new DataContext();
		{
			try
			{
				descriptionCounter = contex.Counters
					.Where(id => id.CounterId == counterId)
					.Select(desc => desc.NameCounter)
					.ToList()
					.First();
			}
			catch (Exception ex)
			{
				logWriter.WriteError($"UserRepositoriesDB.GetDescriptionCounter() - Не вернул наименование счетчика. \n\t {ex}");
			}
		}
		return descriptionCounter;
	}

	public string? GetValueChanel(int hash)
	{
		var value = string.Empty;
		using var contex = new DataContext();
		{
			try
			{
				value = contex.ListValues
					.Where(h => h.Hash == hash)
					.Select(v => v.Value)
					.ToList()
					.First();
			}
			catch (Exception ex)
			{
				logWriter.WriteError($"UserRepositoriesDB.GetValueChanel() - Не вернул значение по хэшу параметра. \n\t {ex}");
			}
		}
		return value;
	}

	public bool GetStatusChanel(int hash)
	{
		string? value = "0";
		using var contex = new DataContext();
		{
			try
			{
				value = contex.ListValues
					.Where(h => h.Hash == hash)
					.Select(v => v.Value)
					.ToList()
					.First();
			}
			catch (Exception ex)
			{
				logWriter.WriteError($"UserRepositoriesDB.GetStatusChanel() - Не вернул статус активности счетчика. \n\t {ex}");
			}
		}
		return !value!.Equals("0");
	}

	public List<(DateTime, string)> GetHistoryData(string? hash)
	{
		if (hash == null)
			return new List<(DateTime, string)>{ new (DateTime.Now, "-25.00") };

		var hashInt = Convert.ToInt32(hash);

		var historyData = new List<(DateTime, string)>();
		using var context = new DataContext();
		{
			try
			{
				var result = from historyH in context.History
							 where historyH.HashId == hashInt &&
							 historyH.DateTime > DateTime.Now.AddMinutes(-60) &&
							 historyH.DateTime <= DateTime.Now
							 orderby historyH.DateTime
							 select new
							 {
								 historyH.DateTime,
								 historyH.Value
							 };
				if (!result.IsNullOrEmpty())
				{
					foreach (var item in result!)
					{
						(DateTime, string) items = (item.DateTime, item.Value.Replace(".", ","));
						historyData.Add(items);
					}
				}
			}
			catch (Exception ex)
			{
				logWriter.WriteError($"UserRepositoriesDB.GetHistoryData() - Не вернул список кортежей " +
					$"содержащих архивные значения по хэшу. \n\t {ex}");
			}
		}
		return historyData;
	}

	public (DateTime, string?) GetLastHistoryData(string? hash)
	{
		if (hash == null) 
			return new (DateTime.Now, "-25.00");

		var hashInt = Convert.ToInt32(hash);
		(DateTime, string?) historyData = new();
		using var context = new DataContext();
		{
			try
			{
				var result = context.ListValues
						.Where(d => d.Hash == hashInt)
						.Select(val => val.Value)
						.ToList()
						.Last();
				if (result is not null)
					result = result.Replace(".", ",");

				historyData = (DateTime.Now, result);
			}
			catch (Exception ex)
			{
				logWriter.WriteError($"UserRepositoriesDB.GetHistoryData() - Не вернул кортеж " +
					$"последних данных из онлайнтаблицы по хэшу. \n\t {ex}");
			}
		}
		return historyData;
	}
}
