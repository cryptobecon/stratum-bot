using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для SaveConfigWindow.xaml
    /// </summary>
    public partial class SaveConfigWindow : Window
    {
        public string FileName { get; set; }
        public SaveConfigWindow(string _cur1 = null, string _cur2 = null)
        {
            InitializeComponent();
            DataContext = this;
            FileName = (_cur1 != null && _cur2 != null) ? $"{_cur1}-{_cur2}" : "My collection";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
