using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace stratumbot.DTO
{
    /// <summary>
    /// Trades history of iterstion
    /// </summary>
    public class Trades
    {
        /// <summary>
        /// Buy orders
        /// </summary>
        [JsonProperty("BUY")]
        public List<Order> BUY { get; set; }

        /// <summary>
        /// Sell orders
        /// </summary>
        [JsonProperty("SELL")]
        public List<Order> SELL { get; set; }

        public Trades()
        {
            this.BUY = new List<Order>();
            this.SELL = new List<Order>();
        }

        /// <summary>
        /// Add order to trades history
        /// </summary>
        /// <param name="order"></param>
        public void AddOrder(Order order)
        {
            // Check if order already exists
            if(this.BUY.Any(x => x.Id == order.Id) || this.SELL.Any(x => x.Id == order.Id)) return;

            if (order.Filled == 0) return;

            if (order.Side == OrderSide.BUY) this.BUY.Add(order);
            else this.SELL.Add(order);
        }

        /// <summary>
        /// Get average buy price for all BUY order in our trades. In case if we have more than one trade within the iteration
        /// </summary>
        public static decimal GetAvgBuyPrice(Trades trades)
        {
            decimal buyCost = 0;
            decimal buyAmount = 0;

            foreach (var order in trades.BUY)
            {
                buyCost += order.Filled * order.Price;
                buyAmount += order.Filled;
            }

            return buyAmount == 0 ? 0 : buyCost / buyAmount;
        }

        /// <summary>
        /// Get average sell price for all SELL order in our trades. In case if we have more than one trade within the iteration
        /// </summary>
        public static decimal GetAvgSellPrice(Trades trades)
        {
            decimal sellCost = 0;
            decimal sellAmount = 0;

            foreach (var order in trades.SELL)
            {
                sellCost += order.Filled * order.Price;
                sellAmount += order.Filled;
            }

            return sellAmount == 0 ? 0 : sellCost / sellAmount;
        }

        /// <summary>
        /// Get full bought amount
        /// </summary>
        public static decimal GetFullBuyAmount(Trades trades)
        {
            decimal amount = 0;

            foreach (var order in trades.BUY)
            {
                amount += order.Filled;
            }

            return amount;
        }

        /// <summary>
        /// Get full sold amount
        /// </summary>
        public static decimal GetFullSellAmount(Trades trades)
        {
            decimal amount = 0;

            foreach (var order in trades.SELL)
            {
                amount += order.Filled;
            }

            return amount;
        }
    }
}
