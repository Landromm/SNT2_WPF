using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using Microsoft.IdentityModel.Tokens;
using SkiaSharp;
using SNT2_WPF.Communication.Repositories;
using SNT2_WPF.Models.DataModel;
using SNT2_WPF.Models.DataModel.DataControl;
using SNT2_WPF.Models.DataSettingsTeg;
using SNT2_WPF.Models.DBModel;
using SNT2_WPF.Services;
using SNT2_WPF.ViewModel.Base;
using SNT2_WPF.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SNT2_WPF.ViewModel.Graph
{
    public class GraphCurrentViewModel : DialogViewModel
    {
		private readonly IUserDialog _userDialog = null!;
		private readonly IMessageBus _messageBus = null!;
		private readonly IDisposable _subscription = null!;
		private readonly IRepositoriesDB _userRepositoriesDB = null!;

		private readonly Random _random = new();
		private readonly List<DateTimePoint> _values = new();
		private readonly DateTimeAxis _customAxis;
		private CDADModel? _cdadModel = null!;
		private Counters? settingsCounter = null!;
		private string? cdadParametr = string.Empty;

		private Dictionary<string, string> HashCDAD = new();

		public CDADModel? CdadModel
		{
			get => _cdadModel;
			set => _cdadModel = value;
		}

		#region HashId : string? - Хэш-id параметра
		/// <summary>Хэш-id параметра - поле.</summary>
		private string? _HashId;

		/// <summary>Хэш-id параметра - свойство.</summary>
		public string? HashId
		{
			get => _HashId;
			set
			{
				_HashId = value;
				OnPropertyChanged(nameof(HashId));
			}
		}
		#endregion

		#region Description : string? - Наименование счетчика.
		/// <summary>Наименование счетчика. - поле.</summary>
		private string? _Description;

		/// <summary>Наименование счетчика. - свойство.</summary>
		public string? Description
		{
			get => _Description;
			set
			{
				_Description = value;
				OnPropertyChanged(nameof(Description));
			}
		}
		#endregion

		#region NameParametr : string? - Наименование параметра счетчика.
		/// <summary>Наименование параметра счетчика. - поле.</summary>
		private string? _nameParametr;

		/// <summary>Наименование параметра счетчика. - свойство.</summary>
		public string? NameParametr
		{
			get => _nameParametr;
			set
			{
				_nameParametr = value;
				OnPropertyChanged(nameof(NameParametr));
			}
		}
		#endregion

		#region LastCurrentValue : string? - Последнее прочитатое текущее значение параметра.
		/// <summary>Последнее прочитатое текущее значение параметра. - поле.</summary>
		private string? _lastCurrentValue;

		/// <summary>Последнее прочитатое текущее значение параметра. - свойство.</summary>
		public string? LastCurrentValue
		{
			get => _lastCurrentValue;
			set
			{
				_lastCurrentValue = value;
				OnPropertyChanged(nameof(LastCurrentValue));
			}
		}
		#endregion

		public ObservableCollection<ISeries>? Series
		{
			get; set;
		}
		public ISeries[]? SeriesCollection
		{
			get; set;
		}

		public Axis[] XAxes { get; set; } 
			= new Axis[]
				{
					 new Axis
					 {
						NamePaint = new SolidColorPaint(SKColors.Black),

						LabelsPaint = new SolidColorPaint(new SKColor(61, 61, 61), 2),
						TextSize = 10,

						SeparatorsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(80), 1)
						{
							StrokeThickness = 1,
							PathEffect = new DashEffect(new float[] { 2, 2 })
						}
						//SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 1 }
					 }
				};
		public Axis[] YAxes { get; set; }
			= new Axis[]
			{
					new Axis
					{
						NamePaint = new SolidColorPaint(SKColors.Black),

						LabelsPaint = new SolidColorPaint(new SKColor(61, 61, 61), 4),
						TextSize = 16,
						SeparatorsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(80), 1)
						{
							StrokeThickness = 1,
							PathEffect = new DashEffect(new float[] { 2, 2 })
						}
					}
			};
		public DrawMarginFrame DrawMarginFrame => new()
		{
			Fill = new SolidColorPaint(new SKColor(153, 153, 153)),
			Stroke = new SolidColorPaint(SKColors.Black.WithAlpha(90), 1)
		};

		public object Sync { get; } = new object();
		public bool IsReading { get; set; } = true;

		public GraphCurrentViewModel(IUserDialog UserDialog, IMessageBus MessageBus)
		{
			_userDialog = UserDialog;
			_messageBus = MessageBus;
			_subscription = MessageBus.RegisterHandler<Message>(OnReceiveMessage);
			_userRepositoriesDB = new UserRepositoriesDB();

			_customAxis = new DateTimeAxis(TimeSpan.FromSeconds(1), Formatter)
			{
				CustomSeparators = GetSeparators(),
				AnimationsSpeed = TimeSpan.FromMilliseconds(0),
				SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(100))
			};
			InitializDictionaryCDAD();
			InitializationCartesianChart(_customAxis);

			_ = ReadData();
		}

		private void InitializationCartesianChart(DateTimeAxis _axis)
		{
			Series = new ObservableCollection<ISeries>
			{
				new LineSeries<DateTimePoint>
				{
					Values = _values,
					Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },
					Fill = null,
					GeometryFill = new SolidColorPaint(SKColors.AliceBlue),
					GeometryStroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
					LineSmoothness = 0,
					GeometrySize = 0
				}
			};

			XAxes = new Axis[] { _axis };
		}

		private async Task ReadData()
		{
			// to keep this sample simple, we run the next infinite loop 
			// in a real application you should stop the loop/task when the view is disposed 

			while (IsReading)
			{
				await Task.Delay(3000);

				lock (Sync)
				{
					if (!_values.IsNullOrEmpty())
					{
						GetCdadParametr(HashId!);
						GetLastListValueData(_values);
					}
					else
					{
						GetCdadParametr(HashId!);
						var tempTouple = _userRepositoriesDB.GetHistoryData(HashId);

						if (!tempTouple.IsNullOrEmpty())
						{
							foreach (var item in tempTouple)
							{
								// Придумать обработку исключений.
								var convertValue = Convert.ToDouble(GetValueWithDot(item.Item2, cdadParametr!));
								_values.Add(new DateTimePoint(item.Item1, convertValue));
							}

							LastCurrentValue = $":[{_values.Last().Value}]";
							_customAxis.CustomSeparators = GetSeparators();
						}
						else
						{
							GetLastListValueData(_values);
						}
					}
				}
			}
		}

		private double[] GetSeparators()
		{
			var now = DateTime.Now;

			return new double[]
			{
				now.AddMinutes(-60).Ticks,
				now.AddMinutes(-50).Ticks,
				now.AddMinutes(-40).Ticks,
				now.AddMinutes(-30).Ticks,
				now.AddMinutes(-20).Ticks,
				now.AddMinutes(-10).Ticks,
				now.Ticks
			};
		}

		private static string Formatter(DateTime date)
		{
			var secsAgo = date.ToString("HH:mm:ss");

			return secsAgo ;
		}

		private void OnReceiveMessage(Message message)
		{
			HashId = message.HashId;
			Description = message.Description;
			NameParametr = $" - {message.NameParametr}";
			Dispose();
		}

		public void Dispose() => _subscription.Dispose();

		private void GetCdadParametr(string hashId)
		{
			cdadParametr = HashCDAD.GetValueOrDefault(hashId);
		}

		private string GetValueWithDot(string? value, string cdad)
		{
			var cdadInt = Convert.ToInt32(cdad);
			if(value is not null)
			{
				if (cdadInt == 0)
					return value;
				return value.Insert(value.Length - cdadInt, ",");
			}
			return value = "";
		}

		private void InitializDictionaryCDAD()
		{
			string? tempCdad = string.Empty;
			try
			{
				using FileStream fs = new FileStream(@"Resources\\db_List_SettingsTeg.json", FileMode.OpenOrCreate);

				settingsCounter = JsonSerializer.Deserialize<Counters>(fs)!;
				if (settingsCounter is not null)
				{
					#region CDAD СНТ-2 №1
					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №1")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch1_CDAD;
					HashCDAD.Add("1116", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №1")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowVolume_ch1_CDAD;
					HashCDAD.Add("1222", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №1")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowMass_ch1_CDAD;
					HashCDAD.Add("1225", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №1")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Pressure_ch1_CDAD;
					HashCDAD.Add("1229", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №1")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch2_CDAD;
					HashCDAD.Add("1117", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №1")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowVolume_ch2_CDAD;
					HashCDAD.Add("1262", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №1")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowMass_ch2_CDAD;
					HashCDAD.Add("1265", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №1")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Pressure_ch2_CDAD;
					HashCDAD.Add("1269", tempCdad!);
					#endregion
					#region CDAD СНТ-2 №2
					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №2")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch1_CDAD;
					HashCDAD.Add("2116", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №2")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowVolume_ch1_CDAD;
					HashCDAD.Add("2222", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №2")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowMass_ch1_CDAD;
					HashCDAD.Add("2225", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №2")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Pressure_ch1_CDAD;
					HashCDAD.Add("2229", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №2")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch2_CDAD;
					HashCDAD.Add("2117", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №2")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowVolume_ch2_CDAD;
					HashCDAD.Add("2262", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №2")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowMass_ch2_CDAD;
					HashCDAD.Add("2265", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №2")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Pressure_ch2_CDAD;
					HashCDAD.Add("2269", tempCdad!);
					#endregion
					#region CDAD СНТ-2 №3
					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №3")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch1_CDAD;
					HashCDAD.Add("3116", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №3")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowVolume_ch1_CDAD;
					HashCDAD.Add("3222", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №3")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowMass_ch1_CDAD;
					HashCDAD.Add("3225", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №3")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Pressure_ch1_CDAD;
					HashCDAD.Add("3229", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №3")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch2_CDAD;
					HashCDAD.Add("3117", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №3")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowVolume_ch2_CDAD;
					HashCDAD.Add("3262", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №3")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowMass_ch2_CDAD;
					HashCDAD.Add("3265", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №3")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Pressure_ch2_CDAD;
					HashCDAD.Add("3269", tempCdad!);
					#endregion
					#region CDAD СНТ-2 №4
					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №4")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch1_CDAD;
					HashCDAD.Add("4116", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №4")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowVolume_ch1_CDAD;
					HashCDAD.Add("4222", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №4")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowMass_ch1_CDAD;
					HashCDAD.Add("4225", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №4")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Pressure_ch1_CDAD;
					HashCDAD.Add("4229", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №4")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch2_CDAD;
					HashCDAD.Add("4117", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №4")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowVolume_ch2_CDAD;
					HashCDAD.Add("4262", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №4")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowMass_ch2_CDAD;
					HashCDAD.Add("4265", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №4")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Pressure_ch2_CDAD;
					HashCDAD.Add("4269", tempCdad!);
					#endregion
					#region CDAD СНТ-2 №5
					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №5")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch1_CDAD;
					HashCDAD.Add("5116", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №5")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowVolume_ch1_CDAD;
					HashCDAD.Add("5222", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №5")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowMass_ch1_CDAD;
					HashCDAD.Add("5225", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №5")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Pressure_ch1_CDAD;
					HashCDAD.Add("5229", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №5")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch2_CDAD;
					HashCDAD.Add("5117", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №5")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowVolume_ch2_CDAD;
					HashCDAD.Add("5262", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №5")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowMass_ch2_CDAD;
					HashCDAD.Add("5265", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №5")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Pressure_ch2_CDAD;
					HashCDAD.Add("5269", tempCdad!);
					#endregion
					#region CDAD СНТ-2 №6
					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №6")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch1_CDAD;
					HashCDAD.Add("6116", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №6")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowVolume_ch1_CDAD;
					HashCDAD.Add("6222", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №6")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowMass_ch1_CDAD;
					HashCDAD.Add("6225", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №6")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Pressure_ch1_CDAD;
					HashCDAD.Add("6229", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №6")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Temperature_ch2_CDAD;
					HashCDAD.Add("6117", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №6")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowVolume_ch2_CDAD;
					HashCDAD.Add("6262", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №6")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.FlowMass_ch2_CDAD;
					HashCDAD.Add("6265", tempCdad!);

					tempCdad = settingsCounter.CountersList!
						.Where(c => c.CounterId == "СНТ-2 №6")
						.Select(p => p.SettingsCounterParameters)
						.First()!
						.Pressure_ch2_CDAD;
					HashCDAD.Add("6269", tempCdad!);
					#endregion
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Этап 1 - Ошибка чтения json-файла.");
			}
		}

		private void GetLastListValueData(List<DateTimePoint> _values)
		{
			var histotyData = _userRepositoriesDB.GetLastHistoryData(HashId);
			_values.Add(new DateTimePoint(histotyData.Item1, Convert.ToDouble(GetValueWithDot(histotyData.Item2, cdadParametr!))));

			var dateFirst = _values.First().DateTime;
			var dateLast = _values.Last().DateTime;
			LastCurrentValue = $":[{_values.Last().Value}]";
			var resultbool = TimeSpan.Compare(dateLast.Subtract(dateFirst), new TimeSpan(00, 60, 00)) > 0;

			if (resultbool)
				_values.RemoveAt(0);

			// we need to update the separators every time we add a new point 
			_customAxis.CustomSeparators = GetSeparators();
		}

	}
}
