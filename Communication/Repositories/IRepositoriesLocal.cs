﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Communication.Repositories;
internal interface IRepositoriesLocal
{
	/// <summary>
	/// Инициализация списка счетчиков.
	/// </summary>
	/// <returns></returns>
	List<string> InitializeCounters();
}
