using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Core
{
    class OSInfo
    {
        // Разрадность системы (бит)
        private static string windowsBit;
        public static string WindowsBit
        {
            get {
                if(windowsBit != null)
                    return windowsBit;
                windowsBit = (Environment.Is64BitOperatingSystem ? "64" : "32");
                return windowsBit;
            }
        }

        // Версия сборки
        private static string winAssembly;
        public static string WinAssembly
        {
            get
            {
                if (winAssembly != null)
                    return winAssembly;
                winAssembly = Environment.OSVersion.ToString();
                return winAssembly;
            }
        }

        // Язык системы
        private static string windowsLang;
        public static string WindowsLang
        {
            get
            {
                if (windowsLang != null)
                    return windowsLang;
                windowsLang = System.Globalization.CultureInfo.CurrentUICulture.ToString();
                return windowsLang;
            }
        }

        // Версия NET Framework
        private static string netFramework;
        public static string NetFramework
        {
            get
            {
                if (netFramework != null)
                    return netFramework;

                string _key = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full";
                using (RegistryKey _regKey = Registry.LocalMachine.OpenSubKey(_key))
                {
                    if (_regKey != null)
                    {
                        try { netFramework = _regKey.GetValue("Version").ToString(); }
                        catch { netFramework = "0"; }
                    }
                }
                return netFramework;
            }
        }

        // Версия Windows
        private static string windowsVersion;
        public static string WindowsVersion
        {
            get
            {
                if (windowsVersion != null)
                    return windowsVersion;

                string key = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(key))
                {
                    if (regKey != null)
                    {
                        try  { windowsVersion = regKey.GetValue("ProductName").ToString(); }
                        catch { windowsVersion = "0"; }
                    }

                }
                return windowsVersion;
            }
        }
    }
}
