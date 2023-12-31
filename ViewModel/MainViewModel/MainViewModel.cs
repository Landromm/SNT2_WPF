﻿using SNT2_WPF.Models.DataModel;
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
using SNT2_WPF.Communication.Repositories;
using LiveChartsCore.Defaults;
using System.ComponentModel.DataAnnotations;
using SNT2_WPF.Communication.ComPort;
using SNT2_WPF.Models.DataModel.DataControl;
using SNT2_WPF.Models.DataSettingsTeg;
using System.IO;
using System.Text.Json;

namespace SNT2_WPF.ViewModel.MainViewModel
{
    public class MainViewModel : DialogViewModel
    {
        //IniFile INI = new IniFile(@"Resources\Config.ini");
        GenerationData generationData = null!;

        //Fields
        private bool _checkActivetedHideMenu = false;
        private bool _isCheckedStyleMode;
        private string pathDefaultStyle = "";
        private string pathDarkStyle = "";
        private ObservableCollection<MainDataModel> _mainData = null!;
        private MainDataModel? _selectedMainData = null!;
		private CDADModel? _cdadModel = null!;
		private Counters? settingsCounter = null!;
		private int[,] arrayCounters = null!;
		private string _countErrorReader;

		private readonly IUserDialog _userDialog = null!;
		private readonly IMessageBus _messageBus = null!;
		private readonly IDisposable _subscription = null!;
        private readonly IRepositoriesDB _userRepositoriesDB = null!;
        private readonly IRepositoriesLocal _userRepositoriesLocal = null!;
		private readonly IReadCounters _readCounters = null!;

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
        //Состояние выбора темы приложения.
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
        public MainDataModel? SelectedMainDataModels
        {
            get => _selectedMainData;
            set
            {
                _selectedMainData = value;
                OnPropertyChanged(nameof(SelectedMainDataModels));
            }
        }
		public CDADModel? CdadModel
		{
			get => _cdadModel;
			set => _cdadModel = value;
		}

		public object Sync { get; } = new object();
		public bool IsReading { get; set; } = true;

		public MainViewModel(IUserDialog UserDialog, IMessageBus MessageBus)
        {
            InitializationStyleSNT2();

			_userDialog = UserDialog;
			_messageBus = MessageBus;
            _userRepositoriesDB = new UserRepositoriesDB();
            _userRepositoriesLocal = new UserRepositoriesLocal();
			_cdadModel = new();

			MainDataModels = new ObservableCollection<MainDataModel>();
            InitializeCounters();

			generationData = new GenerationData();
			_readCounters = new ReadCounters();

			StartReadCounters();
			//RunGenerationThread();

			Thread.Sleep(2000);
			_ = ReadData();
		}

        private void InitializeCounters()
        {
            var listCounters = _userRepositoriesLocal.InitializeCounters();
            arrayCounters = new int[listCounters.Count(),7];

            for (int i = 0; i < arrayCounters.GetUpperBound(0) + 1; i++)
            {
				arrayCounters[i,0] = 1112 + (i * 1000);
				arrayCounters[i,1] = 1229 + (i * 1000);
				arrayCounters[i,2] = 1116 + (i * 1000);
				arrayCounters[i,3] = 1225 + (i * 1000);
				arrayCounters[i,4] = 1269 + (i * 1000);
				arrayCounters[i,5] = 1117 + (i * 1000);
				arrayCounters[i,6] = 1265 + (i * 1000);
			}
		}
		private void StartReadCounters()
		{
			Thread thread = new Thread(() =>
			{
				while (true)
				{
					_readCounters.StartReadCounters();
				}
			});
			thread.IsBackground = true;
			thread.Start();
		}

        private async Task ReadData()
		{
            // to keep this sample simple, we run the next infinite loop 
            // in a real application you should stop the loop/task when the view is disposed 
            var listCounters = _userRepositoriesLocal.InitializeCounters();

            if(listCounters != null)
            {
				InitializeDataValues(listCounters);
				while (IsReading)
				{
					await Task.Delay(3000);
					// Because we are updating the chart from a different thread 
					// we need to use a lock to access the chart data. 
					// this is not necessary if your changes are made in the UI thread. 
					lock (Sync)
					{
						GetAllDataValues(listCounters);
					}
				}
			}
		}

		private void RunGenerationThread()
		{
			Thread thread = new Thread(() =>
			{
				while (true)
				{
					generationData.GenerationDataStart();
					Thread.Sleep(5000);
				}
			});
			thread.IsBackground = true;
			thread.Start();
		}

		private void InitializeDataValues(List<string> listCounters)
		{
			for (int i = 0; i < listCounters.Count; i++)
			{
				var counterData = new MainDataModel();
				string? counterId = _userRepositoriesDB.GetCounterId(listCounters[i]);

				InitializationCDADValue(counterId);

				counterData.NumberCounter = listCounters[i];
				counterData.DescriptionCounter = _userRepositoriesDB.GetDescriptionCounter(counterId);
				counterData.CheckErrorConection = _userRepositoriesDB.GetStatusChanel(arrayCounters[i, 0]);
				counterData.HashCheckErrorConection = arrayCounters[i,0].ToString();

				counterData.Pressure_ch1 = GetValueWithDot(
					_userRepositoriesDB.GetValueChanel(arrayCounters[i, 1]),
					CdadModel!.Pressure_ch1_CDAD);
                counterData.HashPressure_ch1 = arrayCounters[i,1].ToString();

                counterData.Temperature_ch1 = GetValueWithDot(
					_userRepositoriesDB.GetValueChanel(arrayCounters[i, 2]),
					CdadModel!.Temperature_ch1_CDAD);
                counterData.HashTemperature_ch1 = arrayCounters[i,2].ToString();

				counterData.Flow_ch1 = GetValueWithDot(
					_userRepositoriesDB.GetValueChanel(arrayCounters[i, 3]),
					CdadModel!.FlowMass_ch1_CDAD);
				counterData.HashFlow_ch1 = arrayCounters[i, 3].ToString();

				counterData.Pressure_ch2 = GetValueWithDot(
					_userRepositoriesDB.GetValueChanel(arrayCounters[i, 4]),
					CdadModel!.Pressure_ch2_CDAD);
                counterData.HashPressure_ch2 = arrayCounters[i,4].ToString();

				counterData.Temperature_ch2 = GetValueWithDot(
					_userRepositoriesDB.GetValueChanel(arrayCounters[i, 5]),
					CdadModel!.Temperature_ch2_CDAD);
                counterData.HashTemperature_ch2 = arrayCounters[i,5].ToString();

				counterData.Flow_ch2 = GetValueWithDot(
					_userRepositoriesDB.GetValueChanel(arrayCounters[i, 6]),
					CdadModel!.FlowMass_ch2_CDAD);
				counterData.HashFlow_ch2 = arrayCounters[i, 6].ToString();

				MainDataModels.Add(counterData);
			}
		}

		private void GetAllDataValues(List<string> listCounters)
        {
            for (int i = 0; i < listCounters.Count; i++)
            {
				var counterData = new MainDataModel();
				string? counterId = _userRepositoriesDB.GetCounterId(listCounters[i]);

				InitializationCDADValue(counterId);

				counterData.NumberCounter = listCounters[i];
				counterData.DescriptionCounter = _userRepositoriesDB.GetDescriptionCounter(counterId);
				counterData.CheckErrorConection = _userRepositoriesDB.GetStatusChanel(arrayCounters[i, 0]);
				counterData.HashCheckErrorConection = arrayCounters[i, 0].ToString();

				counterData.Pressure_ch1 = GetValueWithDot(
					_userRepositoriesDB.GetValueChanel(arrayCounters[i, 1]),
					CdadModel!.Pressure_ch1_CDAD);
				counterData.HashPressure_ch1 = arrayCounters[i, 1].ToString();

				counterData.Temperature_ch1 = GetValueWithDot(
					_userRepositoriesDB.GetValueChanel(arrayCounters[i, 2]),
					CdadModel!.Temperature_ch1_CDAD);
				counterData.HashTemperature_ch1 = arrayCounters[i, 2].ToString();

				counterData.Flow_ch1 = GetValueWithDot(
					_userRepositoriesDB.GetValueChanel(arrayCounters[i, 3]),
					CdadModel!.FlowMass_ch1_CDAD);
				counterData.HashFlow_ch1 = arrayCounters[i, 3].ToString();

				counterData.Pressure_ch2 = GetValueWithDot(
					_userRepositoriesDB.GetValueChanel(arrayCounters[i, 4]),
					CdadModel!.Pressure_ch2_CDAD);
				counterData.HashPressure_ch2 = arrayCounters[i, 4].ToString();

				counterData.Temperature_ch2 = GetValueWithDot(
					_userRepositoriesDB.GetValueChanel(arrayCounters[i, 5]),
					CdadModel!.Temperature_ch2_CDAD);
				counterData.HashTemperature_ch2 = arrayCounters[i, 5].ToString();

				counterData.Flow_ch2 = GetValueWithDot(
					_userRepositoriesDB.GetValueChanel(arrayCounters[i, 6]),
					CdadModel!.FlowMass_ch2_CDAD);
				counterData.HashFlow_ch2 = arrayCounters[i, 6].ToString();

				MainDataModels[i] = counterData;
			}
		}

		public void Dispose() => _subscription.Dispose();

		#region Command's - Реализования комманд.
		#region Command ActivateHideMenuCommand - Открытие скрытого меню

		/// <summary>Открытие скрытого меню</summary>
		private LambdaCommand? _ActivateHideMenuCommand;

		/// <summary>Открытие скрытого меню</summary>
		public ICommand ActivateHideMenuCommand => _ActivateHideMenuCommand ??= new(ExecutedActivateHideMenuCommand);

		/// <summary>Логика выполнения - Открытие скрытого меню</summary>
		private void ExecutedActivateHideMenuCommand()
		{
			CheckActivetedHideMenu = !CheckActivetedHideMenu;
		}

        #endregion

        #region Command SelectionModeStyleCommand - Выбор стиля приложения: темная или светлая тема.

        /// <summary>Выбор стиля приложения: темная или светлая тема.</summary>
        private LambdaCommand? _SelectionModeStyleCommand;

        /// <summary>Выбор стиля приложения: темная или светлая тема.</summary>
        public ICommand SelectionModeStyleCommand => _SelectionModeStyleCommand ??= new(ExecutedSelectionModeStyleCommand);

        /// <summary>Логика выполнения - Выбор стиля приложения: темная или светлая тема.</summary>
        private void ExecutedSelectionModeStyleCommand()
        {

			IsCheckedStyleMode = !IsCheckedStyleMode;
			CheckActivetedHideMenu = false;
			//pathDefaultStyle = INI.ReadINI("StyleThemeSNT2", "pathDefaultStyle");
			//pathDarkStyle = INI.ReadINI("StyleThemeSNT2", "pathDarkStyle");

			if (!IsCheckedStyleMode)
				ChangeStyleThemes(Properties.Settings.Default.PathDarkStyle, false);
			else
				ChangeStyleThemes(Properties.Settings.Default.PathDefaultStyle, true);
		}
		#endregion

		#region Command OpenCurrentP1GraphCommand - Открытие текущего графика по давлению №1 (P1)

		/// <summary>Открытие текущего графика по давлению №1 (P1)</summary>
		private LambdaCommand? _OpenCurrentP1Graph;

		/// <summary>Открытие текущего графика по давлению №1 (P1)</summary>
		public ICommand OpenCurrentP1GraphCommand => _OpenCurrentP1Graph ??= new(ExecutedCurrentP1GraphCommand);

		/// <summary>Логика выполнения - Открытие графика по давлению №1 (P1)</summary>
		private void ExecutedCurrentP1GraphCommand()
		{
			if (SelectedMainDataModels != null)
			{
				_userDialog.OpenCurrentGrapf();
				_messageBus.Send(new Message(SelectedMainDataModels.HashPressure_ch1, SelectedMainDataModels.DescriptionCounter, "Давление P1."));
			}
		}

        #endregion

        #region Command OpenCurrentT1GraphCommand - Открытие текущего графика по температуре №1 (T1)

        /// <summary>Открытие текущего графика по температуре №1 (T1)</summary>
        private LambdaCommand? _OpenCurrentT1GraphCommand;

        /// <summary>Открытие текущего графика по температуре №1 (T1)</summary>
        public ICommand OpenCurrentT1GraphCommand => _OpenCurrentT1GraphCommand ??= new(ExecutedOpenCurrentT1GraphCommand);

        /// <summary>Логика выполнения - Открытие текущего графика по температуре №1 (T1)</summary>
        private void ExecutedOpenCurrentT1GraphCommand()
        {
			if (SelectedMainDataModels != null)
			{
				_userDialog.OpenCurrentGrapf();
				_messageBus.Send(new Message(SelectedMainDataModels.HashTemperature_ch1, SelectedMainDataModels.DescriptionCounter, "Температура T1"));
			}
		}
		#endregion

		#region Command OpenCurrentF1GraphCommand - Открытие текущего графика по расходу №1 (F1)

		/// <summary>Открытие текущего графика по расходу №1 (F1)</summary>
		private LambdaCommand? _OpenCurrentF1GraphCommand;

        /// <summary>Открытие текущего графика по расходу №1 (F1)</summary>
        public ICommand OpenCurrentF1GraphCommand => _OpenCurrentF1GraphCommand ??= new(ExecutedOpenCurrentF1GraphCommand);

        /// <summary>Логика выполнения - Открытие текущего графика по расходу №1 (F1)</summary>
        private void ExecutedOpenCurrentF1GraphCommand()
        {
			if (SelectedMainDataModels != null)
			{
				_userDialog.OpenCurrentGrapf();
				_messageBus.Send(new Message(SelectedMainDataModels.HashFlow_ch1, SelectedMainDataModels.DescriptionCounter, "Расход F1"));
			}
		}
		#endregion

		#region Command OpenCurrentP2GraphCommand - Открытие текущего графика по давлению №2 (P2)

		/// <summary>Открытие текущего графика по давлению №2 (P2)</summary>
		private LambdaCommand? _OpenCurrentP2GraphCommand;

        /// <summary>Открытие текущего графика по давлению №2 (P2)</summary>
        public ICommand OpenCurrentP2GraphCommand => _OpenCurrentP2GraphCommand ??= new(ExecutedOpenCurrentP2GraphCommand);

        /// <summary>Логика выполнения - Открытие текущего графика по давлению №2 (P2)</summary>
        private void ExecutedOpenCurrentP2GraphCommand()
        {
			if (SelectedMainDataModels != null)
			{
				_userDialog.OpenCurrentGrapf();
				_messageBus.Send(new Message(SelectedMainDataModels.HashPressure_ch2, SelectedMainDataModels.DescriptionCounter, "Давление P2"));
			}
		}
		#endregion

		#region Command OpenCurrentT2GraphCommand - Открытие текущего графика по температуре №2 (T2)

		/// <summary>Открытие текущего графика по температуре №2 (T2)</summary>
		private LambdaCommand? _OpenCurrentT2GraphCommand;

        /// <summary>Открытие текущего графика по температуре №2 (T2)</summary>
        public ICommand OpenCurrentT2GraphCommand => _OpenCurrentT2GraphCommand ??= new(ExecutedOpenCurrentT2GraphCommand);

        /// <summary>Логика выполнения - Открытие текущего графика по температуре №2 (T2)</summary>
        private void ExecutedOpenCurrentT2GraphCommand()
        {
			if (SelectedMainDataModels != null)
			{
				_userDialog.OpenCurrentGrapf();
				_messageBus.Send(new Message(SelectedMainDataModels.HashTemperature_ch2, SelectedMainDataModels.DescriptionCounter, "Температура T2"));
			}
		}
		#endregion

		#region Command OpenCurrentF2GraphCommand - Открытие текущего графика по расходу №2 (F2)

		/// <summary>Открытие текущего графика по расходу №2 (F2)</summary>
		private LambdaCommand? _OpenCurrentF2GraphCommand;

        /// <summary>Открытие текущего графика по расходу №2 (F2)</summary>
        public ICommand OpenCurrentF2GraphCommand => _OpenCurrentF2GraphCommand ??= new(ExecutedOpenCurrentF2GraphCommand);

        /// <summary>Логика выполнения - Открытие текущего графика по расходу №2 (F2)</summary>
        private void ExecutedOpenCurrentF2GraphCommand()
        {
			if (SelectedMainDataModels != null)
			{
				_userDialog.OpenCurrentGrapf();
				_messageBus.Send(new Message(SelectedMainDataModels.HashFlow_ch2, SelectedMainDataModels.DescriptionCounter, "Расход F2"));
			}
		}
		#endregion

		#region Command OpenArchiveGraphsCommand - Открытие окна графиков архивных данных

		/// <summary>Открытие окна графиков архивных данных</summary>
		private LambdaCommand? _OpenArchiveGraphsCommand;

		/// <summary>Открытие окна графиков архивных данных</summary>
		public ICommand OpenArchiveGraphsCommand => _OpenArchiveGraphsCommand ??= new(ExecutedOpenArchiveGraphsCommand);

		/// <summary>Логика выполнения - Открытие окна графиков архивных данных</summary>
		private void ExecutedOpenArchiveGraphsCommand()
		{
			_userDialog.OpenArchiveGraph();
		}
		#endregion

		#region Command OpenSettingsCommand - открытие окна настроек приложения.

		/// <summary>открытие окна настроек приложения.</summary>
		private LambdaCommand? _OpenSettingsCommand;

		/// <summary>открытие окна настроек приложения.</summary>
		public ICommand OpenSettingsCommand => _OpenSettingsCommand ??= new(ExecutedOpenSettingsCommand);

		/// <summary>Логика выполнения - открытие окна настроек приложения.</summary>
		private void ExecutedOpenSettingsCommand()
		{
				Application.Current.Dispatcher.Invoke(() =>
				{
					_userDialog.OpenSettingsWindow();
				});
			
		}
		#endregion
		#endregion 

		//Метод установления стиля в зависимости от принятых параметров.
		private void ChangeStyleThemes(string pathStyle, bool IsDefaultStyle)
        {
            var uri = new Uri(@pathStyle, UriKind.Relative);
            ResourceDictionary? resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);

			Properties.Settings.Default.pathStyleIsChecked = pathStyle;
			Properties.Settings.Default.boolSelected_DefaultStyle = IsDefaultStyle;
			Properties.Settings.Default.boolSelected_DarkStyle = !IsDefaultStyle;
			Properties.Settings.Default.SelectedDefaultStyle = IsDefaultStyle;
			Properties.Settings.Default.Save();
        }        

        // Метод инициализации стиля приложения
        private void InitializationStyleSNT2()
        {
            string isCheckedStyleApplication = Properties.Settings.Default.pathStyleIsChecked;
			bool isCheckedDefaultStyle = Properties.Settings.Default.SelectedDefaultStyle;

			if (isCheckedDefaultStyle)
                IsCheckedStyleMode = true;
            else
                IsCheckedStyleMode = false;

            var uri = new Uri(@isCheckedStyleApplication, UriKind.Relative);
            ResourceDictionary? resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
		}

		private void InitializationCDADValue(string? CounterId)
		{
			try
			{
				using FileStream fs = new FileStream(@"Resources\\db_List_SettingsTeg.json", FileMode.OpenOrCreate);
				settingsCounter = JsonSerializer.Deserialize<Counters>(fs)!;

				if (settingsCounter is not null)
				{
					CdadModel = new()
					{
						Temperature_ch1_CDAD = Int32.Parse(
						settingsCounter.CountersList!
							.Where(c => c.CounterId == CounterId)
							.Select(p => p.SettingsCounterParameters)
							.First()!
							.Temperature_ch1_CDAD!),

						FlowVolume_ch1_CDAD = Convert.ToInt32(
						settingsCounter.CountersList!
							.Where(c => c.CounterId == CounterId)
							.Select(p => p.SettingsCounterParameters)
							.First()!
							.FlowVolume_ch1_CDAD),

						FlowMass_ch1_CDAD = Convert.ToInt32(
						settingsCounter.CountersList!
							.Where(c => c.CounterId == CounterId)
							.Select(p => p.SettingsCounterParameters)
							.First()!
							.FlowMass_ch1_CDAD),

						Pressure_ch1_CDAD = Convert.ToInt32(
						settingsCounter.CountersList!
							.Where(c => c.CounterId == CounterId)
							.Select(p => p.SettingsCounterParameters)
							.First()!
							.Pressure_ch1_CDAD),

						Temperature_ch2_CDAD = Convert.ToInt32(
						settingsCounter.CountersList!
							.Where(c => c.CounterId == CounterId)
							.Select(p => p.SettingsCounterParameters)
							.First()!
							.Temperature_ch2_CDAD),

						FlowVolume_ch2_CDAD = Convert.ToInt32(
						settingsCounter.CountersList!
							.Where(c => c.CounterId == CounterId)
							.Select(p => p.SettingsCounterParameters)
							.First()!
							.FlowVolume_ch2_CDAD),

						FlowMass_ch2_CDAD = Convert.ToInt32(
						settingsCounter.CountersList!
							.Where(c => c.CounterId == CounterId)
							.Select(p => p.SettingsCounterParameters)
							.First()!
							.FlowMass_ch2_CDAD),

						Pressure_ch2_CDAD = Convert.ToInt32(
						settingsCounter.CountersList!
							.Where(c => c.CounterId == CounterId)
							.Select(p => p.SettingsCounterParameters)
							.First()!
							.Pressure_ch2_CDAD)
					};
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Этап 1 - Ошибка чтения json-файла.");
			}
		}

		private string GetValueWithDot(string? value, int cdad)
		{
			var cdadInt = Convert.ToInt32(cdad);
			if (value is not null)
			{
				if (cdadInt == 0)
					return Convert.ToDouble(value).ToString();
					
				return Convert.ToDouble(value.Insert(value.Length - cdadInt, ",")).ToString();
			}
			return value = "";
		}
    }
}
