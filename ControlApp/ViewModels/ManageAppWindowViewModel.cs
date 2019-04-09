using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using ReleaseControlLib;

namespace ControlApp
{
    public class ManageAppWindowViewModel:ViewModelBase
    {
        private bool? _dialogResult;

        public bool? DialogResult
        {
            get { return _dialogResult; }
            protected set
            {
                _dialogResult = value;
                RaisePropertyChanged("DialogResult");
            }
        }

        public ControlledApp CurrentApp
        {
            get { return MainModel.TempApp; }
            set
            {
                MainModel.TempApp = value;
                RaisePropertyChanged("CurrentApp");
            }
        }

        public ICommand ConfirmCommand
        {
            get { return new RelayCommand<object>(ExecuteConfirm); }
        }

        void ExecuteConfirm(object window)
        {
            DialogResult = true;
        }

        public ICommand DiscardCommand
        {
            get { return new RelayCommand<object>(ExecuteDiscard); }
        }

        void ExecuteDiscard(object window)
        {
            DialogResult = false;
        }

        public ICommand OpenFileCommand
        {
            get { return new RelayCommand<object>(ExecuteOpenFile); }
        }

        void ExecuteOpenFile(object param)
        {
            switch(param)
            {
                case "Work":
                    CurrentApp.WorkingReleasePath = MainModel.FindPath();
                    break;
                case "Release":
                    CurrentApp.ReleasePath = MainModel.FindPath();
                    break;
                case "Reestr":
                    CurrentApp.ReestrPath = MainModel.FindPath();
                    break;
            }
        }
    }
}
