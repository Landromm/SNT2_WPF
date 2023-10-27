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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SNT2_WPF.ViewModel.Graph
{
    public class GraphCurrentViewModel : DialogViewModel
    {
		private string? _testText;

		private readonly IUserDialog _userDialog = null!;
		private readonly IMessageBus _messageBus = null!;
		private readonly IDisposable _subscription = null!;
		private readonly IRepositoriesDB _userRepositoriesDB = null!;

		private readonly Random _random = new();
		private readonly List<DateTimePoint> _values = new();
		private readonly DateTimeAxis _customAxis;

		public string? TestText
        {
				get => _testText;
				set
				{
					_testText = value;
					OnPropertyChanged(nameof(TestText));
				}
		}

		public ObservableCollection<ISeries>? Series
		{
			get; set;
		}
		public ISeries[] SeriesCollection
		{
			get; set;
		}

		public Axis[] XAxes { get; set; } 
			= new Axis[]
				{
					 new Axis
					 {
						  NamePaint = new SolidColorPaint(SKColors.Black),

						  LabelsPaint = new SolidColorPaint(SKColors.Blue),
						  TextSize = 10,

						  SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 1 }
					 }
				};
		public Axis[] YAxes { get; set; }
			= new Axis[]
			{
					new Axis
					{
						NamePaint = new SolidColorPaint(SKColors.Black),

						LabelsPaint = new SolidColorPaint(SKColors.Blue),
						TextSize = 14,
						MinLimit = 0,
						MaxLimit = 60,
						SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray)
						{
							StrokeThickness = 1,
							PathEffect = new DashEffect(new float[] { 2, 2 })
						}
					}
			};
		public DrawMarginFrame DrawMarginFrame => new()
		{
			Fill = new SolidColorPaint(SKColors.AliceBlue),
			Stroke = new SolidColorPaint(SKColors.Gray, 1)
		};

		public object Sync { get; } = new object();
		public bool IsReading { get; set; } = true;

		public GraphCurrentViewModel(IUserDialog UserDialog, IMessageBus MessageBus)
      {
			_userDialog = UserDialog;
			_messageBus = MessageBus;
			_subscription = MessageBus.RegisterHandler<Message>(OnReceiveMessage);
			_userRepositoriesDB = new UserRepositoriesDB();

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
					GeometrySize = 2
				}
			};

			_customAxis = new DateTimeAxis(TimeSpan.FromSeconds(1), Formatter)
			{
				CustomSeparators = GetSeparators(),
				AnimationsSpeed = TimeSpan.FromMilliseconds(0),
				SeparatorsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(100))
			};

			XAxes = new Axis[] { _customAxis };
			_ = ReadData();
		}


		private async Task ReadData()
		{
			// to keep this sample simple, we run the next infinite loop 
			// in a real application you should stop the loop/task when the view is disposed 

			while (IsReading)
			{
				await Task.Delay(1000);
				var labelDate = new List<string>();

				lock (Sync)
				{
					if (_values.Count != 0)
					{
						var histotyData = _userRepositoriesDB.GetLastHistoryData(TestText);
						_values.Add(new DateTimePoint(histotyData.Item1, histotyData.Item2));

						var dateFirst = _values.First().DateTime;
						var dateLast = _values.Last().DateTime;
						var resultbool = TimeSpan.Compare( dateLast.Subtract(dateFirst), new TimeSpan(00,30,00)) > 0;
						
						if (resultbool)
							_values.RemoveAt(0);

						// we need to update the separators every time we add a new point 
						_customAxis.CustomSeparators = GetSeparators();
					}
					else
					{
						var tempTouple = _userRepositoriesDB.GetHistoryData(TestText);
						
						foreach (var item in tempTouple)
							_values.Add(new DateTimePoint(item.Item1, item.Item2));

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
				now.AddMinutes(-30).Ticks,
				now.AddMinutes(-25).Ticks,
				now.AddMinutes(-20).Ticks,
				now.AddMinutes(-15).Ticks,
				now.AddMinutes(-10).Ticks,
				now.AddMinutes(-5).Ticks,
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
			TestText = message.Text;
			Dispose();
		}

		public void Dispose() => _subscription.Dispose();

	}
}
