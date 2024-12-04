using Newtonsoft.Json;
using stratumbot.Core;
using stratumbot.Interfaces;
using stratumbot.Models.Filters;
using stratumbot.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Strategies
{
    class ClassicShortConfig : IConfig
    {
        /// <summary>
        /// IConfigText object of this config
        /// </summary>
        public ClassicShortConfigText IConfigText { get; set; }


        /// <summary>
        /// Strategy name
        /// </summary>
        public Strategy Strategy { get; set; } = Strategy.ClassicShort;

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
        public bool IsMarketBuy { get; set; } // dont need yet i suppose
        public bool IsMarketSell { get; set; } // dont need yet i suppose

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
        +++[JsonProperty("Strategy")]
        +++public Strategy Strategy { get; set; }

        ---[JsonProperty("Exchange")]
        ---public Exchange Exchange { get; set; }

        +++[JsonProperty("Budget")]
        ++++public decimal Budget { get; set; } // Бюджет указаный пользователем (возможно со знаком %)


        +++[JsonProperty("Cur1")]
        +++public string Cur1 { get; set; }
        +++[JsonProperty("Cur2")]
        +++public string Cur2 { get; set; }

        +++[JsonProperty("TargetProfitPercent")]
        +++public string TargetProfitPercent { get; set; }

        ++++[JsonProperty("IsProfitTrailing")]
        +++public bool IsProfitTrailing { get; set; }
        +++[JsonProperty("TrailStepPercent")]
        +++public string TrailStepPercent { get; set; }
        +++[JsonProperty("ApproximationPercent")]
        +++public string ApproximationPercent { get; set; }
        +++[JsonProperty("UnApproximationPercent")]
        +++public string UnApproximationPercent { get; set; }

        +++[JsonProperty("IsMarketSell")]
        +++public bool IsMarketSell { get; set; }

        +++[JsonProperty("IsDCA")]
        +++public bool IsDCA { get; set; } // Включёна ли функция усредненеия (DCA)
        +++[JsonProperty("DCAProfitPercentStr")]
        +++public string DCAProfitPercentStr { get; set; }
        +++[JsonProperty("DCAStepCountStr")]
        +++public string DCAStepCountStr { get; set; }
        +++[JsonProperty("DCAStepsStr")]
        +++public List<string[]> DCAStepsStr { get; set; }

        +++[JsonProperty("DCAFilters")]
        +++public Dictionary<int, DCAFilter> DCAFilters { get; set; } // Фильтры по шагам

        +++[JsonProperty("FiltersBuy")]
        +++public List<JsonFilter> FiltersBuy { get; set; } // Список фильтров и индикаторов (ID, без самих настроек)
        +++[JsonProperty("TargetPointBuy")]
        +++public string TargetPointBuy { get; set; }

        +++[JsonProperty("FiltersSell")]
        +++public List<JsonFilter> FiltersSell { get; set; } // Список фильтров и индикаторов (ID, без самих настроек)
        +++[JsonProperty("TargetPointSell")]
        +++public int TargetPointSell { get; set; }

        +++[JsonProperty("StopLoss")]
        +++public string StopLoss { get; set; }
        +++[JsonProperty("StopLossApproximation")]
        +++public string StopLossApproximation { get; set; }

        +++[JsonProperty("FiltersStopLoss")]
        +++public List<JsonFilter> FiltersStopLoss { get; set; } // Список фильтров и индикаторов (ID, без самих настроек)
        +++[JsonProperty("TargetPointStopLoss")]
        +++public int TargetPointStopLoss { get; set; }




        +public decimal DCAProfitPercent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        +public int DCAStepCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        +public List<decimal[]> DCASteps { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        +List<DCAStepConfig> IConfig.DCASteps { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        */

        public ClassicShortConfig(IConfigText config)
        {
            // Upcast

            ClassicShortConfigText textConfig = config as ClassicShortConfigText;

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
            this.IsMarketSell = textConfig.IsMarketSell;


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
