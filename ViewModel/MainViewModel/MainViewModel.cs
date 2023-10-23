using SNT2_WPF.Communication.IniData;
using SNT2_WPF.Models.DataModel;
using SNT2_WPF.Models.GenerationModel;
using SNT2_WPF.View.Graphs;
using SNT2_WPF.ViewModel.Graph;
using SNT2_WPF.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SNT2_WPF.Services;
using SNT2_WPF.ViewModel.Commands;

namespace SNT2_WPF.ViewModel.MainViewModel
{
    public class MainViewModel : DialogViewModel
    {
      IniFile INI = new IniFile(@"Resources\Config.ini");
        GenerationData generationData = null!;

        //Fields
        private bool _checkActivetedHideMenu = false;
        private bool _isCheckedStyleMode;
        private string pathDefaultStyle = "";
        private string pathDarkStyle = "";
        private ObservableCollection<MainDataModel> _mainData = null!;
        private MainDataModel _selectedMainData = null!;
        private ViewModelBase _currentChildView = null!;

		  private readonly IUserDialog _userDialog = null!;
		  private readonly IMessageBus _messageBus = null!;
		  private readonly IDisposable _subscription = null!;

		//Properties
		//Состояние открытия скрытого меню.
		public bool CheckActivetedHideMenu
        {
            get => _checkActivetedHideMenu;
            set
            {
                _checkActivetedHideMenu = value;
                OnPropertyChanged(nameof(CheckActivetedHideMenu));
            }
        }
        //Состояние выбора темы приложения 
        public bool IsCheckedStyleMode
        {
            get => _isCheckedStyleMode;
            set
            {
                _isCheckedStyleMode = value;
                OnPropertyChanged(nameof(IsCheckedStyleMode));
            }
        }
        public ObservableCollection<MainDataModel> MainDataModels
        {
            get => _mainData ?? (_mainData = new ObservableCollection<MainDataModel>());
            set
            {
                _mainData = value;
                OnPropertyChanged(nameof(MainDataModels));
            }
        }
        public MainDataModel SelectedMainDataModels
        {
            get => _selectedMainData;
            set
            {
                _selectedMainData = value;
                OnPropertyChanged(nameof(SelectedMainDataModels));
            }
        }
        public ViewModelBase CurrentChildView
        {
            get => _currentChildView;
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }

        //public ICommand ActivateHideMenuCommand { get; }    //Открытие\закрытие скрытого меню.
        //public ICommand SelectionModeStyleCommand { get; } //Выбор темы приложения (светлая или темная).
        //public ICommand OpenCurrentP1GraphCommand { get; }  //Открытие графика давления №1
        //public ICommand OpenCurrentP2GraphCommand { get; }  //Открытие графика давления №2
        //public ICommand OpenCurrentT1GraphCommand { get; }  //Открытие графика температуры №1
        //public ICommand OpenCurrentT2GraphCommand { get; }  //Открытие графика температуры №2
        //public ICommand OpenCurrentF1GraphCommand { get; }  //Открытие графика расхода №1
        //public ICommand OpenCurrentF2GraphCommand { get; }  //Открытие графика расхода №2

        public MainViewModel(IUserDialog UserDialog, IMessageBus MessageBus)
        {
            InitializationStyleDefectLog();
            MainDataModels = new ObservableCollection<MainDataModel>() 
            {
               new MainDataModel()
               {
                   CheckErrorConection = true,
                   NumberCounter = "1234",
                   DescriptionCounter = "Счетчик №1",
                   Pressure_ch1 = "14,0",
                   Temperature_ch1 = "65,8",
                   Flow_ch1 = "1012,4",
                   Pressure_ch2 = "22,0",
                   Temperature_ch2 = "55,8",
                   Flow_ch2 = "1312,4"
               },
                new MainDataModel()
               {
                   CheckErrorConection = true,
                   NumberCounter = "2345",
                   DescriptionCounter = "Счетчик №2",
                   Pressure_ch1 = "15,0",
                   Temperature_ch1 = "55,8",
                   Flow_ch1 = "1222,4",
                   Pressure_ch2 = "42,0",
                   Temperature_ch2 = "55,8",
                   Flow_ch2 = "1332,43"
               },
                new MainDataModel()
               {
                   CheckErrorConection = false,
                   NumberCounter = "3456",
                   DescriptionCounter = "Счетчик №3",
                   Pressure_ch1 = "14,0",
                   Temperature_ch1 = "65,8",
                   Flow_ch1 = "1012,4",
                   Pressure_ch2 = "22,0",
                   Temperature_ch2 = "55,8",
                   Flow_ch2 = "1312,4"
               },
                new MainDataModel()
               {
                   CheckErrorConection = false,
                   NumberCounter = "4567",
                   DescriptionCounter = "Счетчик №4",
                   Pressure_ch1 = "54,0",
                   Temperature_ch1 = "55,8",
                   Flow_ch1 = "2012,4",
                   Pressure_ch2 = "72,0",
                   Temperature_ch2 = "33,8",
                   Flow_ch2 = "2312,4"
               }
            };

			   _userDialog = UserDialog;
			   _messageBus = MessageBus;

			//ActivateHideMenuCommand = new ViewModelCommand(ExecuteActivateHideMenuCommand);
			//SelectionModeStyleCommand = new ViewModelCommand(ExecuteSelectionModeStyleCommand);
			//OpenCurrentP1GraphCommand = new ViewModelCommand(ExecuteOpenCurrentP1GraphCommand);
			//OpenCurrentP2GraphCommand = new ViewModelCommand(ExecuteOpenCurrentP2GraphCommand);
			//OpenCurrentT1GraphCommand = new ViewModelCommand(ExecuteOpenCurrentT1GraphCommand);
			//OpenCurrentT2GraphCommand = new ViewModelCommand(ExecuteOpenCurrentT2GraphCommand);
			//OpenCurrentF1GraphCommand = new ViewModelCommand(ExecuteOpenCurrentF1GraphCommand);
			//OpenCurrentF2GraphCommand = new ViewModelCommand(ExecuteOpenCurrentF2GraphCommand);

			   generationData = new GenerationData();
            RunGenerationThread();
        }

		public void Dispose() => _subscription.Dispose();

		#region Command SendMessageCommand - Отправка сообщения

		/// <summary>Отправка сообщения</summary>
		private LambdaCommand? _OpenGraphCommand;

		/// <summary>Отправка сообщения</summary>
		public ICommand OpenCurrentP1GraphCommand => _OpenGraphCommand ??= new((Action)ExecuteOpenCurrentP1GraphCommand);

		/// <summary>Логика выполнения - Отправка сообщения</summary>
		private void ExecuteOpenCurrentP1GraphCommand()
		{
			_userDialog.OpenCurrentGrapf();
			_messageBus.Send(new Message(SelectedMainDataModels.NumberCounter));
		}

		#endregion

		//Метод установления стиля в зависимости от принятых параметров.

		private void ChangeStyleThemes(string pathStyle, bool IsDefaultStyle)
        {
            var uri = new Uri(@pathStyle, UriKind.Relative);
            ResourceDictionary? resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);

            INI.WriteINI("StyleThemeSNT2IsChecked", "pathStyleIsChecked", pathStyle);
            INI.WriteINI("StyleThemeSNT2IsChecked", "boolSelected_DefaultStyle", $"{IsDefaultStyle}");
            INI.WriteINI("StyleThemeSNT2IsChecked", "boolSelected_DarkStyle", $"{!IsDefaultStyle}");

        }

        private void RunGenerationThread()
        {
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    generationData.GenerationDataStart();
                    Thread.Sleep(1000);
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        //Выполнения метода смены стиля приложения.
        private void ExecuteSelectionModeStyleCommand(object obj)
        {
            IsCheckedStyleMode = !IsCheckedStyleMode;
            CheckActivetedHideMenu = false;
            pathDefaultStyle = INI.ReadINI("StyleThemeSNT2", "pathDefaultStyle");
            pathDarkStyle = INI.ReadINI("StyleThemeSNT2", "pathDarkStyle");

            if (!IsCheckedStyleMode)
                ChangeStyleThemes(pathDarkStyle, false);
            else
                ChangeStyleThemes(pathDefaultStyle, true);
        }

        private void ExecuteOpenCurrentP1GraphCommand(object obj)
        {
            GraphCurrentDataView graphCurrentData = new GraphCurrentDataView();
            graphCurrentData.Show();
        }
        private void ExecuteOpenCurrentP2GraphCommand(object obj)
        {
            MessageBox.Show($"Давление: {SelectedMainDataModels.Pressure_ch2}");
        }
        private void ExecuteOpenCurrentT1GraphCommand(object obj)
        {
            MessageBox.Show($"Давление: {SelectedMainDataModels.Temperature_ch1}");
        }
        private void ExecuteOpenCurrentT2GraphCommand(object obj)
        {
            MessageBox.Show($"Давление: {SelectedMainDataModels.Temperature_ch2}");
        }
        private void ExecuteOpenCurrentF1GraphCommand(object obj)
        {
            MessageBox.Show($"Давление: {SelectedMainDataModels.Flow_ch1}");
        }
        private void ExecuteOpenCurrentF2GraphCommand(object obj)
        {
            MessageBox.Show($"Давление: {SelectedMainDataModels.Flow_ch2}");
        }

        private void ExecuteActivateHideMenuCommand(object obj)
        {
            CheckActivetedHideMenu = !CheckActivetedHideMenu;            
        }

        // Метод инициализации стиля приложения
        private void InitializationStyleDefectLog()
        {
            string isCheckedStyleApplication = INI.ReadINI("StyleThemeSNT2IsChecked", "pathStyleIsChecked");
            //string isCheckedDefaultStyle = INI.ReadINI("StyleThemeSNT2IsChecked", "boolSelected_DefaultStyle");

            bool isCheckedDefaultStyle = Convert.ToBoolean(INI.ReadINI("StyleThemeSNT2IsChecked", "boolSelected_DefaultStyle"));

            if (isCheckedDefaultStyle)
                IsCheckedStyleMode = true;
            else
                IsCheckedStyleMode = false;

            var uri = new Uri(@isCheckedStyleApplication, UriKind.Relative);
            ResourceDictionary? resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
    }
}
