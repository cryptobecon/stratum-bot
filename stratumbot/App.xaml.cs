using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace stratumbot
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        }


        void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;

            // save
            string log = $"{DateTime.Now.ToString("HH:mm:ss")} - [EXCEPION] {e.ToString()}\n";
            string file = $"Logs/{DateTime.Now.ToString("yyyy.MM.dd")}_MAIN.log";
            System.IO.File.AppendAllText(file, log);

            MessageBox.Show(e.Message + "\n\r" + e.StackTrace, "Error [1]", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // save
            string log = $"{DateTime.Now.ToString("HH:mm:ss")} - [EXCEPION] {e.Exception.ToString()}\n";
            string file = $"Logs/{DateTime.Now.ToString("yyyy.MM.dd")}_MAIN.log";
            System.IO.File.AppendAllText(file, log);

            MessageBox.Show(e.Exception.Message + "\n\r" + e.Exception.StackTrace, "Error [2]", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
