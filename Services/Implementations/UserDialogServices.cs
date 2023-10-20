using Microsoft.Windows.Themes;
using SNT2_WPF.View.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Services.Implementations;
public class UserDialogServices
{
	private readonly IServiceProvider _service;

	public UserDialogServices(IServiceProvider service)
	{
		_service = service;
	}

	private GraphCurrentDataView _graphCurrrent = null!;
	public void OpenCurrentGrapf()
	{

	}

}
