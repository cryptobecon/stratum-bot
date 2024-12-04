using stratumbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.DTO
{
    public delegate void OpenOrderDelegate(int tid, Exchange exchange, Strategy strategy, Order order);
    public delegate void CancelOrderDelegate(string orderId);
    public delegate void GetOrderInfoDelegate(int tid, Exchange exchange, Strategy strategy, Order order);

    public class OpenOrders
    {
        public int TID { get; set; }
        public Strategy Strategy { get; set; }
        public Exchange Exchange { get; set; }
        public Order Order { get; set; }

        // События-прослейки между IExchange и MainVM
        public static event OpenOrderDelegate OpenOrderEvent;
        public static event CancelOrderDelegate CancelOrderEvent;
        public static event GetOrderInfoDelegate GetOrderInfoEvent;

        // Check if there is subscriber
        public static bool IsOpenOrderEventSubscribed { get; set; } = false;
        public static bool IsCancelOrderEventSubscribed { get; set; } = false;
        public static bool IsGetOrderInfoEventSubscribed { get; set; } = false;

        public static void OpenOrderEventHandler(int tid, Exchange exchange, Strategy strategy, Order order)
        {
            OpenOrderEvent?.Invoke(tid, exchange, strategy, order);
        }

        public static void CancelOrderEventHandler(string orderId)
        {
            CancelOrderEvent?.Invoke(orderId);
        }

        public static void GetOrderInfoEventHandler(int tid, Exchange exchange, Strategy strategy, Order order)
        {
            GetOrderInfoEvent?.Invoke(tid, exchange, strategy, order);
        }

        /// <summary>
        /// Метод для отмены ордера вручную из Моих ордеров
        /// </summary>
        /*public void CancelOrder()
        {
            var exchange = AvailableExchanges.CreateExchangeById(this.Exchange);
            // Берем ключи от этого же потока
            exchange.Tokens = TResource.GetAPI(this.TID);
            exchange.CancelOrder(this.Order);
        }*/
    }
}
