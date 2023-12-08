using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.ViewModel.Settings.UserControlViewModel;
internal class ItemTabControlTagViewModel : Base.ViewModel
{
	

	public ItemTabControlTagViewModel(string tabName, string countName)
	{
		TabName = tabName;
		Name = countName;
	}

	public string TabName
	{
		get;
		private set;
	}

	private string name;
	public string Name
	{
		get
		{
			return name;
		}

		set
		{
			if (name != value)
			{
				name = value;
				OnPropertyChanged("Name");
			}
		}
	}
}
