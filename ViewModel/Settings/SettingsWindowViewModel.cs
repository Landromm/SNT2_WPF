using SNT2_WPF.Services;
using SNT2_WPF.View.Settings.UserControlView;
using SNT2_WPF.ViewModel.Base;
using SNT2_WPF.ViewModel.Commands;
using SNT2_WPF.ViewModel.Settings.UserControlViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNT2_WPF.ViewModel.Settings;
public class SettingsWindowViewModel : DialogViewModel
{
	private Base.ViewModel? _currentChildView;

	private readonly IUserDialog _userDialog = null!;

	public Base.ViewModel? CurrentChildView
	{
		get => _currentChildView;
		set
		{
			_currentChildView = value;
			OnPropertyChanged(nameof(CurrentChildView));
		}
	}

	public SettingsWindowViewModel(IUserDialog UserDialog)
	{
		_userDialog = UserDialog;
		CurrentChildView = null!;
	}

	#region Command ShowComViewCommand - Открытие фрэйма настройки COM порта.

	/// <summary>Открытие фрэйма настройки COM порта.</summary>
	private LambdaCommand? _ShowComViewCommand;

	/// <summary>Открытие фрэйма настройки COM порта.</summary>
	public ICommand ShowComViewCommand => _ShowComViewCommand ??= new(ExecutedShowComViewCommand);

	/// <summary>Логика выполнения - Открытие фрэйма настройки COM порта.</summary>
	private void ExecutedShowComViewCommand()
	{
		CurrentChildView = new SettingsComPortViewModel();
	}
	#endregion

	#region Command ShowTagsViewCommand - Открытие фрэйма настройки каналов.

	/// <summary>Открытие фрэйма настройки каналов.</summary>
	private LambdaCommand? _ShowTagsViewCommand;

	/// <summary>Открытие фрэйма настройки каналов.</summary>
	public ICommand ShowTagsViewCommand => _ShowTagsViewCommand ??= new(ExecutedShowTagsViewCommand);

	/// <summary>Логика выполнения - Открытие фрэйма настройки каналов.</summary>
	private void ExecutedShowTagsViewCommand()
	{
		var countCounters = Properties.Settings.Default.CountCounters;
		var viewModel = new SettingTagsViewModel();
		//Не удалять - Универсальная формула отображения количества счетчиков в окне настроек каналов.
		for (int i = 0; i < countCounters; i++)
		{
			viewModel.Items.Add(new ItemTabControlTagViewModel($"Счетчик №{i + 1}", $"СНТ-2 №{i + 1}"));
		}
		//viewModel.Items.Add(new ItemTabControlTagViewModel($"Счетчик №{1}", $"СНТ-2 №{1}"));
		//viewModel.Items.Add(new ItemTabControlTagViewModel($"Счетчик №{2}", $"СНТ-2 №{2}"));
		//viewModel.Items.Add(new ItemTabControlTagViewModel($"Счетчик №{3}", $"СНТ-2 №{3}"));

		CurrentChildView = viewModel;
	}
	#endregion

	#region Command ShowDataBaseViewCommand - Открытие фрэйма тестирования базы данных.

	/// <summary>Открытие фрэйма тестирования базы данных.</summary>
	private LambdaCommand? _ShowDataBaseViewCommand;

	/// <summary>Открытие фрэйма тестирования базы данных.</summary>
	public ICommand ShowDataBaseViewCommand => _ShowDataBaseViewCommand ??= new(ExecutedShowDataBaseViewCommand);

	/// <summary>Логика выполнения - Открытие фрэйма тестирования базы данных.</summary>
	private void ExecutedShowDataBaseViewCommand()
	{
		CurrentChildView = new SettingDataBaseViewModel();
	}
	#endregion

}
