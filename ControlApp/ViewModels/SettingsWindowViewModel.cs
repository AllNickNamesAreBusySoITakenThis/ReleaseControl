using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using ReleaseControlLib;

namespace ControlApp
{
    public class SettingsWindowViewModel:ViewModelBase
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

        public string Database
        {
            get { return StorageService.Database; }
            set
            {
                StorageService.Database = value;
                RaisePropertyChanged("Database");
            }
        }

        public StorageTypes StorageType
        {
            get { return StorageService.StorageType; }
            set
            {
                StorageService.StorageType = value;
                RaisePropertyChanged("StorageType");
            }
        }

        public ConnectionTypes ConnectionType
        {
            get { return StorageService.ConnectionType; }
            set
            {
                StorageService.ConnectionType = value;
                RaisePropertyChanged("ConnectionType");
            }
        }

        public string FileName
        {
            get { return StorageService.FilePath; }
            set
            {
                StorageService.FilePath = value;
                RaisePropertyChanged("FileName");
            }
        }

        public string Server
        {
            get { return StorageService.Server; }
            set
            {
                StorageService.Server = value;
                RaisePropertyChanged("Server");
            }
        }

        public string Table
        {
            get { return StorageService.Table; }
            set
            {
                StorageService.Table = value;
                RaisePropertyChanged("Table");
            }
        }

        public string User
        {
            get { return StorageService.User; }
            set
            {
                StorageService.User = value;
                RaisePropertyChanged("User");
            }
        }

        public string Password
        {
            get { return StorageService.Password; }
            set
            {
                StorageService.Password = value;
                RaisePropertyChanged("Password");
            }
        }

        public string Port
        {
            get { return StorageService.Port; }
            set
            {
                StorageService.Port = value;
                RaisePropertyChanged("Port");
            }
        }

        public string Provider
        {
            get { return StorageService.Provider; }
            set
            {
                StorageService.Provider = value;
                RaisePropertyChanged("Provider");
            }
        }

        public ObservableCollection<string> ForbiddenExt
        {
            get { return StorageService.ForbiddenExt; }
            set
            {
                StorageService.ForbiddenExt = value;
                RaisePropertyChanged("ForbiddenExt");
            }
        }

        public ICommand OpenFolderDialogCommand
        {
            get { return new RelayCommand(ExecuteOpenFolderDialog); }
        }
        void ExecuteOpenFolderDialog()
        {
            FileName = MainModel.FindPath() + System.IO.Path.DirectorySeparatorChar + "Apps.XML";
        }
        public ICommand AddExtentionCommand
        {
            get { return new RelayCommand<object>(ExecuteAddExtention); }
        }

        void ExecuteAddExtention(object ext)
        {
            ForbiddenExt.Add(ext.ToString());
        }

        public ICommand RemoveExtentionCommand
        {
            get { return new RelayCommand<object>(ExecuteRemoveExtention); }
        }

        void ExecuteRemoveExtention(object ext)
        {
            ForbiddenExt.Remove(ext.ToString());
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
    }
}
