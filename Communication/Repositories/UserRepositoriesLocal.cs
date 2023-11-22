using SNT2_WPF.Communication.IniData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Communication.Repositories;
internal class UserRepositoriesLocal : IRepositoriesLocal
{
	readonly IniFile INI = new(@ConfigurationManager.AppSettings["pathConfig"]);

	public List<string> InitializeCounters()
	{
		var numbersCounters = new List<string>();
		string tempNumberCounters = Properties.Settings.Default.NumberCounter;
		string[] counters = tempNumberCounters	.Replace(" ", "")
												.Split(',');
		foreach (string counter in counters)
			numbersCounters.Add(counter);

		return numbersCounters;
	}
}
