using stratumbot.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Логика взаимодействия для ThreadRecoveryWindow.xaml
    /// </summary>
    public partial class ThreadRecoveryWindow : Window
    {
        // Список всех потоков, которые нужно восстановить
        public ObservableCollection<ThreadBackup> ThreadsForRecovery { get; set; } = new ObservableCollection<ThreadBackup>();

        public ThreadRecoveryWindow(List<ThreadBackup> _threadsForRecovery)
        {
            InitializeComponent();
            this.DataContext = this;

            foreach (var thread in _threadsForRecovery)
            {
                ThreadsForRecovery.Add(thread);
            }

            Test.Items.Refresh();
        }

        private void Recovery_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
