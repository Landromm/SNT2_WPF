using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNT2_WPF.ViewModel.MainViewModel
{
    public class MainViewModel : ViewModelBase
    {
        //Fields
        private bool _checkActivetedHideMenu=false;

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


        public ICommand ActivateHideMenuCommand { get; }

        public MainViewModel()
        {
            ActivateHideMenuCommand = new ViewModelCommand(ExecuteActivateHideMenuCommand);
        }

        private void ExecuteActivateHideMenuCommand(object obj)
        {
            CheckActivetedHideMenu = !CheckActivetedHideMenu;
        }
    }
}
