﻿using SNT2_WPF.Communication.IniData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Communication.Repositories;
internal class UserRepositoriesLocal : IRepositoriesLocal
{
	IniFile INI = new IniFile(@ConfigurationManager.AppSettings["pathConfig"]);

	public List<string> InitializeCounters()
	{
		var numbersCounters = new List<string>();
		string tempNumberCounters = INI.ReadINI("SNTConfig", "NumberCounter");
		string[] counters = tempNumberCounters	.Replace(" ", "")
												.Split(',');
		foreach (string counter in counters)
			numbersCounters.Add(counter);

		return numbersCounters;
	}
}