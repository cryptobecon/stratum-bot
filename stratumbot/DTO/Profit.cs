using stratumbot.Core;
using stratumbot.Interfaces;
using stratumbot.Models.Logs;
using stratumbot.Models.Tools.Reminder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.DTO
{
    /// <summary>
    /// Класс для подсчёта профита из пула ордеров и комисии
    /// </summary>
    public class Profit
    {
        /// <summary>
        /// Total cost of bought amount
        /// </summary> 
        public decimal buyCost { get; set; }

        /// <summary>
        /// Total bought amount
        /// </summary>
        public decimal buyAmount { get; set; }

        /// <summary>
        /// Total fee for bought amount
        /// </summary>
        public decimal buyFee { get; set; }

        /// <summary>
        /// Total cost of sold amount
        /// </summary>
        public decimal sellCost { get; set; }

        /// <summary>
        /// Total sold amount
        /// </summary>
        public decimal sellAmount { get; set; }

        /// <summary>
        /// Total fee for sold amount
        /// </summary>
        public decimal sellFee { get; set; }

        /// <summary>
        /// Total fee for both bought and sold amount
        /// </summary>
        public decimal totalFee { get; set; }

        /// <summary>
        /// Average buy price
        /// </summary>
        public decimal avgBuyPrice { get; set; }

        /// <summary>
        /// Average sell price
        /// </summary>
        public decimal avgSellPrice { get; set; }

        /// <summary>
        /// Revenue, gross income 
        /// </summary>
        public decimal revenue { get; set; }

        /// <summary>
        /// Profit
        /// </summary>
        public decimal profitBase { get; set; }

        /// <summary>
        /// Profit in percent (by invested money (buyCost))
        /// </summary>
        public decimal profitPercent { get; set; }

        /// <summary>
        /// Profit in USD
        /// </summary>
        public decimal profitUSD { get; set; }

        private decimal buyLimitCost { get; set; }
        private decimal buyLimitAmount { get; set; }
        private decimal buyLimitFee { get; set; }
        private decimal buyMarketCost { get; set; }
        private decimal buyMarketAmount { get; set; }
        private decimal buyMarketFee { get; set; }

        /// <summary>
        /// Profit computing for an iteration by trade history (Trades) and fee (IExchange.FEE)
        /// </summary>
        public Profit(Trades trades, List<ReminderItem> reminder, IExchange exchange, bool isLong = true)
        {
            this.buyCost = 0;
            this.buyAmount = 0;
            this.buyFee = 0;
            this.sellCost = 0;
            this.sellAmount = 0;
            this.sellFee = 0;
            this.totalFee = 0;
            this.avgBuyPrice = 0;
            this.avgSellPrice = 0;
            this.revenue = 0;
            this.profitBase = 0;
            this.profitPercent = 0;
            this.profitUSD = 0;

            try
			{
                if (isLong) this.ProfitLong(trades, reminder, exchange);
                else this.ProfitShort(trades, reminder, exchange);
            }
            catch (Exception ex)
			{
                Logger.Debug($"code 49");
                Logger.ToFile($"[StopLoss:exeption] {ex.ToString()}");
            }
            
        }

        private void ProfitLong(Trades trades, List<ReminderItem> reminder, IExchange exchange)
		{
            // BUY cost & amount
            foreach (var order in trades.BUY)
            {
                this.buyCost += order.Filled * order.Price;
                this.buyAmount += order.Filled;
                // Buy fee
                this.buyFee += Calc.AmountOfPercent(order.FeeRate, order.Filled * order.Price);
            }

            // SELL cost & amount
            foreach (var order in trades.SELL)
            {
                this.sellCost += order.Filled * order.Price;
                this.sellAmount += order.Filled;
                // Sell fee
                this.sellFee += Calc.AmountOfPercent(order.FeeRate, order.Filled * order.Price);
            }

            // BUY & SELL cost & amount from reminder
            foreach (var item in reminder)
            {
                if (item.Order.Side == OrderSide.BUY)
                {
                    this.buyCost += item.Order.Filled * item.Order.Price;
                    this.buyAmount += item.Order.Filled;
                    // Buy fee
                    this.buyFee += Calc.AmountOfPercent(item.Order.FeeRate, item.Order.Filled * item.Order.Price);
                }
                if (item.Order.Side == OrderSide.SELL)
                {
                    //this.sellCost += item.Order.Filled * item.Order.Price;
                    //this.sellAmount += item.Order.Filled;

                    // 
                    this.buyCost += item.Order.Remainder * item.OriginalBuyPrice;
                    this.buyAmount += item.Order.Remainder;
                    // Sell fee
                    this.buyFee += Calc.AmountOfPercent(item.Order.FeeRate, item.Order.Remainder * item.OriginalBuyPrice);
                }
            }

            // Average buy price
            this.avgBuyPrice = this.buyCost / this.buyAmount;

            // Buy fee
            //this.buyFee = Calc.AmountOfPercent(exchange.FEE, this.buyCost);

            // Average sell price
            this.avgSellPrice = this.sellCost / this.sellAmount;

            // Sell fee
            //this.sellFee = Calc.AmountOfPercent(exchange.FEE, this.sellCost);

            // Total fee
            this.totalFee = this.buyFee + this.sellFee;

            // Gross income
            this.revenue = this.sellCost - this.buyCost;

            // Profit base
            this.profitBase = this.revenue - this.totalFee;

            // Total spent (invested)
            decimal spent = this.buyCost + this.totalFee;

            // Profit percentage
            this.profitPercent = Calc.PercentOfAmount(this.profitBase, spent);

            // Profit in USD
            this.profitUSD = exchange.ConvertBaseToDollar(this.profitBase, trades.BUY[0].Cur2);
        }

        private void ProfitShort(Trades trades, List<ReminderItem> reminder, IExchange exchange)
        {
            // BUY cost & amount
            foreach (var order in trades.BUY)
            {
                this.buyCost += order.Filled * order.Price;
                this.buyAmount += order.Filled;
                // Buy fee
                this.buyFee += Calc.AmountOfPercent(order.FeeRate, order.Filled * order.Price);
            }

            // SELL cost & amount
            foreach (var order in trades.SELL)
            {
                this.sellCost += order.Filled * order.Price;
                this.sellAmount += order.Filled;
                // Sell fee
                this.sellFee += Calc.AmountOfPercent(order.FeeRate, order.Filled * order.Price);
            }

            // BUY & SELL cost & amount from reminder
            foreach (var item in reminder)
            {
                if (item.Order.Side == OrderSide.SELL)
                {
                    this.sellCost += item.Order.Filled * item.OriginalSellPrice;
                    this.sellAmount += item.Order.Filled;
                    // Sell fee
                    this.sellFee += Calc.AmountOfPercent(item.Order.FeeRate, item.Order.Remainder * item.OriginalBuyPrice);
                }
                if (item.Order.Side == OrderSide.BUY)
                {
                    this.sellCost += item.Order.Remainder * item.Order.Price;
                    this.sellAmount += item.Order.Remainder;
                    // Sell fee
                    this.sellFee += Calc.AmountOfPercent(item.Order.FeeRate, item.Order.Filled * item.Order.Price);
                }
            }

            // Average buy price
            this.avgBuyPrice = this.buyCost / this.buyAmount;

            // Buy fee
            //this.buyFee = Calc.AmountOfPercent(exchange.FEE, this.buyCost);

            // Average sell price
            this.avgSellPrice = this.sellCost / this.sellAmount;

            // Sell fee
            //this.sellFee = Calc.AmountOfPercent(exchange.FEE, this.sellCost);

            // Total fee
            this.totalFee = this.buyFee + this.sellFee;

            // Gross income
            this.revenue = this.sellCost - this.buyCost;

            // Profit base
            this.profitBase = this.revenue - this.totalFee;

            // Profit percentage
            this.profitPercent = Calc.PercentOfAmount(this.profitBase, this.buyCost);

            // Profit in USD
            this.profitUSD = exchange.ConvertBaseToDollar(this.profitBase, trades.BUY[0].Cur2);
        }

    }
}
