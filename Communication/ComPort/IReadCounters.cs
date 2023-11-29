using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Communication.ComPort;
public interface IReadCounters
{
	int CountErrorRead {get; set; }
	void StartReadCounters();
}
