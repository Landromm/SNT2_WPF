using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
using SNT2_WPF.Communication.Repositories;
using SNT2_WPF.Services;
using SNT2_WPF.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.ViewModel.Graph;
public class GraphArchiveViewModel : DialogViewModel
{
	private readonly IUserDialog _userDialog = null!;
	private readonly IRepositoriesDB _userRepositoriesDB = null!;
	private static readonly SKColor s_blue = new(25, 118, 210);
	private static readonly SKColor s_red = new(229, 57, 53);
	private static readonly SKColor s_yellow = new(198, 167, 0);

	public GraphArchiveViewModel(IUserDialog UserDialog)
	{
		_userDialog = UserDialog;
		_userRepositoriesDB = new UserRepositoriesDB();
	}

	public ISeries[] Series
	{
		get; set;
	} =
	{
		new LineSeries<double>
		{
			LineSmoothness = 1,
			Name = "Tens",
			Values = new double[] { 14, 13, 14, 15, 17 },
			Stroke = new SolidColorPaint(s_blue, 2),
			GeometrySize = 10,
			GeometryStroke = new SolidColorPaint(s_blue, 2),
			Fill = null,
			ScalesYAt = 0 // it will be scaled at the Axis[0] instance 
        },
		new LineSeries<double>
		{
			Name = "Hundreds",
			Values = new double[] { 533, 586, 425, 579, 518 },
			Stroke = new SolidColorPaint(s_red, 2),
			GeometrySize = 10,
			GeometryStroke = new SolidColorPaint(s_red, 2),
			Fill = null,
			ScalesYAt = 1 // it will be scaled at the YAxes[1] instance 
        },
		new LineSeries<double>
		{
			Name = "Thousands",
			Values = new double[] { 5493, 7843, 4368, 9018, 3902 },
			Stroke = new SolidColorPaint(s_yellow, 2),
			GeometrySize = 10,
			GeometryStroke = new SolidColorPaint(s_yellow, 2),
			Fill = null,
			ScalesYAt = 2  // it will be scaled at the YAxes[2] instance 
        }
	};

	public ICartesianAxis[] YAxes
	{
		get; set;
	} =
	{
		new Axis // the "units" and "tens" series will be scaled on this axis
        {
			Name = "Tens",
			NameTextSize = 14,
			NamePaint = new SolidColorPaint(s_blue),
			NamePadding = new LiveChartsCore.Drawing.Padding(0, 20),
			Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
			TextSize = 12,
			LabelsPaint = new SolidColorPaint(s_blue),
			TicksPaint = new SolidColorPaint(s_blue),
			SubticksPaint = new SolidColorPaint(s_blue),
			DrawTicksPath = true
		},
		new Axis // the "hundreds" series will be scaled on this axis
        {
			Name = "Hundreds",
			NameTextSize = 14,
			NamePaint = new SolidColorPaint(s_red),
			NamePadding = new LiveChartsCore.Drawing.Padding(0, 20),
			Padding =  new LiveChartsCore.Drawing.Padding(20, 0, 0, 0),
			TextSize = 12,
			LabelsPaint = new SolidColorPaint(s_red),
			TicksPaint = new SolidColorPaint(s_red),
			SubticksPaint = new SolidColorPaint(s_red),
			DrawTicksPath = true,
			ShowSeparatorLines =  true,
			Position = LiveChartsCore.Measure.AxisPosition.End
		},
		new Axis // the "thousands" series will be scaled on this axis
        {
			Name = "Thousands",
			NameTextSize = 14,
			NamePadding = new LiveChartsCore.Drawing.Padding(0, 20),
			Padding =  new LiveChartsCore.Drawing.Padding(20, 0, 0, 0),
			NamePaint = new SolidColorPaint(s_yellow),
			TextSize = 12,
			LabelsPaint = new SolidColorPaint(s_yellow),
			TicksPaint = new SolidColorPaint(s_yellow),
			SubticksPaint = new SolidColorPaint(s_yellow),
			DrawTicksPath = true,
			ShowSeparatorLines = false,
			Position = LiveChartsCore.Measure.AxisPosition.End
		}
	};

	public SolidColorPaint LegendTextPaint
	{
		get; set;
	} =
		new SolidColorPaint
		{
			Color = new SKColor(50, 50, 50),
			SKTypeface = SKTypeface.FromFamilyName("Courier New")
		};

	public SolidColorPaint LedgendBackgroundPaint
	{
		get; set;
	} =
		new SolidColorPaint(new SKColor(240, 240, 240));

}
