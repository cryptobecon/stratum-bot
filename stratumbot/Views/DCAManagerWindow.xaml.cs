using stratumbot.Models.Tools;
using stratumbot.ViewModels;
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
    /// Логика взаимодействия для DCAManagerWindow.xaml
    /// </summary>
    public partial class DCAManagerWindow : Window
    {

        public DCAManagerVM VM = new DCAManagerVM(); // Создаём ViewModel

        public DCAManagerWindow(string filtersSide, string DCAStepCount, ObservableCollection<string[]> DCASteps, Dictionary<int, DCAFilter> DCAFilters)
        {
            InitializeComponent();
            VM.FiltersSide = filtersSide; // DCA сторона для автовыбора bid/ask
            VM.DCAStepCount = DCAStepCount; // Количество шагов
            VM.DCASteps = new ObservableCollection<string[]>(DCASteps);
            VM.DCAFilters = new Dictionary<int, DCAFilter>(DCAFilters);

            DataContext = VM;
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void DragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            try { this.DragMove(); }
            catch { }
        }

        private void DCAConfig_Selected(object sender, RoutedEventArgs e)
        {
            /*TreeViewItem tvItem = (TreeViewItem)sender;

            VM.FilterView = null;

            VM.SelectedFilterName = tvItem.Header.ToString();

            if (tvItem.Header.ToString() == "H/L SMA")
                VM.FilterView = new HLSMAView();*/
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
