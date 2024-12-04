using stratumbot.Core;
using stratumbot.Interfaces;
using stratumbot.Models;
using stratumbot.Models.Exchanges;
using stratumbot.Models.Logs;
using stratumbot.Models.Strategies;
using stratumbot.Views.StrategyViews;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using stratumbot.DTO;
using stratumbot.Models.Filters;
using stratumbot.Models.Tools;

namespace stratumbot.ViewModels
{
    public class MainVM : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        private bool locker = false; // Для блокировки работы без лицензии // true - блокировка включена

        #region Общие поля

        private string budget;
        public string Budget
        {
            get { return budget; }
            set
            {
                budget = value;
                OnPropertyChanged();
            }
        }

        private string cur1;
        public string Cur1
        {
            get { return cur1; }
            set
            {
                cur1 = value;
                OnPropertyChanged();
            }
        }

        private string cur2;
        public string Cur2
        {
            get { return cur2; }
            set
            {
                cur2 = value;
                OnPropertyChanged();
            }
        }

        private bool isDCA;
        public bool IsDCA
        {
            get { return isDCA; }
            set
            {
                isDCA = value;
                OnPropertyChanged();
            }
        }

        private string dcaProfitPercent;
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

        public ObservableCollection<string[]> DCASteps { get; set; } // Хранит в себе настройки DCA шагов 

        // Метод генерации параметров шагов по умолчанию
        private void DCAStepsGenerate()
        {
            if (DCASteps == null)
                return;

            DCASteps.Clear();
            for (int i = 0; i < this.dcaStepCount; i++)
            {
                int val = i + 1;
                DCASteps.Add(new string[] { $"#{val}", $"{val}%", $"100%" });
            }
        }

        // StopLoss fields

        private bool isStopLoss;
        public bool IsStopLoss
        {
            get { return isStopLoss; }
            set
            {
                isStopLoss = value;
                OnPropertyChanged();
            }
        }

        private string stopLoss;
        public string StopLoss
        {
            get { return stopLoss; }
            set
            {
                stopLoss = value;
                //if (!IsStopLoss && StopLoss != "0")
                //    this.IsStopLoss = true;
                OnPropertyChanged();
            }
        }

        private string stopLossApproximation;
        public string StopLossApproximation
        {
            get { return stopLossApproximation; }
            set
            {
                stopLossApproximation = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region Поля для скальпинга

        private string minSpread;
        public string MinSpread
        {
            get { return minSpread; }
            set
            {
                minSpread = value;
                OnPropertyChanged();
            }
        }

        private string optSpread;
        public string OptSpread
        {
            get { return optSpread; }
            set
            {
                optSpread = value;
                OnPropertyChanged();
            }
        }

        private string minMarkup;
        public string MinMarkup
        {
            get { return minMarkup; }
            set
            {
                minMarkup = value;
                OnPropertyChanged();
            }
        }

        private string optMarkup;
        public string OptMarkup
        {
            get { return optMarkup; }
            set
            {
                optMarkup = value;
                OnPropertyChanged();
            }
        }

        private string zeroSell;
        public string ZeroSell
        {
            get { return zeroSell; }
            set
            {
                zeroSell = value;
                OnPropertyChanged();
            }
        }

        private string inTimeout;
        public string InTimeout
        {
            get { return inTimeout; }
            set
            {
                inTimeout = value;
                OnPropertyChanged();
            }
        }

        private string firsOredersAmountPercentIgnor;
        public string FirsOredersAmountPercentIgnor
        {
            get { return firsOredersAmountPercentIgnor; }
            set
            {
                firsOredersAmountPercentIgnor = value;
                OnPropertyChanged();
            }
        }

        private string firsOredersCountIgnor;
        public string FirsOredersCountIgnor
        {
            get { return firsOredersCountIgnor; }
            set
            {
                firsOredersCountIgnor = value;
                OnPropertyChanged();
            }
        }

        /*private bool isSellStart;
        public bool IsSellStart
        {
            get { return isSellStart; }
            set
            {
                isSellStart = value;
                OnPropertyChanged();
            }
        }*/


        /*private string buyPriceForSellStart;
        public string BuyPriceForSellStart
        {
            get { return buyPriceForSellStart; }
            set
            {
                buyPriceForSellStart = value;
                OnPropertyChanged();
            }
        }*/

        /*private string amountForSellStart;
        public string AmountForSellStart
        {
            get { return amountForSellStart; }
            set
            {
                amountForSellStart = value;
                OnPropertyChanged();
            }
        }*/


        #endregion

        #region Поля для Classic Long

        private string targetProfitPercent; // Профит
        public string TargetProfitPercent
        {
            get { return targetProfitPercent; }
            set
            {
                targetProfitPercent = value;
                OnPropertyChanged();
            }
        }

        private bool isBuyByMarket; // Покупка по рынку
        public bool IsBuyByMarket
        {
            get { return isBuyByMarket; }
            set
            {
                isBuyByMarket = value;
                OnPropertyChanged();
            }
        }

        private bool isProfitTrailing; // Профит трейлинг
        public bool IsProfitTrailing
        {
            get { return isProfitTrailing; }
            set
            {
                isProfitTrailing = value;
                OnPropertyChanged();
            }
        }

        private string trailStepPercent; // шаг профит трейла
        public string TrailStepPercent
        {
            get { return trailStepPercent; }
            set
            {
                trailStepPercent = value;
                OnPropertyChanged();
            }
        }

        private string approximationPercent; // Процент приближения к ордеру при котором срабатывает профиттрейлинг
        public string ApproximationPercent
        {
            get { return approximationPercent; }
            set
            {
                approximationPercent = value;
                OnPropertyChanged();
            }
        }

        private string unApproximationPercent; // Процент отдаления от последней максимальной цены при котором продаётся. (при условии что профит > 
        public string UnApproximationPercent
        {
            get { return unApproximationPercent; }
            set
            {
                unApproximationPercent = value;
                OnPropertyChanged();
            }
        }

        private bool isMarketBuy; // покупать ли по рынку
        public bool IsMarketBuy
        {
            get { return isMarketBuy; }
            set
            {
                isMarketBuy = value;
                OnPropertyChanged();
            }
        }

        private bool isMarketSell; // продавать ли по рынку
        public bool IsMarketSell
        {
            get { return isMarketSell; }
            set
            {
                isMarketSell = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Стратегии, биржи, конфиги

        // Доступные стратегии: список для представления
        public ObservableCollection<AvailableStrategy> MyAvailableStrategies { get; set; } = new ObservableCollection<AvailableStrategy>();

        // Выбранная стратегия
        private AvailableStrategy selectedStrategy;
        public AvailableStrategy SelectedStrategy
        {
            get { return selectedStrategy ?? MyAvailableStrategies[0]; }
            set
            {
                selectedStrategy = value;
                OnPropertyChanged();
                // Clear filters
                this.FiltersBuy.Clear();
                this.FiltersSell.Clear();

                //if (selectedStrategy == null) return;

                // Views Changes
                if (selectedStrategy.Id == Strategy.Scalping)
                {
                    if(!(StrategyView is ScalpingView))
                        StrategyView = new ScalpingView();
                }
                if (selectedStrategy.Id == Strategy.ClassicLong)
                {
                    if (!(StrategyView is ClassicLongView))
                        StrategyView = new ClassicLongView();
                }
                if (selectedStrategy.Id == Strategy.ClassicShort)
                {
                    if (!(StrategyView is ClassicShortView))
                        StrategyView = new ClassicShortView();
                }
                if (selectedStrategy.Id == Strategy.Arbitrage)
                {
                    if (!(StrategyView is ArbitrageView))
                        StrategyView = new ArbitrageView();
                }
                LoadConfigs();
            }
        }

        // Представление стратегии
        public object strategyView;
        public object StrategyView
        {
            get { return strategyView; }
            set
            {
                strategyView = value;
                OnPropertyChanged();
            }
        }

        // Доступные биржи: список для представления
        public ObservableCollection<AvailableExchange> MyAvailableExchanges { get; set; } = new ObservableCollection<AvailableExchange>();

        // Выбранная биржа
        private AvailableExchange selectedExchange;
        public AvailableExchange SelectedExchange
        {
            get { return selectedExchange ?? MyAvailableExchanges[0]; }
            set
            {
                if (selectedExchange == value)//Если выбранная стратегия итак эта зачем её менять
                    return;
                selectedExchange = value;
                OnPropertyChanged();

                // Формируем список доступных стратегий для биржи
                // Запомнить выбранную стратегию
                var _selectedStrategy = SelectedStrategy;
                var _SelectedStrategyConfig = SelectedStrategyConfig; // Запоминаем выбранный конфиг, чтобы выбрать его

                MyAvailableStrategies.Clear();
                AvailableStrategies.GetAll(SelectedExchange.Id).ForEach(x => { MyAvailableStrategies.Add(x); });

                // Выбираем стратегию. Если выбранная стратегия есть в выбранной бирже то выбираем её. Иначе первую.
                if (MyAvailableStrategies.Any(x => x.Id == _selectedStrategy.Id))
                {
                    foreach (var strategy in MyAvailableStrategies)
                    {
                        if (strategy.Id == _selectedStrategy.Id)
                        {
                            SelectedStrategy = strategy; // Выбираем опять выбранную пользователем стратегию
                            SelectedStrategyConfig = _SelectedStrategyConfig; // Выбираем опять выбранный пользователем конфиг
                            break;
                        }
                    }

                } else
                {
                    SelectedStrategy = MyAvailableStrategies[0];
                }
            }
        }

        #region Конфиги

        // Список доступных конфигов для стратегии
        public ObservableCollection<string> StrategyConfigs { get; set; } = new ObservableCollection<string>();

        // Выбранная конфиг стратегии
        private string selectedStrategyConfig;
        public string SelectedStrategyConfig
        {
            get { return selectedStrategyConfig; }
            set
            {
                selectedStrategyConfig = value;
                OnPropertyChanged();
                ViewConfig();
            }
        }

        // Метод подгрузки списка конфигов выбранной стратегии
        private void LoadConfigs()
        {
            if (SelectedStrategy == null)
                return;

            string[] files = Directory.GetFiles(@"Strategies/" + SelectedStrategy.Id.ToString() + "/", "*.strat");
            StrategyConfigs.Clear();
            foreach (var file in files)
            {
                if(File.ReadAllText(file).IsValidJson())
                {
                    StrategyConfigs.Add(System.IO.Path.GetFileNameWithoutExtension(file));
                }
            }
        }

        /// <summary>
        /// Fill fields from IConfigText object
        /// </summary>
        /// <param name="configText">IConfigText object</param>
        private void FillFieldsFromIConfigText(IConfigText configText)
        {
            if (configText.Strategy == Strategy.Scalping.ToString())
            {
                var config = configText as ScalpingConfigText;

                this.Budget = config.Budget;
                this.Cur1 = config.Cur1;
                this.Cur2 = config.Cur2;
                this.MinSpread = config.MinSpread;
                this.OptSpread = config.OptSpread;
                this.MinMarkup = config.MinMarkup;
                this.OptMarkup = config.OptMarkup;
                this.ZeroSell = config.ZeroSell;
                this.InTimeout = config.InTimeout;
                this.IsDCA = config.IsDCA;
                this.DCAStepCount = config.DCAStepCount;
                this.DCAProfitPercent = config.DCAProfitPercent;
                var DCAStepsStr = new ObservableCollection<string[]>(from x in config.DCASteps select (string[])x.Clone()); // Создаём копию, иначе все как один объект изминяется во всех потоках
                this.DCASteps.Clear();
                foreach (var dcaStep in DCAStepsStr)
                {
                    this.DCASteps.Add(dcaStep);
                }
                this.DCAFilters = new Dictionary<int, DCAFilter>(config.DCAFilters); // dcaf
                this.FiltersBuy.Clear();
                foreach (var filter in config.FiltersBuy)
                {
                    this.FiltersBuy.Add(filter);
                }
                this.FirsOredersAmountPercentIgnor = config.FirsOredersAmountPercentIgnor;
                this.FirsOredersCountIgnor = config.FirsOredersCountIgnor;
                this.IsStopLoss = config.IsStopLoss;
                this.StopLoss = config.StopLoss;
                this.FiltersBuy.Clear();
                foreach (var filter in config.FiltersBuy)
                {
                    this.FiltersBuy.Add(filter);
                }
                this.TargetPointBuy = config.TargetPointBuy;
            }

            if (configText.Strategy == Strategy.ClassicLong.ToString())
            {
                
                var config = configText as ClassicLongConfigText;

                this.Budget = config.Budget;
                this.Cur1 = config.Cur1;
                this.Cur2 = config.Cur2;

                this.TargetProfitPercent = config.TargetProfitPercent;
                this.IsProfitTrailing = config.IsProfitTrailing;
                this.TrailStepPercent = config.TrailStepPercent;
                this.ApproximationPercent = config.ApproximationPercent;
                this.UnApproximationPercent = config.UnApproximationPercent;
                this.IsMarketBuy = config.IsMarketBuy;

                this.IsDCA = config.IsDCA;
                this.DCAStepCount = config.DCAStepCount;
                this.DCAProfitPercent = config.DCAProfitPercent;
                var DCAStepsStr = new ObservableCollection<string[]>(from x in config.DCASteps select (string[])x.Clone()); // Создаём копию, иначе все как один объект изминяется во всех потоках
                this.DCASteps.Clear();
                foreach (var dcaStep in DCAStepsStr)
                {
                    this.DCASteps.Add(dcaStep);
                }
                this.DCAFilters = new Dictionary<int, DCAFilter>(config.DCAFilters); // dcaf

                this.FiltersBuy.Clear();
                foreach (var filter in config.FiltersBuy)
                {
                    this.FiltersBuy.Add(filter);
                }
                this.TargetPointBuy = config.TargetPointBuy;

                this.FiltersSell.Clear();
                foreach (var filter in config.FiltersSell)
                {
                    this.FiltersSell.Add(filter);
                }
                this.TargetPointSell = config.TargetPointSell;

                this.IsStopLoss = config.IsStopLoss;
                this.StopLoss = config.StopLoss;
                this.StopLossApproximation = config.StopLossApproximation;

                this.FiltersStopLoss.Clear();
                foreach (var filter in config.FiltersStopLoss)
                {
                    this.FiltersStopLoss.Add(filter);
                }
                this.TargetPointStopLoss = config.TargetPointStopLoss;
            }

            if (configText.Strategy == Strategy.ClassicShort.ToString())
            {

                var config = configText as ClassicShortConfigText;

                this.Budget = config.Budget;
                this.Cur1 = config.Cur1;
                this.Cur2 = config.Cur2;

                this.TargetProfitPercent = config.TargetProfitPercent;
                this.IsProfitTrailing = config.IsProfitTrailing;
                this.TrailStepPercent = config.TrailStepPercent;
                this.ApproximationPercent = config.ApproximationPercent;
                this.UnApproximationPercent = config.UnApproximationPercent;
                this.IsMarketSell = config.IsMarketSell;

                this.IsDCA = config.IsDCA;
                this.DCAStepCount = config.DCAStepCount;
                this.DCAProfitPercent = config.DCAProfitPercent;
                var DCAStepsStr = new ObservableCollection<string[]>(from x in config.DCASteps select (string[])x.Clone()); // Создаём копию, иначе все как один объект изминяется во всех потоках
                this.DCASteps.Clear();
                foreach (var dcaStep in DCAStepsStr)
                {
                    this.DCASteps.Add(dcaStep);
                }
                this.DCAFilters = new Dictionary<int, DCAFilter>(config.DCAFilters); // dcaf

                this.FiltersBuy.Clear();
                foreach (var filter in config.FiltersBuy)
                {
                    this.FiltersBuy.Add(filter);
                }
                this.TargetPointBuy = config.TargetPointBuy;

                this.FiltersSell.Clear();
                foreach (var filter in config.FiltersSell)
                {
                    this.FiltersSell.Add(filter);
                }
                this.TargetPointSell = config.TargetPointSell;

                this.IsStopLoss = config.IsStopLoss;
                this.StopLoss = config.StopLoss;
                this.StopLossApproximation = config.StopLossApproximation;

                this.FiltersStopLoss.Clear();
                foreach (var filter in config.FiltersStopLoss)
                {
                    this.FiltersStopLoss.Add(filter);
                }
                this.TargetPointStopLoss = config.TargetPointStopLoss;
            }
        }

          /// <summary>
          /// Get IConfigText object from fields
          /// </summary>
          /// <returns>IConfigText object</returns>
          private IConfigText GetIConfigTextFromFields()
          {
              if (SelectedStrategy.Id == Strategy.Scalping)
              {
                  return new ScalpingConfigText()
                  {
                      Strategy = Strategy.Scalping.ToString(),
                      Budget = this.Budget,
                      Cur1 = this.Cur1,
                      Cur2 = this.Cur2,
                      MinSpread = this.MinSpread,
                      OptSpread = this.OptSpread,
                      MinMarkup = this.MinMarkup,
                      OptMarkup = this.OptMarkup,
                      ZeroSell = this.ZeroSell,
                      InTimeout = this.InTimeout,
                      IsDCA = this.IsDCA,
                      DCAStepCount = this.DCAStepCount,
                      DCAProfitPercent = this.DCAProfitPercent,
                      DCASteps = this.DCASteps,
                      DCAFilters = this.DCAFilters,
                      FirsOredersAmountPercentIgnor = this.FirsOredersAmountPercentIgnor,
                      FirsOredersCountIgnor = this.FirsOredersCountIgnor,
                      IsStopLoss = this.IsStopLoss,
                      StopLoss = this.StopLoss,
                      FiltersBuy = new List<JsonFilter>(this.FiltersBuy), // TODO INDICATORS тут добавил и надо везде
                      TargetPointBuy = this.TargetPointBuy
                  };
              }

              throw new Exception("No strategy selected");
          }

          // Метод отображения конфига
          private void ViewConfig()
          {
              if (SelectedStrategyConfig == null)
                  return;
              if (SelectedStrategy == null)
                  return;

              var text = File.ReadAllText(@"Strategies/" + SelectedStrategy.Id.ToString() + "/" + SelectedStrategyConfig + ".strat");

              if (!text.IsValidJson())
                  throw new Exception("code 19");

              if (SelectedStrategy.Id == Strategy.Scalping) {

                  this.FillFieldsFromIConfigText(ConfigManager.Read(text));

                  /*var config = Newtonsoft.Json.JsonConvert.DeserializeObject<ScalpingConfigText>(text);
                  this.Budget = config.Budget;
                  this.Cur1 = config.Cur1;
                  this.Cur2 = config.Cur2;
                  this.MinSpread = config.MinSpread;
                  this.OptSpread = config.OptSpread;
                  this.MinMarkup = config.MinMarkup;
                  this.OptMarkup = config.OptMarkup;
                  this.ZeroSell = config.ZeroSell;
                  this.InTimeout = config.InTimeout;
                  this.IsDCA = config.IsDCA;
                  this.DCAStepCount = config.DCAStepCount;
                  this.DCAProfitPercent = config.DCAProfitPercent;
                  var DCAStepsStr = new ObservableCollection<string[]>(from x in config.DCASteps select (string[])x.Clone()); // Создаём копию, иначе все как один объект изминяется во всех потоках
                  this.DCASteps.Clear();
                  foreach (var dcaStep in DCAStepsStr)
                  {
                      this.DCASteps.Add(dcaStep);
                  }
                  this.DCAFilters = new Dictionary<int, DCAFilter>(config.DCAFilters); // dcaf
                  this.FiltersBuy.Clear();
                  foreach (var filter in config.FiltersBuy)
                  {
                      this.FiltersBuy.Add(filter);
                  }
                  this.FirsOredersAmountPercentIgnor = config.FirsOredersAmountPercentIgnor; 
                  this.FirsOredersCountIgnor = config.FirsOredersCountIgnor; 
                  this.StopLoss = config.StopLoss;
                  this.FiltersBuy.Clear();
                  foreach (var filter in config.FiltersBuy)
                  {
                      this.FiltersBuy.Add(filter);
                  }
                  this.TargetPointBuy = config.TargetPointBuy;*/
            }
            if (SelectedStrategy.Id == Strategy.ClassicLong)
            {
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<ClassicLongConfigText>(text);
                this.Budget = config.Budget;
                this.Cur1 = config.Cur1;
                this.Cur2 = config.Cur2;

                this.TargetProfitPercent = config.TargetProfitPercent;
                this.IsProfitTrailing = config.IsProfitTrailing;
                this.TrailStepPercent = config.TrailStepPercent;
                this.ApproximationPercent = config.ApproximationPercent;
                this.UnApproximationPercent = config.UnApproximationPercent;
                this.IsMarketBuy = config.IsMarketBuy;

                this.IsDCA = config.IsDCA;
                this.DCAStepCount = config.DCAStepCount;
                this.DCAProfitPercent = config.TargetProfitPercent; // config.DCAProfitPercent;
                var DCAStepsStr = new ObservableCollection<string[]>(from x in config.DCASteps select (string[])x.Clone()); // Создаём копию, иначе все как один объект изминяется во всех потоках
                this.DCASteps.Clear();
                foreach (var dcaStep in DCAStepsStr)
                {
                    this.DCASteps.Add(dcaStep);
                }
                this.DCAFilters = new Dictionary<int, DCAFilter>(config.DCAFilters); // dcaf

                this.FiltersBuy.Clear();
                foreach (var filter in config.FiltersBuy)
                {
                    this.FiltersBuy.Add(filter);
                }
                this.TargetPointBuy = config.TargetPointBuy;
                this.FiltersSell.Clear();
                foreach (var filter in config.FiltersSell)
                {
                    this.FiltersSell.Add(filter);
                }
                this.TargetPointSell = config.TargetPointSell;

                this.IsStopLoss = config.IsStopLoss;
                this.StopLoss = config.StopLoss;
                this.StopLossApproximation = config.StopLossApproximation;
                this.FiltersStopLoss.Clear();
                foreach (var filter in config.FiltersStopLoss)
                {
                    this.FiltersStopLoss.Add(filter);
                }
                this.TargetPointStopLoss = config.TargetPointStopLoss;
            }

            if (SelectedStrategy.Id == Strategy.ClassicShort)
            {
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<ClassicShortConfigText>(text);
                this.Budget = config.Budget;
                this.Cur1 = config.Cur1;
                this.Cur2 = config.Cur2;

                this.TargetProfitPercent = config.TargetProfitPercent;
                this.IsProfitTrailing = config.IsProfitTrailing;
                this.TrailStepPercent = config.TrailStepPercent;
                this.ApproximationPercent = config.ApproximationPercent;
                this.UnApproximationPercent = config.UnApproximationPercent;
                this.IsMarketSell = config.IsMarketSell;

                this.IsDCA = config.IsDCA;
                this.DCAStepCount = config.DCAStepCount;
                this.DCAProfitPercent = config.TargetProfitPercent; // config.DCAProfitPercent;
                var DCAStepsStr = new ObservableCollection<string[]>(from x in config.DCASteps select (string[])x.Clone()); // Создаём копию, иначе все как один объект изминяется во всех потоках
                this.DCASteps.Clear();
                foreach (var dcaStep in DCAStepsStr)
                {
                    this.DCASteps.Add(dcaStep);
                }
                this.DCAFilters = new Dictionary<int, DCAFilter>(config.DCAFilters); // dcaf
                this.FiltersBuy.Clear();
                foreach (var filter in config.FiltersBuy)
                {
                    this.FiltersBuy.Add(filter);
                }
                this.TargetPointBuy = config.TargetPointBuy;
                this.FiltersSell.Clear();
                foreach (var filter in config.FiltersSell)
                {
                    this.FiltersSell.Add(filter);
                }
                this.TargetPointSell = config.TargetPointSell;

                this.IsStopLoss = config.IsStopLoss;
                this.StopLoss = config.StopLoss;
                this.StopLossApproximation = config.StopLossApproximation;
                this.FiltersStopLoss.Clear();
                foreach (var filter in config.FiltersStopLoss)
                {
                    this.FiltersStopLoss.Add(filter);
                }
                this.TargetPointStopLoss = config.TargetPointStopLoss;
            }

        }

        #endregion

        #endregion

        #region Потоки

        // Список потоков
        public ObservableCollection<TThread> Threads { get; set; } = new ObservableCollection<TThread>();

        // Выбранный поток
        private TThread selectedThread;
        public TThread SelectedThread
        {
            get { return selectedThread; }
            set
            {
                selectedThread = value;
                OnPropertyChanged();

                if(selectedThread != null )
                {
                    if (selectedThread.Status == "draft" || selectedThread.Status == "stop")
                    {
                        ClickBablo = ClickBabloStart;
                        BabloText = _.BabloStartButton;
                    }
                    if (selectedThread.Status == "work")
                    {
                        ClickBablo = ClickBabloStop;
                        BabloText = _.BabloStopButton;
                    }
                    if (selectedThread.Status == "edit")
                    {
                        ClickBablo = EditBabloClick;
                        BabloText = _.BabloSaveButton; // СОХРАНИТЬ
                    }

                    foreach (var strategy in MyAvailableStrategies)
                    {
                        if (strategy.Id == selectedThread.config.Strategy)
                        {
                            SelectedStrategy = strategy;
                            break;
                        }

                    }
                    foreach (var exchange in MyAvailableExchanges)
                    {
                        if (exchange.Id == selectedThread.deal.Exchange.Id)
                        {
                            SelectedExchange = exchange;
                            break;
                        }
                    }

                    // Отображаем поля
                    //Budget = selectedThread.config.Budget.ToString();
                    //Cur1 = selectedThread.config.Cur1.ToString();
                    //Cur2 = selectedThread.config.Cur2.ToString();
                    
                    if (selectedThread.config is ScalpingConfig)
                    {
                        //SelectedStrategy = MyAvailableStrategies.FirstOrDefault(x => x.Id == selectedThread.config.Strategy);
                        //SelectedExchange = MyAvailableExchanges.FirstOrDefault(x => x.Id == selectedThread.config.Exchange);

                        this.FillFieldsFromIConfigText((selectedThread.config as ScalpingConfig).IConfigText);

                        /*this.MinSpread = (selectedThread.config as ScalpingConfig).MinSpread.ToString();
                        this.OptSpread = (selectedThread.config as ScalpingConfig).OptSpread.ToString();
                        this.MinMarkup = (selectedThread.config as ScalpingConfig).MinMarkup.ToString();
                        this.OptMarkup = (selectedThread.config as ScalpingConfig).OptMarkup.ToString();
                        this.ZeroSell = (selectedThread.config as ScalpingConfig).ZeroSell.ToString();
                        this.InTimeout = (selectedThread.config as ScalpingConfig).InTimeout.ToString();
                        this.FirsOredersAmountPercentIgnor = (selectedThread.config as ScalpingConfig).FirsOredersAmountPercentIgnor.ToString();
                        this.FirsOredersCountIgnor = (selectedThread.config as ScalpingConfig).FirsOredersCountIgnor.ToString();
                        this.StopLoss = ((selectedThread.config as ScalpingConfig).IsStopLossAsPercent) ? (selectedThread.config as ScalpingConfig).StopLossPercent.ToString() + "%" : (selectedThread.config as ScalpingConfig).StopLossPrice.ToString();
                        this.IsDCA = (selectedThread.config as ScalpingConfig).IsDCA;
                        this.DCAProfitPercent = (selectedThread.config as ScalpingConfig).DCAProfitPercent.ToString();
                        this.DCAStepCount = (selectedThread.config as ScalpingConfig).DCAStepCount.ToString();
                        //var DCAStepsStr = new ObservableCollection<string[]>(from x in (selectedThread.config as ScalpingConfig).DCAStepsStr select (string[])x.Clone()); // Создаём копию, иначе все как один объект изминяется во всех потоках
                        var DCASteps = (selectedThread.config as ScalpingConfig).DCASteps;
                        this.DCASteps.Clear();
                        // TODO check if it is norm 
                        foreach (var dcaStep in DCASteps)
                        {
                            this.DCASteps.Add(new string[] {
                                dcaStep.Number.ToString(),
                                dcaStep.Drop.ToString(),
                                dcaStep.Amount.ToString()
                            });
                        }
                        this.DCAFilters = new Dictionary<int, DCAFilter>((selectedThread.config as ScalpingConfig).Filters.DCAFilters); //dcaf

                        this.FiltersBuy.Clear();
                        foreach (var filter in (selectedThread.config as ScalpingConfig).iConfigText.FiltersBuy)
                        {
                            this.FiltersBuy.Add(filter);
                        }
                        this.TargetPointBuy = (selectedThread.config as ScalpingConfig).Filters.TargetPointBuy;
                        //this.IsSellStart = (selectedThread.config as ScalpingConfigJson).IsSellStart;
                        //this.BuyPriceForSellStart = (selectedThread.config as ScalpingConfigJson).BuyPriceForSellStartStr;
                        //this.AmountForSellStart = (selectedThread.config as ScalpingConfigJson).AmountForSellStartStr;
                        this.FiltersBuy.Clear();
                        foreach (var filter in  (selectedThread.config as ScalpingConfig).iConfigText.FiltersBuy)
                        {
                            this.FiltersBuy.Add(filter);
                        }
                        this.TargetPointBuy = (selectedThread.config as ScalpingConfig).Filters.TargetPointBuy;
                        */
                    }
                    // Classic Long
                    if (selectedThread.config is ClassicLongConfig)
                    {
                        this.FillFieldsFromIConfigText((selectedThread.config as ClassicLongConfig).IConfigText);

                        /*foreach (var strategy in MyAvailableStrategies)
                        {
                            if (strategy.Id == selectedThread.config.Strategy)
                            {
                                SelectedStrategy = strategy;
                                break;
                            }

                        }
                        foreach (var exchange in MyAvailableExchanges)
                        {
                            if (exchange.Id == selectedThread.deal.Exchange.Id)
                            {
                                SelectedExchange = exchange;
                                break;
                            }
                        }*/

                        /*this.TargetProfitPercent = (selectedThread.config as ClassicLongConfig).TargetProfitPercent;
                        this.IsProfitTrailing = (selectedThread.config as ClassicLongConfig).IsProfitTrailing;
                        this.TrailStepPercent = (selectedThread.config as ClassicLongConfig).TrailStepPercent;
                        this.ApproximationPercent = (selectedThread.config as ClassicLongConfig).ApproximationPercent;
                        this.UnApproximationPercent = (selectedThread.config as ClassicLongConfig).UnApproximationPercent;
                        this.IsMarketBuy = (selectedThread.config as ClassicLongConfig).IsMarketBuy;

                        this.IsDCA = (selectedThread.config as ClassicLongConfig).IsDCA;
                        this.DCAProfitPercent = (selectedThread.config as ClassicLongConfig).DCAProfitPercentStr;
                        this.DCAStepCount = (selectedThread.config as ClassicLongConfig).DCAStepCountStr;
                        var DCAStepsStr = new ObservableCollection<string[]>(from x in (selectedThread.config as ClassicLongConfig).DCAStepsStr select (string[])x.Clone()); // Создаём копию, иначе все как один объект изминяется во всех потоках
                        this.DCASteps.Clear();
                        foreach (var dcaStep in DCAStepsStr)
                        {
                            this.DCASteps.Add(dcaStep);
                        }
                        this.DCAFilters = new Dictionary<int, DCAFilter>((selectedThread.config as ClassicLongConfig).DCAFilters); //dcaf
                        
                        this.FiltersBuy.Clear();
                        foreach (var filter in (selectedThread.config as ClassicLongConfig).FiltersBuy)
                        {
                            this.FiltersBuy.Add(filter);
                        }
                        this.TargetPointBuy = int.Parse((selectedThread.config as ClassicLongConfig).TargetPointBuy);

                        this.FiltersSell.Clear();
                        foreach (var filter in (selectedThread.config as ClassicLongConfig).FiltersSell)
                        {
                            this.FiltersSell.Add(filter);
                        }
                        this.TargetPointSell = (selectedThread.config as ClassicLongConfig).TargetPointSell;

                        this.StopLoss = (selectedThread.config as ClassicLongConfig).StopLoss;
                        this.StopLossApproximation = (selectedThread.config as ClassicLongConfig).StopLossApproximation;

                        this.FiltersStopLoss.Clear();
                        foreach (var filter in (selectedThread.config as ClassicLongConfig).FiltersStopLoss)
                        {
                            this.FiltersStopLoss.Add(filter);
                        }
                        this.TargetPointStopLoss = (selectedThread.config as ClassicLongConfig).TargetPointStopLoss;*/
                    }

                    // Classic Short
                    if (selectedThread.config is ClassicShortConfig)
                    {
                        this.FillFieldsFromIConfigText((selectedThread.config as ClassicShortConfig).IConfigText);

                        /*foreach (var strategy in MyAvailableStrategies)
                        {
                            if (strategy.Id == selectedThread.config.Strategy)
                            {
                                SelectedStrategy = strategy;
                                break;
                            }

                        }
                        foreach (var exchange in MyAvailableExchanges)
                        {
                            if (exchange.Id == selectedThread.deal.Exchange.Id)
                            {
                                SelectedExchange = exchange;
                                break;
                            }
                        }*/

                        /*this.TargetProfitPercent = (selectedThread.config as ClassicShortConfig).TargetProfitPercent.ToString();
                        this.IsProfitTrailing = (selectedThread.config as ClassicShortConfig).IsProfitTrailing;
                        this.TrailStepPercent = (selectedThread.config as ClassicShortConfig).TrailStepPercent.ToString();
                        this.ApproximationPercent = (selectedThread.config as ClassicShortConfig).ApproximationPercent.ToString();
                        this.UnApproximationPercent = (selectedThread.config as ClassicShortConfig).UnApproximationPercent.ToString();
                        this.IsMarketSell = (selectedThread.config as ClassicShortConfig).IsMarketSell;

                        this.IsDCA = (selectedThread.config as ClassicShortConfig).IsDCA;
                        this.DCAProfitPercent = (selectedThread.config as ClassicShortConfig).DCAProfitPercent.ToString();
                        this.DCAStepCount = (selectedThread.config as ClassicShortConfig).DCAStepCount.ToString();
                        var DCAStepsStr = new ObservableCollection<string[]>(from x in (selectedThread.config as ClassicShortConfig).DCAStepsStr select (string[])x.Clone()); // Создаём копию, иначе все как один объект изминяется во всех потоках
                        this.DCASteps.Clear();
                        foreach (var dcaStep in DCAStepsStr)
                        {
                            this.DCASteps.Add(dcaStep);
                        }
                        this.DCAFilters = new Dictionary<int, DCAFilter>((selectedThread.config as ClassicShortConfig).DCAFilters); //dcaf
                        this.FiltersBuy.Clear();
                        foreach (var filter in (selectedThread.config as ClassicShortConfig).FiltersBuy)
                        {
                            this.FiltersBuy.Add(filter);
                        }
                        this.TargetPointBuy = int.Parse((selectedThread.config as ClassicShortConfig).TargetPointBuy);

                        this.FiltersSell.Clear();
                        foreach (var filter in (selectedThread.config as ClassicShortConfig).FiltersSell)
                        {
                            this.FiltersSell.Add(filter);
                        }
                        this.TargetPointSell = (selectedThread.config as ClassicShortConfig).TargetPointSell;

                        this.StopLoss = (selectedThread.config as ClassicShortConfig).StopLoss;
                        this.StopLossApproximation = (selectedThread.config as ClassicShortConfig).StopLossApproximation;

                        this.FiltersStopLoss.Clear();
                        foreach (var filter in (selectedThread.config as ClassicShortConfig).FiltersStopLoss)
                        {
                            this.FiltersStopLoss.Add(filter);
                        }
                        this.TargetPointStopLoss = (selectedThread.config as ClassicShortConfig).TargetPointStopLoss;*/
                    }


                } else
                {
                    if (editMode)
                    {
                        ClickBablo = EditBabloClick;
                        BabloText = _.BabloSaveButton; // СОХРАНИТЬ
                    }
                    else
                    {
                        ClickBablo = ClickBabloAdd;
                        BabloText = _.BabloAddButton; // ДОБАВИТЬ
                    }
                }
                
            }
        }

        // Остановка после продажи
        bool isStopAfterSell;
        public bool IsStopAfterSell
        {
            get {
                return isStopAfterSell;
            }
            set
            {
                isStopAfterSell = value;
                if (this.SelectedThread != null)
                    SelectedThread.IsStopAfterSell = value;
                OnPropertyChanged();
            }
        }

        // Обновление контекстного меню (нужно, чтобы IsStopAfterSell для каждого потока отдельно работало)
        public void UpdateContextMenu()
        {
            if(this.SelectedThread != null)
                this.IsStopAfterSell = SelectedThread.IsStopAfterSell;
            else
                this.IsStopAfterSell = false;
        }

        // Счётчик рабочих потоков
        private int WorkThreadCacl()
        {
            int count = 0;
            foreach (var thread in Threads)
            {
                if (thread.Status == "work")
                    count++;
            }
            return count;
        }

        #endregion

        private bool proMode;
        public bool ProMode
        {
            get { return proMode; }
            set
            {
                proMode = value;
                OnPropertyChanged();
            }
        }
        public void ProModeChange(bool _state) // Метод которые обрабатывает событие SettingsVM.ProModeChangeEvent
        {
            ProMode = _state;
        }

        public static bool editMode = false; // Для редактирования потока. Если true то кнопка и состояние EditBabloClick()

        // Constructor
        public MainVM ()
        {
            // Запрещаем запуск приложения несколько раз
            Security.Instance();

            Settings.Load(); // Загрузка настроек

            // Загрузка настроек
            StratumBoxVM.StratumBoxEvent += this.BoxSettingsLoad; // Чтобы обновлялся при изменениях из окна боксов
            this.BoxSettingsLoad(); // Загрузка настроек по StratumBox'ам

            IterationsStr = $"{_.Iterations} ∞";
            ProfitStr = $"{_.Profits} ∞";

            this.DefaultParamInit(); // Значения полей по умолчанию

            // Получение всех доступных бирж
            AvailableExchanges.GetAll().ForEach(x => { MyAvailableExchanges.Add(x); });

            // Получение всех доступных стратегий
            AvailableStrategies.GetAll(SelectedExchange.Id).ForEach(x => { MyAvailableStrategies.Add(x); });

            // Стратегия по умолчанию
            if (SelectedStrategy.Id == Strategy.Scalping) { StrategyView = new ScalpingView(); }
            if (SelectedStrategy.Id == Strategy.Arbitrage) { StrategyView = new ArbitrageView(); }
            if (SelectedStrategy.Id == Strategy.ClassicLong) { StrategyView = new ClassicLongView(); }
            if (SelectedStrategy.Id == Strategy.ClassicShort) { StrategyView = new ClassicShortView(); }

            AddState(); // По умолчанию команда кнопки - добавить

            ClickAutoFit = new Command(AutoFitAsync); // Указываем команду для автоподбора

            LoadConfigs();

            // ThreadBackupRecovery();

            this.ProMode = Settings.ProMode; // текущий режим из настроек берем
            SettingsVM.ProModeChangeEvent += ProModeChange; // Подписываемся на событие переключения Профессианального режима

        }


        /// <summary>
        /// Метод для резервного восстановления потоков, если есть файлы бекапов
        /// </summary>
        /*private void ThreadBackupRecovery()
        {
            // Список всех найденых бекапов потоков
            List<ThreadBackup> foundThreadsForRecovery = new List<ThreadBackup>();

            string[] files = Directory.GetFiles(@"Temp/Threads/", "*.thread");

            if (files.Count() == 0)
                return;

            foreach (var file in files)
            {
                if (File.ReadAllText(file).IsValidJson())
                {
                    ThreadBackup backup = ThreadBackup.ReadBackup(File.ReadAllText(file));
                    backup.BackupFile = file;
                    foundThreadsForRecovery.Add(backup);
                }
                else
                {
                    Logger.Error(_.Msg9); // "При загрузке бекапа файл оказался невалидным"
                }
            }


            Views.ThreadRecoveryWindow recovery = new Views.ThreadRecoveryWindow(foundThreadsForRecovery);

            if (recovery.ShowDialog() == true)
            {
                if (recovery.ThreadsForRecovery != null)
                {
                    foreach (var backup in recovery.ThreadsForRecovery)
                    {
                        if (backup.IsMustRecover == true)
                        {
                            // Создаём поток с настройками если он отмечен на восстановление
                            TThread newThread = new TThread(backup);
                            newThread.NewIterationEvent += UpdateStat; // Подписываемся на событие "Новая итерация"
                            newThread.Recovering = true;
                            Threads.Add(newThread);
                            Threads.Last().Start();
                            SelectedThread = Threads.Last();
                            // Изеняем общую статистику
                            this.iterations += backup.Iteration;
                            this.profitUSD += backup.ProfitUSD;
                            //this.profitPercent += backup.ProfitPercent; // TODO FUTURE проценты суммируются, что не есть хорошо. 

                            this.IterationsStr = $"{_.Iterations} {this.iterations}";
                            this.ProfitStr = $"{_.Profits} {Calc.RoundUp(this.profitUSD, (decimal)0.00000001)}$";
                        }
                        else
                        {
                            // Удаляем если поток решили не восстанавливать
                            File.Delete(backup.BackupFile);
                        }
                    }
                }
            }
            else
            {
                // Удаление бекапов потоков, если их не нужно восстанавливать
                foreach (var file in Directory.GetFiles(@"Temp/Threads/", "*.thread"))
                {
                    File.Delete(file);
                }
            }
        }*/

        /// <summary>
        /// Значения параметров стратегии по умолчанию
        /// </summary>
        private void DefaultParamInit(IConfig _config = null)
        {
            // Общие поля
            this.Budget = _config?.Budget.ToString() ?? "1000";
            this.Cur1 = _config?.Cur1 ?? "ETH";
            this.Cur2 = _config?.Cur2 ?? "BTC";

            if ((_config as ScalpingConfig)?.IsDCA == true)
            {
                this.IsDCA = true;
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.DCAStepCount = (_config as ScalpingConfig)?.DCAStepCount.ToString();
                    DCASteps.Clear();
                    foreach (var dcaStep in (_config as ScalpingConfig).DCASteps)
                    {
                        //this.DCASteps.Add(new string[] { dcaStep[0].ToString(), dcaStep[1].ToString(), dcaStep[2].ToString() });

                        // TODO norm to DCAStep object if it can be updated
                        // TODO check if it works correctly
                        this.DCASteps.Add(
                            new string[]
                            {
                                dcaStep.Number.ToString(),
                                dcaStep.Drop.ToString(),
                                dcaStep.Amount.ToString()
                            }
                        );
                    }
                });
                this.DCAProfitPercent = (_config as ScalpingConfig)?.DCAProfitPercent + "%";
                
            }
            else
            {
                // Грузим DCA из настроек
                this.IsDCA = Settings.IsDCAAutofit;
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.DCAStepCount = Settings.DCAStepCountAutofit;
                    if(this.DCASteps == null)
                        this.DCASteps = new ObservableCollection<string[]>();
                    DCASteps.Clear();
                    foreach (var dcaStep in Settings.DCAStepsAutofit)
                    {
                        this.DCASteps.Add(new string[] { dcaStep[0], dcaStep[1], dcaStep[2] });
                    }
                });
                this.DCAProfitPercent = Settings.DCAProfitPercentAutofit + "%";
            }

            // Скальпинг
            if ((_config as ScalpingConfig)?.IsMinSpreadAsPercent == true)
            {
                this.MinSpread = (_config as ScalpingConfig)?.MinSpreadAsPercent.ToString() + "%" ?? Settings.MinSpreadScalpingAutofit + "%";
            }
            else
            {
                this.MinSpread = (_config as ScalpingConfig)?.MinSpread.ToString() ?? Settings.MinSpreadScalpingAutofit + "%";
            }

            if ((_config as ScalpingConfig)?.IsOptSpreadAsPercent == true)
            {
                this.OptSpread = (_config as ScalpingConfig)?.OptSpreadAsPercent.ToString() + "%" ?? Settings.OptSpreadScalpingAutofit + "%";
            }
            else
            {
                this.OptSpread = (_config as ScalpingConfig)?.OptSpread.ToString() ?? Settings.OptSpreadScalpingAutofit + "%";
            }

            if ((_config as ScalpingConfig)?.IsMinMarkupAsPercent == true)
            {
                this.MinMarkup = (_config as ScalpingConfig)?.MinMarkupAsPercent.ToString() + "%" ?? Settings.MinMarkupScalpingAutofit + "%";
            }
            else
            {
                this.MinMarkup = (_config as ScalpingConfig)?.MinMarkup.ToString() ?? Settings.MinMarkupScalpingAutofit + "%";
            }

            if ((_config as ScalpingConfig)?.IsOptMarkupAsPercent == true)
            {
                this.OptMarkup = (_config as ScalpingConfig)?.OptMarkupAsPercent.ToString() + "%" ?? Settings.OptMarkupScalpingAutofit + "%";
            }
            else
            {
                this.OptMarkup = (_config as ScalpingConfig)?.OptMarkup.ToString() ?? Settings.OptMarkupScalpingAutofit + "%";
            }

            if ((_config as ScalpingConfig)?.IsZeroSellAsPercent == true)
            {
                this.ZeroSell = (_config as ScalpingConfig)?.ZeroSellAsPercent.ToString() + "%" ?? Settings.ZeroSellScalpingAutofit + "%";
            }
            else
            {
                this.ZeroSell = (_config as ScalpingConfig)?.ZeroSell.ToString() ?? Settings.ZeroSellScalpingAutofit + "%";
            }

            this.InTimeout = (_config as ScalpingConfig)?.InTimeout.ToString() ?? Settings.InTimeoutScalpingAutofit;
            this.FirsOredersAmountPercentIgnor = (_config as ScalpingConfig)?.FirsOredersAmountPercentIgnor.ToString() ?? "30%";
            this.FirsOredersCountIgnor = (_config as ScalpingConfig)?.FirsOredersCountIgnor.ToString() ?? "4";
            this.StopLoss = ( ((_config as ScalpingConfig)?.IsStopLossAsPercent != null && (_config as ScalpingConfig).IsStopLossAsPercent) ? (_config as ScalpingConfig) ?.StopLossPercent.ToString() + "%" : (_config as ScalpingConfig)?.StopLossPrice.ToString() ) ?? "0";
            //this.IsSellStart = (_config as ScalpingConfigJson)?.IsSellStart ?? false;
            //this.BuyPriceForSellStart = (_config as ScalpingConfigJson)?.BuyPriceForSellStartStr ?? "0";
            //this.AmountForSellStart = (_config as ScalpingConfigJson)?.AmountForSellStartStr ?? "0";

            // TODO не знаю что тут добавить по умолчанию для фильтров
            // this.TargetPoint = config.TargetPoint;

            // Classic Long
            this.DCAFilters = (_config as ClassicLongConfig)?.Filters.DCAFilters ?? new Dictionary<int, DCAFilter>(); // dcaf
            this.TargetProfitPercent = (_config as ClassicLongConfig)?.TargetProfitPercent.ToString() ?? "1%";
            this.IsProfitTrailing = (_config as ClassicLongConfig)?.IsProfitTrailing ?? false;
            this.TrailStepPercent = (_config as ClassicLongConfig)?.TrailStepPercent.ToString() ?? "0.2%";
            this.ApproximationPercent = (_config as ClassicLongConfig)?.ApproximationPercent.ToString() ?? "0.1%";
            this.UnApproximationPercent = (_config as ClassicLongConfig)?.UnApproximationPercent.ToString() ?? "0.2%";
            this.IsMarketBuy = (_config as ClassicLongConfig)?.IsMarketBuy ?? true;
            this.StopLoss = (_config as ClassicLongConfig)?.StopLossPrice.ToString() ?? "0"; // тут было StopLoss без Price на конце до 3,13
            //this.IsStopLoss = (this.StopLoss == "0") ? false : true;
            this.IsStopLoss = false;
            this.StopLossApproximation = (_config as ClassicLongConfig)?.StopLossApproximation.ToString() ?? "0";
            // Classic Short
            this.IsMarketSell = (_config as ClassicShortConfig)?.IsMarketSell ?? true;
        }

        #region Commands

        // Кнопка Бабло // Для биндинга разных команд на кнопку
        private ICommand сlickBablo;
        public ICommand ClickBablo
        {
            get { return сlickBablo; }
            set
            {
                сlickBablo = value;
                OnPropertyChanged();
            }
        }

        // Кнопка Бабло: Добавить поток
        public ICommand ClickBabloAdd
        {
            get
            {
                return new Command((obj) =>
                {
                    Logger.ToFile("Add thread");

                    if ( ( (SelectedStrategy?.Id == Strategy.ClassicLong && this.FiltersSell.Count > 0) || (SelectedStrategy?.Id == Strategy.ClassicShort && this.FiltersBuy.Count > 0) ) && this.isProfitTrailing == true)
                    {
                        MessageBox.Show("Закрытие сделок либо по фильтрам либо по трейлингу.", "Противоречие!"); // TODO TEXT
                        return;
                    }
                        

                    // Создаём поток с настройками
                    TThread newThread = new TThread(this.GetConfigByParam(), this.SelectedExchange.Id);
                    newThread.NewIterationEvent += UpdateStat; // Подписываемся на событие "Новая итерация"
                    Threads.Add(newThread);
                    SelectedThread = Threads.Last();

                }, x => locker != true);
            }
        }

        // Кнопка Бабло: Запустить поток
        public ICommand ClickBabloStart
        {
            get
            {
                return new Command((obj) =>
                {
                    Logger.Info(_.Log1);
                    Logger.ToFile(SelectedExchange.Id + " + " + SelectedStrategy.Id);

                    if (WorkThreadCacl() >= Encryption.HEXToNum(Conv.GetClearInt(_.Msg20)))
                        return; // Ограничение по потокам
                    
                    Threads[Threads.IndexOf(SelectedThread)].Start(); // запускаю поток
                    Threads[Threads.IndexOf(SelectedThread)].Status = "work";

                    ClickBablo = ClickBabloStop;
                    BabloText = _.BabloStopButton; // ОСТАНОВИТЬ

                }, x => Threads[Threads.IndexOf(SelectedThread)].Status == "stop" || Threads[Threads.IndexOf(SelectedThread)].Status == "draft");
            }
        }

        // Сохраняем изменения конфига
        public ICommand EditBabloClick
        {
            get
            {
                return new Command((obj) =>
                {
                    Logger.Info(_.Log3);

                    // Обновить настройки
                    Threads[Threads.IndexOf(SelectedThread)].TThreadAgain(this.GetConfigByParam(), this.SelectedExchange.Id);
                    Threads[Threads.IndexOf(SelectedThread)].Status = "stop";
                    editMode = false;
                    this.WorkState();
                });
            }
        }
        // Включаем режим редактирования
        public ICommand EditModeClick
        {
            get
            {
                return new Command((obj) =>
                {
                    if (SelectedThread == null)
                        return;

                    if (Threads[Threads.IndexOf(SelectedThread)].Status == "work")
                    {
                        MessageBox.Show("Невозможно отредактировать параметры стратегии потока пока он запущен!"); // TODO text
                        return;
                    }


                    Logger.ToFile("editMode = true");
                    editMode = true;
                    Threads[Threads.IndexOf(SelectedThread)].Status = "edit";
                    this.WorkState();

                });
            }
        }



        /// <summary>
        /// Метод подготоваливает и возвращает конфиг для выбраной стратегии на основе указаных параметров в полях
        /// </summary>
        /// <returns></returns>
        private IConfig GetConfigByParam()
        {
            IConfig config = null; // Инициализируем переменную конфига
            IConfigText textConfig = null; // Инициализируем переменную конфига

            if (SelectedStrategy.Id == Strategy.Scalping)
            {
                textConfig = new ScalpingConfigText()
                {
                    Strategy = Strategy.Scalping.ToString(),
                    /*Exchange = this.SelectedExchange.Id,*/
                    Budget = this.Budget,
                    Cur1 = this.Cur1,
                    Cur2 = this.Cur2,
                    IsDCA = this.IsDCA,
                    DCAStepCount = this.DCAStepCount,
                    DCAProfitPercent = this.DCAProfitPercent,
                    DCASteps = new ObservableCollection<string[]>(this.DCASteps),
                    DCAFilters = new Dictionary<int, DCAFilter>(this.DCAFilters),
                    MinSpread = this.MinSpread,
                    OptSpread = this.OptSpread,
                    MinMarkup = this.MinMarkup,
                    OptMarkup = this.OptMarkup,
                    ZeroSell = this.ZeroSell,
                    InTimeout = this.InTimeout,
                    FirsOredersAmountPercentIgnor = this.FirsOredersAmountPercentIgnor,
                    FirsOredersCountIgnor = this.FirsOredersCountIgnor,
                    IsStopLoss = this.IsStopLoss,
                    StopLoss = this.StopLoss,
                    //IsSellStart = this.IsSellStart,
                    //BuyPriceForSellStart = this.BuyPriceForSellStart,
                    //AmountForSellStart = this.AmountForSellStart,
                    FiltersBuy = new List<JsonFilter>(this.FiltersBuy),
                    TargetPointBuy = this.TargetPointBuy
                };

                config = ConfigManager.ConfigTextToConfig(textConfig);

                //config.Strategy = new Scalping(config as ScalpingConfig);

            }
            if (SelectedStrategy.Id == Strategy.ClassicLong)
            {
                textConfig = new ClassicLongConfigText()
                {
                    Strategy = Strategy.ClassicLong.ToString(),
                    //Exchange = this.SelectedExchange.Id,
                    Budget = this.Budget,
                    Cur1 = this.Cur1,
                    Cur2 = this.Cur2,
                    TargetProfitPercent = this.TargetProfitPercent,
                    IsProfitTrailing = this.IsProfitTrailing,
                    TrailStepPercent = this.TrailStepPercent,
                    ApproximationPercent = this.ApproximationPercent,
                    UnApproximationPercent = this.UnApproximationPercent,
                    IsMarketBuy = this.IsMarketBuy,
                    IsDCA = this.IsDCA,
                    DCAStepCount = this.DCAStepCount,
                    DCAProfitPercent = this.TargetProfitPercent,//this.DCAProfitPercent,
                    DCASteps = new ObservableCollection<string[]>(this.DCASteps),
                    DCAFilters = new Dictionary<int, DCAFilter>(this.DCAFilters),
                    FiltersBuy = new List<JsonFilter>(this.FiltersBuy),
                    TargetPointBuy = this.TargetPointBuy,
                    FiltersSell = new List<JsonFilter>(this.FiltersSell),
                    TargetPointSell = this.TargetPointSell,
                    IsStopLoss = this.IsStopLoss,
                    StopLoss = this.StopLoss,
                    StopLossApproximation = this.StopLossApproximation,
                    FiltersStopLoss = new List<JsonFilter>(this.FiltersStopLoss),
                    TargetPointStopLoss = this.TargetPointStopLoss
                };

                config = ConfigManager.ConfigTextToConfig(textConfig);

                /*config = new ClassicLongConfig()
                {
                    Strategy = Strategy.ClassicLong,
                    Exchange = this.SelectedExchange.Id,
                    Budget = this.Budget,
                    Cur1 = this.Cur1,
                    Cur2 = this.Cur2,
                    TargetProfitPercent = this.TargetProfitPercent,
                    IsProfitTrailing = this.IsProfitTrailing,
                    TrailStepPercent = this.TrailStepPercent,
                    ApproximationPercent = this.ApproximationPercent,
                    UnApproximationPercent = this.UnApproximationPercent,
                    IsMarketBuy = this.IsMarketBuy,
                    IsDCA = this.IsDCA,
                    DCAStepCountStr = this.DCAStepCount,
                    DCAProfitPercentStr = this.TargetProfitPercent,//this.DCAProfitPercent,
                    DCAStepsStr = new List<string[]>(this.DCASteps),
                    DCAFilters = new Dictionary<int, DCAFilter>(this.DCAFilters),
                    FiltersBuy = new List<JsonFilter>(this.FiltersBuy),
                    TargetPointBuy = this.TargetPointBuy.ToString(),
                    FiltersSell = new List<JsonFilter>(this.FiltersSell),
                    TargetPointSell = this.TargetPointSell,
                    StopLoss = this.StopLoss,
                    StopLossApproximation = this.StopLossApproximation,
                    FiltersStopLoss = new List<JsonFilter>(this.FiltersStopLoss),
                    TargetPointStopLoss = this.TargetPointStopLoss
                };*/



            }
            if (SelectedStrategy.Id == Strategy.ClassicShort)
            {

                textConfig = new ClassicShortConfigText()
                {
                    Strategy = Strategy.ClassicShort.ToString(),
                    //Exchange = this.SelectedExchange.Id,
                    Budget = this.Budget,
                    Cur1 = this.Cur1,
                    Cur2 = this.Cur2,
                    TargetProfitPercent = this.TargetProfitPercent,
                    IsProfitTrailing = this.IsProfitTrailing,
                    TrailStepPercent = this.TrailStepPercent,
                    ApproximationPercent = this.ApproximationPercent,
                    UnApproximationPercent = this.UnApproximationPercent,
                    //IsMarketBuy = this.IsMarketBuy,
                    IsMarketSell = this.IsMarketSell,
                    IsDCA = this.IsDCA,
                    DCAStepCount = this.DCAStepCount,
                    DCAProfitPercent = this.TargetProfitPercent,//this.DCAProfitPercent,
                    DCASteps = new ObservableCollection<string[]>(this.DCASteps),
                    DCAFilters = new Dictionary<int, DCAFilter>(this.DCAFilters),
                    FiltersBuy = new List<JsonFilter>(this.FiltersBuy),
                    TargetPointBuy = this.TargetPointBuy,
                    FiltersSell = new List<JsonFilter>(this.FiltersSell),
                    TargetPointSell = this.TargetPointSell,
                    IsStopLoss = this.IsStopLoss,
                    StopLoss = this.StopLoss,
                    StopLossApproximation = this.StopLossApproximation,
                    FiltersStopLoss = new List<JsonFilter>(this.FiltersStopLoss),
                    TargetPointStopLoss = this.TargetPointStopLoss
                };

                config = ConfigManager.ConfigTextToConfig(textConfig);

                /*config = new ClassicShortConfig()
                {
                    Strategy = Strategy.ClassicShort,
                    Exchange = this.SelectedExchange.Id,
                    Budget = this.Budget,
                    Cur1 = this.Cur1,
                    Cur2 = this.Cur2,
                    TargetProfitPercent = this.TargetProfitPercent,
                    IsProfitTrailing = this.IsProfitTrailing,
                    TrailStepPercent = this.TrailStepPercent,
                    ApproximationPercent = this.ApproximationPercent,
                    UnApproximationPercent = this.UnApproximationPercent,
                    IsMarketSell = this.IsMarketSell,
                    IsDCA = this.IsDCA,
                    DCAStepCountStr = this.DCAStepCount,
                    DCAProfitPercentStr = this.TargetProfitPercent,//this.DCAProfitPercent,
                    DCAStepsStr = new List<string[]>(this.DCASteps),
                    DCAFilters = new Dictionary<int, DCAFilter>(this.DCAFilters),
                    FiltersBuy = new List<JsonFilter>(this.FiltersBuy),
                    TargetPointBuy = this.TargetPointBuy.ToString(),
                    FiltersSell = new List<JsonFilter>(this.FiltersSell),
                    TargetPointSell = this.TargetPointSell,
                    StopLoss = this.StopLoss,
                    StopLossApproximation = this.StopLossApproximation,
                    FiltersStopLoss = new List<JsonFilter>(this.FiltersStopLoss),
                    TargetPointStopLoss = this.TargetPointStopLoss
                };*/
            }

            return config;
        }

        // Кнопка Бабло: Запустить поток
        public ICommand ClickBabloStop
        {
            get
            {
                return new Command((obj) =>
                {
                    Logger.Info(_.Log2);

                    Threads[Threads.IndexOf(SelectedThread)].Stop(); // запускаю поток
                    Threads[Threads.IndexOf(SelectedThread)].Status = "stopping";
                    //ThreadBackup.Delete(int.Parse(Threads[Threads.IndexOf(SelectedThread)].TID));
                    ClickBablo = ClickBabloStart;
                    BabloText = _.BabloStartButton; // ЗАПУСТИТЬ

                    this.DeleteAllOrdersByTid(int.Parse(SelectedThread.TID));
                });
            }
        }

        // Состояние добавления потока
        public void AddState()
        {
            if (editMode)
            {
                ClickBablo = EditBabloClick;
                BabloText = _.BabloSaveButton; // СОХРАНИТЬ
            }
            else
            {
                ClickBablo = ClickBabloAdd;
                BabloText = _.BabloAddButton; // ДОБАВИТЬ
            }
        }

        // Состояние работы с уже добавленными потоками
        public void WorkState()
        {
            if (selectedThread != null)
            {
                if (selectedThread.Status == "draft" || selectedThread.Status == "stop")
                {
                    ClickBablo = ClickBabloStart;
                    BabloText = _.BabloStartButton; // ЗАПУСТИТЬ
                }
                if (selectedThread.Status == "work")
                {
                    ClickBablo = ClickBabloStop;
                    BabloText = _.BabloStopButton; // ОСТАНОВИТЬ
                }
                if(selectedThread.Status == "edit")
                {
                    ClickBablo = EditBabloClick;
                    BabloText = _.BabloSaveButton; // СОХРАНИТЬ
                }
            }
            else
            {
                if(editMode)
                {
                    ClickBablo = EditBabloClick;
                    BabloText = _.BabloSaveButton; // СОХРАНИТЬ
                } else
                {
                    ClickBablo = ClickBabloAdd;
                    BabloText = _.BabloAddButton; // ДОБАВИТЬ
                }
                
            }
        }


        // Контекстное меню потоков: Удалить
        public ICommand ClickThreadDelete
        {
            get
            {
                return new Command((obj) =>
                {

                    Logger.ToFile("Thread delete");
                    if (SelectedThread == null)
                        return;

                    Logger.Info(_.Log2);
                    Threads[Threads.IndexOf(SelectedThread)].Stop(); // запускаю поток
                    Threads[Threads.IndexOf(SelectedThread)].Status = "stop";
                    //ThreadBackup.Delete(int.Parse(Threads[Threads.IndexOf(SelectedThread)].TID));
                    ClickBablo = ClickBabloAdd;
                    BabloText = _.BabloAddButton; // ДОБАВИТЬ

                    SelectedThread.NewIterationEvent -= UpdateStat; // Отписываемся от события "Новая итерация"
                    this.Threads.Remove(SelectedThread);
                });
            }
        }


        // Кнопка: Автоподбор параметров стратегии (Асинхронно)
        public ICommand ClickAutoFit { get; set; }
        async void AutoFitAsync(object x)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                await System.Threading.Tasks.Task.Run(() =>
                {
                    DefaultParamInit(AutoFit.GetConfig(SelectedStrategy.Id, SelectedExchange.Id, this.Cur1, this.Cur2, this.Budget));
                });
                MessageBox.Show(_.Msg1);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }


        // Кнопка Сохранить параметры стратегии
        public ICommand ClickSaveConfig
        {
            get
            {
                return new Command((obj) =>
                {
                    Logger.ToFile("Save config");

                    // Скальпинг
                    if (SelectedStrategy.Id == Strategy.Scalping)
                    {
                        IConfigText config = this.GetIConfigTextFromFields();
                        /*IConfigText config = new ScalpingConfigText() 
                        {
                            Strategy = Strategy.Scalping.ToString(),
                            Budget = this.Budget,
                            Cur1 = this.Cur1,
                            Cur2 = this.Cur2,
                            MinSpread = this.MinSpread,
                            OptSpread = this.OptSpread,
                            MinMarkup = this.MinMarkup,
                            OptMarkup = this.OptMarkup,
                            ZeroSell = this.ZeroSell,
                            InTimeout = this.InTimeout,
                            IsDCA = this.IsDCA,
                            DCAStepCount = this.DCAStepCount,
                            DCAProfitPercent = this.DCAProfitPercent,
                            DCASteps = this.DCASteps,
                            DCAFilters = this.DCAFilters,
                            FirsOredersAmountPercentIgnor = this.FirsOredersAmountPercentIgnor,
                            FirsOredersCountIgnor = this.FirsOredersCountIgnor,
                            StopLoss = this.StopLoss,
                            FiltersBuy = new List<JsonFilter>(this.FiltersBuy), // TODO INDICATORS тут добавил и надо везде
                            TargetPointBuy = this.TargetPointBuy
                        };*/

                        // Save config 
                        Views.SaveConfigWindow save = new Views.SaveConfigWindow(this.Cur1, this.Cur2);
                        if (save.ShowDialog() == true)
                        {
                            if (save.FileName != null)
                            {
                                ConfigManager.Save(save.FileName, config);
                                StrategyConfigs.Add(save.FileName);
                            }
                            
                            MessageBox.Show(_.Msg2);
                        }
                    }

                    // Classic Long
                    if (SelectedStrategy.Id == Strategy.ClassicLong)
                    {
                        IConfigText config = new ClassicLongConfigText()
                        {
                            Strategy = Strategy.ClassicLong.ToString(),
                            Budget = this.Budget,
                            Cur1 = this.Cur1,
                            Cur2 = this.Cur2,
                            TargetProfitPercent = this.TargetProfitPercent,
                            IsProfitTrailing = this.IsProfitTrailing,
                            TrailStepPercent = this.TrailStepPercent,
                            ApproximationPercent = this.ApproximationPercent,
                            UnApproximationPercent = this.UnApproximationPercent,
                            IsMarketBuy = this.IsMarketBuy,
                            IsDCA = this.IsDCA,
                            TargetPointBuy = this.TargetPointBuy,
                            DCAStepCount = this.DCAStepCount,
                            DCAProfitPercent = this.TargetProfitPercent,//this.DCAProfitPercent,
                            DCASteps = this.DCASteps,
                            DCAFilters = this.DCAFilters,
                            FiltersBuy = new List<JsonFilter>(this.FiltersBuy), // TODO INDICATORS тут добавил и надо везде
                            FiltersSell = new List<JsonFilter>(this.FiltersSell), // TODO INDICATORS тут добавил и надо везде
                            TargetPointSell = this.TargetPointSell,
                            IsStopLoss = this.IsStopLoss,
                            StopLoss = this.StopLoss,
                            StopLossApproximation = this.StopLossApproximation,
                            FiltersStopLoss = new List<JsonFilter>(this.FiltersStopLoss), // TODO INDICATORS тут добавил и надо везде
                            TargetPointStopLoss = this.TargetPointStopLoss
                        };

                        // Save config 
                        Views.SaveConfigWindow save = new Views.SaveConfigWindow(this.Cur1, this.Cur2);
                        if (save.ShowDialog() == true)
                        {
                            if (save.FileName != null)
                            {
                                ConfigManager.Save(save.FileName, config);
                                StrategyConfigs.Add(save.FileName);
                            }

                            MessageBox.Show(_.Msg2);
                        }
                    }

                    // Classic Short
                    if (SelectedStrategy.Id == Strategy.ClassicShort)
                    {
                        IConfigText config = new ClassicShortConfigText()
                        {
                            Strategy = Strategy.ClassicShort.ToString(),
                            Budget = this.Budget,
                            Cur1 = this.Cur1,
                            Cur2 = this.Cur2,
                            TargetProfitPercent = this.TargetProfitPercent,
                            IsProfitTrailing = this.IsProfitTrailing,
                            TrailStepPercent = this.TrailStepPercent,
                            ApproximationPercent = this.ApproximationPercent,
                            UnApproximationPercent = this.UnApproximationPercent,
                            IsMarketSell = this.IsMarketSell,
                            IsDCA = this.IsDCA,
                            DCAStepCount = this.DCAStepCount,
                            DCAProfitPercent = this.TargetProfitPercent,//this.DCAProfitPercent,
                            DCASteps = this.DCASteps,
                            DCAFilters = this.DCAFilters,
                            FiltersBuy = new List<JsonFilter>(this.FiltersBuy), // TODO INDICATORS тут добавил и надо везде
                            TargetPointBuy = this.TargetPointBuy,
                            FiltersSell = new List<JsonFilter>(this.FiltersSell), // TODO INDICATORS тут добавил и надо везде
                            TargetPointSell = this.TargetPointSell,
                            IsStopLoss = this.IsStopLoss,
                            StopLoss = this.StopLoss,
                            StopLossApproximation = this.StopLossApproximation,
                            FiltersStopLoss = new List<JsonFilter>(this.FiltersStopLoss), // TODO INDICATORS тут добавил и надо везде
                            TargetPointStopLoss = this.TargetPointStopLoss
                        };

                        // Save config 
                        Views.SaveConfigWindow save = new Views.SaveConfigWindow(this.Cur1, this.Cur2);
                        if (save.ShowDialog() == true)
                        {
                            if (save.FileName != null)
                            {
                                ConfigManager.Save(save.FileName, config);
                                StrategyConfigs.Add(save.FileName);
                            }

                            MessageBox.Show(_.Msg2);
                        }
                    }

                });
            }
        }

        // Кнопка Открыть/Загрузить файл конфига
        public ICommand ClickOpenConfig
        {
            get
            {
                return new Command((obj) =>
                {
                    Logger.Info("Open config");

                    // Open config file
                    var openFileDialog = new Microsoft.Win32.OpenFileDialog();
                    openFileDialog.DefaultExt = ".strat";
                    openFileDialog.Filter = "Stratum-bot (.strat)|*.strat";

                    if(openFileDialog.ShowDialog() == true)
                    {
                        string filepath = openFileDialog.FileName;
                        string filename = openFileDialog.SafeFileName.Replace(".strat", "");
                        string configFile = File.ReadAllText(filepath);

                        if(!configFile.IsValidJson())
                        {
                            MessageBox.Show(_.Msg5); // С этим файлом что-то не так...
                            return;
                        }

                        // Считать конфиг
                        var config = ConfigManager.Read(configFile);

                        // Сохранить конфиг как новый в локальную директорию
                        ConfigManager.Save(filename, config);

                        // Доступна ли стратегия из конфига для выбранной биржи
                        Enum.TryParse(config.Strategy, out Strategy strategyId);
                        if (!AvailableStrategies.IsExchangeCompatible(strategyId, SelectedExchange.Id))
                        {
                            MessageBox.Show(_.Msg4); // Для данной биржи эта стратегия недоступна
                            return;
                        }

                        // Scalping
                        if (config.Strategy == Strategy.Scalping.ToString())
                        {

                            this.FillFieldsFromIConfigText(config);

                            /*this.Budget = (config as ScalpingConfigText).Budget;
                            this.Cur1 = (config as ScalpingConfigText).Cur1;
                            this.Cur2 = (config as ScalpingConfigText).Cur2;
                            this.MinSpread = (config as ScalpingConfigText).MinSpread;
                            this.OptSpread = (config as ScalpingConfigText).OptSpread;
                            this.MinMarkup = (config as ScalpingConfigText).MinMarkup;
                            this.OptMarkup = (config as ScalpingConfigText).OptMarkup;
                            this.ZeroSell = (config as ScalpingConfigText).ZeroSell;
                            this.InTimeout = (config as ScalpingConfigText).InTimeout;
                            this.IsDCA = (config as ScalpingConfigText).IsDCA;
                            this.DCAStepCount = (config as ScalpingConfigText).DCAStepCount;
                            this.DCAProfitPercent = (config as ScalpingConfigText).DCAProfitPercent;
                            this.DCASteps = (config as ScalpingConfigText).DCASteps;
                            this.DCAFilters = new Dictionary<int, DCAFilter>((config as ScalpingConfigText).DCAFilters); //dcaf
                            this.FirsOredersAmountPercentIgnor = (config as ScalpingConfigText).FirsOredersAmountPercentIgnor;
                            this.FirsOredersCountIgnor = (config as ScalpingConfigText).FirsOredersCountIgnor;
                            this.StopLoss = (config as ScalpingConfigText).StopLoss;
                            this.FiltersBuy.Clear();
                            foreach (var filter in (config as ScalpingConfigText).FiltersBuy)
                            {
                                this.FiltersBuy.Add(filter);
                            }
                            this.TargetPointBuy = (config as ScalpingConfigText).TargetPointBuy;*/
                        }

                        // Classic Long
                        if (config.Strategy == Strategy.ClassicLong.ToString())
                        {
                            this.Budget = (config as ClassicLongConfigText).Budget;
                            this.Cur1 = (config as ClassicLongConfigText).Cur1;
                            this.Cur2 = (config as ClassicLongConfigText).Cur2;

                            this.TargetProfitPercent = (config as ClassicLongConfigText).TargetProfitPercent;
                            this.IsProfitTrailing = (config as ClassicLongConfigText).IsProfitTrailing;
                            this.TrailStepPercent = (config as ClassicLongConfigText).TrailStepPercent;
                            this.ApproximationPercent = (config as ClassicLongConfigText).ApproximationPercent;
                            this.UnApproximationPercent = (config as ClassicLongConfigText).UnApproximationPercent;
                            this.IsMarketBuy = (config as ClassicLongConfigText).IsMarketBuy;

                            this.IsDCA = (config as ClassicLongConfigText).IsDCA;
                            this.DCAStepCount = (config as ClassicLongConfigText).DCAStepCount;
                            this.DCAProfitPercent = (config as ClassicLongConfigText).DCAProfitPercent;
                            this.DCASteps = (config as ClassicLongConfigText).DCASteps;
                            this.DCAFilters = new Dictionary<int, DCAFilter>((config as ClassicLongConfigText).DCAFilters); //dcaf

                            this.FiltersBuy.Clear();
                            foreach (var filter in (config as ClassicLongConfigText).FiltersBuy)
                            {
                                this.FiltersBuy.Add(filter);
                            }
                            this.TargetPointBuy = (config as ClassicLongConfigText).TargetPointBuy;

                            this.FiltersSell.Clear();
                            foreach (var filter in (config as ClassicLongConfigText).FiltersSell)
                            {
                                this.FiltersSell.Add(filter);
                            }
                            this.TargetPointSell = (config as ClassicLongConfigText).TargetPointSell;

                            this.StopLoss = (config as ClassicLongConfigText).StopLoss;
                            this.StopLossApproximation = (config as ClassicLongConfigText).StopLossApproximation;

                            this.FiltersStopLoss.Clear();
                            foreach (var filter in (config as ClassicLongConfigText).FiltersStopLoss)
                            {
                                this.FiltersStopLoss.Add(filter);
                            }
                            this.TargetPointStopLoss = (config as ClassicLongConfigText).TargetPointStopLoss;
                        }

                        // Classic Short
                        if (config.Strategy == Strategy.ClassicShort.ToString())
                        {
                            this.Budget = (config as ClassicShortConfigText).Budget;
                            this.Cur1 = (config as ClassicShortConfigText).Cur1;
                            this.Cur2 = (config as ClassicShortConfigText).Cur2;

                            this.TargetProfitPercent = (config as ClassicShortConfigText).TargetProfitPercent;
                            this.IsProfitTrailing = (config as ClassicShortConfigText).IsProfitTrailing;
                            this.TrailStepPercent = (config as ClassicShortConfigText).TrailStepPercent;
                            this.ApproximationPercent = (config as ClassicShortConfigText).ApproximationPercent;
                            this.UnApproximationPercent = (config as ClassicShortConfigText).UnApproximationPercent;
                            this.IsMarketSell = (config as ClassicShortConfigText).IsMarketSell;

                            this.IsDCA = (config as ClassicShortConfigText).IsDCA;
                            this.DCAStepCount = (config as ClassicShortConfigText).DCAStepCount;
                            this.DCAProfitPercent = (config as ClassicShortConfigText).DCAProfitPercent;
                            this.DCASteps = (config as ClassicShortConfigText).DCASteps;

                            this.DCAFilters = new Dictionary<int, DCAFilter>((config as ClassicShortConfigText).DCAFilters); //dcaf
                            this.FiltersBuy.Clear();
                            foreach (var filter in (config as ClassicShortConfigText).FiltersBuy)
                            {
                                this.FiltersBuy.Add(filter);
                            }
                            this.TargetPointBuy = (config as ClassicShortConfigText).TargetPointBuy;

                            this.FiltersSell.Clear();
                            foreach (var filter in (config as ClassicShortConfigText).FiltersSell)
                            {
                                this.FiltersSell.Add(filter);
                            }
                            this.TargetPointSell = (config as ClassicShortConfigText).TargetPointSell;

                            this.StopLoss = (config as ClassicShortConfigText).StopLoss;
                            this.StopLossApproximation = (config as ClassicShortConfigText).StopLossApproximation;

                            this.FiltersStopLoss.Clear();
                            foreach (var filter in (config as ClassicShortConfigText).FiltersStopLoss)
                            {
                                this.FiltersStopLoss.Add(filter);
                            }
                            this.TargetPointStopLoss = (config as ClassicShortConfigText).TargetPointStopLoss;
                        }

                        // Выбираем стратегию из конфига
                        foreach (var strategy in MyAvailableStrategies)
                        {
                            if (strategy.Id == strategyId)
                            {
                                SelectedStrategy = strategy;
                                break;
                            }
                        }
                        // Выбрать конфиг из списка
                        foreach(var selectConfig in StrategyConfigs)
                        {
                            if(selectConfig == filename)
                            {
                                SelectedStrategyConfig = selectConfig;
                                break;
                            }
                        }
                        MessageBox.Show(_.Msg3); // Конфиг загружён!
                        
                    }

                });
            }
        }

        // Кнопка: Настройки
        public ICommand SettingsClick
        {
            get
            {
                return new Command((obj) =>
                {
                    Views.SettingsWindow settingsWindow = new Views.SettingsWindow();
                    settingsWindow.Show();
                });
            }
        }

        // Кнопка: StratumBox
        public ICommand StratumBoxClick
        {
            get
            {
                return new Command((obj) =>
                {
                    Views.StratumBoxWindow stratumBoxWindow = new Views.StratumBoxWindow();
                    stratumBoxWindow.Show();
                });
            }
        }

        // Кнопка: Помощь Telegram Кабинет
        public ICommand GotoPageClick
        {
            get
            {
                return new Command((obj) =>
                {
                    if (obj.ToString() == "help")
                    {
                        try { System.Diagnostics.Process.Start(_.BtnPlusWikiUrl); }
                        catch { }
                    }
                    if (obj.ToString() == "telegram")
                    {
                        try { System.Diagnostics.Process.Start(_.TelegramChannelUrl); }
                        catch { }
                    }
                    if (obj.ToString() == "cabinet")
                    {
                        try { System.Diagnostics.Process.Start(_.BtnPlusLoginUrl); }
                        catch { }
                    }
                });
            }
        }

        // Кнопка Обновить файл конфига
        public ICommand ClickUpdateConfig
        {
            get
            {
                return new Command((obj) =>
                {
                    Logger.ToFile("Update config");

                    // Скальпинг
                    if (SelectedStrategy.Id == Strategy.Scalping)
                    {
                        IConfigText config = new ScalpingConfigText()
                        {
                            Strategy = Strategy.Scalping.ToString(),
                            Budget = this.Budget,
                            Cur1 = this.Cur1,
                            Cur2 = this.Cur2,
                            MinSpread = this.MinSpread,
                            OptSpread = this.OptSpread,
                            MinMarkup = this.MinMarkup,
                            OptMarkup = this.OptMarkup,
                            ZeroSell = this.ZeroSell,
                            InTimeout = this.InTimeout,
                            IsDCA = this.IsDCA,
                            DCAStepCount = this.DCAStepCount,
                            DCAProfitPercent = this.DCAProfitPercent,
                            DCASteps = this.DCASteps,
                            DCAFilters = this.DCAFilters,
                            FirsOredersAmountPercentIgnor = this.FirsOredersAmountPercentIgnor,
                            FirsOredersCountIgnor = this.FirsOredersCountIgnor,
                            StopLoss = this.StopLoss,
                            FiltersBuy = new List<JsonFilter>(this.FiltersBuy),
                            TargetPointBuy = this.TargetPointBuy
                        };

                        // Update config
                        ConfigManager.Save(SelectedStrategyConfig, config);
                        MessageBox.Show(_.Msg6); // Конфиг обновлён!
                    }


                    // Classic Long
                    if (SelectedStrategy.Id == Strategy.ClassicLong)
                    {

                        IConfigText config = new ClassicLongConfigText()
                        {
                            Strategy = Strategy.ClassicLong.ToString(),
                            Budget = this.Budget,
                            Cur1 = this.Cur1,
                            Cur2 = this.Cur2,
                            TargetProfitPercent = this.TargetProfitPercent,
                            IsProfitTrailing = this.IsProfitTrailing,
                            TrailStepPercent = this.TrailStepPercent,
                            ApproximationPercent = this.ApproximationPercent,
                            UnApproximationPercent = this.UnApproximationPercent,
                            IsMarketBuy = this.IsMarketBuy,
                            IsDCA = this.IsDCA,
                            DCAStepCount = this.DCAStepCount,
                            DCAProfitPercent = this.DCAProfitPercent,
                            DCASteps = this.DCASteps,
                            DCAFilters = this.DCAFilters,
                            FiltersBuy = new List<JsonFilter>(this.FiltersBuy),
                            TargetPointBuy = this.TargetPointBuy,
                            FiltersSell = new List<JsonFilter>(this.FiltersSell),
                            TargetPointSell = this.TargetPointSell,
                            StopLoss = this.StopLoss,
                            StopLossApproximation = this.StopLossApproximation,
                            FiltersStopLoss = new List<JsonFilter>(this.FiltersStopLoss),
                            TargetPointStopLoss = this.TargetPointStopLoss
                        };

                        // Update config
                        ConfigManager.Save(SelectedStrategyConfig, config);
                        MessageBox.Show(_.Msg6); // Конфиг обновлён!
                    }


                    // Classic Short
                    if (SelectedStrategy.Id == Strategy.ClassicShort)
                    {

                        IConfigText config = new ClassicShortConfigText()
                        {
                            Strategy = Strategy.ClassicShort.ToString(),
                            Budget = this.Budget,
                            Cur1 = this.Cur1,
                            Cur2 = this.Cur2,
                            TargetProfitPercent = this.TargetProfitPercent,
                            IsProfitTrailing = this.IsProfitTrailing,
                            TrailStepPercent = this.TrailStepPercent,
                            ApproximationPercent = this.ApproximationPercent,
                            UnApproximationPercent = this.UnApproximationPercent,
                            IsMarketSell = this.IsMarketSell,
                            IsDCA = this.IsDCA,
                            DCAStepCount = this.DCAStepCount,
                            DCAProfitPercent = this.DCAProfitPercent,
                            DCASteps = this.DCASteps,
                            DCAFilters = this.DCAFilters,
                            FiltersBuy = new List<JsonFilter>(this.FiltersBuy),
                            TargetPointBuy = this.TargetPointBuy,
                            FiltersSell = new List<JsonFilter>(this.FiltersSell),
                            TargetPointSell = this.TargetPointSell,
                            StopLoss = this.StopLoss,
                            StopLossApproximation = this.StopLossApproximation,
                            FiltersStopLoss = new List<JsonFilter>(this.FiltersStopLoss),
                            TargetPointStopLoss = this.TargetPointStopLoss
                        };

                        // Update config
                        ConfigManager.Save(SelectedStrategyConfig, config);
                        MessageBox.Show(_.Msg6); // Конфиг обновлён!
                    }
                });
            }
        }


        // Контекстное меню: Сохранить коллекцию
        public ICommand SaveCollectionClick
        {
            get
            {
                return new Command((obj) =>
                {
                    Logger.ToFile("Collection save");
                    /*
                    // Save collection 
                    Views.SaveConfigWindow save = new Views.SaveConfigWindow();
                    if (save.ShowDialog() == true)
                    {
                        if (save.FileName != null)
                        {
                            // Подготовим директорию для коллекции
                            string collectionname = Config.CreateCollectionDirectory(save.FileName);

                            foreach (var thread in Threads)
                            {
                                // Scalping
                                if (thread.config.Strategy == Strategy.Scalping)
                                {
                                    IConfigText config = new ScalpingConfigText()
                                    {
                                        Strategy = Strategy.Scalping.ToString(),
                                        Budget = thread.config.Budget.ToString(),
                                        Cur1 = thread.config.Cur1,
                                        Cur2 = thread.config.Cur2,
                                        MinSpread = (thread.config as ScalpingConfigJson).MinSpreadStr,
                                        OptSpread = (thread.config as ScalpingConfigJson).OptSpreadStr,
                                        MinMarkup = (thread.config as ScalpingConfigJson).MinMarkupStr,
                                        OptMarkup = (thread.config as ScalpingConfigJson).OptMarkupStr,
                                        ZeroSell = (thread.config as ScalpingConfigJson).ZeroSellStr,
                                        InTimeout = (thread.config as ScalpingConfigJson).InTimeoutStr,
                                        IsDCA = thread.config.IsDCA,
                                        DCAStepCount = thread.config.DCAStepCountStr,
                                        DCAProfitPercent = thread.config.DCAProfitPercentStr,
                                        DCASteps = new ObservableCollection<string[]>(thread.config.DCAStepsStr as List<string[]>),
                                        DCAFilters = new Dictionary<int, DCAFilter>((thread.config as ScalpingConfigJson).DCAFilters),
                                        FirsOredersAmountPercentIgnor = (thread.config as ScalpingConfigJson).FirsOredersAmountPercentIgnorStr,
                                        FirsOredersCountIgnor = (thread.config as ScalpingConfigJson).FirsOredersCountIgnorStr,
                                        StopLoss = (thread.config as ScalpingConfigJson).StopLossStr,
                                        FiltersBuy = new List<JsonFilter>((thread.config as ScalpingConfigJson).FiltersBuy),
                                        TargetPointBuy = int.Parse((thread.config as ScalpingConfigJson).TargetPointBuy)
                                    };

                                    Config.SaveCollection(collectionname, $"{thread.config.Exchange}_{thread.config.Cur1}-{thread.config.Cur2}_{thread.TID}", config);
                                }

                                // Classic Long
                                if (thread.config.Strategy == Strategy.ClassicLong)
                                {
                                    IConfigText config = new ClassicLongConfigText()
                                    {
                                        Strategy = Strategy.ClassicLong.ToString(),
                                        Budget = thread.config.Budget.ToString(),
                                        Cur1 = thread.config.Cur1,
                                        Cur2 = thread.config.Cur2,

                                        TargetProfitPercent = (thread.config as ClassicLongConfig).TargetProfitPercent,
                                        IsProfitTrailing = (thread.config as ClassicLongConfig).IsProfitTrailing,
                                        TrailStepPercent = (thread.config as ClassicLongConfig).TrailStepPercent,
                                        ApproximationPercent = (thread.config as ClassicLongConfig).ApproximationPercent,
                                        UnApproximationPercent = (thread.config as ClassicLongConfig).UnApproximationPercent,
                                        IsMarketBuy = (thread.config as ClassicLongConfig).IsMarketBuy,
                                        IsDCA = thread.config.IsDCA,
                                        DCAStepCount = thread.config.DCAStepCountStr,
                                        DCAProfitPercent = thread.config.DCAProfitPercentStr,
                                        DCASteps = new ObservableCollection<string[]>(thread.config.DCAStepsStr as List<string[]>),
                                        DCAFilters = new Dictionary<int, DCAFilter>((thread.config as ClassicLongConfig).DCAFilters),
                                        FiltersBuy = new List<JsonFilter>((thread.config as ClassicLongConfig).FiltersBuy),
                                        TargetPointBuy = (thread.config as ClassicLongConfig).TargetPointBuy,
                                        FiltersSell = new List<JsonFilter>((thread.config as ClassicLongConfig).FiltersSell),
                                        TargetPointSell = (thread.config as ClassicLongConfig).TargetPointSell,
                                        StopLoss = (thread.config as ClassicLongConfig).StopLoss,
                                        StopLossApproximation = (thread.config as ClassicLongConfig).StopLossApproximation,
                                        FiltersStopLoss = new List<JsonFilter>((thread.config as ClassicLongConfig).FiltersStopLoss),
                                        TargetPointStopLoss = (thread.config as ClassicLongConfig).TargetPointStopLoss,
                                    };

                                    Config.SaveCollection(collectionname, $"{thread.config.Exchange}_{thread.config.Cur1}-{thread.config.Cur2}_{thread.TID}", config);
                                }

                                // Classic Short
                                if (thread.config.Strategy == Strategy.ClassicShort)
                                {
                                    IConfigText config = new ClassicShortConfigText()
                                    {
                                        Strategy = Strategy.ClassicShort.ToString(),
                                        Budget = thread.config.Budget.ToString(),
                                        Cur1 = thread.config.Cur1,
                                        Cur2 = thread.config.Cur2,

                                        TargetProfitPercent = (thread.config as ClassicShortConfig).TargetProfitPercent,
                                        IsProfitTrailing = (thread.config as ClassicShortConfig).IsProfitTrailing,
                                        TrailStepPercent = (thread.config as ClassicShortConfig).TrailStepPercent,
                                        ApproximationPercent = (thread.config as ClassicShortConfig).ApproximationPercent,
                                        UnApproximationPercent = (thread.config as ClassicShortConfig).UnApproximationPercent,
                                        IsMarketSell = (thread.config as ClassicShortConfig).IsMarketSell,
                                        IsDCA = thread.config.IsDCA,
                                        DCAStepCount = thread.config.DCAStepCountStr,
                                        DCAProfitPercent = thread.config.DCAProfitPercentStr,
                                        DCASteps = new ObservableCollection<string[]>(thread.config.DCAStepsStr as List<string[]>),
                                        DCAFilters = new Dictionary<int, DCAFilter>((thread.config as ClassicShortConfig).DCAFilters),
                                        FiltersBuy = new List<JsonFilter>((thread.config as ClassicShortConfig).FiltersBuy),
                                        TargetPointBuy = int.Parse((thread.config as ClassicShortConfig).TargetPointBuy),
                                        FiltersSell = new List<JsonFilter>((thread.config as ClassicShortConfig).FiltersSell),
                                        TargetPointSell = (thread.config as ClassicShortConfig).TargetPointSell,
                                        StopLoss = (thread.config as ClassicShortConfig).StopLoss,
                                        StopLossApproximation = (thread.config as ClassicShortConfig).StopLossApproximation,
                                        FiltersStopLoss = new List<JsonFilter>((thread.config as ClassicShortConfig).FiltersStopLoss),
                                        TargetPointStopLoss = (thread.config as ClassicShortConfig).TargetPointStopLoss,
                                    };

                                    Config.SaveCollection(collectionname, $"{thread.config.Exchange}_{thread.config.Cur1}-{thread.config.Cur2}_{thread.TID}", config);
                                }

                            }
                        }
                        MessageBox.Show(_.Msg7); // Коллекция сохранёна!
                    }
                    */
                });
            }
        }

        // Контекстное меню: Загрузить коллекцию
        public ICommand LoadCollectionClick
        {
            get
            {

                return new Command((obj) =>
                {
                    /*
                    try
                    {

                        Logger.ToFile("Collection Load");

                        // Получаем элементы коллекции по имени
                        var collectionItems = Config.LoadCollection(obj.ToString());

                        foreach (var item in collectionItems)
                        {
                            var oneConfig = item.Config; // Конфиг элемента коллекции

                            IConfig config = new ScalpingConfigJson(); // если тут скальпинг иниц сработает ли арбитраж?  Инициализируем переменную конфига

                            // Scalping
                            if (oneConfig.Strategy == Strategy.Scalping.ToString())
                            {
                                config = new ScalpingConfigJson()
                                {
                                    Strategy = Strategy.Scalping,
                                    Exchange = item.Exchange,
                                    Budget = Conv.dec((oneConfig as ScalpingConfigText).Budget, true), // TODO can be some issue with %. before it was a string type
                                    Cur1 = (oneConfig as ScalpingConfigText).Cur1,
                                    Cur2 = (oneConfig as ScalpingConfigText).Cur2,
                                    IsDCA = (oneConfig as ScalpingConfigText).IsDCA,
                                    DCAStepCountStr = (oneConfig as ScalpingConfigText).DCAStepCount,
                                    DCAProfitPercentStr = (oneConfig as ScalpingConfigText).DCAProfitPercent,
                                    DCAStepsStr = new List<string[]>((oneConfig as ScalpingConfigText).DCASteps),
                                    DCAFilters = new Dictionary<int, DCAFilter>((oneConfig as ScalpingConfigText).DCAFilters),
                                    MinSpreadStr = (oneConfig as ScalpingConfigText).MinSpread,
                                    OptSpreadStr = (oneConfig as ScalpingConfigText).OptSpread,
                                    MinMarkupStr = (oneConfig as ScalpingConfigText).MinMarkup,
                                    OptMarkupStr = (oneConfig as ScalpingConfigText).OptMarkup,
                                    ZeroSellStr = (oneConfig as ScalpingConfigText).ZeroSell,
                                    InTimeoutStr = (oneConfig as ScalpingConfigText).InTimeout,
                                    FirsOredersAmountPercentIgnorStr = (oneConfig as ScalpingConfigText).FirsOredersAmountPercentIgnor,
                                    FirsOredersCountIgnorStr = (oneConfig as ScalpingConfigText).FirsOredersCountIgnor,
                                    StopLossStr = (oneConfig as ScalpingConfigText).StopLoss,
                                    FiltersBuy = new List<JsonFilter>((oneConfig as ScalpingConfigText).FiltersBuy),
                                    TargetPointBuy = (oneConfig as ScalpingConfigText).TargetPointBuy.ToString(),
                                    FiltersSell = new List<JsonFilter>((oneConfig as ScalpingConfigText).FiltersSell),
                                    TargetPointSell = (oneConfig as ScalpingConfigText).TargetPointSell,
                                    IsSellStart = false,
                                    BuyPriceForSellStartStr = "0",
                                    AmountForSellStartStr = "0"
                                };
                            }

                            // Classic Long
                            if (oneConfig.Strategy == Strategy.ClassicLong.ToString())
                            {
                                config = new ClassicLongConfig()
                                {
                                    Strategy = Strategy.ClassicLong,
                                    Exchange = item.Exchange,
                                    TargetProfitPercent = (oneConfig as ClassicLongConfigText).TargetProfitPercent,
                                    IsProfitTrailing = (oneConfig as ClassicLongConfigText).IsProfitTrailing,
                                    TrailStepPercent = (oneConfig as ClassicLongConfigText).TrailStepPercent,
                                    ApproximationPercent = (oneConfig as ClassicLongConfigText).ApproximationPercent,
                                    UnApproximationPercent = (oneConfig as ClassicLongConfigText).UnApproximationPercent,
                                    IsMarketBuy = (oneConfig as ClassicLongConfigText).IsMarketBuy,
                                    Budget = Conv.dec((oneConfig as ClassicLongConfigText).Budget, true), // Can be some issue with %. Befero here was a string type
                                    Cur1 = (oneConfig as ClassicLongConfigText).Cur1,
                                    Cur2 = (oneConfig as ClassicLongConfigText).Cur2,
                                    IsDCA = (oneConfig as ClassicLongConfigText).IsDCA,
                                    DCAStepCountStr = (oneConfig as ClassicLongConfigText).DCAStepCount,
                                    DCAProfitPercentStr = (oneConfig as ClassicLongConfigText).DCAProfitPercent,
                                    DCAStepsStr = new List<string[]>((oneConfig as ClassicLongConfigText).DCASteps),
                                    DCAFilters = new Dictionary<int, DCAFilter>((oneConfig as ClassicLongConfigText).DCAFilters),
                                    FiltersBuy = new List<JsonFilter>((oneConfig as ClassicLongConfigText).FiltersBuy),
                                    TargetPointBuy = (oneConfig as ClassicLongConfigText).TargetPointBuy.ToString(),
                                    FiltersSell = new List<JsonFilter>((oneConfig as ClassicLongConfigText).FiltersSell),
                                    TargetPointSell = (oneConfig as ClassicLongConfigText).TargetPointSell,
                                    StopLoss = (oneConfig as ClassicLongConfigText).StopLoss,
                                    StopLossApproximation = (oneConfig as ClassicLongConfigText).StopLossApproximation,
                                    FiltersStopLoss = new List<JsonFilter>((oneConfig as ClassicLongConfigText).FiltersStopLoss),
                                    TargetPointStopLoss = (oneConfig as ClassicLongConfigText).TargetPointStopLoss
                                };
                            }

                            // Classic Short
                            if (oneConfig.Strategy == Strategy.ClassicShort.ToString())
                            {
                                config = new ClassicShortConfig()
                                {
                                    Strategy = Strategy.ClassicShort,
                                    Exchange = item.Exchange,
                                    TargetProfitPercent = (oneConfig as ClassicShortConfigText).TargetProfitPercent,
                                    IsProfitTrailing = (oneConfig as ClassicShortConfigText).IsProfitTrailing,
                                    TrailStepPercent = (oneConfig as ClassicShortConfigText).TrailStepPercent,
                                    ApproximationPercent = (oneConfig as ClassicShortConfigText).ApproximationPercent,
                                    UnApproximationPercent = (oneConfig as ClassicShortConfigText).UnApproximationPercent,
                                    IsMarketSell = (oneConfig as ClassicShortConfigText).IsMarketSell,
                                    Budget = Conv.dec((oneConfig as ClassicShortConfigText).Budget, true), // Can be some issue with %. Befero here was a string type
                                    Cur1 = (oneConfig as ClassicShortConfigText).Cur1,
                                    Cur2 = (oneConfig as ClassicShortConfigText).Cur2,
                                    IsDCA = (oneConfig as ClassicShortConfigText).IsDCA,
                                    DCAStepCountStr = (oneConfig as ClassicShortConfigText).DCAStepCount,
                                    DCAProfitPercentStr = (oneConfig as ClassicShortConfigText).DCAProfitPercent,
                                    DCAStepsStr = new List<string[]>((oneConfig as ClassicShortConfigText).DCASteps),
                                    DCAFilters = new Dictionary<int, DCAFilter>((oneConfig as ClassicShortConfigText).DCAFilters),
                                    FiltersBuy = new List<JsonFilter>((oneConfig as ClassicShortConfigText).FiltersBuy),
                                    TargetPointBuy = (oneConfig as ClassicShortConfigText).TargetPointBuy.ToString(),
                                    FiltersSell = new List<JsonFilter>((oneConfig as ClassicShortConfigText).FiltersSell),
                                    TargetPointSell = (oneConfig as ClassicShortConfigText).TargetPointSell,
                                    StopLoss = (oneConfig as ClassicShortConfigText).StopLoss,
                                    StopLossApproximation = (oneConfig as ClassicShortConfigText).StopLossApproximation,
                                    FiltersStopLoss = new List<JsonFilter>((oneConfig as ClassicShortConfigText).FiltersStopLoss),
                                    TargetPointStopLoss = (oneConfig as ClassicShortConfigText).TargetPointStopLoss
                                };
                            }

                            // Создаём поток с настройками
                            TThread newThread = new TThread(config, this.SelectedExchange.Id);
                            newThread.NewIterationEvent += UpdateStat; // Подписываемся на событие "Новая итерация"
                            Threads.Add(newThread);
                        }
                        SelectedThread = Threads.Last();
                    }
                    catch
                    {
                        MessageBox.Show(_.Msg21); // Временно отключено из-за добавления новых функций
                    }
                    */
                });

                
            }
        }

        #endregion

        // Состояние / надпись / команда кнопки бабло
        private string babloText;
        public string BabloText
        {
            get { return babloText; }
            set
            {
                babloText = value;
                OnPropertyChanged();
            }
        }

        // Счётчик итераций
        private int iterations = 0; // Общее количество итераций по всем потокам
        private string iterationsStr;
        public string IterationsStr // Строка Итераций: ∞ 
        {
            get { return iterationsStr; }
            set
            {
                iterationsStr = value;
                OnPropertyChanged();
            }
        }

        // Статистика по заработку
        private decimal profitUSD = 0; // Общий профит в долларах
        private string profitStr;
        public string ProfitStr // Строка Заработали: ∞ (∞ %)
        {
            get { return profitStr; }
            set
            {
                profitStr = value;
                OnPropertyChanged();
            }
        }

        // Метод обновления статистики по итерациям и профиту
        public void UpdateStat(decimal _profitUSD, decimal _profitPercent)
        {
            if (_profitPercent < -6 || _profitPercent > 50)
            {
                MessageBox.Show(_.Msg22); // Скорее всего произошёл сбой статистики! Пожалуйста отправьте логи разработчику прямо сейчас, чтобы помочь улучшить продукт!
                return;
            }

            this.iterations++;
            this.profitUSD += _profitUSD;
            //this.profitPercent += _profitPercent; // TODO FUTURE проценты суммируются, что не есть хорошо. 

            this.IterationsStr = $"{_.Iterations} {this.iterations}";
            this.ProfitStr = $"{_.Profits} {Calc.RoundUp(this.profitUSD, (decimal)0.00000001)}$";
        }

        // Загрузка настроек StratumBox
        private void BoxSettingsLoad()
        {
            // TODO временно отключая функцию
            //this.IsMyOrdersBox = (Settings.MyOrdersBox) ? true : false;
            this.IsMyOrdersBox =  false; 

            if(Settings.MyOrdersBox)
            {
                if (!DTO.OpenOrders.IsOpenOrderEventSubscribed)
				{
                    DTO.OpenOrders.IsOpenOrderEventSubscribed = true;
                    DTO.OpenOrders.OpenOrderEvent += OpenOrderEventHandler;
                }

                if (!DTO.OpenOrders.IsCancelOrderEventSubscribed)
                {
                    DTO.OpenOrders.IsCancelOrderEventSubscribed = true;
                    DTO.OpenOrders.CancelOrderEvent += CancelOrderEventHandler;
                }

                if (!DTO.OpenOrders.IsGetOrderInfoEventSubscribed)
                {
                    DTO.OpenOrders.IsGetOrderInfoEventSubscribed = true;
                    DTO.OpenOrders.GetOrderInfoEvent += GetOrderInfoEventHandler;
                }
            }
        }

        /// <summary>
        /// StratumBox : My open orders collection
        /// </summary>
        public ObservableCollection<OpenOrders> OpenOrders { get; set; } = new ObservableCollection<OpenOrders>();

        // Выбранный открытый ордер
        /*private OpenOrders openOrderSelected;
        public OpenOrders OpenOrderSelected
        {
            get { return openOrderSelected; }
            set
            {
                openOrderSelected = value;
                OnPropertyChanged();
            }
        }*/

        // Включен ли StartumBox "Мои ордера"
        private bool isMyOrdersBox;
        public bool IsMyOrdersBox
        {
            get { return isMyOrdersBox; }
            set
            {
                isMyOrdersBox = value;
                OnPropertyChanged();
            }
        }

        // Показывать ли картинку на кнопке
        private bool isBabloImage;
        public bool IsBabloImage
        {
            get
            {
                try
                {
                    DateTime dt1 = DateTime.Parse("19/06/2020");
                    DateTime dt2 = DateTime.Now;

                    if (dt1.Date == dt2.Date)
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }

                return false;
            }
            set
            {
                isBabloImage = value;
                OnPropertyChanged();
            }
        }

        

        public void OpenOrderEventHandler(int _tid, Exchange _exchange, Strategy _strategy, Order _order)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                this.OpenOrders.Add(new OpenOrders { TID = _tid, Exchange = _exchange, Strategy = _strategy, Order = _order });
            });
        }

        private void CancelOrderEventHandler(string _orderId)
        {
            foreach (var openOrder in this.OpenOrders)
            {
                if (openOrder.Order.Id == _orderId)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        this.OpenOrders.Remove(openOrder);
                    });
                    break;
                }
            }
        }

        private void DeleteAllOrdersByTid(int tid)
		{
            foreach (var openOrder in this.OpenOrders)
            {
                if (openOrder.TID == tid)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        this.OpenOrders.Remove(openOrder);
                    });
                    // TODO Add logs that order still on the exchange but remove from table here
                    break;
                }
            }
        }

        private void GetOrderInfoEventHandler(int _tid, Exchange _exchange, Strategy _strategy, Order _order)
        {
            return; // TODO временно отключая функцию

            bool orderFound = false; // Флаг, чтобы если ордер не найден то добавить его
            var openOrder = this.OpenOrders.FirstOrDefault(x => x.Order.Id == _order.Id);
            if(openOrder != null)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.OpenOrders.Remove(openOrder);
                    if (_order.Status != OrderStatus.Canceled && _order.Status != OrderStatus.Filled) // Если ордре отменен то не надо добавлять его
                        this.OpenOrders.Add(new OpenOrders { TID = _tid, Exchange = _exchange, Strategy = _strategy, Order = _order });
                });
                orderFound = true;
            }
            /*foreach (var openOrder in this.OpenOrders)
            {
                if (openOrder.Order.Id != _order.Id || openOrder.Order.Status != _order.Status)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        this.OpenOrders.Remove(openOrder);
                        if(_order.Status != OrderStatus.Canceled) // Если ордре отменен то не надо добавлять его
                            this.OpenOrders.Add(new OpenOrders { TID = _tid, Exchange = _exchange, Strategy = _strategy, Order = _order });
                    });
                    orderFound = true;
                    break;
                }
            }*/
            if(!orderFound) // Если ордер не найден в Моих ордерах, то добавляем его (нужно для резервного восстанволения)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    if (_order.Status != OrderStatus.Canceled) // Если ордре отменен то не надо добавлять его
                        this.OpenOrders.Add(new OpenOrders { TID = _tid, Exchange = _exchange, Strategy = _strategy, Order = _order });
                });
            }
                
        }

        // Кнопка Отменить: ордер из Мои ордера
        /*public ICommand CancelOrderMyOrdersBoxClick
        {
            get
            {
                return new Command((obj) =>
                {
                    if(OpenOrderSelected == null)
                    {
                        MessageBox.Show(_.Msg8);
                        return;
                    }

                    Logger.ToFile($"Cancel order for {OpenOrderSelected.TID}");

                    OpenOrderSelected.CancelOrder();

                });
            }
        }*/

        // Кнопка: Фильтры&Индикаторы Окно
        public ICommand FiltersAndIndicatorsManagerClick
        {
            get
            {
                return new Command((obj) =>
                {
                    // Фильтры для покупки
                    if (obj.ToString() == "BUY")
                    {
                        Views.FiltersAndIndicatorsManagerWindow faim = new Views.FiltersAndIndicatorsManagerWindow(new List<JsonFilter>(FiltersBuy), this.TargetPointBuy, "BUY");
                        faim.VM.AddFiltersEvent += AddFilters; // Филтры (ID, без настроек) для конфига (+отображения в голубой панели)
                        //faim.VM.AddFiltersSettingsEvent += AddFiltersSettings; // Фильтры с настройками для отображения в окне
                        faim.Show();
                    }
                    // Фильтры для продажи
                    if (obj.ToString() == "SELL")
                    {
                        Views.FiltersAndIndicatorsManagerWindow faim = new Views.FiltersAndIndicatorsManagerWindow(new List<JsonFilter>(FiltersSell), this.TargetPointSell, "SELL");
                        faim.VM.AddFiltersEvent += AddFilters; // Филтры (ID, без настроек) для конфига (+отображения в голубой панели)
                        faim.Show();
                    }
                    // Фильтры для StopLoss
                    if (obj.ToString() == "STOPLOSS")
                    {
                        if (SelectedStrategy.Id == Strategy.ClassicLong) // Нужно отдельно по стратегиям т.к. стоплосс для лонга будет по SELL смотреть а для шорта по BUY
                        {
                            Views.FiltersAndIndicatorsManagerWindow faim = new Views.FiltersAndIndicatorsManagerWindow(new List<JsonFilter>(FiltersStopLoss), this.TargetPointStopLoss, "SELL", true);
                            faim.VM.AddFiltersEvent += AddFilters; // Филтры (ID, без настроек) для конфига (+отображения в голубой панели)
                            faim.Show();
                        }

                        if (SelectedStrategy.Id == Strategy.ClassicShort) // Нужно отдельно по стратегиям т.к. стоплосс для лонга будет по SELL смотреть а для шорта по BUY
                        {
                            Views.FiltersAndIndicatorsManagerWindow faim = new Views.FiltersAndIndicatorsManagerWindow(new List<JsonFilter>(FiltersStopLoss), this.TargetPointStopLoss, "BUY", true);
                            faim.VM.AddFiltersEvent += AddFilters; // Филтры (ID, без настроек) для конфига (+отображения в голубой панели)
                            faim.Show();
                        }
                    }
                });
            }
        }

        // Кнопка: DCA Manager Окно
        public ICommand DCAManagerClick
        {
            get
            {
                return new Command((obj) =>
                {
                    Views.DCAManagerWindow dcaManager = new Views.DCAManagerWindow(obj.ToString(), this.DCAStepCount, this.DCASteps, this.DCAFilters);
                    dcaManager.VM.AddDCABoxEvent += AddDCABox; 
                    dcaManager.Show();
                });
            }
        }

        public int TargetPointBuy { get; set; } = 0;
        public ObservableCollection<JsonFilter> FiltersBuy { get; set; } = new ObservableCollection<JsonFilter>(); // Списк индикаторов для голубой панели (ID, без настроек)
        //public List<IFilter> FiltersSettings { get; set; } = new List<IFilter>(); // Список индикаторов с полными настройками (для отображения в окне Фильтров&Индикаторов

        public int TargetPointSell { get; set; } = 0;
        public ObservableCollection<JsonFilter> FiltersSell { get; set; } = new ObservableCollection<JsonFilter>(); // Списк индикаторов для голубой панели (ID, без настроек)

        public int TargetPointStopLoss { get; set; } = 0;
        public ObservableCollection<JsonFilter> FiltersStopLoss { get; set; } = new ObservableCollection<JsonFilter>(); // Списк индикаторов для голубой панели (ID, без настроек)

        // Список списков фильтров по шагам DCA - DCAFilters[номерШага] = список фильтров
        private Dictionary<int, DCAFilter> dcaFilters;
        public Dictionary<int, DCAFilter> DCAFilters // Хранит в себе настройки Фильтров по DCA шагам
        {
            get { return dcaFilters; }
            set
            {
                dcaFilters = value;
                OnPropertyChanged();
            }
        }
        // dcaf

        // Добавление списка индикаторов (ID, без настроек)
        public void AddFilters(List<JsonFilter> filters, int targetPoint, string filtersSide, bool isStopLoss = false, int step = 0)
        {
            // StopLoss Long / StopLoss Short
            if (isStopLoss)
            {
                FiltersStopLoss.Clear();

                foreach (var filter in filters)
                {
                    FiltersStopLoss.Add(filter);
                }

                this.TargetPointStopLoss = targetPoint;
                return; // Чтобы дальше не шли и не зашли в SELL
            }

            if (filtersSide == "BUY")
            {
                FiltersBuy.Clear();

                foreach (var filter in filters)
                {
                    FiltersBuy.Add(filter);
                }

                this.TargetPointBuy = targetPoint;
            }
            // TODO Sell
            if (filtersSide == "SELL")
            {
                FiltersSell.Clear();

                foreach (var filter in filters)
                {
                    FiltersSell.Add(filter);
                }

                this.TargetPointSell = targetPoint;
            }

            

        }

        // Добавление списка индикаторов (с настройками, для отображения в окне Фильтров&Индикаторов)
        /*public void AddFiltersSettings(List<IFilter> filters)
        {
            FiltersSettings.Clear();

            foreach (var filter in filters)
            {
                FiltersSettings.Add(filter);
            }
        }*/

        // Обрботка события добавления DCA из DCA-менеджера
        public void AddDCABox(string stepCount, ObservableCollection<string[]> DCASteps, Dictionary<int, DCAFilter> dcaFilters)
        {
            this.IsDCA = true;
            this.DCAStepCount = stepCount;

            var DCAStepsStr = new ObservableCollection<string[]>(from x in DCASteps select (string[])x.Clone()); // Создаём копию, иначе все как один объект изминяется во всех потоках
            this.DCASteps.Clear();
            foreach (var dcaStep in DCAStepsStr)
            {
                this.DCASteps.Add(dcaStep);
            }

            // принять фильтры
            this.DCAFilters = new Dictionary<int, DCAFilter>(dcaFilters);
        }
    }
}
