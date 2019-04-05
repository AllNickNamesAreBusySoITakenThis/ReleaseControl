using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace ControlApp
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<ReleaseControlLib.ControlledApp> MyApps
        {
            get { return ReleaseControlLib.StorageService.Apps; }
            set { ReleaseControlLib.StorageService.Apps = value; }
        }
        #region Команды

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
