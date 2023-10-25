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
}
