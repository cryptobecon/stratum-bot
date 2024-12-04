using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace stratumbot.Core
{
    class Security
    {
        public static void Instance()
        {
            if(System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > Encryption.HEXx2ToInt(_.c))
                Application.Current.Shutdown();
        }
    }
}
