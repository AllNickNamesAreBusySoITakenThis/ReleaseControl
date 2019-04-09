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
    public class MainWindowViewModel : ViewModelBase
    {
        ControlledApp selecetdapp = new ControlledApp();

        public ObservableCollection<ReleaseControlLib.ControlledApp> MyApps
        {
            get
            {
                if (StorageService.Apps != null)
                    return ReleaseControlLib.StorageService.Apps;
                else
                    return new ObservableCollection<ControlledApp>();
            }
            set { ReleaseControlLib.StorageService.Apps = value; }
        }

        public ControlledApp SelectedApp
        {
            get { return selecetdapp; }
            set
            {
                selecetdapp = value;
                RaisePropertyChanged("SelectedApp");
            }
        }

        public MainWindowViewModel()
        {
            Service.ReadSettings();
            MainModel.GetApps();
        }

        #region Команды

        #region Показать настройки
        public ICommand ExitCommand
        {
            get { return new RelayCommand(ExecuteExit); }
        }

        void ExecuteExit()
        {
            App.Current.Shutdown();
        }
        #endregion

        #region Показать настройки
        public ICommand ShowSettingsCommand
        {
            get { return new RelayCommand(ExecuteShowSettings); }
        }

        void ExecuteShowSettings()
        {
            MainModel.ShowSettings();
        }
        #endregion

        #region Экспортировать данные
        public ICommand ExportCommand
        {
            get { return new RelayCommand(ExecuteExport); }
        }

        void ExecuteExport()
        {
            MainModel.ExportToXml();
        }
        #endregion

        #region Импортировать данные
        public ICommand ImportCommand
        {
            get { return new RelayCommand(ExecuteImport); }
        }

        void ExecuteImport()
        {
            MainModel.Import();
        }
        #endregion

        #region Добавить приложение
        public ICommand AddAppCommand
        {
            get { return new RelayCommand(ExecuteAddApp); }
        }

        void ExecuteAddApp()
        {
            MainModel.AddApp();
        }
        #endregion

        #region Редактировать приложение
        public ICommand EditAppCommand
        {
            get { return new RelayCommand<object>(ExecuteEditApp); }
        }

        void ExecuteEditApp(object app)
        {
            MainModel.EditApp(app);
        }
        #endregion

        #region Удалить приложение
        public ICommand RemoveAppCommand
        {
            get { return new RelayCommand<object>(ExecuteRemoveApp); }
        }

        void ExecuteRemoveApp(object app)
        {
            MainModel.RemoveApp(app);
        }
        #endregion

        #region Выгрузить приложение в релиз
        public ICommand ReleaseAppCommand
        {
            get { return new RelayCommand<object>(ExecuteReleaseApp); }
        }

        void ExecuteReleaseApp(object app)
        {
            MainModel.PushToRelease(app);
        }
        #endregion

        #endregion

    }
}
