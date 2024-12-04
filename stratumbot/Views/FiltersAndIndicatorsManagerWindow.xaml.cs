using stratumbot.Models.Filters;
using stratumbot.ViewModels;
using stratumbot.Views.FiltersViews;
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
    /// Логика взаимодействия для FiltersAndIndicatorsManagerWindow.xaml
    /// </summary>
    public partial class FiltersAndIndicatorsManagerWindow : Window
    {
        public FilterVM VM = new FilterVM(); // Создаём ViewModel

        public FiltersAndIndicatorsManagerWindow(List<JsonFilter> filters, int targetPoint, string filtersSide, bool isStopLoss = false, int step = 0)
        {
            InitializeComponent();
            this.DataContext = VM;

            VM.FiltersSide = filtersSide; // тип списка фильтров (на покупку или продажу)
            VM.Step = step; // Номер DCA шага //dcaf
            VM.TargetPoint = targetPoint;
            VM.Filters = filters; // Передаем список фильтров из голубой панели для их инициализации в окне фильтров
            VM.IsStopLoss = isStopLoss; // Для стоплосса ли фильтры щас будем настраивать?
            VM.LoadFilters();

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

        // Выбор индикатора
        private void FaI_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvItem = (TreeViewItem)sender;

            VM.FilterView = null;

            VM.SelectedFilterName = tvItem.Header.ToString();

            if (tvItem.Header.ToString() == "H/L SMA")
                VM.FilterView = new HLSMAView();
            if (tvItem.Header.ToString() == "H/L EMA")
                VM.FilterView = new HLEMAView();
            if (tvItem.Header.ToString() == "Price Change")
                VM.FilterView = new PriceChangeView();
            if (tvItem.Header.ToString() == "Price Limit")
                VM.FilterView = new PriceLimitView();
            if (tvItem.Header.ToString() == "DOM Volume Diff")
                VM.FilterView = new DOMVolumeDiffView();
            if (tvItem.Header.ToString() == "H/L SMMA") // 6
                VM.FilterView = new HLSMMAView();
            if (tvItem.Header.ToString() == "NGA") // 7
                VM.FilterView = new NGAView();
            if (tvItem.Header.ToString() == "OHLC+ Limit") // 8
                VM.FilterView = new OHLCPlusView();
            if (tvItem.Header.ToString() == "Cross") // 9
                VM.FilterView = new CrossView();
            if (tvItem.Header.ToString() == "Bollinger Bands") // 10
                VM.FilterView = new BollingerBandsView();
            if (tvItem.Header.ToString() == "RSI") // 11
                VM.FilterView = new RSIView();
            if (tvItem.Header.ToString() == "Stoch") // 12
                VM.FilterView = new StochView();
            if (tvItem.Header.ToString() == "Stoch RSI") // 13
                VM.FilterView = new StochRSIView();
            if (tvItem.Header.ToString() == "Email Notify") // 14
                VM.FilterView = new EmailNotifyView();
            if (tvItem.Header.ToString() == "URL") // 15
                VM.FilterView = new URLView();
            if (tvItem.Header.ToString() == "Spread") // 16
                VM.FilterView = new SpreadView();
            if (tvItem.Header.ToString() == "MA Spread") // 17
                VM.FilterView = new MASpreadView();
            if (tvItem.Header.ToString() == "Candle Price Change") // 18
                VM.FilterView = new CandlePriceChangeView();
            if (tvItem.Header.ToString() == "MFI") // 19
                VM.FilterView = new MFIView();
            if (tvItem.Header.ToString() == "CCI") // 20
                VM.FilterView = new CCIView();
            if (tvItem.Header.ToString() == "Timer") // 21
                VM.FilterView = new TimerView();
            if (tvItem.Header.ToString() == "Candle Color") // 22
                VM.FilterView = new CandleColorView();
            if (tvItem.Header.ToString() == "Volume Limit") // 23
                VM.FilterView = new VolumeLimitView();
            if (tvItem.Header.ToString() == "BBW") // 24
                VM.FilterView = new BollingerBandsWidthView();
            if (tvItem.Header.ToString() == "Keltner Channels") // 25
                VM.FilterView = new KeltnerChannelsView();
            if (tvItem.Header.ToString() == "STARC Bands") // 26
                VM.FilterView = new STARCBandsView();
            if (tvItem.Header.ToString() == "MA Envelopes") // 27
                VM.FilterView = new MAEnvelopesView();
			if (tvItem.Header.ToString() == "Donchian Channel") // 28
				VM.FilterView = new DonchianChannelView();
			if (tvItem.Header.ToString() == "SuperTrend") // 29
				VM.FilterView = new SuperTrendView();

			VM.DefaultParam();
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
