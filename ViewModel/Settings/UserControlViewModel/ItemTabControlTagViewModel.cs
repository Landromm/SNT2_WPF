using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.ViewModel.Settings.UserControlViewModel;
internal class ItemTabControlTagViewModel : Base.ViewModel
{
	//Инициализация наименования вкладок в "TabControl".
    public string TabName { get; init; }

    #region CounterNumber : string? - Номер Счетчика в списке.

    /// <summary>Номер Счетчика в списке. - поле.</summary>
    private string? _counterNumber;

	/// <summary>Номер Счетчика в списке. - свойство.</summary>
	public string? CounterNumber
	{
		get => _counterNumber;
		set
		{
			_counterNumber = value;
			OnPropertyChanged(nameof(CounterNumber));
		}
	}
	#endregion




	public ItemTabControlTagViewModel(string tabName, string countName)
	{
		TabName = tabName;
		CounterNumber = countName;
	}





}
