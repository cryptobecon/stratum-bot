using stratumbot.Interfaces;
using stratumbot.Models.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models
{
    // DTO for Available Strategies
    public class AvailableStrategy
    {
        public Strategy Id { get; set; }
        public Level Level { get; set; }
        public string Name { get; set; }
        public Exchange[] Exchanges { get; set; } 
    }

    // List of all strategies in the bot
    static class AvailableStrategies
    {
        public static List<AvailableStrategy> AllAvailableStrategies = new List<AvailableStrategy>();

        static AvailableStrategies()
        {
            // Scalping
            AllAvailableStrategies.Add(
                new AvailableStrategy
                {
                    Id = Strategy.Scalping,
                    Level = Level.BuyLimit,
                    Name = "Scalping",
                    Exchanges = new[] {
                        Exchange.Binance,
                        Exchange.BinanceFutures,
                        Exchange.YoBit
                    }
                }
            );

            // Classic Long
            AllAvailableStrategies.Add(
                new AvailableStrategy
                {
                    Id = Strategy.ClassicLong,
                    Level = Level.BuyLimit,
                    Name = "Classic Long",
                    Exchanges = new[] {
                        Exchange.Binance,
                        Exchange.BinanceFutures
                    }
                }
            );

            // Classic Short
            AllAvailableStrategies.Add(
                new AvailableStrategy
                {
                    Id = Strategy.ClassicShort,
                    Level = Level.BuyLimit,
                    Name = "Classic Short",
                    Exchanges = new[] {
                        Exchange.Binance,
                        Exchange.BinanceFutures
                    }
                }
            );

        }

        public static IStrategy CreateStrategyByConfig(IConfig _config, Exchange exchange)
        {
            if (_config.Strategy == Strategy.Scalping)
                return new Strategies.Scalping((_config as ScalpingConfig), exchange);
            if (_config.Strategy == Strategy.ClassicLong)
                return new Strategies.ClassicLong((_config as ClassicLongConfig), exchange);
            if (_config.Strategy == Strategy.ClassicShort)
                return new Strategies.ClassicShort((_config as ClassicShortConfig), exchange);

            throw new Exception("code 21"); // Нет такой стратегии {_config.Strategy}
        }

        /// <summary>
        /// Получить все доступные стратегии по определенной бирже
        /// </summary>
        public static List<AvailableStrategy> GetAll(Exchange exchange)
        {
            List<AvailableStrategy> MyAvailableStrategies = new List<AvailableStrategy>();
            foreach (var strategy in AllAvailableStrategies)
            {
                // Проверка по уровню доступа
                if (Build.Level >= (int)strategy.Level)
                {
                    // Проверка по подключенной бирже
                    if(strategy.Exchanges.Contains(exchange))
                    {
                        MyAvailableStrategies.Add(strategy);
                    }
                }
            }
            return MyAvailableStrategies;
        }

        /// <summary>
        /// Получить все доступные стратегии
        /// </summary>
        public static List<AvailableStrategy> GetAll()
        {
            List<AvailableStrategy> MyAvailableStrategies = new List<AvailableStrategy>();
            foreach (var strategy in AllAvailableStrategies)
            {
                // Проверка по уровню доступа
                if (Build.Level >= (int)strategy.Level)
                {
                    MyAvailableStrategies.Add(strategy);
                }
            }
            return MyAvailableStrategies;
        }

        /// <summary>
        /// Метод для проверки доступна ли стратегия _strategyId для биржи _exchangeId
        /// </summary>
        public static bool IsExchangeCompatible(Strategy _strategyId, Exchange _exchangeId)
        {
            foreach(var strategy in AllAvailableStrategies)
            {
                if(strategy.Id == _strategyId)
                {
                    return strategy.Exchanges.Contains(_exchangeId);
                }
            }

            throw new Exception("code 15");
        }

    }
}
