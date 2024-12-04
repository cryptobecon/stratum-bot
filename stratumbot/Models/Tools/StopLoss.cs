using stratumbot.DTO;
using stratumbot.Interfaces;
using stratumbot.Models.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Tools
{
    public static class StopLoss
    {
        /// <summary>
        /// Check can we cancel sell-order and open new sell-order by market price. 
		/// True if we can.
        /// Has Test
        /// </summary>
        public static bool CheckMin(Order _order, IExchange _exchange, decimal _stopLossPrice)
        {
            // Делаем проверек, если это селл то стоплосс будет на остаток, а если бай то на исполненую часть
            decimal amount = 0;
            if (_order.Side == OrderSide.SELL || _exchange.Id == Exchange.BinanceFutures)
                amount = _order.Remainder;
            else
                amount = _order.Filled;

            // Проверка по количеству
            if (amount < _exchange.MinAmount)
            {
                Logger.Info(String.Format(_.Log24, _exchange.MinAmount));
                return false;
            }
            // Проверка по стоимости
            if((_stopLossPrice * amount) <= _exchange.MinCost)
            {
                Logger.Info(String.Format(_.Log25, _exchange.MinCost));
                return false;
            }

            return true;
        }

		public static decimal GetApproximatePrice(decimal amount, IExchange exchange, string cur1, string cur2)
		{
			// Get DOMs
			var doms = exchange.GetDOM(cur1, cur2, 50);

			decimal domAmount = 0;

			foreach (var bid in doms["bids"])
			{
				if (domAmount > amount) return bid.Price;
				else domAmount += bid.Amount;
			}

			return 0;
		}

        public static decimal GetApproximatePriceShort(decimal amount, IExchange exchange, string cur1, string cur2)
        {
            // Get DOMs
            var doms = exchange.GetDOM(cur1, cur2, 50);

            decimal domAmount = 0;

            foreach (var ask in doms["asks"])
            {
                if (domAmount > amount) return ask.Price;
                else domAmount += ask.Amount;
            }

            return 0;
        }

        // TODO test SellByMarket
        /// <summary>
        /// Sell by market. Return SL-order
        /// </summary>
        public static Order SellByMarket(Order _sellOrder, IExchange _exchange, decimal _stopLossPrice)
        {
            var dom = _exchange.GetDOM(_sellOrder.Cur1, _sellOrder.Cur2, 10);
            decimal sumAmount = 0; // Суммарный объём из стакана
            decimal stopLossPrice = 0; // Цена по которой StopLoss будет выставлен
            decimal necessaryAmount = _sellOrder.Remainder * (decimal)1.2; // Необходимое количество. Навсякий случай с запасом
            foreach (var bid in dom["bids"])
            {
                sumAmount += bid.Amount;
                if(sumAmount >= necessaryAmount)
                {
                    stopLossPrice = bid.Price;
                }
            }

            Order stopLoss = _exchange.OpenLimitOrder(OrderSide.SELL, _sellOrder.Cur1, _sellOrder.Cur2, _sellOrder.Remainder, stopLossPrice);
            stopLoss.Price = _stopLossPrice; // Исскуственно поправляем цену, чтобы статистика не преувеличивала минус
            return stopLoss; 
        }
    }
}
