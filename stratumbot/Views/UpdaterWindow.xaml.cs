using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace stratumbot.Views
{
    /// <summary>
    /// Логика взаимодействия для UpdaterWindow.xaml
    /// </summary>
    public partial class UpdaterWindow : Window
    {
        private string downloadUrl = "";

        public UpdaterWindow(string _downloadUrl)
        {
            InitializeComponent();

            downloadUrl = _downloadUrl;

            var web = new WebClient();
            var html = web.DownloadString(_.ChangeLogUrl);
            changelog.Text = html;
        }

        // Кнопка Обновить!
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            cancel_btn.IsEnabled = false;
            update_btn.IsEnabled = false;

            if (Directory.Exists("new"))
                Directory.Delete("new", true);
            Directory.CreateDirectory("new");
            string newVersionArchiveName = "new/new.zip";

            WebClient DownloaderClient = new WebClient();
            DownloaderClient.DownloadFileAsync(new Uri(downloadUrl), newVersionArchiveName);
            DownloaderClient.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            DownloaderClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
        }

        // Загрузка...
        void webClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            this.Title = String.Format("{0} Kb / {1} Kb", e.BytesReceived / 1024, e.TotalBytesToReceive / 1024);
            progress_bar.Value = e.ProgressPercentage;
        }

        // Загрузка завершена
        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(_.Msg19); // Ошибка при загрузке файла. Скачайте архив вручную в кабинете ...
            }
            else
            {
                // Готово
                try { System.Diagnostics.Process.Start("Updater.exe", "bablo"); }
                catch { MessageBox.Show(_.Msg19); }
                System.Windows.Application.Current.Shutdown();
                this.Close();
            }
        }
        
        // Кнопка закрыть / позже
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
