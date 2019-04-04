﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace ReleaseControlLib
{
    public class ControlledApp : INotifyPropertyChanged
    {
        string name = "";
        string workingReleasePath = "";
        string releasePath = "";
        string reestrPath = "";
        ObservableCollection<ControlledFile> files = new ObservableCollection<ControlledFile>();
        bool existInDB = false;
        int id = 0;


        public int Id
        {
            get { return id; }
            set
            {
                id= value;
                OnPropertyChanged("Id");
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string WorkingReleasePath
        {
            get
            {
                return workingReleasePath;
            }
            set
            {
                workingReleasePath = value;
                OnPropertyChanged("WorkingReleasePath");
            }
        }

        public string ReleasePath
        {
            get
            {
                return releasePath;
            }
            set
            {
                releasePath = value;
                OnPropertyChanged("ReleasePath");
            }
        }

        public string ReestrPath
        {
            get
            {
                return reestrPath;
            }
            set
            {
                reestrPath = value;
                OnPropertyChanged("ReestrPath");
            }
        }

        public bool IsUpToDate
        {
            get
            {
                foreach (var file in Files)
                {
                    if (!file.IsUpToDate)
                        return false;
                }
                return true;
            }
        }

        public bool ExistInDb
        {
            get { return existInDB; }
            set
            {
                existInDB = value;
                OnPropertyChanged("ExistInDb");
            }
        }

        public ObservableCollection<ControlledFile> Files
        {
            get
            {
                return files;
            }
            set
            {
                files = value;
                OnPropertyChanged("Files");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        /// <summary>
        /// Добавлению подлежат все файлы в указанном каталоге и в директориях первого уровня вложенности
        /// </summary>
        /// <param name="name"></param>
        /// <param name="workFolderPath"></param>
        /// <param name="relFolderPath"></param>
        /// <returns></returns>
        public static ControlledApp AddApp(string name, string workFolderPath, string relFolderPath, string reestrPath, int id)
        {
            try
            {
                ControlledApp result = new ControlledApp();
                result.Name = name;
                result.ReleasePath = relFolderPath;
                result.WorkingReleasePath = workFolderPath;
                result.ReestrPath = reestrPath;
                result.Id = id;
                var dir = new DirectoryInfo(workFolderPath);
                result.Files = new ObservableCollection<ControlledFile>(ControlledFile.GetFilesList(workFolderPath, result));
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Добавить файл в релиз
        /// </summary>
        /// <param name="path"></param>
        public void AddFile(string path)
        {
            Files.Add(new ControlledFile()
            {
                Parent = this,
                Path = path
            });
        }


        /// <summary>
        /// Отправить приложение в релиз
        /// </summary>
        /// <returns>Ошибки отправки</returns>
        public string ReleaseApp()
        {
            string errors = "";
            foreach (var file in Files)
            {
                var err = file.SendToRelease();
                if (err != "OK")
                {
                    errors += string.Format("{0} - {1} /r/n", file.Path, err);
                }
            }
            return errors;
        }
    }
}
