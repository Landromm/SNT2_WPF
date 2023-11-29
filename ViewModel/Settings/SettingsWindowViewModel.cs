using SNT2_WPF.Services;
using SNT2_WPF.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.ViewModel.Settings;
public class SettingsWindowViewModel : DialogViewModel
{
	private readonly IUserDialog _userDialog = null!;


	public SettingsWindowViewModel(IUserDialog UserDialog)
	{
		_userDialog = UserDialog;
	}


}
