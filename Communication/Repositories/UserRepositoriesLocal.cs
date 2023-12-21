
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Communication.Repositories;
internal class UserRepositoriesLocal : IRepositoriesLocal
{
	public List<string> InitializeCounters()
	{
		var numbersCounters = new List<string>();
		string tempNumberCounters = Properties.Settings.Default.NumberCounter;
		string[] counters = tempNumberCounters	.Replace(" ", "")
												.Split(',');
		foreach (string counter in counters)
			numbersCounters.Add(counter);

		Properties.Settings.Default.CountCounters = numbersCounters.Count;
		Properties.Settings.Default.Save();

		return numbersCounters;
	}
}
