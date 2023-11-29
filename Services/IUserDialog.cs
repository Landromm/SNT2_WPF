using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Services;
public interface IUserDialog
{
	void OpenMainWindow();
	void OpenCurrentGrapf();
	void OpenArchiveGraph();
	void OpenSettingsWindow();
}
