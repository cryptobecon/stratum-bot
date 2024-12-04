using stratumbot.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace stratumbot
{

    #region Прозрачность
    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        WCA_ACCENT_POLICY = 19
    }
    #endregion

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainVM ViewModel = new MainVM();
        public MainWindow()
        {
            InitializeComponent();

            stratum_title.Content = "STRATUM-BOT v" + Core.Version.Current;

            DataContext = ViewModel;// new MainVM();

            //threadscontextmenu.ContextMenuOpening += Threadscontextmenu_ContextMenuOpening;
            //threadscontextmenu.Opened

            //List<Employeedata> Employeedatalist = new List<Employeedata>();
            //Employeedatalist.Add(new Employeedata { ID = 1, Photo = "/Views/Resources/Images/Exchanges/binance.png", Name = "Binance" });
            //Employeedatalist.Add(new Employeedata { ID = 1, Photo = "/Views/Resources/Images/Exchanges/binance2.png", Name = "YoBit" });
            //Employeedatalist.Add(new Employeedata { ID = 1, Photo = "/Views/Resources/Images/Exchanges/binance3.png", Name = "KuCoin" });
            //lstwithimg.ItemsSource = Employeedatalist;
            //lstwithimg.SelectedIndex = 0;

            /*List<Employeedata> Employeedatalist2 = new List<Employeedata>();
            Employeedatalist2.Add(new Employeedata { ID = 1, Photo = "/Views/Resources/Images/Strategies/Scalping.png", Name = "Scalping" });
            Employeedatalist2.Add(new Employeedata { ID = 1, Photo = "/Views/Resources/Images/Strategies/Scalping.png", Name = "Long" });
            Employeedatalist2.Add(new Employeedata { ID = 1, Photo = "/Views/Resources/Images/Strategies/Scalping.png", Name = "Arbitrage" });
            lstwithimg2.ItemsSource = Employeedatalist2;
            lstwithimg2.SelectedIndex = 0;*/

            /*System.Threading.Thread toyThread = new System.Threading.Thread(toyHide);
            toyThread.IsBackground = true;
            toyThread.Start();*/
        }

        /*public void toyHide()
        {
            System.Threading.Thread.Sleep(25000);
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                toy.Visibility = Visibility.Hidden;
            });
        }*/


        #region Прозрачность
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if(Core.OSInfo.WindowsVersion.Contains("10"))
                EnableBlur();
        }

        private void EnableBlur()
        {
            var windowHelper = new System.Windows.Interop.WindowInteropHelper(this);

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
            var accentStructSize = System.Runtime.InteropServices.Marshal.SizeOf(accent);

            var accentPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(accentStructSize);
            System.Runtime.InteropServices.Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            System.Runtime.InteropServices.Marshal.FreeHGlobal(accentPtr);
        }
        #endregion

        // Перемещение окна ЛКМ
        private void DragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            try { this.DragMove(); }
            catch { }
        }



        #region TitleBar
        // Закрыть программу
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        // Размер окна
        private void StateButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.Width = 925;
                this.Height = 570;
                this.WindowState = WindowState.Normal;
                Wrap.Margin = new Thickness(0, 0, 0, 0);
                // change icon
            }
            else
            {
                MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
                this.MaxWidth = MaxWidth;
                this.MaxHeight = MaxHeight;
                Wrap.Margin = new Thickness(5, 5, 5, 5); // TODO TEST протестировать на всех экранах норм ли это смотрится
                this.WindowState = WindowState.Maximized;
                // change icon
            }
        }

        // Свернуть окно
        private void StateMinimizedButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        #endregion

        // Фокус на панели стратегии
        private void Grid_GotFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.AddState();
        }

        // Фокус на таблице рабочих потоков
        private void Table_GotFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.WorkState();
        }

        // Загрузка коллекций при наведении
        private void LoadCollectionMenuItem_MouseEnter(object sender, MouseEventArgs e)
        {
            System.Windows.Controls.MenuItem newExistMenuItem = (System.Windows.Controls.MenuItem)this.threadscontextmenu.Items[4]; // пункт загрузить колл

            // загрука всех папок коллекций 
            string[] stratFiles = System.IO.Directory.GetDirectories(@"Collections/", "*", System.IO.SearchOption.AllDirectories);
            newExistMenuItem.Items.Clear();
            foreach (var file in stratFiles)
            {
                System.Windows.Controls.MenuItem newMenuItem2 = new System.Windows.Controls.MenuItem();
                newMenuItem2.SetBinding(System.Windows.Controls.MenuItem.CommandProperty, new System.Windows.Data.Binding("LoadCollectionClick"));
                newMenuItem2.CommandParameter = System.IO.Path.GetFileNameWithoutExtension(file);
                newMenuItem2.Header = System.IO.Path.GetFileNameWithoutExtension(file);
                newExistMenuItem.Items.Add(newMenuItem2);

            }

            // Если нет сохранёных потоков то выводим (пусто)
            if (stratFiles.Length == 0)
            {
                System.Windows.Controls.MenuItem newMenuItem2 = new System.Windows.Controls.MenuItem();
                newMenuItem2.Header = _.Empty;
                newExistMenuItem.Items.Add(newMenuItem2);
                newMenuItem2.IsEnabled = false;
            }
        }

        // Обновление контекстного меню (нужно, чтобы IsStopAfterSell для каждого потока отдельно работало)
        private void Threadscontextmenu_Opened(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateContextMenu();
        }
    }

}
