using stratumbot.Core;
using stratumbot.Interfaces;
using stratumbot.Models.Tools;
using System.Collections.Generic;

namespace stratumbot.Models.Strategies
{
    /// <summary>
    /// 
    /// ScalpingConfig DTO and constructor from IConfigText.
    /// 
    /// Scalping configuration of thread to use in process.
    /// 
    /// </summary>
    public class ScalpingConfig : IConfig
    {
        /// <summary>
        /// IConfigText object of this config
        /// </summary>
        public ScalpingConfigText IConfigText { get; set; }

        /// <summary>
        /// Strategy name
        /// </summary>
        public Strategy Strategy { get; set; } = Strategy.Scalping;

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

        /// <summary>
        /// Is minimal spread percentage
        /// </summary>
        public bool IsMinSpreadAsPercent { get; set; }

        /// <summary>
        /// Minimal spread
        /// </summary>
        public decimal MinSpread { get; set; }

        /// <summary>
        /// Minimal spread (percentage)
        /// </summary>
        public decimal MinSpreadAsPercent { get; set; }

        /// <summary>
        /// Is optimal spread percentage
        /// </summary>
        public bool IsOptSpreadAsPercent { get; set; }

        /// <summary>
        /// Optinal spread
        /// </summary>
        public decimal OptSpread { get; set; }

        /// <summary>
        /// Optimal spread (percentage)
        /// </summary>
        public decimal OptSpreadAsPercent { get; set; }

        /// <summary>
        /// Is minimal markup percentage
        /// </summary>
        public bool IsMinMarkupAsPercent { get; set; }

        /// <summary>
        /// Minimal markup
        /// </summary>
        public decimal MinMarkup { get; set; }

        /// <summary>
        /// Minimal markup (percentage)
        /// </summary>
        public decimal MinMarkupAsPercent { get; set; }

        /// <summary>
        /// Is optimal markup percentage
        /// </summary>
        public bool IsOptMarkupAsPercent { get; set; }

        /// <summary>
        /// Optimal markup
        /// </summary>
        public decimal OptMarkup { get; set; }

        /// <summary>
        /// Optimal markup (percentage)
        /// </summary>
        public decimal OptMarkupAsPercent { get; set; }

        /// <summary>
        /// Is zero sell markup percentage
        /// </summary>
        public bool IsZeroSellAsPercent { get; set; }

        /// <summary>
        /// Zero sell markup
        /// </summary>
        public decimal ZeroSell { get; set; }

        /// <summary>
        /// Zero sell markup (percentage)
        /// </summary>
        public decimal ZeroSellAsPercent { get; set; }

        /// <summary>
        /// Timout for ckeck the spread again before start the deal
        /// </summary>
        public int InTimeout { get; set; }

        /// <summary>
        /// Percent of our order to ignore
        /// </summary>
        public decimal FirsOredersAmountPercentIgnor { get; set; }

        /// <summary>
        /// Nomber of orders before re-place order with zero sell markup
        /// </summary>
        public int FirsOredersCountIgnor { get; set; }

        /// <summary>
        /// Is stoploss feature turned on
        /// </summary>
        public bool IsStopLoss { get; set; }

        /// <summary>
        /// Is stoploss percentage
        /// </summary>
        public bool IsStopLossAsPercent { get; set; }

        /// <summary>
        /// Stoploss value (percentage)
        /// </summary>
        public decimal StopLossPercent { get; set; }

        /// <summary>
        /// Stoploss price
        /// </summary>
        public decimal StopLossPrice { get; set; }

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


        public ScalpingConfig(IConfigText config)
        {
            // Upcast

            ScalpingConfigText textConfig = config as ScalpingConfigText;

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

            // Min spread

            if (textConfig.MinSpread.Contains("%"))
            {
                this.IsMinSpreadAsPercent = true;
                this.MinSpread = 0;
                this.MinSpreadAsPercent = Conv.dec(textConfig.MinSpread, true);
            }
            else
            {
                this.IsMinSpreadAsPercent = false;
                this.MinSpreadAsPercent = 0;
                this.MinSpread = Conv.dec(textConfig.MinSpread);
            }

            // Opt spread

            if (textConfig.OptSpread.Contains("%"))
            {
                this.IsOptSpreadAsPercent = true;
                this.OptSpread = 0;
                this.OptSpreadAsPercent = Conv.dec(textConfig.OptSpread, true);
            }
            else
            {
                this.IsOptSpreadAsPercent = false;
                this.OptSpreadAsPercent = 0;
                this.OptSpread = Conv.dec(textConfig.OptSpread);
            }

            // Min markup

            if (textConfig.MinMarkup.Contains("%"))
            {
                this.IsMinMarkupAsPercent = true;
                this.MinMarkup = 0;
                this.MinMarkupAsPercent = Conv.dec(textConfig.MinMarkup, true);
            }
            else
            {
                this.IsMinMarkupAsPercent = false;
                this.MinMarkupAsPercent = 0;
                this.MinMarkup = Conv.dec(textConfig.MinMarkup);
            }

            // Opt markup

            if (textConfig.OptMarkup.Contains("%"))
            {
                this.IsOptMarkupAsPercent = true;
                this.OptMarkup = 0;
                this.OptMarkupAsPercent = Conv.dec(textConfig.OptMarkup, true);
            }
            else
            {
                this.IsOptMarkupAsPercent = false;
                this.OptMarkupAsPercent = 0;
                this.OptMarkup = Conv.dec(textConfig.OptMarkup);
            }

            // Zero sell markup

            if (textConfig.ZeroSell.Contains("%"))
            {
                this.IsZeroSellAsPercent = true;
                this.ZeroSell = 0;
                this.ZeroSellAsPercent = Conv.dec(textConfig.ZeroSell, true);
            }
            else
            {
                this.IsZeroSellAsPercent = false;
                this.ZeroSellAsPercent = 0;
                this.ZeroSell = Conv.dec(textConfig.ZeroSell);
            }

            // Timeout

            this.InTimeout = int.Parse(textConfig.InTimeout);

            // Firsts orders ignore

            this.FirsOredersAmountPercentIgnor = Conv.dec(textConfig.FirsOredersAmountPercentIgnor, true);
            this.FirsOredersCountIgnor = Conv.cleanInt(textConfig.FirsOredersCountIgnor);

            // StopLoss

            this.IsStopLoss = textConfig.IsStopLoss;

            if (Conv.dec(textConfig.StopLoss, true) == 0)
            {
                this.IsStopLoss = false;
            }

            if (this.IsStopLoss)
            {
                if (textConfig.StopLoss.Contains("%"))
                {
                    this.IsStopLossAsPercent = true;
                    this.StopLossPrice = 0;
                    this.StopLossPercent = Conv.dec(textConfig.StopLoss, true);
                }
                else
                {
                    this.IsStopLossAsPercent = false;
                    this.StopLossPercent = 0;
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

            // BUY Filters

            // ... init in IStrategy constructor

            this.Filters.TargetPointBuy = textConfig.TargetPointBuy;

            // DCA Filters

            // ... init in DCA.Compute()

        }
    }
}
