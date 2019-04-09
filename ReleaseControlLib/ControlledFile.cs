using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace ReleaseControlLib
{
    public class ControlledFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        string path = "";
        string currentHash = "";
        string releaseHash = "";
        ControlledApp parent = new ControlledApp();
        /// <summary>
        /// Путь к файлу (относительный)
        /// </summary>
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                UpdateHash();
                OnPropertyChanged("Path");
            }
        }
        /// <summary>
        /// Хеш-сумма файла из рабочего релиза
        /// </summary>
        public string CurrentHash
        {
            get { return currentHash; }
            set
            {
                currentHash = value;
                OnPropertyChanged("CurrentHash");
            }
        }
        /// <summary>
        /// Хеш-сумма в релизной директории
        /// </summary>
        public string ReleaseHash
        {
            get { return releaseHash; }
            set
            {
                releaseHash = value;
                OnPropertyChanged("ReleaseHash");
            }
        }

        public ControlledApp Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                OnPropertyChanged("Parent");
            }
        }
        /// <summary>
        /// Хеш релиза соответствует хешу рабочей копии
        /// </summary>
        public bool IsUpToDate
        {
            get
            {
                return CurrentHash == ReleaseHash;
            }
        }

        /// <summary>
        /// Отправить файл в релиз
        /// </summary>
        /// <returns></returns>
        public string SendToRelease()
        {
            try
            {
                File.Copy(string.Format("{0}{1}{2}", Parent.WorkingReleasePath, System.IO.Path.DirectorySeparatorChar, Path),
                            string.Format("{0}{1}{2}", Parent.ReleasePath, System.IO.Path.DirectorySeparatorChar, Path), true);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public void UpdateHash()
        {
            CurrentHash = HashService.ComputeMD5Checksum(Parent.WorkingReleasePath + System.IO.Path.DirectorySeparatorChar + path);
            ReleaseHash = HashService.ComputeMD5Checksum(Parent.ReleasePath + System.IO.Path.DirectorySeparatorChar + path);
        }
        public static Task<List<ControlledFile>> GetFilesList(string root, ControlledApp parent)
        {
            try
            {
                System.Func<List<ControlledFile>> myFunction = GetList;
                Task<List<ControlledFile>> result = new Task<List<ControlledFile>>(myFunction);
                DirectoryInfo di = new DirectoryInfo(root);
                foreach(var f in di.EnumerateFiles().ToList())
                {
                    if(!StorageService.ForbiddenExt.Contains(f.Extension))
                    {
                        result.Result.Add(new ControlledFile()
                        {
                            Parent = parent,
                            Path = f.FullName.Replace(parent.WorkingReleasePath + System.IO.Path.DirectorySeparatorChar, "")
                        });
                    }                    
                }
                foreach(var d in di.EnumerateDirectories().ToList())
                {
                    var temp = GetFilesList(string.Format("{0}{1}{2}", root, System.IO.Path.DirectorySeparatorChar, d.Name),parent);
                    if(temp!=null)
                    {
                        result.Result.AddRange(temp.Result);
                    }
                }
                return result;
            }
            catch(Exception)
            {
                return null;
            }
        }

        static List<ControlledFile> GetList()
        {
            return new List<ControlledFile>();
        }
    }
}
