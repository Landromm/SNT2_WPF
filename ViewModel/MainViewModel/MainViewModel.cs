﻿using SNT2_WPF.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SNT2_WPF.ViewModel.MainViewModel
{
    public class MainViewModel : ViewModelBase
    {
        //Fields
        private bool _checkActivetedHideMenu=false;
        private ObservableCollection<MainDataModel> _mainData;
        private MainDataModel _selectedMainData;

        //Properties
        public bool CheckActivetedHideMenu
        {
            get => _checkActivetedHideMenu;
            set
            {
                _checkActivetedHideMenu = value;
                OnPropertyChanged(nameof(CheckActivetedHideMenu));
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


        public ICommand ActivateHideMenuCommand { get; }
        public ICommand OpenCurrentP1GraphCommand { get; }
        public ICommand OpenCurrentP2GraphCommand { get; }
        public ICommand OpenCurrentT1GraphCommand { get; }
        public ICommand OpenCurrentT2GraphCommand { get; }
        public ICommand OpenCurrentF1GraphCommand { get; }
        public ICommand OpenCurrentF2GraphCommand { get; }

        public MainViewModel()
        {
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
            ActivateHideMenuCommand = new ViewModelCommand(ExecuteActivateHideMenuCommand);
            OpenCurrentP1GraphCommand = new ViewModelCommand(ExecuteOpenCurrentP1GraphCommand);
            OpenCurrentP2GraphCommand = new ViewModelCommand(ExecuteOpenCurrentP2GraphCommand);
            OpenCurrentT1GraphCommand = new ViewModelCommand(ExecuteOpenCurrentT1GraphCommand);
            OpenCurrentT2GraphCommand = new ViewModelCommand(ExecuteOpenCurrentT2GraphCommand);
            OpenCurrentF1GraphCommand = new ViewModelCommand(ExecuteOpenCurrentF1GraphCommand);
            OpenCurrentF2GraphCommand = new ViewModelCommand(ExecuteOpenCurrentF2GraphCommand);
        }

        private void ExecuteOpenCurrentP1GraphCommand(object obj)
        {
            MessageBox.Show($"Давление: {SelectedMainDataModels.Pressure_ch1}");
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
    }
}
