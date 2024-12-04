using stratumbot.Core;
using stratumbot.Interfaces;
using stratumbot.Models;
using stratumbot.Models.Filters;
using stratumbot.Models.Filters.Implementations;
using stratumbot.Views.FiltersViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace stratumbot.ViewModels
{
    // Событие для добавления списка фильтров в голубую панель предварительного настроек параметров стратегии
    public delegate void AddFilters(List<JsonFilter> filters, int targetPoint, string filtersSide, bool isStopLoss, int step = 0);
    public delegate void AddFiltersSettings(List<IFilter> filters);

    public class FilterVM : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        // Тип фильтров (на покупку или продажу)
        public string FiltersSide;

        // Представление фильтра
        public object filterView;
        public object FilterView
        {
            get { return filterView; }
            set
            {
                filterView = value;
                OnPropertyChanged();
            }
        }

        public bool addingMode;
        public bool AddingMode
        {
            get { return addingMode; }
            set
            {
                addingMode = value;
                OnPropertyChanged();
            }
        }

        public bool editMode;
        public bool EditMode
        {
            get { return editMode; }
            set
            {
                editMode = value;
                OnPropertyChanged();
            }
        }

        // Выбранный фильтр из списка фильтров (PreparedFilters)
        public IFilter selectedFilter;
        public IFilter SelectedFilter
        {
            get { return selectedFilter; }
            set
            {
                if(value == null)
                {
                    AddingMode = true;
                    EditMode = false;
                } else
                {
                    EditMode = true;
                    AddingMode = false;
                }

                selectedFilter = value;
                OnPropertyChanged();

                // Отображение выбранного филтра из подготовленных PreparedFilters

                if (SelectedFilter.ID == "1")
                {
                    this.FilterView = new HLSMAView();
                    this.Mode = (SelectedFilter as HLSMA).Mode;
                    this.Period = (SelectedFilter as HLSMA).Period;
                    this.TimeFrame = (SelectedFilter as HLSMA).TimeFrame;
                    this.Indent = (SelectedFilter as HLSMA).Indent;
                    this.Duration = (SelectedFilter as HLSMA).Duration;
                    this.DepthSide = (SelectedFilter as HLSMA).DepthSide;
                }
                if (SelectedFilter.ID == "2")
                {
                    this.FilterView = new HLEMAView();
                    this.Mode = (SelectedFilter as HLEMA).Mode;
                    this.Period = (SelectedFilter as HLEMA).Period;
                    this.TimeFrame = (SelectedFilter as HLEMA).TimeFrame;
                    this.Indent = (SelectedFilter as HLEMA).Indent;
                    this.Duration = (SelectedFilter as HLEMA).Duration;
                    this.DepthSide = (SelectedFilter as HLEMA).DepthSide;
                }
                if (SelectedFilter.ID == "3")
                {
                    this.FilterView = new PriceChangeView();
                    this.Mode = (SelectedFilter as PriceChange).Mode;
                    this.Cur1 = (SelectedFilter as PriceChange).Cur1;
                    this.Cur2 = (SelectedFilter as PriceChange).Cur2;
                    this.Side = (SelectedFilter as PriceChange).Side;
                    this.PriceValue = (SelectedFilter as PriceChange).PriceValue;
                    this.Duration = (SelectedFilter as PriceChange).Duration;
                }
                if (SelectedFilter.ID == "4")
                {
                    this.FilterView = new PriceLimitView();
                    this.Mode = (SelectedFilter as PriceLimit).Mode;
                    this.PriceLimitValue = (SelectedFilter as PriceLimit).PriceLimitValue;
                    this.Duration = (SelectedFilter as PriceLimit).Duration;
                    this.DepthSide = (SelectedFilter as PriceLimit).DepthSide;
                }
                if (SelectedFilter.ID == "5")
                {
                    this.FilterView = new DOMVolumeDiffView();
                    this.Mode = (SelectedFilter as DOMVolumeDiff).Mode;
                    this.Side = (SelectedFilter as DOMVolumeDiff).Side;
                    this.Period = (SelectedFilter as DOMVolumeDiff).Period;
                    this.VolumeValue = (SelectedFilter as DOMVolumeDiff).VolumeValue;
                    this.Duration = (SelectedFilter as DOMVolumeDiff).Duration;
                }
                if (SelectedFilter.ID == "6")
                {
                    this.FilterView = new HLSMMAView();
                    this.Mode = (SelectedFilter as HLSMMA).Mode;
                    this.Period = (SelectedFilter as HLSMMA).Period;
                    this.TimeFrame = (SelectedFilter as HLSMMA).TimeFrame;
                    this.Indent = (SelectedFilter as HLSMMA).Indent;
                    this.Duration = (SelectedFilter as HLSMMA).Duration;
                }
                if (SelectedFilter.ID == "7")
                {
                    this.FilterView = new NGAView();
                    this.Mode = (SelectedFilter as NGA).Mode;
                    this.Period = (SelectedFilter as NGA).Period;
                    this.TimeFrame = (SelectedFilter as NGA).TimeFrame;
                    this.Indent = (SelectedFilter as NGA).Indent;
                    this.Duration = (SelectedFilter as NGA).Duration;
                }

                if (SelectedFilter.ID == "8")
                {
                    this.FilterView = new OHLCPlusView();
                    this.Mode = (SelectedFilter as OHLCPlusLimit).Mode;
                    this.Source = (SelectedFilter as OHLCPlusLimit).Source;
                    this.PriceType = (SelectedFilter as OHLCPlusLimit).PriceType;
                    this.Period = (SelectedFilter as OHLCPlusLimit).Period;
                    this.TimeFrame = (SelectedFilter as OHLCPlusLimit).TimeFrame;
                    this.Indent = (SelectedFilter as OHLCPlusLimit).Indent;
                    this.Duration = (SelectedFilter as OHLCPlusLimit).Duration;
                }
                if (SelectedFilter.ID == "9")
                {
                    this.FilterView = new CrossView();
                    this.Mode = (SelectedFilter as Cross).Mode;
                    this.Line1 = (SelectedFilter as Cross).Line1;
                    this.Period = (SelectedFilter as Cross).Period;
                    this.Line2 = (SelectedFilter as Cross).Line2;
                    this.Period2 = (SelectedFilter as Cross).Period2;
                    this.TimeFrame = (SelectedFilter as Cross).TimeFrame;
                    this.Duration = (SelectedFilter as Cross).Duration;
                }
                if (SelectedFilter.ID == "10")
                {
                    this.FilterView = new BollingerBandsView();
                    this.Mode = (SelectedFilter as BollingerBands).Mode;
                    this.Rate = (SelectedFilter as BollingerBands).Rate;
                    this.Period = (SelectedFilter as BollingerBands).Period;
                    this.TimeFrame = (SelectedFilter as BollingerBands).TimeFrame;
                    this.Indent = (SelectedFilter as BollingerBands).Indent;
                    this.Duration = (SelectedFilter as BollingerBands).Duration;
                }
                if (SelectedFilter.ID == "11")
                {
                    this.FilterView = new RSIView();
                    this.Mode = (SelectedFilter as RSI).Mode;
                    this.PriceValue = (SelectedFilter as RSI).PriceValue;
                    this.Period = (SelectedFilter as RSI).Period;
                    this.TimeFrame = (SelectedFilter as RSI).TimeFrame;
                    this.Duration = (SelectedFilter as RSI).Duration;
                }
                if (SelectedFilter.ID == "12")
                {
                    this.FilterView = new StochView();
                    this.Mode = (SelectedFilter as Stoch).Mode;
                    this.PriceValue = (SelectedFilter as Stoch).PriceValue;
                    this.Period = (SelectedFilter as Stoch).Period;
                    this.Period2 = (SelectedFilter as Stoch).Period2;
                    this.Period3 = (SelectedFilter as Stoch).Period3;
                    this.TimeFrame = (SelectedFilter as Stoch).TimeFrame;
                    this.Duration = (SelectedFilter as Stoch).Duration;
                }
                if (SelectedFilter.ID == "13")
                {
                    this.FilterView = new StochRSIView();
                    this.Mode = (SelectedFilter as StochRSI).Mode;
                    this.PriceValue = (SelectedFilter as StochRSI).PriceValue;
                    this.Period = (SelectedFilter as StochRSI).Period;
                    this.Period2 = (SelectedFilter as StochRSI).Period2;
                    this.Period3 = (SelectedFilter as StochRSI).Period3;
                    this.Period4 = (SelectedFilter as StochRSI).Period4;
                    this.TimeFrame = (SelectedFilter as StochRSI).TimeFrame;
                    this.Duration = (SelectedFilter as StochRSI).Duration;
                }
                if (SelectedFilter.ID == "14")
                {
                    this.FilterView = new EmailNotifyView();
                    this.GoogleLogin = (SelectedFilter as EmailNotify).Login;
                    this.GooglePassword = (SelectedFilter as EmailNotify).Password;
                    this.Text = (SelectedFilter as EmailNotify).Text;
                    this.Duration = (SelectedFilter as EmailNotify).Duration;
                }
                if (SelectedFilter.ID == "15")
                {
                    this.FilterView = new URLView();
                    this.Text = (SelectedFilter as URL).Text;
                    this.Duration = (SelectedFilter as EmailNotify).Duration;
                }
                if (SelectedFilter.ID == "16")
                {
                    this.FilterView = new SpreadView();
                    this.Mode = (SelectedFilter as Spread).Mode;
                    this.Diff = (SelectedFilter as Spread).Diff;
                    this.Duration = (SelectedFilter as Spread).Duration;
                }
                if (SelectedFilter.ID == "17")
                {
                    this.FilterView = new MASpreadView();
                    this.Mode = (SelectedFilter as MASpread).Mode;
                    this.Diff = (SelectedFilter as MASpread).Diff;
                    this.Line1 = (SelectedFilter as MASpread).Line1;
                    this.Period = (SelectedFilter as MASpread).Period;
                    this.Line1 = (SelectedFilter as MASpread).Line2;
                    this.Period2 = (SelectedFilter as MASpread).Period2;
                    this.TimeFrame = (SelectedFilter as MASpread).TimeFrame;
                    this.Duration = (SelectedFilter as MASpread).Duration;
                }
                if (SelectedFilter.ID == "18")
                {
                    this.FilterView = new CandlePriceChangeView();
                    this.Cur1 = (SelectedFilter as CandlePriceChange).Cur1;
                    this.Cur2 = (SelectedFilter as CandlePriceChange).Cur2;
                    this.Mode = (SelectedFilter as CandlePriceChange).Mode;
                    this.Side = (SelectedFilter as CandlePriceChange).Side;
                    this.PriceValue = (SelectedFilter as CandlePriceChange).PriceValue;
                    this.Period = (SelectedFilter as CandlePriceChange).Period;
                    this.TimeFrame = (SelectedFilter as CandlePriceChange).TimeFrame;
                    this.Duration = (SelectedFilter as CandlePriceChange).Duration;
                }
                if (SelectedFilter.ID == "19")
                {
                    this.FilterView = new MFIView();
                    this.Mode = (SelectedFilter as MFI).Mode;
                    this.PriceValue = (SelectedFilter as MFI).PriceValue;
                    this.Period = (SelectedFilter as MFI).Period;
                    this.TimeFrame = (SelectedFilter as MFI).TimeFrame;
                    this.Duration = (SelectedFilter as MFI).Duration;
                }
                if (SelectedFilter.ID == "20")
                {
                    this.FilterView = new CCIView();
                    this.Mode = (SelectedFilter as CCI).Mode;
                    this.PriceValue = (SelectedFilter as CCI).PriceValue;
                    this.Period = (SelectedFilter as CCI).Period;
                    this.TimeFrame = (SelectedFilter as CCI).TimeFrame;
                    this.Duration = (SelectedFilter as CCI).Duration;
                }
                if (SelectedFilter.ID == "21")
                {
                    this.FilterView = new TimerView();
                    this.Mode = (SelectedFilter as Timer).Mode;
                    this.Duration = (SelectedFilter as Timer).Duration;
                }
                if (SelectedFilter.ID == "22")
                {
                    this.FilterView = new CandleColorView();
                    this.Mode = (SelectedFilter as CandleColor).Mode;
                    this.Side = (SelectedFilter as CandleColor).Side;
                    this.Period = (SelectedFilter as CandleColor).Period;
                    this.TimeFrame = (SelectedFilter as CandleColor).TimeFrame;
                    this.Duration = (SelectedFilter as CandleColor).Duration;
                }
                if (SelectedFilter.ID == "23")
                {
                    this.FilterView = new VolumeLimitView();
                    this.Mode = (SelectedFilter as VolumeLimit).Mode;
                    this.Cur1 = (SelectedFilter as VolumeLimit).Cur1;
                    this.Cur2 = (SelectedFilter as VolumeLimit).Cur2;
                    this.VolumeValue = (SelectedFilter as VolumeLimit).VolumeValue;
                    this.Duration = (SelectedFilter as VolumeLimit).Duration;
                }
                if (SelectedFilter.ID == "24")
                {
                    this.FilterView = new BollingerBandsWidthView();
                    this.Mode = (SelectedFilter as BollingerBandsWidth).Mode;
                    this.Rate = (SelectedFilter as BollingerBandsWidth).Rate;
                    this.Period = (SelectedFilter as BollingerBandsWidth).Period;
                    this.TimeFrame = (SelectedFilter as BollingerBandsWidth).TimeFrame;
                    this.PriceValue = (SelectedFilter as BollingerBandsWidth).PriceValue;
                    this.Duration = (SelectedFilter as BollingerBandsWidth).Duration;
                }
                if (SelectedFilter.ID == "25")
                {
                    this.FilterView = new KeltnerChannelsView();
                    this.Mode = (SelectedFilter as KeltnerChannels).Mode;
                    this.Rate = (SelectedFilter as KeltnerChannels).Rate;
                    this.Period = (SelectedFilter as KeltnerChannels).Period;
                    this.Period2 = (SelectedFilter as KeltnerChannels).Period2;
                    this.TimeFrame = (SelectedFilter as KeltnerChannels).TimeFrame;
                    this.Indent = (SelectedFilter as KeltnerChannels).Indent;
                    this.Duration = (SelectedFilter as KeltnerChannels).Duration;
                }
                if (SelectedFilter.ID == "26")
                {
                    this.FilterView = new STARCBandsView();
                    this.Mode = (SelectedFilter as STARCBands).Mode;
                    this.Rate = (SelectedFilter as STARCBands).Rate;
                    this.Period = (SelectedFilter as STARCBands).Period;
                    this.Period2 = (SelectedFilter as STARCBands).Period2;
                    this.TimeFrame = (SelectedFilter as STARCBands).TimeFrame;
                    this.Indent = (SelectedFilter as STARCBands).Indent;
                    this.Duration = (SelectedFilter as STARCBands).Duration;
                }
                if (SelectedFilter.ID == "27")
                {
                    this.FilterView = new MAEnvelopesView();
                    this.Mode = (SelectedFilter as MAEnvelopes).Mode;
                    this.Rate = (SelectedFilter as MAEnvelopes).Rate;
                    this.Period = (SelectedFilter as MAEnvelopes).Period;
                    this.Source = (SelectedFilter as MAEnvelopes).Source;
                    this.TimeFrame = (SelectedFilter as MAEnvelopes).TimeFrame;
                    this.Duration = (SelectedFilter as MAEnvelopes).Duration;
                }
				if (SelectedFilter.ID == "28")
				{
					this.FilterView = new DonchianChannelView();
					this.Mode = (SelectedFilter as DonchianChannel).Mode;
					this.Period = (SelectedFilter as DonchianChannel).Period;
					this.TimeFrame = (SelectedFilter as DonchianChannel).TimeFrame;
					this.Indent = (SelectedFilter as DonchianChannel).Indent;
					this.Duration = (SelectedFilter as DonchianChannel).Duration;
				}
                if (SelectedFilter.ID == "29")
                {
                    this.FilterView = new SuperTrendView();
                    this.Mode = (SelectedFilter as SuperTrend).Mode;
                    this.Rate = (SelectedFilter as SuperTrend).Rate;
                    this.Period = (SelectedFilter as SuperTrend).Period;
                    this.TimeFrame = (SelectedFilter as SuperTrend).TimeFrame;
                    this.Indent = (SelectedFilter as SuperTrend).Indent;
                    this.Duration = (SelectedFilter as SuperTrend).Duration;
                }
                this.MyName = SelectedFilter.MyName;
                this.Weight = SelectedFilter.Weight;
                this.Group = SelectedFilter.Group;
                this.Color = SelectedFilter.Color;
                this.ColorIndex = colors.IndexOf(this.Color.ToString());

            }
        }

        // Цвета фильтров
        List<string> colors = new List<string>()
        {
            // 1) Добавить цвет во все View фильтров
            // 2) Добавить цвет в этот список в соответствующий индекс

            "#00000000",
            "#FFFFFF00",
            "#FFFF0000",
            "#FFFFC000",
            "#FF00B0F0",
            "#FF5B9BD5",
            "#FF0070C0",
            "#FF66FFFF",
            "#FFFF00FF",
            "#FFCC99FF",
            "#FF8EA9DB",
            "#FFFFD966",
            "#FFF4B084",
            "#FFA9D08E",
            "#FF92D050",
            "#FF00B050",
        };

        // Название выбранного фильтра
        public string selectedFilterName;
        public string SelectedFilterName
        {
            get { return selectedFilterName; }
            set
            {
                selectedFilterName = value;
                if(this.MyName == null)
                    this.MyName = value; // В поле по умолчанию записываем также само название фильтра
                OnPropertyChanged();
            }
        }

        public IFilter filter;
        public IFilter Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                OnPropertyChanged();
            }
        }

        // Параметры фильтров

        // Higher Or Lower - index for ComboBox
        public int mode;
        public int Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                OnPropertyChanged();
            }
        }

        // Up or Down - куда сдвинулась цена
        public int side;
        public int Side
        {
            get { return side; }
            set
            {
                side = value;
                OnPropertyChanged();
            }
        }

        // Period
        public int period;
        public int Period
        {
            get { return period; }
            set
            {
                period = value;
                OnPropertyChanged();
            }
        }

        // TimeFrame
        public string timeFrame;
        public string TimeFrame
        {
            get { return timeFrame; }
            set
            {
                timeFrame = value;
                OnPropertyChanged();
            }
        }

        // Отступ
        public decimal indent;
        public decimal Indent
        {
            get { return indent; }
            set
            {
                indent = value;
                OnPropertyChanged();
            }
        }

        // Duratiom - продолжительность действия (сек)
        public int duration;
        public int Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                OnPropertyChanged();
            }
        }

        // DepthSide
        public string depthSide;
        public string DepthSide
        {
            get { return depthSide; }
            set
            {
                depthSide = value;
                OnPropertyChanged();
            }
        }


        // Значение цены
        public decimal priceValue;
        public decimal PriceValue
        {
            get { return priceValue; }
            set
            {
                priceValue = value;
                OnPropertyChanged();
            }
        }

        // Значение ограничения цены 
        public decimal priceLimitValue;
        public decimal PriceLimitValue
        {
            get { return priceLimitValue; }
            set
            {
                priceLimitValue = value;
                OnPropertyChanged();
            }
        }

        // Значение объема 
        public decimal volumeValue;
        public decimal VolumeValue
        {
            get { return volumeValue; }
            set
            {
                volumeValue = value;
                OnPropertyChanged();
            }
        }

        // Source - источник цены OHCL+
        public int source;
        public int Source
        {
            get { return source; }
            set
            {
                source = value;
                OnPropertyChanged();
            }
        }

        // PriceType - Max Min Avg
        public int priceType;
        public int PriceType
        {
            get { return priceType; }
            set
            {
                priceType = value;
                OnPropertyChanged();
            }
        }

        // Line 1
        public int line1;
        public int Line1
        {
            get { return line1; }
            set
            {
                line1 = value;
                OnPropertyChanged();
            }
        }

        // Line 2
        public int line2;
        public int Line2
        {
            get { return line2; }
            set
            {
                line2 = value;
                OnPropertyChanged();
            }
        }

        // Period2
        public int period2;
        public int Period2
        {
            get { return period2; }
            set
            {
                period2 = value;
                OnPropertyChanged();
            }
        }

        // Period3 (сглаживание в Stoch / Stoch RSI)
        public int period3;
        public int Period3
        {
            get { return period3; }
            set
            {
                period3 = value;
                OnPropertyChanged();
            }
        }

        // Period3 (сглаживание в Stoch / Stoch RSI)
        public int period4;
        public int Period4
        {
            get { return period4; }
            set
            {
                period4 = value;
                OnPropertyChanged();
            }
        }

        // Коэффицент (кол-во стандартных отклонений, напр)
        public decimal rate;
        public decimal Rate
        {
            get { return rate; }
            set
            {
                rate = value;
                OnPropertyChanged();
            }
        }

        // Разница (Spread)
        public decimal diff;
        public decimal Diff
        {
            get { return diff; }
            set
            {
                diff = value;
                OnPropertyChanged();
            }
        }

        public string text; // Заголовок сообщения (Email Notify)
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }

        // Данные 

        public string googleLogin;
        public string GoogleLogin
        {
            get { return googleLogin; }
            set
            {
                googleLogin = value;
                OnPropertyChanged();
            }
        }

        public string googlePassword;
        public string GooglePassword
        {
            get { return googlePassword; }
            set
            {
                googlePassword = value;
                OnPropertyChanged();
            }
        }

        // Пара

        public string cur1;
        public string Cur1
        {
            get { return cur1; }
            set
            {
                cur1 = value;
                OnPropertyChanged();
            }
        }
        public string cur2;
        public string Cur2
        {
            get { return cur2; }
            set
            {
                cur2 = value;
                OnPropertyChanged();
            }
        }

        // Общие параметры фильтров

        // Weight
        public int weight;
        public int Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                OnPropertyChanged();
            }
        }

        // myName
        public string myName;
        public string MyName
        {
            get { return myName; }
            set
            {
                myName = value;
                OnPropertyChanged();
            }
        }

        // Группа
        public string group;
        public string Group
        {
            get { return group; }
            set
            {
                group = value;
                OnPropertyChanged();
            }
        }

        // Group A Weight
        public int weightGroupA;
        public int WeightGroupA
        {
            get { return weightGroupA; }
            set
            {
                weightGroupA = value;
                OnPropertyChanged();
            }
        }
        // Group B Weight
        public int weightGroupB;
        public int WeightGroupB
        {
            get { return weightGroupB; }
            set
            {
                weightGroupB = value;
                OnPropertyChanged();
            }
        }
        // Group C Weight
        public int weightGroupC;
        public int WeightGroupC
        {
            get { return weightGroupC; }
            set
            {
                weightGroupC = value;
                OnPropertyChanged();
            }
        }

        // TargetPoint - целевой вес для прохождения
        public int targetPoint;
        public int TargetPoint
        {
            get { return targetPoint; }
            set
            {
                targetPoint = value;
                OnPropertyChanged();
            }
        }

        // Цвет
        public System.Windows.Media.Brush color;
        public System.Windows.Media.Brush Color
        {
            get { return color; }
            set
            {
                color = value;
                OnPropertyChanged();
            }
        }

        public int colorIndex;
        public int ColorIndex
        {
            get { return colorIndex; }
            set
            {
                colorIndex = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IFilter> PreparedFilters { get; set; } = new ObservableCollection<IFilter>(); // Список фильтров с полными настрйоками для отправки на сайт
        public List<JsonFilter> Filters { get; set; } = new List<JsonFilter>(); // Списк индикаторов (ID, без настроек) для голубой панели

        private bool proMode; // Pro mode
        public bool ProMode
        {
            get { return proMode; }
            set
            {
                proMode = value;
                OnPropertyChanged();
            }
        }

        private bool isStopLoss; // Для стоплосса ли фильтры настраиваются
        public bool IsStopLoss
        {
            get { return isStopLoss; }
            set
            {
                isStopLoss = value;
                OnPropertyChanged();
            }
        }

        public int Step; // Step of DCA //dcaf

        public FilterVM()
        {
            this.ProMode = Settings.ProMode;
        }

        // TODO тут будут дифферент фильтры
        // Загрузка фильтров (их полных настроек) с сайта в список с настройками (PreparedFilters) для отображения по списку без настроек (this.Filters)
        public void LoadFilters()
        {
            if(this.Filters.Count != 0)
            {
                foreach (var filter in this.Filters)
                {
                    var newFilter = FilterManager.GetFilterObjectByBtnPlusId(filter, this.FiltersSide); // Create FilterByID(filter);
                    PreparedFilters.Add(newFilter);
                }

                this.UpdateGroupWeights();
            }
        }

        // Значения по умолчанию для параметров фильтров
        public void DefaultParam()
        {
            this.Mode = 0;
            this.Line1 = 0;
            this.Line2 = 0;
            this.Period2 = 0;
            this.Period3 = 0;
            this.Period4 = 0;
            this.Period = 0;
            this.Indent = 0;
            this.Duration = 0;
            this.Side = 0;
            this.Source = 0;
            this.PriceType = 0;
            this.PriceValue = 0;
            this.VolumeValue = 0;
            this.Rate = 0;
            this.PriceLimitValue = 0;
            this.Group = "A";
            this.Weight = 1;
            this.ColorIndex = -1;
            this.AddingMode = true;
            this.EditMode = false;
            this.MyName = SelectedFilterName;
            this.GoogleLogin = Settings.GoogleLogin;
            this.GooglePassword = Settings.GooglePassword;
            this.Text = "";
            this.Cur1 = "";
            this.Cur2 = "";
            this.Diff = 0;
            this.DepthSide = (FiltersSide == "SELL") ? "Bid" : "Ask";
        }

        // Добавить индикатор (в список сбоку c настройками (PreparedFilters))
        public ICommand PrepareFilterClick
        {
            get
            {
                return new Command((obj) =>
                {
                    if (this.SelectedFilterName == null)
                        return; // TODO normal error window

                    var newFilter = CreateFilter(); // Создаём объект фильтра в список справа
                    PreparedFilters.Add(newFilter);

                    this.UpdateGroupWeights();
                    this.SetMinimalTargetPoint();
                });
            }
        }

        // Удаление выбранного фильтра из списка
        public ICommand DeleteFilterClick
        {
            get
            {
                return new Command((obj) =>
                {
                    PreparedFilters.Remove(this.SelectedFilter);
                    this.UpdateGroupWeights();
                    this.SetMinimalTargetPoint();
                });
            }
        }

        // Событие для добавления списка фильтров в голубую панель предварительного настроек параметров стратегии
        public event AddFilters AddFiltersEvent;
        //public event AddFiltersSettings AddFiltersSettingsEvent; // Филттры с настройками для отображения в окне

        // 
        public ICommand OkClick
        {
            get
            {
                return new Command((obj) =>
                {

                    // Сформировать Filter Filters
                    var filters = new List<JsonFilter>();
                    foreach (var filter in PreparedFilters)
                    {
                        // Загрузить данные
                        // Получить ID

                        // removed

                        int registeredId = 0;

                        if (filter.Name == "Bollinger Bands") { registeredId = 10; }
                        if (filter.Name == "Bollinger Bands Width") { registeredId = 24; }
                        if (filter.Name == "Candle Color") { registeredId = 22; }
                        if (filter.Name == "Candle Price Change") { registeredId = 18; }
                        if (filter.Name == "CCI") { registeredId = 20; }
                        if (filter.Name == "Cross") { registeredId = 9; }
                        if (filter.Name == "DOM Volume Diff") { registeredId = 5; }
                        if (filter.Name == "Donchian Channel") { registeredId = 28; }
                        if (filter.Name == "Email Notify") { registeredId = 14; }
                        if (filter.Name == "H/L EMA") { registeredId = 2; }
                        if (filter.Name == "H/L SMA") { registeredId = 1; }
                        if (filter.Name == "H/L SMMA") { registeredId = 6; }
                        if (filter.Name == "Keltner Channels") { registeredId = 25; }
                        if (filter.Name == "MA Envelopes") { registeredId = 27; }
                        if (filter.Name == "MA Spread") { registeredId = 17; }
                        if (filter.Name == "MFI") { registeredId = 19; }
                        if (filter.Name == "NGA") { registeredId = 7; }
                        if (filter.Name == "OHLC+ Limit") { registeredId = 8; }
                        if (filter.Name == "Price Change") { registeredId = 3; }
                        if (filter.Name == "Price Limit") { registeredId = 4; }
                        if (filter.Name == "RSI") { registeredId = 11; }
                        if (filter.Name == "Spread") { registeredId = 16; }
                        if (filter.Name == "STARC Bands") { registeredId = 26; }
                        if (filter.Name == "Stoch") { registeredId = 12; }
                        if (filter.Name == "Stoch RSI") { registeredId = 13; }
                        if (filter.Name == "SuperTrend") { registeredId = 29; }
                        if (filter.Name == "Timer") { registeredId = 21; }
                        if (filter.Name == "URL") { registeredId = 15; }
                        if (filter.Name == "Volume Limit") { registeredId = 23; }

                        // Создаем объект
                        filters.Add(new Models.Filters.JsonFilter { Id = (int)registeredId, MyName = filter.MyName, Group = filter.Group, Weight = filter.Weight, Color = filter.Color });
                    }
                    // Передать Filters
                    AddFiltersEvent?.Invoke(filters, this.TargetPoint, this.FiltersSide, this.IsStopLoss, this.Step); // Филтры без настроек
                    //AddFiltersSettingsEvent?.Invoke(new List<IFilter>(PreparedFilters));  // Фильтры с настройками
                });
            }
        }

        // Сохраняем изменёный фильтр
        public ICommand SaveFilerClick
        {
            get
            {
                return new Command((obj) =>
                {
                    this.SelectedFilterName = SelectedFilter.Name;
                    DeleteFilterClick.Execute(null);
                    PrepareFilterClick.Execute(null);
                });
            }
        }

        // Создание объекта фильтра для списка фильтров с полными настройками для их отправки на сайт (для List<IFilter> PreparedFilters)
        private IFilter CreateFilter()
        {
            // H/L SMA
            if (this.SelectedFilterName == "H/L SMA")
            {
                int higherOrlower = 0;
                int period = 0;
                string timeFrame = "";
                decimal indent = 0;
                int duration = 0;
                string depthSide = "";

                higherOrlower = this.Mode;
                period = this.Period;
                timeFrame = this.TimeFrame;
                indent = this.Indent;
                duration = this.Duration;
                depthSide = this.DepthSide;

                var filter = new HLSMA(higherOrlower, period, timeFrame, indent, duration, depthSide);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // H/L EMA
            if (this.SelectedFilterName == "H/L EMA")
            {
                int higherOrlower = 0;
                int period = 0;
                string timeFrame = "";
                decimal indent = 0;
                int duration = 0;
                string depthSide = "";

                higherOrlower = this.Mode;
                period = this.Period;
                timeFrame = this.TimeFrame;
                indent = this.Indent;
                duration = this.Duration;
                depthSide = this.DepthSide;

                var filter = new HLEMA(higherOrlower, period, timeFrame, indent, duration, depthSide);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // Price Change
            if (this.SelectedFilterName == "Price Change")
            {
                string cur1 = "";
                string cur2 = "";
                int moreOrLess = 0;
                int side = 0;
                decimal priceValue = 0;
                int duration = 0;


                cur1 = this.Cur1;
                cur2 = this.Cur2;
                moreOrLess = this.Mode;
                side = this.Side;
                priceValue = this.PriceValue;
                duration = this.Duration;

                var filter = new PriceChange(cur1, cur2, moreOrLess, side, priceValue, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // Price Limit
            if (this.SelectedFilterName == "Price Limit")
            {
                int moreOrLess = 0;
                decimal priceLimitValue = 0;
                int duration = 0;
                string depthSide = "";

                moreOrLess = this.Mode;
                priceLimitValue = this.PriceLimitValue;
                duration = this.Duration;
                depthSide = this.DepthSide;

                var filter = new PriceLimit(moreOrLess, priceLimitValue, duration, depthSide);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // DOM Volume Diff
            if (this.SelectedFilterName == "DOM Volume Diff")
            {
                int moreOrLess = 0;
                int side = 0;
                int period = 0;
                decimal volumeValue = 0;
                int duration = 0;

                moreOrLess = this.Mode;
                side = this.Side;
                period = this.Period;
                volumeValue = this.VolumeValue;
                duration = this.Duration;

                var filter = new DOMVolumeDiff(moreOrLess, side, volumeValue, period, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // H/L SMMA
            if (this.SelectedFilterName == "H/L SMMA")
            {
                int higherOrlower = 0;
                int period = 0;
                string timeFrame = "";
                decimal indent = 0;
                int duration = 0;
                string depthSide = "";

                higherOrlower = this.Mode;
                period = this.Period;
                timeFrame = this.TimeFrame;
                indent = this.Indent;
                duration = this.Duration;
                depthSide = this.DepthSide;

                var filter = new HLSMMA(higherOrlower, period, timeFrame, indent, duration, depthSide);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }
            // NGA
            if (this.SelectedFilterName == "NGA")
            {
                int higherOrlower = 0;
                int period = 0;
                string timeFrame = "";
                decimal indent = 0;
                int duration = 0;
                string depthSide = "";

                higherOrlower = this.Mode;
                period = this.Period;
                timeFrame = this.TimeFrame;
                indent = this.Indent;
                duration = this.Duration;
                depthSide = this.DepthSide;

                var filter = new NGA(higherOrlower, period, timeFrame, indent, duration, depthSide);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }
            // OHLC+ Limit
            if (this.SelectedFilterName == "OHLC+ Limit")
            {
                int higherOrlower = 0;
                int source = 0;
                int priceType = 0;
                int period = 0;
                string timeFrame = "";
                decimal indent = 0;
                int duration = 0;
                string depthSide = "";

                higherOrlower = this.Mode;
                source = this.Source;
                priceType = this.PriceType;
                period = this.Period;
                timeFrame = this.TimeFrame;
                indent = this.Indent;
                duration = this.Duration;
                depthSide = this.DepthSide;

                var filter = new OHLCPlusLimit(higherOrlower, source, priceType, period, timeFrame, indent, duration, depthSide);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // Cross
            if (this.SelectedFilterName == "Cross")
            {
                int higherOrlower = 0;
                int line1 = 0;
                int period = 0;
                int line2 = 0;
                int period2 = 0;
                string timeFrame = "";
                int duration = 0;


                higherOrlower = this.Mode;
                line1 = this.Line1;
                period = this.Period;
                line2 = this.Line2;
                period2 = this.Period2;
                timeFrame = this.TimeFrame;
                duration = this.Duration;

                var filter = new Cross(higherOrlower, line1, period, line2, period2, timeFrame, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }
            // Bollinger Bands
            if (this.SelectedFilterName == "Bollinger Bands")
            {
                int higherOrlower = 0;
                decimal rate = 0;
                int period = 0;
                string timeFrame = "";
                decimal indent = 0;
                int duration = 0;
                string depthSide = "";

                higherOrlower = this.Mode;
                rate = this.Rate;
                period = this.Period;
                timeFrame = this.TimeFrame;
                indent = this.Indent;
                duration = this.Duration;
                depthSide = this.DepthSide;

                var filter = new BollingerBands(higherOrlower, period, rate, timeFrame, indent, duration, depthSide);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // RSI
            if (this.SelectedFilterName == "RSI")
            {
                int higherOrlower = 0;
                decimal priceValue = 0;
                int period = 0;
                string timeFrame = "";
                int duration = 0;

                higherOrlower = this.Mode;
                priceValue = this.PriceValue;
                period = this.Period;
                timeFrame = this.TimeFrame;
                duration = this.Duration;

                var filter = new RSI(higherOrlower, priceValue, period, timeFrame, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // Stoch
            if (this.SelectedFilterName == "Stoch")
            {
                int higherOrlower = 0;
                decimal priceValue = 0;
                int period = 0;
                int period2 = 0;
                int period3 = 0;
                string timeFrame = "";
                int duration = 0;

                higherOrlower = this.Mode;
                priceValue = this.PriceValue;
                period = this.Period;
                period2 = this.Period2;
                period3 = this.Period3;
                timeFrame = this.TimeFrame;
                duration = this.Duration;

                var filter = new Stoch(higherOrlower, priceValue, period, period2, period3, timeFrame, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // Stoch RSI
            if (this.SelectedFilterName == "Stoch RSI")
            {
                int higherOrlower = 0;
                decimal priceValue = 0;
                int period = 0;
                int period2 = 0;
                int period3 = 0;
                int period4 = 0;
                string timeFrame = "";
                int duration = 0;

                higherOrlower = this.Mode;
                priceValue = this.PriceValue;
                period = this.Period;
                period2 = this.Period2;
                period3 = this.Period3;
                period4 = this.Period4;
                timeFrame = this.TimeFrame;
                duration = this.Duration;

                var filter = new StochRSI(higherOrlower, priceValue, period, period2, period3, period4, timeFrame, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // Email Notify
            if (this.SelectedFilterName == "Email Notify")
            {
                string login = "";
                string password = "";
                string text = "";
                int duration = 0;

                login = this.GoogleLogin;
                password = this.GooglePassword;
                text = this.Text;
                duration = this.Duration;

                var filter = new EmailNotify(login, password, text, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // URL
            if (this.SelectedFilterName == "URL")
            {
                string url = "";
                int duration = 0;
                
                url = this.Text;
                duration = this.Duration;

                var filter = new URL(url, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // Spread
            if (this.SelectedFilterName == "Spread")
            {
                int mode = 0;
                decimal diff = 0;
                int duration = 0;

                mode = this.Mode;
                diff = this.Diff;
                duration = this.Duration;

                var filter = new Spread(mode, diff, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // MA Spread
            if (this.SelectedFilterName == "MA Spread")
            {
                int mode = 0;
                decimal diff = 0;
                int line1 = 0;
                int period = 0;
                int line2 = 0;
                int period2 = 0;
                string timeFrame = "";
                int duration = 0;


                mode = this.Mode;
                diff = this.Diff;
                line1 = this.Line1;
                period = this.Period;
                line2 = this.Line2;
                period2 = this.Period2;
                timeFrame = this.TimeFrame;
                duration = this.Duration;

                var filter = new MASpread(mode, diff, line1, period, line2, period2, timeFrame, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // Coin Price Change
            if (this.SelectedFilterName == "Candle Price Change")
            {
                string cur1 = "";
                string cur2 = "";
                int moreOrLess = 0;
                int side = 0;
                decimal priceValue = 0;
                int period = 2;
                string timeFrame = "";
                int duration = 0;

                cur1 = this.Cur1;
                cur2 = this.Cur2;
                moreOrLess = this.Mode;
                side = this.Side;
                priceValue = this.PriceValue;
                timeFrame = this.TimeFrame;
                duration = this.Duration;

                var filter = new CandlePriceChange(cur1, cur2, moreOrLess, side, priceValue, period, timeFrame, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // MFI
            if (this.SelectedFilterName == "MFI")
            {
                int higherOrlower = 0;
                decimal priceValue = 0;
                int period = 0;
                string timeFrame = "";
                int duration = 0;

                higherOrlower = this.Mode;
                priceValue = this.PriceValue;
                period = this.Period;
                timeFrame = this.TimeFrame;
                duration = this.Duration;

                var filter = new MFI(higherOrlower, priceValue, period, timeFrame, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // CCI
            if (this.SelectedFilterName == "CCI")
            {
                int higherOrlower = 0;
                decimal priceValue = 0;
                int period = 0;
                string timeFrame = "";
                int duration = 0;

                higherOrlower = this.Mode;
                priceValue = this.PriceValue;
                period = this.Period;
                timeFrame = this.TimeFrame;
                duration = this.Duration;

                var filter = new CCI(higherOrlower, priceValue, period, timeFrame, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // Timer
            if (this.SelectedFilterName == "Timer")
            {
                int mode = 0;
                int duration = 0;

                mode = this.Mode;
                duration = this.Duration;

                var filter = new Timer(mode, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // Candle Color
            if (this.SelectedFilterName == "Candle Color")
            {
                int mode = 0;
                int side = 0;
                int period = 0;
                string timeFrame = "";
                int duration = 0;

                mode = this.Mode;
                side = this.Side;
                period = this.Period;
                timeFrame = this.TimeFrame;
                duration = this.Duration;

                var filter = new CandleColor(mode, side, period, timeFrame, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // Volume Limit
            if (this.SelectedFilterName == "Volume Limit")
            {
                string cur1 = "";
                string cur2 = "";
                int mode = 0;
                decimal volumeValue = 0;
                int duration = 0;


                cur1 = this.Cur1;
                cur2 = this.Cur2;
                mode = this.Mode;
                volumeValue = this.VolumeValue;
                duration = this.Duration;

                var filter = new VolumeLimit(cur1, cur2, mode, volumeValue, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // Bollinger Bands
            if (this.SelectedFilterName == "BBW")
            {
                int higherOrlower = 0;
                decimal rate = 0;
                int period = 0;
                string timeFrame = "";
                decimal priceValue = 0;
                int duration = 0;

                higherOrlower = this.Mode;
                rate = this.Rate;
                period = this.Period;
                timeFrame = this.TimeFrame;
                priceValue = this.PriceValue;
                duration = this.Duration;

                var filter = new BollingerBandsWidth(higherOrlower, period, rate, timeFrame, priceValue, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // Keltner Channels
            if (this.SelectedFilterName == "Keltner Channels")
            {
                int mode = 0;
                decimal rate = 0;
                int period = 0;
                int period2 = 0;
                string timeFrame = "";
                decimal indent = 0;
                int duration = 0;
                
                mode = this.Mode;
                rate = this.Rate;
                period = this.Period;
                period2 = this.Period2;
                timeFrame = this.TimeFrame;
                indent = this.Indent;
                duration = this.Duration;

                var filter = new KeltnerChannels(mode, period, period2, rate, timeFrame, indent, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // STARC Bands
            if (this.SelectedFilterName == "STARC Bands")
            {
                int mode = 0;
                decimal rate = 0;
                int period = 0;
                int period2 = 0;
                string timeFrame = "";
                decimal indent = 0;
                int duration = 0;

                mode = this.Mode;
                rate = this.Rate;
                period = this.Period;
                period2 = this.Period2;
                timeFrame = this.TimeFrame;
                indent = this.Indent;
                duration = this.Duration;

                var filter = new STARCBands(mode, period, period2, rate, timeFrame, indent, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            // MA Envelopes
            if (this.SelectedFilterName == "MA Envelopes")
            {
                int mode = 0;
                decimal rate = 0;
                int period = 0;
                int source = 0;
                string timeFrame = "";
                decimal indent = 0;
                int duration = 0;

                mode = this.Mode;
                rate = this.Rate;
                period = this.Period;
                source = this.Source;
                timeFrame = this.TimeFrame;
                duration = this.Duration;

                var filter = new MAEnvelopes(mode, period, source, rate, timeFrame, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

			// Donchian Channel
			if (this.SelectedFilterName == "Donchian Channel")
			{
				int mode = 0;
				int period = 0;
				string timeFrame = "";
				decimal indent = 0;
				int duration = 0;

				mode = this.Mode;
				period = this.Period;
				timeFrame = this.TimeFrame;
				indent = this.Indent;
				duration = this.Duration;

				var filter = new DonchianChannel(mode, period, timeFrame, indent, duration);

				filter.MyName = this.MyName;
				filter.Group = this.Group;
				filter.Color = this.Color;
				filter.Weight = this.Weight;

				return filter;
			}

            // SuperTrend
            if (this.SelectedFilterName == "SuperTrend")
            {
                int mode = 0;
                decimal rate = 0;
                int period = 0;
                string timeFrame = "";
                decimal indent = 0;
                int duration = 0;

                mode = this.Mode;
                rate = this.Rate;
                period = this.Period;
                timeFrame = this.TimeFrame;
                indent = this.Indent;
                duration = this.Duration;

                var filter = new SuperTrend(mode, period, rate, timeFrame, indent, duration);

                filter.MyName = this.MyName;
                filter.Group = this.Group;
                filter.Color = this.Color;
                filter.Weight = this.Weight;

                return filter;
            }

            throw new Exception("code 27"); // Выбран неизвестный фильтр
        }

        // Пересчёт суммарных баллов по группам
        private void UpdateGroupWeights()
        {
            this.WeightGroupA = 0;
            this.WeightGroupB = 0;
            this.WeightGroupC = 0;
            foreach (var filter in PreparedFilters)
            {
                if (filter.Group == "A")
                    this.WeightGroupA += filter.Weight;
                if (filter.Group == "B")
                    this.WeightGroupB += filter.Weight;
                if (filter.Group == "C")
                    this.WeightGroupC += filter.Weight;
            }
        }

        // Задаём минимальный вес
        private void SetMinimalTargetPoint()
        {
            // TargetPoint по умолчанию минимальный вес из групп
            int min = this.WeightGroupA;

            if (this.WeightGroupB != 0)
                if (this.WeightGroupB < min)
                    min = this.WeightGroupB;

            if (this.WeightGroupC != 0)
                if (this.WeightGroupC < min)
                    min = this.WeightGroupC;

            this.TargetPoint = min;
        }

        // Открыть ссылку помощи по фильру
        public ICommand OpenHelpClick
        {
            get
            {
                return new Command((obj) =>
                {
                    try {System.Diagnostics.Process.Start(obj.ToString());}
                    catch { }
                });
            }
        }

    }
}
