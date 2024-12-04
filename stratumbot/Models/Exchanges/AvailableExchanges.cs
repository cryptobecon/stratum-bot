using stratumbot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models
{
    public class AvailableExchange
    {
        public Exchange Id { get; set; }
        public string Name { get; set; }
        public Level Level { get; set; }
    }

    public static class AvailableExchanges
    {
        public static List<AvailableExchange> AllAvailableExchanges = new List<AvailableExchange>();

        static AvailableExchanges()
        {
            // Binance
            AllAvailableExchanges.Add(
                new AvailableExchange
                {
                    Id = Exchange.Binance,
                    Name = "Binance",
                    Level = Level.BuyLimit
                }
            );

            // Binance Futures
            AllAvailableExchanges.Add(
                new AvailableExchange
                {
                    Id = Exchange.BinanceFutures,
                    Name = "Binance Futures",
                    Level = Level.BuyLimit
                }
            );

            // YoBit
            AllAvailableExchanges.Add(
                new AvailableExchange
                {
                    Id = Exchange.YoBit,
                    Name = "YoBit",
                    Level = Level.SellMarket
                }
            );
        }

        public static IExchange CreateExchangeById(Exchange _id)
        {
            if (_id == Exchange.Binance)
                return new Exchanges.BinanceSpot();
                //return new Exchanges.Binance();
            if (_id == Exchange.BinanceFutures)
                return new Exchanges.BinanceFutures();
            if (_id == Exchange.YoBit)
                return new Exchanges.Binance();// Yobit и тд

            throw new Exception($"{_id} exchange does not exist"); // Чтобы не ругался компилятор
        }

        // Получить все доступные стратегии
        public static List<AvailableExchange> GetAll()
        {
            List<AvailableExchange> MyAvailableExchanges = new List<AvailableExchange>();
            foreach (var exchange in AllAvailableExchanges)
            {
                // Проверка по уровню доступа
                if (Build.Level >= (int)exchange.Level)
                {
                    MyAvailableExchanges.Add(exchange);
                }
            }
            return MyAvailableExchanges;
        }

        
    }
}
