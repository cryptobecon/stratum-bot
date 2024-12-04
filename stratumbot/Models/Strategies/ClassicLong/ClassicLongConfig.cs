using stratumbot.Core;
using stratumbot.Interfaces;
using stratumbot.Models.Tools;
using System.Collections.Generic;

namespace stratumbot.Models.Strategies
{
    public class ClassicLongConfig : IConfig
    {

        /// <summary>
        /// IConfigText object of this config
        /// </summary>
        public ClassicLongConfigText IConfigText { get; set; }


        /// <summary>
        /// Strategy name
        /// </summary>
        public Strategy Strategy { get; set; } = Strategy.ClassicLong;

        /// <summary>
        /// Budget (absolute number)
        /// </summary>
        public decimal Budget { get; set; }

        /// <summary>
        /// Budget (as percent of balance)
        /// </summary>
        public decimal BudgetAsPercent { get; set; }

        /// <summary>
        /// Is budget set as percent
        /// </summary>
        public bool IsBudgetAsPercent { get; set; }

        /// <summary>
        /// First currency
        /// </summary>
        public string Cur1 { get; set; }

        /// <summary>
        /// Second (base) currency
        /// </summary>
        public string Cur2 { get; set; }


        public decimal TargetProfitPercent { get; set; }
        public bool IsProfitTrailing { get; set; }
        public decimal TrailStepPercent { get; set; }
        public decimal ApproximationPercent { get; set; }
        public decimal UnApproximationPercent { get; set; }
        public bool IsMarketBuy { get; set; }

        // ...



        /// <summary>
        /// Is stoploss feature turned on
        /// </summary>
        public bool IsStopLoss { get; set; }

        public decimal StopLossApproximation { get; set; } // Приближение для выставления лимитного ордера StopLoss (без знака - или %)

        public bool IsStopLossApproximationAsPercent { get; set; }

        /// <summary>
        /// Is stoploss percentage
        /// </summary>
        public bool IsStopLossAsPercent { get; set; }

        public bool IsStopLossAsMinusPoints { get; set; }

        /// <summary>
        /// Stoploss value (percentage)
        /// </summary>
        public decimal StopLossPercent { get; set; }

        /// <summary>
        /// Stoploss price
        /// </summary>
        public decimal StopLossPrice { get; set; }
        public decimal StopLossMinus { get; set; }

        /// <summary>
        /// Is DCA feature turned on
        /// </summary>
        public bool IsDCA { get; set; }

        /// <summary>
        /// Profit (percentage) if DCA triggered
        /// </summary>
        public decimal DCAProfitPercent { get; set; }

        /// <summary>
        /// Number of DCA steps
        /// </summary>
        public int DCAStepCount { get; set; }

        /// <summary>
        /// List of DCA steps configuration
        /// </summary>
        public List<DCAStepConfig> DCASteps { get; set; } = new List<DCAStepConfig>();

        /// <summary>
        /// Filters object
        /// </summary>
        public Filters.Filters Filters = new Filters.Filters();


        /*
        
        +++public string Strategy { get; set; } = "Classic Long";
        +++public string Budget { get; set; }
        +++public string Cur1 { get; set; }
        +++public string Cur2 { get; set; }

        +++public string TargetProfitPercent { get; set; }
        +++public bool IsProfitTrailing { get; set; }
        +++public string TrailStepPercent { get; set; }
        +++public string ApproximationPercent { get; set; }
        +++public string UnApproximationPercent { get; set; }

        +++public bool IsMarketBuy { get; set; }

        +++public bool IsDCA { get; set; }
        +++public string DCAStepCount { get; set; }
        +++public string DCAProfitPercent { get; set; }
        +++public ObservableCollection<string[]> DCASteps { get; set; }

        [JsonProperty("DCAFilters")]
        public Dictionary<int, DCAFilter> DCAFilters { get; set; } // Фильтры по шагам

        +++public List<Filters.JsonFilter> FiltersBuy { get; set; } // TODO INDICATORS тут добавил и надо везде
        +++[JsonProperty("TargetPointBuy")] // именно в этом месте надо указывать как сохранять строки json
        +++public string TargetPointBuy { get; set; }

        +++public List<Filters.JsonFilter> FiltersSell { get; set; }
        +++[JsonProperty("TargetPointSell")]
        +++public int TargetPointSell { get; set; }

        +++public string StopLoss { get; set; }
        +++public string StopLossApproximation { get; set; }

        +++public List<Filters.JsonFilter> FiltersStopLoss { get; set; }
        +++[JsonProperty("TargetPointStopLoss")]
        +++public int TargetPointStopLoss { get; set; }*/
        
        
        public ClassicLongConfig(IConfigText config)
        {

            // Upcast

            ClassicLongConfigText textConfig = config as ClassicLongConfigText;

            // Save IConfigText object

            this.IConfigText = textConfig;

            // Budget

            if (textConfig.Budget.Contains("%"))
            {
                this.IsBudgetAsPercent = true;
                this.Budget = 0;
                this.BudgetAsPercent = Conv.dec(textConfig.Budget, true);
            }
            else
            {
                this.IsBudgetAsPercent = false;
                this.BudgetAsPercent = 0;
                this.Budget = Conv.dec(textConfig.Budget);
            }

            // Pair

            this.Cur1 = textConfig.Cur1;
            this.Cur2 = textConfig.Cur2;

            // Parameters

            // Target profit percent

            this.TargetProfitPercent = Conv.dec(textConfig.TargetProfitPercent, true);

            // Is trailing profit

            this.IsProfitTrailing = textConfig.IsProfitTrailing;

            // 

            this.TrailStepPercent = Conv.dec(textConfig.TrailStepPercent, true);

            //

            this.ApproximationPercent = Conv.dec(textConfig.ApproximationPercent, true);

            //
            this.UnApproximationPercent = Conv.dec(textConfig.UnApproximationPercent, true);

            //
            this.IsMarketBuy = textConfig.IsMarketBuy;


            // StopLoss

            this.IsStopLoss = textConfig.IsStopLoss;

            if (Conv.dec(textConfig.StopLoss, true) == 0)
            {
                this.IsStopLoss = false;
            }

            this.StopLossApproximation = 0; // лимитный ордер если есть аппроксимация // маркет ордер если 0 в аппроксимации
            this.StopLossPercent = 0;
            this.StopLossMinus = 0;

            if (this.IsStopLoss)
            {
                this.StopLossApproximation = Conv.dec(textConfig.StopLossApproximation, true);
                this.IsStopLossApproximationAsPercent = textConfig.StopLossApproximation.Contains("%");

                // StopLoss as percent

                if (textConfig.StopLoss.Contains("%"))
                {
                    this.IsStopLossAsPercent = true;
                    this.StopLossPercent = Conv.dec(textConfig.StopLoss, true);
                }
                else
                {
                    this.IsStopLossAsPercent = false;
                }

                // StopLoss as minus points

                if (textConfig.StopLoss.Contains("-") && !this.IsStopLossAsPercent)
                {
                    this.StopLossMinus = Conv.dec(textConfig.StopLoss, true);
                    this.IsStopLossAsMinusPoints = true;
                }
                else
                {
                    this.IsStopLossAsMinusPoints = false;
                }

                // StopLoss as Price

                if (!this.IsStopLossAsPercent && !this.IsStopLossAsMinusPoints)
                {
                    this.StopLossPrice = Conv.dec(textConfig.StopLoss);
                }
            }
            

            // DCA

            this.IsDCA = textConfig.IsDCA;

            if (textConfig.IsDCA)
            {
                this.DCAProfitPercent = Conv.dec(textConfig.DCAProfitPercent, true);
                this.DCAStepCount = Conv.cleanInt(textConfig.DCAStepCount);
                this.DCASteps = new List<DCAStepConfig>();

                foreach (var step in textConfig.DCASteps)
                {
                    this.DCASteps.Add(
                        new DCAStepConfig
                        {
                            Number = Conv.cleanInt(step[0]),
                            IsDropPercentage = step[1].Contains("%"),
                            Drop = Conv.dec(step[1], true),
                            IsAmountPercentage = step[2].Contains("%"),
                            Amount = Conv.dec(step[2], true)
                        }
                    );
                }
            }
            else
            {
                this.DCAProfitPercent = 0;
                this.DCAStepCount = 0;
                this.DCASteps = new List<DCAStepConfig>();
            }

            // Filters

            // BUY, SELL, StopLoss Filters

            // ... init in IStrategy constructor

            this.Filters.TargetPointBuy = textConfig.TargetPointBuy;
            this.Filters.TargetPointSell = textConfig.TargetPointSell;
            this.Filters.TargetPointStopLoss = textConfig.TargetPointStopLoss;

            // DCA Filters

            // ... init in DCA.Compute()

        }
    }

}
