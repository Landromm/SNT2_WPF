using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SNT2_WPF.View.Graphs;
/// <summary>
/// Логика взаимодействия для GraphArchiveDataView.xaml
/// </summary>
public partial class GraphArchiveDataView : Window
{
	public GraphArchiveDataView()
	{
		InitializeComponent();
	}

	[DllImport("user32.dll")]
	public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

	private void grdHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		WindowInteropHelper helper = new WindowInteropHelper(this);
		SendMessage(helper.Handle, 161, 2, 0);
	}

	private void btnClose_Click(object sender, RoutedEventArgs e)
	{
		this.Close();
	}

	private void btnMaximize_Click(object sender, RoutedEventArgs e)
	{
		if (this.WindowState == WindowState.Normal)
			this.WindowState = WindowState.Maximized;
		else
			this.WindowState = WindowState.Normal;
	}

	private void btnMinimize_Сlick(object sender, RoutedEventArgs e)
	{
		this.WindowState = WindowState.Minimized;
	}
}
