using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;
using SNT2_WPF.Communication.Repositories;
using SNT2_WPF.Models.DataModel;
using SNT2_WPF.Models.DBModel;
using SNT2_WPF.Services;
using SNT2_WPF.ViewModel.Base;
using SNT2_WPF.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
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
		private string? _lastCurrentValue = "0";

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
					if (_values.Count != 0)
					{
						var histotyData = _userRepositoriesDB.GetLastHistoryData(HashId);
						_values.Add(new DateTimePoint(histotyData.Item1, Convert.ToDouble(histotyData.Item2)));

						var dateFirst = _values.First().DateTime;
						var dateLast = _values.Last().DateTime;
						LastCurrentValue = $":[{_values.Last().Value}]";
						var resultbool = TimeSpan.Compare( dateLast.Subtract(dateFirst), new TimeSpan(00,60,00)) > 0;
						
						if (resultbool)
							_values.RemoveAt(0);

						// we need to update the separators every time we add a new point 
						_customAxis.CustomSeparators = GetSeparators();
					}
					else
					{
						var tempTouple = _userRepositoriesDB.GetHistoryData(HashId);
						
						foreach (var item in tempTouple)
							_values.Add(new DateTimePoint(item.Item1, Convert.ToDouble(item.Item2)));

						LastCurrentValue = $":[{_values.Last().Value}]";
						_customAxis.CustomSeparators = GetSeparators();
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

	}
}
