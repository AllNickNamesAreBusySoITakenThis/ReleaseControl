using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlApp
{
    class CommonModel
    {
        public static void ExitWithConfirm(object window)
        {
            if(window is System.Windows.Window)
            {
                (window as System.Windows.Window).DialogResult = true;
                (window as System.Windows.Window).Close();
            }
        }

        public static void ExitWithoutConfirm(object window)
        {
            if (window is System.Windows.Window)
            {
                (window as System.Windows.Window).DialogResult = false;
                (window as System.Windows.Window).Close();
            }
        }
    }
}
