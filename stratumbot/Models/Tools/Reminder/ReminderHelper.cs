using stratumbot.DTO;
using System.Collections.Generic;

namespace stratumbot.Models.Tools.Reminder
{
    /// <summary>
    /// Helper for working with ReminderItem
    /// </summary>
    public static class ReminderHelper
    {
        /// <summary>
        /// Convert Order object to ReminderItem object
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>ReminderItem</returns>
        public static ReminderItem OrderToReminderItem(Order order, Trades trades)
        {
            return new ReminderItem()
            {
                Order = order,
                OriginalBuyPrice = Trades.GetAvgBuyPrice(trades),
                OriginalSellPrice = Trades.GetAvgSellPrice(trades)
            };
        }

        /// <summary>
        /// Get total remider amount for sell order which was bought before (for long and scalping)
        /// </summary>
        /// <param name="reminder">List of reminder items</param>
        /// <returns>Total filled amount of BUY orders with problem</returns>
        public static decimal GetTotalReminderAmountForSell(List<ReminderItem> reminder)
        {
            decimal amount = 0;

            foreach (var item in reminder)
            {
                if (item.Order.Side == OrderSide.BUY) amount += item.Order.Filled;
                if (item.Order.Side == OrderSide.SELL) amount += item.Order.Remainder;
            }

            return amount;
        }

        public static decimal GetTotalReminderCostForSell(List<ReminderItem> reminder)
        {
            decimal cost = 0;

            foreach (var item in reminder)
            {
                if (item.Order.Side == OrderSide.BUY) cost += (item.Order.Price * item.Order.Filled);
                if (item.Order.Side == OrderSide.SELL) cost += item.OriginalBuyPrice;
            }

            return cost;
        }

        /// <summary>
        /// Get average buy price of reminder orders (for long and scalping)
        /// </summary>
        /// <param name="reminder">List of reminder orders</param>
        /// <returns>Average price of current order + reminder orders</returns>
        public static decimal GetAvgBuyPriceForSell(List<ReminderItem> reminder)
        {
            decimal remiderAmount = ReminderHelper.GetTotalReminderAmountForSell(reminder);

            if (remiderAmount == 0) return 0;

            return ReminderHelper.GetTotalReminderCostForSell(reminder) / remiderAmount;
        }

        /// <summary>
        /// Get average price of (current order + reminder orders) for SELL (for long and scalping)
        /// </summary>
        /// <param name="reminder">List of reminder orders</param>
        /// <param name="amount">Bought amount of current order</param>
        /// <param name="buyPrice">bouht price of current order</param>
        /// <returns>Average price of current order + reminder orders</returns>
        public static decimal GetAvgPriceWithAdditionalAmountForSell(List<ReminderItem> reminder, decimal amount, decimal buyPrice)
        {
            decimal totalAmount = amount;
            decimal totalCost = amount * buyPrice;

            totalAmount += ReminderHelper.GetTotalReminderAmountForSell(reminder);
            totalCost += ReminderHelper.GetTotalReminderCostForSell(reminder);

            return (totalCost / totalAmount);
        }

        public static decimal GetAvgPriceWithAdditionalAmountForBuy(List<ReminderItem> reminder, decimal amount, decimal sellPrice)
        {
            decimal totalAmount = amount;
            decimal totalCost = amount * sellPrice;

            totalAmount += ReminderHelper.GetTotalReminderAmountForBuy(reminder);
            totalCost += ReminderHelper.GetTotalReminderCostForBuy(reminder);

            return (totalCost / totalAmount);
        }


        public static decimal GetTotalReminderAmountForBuy(List<ReminderItem> reminder)
        {
            decimal amount = 0;

            foreach (var item in reminder)
            {
                if (item.Order.Side == OrderSide.BUY) amount += item.Order.Remainder;
                if (item.Order.Side == OrderSide.SELL) amount += item.Order.Remainder;
            }

            return amount;
        }

        public static decimal GetAvgSellPriceForBuy(List<ReminderItem> reminder)
        {
            decimal remiderAmount = ReminderHelper.GetTotalReminderAmountForBuy(reminder);

            if (remiderAmount == 0) return 0;

            return ReminderHelper.GetTotalReminderCostForBuy(reminder) / remiderAmount;
        }

        public static decimal GetTotalReminderCostForBuy(List<ReminderItem> reminder)
        {
            decimal cost = 0;

            foreach (var item in reminder)
            {
                if (item.Order.Side == OrderSide.BUY) cost += item.OriginalSellPrice;
                if (item.Order.Side == OrderSide.SELL) cost += (item.Order.Price * item.Order.Remainder);
            }

            return cost;
        }

    }
}
