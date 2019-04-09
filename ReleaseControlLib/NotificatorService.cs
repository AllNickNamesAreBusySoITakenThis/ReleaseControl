using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseControlLib
{
    public class NotificatorService
    {
        public static event System.ComponentModel.PropertyChangedEventHandler StaticPropertyChanged;
        static void OnStaticPorpertyChanged(string name)
        {
            StaticPropertyChanged?.Invoke(null, new System.ComponentModel.PropertyChangedEventArgs(name));
        }

        static int fileMaximum = 0;

        public static int FileMaximum
        {
            get { return fileMaximum; }
            set
            {
                fileMaximum = value;
                OnStaticPorpertyChanged("FileMaximum");
            }
        }

        private int currentFile;

        public int CurrentFile
        {
            get { return currentFile; }
            set { currentFile = value; OnStaticPorpertyChanged("CurrentFile"); }
        }

        private string currentFileName;

        public string CurrentFileName
        {
            get { return currentFileName; }
            set { currentFileName = value; OnStaticPorpertyChanged("CurrentFileName"); }
        }

    }
}
