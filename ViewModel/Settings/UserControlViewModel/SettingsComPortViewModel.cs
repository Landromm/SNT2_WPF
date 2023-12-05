using SNT2_WPF.ViewModel.Base;
using SNT2_WPF.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNT2_WPF.ViewModel.Settings.UserControlViewModel;
internal class SettingsComPortViewModel : Base.ViewModel
{
	#region PortName : string? - Имя COM-порта.

	/// <summary>Имя COM-порта. - поле.</summary>
	private string? _portName;

	/// <summary>Имя COM-порта. - свойство.</summary>
	public string? PortName
	{
		get => _portName;
		set
		{
			_portName = value;
			OnPropertyChanged(nameof(PortName));
		}
	}
	#endregion

	#region BaudRate : string? - Скорость.

	/// <summary>Скорость. - поле.</summary>
	private string? _baudRate;

	/// <summary>Скорость. - свойство.</summary>
	public string? BaudRate
	{
		get => _baudRate;
		set
		{
			_baudRate = value;
			OnPropertyChanged(nameof(BaudRate));
		}
	}
	#endregion

	#region Parity : string? - Четность.

	/// <summary>Четность. - поле.</summary>
	private string? _parity;

	/// <summary>Четность. - свойство.</summary>
	public string? Parity
	{
		get => _parity;
		set
		{
			_parity = value;
			OnPropertyChanged(nameof(Parity));
		}
	}
	#endregion

	#region StopBits : string? - Стоповые биты.

	/// <summary>Стоповые биты. - поле.</summary>
	private string? _stopBits;

	/// <summary>Стоповые биты. - свойство.</summary>
	public string? StopBits
	{
		get => _stopBits;
		set
		{
			_stopBits = value;
			OnPropertyChanged(nameof(StopBits));
		}
	}
	#endregion

	#region DataBits : string? - Биты данных.

	/// <summary>Биты данных. - поле.</summary>
	private string? _dataBits;

	/// <summary>Биты данных. - свойство.</summary>
	public string? DataBits
	{
		get => _dataBits;
		set
		{
			_dataBits = value;
			OnPropertyChanged(nameof(DataBits));
		}
	}
	#endregion

	#region Timeout : string? - Время задержки.

	/// <summary>Время задержки. - поле.</summary>
	private string? _timeout;

	/// <summary>Время задержки. - свойство.</summary>
	public string? Timeout
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





}
