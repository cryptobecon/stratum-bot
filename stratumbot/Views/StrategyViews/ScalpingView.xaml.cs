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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace stratumbot.Views.StrategyViews
{
    /// <summary>
    /// Логика взаимодействия для ScalpingView.xaml
    /// </summary>
    public partial class ScalpingView : UserControl
    {
        public ScalpingView()
        {
            InitializeComponent();
        }

        // Далее мы будем гавнокодить. Быстро и эффективно.

        private void more_open_btn_Click(object sender, MouseButtonEventArgs e)
        {
            more_open_btn.Visibility = Visibility.Hidden;
            hide_part.Visibility = Visibility.Visible;
            more_close_btn.Visibility = Visibility.Visible;
            scalping_scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void more_close_btn_Click(object sender, MouseButtonEventArgs e)
        {
            more_close_btn.Visibility = Visibility.Hidden;
            hide_part.Visibility = Visibility.Hidden;
            more_open_btn.Visibility = Visibility.Visible;
            scalping_scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scalping_scroll.ScrollToTop();
        }
    }
}
