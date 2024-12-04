using stratumbot.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace stratumbot.Interfaces
{
    public interface IExchange
    {
        // Propoties:
        string Name { get; set; }



        /// <summary>
        /// Maker fee (limit order)
        /// </summary>
        decimal MakerFEE { get; set; }

        /// <summary>
        /// Taker fee (market order)
        /// </summary>
        decimal TakerFEE { get; set; }

        //decimal FEE { get; set; }

        decimal MinAmount { get; set; } // Минимальное количество
        decimal MinCost { get; set; } // Минимальная стоимость
        decimal TickSize { get; set; } // Размер шага по цене
        decimal StepSize { get; set; } // Размер шага по объёму

        Tokens Tokens { get; set; } // API токены для работы с биржей

        /// <summary>
        /// Exchange's enum identificator 
        /// </summary>
        Exchange Id { get; set; }

        // Methods: 
        Dictionary<string, List<Depth>> GetDOM(string cur1, string cur2, int limit = 1);
        void GetMinimals(string cur1, string cur2); // Получение минималок по ордерам
        decimal GetBalance(string cur); // Получить баланс по валюте
        //List<Quote> GetKlines(string cur1, string cur2, string interval, int limit); // Получить свечи
        decimal ConvertBaseToDollar(decimal amount, string cur2); // Конвертация базовой валюты в доллар (для расчёта профита)
        Order OpenLimitOrder(OrderSide orderSide, string cur1, string cur2, decimal amount, decimal price);
        Order OpenMarketOrder(OrderSide orderSide, string cur1, string cur2, decimal? amount = null, decimal? price = null, decimal? quote = null);
        Order GetOrderInfo(Order order); // Получить информацию по ордеру
        Order CancelOrder(Order order); // Отменить ордер

        //bool IsTrading { get; set; } // Флаг разрешающий торговлю : Для остановки бота вручную (изнутри класса биржи)

        decimal PriceChange(string cur1, string cur2); // Изменение цены за 24ч
        decimal Volume24h(string cur1, string cur2); // Текущий объем за 24 часа

        #region Methods

        Task<List<Quote>> GetQuotes(string cur1, string cur2, string interval, int limit);

        #endregion

        /// <summary>
        /// Token for the thread stopping
        /// </summary>
        CancellationTokenSource ts { get; set; }
        CancellationToken cancellationToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tid">TID for logging</param>
        void ClientInit(int tid);
    }
}
