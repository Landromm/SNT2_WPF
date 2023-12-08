using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.ViewModel.Settings.UserControlViewModel;
internal class SettingTagsViewModel : Base.ViewModel
{
	private ObservableCollection<ItemTabControlTagViewModel> items = new ObservableCollection<ItemTabControlTagViewModel>();

	public ObservableCollection<ItemTabControlTagViewModel> Items
	{
		get => items;
	}
}
