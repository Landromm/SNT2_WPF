using Microsoft.Extensions.DependencyInjection;
using Microsoft.Windows.Themes;
using SNT2_WPF.View.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Services.Implementations;
internal class UserDialogServices  : IUserDialog
{
	private readonly IServiceProvider _service;

	public UserDialogServices(IServiceProvider service)
	{
		_service = service;
	}

	private MainWindow? _mainWindow = null!;
	public void OpenMainWindow()
	{
		if (_mainWindow is { } window)
		{
			window.Show();
			return;
		}

		window = _service.GetRequiredService<MainWindow>();
		window.Closed += (_, _) => _mainWindow = null;
		_mainWindow = window;
		window.Show();
	}

	private GraphCurrentDataView _graphCurrrent = null!;	
	public void OpenCurrentGrapf()
	{
		var window = _service.GetRequiredService<GraphCurrentDataView>();
		window.Closed += (_, _) => _graphCurrrent = null;
		_graphCurrrent = window;
		window.Show();
	}

}
