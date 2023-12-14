using Microsoft.IdentityModel.Tokens;
using Microsoft.Windows.Themes;
using SNT2_WPF.Communication.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SNT2_WPF.ViewModel.Settings.UserControlViewModel;
internal class ItemTabControlTagViewModel : Base.ViewModel
{
	//Список хэшей.
	private List<int> _hash;
	//Инициализация наименования вкладок в "TabControl".
    public string TabName { get; init; }

	#region CounterId : string? - ID Счетчика.

	/// <summary>ID Счетчика. - поле.</summary>
	private string? _counterId;

	/// <summary>ID Счетчика. - свойство.</summary>
	public string? CounterId
	{
		get => _counterId;
		set
		{
			_counterId = value;
			OnPropertyChanged(nameof(CounterId));
		}
	}
	#endregion

	#region DiscriptionCounter : string? - Наименование счетчика.

	/// <summary>Наименование счетчика. - поле.</summary>
	private string? _discriptionCounter;

	/// <summary>Наименование счетчика. - свойство.</summary>
	public string? DiscriptionCounter
	{
		get => _discriptionCounter;
		set
		{
			_discriptionCounter = value;
			OnPropertyChanged(nameof(DiscriptionCounter));
		}
	}
	#endregion

	#region TimeAndDate : string? - Текущее время и дата со счетчика.

	/// <summary>Текущее время и дата со счетчика. - поле.</summary>
	private string? _timeAndDate;

	/// <summary>Текущее время и дата со счетчика. - свойство.</summary>
	public string? TimeAndDate
	{
		get => _timeAndDate;
		set
		{
			_timeAndDate = value;
			OnPropertyChanged(nameof(TimeAndDate));
		}
	}
	#endregion

	#region SerialNumber : string? - Заводской номер.

	/// <summary>Заводской номер. - поле.</summary>
	private string? _serialNumber;

	/// <summary>Заводской номер. - свойство.</summary>
	public string? SerialNumber
	{
		get => _serialNumber;
		set
		{
			_serialNumber = value;
			OnPropertyChanged(nameof(SerialNumber));
		}
	}
	#endregion

	#region InterfaceNumber : string? - Интерфейс (номер).

	/// <summary>Интерфейс (номер). - поле.</summary>
	private string? _interfaceNumber;

	/// <summary>Интерфейс (номер). - свойство.</summary>
	public string? InterfaceNumber
	{
		get => _interfaceNumber;
		set
		{
			_interfaceNumber = value;
			OnPropertyChanged(nameof(InterfaceNumber));
		}
	}
	#endregion
	
	#region Commissioning : string? - Ввод в эксплуатацию.

	/// <summary>Ввод в эксплуатацию. - поле.</summary>
	private string? _commissioning;

	/// <summary>Ввод в эксплуатацию. - свойство.</summary>
	public string? Commissioning
	{
		get => _commissioning;
		set
		{
			_commissioning = value;
			OnPropertyChanged(nameof(Commissioning));
		}
	}
	#endregion

	#region DownTime : string? - Время простоя.

	/// <summary>Время простоя. - поле.</summary>
	private string? _downTime;

	/// <summary>Время простоя. - свойство.</summary>
	public string? DownTime
	{
		get => _downTime;
		set
		{
			_downTime = value;
			OnPropertyChanged(nameof(DownTime));
		}
	}
	#endregion

	#region RunningTime : string? - Время наработки.

	/// <summary>Время наработки. - поле.</summary>
	private string? _runningtime;

	/// <summary>Время наработки. - свойство.</summary>
	public string? RunningTime
	{
		get => _runningtime;
		set
		{
			_runningtime = value;
			OnPropertyChanged(nameof(RunningTime));
		}
	}
	#endregion

	#region MaxSensorsPressure : string? - Максимум датчиков давления.

	/// <summary>Максимум датчиков давления. - поле.</summary>
	private string? _maxSensorsPressure;

	/// <summary>Максимум датчиков давления. - свойство.</summary>
	public string? MaxSensorsPressure
	{
		get => _maxSensorsPressure;
		set
		{
			_maxSensorsPressure = value;
			OnPropertyChanged(nameof(MaxSensorsPressure));
		}
	}
	#endregion

	#region FlowMax_ch1 : string? - Расход максимальный - канал №1.

	/// <summary>Расход максимальный - канал №1. - поле.</summary>
	private string? _flowMax_ch1;

	/// <summary>Расход максимальный - канал №1. - свойство.</summary>
	public string? FlowMax_ch1
	{
		get => _flowMax_ch1;
		set
		{
			_flowMax_ch1 = value;
			OnPropertyChanged(nameof(FlowMax_ch1));
		}
	}
	#endregion

	#region FlowBoundary_ch1 : string? - Расход граничный - канал №1.

	/// <summary>Расход граничный - канал №1. - поле.</summary>
	private string? _flowBoundary_ch1;

	/// <summary>Расход граничный - канал №1. - свойство.</summary>
	public string? FlowBoundary_ch1
	{
		get => _flowBoundary_ch1;
		set
		{
			_flowBoundary_ch1 = value;
			OnPropertyChanged(nameof(FlowBoundary_ch1));
		}
	}
	#endregion

	#region FlowMax_ch2 : string? - Расход максимальный - канал №2.

	/// <summary>Расход максимальный - канал №2. - поле.</summary>
	private string? _flowMax_ch2;

	/// <summary>Расход максимальный - канал №2. - свойство.</summary>
	public string? FlowMax_ch2
	{
		get => _flowMax_ch2;
		set
		{
			_flowMax_ch2 = value;
			OnPropertyChanged(nameof(FlowMax_ch2));
		}
	}
	#endregion

	#region FlowBoundary_ch2 : string? - Расход граничный - канал №2.

	/// <summary>Расход граничный - канал №2. - поле.</summary>
	private string? _flowBoundary_ch2;

	/// <summary>Расход граничный - канал №2. - свойство.</summary>
	public string? FlowBoundary_ch2
	{
		get => _flowBoundary_ch2;
		set
		{
			_flowBoundary_ch2 = value;
			OnPropertyChanged(nameof(FlowBoundary_ch2));
		}
	}
	#endregion

	private readonly IRepositoriesDB _userRepositoriesDB = null!;


	public ItemTabControlTagViewModel(string tabName, string countId)
	{
		TabName = tabName;
		CounterId = countId;
		_hash = new List<int>();

		_userRepositoriesDB = new UserRepositoriesDB();

		InitializeHashCounter();
		InitializationData();
	}

	private string? GetDiscriptionCounter(string? countId)
	{
		return countId is not null ? _userRepositoriesDB.GetDescriptionCounter(countId) : "Ошибка чтения счетчика.";
	}

	private void InitializeHashCounter()
	{
		if(CounterId is not null)
		{
			int numCounter = int.Parse(CounterId!.Substring(CounterId.Length - 1, 1));
			_hash.Add(101 + (numCounter * 1000)); //Текущее время и дата.				(TimeAndDate)
			_hash.Add(102 + (numCounter * 1000)); //Заводской номер.					(SerialNumber)
			_hash.Add(105 + (numCounter * 1000)); //Интерфейс.							(InterfaceNumber)
			_hash.Add(120 + (numCounter * 1000)); //Ввод в эксплуатацию.				(Commissioning)
			_hash.Add(124 + (numCounter * 1000)); //Время простоя.						(DownTime)
			_hash.Add(125 + (numCounter * 1000)); //Время наработки						(RunningTime)
			_hash.Add(132 + (numCounter * 1000)); //Максимум датчиков давления.			(MaxSensorsPressure)
			_hash.Add(218 + (numCounter * 1000)); //Расход максимальный - канал №1.		(FlowMax_ch1)
			_hash.Add(219 + (numCounter * 1000)); //Расход граничный - канал №1.		(FlowBoundary_ch1)
			_hash.Add(258 + (numCounter * 1000)); //Расход максимальный - канал №2.		(FlowMax_ch2)
			_hash.Add(259 + (numCounter * 1000)); //Расход граничный - канал №2.		(FlowBoundary_ch2)
		}
		else
		{
			MessageBox.Show(
				"Ошибка! Проверьте инициализацию счетчиков.\n" +
				".в ItemTabControlTagViewModel!",
				"Внимание!",
				MessageBoxButton.OK,
				MessageBoxImage.Error);
		}
	}

	private void InitializationData()
	{
		if(!_hash.IsNullOrEmpty())
		{
			DiscriptionCounter = GetDiscriptionCounter(CounterId);
			TimeAndDate = _userRepositoriesDB.GetValueChanel(_hash[0]);
			SerialNumber = _userRepositoriesDB.GetValueChanel(_hash[1]);
			InterfaceNumber = _userRepositoriesDB.GetValueChanel(_hash[2]);
			Commissioning = _userRepositoriesDB.GetValueChanel(_hash[3])!.Insert(2, "-").Insert(5, "-").Insert(8, ":");
			DownTime = _userRepositoriesDB.GetValueChanel(_hash[4]);
			RunningTime = _userRepositoriesDB.GetValueChanel(_hash[5]);
			MaxSensorsPressure = _userRepositoriesDB.GetValueChanel(_hash[6])!.Insert(1, ".");
			FlowMax_ch1 = _userRepositoriesDB.GetValueChanel(_hash[7]);
			FlowBoundary_ch1 = _userRepositoriesDB.GetValueChanel(_hash[8]);
			FlowMax_ch2 = _userRepositoriesDB.GetValueChanel(_hash[9]);
			FlowBoundary_ch2 = _userRepositoriesDB.GetValueChanel(_hash[10]);
		}
	}

}
