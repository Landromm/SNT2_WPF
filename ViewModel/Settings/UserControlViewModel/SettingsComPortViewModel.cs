using SNT2_WPF.ViewModel.Base;
using SNT2_WPF.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SNT2_WPF.ViewModel.Settings.UserControlViewModel;
internal class SettingsComPortViewModel : Base.ViewModel
{
	private readonly List<string> baudRateList = new List<string>(){"4800", "9600", "38400", "57600"};
	private readonly List<string> parityList = new List<string>(){"Even", "Odd", "None", "Mark", "Space"};
	private readonly List<string> stopBitsList = new List<string>(){"One", "Two"};
	private readonly List<int> dataBitsList = new List<int>(){5, 6, 7, 8};

	#region PortName : string - Имя COM-порта.

	/// <summary>Имя COM-порта. - поле.</summary>
	private string _portName;

	/// <summary>Имя COM-порта. - свойство.</summary>
	public string PortName
	{
		get => _portName;
		set
		{
			_portName = value;
			OnPropertyChanged(nameof(PortName));
		}
	}
	#endregion

	#region BaudRate : string - Скорость.

	/// <summary>Скорость. - поле.</summary>
	private string _baudRate;

	/// <summary>Скорость. - свойство.</summary>
	public string BaudRate
	{
		get => _baudRate;
		set
		{
			_baudRate = value;
			OnPropertyChanged(nameof(BaudRate));
		}
	}
	#endregion

	#region Parity : string - Четность.

	/// <summary>Четность. - поле.</summary>
	private string _parity;

	/// <summary>Четность. - свойство.</summary>
	public string Parity
	{
		get => _parity;
		set
		{
			_parity = value;
			OnPropertyChanged(nameof(Parity));
		}
	}
	#endregion

	#region StopBits : string - Стоповые биты.

	/// <summary>Стоповые биты. - поле.</summary>
	private string _stopBits;

	/// <summary>Стоповые биты. - свойство.</summary>
	public string StopBits
	{
		get => _stopBits;
		set
		{
			_stopBits = value;
			OnPropertyChanged(nameof(StopBits));
		}
	}
	#endregion

	#region DataBits : string - Биты данных.

	/// <summary>Биты данных. - поле.</summary>
	private string _dataBits;

	/// <summary>Биты данных. - свойство.</summary>
	public string DataBits
	{
		get => _dataBits;
		set
		{
			_dataBits = value;
			OnPropertyChanged(nameof(DataBits));
		}
	}
	#endregion

	#region Timeout : string - Время задержки.

	/// <summary>Время задержки. - поле.</summary>
	private string _timeout;

	/// <summary>Время задержки. - свойство.</summary>
	public string Timeout
	{
		get => _timeout;
		set
		{
			_timeout = value;
			OnPropertyChanged(nameof(Timeout));
		}
	}
	#endregion

	public SettingsComPortViewModel()
	{
		PortName = Properties.Settings.Default.PortName;
		BaudRate = Properties.Settings.Default.BaudRate.ToString();
		Parity = Properties.Settings.Default.Parity;
		StopBits = Properties.Settings.Default.StopBits;
		DataBits = Properties.Settings.Default.DataBits.ToString();
		Timeout = Properties.Settings.Default.TimeOutRead.ToString();
	}

	#region Command ResetCommand - Сброс данных по умолчанию.

	/// <summary>Сброс данных по умолчанию.</summary>
	private LambdaCommand? _ResetCommand;

	/// <summary>Сброс данных по умолчанию.</summary>
	public ICommand ResetCommand => _ResetCommand ??= new(ExecutedResetCommand);

	/// <summary>Логика выполнения - Сброс данных по умолчанию.</summary>
	private void ExecutedResetCommand()
	{
		PortName = Properties.Settings.Default.PortName;
		BaudRate = Properties.Settings.Default.BaudRate.ToString();
		Parity = Properties.Settings.Default.Parity;
		StopBits = Properties.Settings.Default.StopBits;
		DataBits = Properties.Settings.Default.DataBits.ToString();
		Timeout = Properties.Settings.Default.TimeOutRead.ToString();
	}
	#endregion

	#region Command SaveCommand - Сохранение настроек COM порта.

	/// <summary>Сохранение настроек COM порта.</summary>
	private LambdaCommand? _SaveCommand;

	/// <summary>Сохранение настроек COM порта.</summary>
	public ICommand SaveCommand => _SaveCommand ??= new(ExecutedSaveCommand, CanExecutedSaveCommand);	

	/// <summary>Логика выполнения - Сохранение настроек COM порта.</summary>
	private void ExecutedSaveCommand()
	{
		// Проверка поля: COM-порта.
		if (!PortName.StartsWith("COM") || !int.TryParse(PortName.AsSpan("COM".Length), out _))
		{
			MessageBox.Show(
				"Некорректное 'наименование' COM-порта.",
				"Внимание!",
				MessageBoxButton.OK,
				MessageBoxImage.Error
				);
			return ;
		}
		// Проверка поля: Скорость.
		if(!baudRateList.Contains(BaudRate))
		{
			MessageBox.Show(
			"Введена неправильная 'скорость'.",
			"Внимание!",
			MessageBoxButton.OK,
			MessageBoxImage.Error
			);
			return;
		}
		// Проверка поля: Четность.
		if (!parityList.Contains(Parity))
		{
			MessageBox.Show(
			"Введена неправильная 'четность'.",
			"Внимание!",
			MessageBoxButton.OK,
			MessageBoxImage.Error
			);
			return;
		}
		// Проверка поля: Стоповые биты.
		if (!stopBitsList.Contains(StopBits))
		{
			MessageBox.Show(
			"Введено неправильное количество 'Стоповых битов'.",
			"Внимание!",
			MessageBoxButton.OK,
			MessageBoxImage.Error
			);
			return;
		}
		// Проверка поля: Биты данных.
		if(!int.TryParse(DataBits, out int dataBit) || !dataBitsList.Contains(dataBit))
		{
			MessageBox.Show(
			"Введено неправильное количество 'Бит данных'.",
			"Внимание!",
			MessageBoxButton.OK,
			MessageBoxImage.Error
			);
			return;
		}
		// Проверка поля: Время задержки.
		if (!int.TryParse(Timeout, out _))
		{
			MessageBox.Show(
			"Введено некорректное значение 'задержки времени'.",
			"Внимание!",
			MessageBoxButton.OK,
			MessageBoxImage.Error
			);
			return;
		}

		// Запись введенных данных в файл конфигурации.
		Properties.Settings.Default.PortName = PortName;
		Properties.Settings.Default.BaudRate = Convert.ToInt32(BaudRate);
		Properties.Settings.Default.Parity = Parity;
		Properties.Settings.Default.StopBits = StopBits;
		Properties.Settings.Default.DataBits = Convert.ToInt32(DataBits);
		Properties.Settings.Default.TimeOutRead = Convert.ToInt32(Timeout);
		Properties.Settings.Default.Save();	//Сохранение введенных данных в файл конфигурации.
	}
	/// <summary>Условия выполнения - Сохранение настроек COM порта.</summary>
	private bool CanExecutedSaveCommand()
	{
		return (GetResultIsEmptyString(PortName)
			|| GetResultIsEmptyString(BaudRate)
			|| GetResultIsEmptyString(Parity)
			|| GetResultIsEmptyString(StopBits)
			|| GetResultIsEmptyString(DataBits)
			|| GetResultIsEmptyString(Timeout)) ? false : true;
	}
	#endregion

	/// <summary>Проверка на пустую строку.</summary>
	/// <param name="str">Строковый параметр</param>
	/// <returns>Возвращает 'true' если строка пуста или равна 'null'</returns>
	private bool GetResultIsEmptyString(string str)
	{
		return str is null || str.Length == 0 ? true : false;
	}






}
