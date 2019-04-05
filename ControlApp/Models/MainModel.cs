using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using ReleaseControlLib;

namespace ControlApp
{
    public class MainModel
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public static ControlledApp tempApp { get; set; }

        public static void GetApps()
        {
            string result = StorageService.Get();
            if (result != "OK")
            {
                logger.Error("Ошибка при получени данных: {0}", result);
            }
        }

        public static void AddApp()
        {
            ManageAppWindow window = new ManageAppWindow();
            window.Owner = App.Current.MainWindow;
            window.ShowDialog();
        }

        public static void RemoveApp(object appToRemove)
        {
            if (appToRemove is ControlledApp)
            {
                string result = StorageService.RemoveApp(appToRemove as ControlledApp);
                if (result != "OK")
                {
                    logger.Error("Ошибка при удалении приложения: {0}", result);
                }
            }
        }

        public static void EditApp(object appToEdit)
        {
            ManageAppWindow window = new ManageAppWindow();
            window.Owner = App.Current.MainWindow;
            tempApp = (ControlledApp)appToEdit;
            window.ShowDialog();
        }

        public static void PushToRelease(object appToPush)
        {
            if(appToPush is ControlledApp)
            {
                string result = (appToPush as ControlledApp).ReleaseApp();
                if (result != "OK")
                {
                    logger.Error("Ошибка при обновлении релиза: {0}", result);
                }
            }
        }

        public static void ShowSettings()
        {
            SettingsWindow window = new SettingsWindow();
            window.Owner = App.Current.MainWindow;
            if(window.ShowDialog()==true)
            {
                Service.SaveSettings();
            }
            else
            {
                Service.ReadSettings();
            }
        }
    }
}
