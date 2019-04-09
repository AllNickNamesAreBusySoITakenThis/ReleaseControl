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
        public static event System.ComponentModel.PropertyChangedEventHandler StaticPropertyChanged;
        static void OnStaticPropertyChanged(string name)
        {
            StaticPropertyChanged?.Invoke(null, new System.ComponentModel.PropertyChangedEventArgs(name));
        }

        static ControlledApp tempApp = new ControlledApp();
        static Logger logger = LogManager.GetCurrentClassLogger();
        public static ControlledApp TempApp
        {
            get { return tempApp; }
            set
            {
                tempApp = value;
                OnStaticPropertyChanged("TempApp");
            }
        }


        public async static void GetApps()
        {
            string result = "";
            await new Task(()=>StorageService.Get());
            //if (result != "OK")
            //{
            //    logger.Error("Ошибка при получени данных: {0}", result);
            //}
        }

        public static void Import()
        {
            try
            {
                System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
                if(dialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
                {
                    if(StorageService.ImportData(dialog.FileName)!="OK")
                    {
                        System.Windows.MessageBox.Show("Ошибка импорта!");
                    }
                }
                StorageService.Save();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public static void AddApp()
        {
            ManageAppWindow window = new ManageAppWindow();
            window.Owner = App.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                StorageService.Apps.Add(ControlledApp.AddApp(TempApp.Name, TempApp.WorkingReleasePath, TempApp.ReleasePath, TempApp.ReestrPath,StorageService.Apps.Count));
                StorageService.Save();
            }
            else
            {                
                StorageService.Get();
            }
            TempApp = new ControlledApp();
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
            TempApp = (ControlledApp)appToEdit;
            ManageAppWindow window = new ManageAppWindow();
            window.Owner = App.Current.MainWindow;            
            if(window.ShowDialog()==true)
            {
                StorageService.Save();
            }
            else
            {
                StorageService.Get();
            }
            TempApp = new ControlledApp();
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
            StorageService.Get();
        }

        public static void ExportToXml()
        {
            if(string.IsNullOrEmpty(StorageService.FilePath))
            {
                string temp = FindPath();
                if(!string.IsNullOrEmpty(temp))
                {
                    StorageService.ExprotToXml(temp + System.IO.Path.DirectorySeparatorChar + "Apps.XML");
                }
                else
                {
                    System.Windows.MessageBox.Show("Операция отменена!");
                }
            }
            else
            {
                StorageService.ExprotToXml(StorageService.FilePath);
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

        public static string FindPath()
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if(dialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            return "";
        }
    }
}
