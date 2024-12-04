using stratumbot.Core;
using stratumbot.DTO;
using stratumbot.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace stratumbot.ViewModels
{
    // Добавление нового пункта: создать поле, загрузка в SettingsVM, сохранение в ClickSave. -> Settings.cs: поле, иниц, сохранение
    class SettingsVM : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion



        // Общее

        public delegate void ProModeChange(bool proModeState); // ProMode и событие изменения состояния
        public static event ProModeChange ProModeChangeEvent;
        private bool proMode;
        public bool ProMode
        {
            get { return proMode; }
            set
            {
                proMode = value;
                ProModeChangeEvent(value);
                OnPropertyChanged();
            }
        }

        private string logsLenth;
        public string LogsLenth
        {
            get { return logsLenth; }
            set
            {
                logsLenth = value;
                OnPropertyChanged();
            }
        }

        private List<string> languages;
        public List<string> Languages
        {
            get { return languages; }
            set
            {
                languages = value;
                OnPropertyChanged();
            }
        }

        private string lang; // Выбранный язык
        public string Lang
        {
            get { return lang; }
            set
            {
                lang = value;
                OnPropertyChanged();
            }
        }

        private bool babloVoice;
        public bool BabloVoice
        {
            get { return babloVoice; }
            set
            {
                babloVoice = value;
                OnPropertyChanged();
            }
        }

        private bool debug;
        public bool Debug
        {
            get { return debug; }
            set
            {
                debug = value;
                OnPropertyChanged();
            }
        }

        // Таймауты

        public int checkTimeout;
        public int CheckTimeout
        {
            get { return checkTimeout; }
            set
            {
                checkTimeout = value;
                OnPropertyChanged();
            }
        }

        public int checkOrderTimeout;
        public int CheckOrderTimeout
        {
            get { return checkOrderTimeout; }
            set
            {
                checkOrderTimeout = value;
                OnPropertyChanged();
            }
        }

        public int betweenRequestTimeout;
        public int BetweenRequestTimeout
        {
            get { return betweenRequestTimeout; }
            set
            {
                betweenRequestTimeout = value;
                OnPropertyChanged();
            }
        }

        public int filtersTimeout;
        public int FiltersTimeout
        {
            get { return filtersTimeout; }
            set
            {
                filtersTimeout = value;
                OnPropertyChanged();
            }
        }

        public int recheckBuyFiltersTimeout;
        public int RecheckBuyFiltersTimeout
        {
            get { return recheckBuyFiltersTimeout; }
            set
            {
                recheckBuyFiltersTimeout = value;
                OnPropertyChanged();
            }
        }

        public int recheckSellFiltersTimeout;
        public int RecheckSellFiltersTimeout
        {
            get { return recheckSellFiltersTimeout; }
            set
            {
                recheckSellFiltersTimeout = value;
                OnPropertyChanged();
            }
        }


        // Биржи
        /* Временное решение, работает только с бинанс */
        public ObservableCollection<Tokens> APIBinance { get; set; } = new ObservableCollection<Tokens>(); // Список токенов для Binance

        public Tokens apiSelected; // Выделенные токены
        public Tokens APISelected
        {
            get { return apiSelected; }
            set
            {
                apiSelected = value;
                OnPropertyChanged();
            }
        }

        public string apiKey; // добавление
        public string APIKey
        {
            get { return apiKey; }
            set
            {
                apiKey = value;
                OnPropertyChanged();
            }
        }
        public string apiSecret; // добавление
        public string APISecret
        {
            get { return apiSecret; }
            set
            {
                apiSecret = value;
                OnPropertyChanged();
            }
        }


        // Binance Futures

        public ObservableCollection<Tokens> APIBinanceFutures { get; set; } = new ObservableCollection<Tokens>(); // Список токенов для Binance

#pragma warning disable CS0649 // Field 'SettingsVM.apiFuturesSelected' is never assigned to, and will always have its default value null
        public Tokens apiFuturesSelected; // Выделенные токены
#pragma warning restore CS0649 // Field 'SettingsVM.apiFuturesSelected' is never assigned to, and will always have its default value null
        public Tokens APIFuturesSelected
        {
            get { return apiSelected; }
            set
            {
                apiSelected = value;
                OnPropertyChanged();
            }
        }

        public string apiFuturesKey; // добавление
        public string APIFuturesKey
        {
            get { return apiFuturesKey; }
            set
            {
                apiFuturesKey = value;
                OnPropertyChanged();
            }
        }
        public string apiFuturesSecret; // добавление
        public string APIFuturesSecret
        {
            get { return apiFuturesSecret; }
            set
            {
                apiFuturesSecret = value;
                OnPropertyChanged();
            }
        }



        // Стратегии
        private bool isScalpingyAvailable; // Отображения для этой версии данной стратегии
        public bool IsScalpingyAvailable
        {
            get { return isScalpingyAvailable; }
            set
            {
                isScalpingyAvailable = value;
                OnPropertyChanged();
            }
        }
        private bool isArbitrageAvailable;
        public bool IsArbitrageAvailable
        {
            get { return isArbitrageAvailable; }
            set
            {
                isArbitrageAvailable = value;
                OnPropertyChanged();
            }
        }

        private bool isClassicLongAvailable; // Отображения для этой версии данной стратегии
        public bool IsClassicLongAvailable
        {
            get { return isClassicLongAvailable; }
            set
            {
                isClassicLongAvailable = value;
                OnPropertyChanged();
            }
        }

        // Скальпинг
        public string minSpreadScalpingAutofit; // % Мин.спред
        public string MinSpreadScalpingAutofit
        {
            get { return minSpreadScalpingAutofit; }
            set
            {
                minSpreadScalpingAutofit = value;
                OnPropertyChanged();
            }
        }

        public string optSpreadScalpingAutofit; // % Опт.спред
        public string OptSpreadScalpingAutofit
        {
            get { return optSpreadScalpingAutofit; }
            set
            {
                optSpreadScalpingAutofit = value;
                OnPropertyChanged();
            }
        }

        public string minMarkupScalpingAutofit; // % Мин.наценка
        public string MinMarkupScalpingAutofit
        {
            get { return minMarkupScalpingAutofit; }
            set
            {
                minMarkupScalpingAutofit = value;
                OnPropertyChanged();
            }
        }

        public string optMarkupScalpingAutofit; // % Опт.наценка
        public string OptMarkupScalpingAutofit
        {
            get { return optMarkupScalpingAutofit; }
            set
            {
                optMarkupScalpingAutofit = value;
                OnPropertyChanged();
            }
        }

        public string zeroSellScalpingAutofit; // % продажи в ноль
        public string ZeroSellScalpingAutofit
        {
            get { return zeroSellScalpingAutofit; }
            set
            {
                zeroSellScalpingAutofit = value;
                OnPropertyChanged();
            }
        }

        public string inTimeoutScalpingAutofit; // Время ожидания
        public string InTimeoutScalpingAutofit
        {
            get { return inTimeoutScalpingAutofit; }
            set
            {
                inTimeoutScalpingAutofit = value;
                OnPropertyChanged();
            }
        }


        public bool paramInPercentScalpingAutofit; // В процентах true, если в пунктах false
        public bool ParamInPercentScalpingAutofit
        {
            get { return paramInPercentScalpingAutofit; }
            set
            {
                paramInPercentScalpingAutofit = value;
                OnPropertyChanged();
            }
        }

        public bool isScalpingPercentAutofit; // В процентах true, не в процентах false
        public bool IsScalpingPercentAutofit
        {
            get { return isScalpingPercentAutofit; }
            set
            {
                isScalpingPercentAutofit = value;
                OnPropertyChanged();
            }
        }

        public bool isScalpingPointAutofit; // В пунктах true, не в процентах false
        public bool IsScalpingPointAutofit
        {
            get { return isScalpingPointAutofit; }
            set
            {
                isScalpingPointAutofit = value;
                OnPropertyChanged();
            }
        }

        public bool isDCA; // DCA включен ли
        public bool IsDCA
        {
            get { return isDCA; }
            set
            {
                isDCA = value;
                OnPropertyChanged();
            }
        }

        public string dcaProfitPercent; // DCA % профита
        public string DCAProfitPercent
        {
            get { return dcaProfitPercent; }
            set
            {
                dcaProfitPercent = value;
                OnPropertyChanged();
            }
        }

        private int dcaStepCount; // Количество DCA шагов
        public string DCAStepCount
        {
            get { return dcaStepCount.ToString(); }
            set
            {
                dcaStepCount = Conv.cleanInt(value);
                DCAStepsGenerate(); // Генерируем шаги, т.к. их кол-во изменилось
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string[]> DCASteps { get; set; } = new ObservableCollection<string[]>();// Хранит в себе настройки DCA шагов 

        // Метод генерации параметров шагов по умолчанию
        private void DCAStepsGenerate()
        {
            DCASteps.Clear();
            for (int i = 0; i < this.dcaStepCount; i++)
            {
                int val = i + 1;
                DCASteps.Add(new string[] { $"#{val}", $"{val}%", $"100%" });
            }
        }

        public string approximationPercent; // % аппроксимации (приближения, отдаления)
        public string ApproximationPercent
        {
            get { return approximationPercent; }
            set
            {
                approximationPercent = value;
                OnPropertyChanged();
            }
        }

        public int stopLossTimeout; // StopLoss Timeout
        public int StopLossTimeout
        {
            get { return stopLossTimeout; }
            set
            {
                stopLossTimeout = value;
                OnPropertyChanged();
            }
        }

        // Scalping

        public string stopAfterXStopLoss; // Остановка после X стоплоссов
        public string StopAfterXStopLoss
        {
            get { return stopAfterXStopLoss; }
            set
            {
                stopAfterXStopLoss = value;
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

        // Кабинет

        public string hash;
        public string Hash
        {
            get { return hash; }
            set
            {
                hash = value;
                OnPropertyChanged();
            }
        }

        // Лицензия

        public string license;
        public string License
        {
            get { return license; }
            set
            {
                license = value;
                OnPropertyChanged();
            }
        }

        public string paidTill;
        public string PaidTill
        {
            get { return paidTill; }
            set
            {
                paidTill = value;
                OnPropertyChanged();
            }
        }


        // Scalping Situations

        // [19] Buy Canceled Situation

        private int buyCanceledScalpingSituation;
        public int BuyCanceledScalpingSituation
        {
            get { return buyCanceledScalpingSituation; }
            set
            {
                buyCanceledScalpingSituation = value;
                OnPropertyChanged();
            }
        }

        // [20] Buy Little Filled Price Increased Situation

        private int buyLittleFilledPriceIncreasedScalpingSituation;
        public int BuyLittleFilledPriceIncreasedScalpingSituation
        {
            get { return buyLittleFilledPriceIncreasedScalpingSituation; }
            set
            {
                buyLittleFilledPriceIncreasedScalpingSituation = value;
                OnPropertyChanged();
            }
        }

        // [21] Buy Little Filled Canceled Situation

        private int buyLittleFilledCanceledScalpingSituation;
        public int BuyLittleFilledCanceledScalpingSituation
        {
            get { return buyLittleFilledCanceledScalpingSituation; }
            set
            {
                buyLittleFilledCanceledScalpingSituation = value;
                OnPropertyChanged();
            }
        }

        // [22] Sell Canceled Little Reminder Situation

        private int sellCanceledLittleReminderScalpingSituation;
        public int SellCanceledLittleReminderScalpingSituation
        {
            get { return sellCanceledLittleReminderScalpingSituation; }
            set
            {
                sellCanceledLittleReminderScalpingSituation = value;
                OnPropertyChanged();
            }
        }

        // [23] Sell Little Reminder Price Dropped Situation

        private int sellLittleReminderPriceDroppedScalpingSituation;
        public int SellLittleReminderPriceDroppedScalpingSituation
        {
            get { return sellLittleReminderPriceDroppedScalpingSituation; }
            set
            {
                sellLittleReminderPriceDroppedScalpingSituation = value;
                OnPropertyChanged();
            }
        }


        // [24] Four waiting conditions for [20] Buy Little Filled Price Increased Situation

        private int xOrdersAheadLittleFilledPriceIncreasedScalpingSituation;
        public int XOrdersAheadLittleFilledPriceIncreasedScalpingSituation
        {
            get { return xOrdersAheadLittleFilledPriceIncreasedScalpingSituation; }
            set
            {
                xOrdersAheadLittleFilledPriceIncreasedScalpingSituation = value;
                OnPropertyChanged();
            }
        }

        private int secondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation;
        public int SecondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation
        {
            get { return secondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation; }
            set
            {
                secondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation = value;
                OnPropertyChanged();
            }
        }

        private decimal dropPercentLittleFilledPriceIncreasedScalpingSituation;
        public decimal DropPercentLittleFilledPriceIncreasedScalpingSituation
        {
            get { return dropPercentLittleFilledPriceIncreasedScalpingSituation; }
            set
            {
                dropPercentLittleFilledPriceIncreasedScalpingSituation = value;
                OnPropertyChanged();
            }
        }

        private decimal aheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation;
        public decimal AheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation
        {
            get { return aheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation; }
            set
            {
                aheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation = value;
                OnPropertyChanged();
            }
        }

        // [25] Four waiting conditions for [26] Buy Filled Enough Price Increased

        private int xOrdersAheadBuyFilledEnoughPriceIncreasedScalping;
        public int XOrdersAheadBuyFilledEnoughPriceIncreasedScalping
        {
            get { return xOrdersAheadBuyFilledEnoughPriceIncreasedScalping; }
            set
            {
                xOrdersAheadBuyFilledEnoughPriceIncreasedScalping = value;
                OnPropertyChanged();
            }
        }

        private int secondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping;
        public int SecondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping
        {
            get { return secondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping; }
            set
            {
                secondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping = value;
                OnPropertyChanged();
            }
        }

        private decimal dropPercentBuyFilledEnoughPriceIncreasedScalping;
        public decimal DropPercentBuyFilledEnoughPriceIncreasedScalping
        {
            get { return dropPercentBuyFilledEnoughPriceIncreasedScalping; }
            set
            {
                dropPercentBuyFilledEnoughPriceIncreasedScalping = value;
                OnPropertyChanged();
            }
        }

        private decimal aheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping;
        public decimal AheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping
        {
            get { return aheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping; }
            set
            {
                aheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping = value;
                OnPropertyChanged();
            }
        }

        // [27] Four waiting conditions for [23] Sell Little Reminder Price Dropped Situation

        private int xOrdersAheadSellLittleReminderPriceDroppedScalpingSituation;
        public int XOrdersAheadSellLittleReminderPriceDroppedScalpingSituation
        {
            get { return xOrdersAheadSellLittleReminderPriceDroppedScalpingSituation; }
            set
            {
                xOrdersAheadSellLittleReminderPriceDroppedScalpingSituation = value;
                OnPropertyChanged();
            }
        }

        private int secondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation;
        public int SecondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation
        {
            get { return secondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation; }
            set
            {
                secondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation = value;
                OnPropertyChanged();
            }
        }

        private decimal dropPercentSellLittleReminderPriceDroppedScalpingSituation;
        public decimal DropPercentSellLittleReminderPriceDroppedScalpingSituation
        {
            get { return dropPercentSellLittleReminderPriceDroppedScalpingSituation; }
            set
            {
                dropPercentSellLittleReminderPriceDroppedScalpingSituation = value;
                OnPropertyChanged();
            }
        }

        private decimal aheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation;
        public decimal AheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation
        {
            get { return aheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation; }
            set
            {
                aheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation = value;
                OnPropertyChanged();
            }
        }


        // Classic Long Situations

        // [1] Buy Canceled Situation

        private int buyCanceledClassicLongSituation;
		public int BuyCanceledClassicLongSituation
		{
			get { return buyCanceledClassicLongSituation; }
			set
			{
				buyCanceledClassicLongSituation = value;
				OnPropertyChanged();
			}
		}

		// [2] Buy Little Filled Price Increased Situation

		private int buyLittleFilledPriceIncreasedClassicLongSituation;
		public int BuyLittleFilledPriceIncreasedClassicLongSituation
		{
			get { return buyLittleFilledPriceIncreasedClassicLongSituation; }
			set
			{
				buyLittleFilledPriceIncreasedClassicLongSituation = value;
				OnPropertyChanged();
			}
		}

		// [3] Buy Little Filled Canceled Situation

		private int buyLittleFilledCanceledClassicLongSituation;
		public int BuyLittleFilledCanceledClassicLongSituation
		{
			get { return buyLittleFilledCanceledClassicLongSituation; }
			set
			{
				buyLittleFilledCanceledClassicLongSituation = value;
				OnPropertyChanged();
			}
		}

		// [4] Sell Canceled Little Reminder Situation

		private int sellCanceledLittleReminderClassicLongSituation;
		public int SellCanceledLittleReminderClassicLongSituation
		{
			get { return sellCanceledLittleReminderClassicLongSituation; }
			set
			{
				sellCanceledLittleReminderClassicLongSituation = value;
				OnPropertyChanged();
			}
		}

		// [5] Sell Little Reminder Price Dropped Situation

		private int sellLittleReminderPriceDroppedClassicLongSituation;
		public int SellLittleReminderPriceDroppedClassicLongSituation
		{
			get { return sellLittleReminderPriceDroppedClassicLongSituation; }
			set
			{
				sellLittleReminderPriceDroppedClassicLongSituation = value;
				OnPropertyChanged();
			}
		}


		// [6] Four waiting conditions for [2] Buy Little Filled Price Increased Situation

		private int xOrdersAheadLittleFilledPriceIncreasedClassicLongSituation;
		public int XOrdersAheadLittleFilledPriceIncreasedClassicLongSituation
		{
			get { return xOrdersAheadLittleFilledPriceIncreasedClassicLongSituation; }
			set
			{
				xOrdersAheadLittleFilledPriceIncreasedClassicLongSituation = value;
				OnPropertyChanged();
			}
		}

		private int secondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation;
		public int SecondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation
		{
			get { return secondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation; }
			set
			{
				secondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation = value;
				OnPropertyChanged();
			}
		}

		private decimal dropPercentLittleFilledPriceIncreasedClassicLongSituation;
		public decimal DropPercentLittleFilledPriceIncreasedClassicLongSituation
		{
			get { return dropPercentLittleFilledPriceIncreasedClassicLongSituation; }
			set
			{
				dropPercentLittleFilledPriceIncreasedClassicLongSituation = value;
				OnPropertyChanged();
			}
		}

		private decimal aheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation;
		public decimal AheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation
		{
			get { return aheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation; }
			set
			{
				aheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation = value;
				OnPropertyChanged();
			}
		}

		// [7] Four waiting conditions for [8] Buy Filled Enough Price Increased

		private int xOrdersAheadBuyFilledEnoughPriceIncreased;
		public int XOrdersAheadBuyFilledEnoughPriceIncreased
		{
			get { return xOrdersAheadBuyFilledEnoughPriceIncreased; }
			set
			{
				xOrdersAheadBuyFilledEnoughPriceIncreased = value;
				OnPropertyChanged();
			}
		}

		private int secondsAfterLastUpdateBuyFilledEnoughPriceIncreased;
		public int SecondsAfterLastUpdateBuyFilledEnoughPriceIncreased
		{
			get { return secondsAfterLastUpdateBuyFilledEnoughPriceIncreased; }
			set
			{
				secondsAfterLastUpdateBuyFilledEnoughPriceIncreased = value;
				OnPropertyChanged();
			}
		}

		private decimal dropPercentBuyFilledEnoughPriceIncreased;
		public decimal DropPercentBuyFilledEnoughPriceIncreased
		{
			get { return dropPercentBuyFilledEnoughPriceIncreased; }
			set
			{
				dropPercentBuyFilledEnoughPriceIncreased = value;
				OnPropertyChanged();
			}
		}

		private decimal aheadOrdersVolumeBuyFilledEnoughPriceIncreased;
		public decimal AheadOrdersVolumeBuyFilledEnoughPriceIncreased
		{
			get { return aheadOrdersVolumeBuyFilledEnoughPriceIncreased; }
			set
			{
				aheadOrdersVolumeBuyFilledEnoughPriceIncreased = value;
				OnPropertyChanged();
			}
		}

		// [9] Four waiting conditions for [5] Sell Little Reminder Price Dropped Situation

		private int xOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation;
		public int XOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation
		{
			get { return xOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation; }
			set
			{
				xOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation = value;
				OnPropertyChanged();
			}
		}

		private int secondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation;
		public int SecondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation
		{
			get { return secondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation; }
			set
			{
				secondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation = value;
				OnPropertyChanged();
			}
		}

		private decimal dropPercentSellLittleReminderPriceDroppedClassicLongSituation;
		public decimal DropPercentSellLittleReminderPriceDroppedClassicLongSituation
		{
			get { return dropPercentSellLittleReminderPriceDroppedClassicLongSituation; }
			set
			{
				dropPercentSellLittleReminderPriceDroppedClassicLongSituation = value;
				OnPropertyChanged();
			}
		}

		private decimal aheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation;
		public decimal AheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation
		{
			get { return aheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation; }
			set
			{
				aheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation = value;
				OnPropertyChanged();
			}
		}


        // Classic Short Situations

        // [10] Sell Canceled Situation

        private int sellCanceledClassicShortSituation;
        public int SellCanceledClassicShortSituation
        {
            get { return sellCanceledClassicShortSituation; }
            set
            {
                sellCanceledClassicShortSituation = value;
                OnPropertyChanged();
            }
        }

        // [11] Sell Little Filled Price Dropped Situation

        private int sellLittleFilledPriceDroppedClassicShortSituation;
        public int SellLittleFilledPriceDroppedClassicShortSituation
        {
            get { return sellLittleFilledPriceDroppedClassicShortSituation; }
            set
            {
                sellLittleFilledPriceDroppedClassicShortSituation = value;
                OnPropertyChanged();
            }
        }

        // [12] Sell Little Filled Canceled Situation

        private int sellLittleFilledCanceledClassicShortSituation;
        public int SellLittleFilledCanceledClassicShortSituation
        {
            get { return sellLittleFilledCanceledClassicShortSituation; }
            set
            {
                sellLittleFilledCanceledClassicShortSituation = value;
                OnPropertyChanged();
            }
        }

        // [13] Buy Canceled Little Reminder Situation

        private int buyCanceledLittleReminderClassicShortSituation;
        public int BuyCanceledLittleReminderClassicShortSituation
        {
            get { return buyCanceledLittleReminderClassicShortSituation; }
            set
            {
                buyCanceledLittleReminderClassicShortSituation = value;
                OnPropertyChanged();
            }
        }

        // [14] Buy Little Reminder Price Increased Situation

        private int buyLittleReminderPriceIncreasedClassicShortSituation;
        public int BuyLittleReminderPriceIncreasedClassicShortSituation
        {
            get { return buyLittleReminderPriceIncreasedClassicShortSituation; }
            set
            {
                buyLittleReminderPriceIncreasedClassicShortSituation = value;
                OnPropertyChanged();
            }
        }

        // [15] Four waiting conditions for [11] Sell Little Filled Price Dropped Situation

        private int xOrdersAheadLittleFilledPriceDroppedClassicShortSituation;
        public int XOrdersAheadLittleFilledPriceDroppedClassicShortSituation
        {
            get { return xOrdersAheadLittleFilledPriceDroppedClassicShortSituation; }
            set
            {
                xOrdersAheadLittleFilledPriceDroppedClassicShortSituation = value;
                OnPropertyChanged();
            }
        }

        private int secondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation;
        public int SecondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation
        {
            get { return secondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation; }
            set
            {
                secondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation = value;
                OnPropertyChanged();
            }
        }

        private decimal dropPercentLittleFilledPriceDroppedClassicShortSituation;
        public decimal DropPercentLittleFilledPriceDroppedClassicShortSituation
        {
            get { return dropPercentLittleFilledPriceDroppedClassicShortSituation; }
            set
            {
                dropPercentLittleFilledPriceDroppedClassicShortSituation = value;
                OnPropertyChanged();
            }
        }

        private decimal behindOrdersVolumeLittleFilledPriceDroppedClassicShortSituation;
        public decimal AheadOrdersVolumeLittleFilledPriceDroppedClassicShortSituation
        {
            get { return behindOrdersVolumeLittleFilledPriceDroppedClassicShortSituation; }
            set
            {
                behindOrdersVolumeLittleFilledPriceDroppedClassicShortSituation = value;
                OnPropertyChanged();
            }
        }

        // [16] Four waiting conditions for [17] Sell Filled Enough Price Dropped

        private int xOrdersAheadSellFilledEnoughPriceDropped;
        public int XOrdersAheadSellFilledEnoughPriceDropped
        {
            get { return xOrdersAheadSellFilledEnoughPriceDropped; }
            set
            {
                xOrdersAheadSellFilledEnoughPriceDropped = value;
                OnPropertyChanged();
            }
        }

        private int secondsAfterLastUpdateSellFilledEnoughPriceDropped;
        public int SecondsAfterLastUpdateSellFilledEnoughPriceDropped
        {
            get { return secondsAfterLastUpdateSellFilledEnoughPriceDropped; }
            set
            {
                secondsAfterLastUpdateSellFilledEnoughPriceDropped = value;
                OnPropertyChanged();
            }
        }

        private decimal dropPercentSellFilledEnoughPriceDropped;
        public decimal DropPercentSellFilledEnoughPriceDropped
        {
            get { return dropPercentSellFilledEnoughPriceDropped; }
            set
            {
                dropPercentSellFilledEnoughPriceDropped = value;
                OnPropertyChanged();
            }
        }

        private decimal behindOrdersVolumeSellFilledEnoughPriceDropped;
        public decimal AheadOrdersVolumeSellFilledEnoughPriceDropped
        {
            get { return behindOrdersVolumeSellFilledEnoughPriceDropped; }
            set
            {
                behindOrdersVolumeSellFilledEnoughPriceDropped = value;
                OnPropertyChanged();
            }
        }

        // [18] Four waiting conditions for [14] Buy Little Reminder Price Increased Situation

        private int xOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation;
        public int XOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation
        {
            get { return xOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation; }
            set
            {
                xOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation = value;
                OnPropertyChanged();
            }
        }

        private int secondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation;
        public int SecondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation
        {
            get { return secondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation; }
            set
            {
                secondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation = value;
                OnPropertyChanged();
            }
        }

        private decimal dropPercentSellLittleReminderPriceIncreasedClassicShortSituation;
        public decimal DropPercentSellLittleReminderPriceIncreasedClassicShortSituation
        {
            get { return dropPercentSellLittleReminderPriceIncreasedClassicShortSituation; }
            set
            {
                dropPercentSellLittleReminderPriceIncreasedClassicShortSituation = value;
                OnPropertyChanged();
            }
        }

        private decimal aheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation;
        public decimal AheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation
        {
            get { return aheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation; }
            set
            {
                aheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation = value;
                OnPropertyChanged();
            }
        }



        // Загрузка окна и настроек
        public SettingsVM()
        {
            // Общее 
            this.ProMode = Settings.ProMode;
            this.LogsLenth = Settings.LogsLenth.ToString();
            this.Languages = Settings.Languages;
            this.Lang = Settings.LanguagesCode[Settings.Lang];
            this.BabloVoice = Settings.BabloVoice;
            this.Debug = Settings.Debug;

            // Таймауты
            this.CheckTimeout = Settings.CheckTimeout;
            this.CheckOrderTimeout = Settings.CheckOrderTimeout;
            this.BetweenRequestTimeout = Settings.BetweenRequestTimeout;
            this.FiltersTimeout = Settings.FiltersTimeout;
            this.RecheckBuyFiltersTimeout = Settings.RecheckBuyFiltersTimeout;
            this.RecheckSellFiltersTimeout = Settings.RecheckSellFiltersTimeout;

            // Биржи
            // Binance
            if (Settings.API.Any(x => x.Exchange == Exchange.Binance))
            {
                foreach (var tokens in Settings.API.First(x => x.Exchange == Exchange.Binance).Tokens)
                {
                    this.APIBinance.Add(tokens);
                }
            }
            // Binance Futures
            if (Settings.API.Any(x => x.Exchange == Exchange.BinanceFutures))
            {
                foreach (var tokens in Settings.API.First(x => x.Exchange == Exchange.BinanceFutures).Tokens)
                {
                    this.APIBinanceFutures.Add(tokens);
                }
            }
            // Стратегии

            // Получение всех доступных стратегий
            this.IsScalpingyAvailable = false;
            this.IsArbitrageAvailable = false;
            this.IsClassicLongAvailable = false;
            AvailableStrategies.GetAll().ForEach(x => 
            {
                if (x.Id == Strategy.Scalping)
                    this.IsScalpingyAvailable = true;
                if (x.Id == Strategy.Arbitrage)
                    this.IsArbitrageAvailable = true;
                if (x.Id == Strategy.ClassicLong)
                    this.IsClassicLongAvailable = true;
            });

            // Скальпинг
            if(this.IsScalpingyAvailable)
            {
                this.MinSpreadScalpingAutofit = Settings.MinSpreadScalpingAutofit;
                this.OptSpreadScalpingAutofit = Settings.OptSpreadScalpingAutofit;
                this.MinMarkupScalpingAutofit = Settings.MinMarkupScalpingAutofit;
                this.OptMarkupScalpingAutofit = Settings.OptMarkupScalpingAutofit;
                this.ZeroSellScalpingAutofit = Settings.ZeroSellScalpingAutofit;
                this.InTimeoutScalpingAutofit = Settings.InTimeoutScalpingAutofit;
                this.ParamInPercentScalpingAutofit = Settings.ParamInPercentScalpingAutofit;
            
                if(this.ParamInPercentScalpingAutofit)
                {
                    this.IsScalpingPercentAutofit = true;
                    this.IsScalpingPointAutofit = false;
                } else
                {
                    this.IsScalpingPercentAutofit = false;
                    this.IsScalpingPointAutofit = true;
                }
            }
            this.IsDCA = Settings.IsDCAAutofit;
            this.DCAProfitPercent = Settings.DCAProfitPercentAutofit;
            this.DCAStepCount = Settings.DCAStepCountAutofit;
            this.DCASteps.Clear();
            foreach (var dcaStep in Settings.DCAStepsAutofit)
            {
                DCASteps.Add(new string[] { dcaStep[0], dcaStep[1], dcaStep[2] });
            }
            this.ApproximationPercent = Settings.ApproximationPercent;
            this.StopLossTimeout = Settings.StopLossTimeout;

            // Scalping
            this.BuyCanceledScalpingSituation = Settings.BuyCanceledScalpingSituation;
            this.BuyLittleFilledCanceledScalpingSituation = Settings.BuyLittleFilledCanceledScalpingSituation;
            this.BuyLittleFilledPriceIncreasedScalpingSituation = Settings.BuyLittleFilledPriceIncreasedScalpingSituation;
            this.SellCanceledLittleReminderScalpingSituation = Settings.SellCanceledLittleReminderScalpingSituation;
            this.SellLittleReminderPriceDroppedScalpingSituation = Settings.SellLittleReminderPriceDroppedScalpingSituation;

            this.XOrdersAheadLittleFilledPriceIncreasedScalpingSituation = Settings.XOrdersAheadLittleFilledPriceIncreasedScalpingSituation;
            this.SecondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation = Settings.SecondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation;
            this.DropPercentLittleFilledPriceIncreasedScalpingSituation = Settings.DropPercentLittleFilledPriceIncreasedScalpingSituation;
            this.AheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation = Settings.AheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation;

            this.XOrdersAheadBuyFilledEnoughPriceIncreasedScalping = Settings.XOrdersAheadBuyFilledEnoughPriceIncreasedScalping;
            this.SecondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping = Settings.SecondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping;
            this.DropPercentBuyFilledEnoughPriceIncreasedScalping = Settings.DropPercentBuyFilledEnoughPriceIncreasedScalping;
            this.AheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping = Settings.AheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping;

            this.XOrdersAheadSellLittleReminderPriceDroppedScalpingSituation = Settings.XOrdersAheadSellLittleReminderPriceDroppedScalpingSituation;
            this.SecondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation = Settings.SecondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation;
            this.DropPercentSellLittleReminderPriceDroppedScalpingSituation = Settings.DropPercentSellLittleReminderPriceDroppedScalpingSituation;
            this.AheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation = Settings.AheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation;


            // Classic Long
            this.BuyCanceledClassicLongSituation = Settings.BuyCanceledClassicLongSituation;
			this.BuyLittleFilledCanceledClassicLongSituation = Settings.BuyLittleFilledCanceledClassicLongSituation;
			this.BuyLittleFilledPriceIncreasedClassicLongSituation = Settings.BuyLittleFilledPriceIncreasedClassicLongSituation;
			this.SellCanceledLittleReminderClassicLongSituation = Settings.SellCanceledLittleReminderClassicLongSituation;
			this.SellLittleReminderPriceDroppedClassicLongSituation = Settings.SellLittleReminderPriceDroppedClassicLongSituation;

			this.XOrdersAheadLittleFilledPriceIncreasedClassicLongSituation = Settings.XOrdersAheadLittleFilledPriceIncreasedClassicLongSituation;
			this.SecondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation = Settings.SecondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation;
			this.DropPercentLittleFilledPriceIncreasedClassicLongSituation = Settings.DropPercentLittleFilledPriceIncreasedClassicLongSituation;
			this.AheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation = Settings.AheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation;

			this.XOrdersAheadBuyFilledEnoughPriceIncreased = Settings.XOrdersAheadBuyFilledEnoughPriceIncreased;
			this.SecondsAfterLastUpdateBuyFilledEnoughPriceIncreased = Settings.SecondsAfterLastUpdateBuyFilledEnoughPriceIncreased;
			this.DropPercentBuyFilledEnoughPriceIncreased = Settings.DropPercentBuyFilledEnoughPriceIncreased;
			this.AheadOrdersVolumeBuyFilledEnoughPriceIncreased = Settings.AheadOrdersVolumeBuyFilledEnoughPriceIncreased;

			this.XOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation = Settings.XOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation;
			this.SecondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation = Settings.SecondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation;
			this.DropPercentSellLittleReminderPriceDroppedClassicLongSituation = Settings.DropPercentSellLittleReminderPriceDroppedClassicLongSituation;
			this.AheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation = Settings.AheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation;

            // Classic Short
            this.SellCanceledClassicShortSituation = Settings.SellCanceledClassicShortSituation;
            this.SellLittleFilledPriceDroppedClassicShortSituation = Settings.SellLittleFilledPriceDroppedClassicShortSituation;
            this.SellLittleFilledCanceledClassicShortSituation = Settings.SellLittleFilledCanceledClassicShortSituation;
            this.BuyCanceledLittleReminderClassicShortSituation = Settings.BuyCanceledLittleReminderClassicShortSituation;
            this.BuyLittleReminderPriceIncreasedClassicShortSituation = Settings.BuyLittleReminderPriceIncreasedClassicShortSituation;

            this.XOrdersAheadLittleFilledPriceDroppedClassicShortSituation = Settings.XOrdersAheadLittleFilledPriceDroppedClassicShortSituation;
            this.SecondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation = Settings.SecondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation;
            this.DropPercentLittleFilledPriceDroppedClassicShortSituation = Settings.DropPercentLittleFilledPriceDroppedClassicShortSituation;
            this.AheadOrdersVolumeLittleFilledPriceDroppedClassicShortSituation = Settings.AheadOrdersVolumeLittleFilledPriceDroppedClassicShortSituation;

            this.XOrdersAheadSellFilledEnoughPriceDropped = Settings.XOrdersAheadSellFilledEnoughPriceDropped;
            this.SecondsAfterLastUpdateSellFilledEnoughPriceDropped = Settings.SecondsAfterLastUpdateSellFilledEnoughPriceDropped;
            this.DropPercentSellFilledEnoughPriceDropped = Settings.DropPercentSellFilledEnoughPriceDropped;
            this.AheadOrdersVolumeSellFilledEnoughPriceDropped = Settings.AheadOrdersVolumeSellFilledEnoughPriceDropped;

            this.XOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation = Settings.XOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation;
            this.SecondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation = Settings.SecondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation;
            this.DropPercentSellLittleReminderPriceIncreasedClassicShortSituation = Settings.DropPercentSellLittleReminderPriceIncreasedClassicShortSituation;
            this.AheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation = Settings.AheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation;

            // Общее
            this.StopAfterXStopLoss = Settings.StopAfterXStopLoss;

            // Данные
            this.GoogleLogin = Settings.GoogleLogin;
            this.GooglePassword = Settings.GooglePassword;

            // Кабинет
            this.Hash = Device.Hash;

            // Лицензия
            this.License = Models.License.LicenseHash;
            this.PaidTill = Models.License.PaidTill;

        }

        // Кнопка Сохранить настройки
        public ICommand ClickSave
        {
            get
            {
                return new Command((obj) =>
                {
                    // Общее
                    Settings.ProMode = this.ProMode;
                    Settings.LogsLenth = int.Parse(this.LogsLenth);
                    Settings.Lang = Settings.LanguagesCode[this.Lang];
                    Settings.BabloVoice = this.BabloVoice;
                    Settings.Debug = this.Debug;
                    // Таймауты
                    Settings.CheckTimeout = this.CheckTimeout;
                    Settings.CheckOrderTimeout = this.CheckOrderTimeout;
                    Settings.BetweenRequestTimeout = this.BetweenRequestTimeout;
                    Settings.FiltersTimeout = this.FiltersTimeout;
                    Settings.RecheckBuyFiltersTimeout = this.RecheckBuyFiltersTimeout;
                    Settings.RecheckSellFiltersTimeout = this.RecheckSellFiltersTimeout;
                    // Биржи
                    Settings.API.Clear();
                    // Binance
                    List<Tokens> Tokens = new List<Tokens>();
                    var APITokens = new APITokens();
                    foreach (var tokens in APIBinance)
                    {
                        Tokens.Add(tokens);
                    }
                    APITokens.Exchange = Exchange.Binance;
                    APITokens.Tokens = Tokens;
                    Settings.API.Add(APITokens);
                    // Binance Futures
                    Tokens = new List<Tokens>();
                    APITokens = new APITokens();
                    foreach (var tokens in APIBinanceFutures)
                    {
                        Tokens.Add(tokens);
                    }
                    APITokens.Exchange = Exchange.BinanceFutures;
                    APITokens.Tokens = Tokens;
                    Settings.API.Add(APITokens);
                    // Стратегии
                    // Скальпинг
                    if (this.IsScalpingyAvailable)
                    {
                        Settings.MinSpreadScalpingAutofit = this.MinSpreadScalpingAutofit;
                        Settings.OptSpreadScalpingAutofit = this.OptSpreadScalpingAutofit;
                        Settings.MinMarkupScalpingAutofit = this.MinMarkupScalpingAutofit;
                        Settings.OptMarkupScalpingAutofit = this.OptMarkupScalpingAutofit;
                        Settings.ZeroSellScalpingAutofit = this.ZeroSellScalpingAutofit;
                        Settings.InTimeoutScalpingAutofit = this.InTimeoutScalpingAutofit;
                    
                        if (this.IsScalpingPercentAutofit)
                        {
                            Settings.ParamInPercentScalpingAutofit = true;
                        } else
                        {
                            Settings.ParamInPercentScalpingAutofit = false;
                        }
                    }
                    Settings.IsDCAAutofit = this.IsDCA;
                    Settings.DCAProfitPercentAutofit = this.DCAProfitPercent;
                    Settings.DCAStepCountAutofit = this.DCAStepCount;
                    Settings.DCAStepsAutofit.Clear();
                    foreach (var dcaStep in this.DCASteps)
                    {
                        Settings.DCAStepsAutofit.Add(new string[] { dcaStep[0], dcaStep[1], dcaStep[2] });
                    }
                    Settings.ApproximationPercent = this.ApproximationPercent;
                    Settings.StopLossTimeout = this.StopLossTimeout;

                    // Scalping

                    Settings.BuyCanceledScalpingSituation = this.BuyCanceledScalpingSituation;
                    Settings.BuyLittleFilledCanceledScalpingSituation = this.BuyLittleFilledCanceledScalpingSituation;
                    Settings.BuyLittleFilledPriceIncreasedScalpingSituation = this.BuyLittleFilledPriceIncreasedScalpingSituation;
                    Settings.SellCanceledLittleReminderScalpingSituation = this.SellCanceledLittleReminderScalpingSituation;
                    Settings.SellLittleReminderPriceDroppedScalpingSituation = this.SellLittleReminderPriceDroppedScalpingSituation;

                    Settings.XOrdersAheadLittleFilledPriceIncreasedScalpingSituation = this.XOrdersAheadLittleFilledPriceIncreasedScalpingSituation;
                    Settings.SecondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation = this.SecondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation;
                    Settings.DropPercentLittleFilledPriceIncreasedScalpingSituation = this.DropPercentLittleFilledPriceIncreasedScalpingSituation;
                    Settings.AheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation = this.AheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation;

                    Settings.XOrdersAheadBuyFilledEnoughPriceIncreasedScalping = this.XOrdersAheadBuyFilledEnoughPriceIncreasedScalping;
                    Settings.SecondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping = this.SecondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping;
                    Settings.DropPercentBuyFilledEnoughPriceIncreasedScalping = this.DropPercentBuyFilledEnoughPriceIncreasedScalping;
                    Settings.AheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping = this.AheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping;

                    Settings.XOrdersAheadSellLittleReminderPriceDroppedScalpingSituation = this.XOrdersAheadSellLittleReminderPriceDroppedScalpingSituation;
                    Settings.SecondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation = this.SecondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation;
                    Settings.DropPercentSellLittleReminderPriceDroppedScalpingSituation = this.DropPercentSellLittleReminderPriceDroppedScalpingSituation;
                    Settings.AheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation = this.AheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation;

                    // Classic Long

                    Settings.BuyCanceledClassicLongSituation = this.BuyCanceledClassicLongSituation;
					Settings.BuyLittleFilledCanceledClassicLongSituation = this.BuyLittleFilledCanceledClassicLongSituation;
					Settings.BuyLittleFilledPriceIncreasedClassicLongSituation = this.BuyLittleFilledPriceIncreasedClassicLongSituation;
					Settings.SellCanceledLittleReminderClassicLongSituation = this.SellCanceledLittleReminderClassicLongSituation;
					Settings.SellLittleReminderPriceDroppedClassicLongSituation = this.SellLittleReminderPriceDroppedClassicLongSituation;

					Settings.XOrdersAheadLittleFilledPriceIncreasedClassicLongSituation = this.XOrdersAheadLittleFilledPriceIncreasedClassicLongSituation;
					Settings.SecondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation = this.SecondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation;
					Settings.DropPercentLittleFilledPriceIncreasedClassicLongSituation = this.DropPercentLittleFilledPriceIncreasedClassicLongSituation;
					Settings.AheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation = this.AheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation;

					Settings.XOrdersAheadBuyFilledEnoughPriceIncreased = this.XOrdersAheadBuyFilledEnoughPriceIncreased;
					Settings.SecondsAfterLastUpdateBuyFilledEnoughPriceIncreased = this.SecondsAfterLastUpdateBuyFilledEnoughPriceIncreased;
					Settings.DropPercentBuyFilledEnoughPriceIncreased = this.DropPercentBuyFilledEnoughPriceIncreased;
					Settings.AheadOrdersVolumeBuyFilledEnoughPriceIncreased = this.AheadOrdersVolumeBuyFilledEnoughPriceIncreased;

					Settings.XOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation = this.XOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation;
					Settings.SecondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation = this.SecondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation;
					Settings.DropPercentSellLittleReminderPriceDroppedClassicLongSituation = this.DropPercentSellLittleReminderPriceDroppedClassicLongSituation;
					Settings.AheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation = this.AheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation;

                    // Classic Short

                    Settings.SellCanceledClassicShortSituation = this.SellCanceledClassicShortSituation;
                    Settings.SellLittleFilledPriceDroppedClassicShortSituation = this.SellLittleFilledPriceDroppedClassicShortSituation;
                    Settings.SellLittleFilledCanceledClassicShortSituation = this.SellLittleFilledCanceledClassicShortSituation;
                    Settings.BuyCanceledLittleReminderClassicShortSituation = this.BuyCanceledLittleReminderClassicShortSituation;
                    Settings.BuyLittleReminderPriceIncreasedClassicShortSituation = this.BuyLittleReminderPriceIncreasedClassicShortSituation;

                    Settings.XOrdersAheadLittleFilledPriceDroppedClassicShortSituation = this.XOrdersAheadLittleFilledPriceDroppedClassicShortSituation;
                    Settings.SecondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation = this.SecondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation;
                    Settings.DropPercentLittleFilledPriceDroppedClassicShortSituation = this.DropPercentLittleFilledPriceDroppedClassicShortSituation;
                    Settings.AheadOrdersVolumeLittleFilledPriceDroppedClassicShortSituation = this.AheadOrdersVolumeLittleFilledPriceDroppedClassicShortSituation;

                    Settings.XOrdersAheadSellFilledEnoughPriceDropped = this.XOrdersAheadSellFilledEnoughPriceDropped;
                    Settings.SecondsAfterLastUpdateSellFilledEnoughPriceDropped = this.SecondsAfterLastUpdateSellFilledEnoughPriceDropped;
                    Settings.DropPercentSellFilledEnoughPriceDropped = this.DropPercentSellFilledEnoughPriceDropped;
                    Settings.AheadOrdersVolumeSellFilledEnoughPriceDropped = this.AheadOrdersVolumeSellFilledEnoughPriceDropped;

                    Settings.XOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation = this.XOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation;
                    Settings.SecondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation = this.SecondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation;
                    Settings.DropPercentSellLittleReminderPriceIncreasedClassicShortSituation = this.DropPercentSellLittleReminderPriceIncreasedClassicShortSituation;
                    Settings.AheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation = this.AheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation;


                    // Общее

                    Settings.StopAfterXStopLoss = this.StopAfterXStopLoss;

                    // Данные
                    Settings.GoogleLogin = this.GoogleLogin;
                    Settings.GooglePassword = this.GooglePassword;
                    Settings.Save();
                });
            }
        }

        // Кнопка Добавить
        public ICommand AddApiClick
        {
            get
            {
                return new Command((obj) =>
                {
                    if (this.APIKey == "" || this.APISecret == "" || this.APIKey == null || this.APISecret == null )
                    {
                        System.Windows.MessageBox.Show("API keys are empty!");
                        return;
                    }

                    /* Временное решение, работает только с бинанс */
                    if (obj.ToString() == "Binance")
                        this.APIBinance.Add(new Tokens { APIKey = this.APIKey, APISecret = this.APISecret });

                    if (obj.ToString() == "Binance Futures")
                        this.APIBinanceFutures.Add(new Tokens { APIKey = this.APIKey, APISecret = this.APISecret });

                    // Опустошаем текстбоксы
                    this.APIKey = "";
                    this.APISecret = "";

                }, x => this.APIBinance.Count() < Encryption.HEXToNum(Conv.GetClearInt(_.Msg20)));
            }
        }

        
        public ICommand DeleteApiClick
        {
            get
            {
                return new Command((obj) =>
                {
                    /* Временное решение, работает только с бинанс */
                    if (obj.ToString() == "Binance")
                        APIBinance.Remove(APISelected); // Удаляем выбранные токены

                    if (obj.ToString() == "Binance Futures")
                        APIBinanceFutures.Remove(APIFuturesSelected); // Удаляем выбранные токены

                }, (x) => APISelected != null);
            }
        }

        // Кнопка Активировать (Лицензию) по хэшу
        public ICommand ActivateLicenseClick
        {
            get
            {
                return new Command((obj) =>
                {
                    // Try to register the license

                    /*string regResult = BtnPlus.RegLicenseCode(this.License);

                    if (regResult == "nope")
                    {
                        System.Windows.MessageBox.Show(_.Msg11, _.Attention); // Лицензии не существует или уже активирована
                    }
                    else if (regResult == "user_error")
                    {
                        System.Windows.MessageBox.Show(_.Msg12, _.Error); // Нужно сначала девайс добавить
                    }
                    else
                    {
                        // Registrated

                        this.License = regResult;
                        File.WriteAllText("license", this.License);

                        string activationResult = BtnPlus.ActiveteLicense(regResult);

                        if (activationResult == "ok")
                        {
                            System.Windows.MessageBox.Show(_.Msg14, _.Accepted); // Готово, бот перезагрузится
                            Environment.Exit(0);
                        }
                        if (activationResult == "error")
                        {
                            System.Windows.MessageBox.Show(_.Msg15, _.Error);
                        }
                        if (activationResult == "user_error")
                        {
                            System.Windows.MessageBox.Show(_.Msg16, _.Error);
                        }
                        if (activationResult == "error_lic")
                        {
                            System.Windows.MessageBox.Show(_.Msg17, _.Error);
                        }
                    }*/

                });
            }
        }

        // Кнопка Продлить лицензию
        public ICommand ExtendLicenseClick
        {
            get
            {
                return new Command((obj) =>
                {
                    // System.Diagnostics.Process.Start(_.BtnPlusLoginUrl);
                });
            }
        }

    }
}
