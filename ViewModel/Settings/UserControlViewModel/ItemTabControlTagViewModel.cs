using Microsoft.IdentityModel.Tokens;
using Microsoft.Windows.Themes;
using SNT2_WPF.Communication.Logger;
using SNT2_WPF.Communication.Repositories;
using SNT2_WPF.Models.DataSettingsTeg;
using SNT2_WPF.Models.DBModel;
using SNT2_WPF.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SNT2_WPF.ViewModel.Settings.UserControlViewModel;
internal class ItemTabControlTagViewModel : Base.ViewModel
{
	//Список хэшей.
	private List<int> _hash;
	//Инициализация наименования вкладок в "TabControl".
    public string TabName { get; init; }

	private Counters settingsCounter = null!;
	private Counters changedSettingsCounter = null!;

	readonly JsonSerializerOptions options = new (){ WriteIndented = true };

	#region Свойства неизменных параметров счетчика.

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

	#endregion

	#region Свойства изменяемых параметров счетчика.

	#region Temperature_ch1_Max : string? - Температура максимальная - канал №1.

	/// <summary>Температура максимальная - канал №1. - поле.</summary>
	private string? _temperature_ch1_Max;

	/// <summary>Температура максимальная - канал №1. - свойство.</summary>
	public string? Temperature_ch1_Max
	{
		get => _temperature_ch1_Max;
		set
		{
			_temperature_ch1_Max = value;
			OnPropertyChanged(nameof(Temperature_ch1_Max));
		}
	}
	#endregion

	#region Temperature_ch1_Min : string? - Температура минимальная - канал №1.

	/// <summary>Температура минимальная - канал №1. - поле.</summary>
	private string? _temperature_ch1_Min;

	/// <summary>Температура манимальная - канал №1. - свойство.</summary>
	public string? Temperature_ch1_Min
	{
		get => _temperature_ch1_Min;
		set
		{
			_temperature_ch1_Min = value;
			OnPropertyChanged(nameof(Temperature_ch1_Min));
		}
	}
	#endregion

	#region Temperature_ch1_CDAD : string? - Кол-во знаков после запятой, Температура - канал №1.

	/// <summary>Кол-во знаков после запятой, Температура - канал №1. - поле.</summary>
	private string? _temperature_ch1_CDAD;

	/// <summary>Кол-во знаков после запятой, Температура - канал №1. - свойство.</summary>
	public string? Temperature_ch1_CDAD
	{
		get => _temperature_ch1_CDAD;
		set
		{
			_temperature_ch1_CDAD = value;
			OnPropertyChanged(nameof(Temperature_ch1_CDAD));
		}
	}
	#endregion

	#region FlowVolume_ch1_Max : string? - Расход объемный максимальный - канал №1

	/// <summary>Расход объемный максимальный - канал №1 - поле.</summary>
	private string? _flowVolume_ch1_Max;

	/// <summary>Расход объемный максимальный - канал №1 - свойство.</summary>
	public string? FlowVolume_ch1_Max
	{
		get => _flowVolume_ch1_Max;
		set
		{
			_flowVolume_ch1_Max = value;
			OnPropertyChanged(nameof(FlowVolume_ch1_Max));
		}
	}
	#endregion

	#region FlowVolume_ch1_Min : string? - Расход объемный минимальный - канал №1

	/// <summary>Расход объемный минимальный - канал №1 - поле.</summary>
	private string? _flowVolume_ch1_Min;

	/// <summary>Расход объемный минимальный - канал №1 - свойство.</summary>
	public string? FlowVolume_ch1_Min
	{
		get => _flowVolume_ch1_Min;
		set
		{
			_flowVolume_ch1_Min = value;
			OnPropertyChanged(nameof(FlowVolume_ch1_Min));
		}
	}
	#endregion

	#region FlowVolume_ch1_CDAD : string? - Кол-во знаков после запятой, Расход объемный - канал №1

	/// <summary>Кол-во знаков после запятой, Расход объемный - канал №1 - поле.</summary>
	private string? _flowVolume_ch1_CDAD;

	/// <summary>Кол-во знаков после запятой, Расход объемный - канал №1 - свойство.</summary>
	public string? FlowVolume_ch1_CDAD
	{
		get => _flowVolume_ch1_CDAD;
		set
		{
			_flowVolume_ch1_CDAD = value;
			OnPropertyChanged(nameof(FlowVolume_ch1_CDAD));
		}
	}
	#endregion

	#region FlowMass_ch1_Max : string? - Расход массовый максимальный - канал №1

	/// <summary>Расход массовый максимальный - канал №1 - поле.</summary>
	private string? _flowMass_ch1_Max;

	/// <summary>Расход массовый максимальный - канал №1 - свойство.</summary>
	public string? FlowMass_ch1_Max
	{
		get => _flowMass_ch1_Max;
		set
		{
			_flowMass_ch1_Max = value;
			OnPropertyChanged(nameof(FlowMass_ch1_Max));
		}
	}
	#endregion

	#region FlowMass_ch1_Min : string? - Расход массовый минимальный - канал №1

	/// <summary>Расход массовый минимальный - канал №1 - поле.</summary>
	private string? _flowMass_ch1_Min;

	/// <summary>Расход массовый минимальный - канал №1 - свойство.</summary>
	public string? FlowMass_ch1_Min
	{
		get => _flowMass_ch1_Min;
		set
		{
			_flowMass_ch1_Min = value;
			OnPropertyChanged(nameof(FlowMass_ch1_Min));
		}
	}
	#endregion

	#region FlowMass_ch1_CDAD : string? - Кол-во знаков после запятой, Расход массовый - канал №1

	/// <summary>Кол-во знаков после запятой, Расход массовый - канал №1 - поле.</summary>
	private string? _flowMass_ch1_CDAD;

	/// <summary>Кол-во знаков после запятой, Расход массовый - канал №1 - свойство.</summary>
	public string? FlowMass_ch1_CDAD
	{
		get => _flowMass_ch1_CDAD;
		set
		{
			_flowMass_ch1_CDAD = value;
			OnPropertyChanged(nameof(FlowMass_ch1_CDAD));
		}
	}
	#endregion

	#region Pressure_ch1_Max : string? - Давление максимальное - канал №1.

	/// <summary>Давление максимальное - канал №1. - поле.</summary>
	private string? _pressure_ch1_Max;

	/// <summary>Давление максимальное - канал №1. - свойство.</summary>
	public string? Pressure_ch1_Max
	{
		get => _pressure_ch1_Max;
		set
		{
			_pressure_ch1_Max = value;
			OnPropertyChanged(nameof(Pressure_ch1_Max));
		}
	}
	#endregion

	#region Pressure_ch1_Min : string? - Давление минимальное - канал №1.

	/// <summary>Давление минимальное - канал №1. - поле.</summary>
	private string? _pressure_ch1_Min;

	/// <summary>Давление минимальное - канал №1. - свойство.</summary>
	public string? Pressure_ch1_Min
	{
		get => _pressure_ch1_Min;
		set
		{
			_pressure_ch1_Min = value;
			OnPropertyChanged(nameof(Pressure_ch1_Min));
		}
	}
	#endregion

	#region Pressure_ch1_CDAD : string? - Кол-во знаков после запятой, Давление - канал №1.

	/// <summary>Кол-во знаков после запятой, Давление - канал №1. - поле.</summary>
	private string? _pressure_ch1_CDAD;

	/// <summary>Кол-во знаков после запятой, Давление - канал №1. - свойство.</summary>
	public string? Pressure_ch1_CDAD
	{
		get => _pressure_ch1_CDAD;
		set
		{
			_pressure_ch1_CDAD = value;
			OnPropertyChanged(nameof(Pressure_ch1_CDAD));
		}
	}
	#endregion

	#region Temperature_ch2_Max : string? - Температура максимальная - канал №2.

	/// <summary>Температура максимальная - канал №2. - поле.</summary>
	private string? _temperature_ch2_Max;

	/// <summary>Температура максимальная - канал №2. - свойство.</summary>
	public string? Temperature_ch2_Max
	{
		get => _temperature_ch2_Max;
		set
		{
			_temperature_ch2_Max = value;
			OnPropertyChanged(nameof(Temperature_ch2_Max));
		}
	}
	#endregion

	#region Temperature_ch2_Min : string? - Температура минимальная - канал №2.

	/// <summary>Температура минимальная - канал №2. - поле.</summary>
	private string? _temperature_ch2_Min;

	/// <summary>Температура манимальная - канал №2. - свойство.</summary>
	public string? Temperature_ch2_Min
	{
		get => _temperature_ch2_Min;
		set
		{
			_temperature_ch2_Min = value;
			OnPropertyChanged(nameof(Temperature_ch2_Min));
		}
	}
	#endregion

	#region Temperature_ch2_CDAD : string? - Кол-во знаков после запятой, Температура - канал №2.

	/// <summary>Кол-во знаков после запятой, Температура - канал №2. - поле.</summary>
	private string? _temperature_ch2_CDAD;

	/// <summary>Кол-во знаков после запятой, Температура - канал №2. - свойство.</summary>
	public string? Temperature_ch2_CDAD
	{
		get => _temperature_ch2_CDAD;
		set
		{
			_temperature_ch2_CDAD = value;
			OnPropertyChanged(nameof(Temperature_ch2_CDAD));
		}
	}
	#endregion

	#region FlowVolume_ch2_Max : string? - Расход объемный максимальный - канал №2

	/// <summary>Расход объемный максимальный - канал №2 - поле.</summary>
	private string? _flowVolume_ch2_Max;

	/// <summary>Расход объемный максимальный - канал №2 - свойство.</summary>
	public string? FlowVolume_ch2_Max
	{
		get => _flowVolume_ch2_Max;
		set
		{
			_flowVolume_ch2_Max = value;
			OnPropertyChanged(nameof(FlowVolume_ch2_Max));
		}
	}
	#endregion

	#region FlowVolume_ch2_Min : string? - Расход объемный минимальный - канал №2

	/// <summary>Расход объемный минимальный - канал №2 - поле.</summary>
	private string? _flowVolume_ch2_Min;

	/// <summary>Расход объемный минимальный - канал №2 - свойство.</summary>
	public string? FlowVolume_ch2_Min
	{
		get => _flowVolume_ch2_Min;
		set
		{
			_flowVolume_ch2_Min = value;
			OnPropertyChanged(nameof(FlowVolume_ch2_Min));
		}
	}
	#endregion

	#region FlowVolume_ch2_CDAD : string2 - Кол-во знаков после запятой, Расход объемный - канал №2

	/// <summary>Кол-во знаков после запятой, Расход объемный - канал №2 - поле.</summary>
	private string? _flowVolume_ch2_CDAD;

	/// <summary>Кол-во знаков после запятой, Расход объемный - канал №2 - свойство.</summary>
	public string? FlowVolume_ch2_CDAD
	{
		get => _flowVolume_ch2_CDAD;
		set
		{
			_flowVolume_ch2_CDAD = value;
			OnPropertyChanged(nameof(FlowVolume_ch2_CDAD));
		}
	}
	#endregion

	#region FlowMass_ch2_Max : string? - Расход массовый максимальный - канал №2

	/// <summary>Расход массовый максимальный - канал №2 - поле.</summary>
	private string? _flowMass_ch2_Max;

	/// <summary>Расход массовый максимальный - канал №2 - свойство.</summary>
	public string? FlowMass_ch2_Max
	{
		get => _flowMass_ch2_Max;
		set
		{
			_flowMass_ch2_Max = value;
			OnPropertyChanged(nameof(FlowMass_ch2_Max));
		}
	}
	#endregion

	#region FlowMass_ch2_Min : string? - Расход массовый минимальный - канал №2

	/// <summary>Расход массовый минимальный - канал №2 - поле.</summary>
	private string? _flowMass_ch2_Min;

	/// <summary>Расход массовый минимальный - канал №2 - свойство.</summary>
	public string? FlowMass_ch2_Min
	{
		get => _flowMass_ch2_Min;
		set
		{
			_flowMass_ch2_Min = value;
			OnPropertyChanged(nameof(FlowMass_ch2_Min));
		}
	}
	#endregion

	#region FlowMass_ch2_CDAD : string? - Кол-во знаков после запятой, Расход массовый - канал №2

	/// <summary>Кол-во знаков после запятой, Расход массовый - канал №2 - поле.</summary>
	private string? _flowMass_ch2_CDAD;

	/// <summary>Кол-во знаков после запятой, Расход массовый - канал №2 - свойство.</summary>
	public string? FlowMass_ch2_CDAD
	{
		get => _flowMass_ch2_CDAD;
		set
		{
			_flowMass_ch2_CDAD = value;
			OnPropertyChanged(nameof(FlowMass_ch2_CDAD));
		}
	}
	#endregion

	#region Pressure_ch2_Max : string? - Давление максимальное - канал №2.

	/// <summary>Давление максимальное - канал №2. - поле.</summary>
	private string? _pressure_ch2_Max;

	/// <summary>Давление максимальное - канал №2. - свойство.</summary>
	public string? Pressure_ch2_Max
	{
		get => _pressure_ch2_Max;
		set
		{
			_pressure_ch2_Max = value;
			OnPropertyChanged(nameof(Pressure_ch2_Max));
		}
	}
	#endregion

	#region Pressure_ch2_Min : string? - Давление минимальное - канал №2.

	/// <summary>Давление минимальное - канал №2. - поле.</summary>
	private string? _pressure_ch2_Min;

	/// <summary>Давление минимальное - канал №2. - свойство.</summary>
	public string? Pressure_ch2_Min
	{
		get => _pressure_ch2_Min;
		set
		{
			_pressure_ch2_Min = value;
			OnPropertyChanged(nameof(Pressure_ch2_Min));
		}
	}
	#endregion

	#region Pressure_ch2_CDAD : string? - Кол-во знаков после запятой, Давление - канал №2.

	/// <summary>Кол-во знаков после запятой, Давление - канал №2. - поле.</summary>
	private string? _pressure_ch2_CDAD;

	/// <summary>Кол-во знаков после запятой, Давление - канал №2. - свойство.</summary>
	public string? Pressure_ch2_CDAD
	{
		get => _pressure_ch2_CDAD;
		set
		{
			_pressure_ch2_CDAD = value;
			OnPropertyChanged(nameof(Pressure_ch2_CDAD));
		}
	}
	#endregion
	
	#endregion

	private readonly IRepositoriesDB _userRepositoriesDB = null!;


	#region Command Temperature_ch1_SaveCommand - Сохранение параметров температуры канала №1. 

	/// <summary>Сохранение параметров температуры канала №1. </summary>
	private LambdaCommand? _Temperature_ch1_SaveCommand;

	/// <summary>Сохранение параметров температуры канала №1. </summary>
	public ICommand Temperature_ch1_SaveCommand => _Temperature_ch1_SaveCommand ??= new(ExecutedTemperature_ch1_SaveCommand);

	/// <summary>Логика выполнения - Сохранение параметров температуры канала №1. </summary>
	private void ExecutedTemperature_ch1_SaveCommand()
	{
		try
		{
			using (FileStream fs = new FileStream(@"Resources\\db_List_SettingsTeg.json", FileMode.OpenOrCreate))
			{
				settingsCounter.CountersList!
						.Where(c => c.CounterId == CounterId)
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch1_Max = Temperature_ch1_Max;
				settingsCounter.CountersList!
						.Where(c => c.CounterId == CounterId)
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch1_Min = Temperature_ch1_Min;
				settingsCounter.CountersList!
						.Where(c => c.CounterId == CounterId)
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch1_CDAD = Temperature_ch1_CDAD;
				JsonSerializer.Serialize(fs, settingsCounter, options);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Этап 1 - Ошибка чтения json-файла.");
		}
	}
	#endregion


	public ItemTabControlTagViewModel(string tabName, string countId)
	{
		TabName = tabName;
		CounterId = countId;
		_hash = new List<int>();
		settingsCounter = new();
		changedSettingsCounter = new();

		_userRepositoriesDB = new UserRepositoriesDB();

		InitializeHashCounter();
		InitializationData();
		InitializationSettingsTegCounter();
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

	private void InitializationSettingsTegCounter()
	{
		try
		{
			using (FileStream fs = new FileStream(@"Resources\\db_List_SettingsTeg.json", FileMode.OpenOrCreate))
			{
				settingsCounter = JsonSerializer.Deserialize<Counters>(fs)!;

				if (settingsCounter is not null)
				{
					var listParametrs = settingsCounter.CountersList!
						.Where(c => c.CounterId == CounterId)
						.Select(p => p.SettingsCounterParameters)
						.First();

					InitializationSettingsTeg(listParametrs!);
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Этап 1 - Ошибка чтения json-файла.");
		}
	}

	private void InitializationSettingsTeg(SettingsCounterParameters settingsCounterParameters)
	{
		Temperature_ch1_Max = settingsCounterParameters.Temperature_ch1_Max;
		Temperature_ch1_Min = settingsCounterParameters.Temperature_ch1_Min;
		Temperature_ch1_CDAD = settingsCounterParameters.Temperature_ch1_CDAD;
		FlowVolume_ch1_Max = settingsCounterParameters.FlowVolume_ch1_Max;
		FlowVolume_ch1_Min = settingsCounterParameters.FlowVolume_ch1_Min;
		FlowVolume_ch1_CDAD = settingsCounterParameters.FlowVolume_ch1_CDAD;
		FlowMass_ch1_Max = settingsCounterParameters.FlowMass_ch1_Max;
		FlowMass_ch1_Min = settingsCounterParameters.FlowMass_ch1_Min;
		FlowMass_ch1_CDAD = settingsCounterParameters.FlowMass_ch1_CDAD;
		Pressure_ch1_Max = settingsCounterParameters.Pressure_ch1_Max;
		Pressure_ch1_Min = settingsCounterParameters.Pressure_ch1_Min;
		Pressure_ch1_CDAD = settingsCounterParameters.Pressure_ch1_CDAD;
		Temperature_ch2_Max = settingsCounterParameters.Temperature_ch2_Max;
		Temperature_ch2_Min = settingsCounterParameters.Temperature_ch2_Min;
		Temperature_ch2_CDAD = settingsCounterParameters.Temperature_ch2_CDAD;
		FlowVolume_ch2_Max = settingsCounterParameters.FlowVolume_ch2_Max;
		FlowVolume_ch2_Min = settingsCounterParameters.FlowVolume_ch2_Min;
		FlowVolume_ch2_CDAD = settingsCounterParameters.FlowVolume_ch2_CDAD;
		FlowMass_ch2_Max = settingsCounterParameters.FlowMass_ch2_Max;
		FlowMass_ch2_Min = settingsCounterParameters.FlowMass_ch2_Min;
		FlowMass_ch2_CDAD = settingsCounterParameters.FlowMass_ch2_CDAD;
		Pressure_ch2_Max = settingsCounterParameters.Pressure_ch2_Max;
		Pressure_ch2_Min = settingsCounterParameters.Pressure_ch2_Min;
		Pressure_ch2_CDAD = settingsCounterParameters.Pressure_ch2_CDAD;
	}

}
