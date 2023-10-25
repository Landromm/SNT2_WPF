using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;
using SNT2_WPF.Models.DataModel;
using SNT2_WPF.Services;
using SNT2_WPF.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SNT2_WPF.ViewModel.Graph
{
    public class GraphCurrentViewModel : DialogViewModel
    {
		private string? _testText;

		private readonly IUserDialog _userDialog = null!;
		private readonly IMessageBus _messageBus = null!;
		private readonly IDisposable _subscription = null!;

		private readonly Random _random = new();
		private readonly List<DateTimePoint> _values = new();
		private readonly DateTimeAxis? _customAxis;

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
		public Axis[]? XAxes { get; set; } 
			= new Axis[]
				{
					 new Axis
					 {
						  NamePaint = new SolidColorPaint(SKColors.Black),

						  LabelsPaint = new SolidColorPaint(SKColors.Blue),
						  TextSize = 14,

						  SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 1 }
					 }
				};
		public Axis[] YAxes
		{
			get; set;
		}
				= new Axis[]
				{
					 new Axis
					 {
						  NamePaint = new SolidColorPaint(SKColors.Black),

						  LabelsPaint = new SolidColorPaint(SKColors.Blue),
						  TextSize = 14,

						  SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray)
						  {
								StrokeThickness = 1,
								PathEffect = new DashEffect(new float[] { 2, 2 })
						  }
					 }
				};
		public DrawMarginFrame DrawMarginFrame => new DrawMarginFrame
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
					GeometrySize = 5
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

			//var values = new int[100];
			//var r = new Random();
			//var t = 0;

			//for (var i = 0; i < 100; i++)
			//{
			//	t += r.Next(-10, 10);
			//	values[i] = t;
			//}

			//SeriesCollection = new ISeries[] 
			//{ 
			//	new LineSeries<int> 
			//	{ 
			//		Values = values,
			//		Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },
			//		Fill = null,
			//		GeometryFill = new SolidColorPaint(SKColors.AliceBlue),
			//		GeometryStroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
			//		LineSmoothness = 0,
			//		GeometrySize = 5
			//	} 
			//};
		}

		private async Task ReadData()
		{
			// to keep this sample simple, we run the next infinite loop 
			// in a real application you should stop the loop/task when the view is disposed 

			while (IsReading)
			{
				await Task.Delay(1000);

				// Because we are updating the chart from a different thread 
				// we need to use a lock to access the chart data. 
				// this is not necessary if your changes are made in the UI thread. 
				lock (Sync)
				{
					_values.Add(new DateTimePoint(DateTime.Now, _random.Next(40, 55)));
					if (_values.Count > 60)
						_values.RemoveAt(0);

					// we need to update the separators every time we add a new point 
					_customAxis.CustomSeparators = GetSeparators();
				}
			}
		}

		private double[] GetSeparators()
		{
			var now = DateTime.Now;

			return new double[]
			{
				now.AddSeconds(-60).Ticks,
				now.AddSeconds(-50).Ticks,
				now.AddSeconds(-40).Ticks,
				now.AddSeconds(-30).Ticks,
				now.AddSeconds(-20).Ticks,
				now.AddSeconds(-10).Ticks,
				now.Ticks
			};
		}

		private static string Formatter(DateTime date)
		{
			var secsAgo = (DateTime.Now - date).TotalSeconds;

			return secsAgo < 1
				 ? "now"
				 : $"{secsAgo:N0}s ago";
		}

		private void OnReceiveMessage(Message message)
		{
			TestText = message.Text;
			Dispose();
		}

		public void Dispose() => _subscription.Dispose();

	}
}
