using Microsoft.Win32;
using stratumbot.Models;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace stratumbot.Views
{
    /// <summary>
    /// Логика взаимодействия для LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            // Directory name in regisry
            string directory = "SB3";
            string secretKeyForSettingEncriptionFieldName = "s";
            string guidForDeviceHashingFieldName = "g";

            // Get direcory
            RegistryKey SB3 = Registry.CurrentUser.OpenSubKey(directory, true);

            // Launched before
            if (SB3 != null)
            {
                // Get secret key for settings decrypting
                Settings.SecretKey = ushort.Parse(SB3.GetValue(secretKeyForSettingEncriptionFieldName).ToString());

                // TODO DELETE Temporary for old users
                var guid = SB3.GetValue(guidForDeviceHashingFieldName);
                if (guid == null)
                {
                    SB3.SetValue(guidForDeviceHashingFieldName, Guid.NewGuid().ToString("N"));
                }

                SB3.Close();
            }
            // The first launch of software
            else
            {
                // TODO welcome window

                // Create a directory for software 
                RegistryKey newDirectory = Registry.CurrentUser.CreateSubKey(directory);

                // Generate and save random key for settings encrypting
                // TODO random
                // Random rand = new Random();
                // int seceretKey = rand.Next(1000,7000);
                int secretKey = 9000;
                newDirectory.SetValue(secretKeyForSettingEncriptionFieldName, secretKey);

                // Generate and save random guid key for device hashing
                string guid = Guid.NewGuid().ToString("N");
                newDirectory.SetValue(guidForDeviceHashingFieldName, secretKey);

                newDirectory.Close();

                Settings.SecretKey = 9000; // By default
                Settings.Load();
                Settings.SecretKey = (ushort)secretKey; // Save with the new key
                Settings.Save();
            }

            InitializeComponent();
            LoadingAsync();
        }

        private async void LoadingAsync()
        {
            await Task.Delay(10);
            MainWindow stratubbot = new MainWindow();
            try { stratubbot.Show(); } catch { }
            this.Close();
        }
    }
}
