using stratumbot.Core;
using stratumbot.DTO;
using stratumbot.Interfaces;
using stratumbot.Models.Logs;
using stratumbot.Models.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models
{
    class BtnPlus
    {
        /// <summary>
        /// Регистрация сессии в BtnPlus
        /// </summary>
        public static string GetSessionID(IConfig _config)
        {
            // Скальпинг + Classic Long + Classic Short
            if(_config.Strategy == Strategy.Scalping || _config.Strategy == Strategy.ClassicLong || _config.Strategy == Strategy.ClassicShort)
            {
                return GenerateRandomString(16);
            }

            throw new Exception("code 14"); // Нет стратегии? GetSessionID()
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";  // Define your character set
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
        }

        public static bool RegIteration(IIterationInfo _iterationInfo) 
        {
            // removed 
            return true;
        }

        public static void NewIteration(IStrategy _deal, decimal _iterationDurationSec, string _sid)
        {
            /*if(_deal is Scalping)
            {
                Scalping deal = _deal as Scalping;
                RegIteration(new ScalpingIterationInfo()
                {
                    Strategy = Strategy.Scalping,
                    Sid = _sid,
                    Exchange = deal.Exchange.Id,
                    Cur1 = deal.configuration.Cur1,
                    Cur2 = deal.configuration.Cur2,
                    Profit = deal.Profit,
                    BuyCounter = deal.BuyCounter,
                    SellCounter = deal.SellCounter,
                    DCAStepTriggered = deal?.DCA?.CurrentStep ?? 0, // TODO TEST DCA в итерацию в кабинет записывать какой
                    Duration = String.Format("{0:0}", _iterationDurationSec)
                });
            }

            if (_deal is ClassicLong)
            {
                ClassicLong deal = _deal as ClassicLong;
                RegIteration(new ClassicIterationInfo()
                {
                    Strategy = Strategy.ClassicLong,
                    Sid = _sid,
                    Exchange = deal.Exchange.Id,
                    Cur1 = deal.configuration.Cur1,
                    Cur2 = deal.configuration.Cur2,
                    Profit = deal.Profit,
                    BuyCounter = deal.BuyCounter,
                    SellCounter = deal.SellCounter,
                    DCAStepTriggered = deal?.DCA?.CurrentStep ?? 0, // TODO TEST DCA в итерацию в кабинет записывать какой
                    // TODO PT step добавить в будущем
                    Duration = String.Format("{0:0}", _iterationDurationSec)
                });
            }

            if (_deal is ClassicShort)
            {
                ClassicShort deal = _deal as ClassicShort;
                RegIteration(new ClassicIterationInfo()
                {
                    Strategy = Strategy.ClassicShort,
                    Sid = _sid,
                    Exchange = deal.Exchange.Id,
                    Cur1 = deal.configuration.Cur1,
                    Cur2 = deal.configuration.Cur2,
                    Profit = deal.Profit,
                    BuyCounter = deal.BuyCounter,
                    SellCounter = deal.SellCounter,
                    DCAStepTriggered = deal?.DCA?.CurrentStep ?? 0, // TODO TEST DCA в итерацию в кабинет записывать какой
                    // TODO PT step добавить в будущем
                    Duration = String.Format("{0:0}", _iterationDurationSec)
                });
            }*/
        }

        // Запросить лицензию по коду
        public static string RegLicenseCode(string _code)
        {
            return "removed";
        }

        // Активировать лицензию
        public static string ActiveteLicense(string _hash)
        {
            return "removed";
        }

        // Отправка Ошибки
        public static void SendError(string _errorText, string _code = "0")
        {
            // removed
        }
    }

    /// <summary>
    /// DTO для итерации  Скальпинг
    /// </summary>
    class ScalpingIterationInfo : IIterationInfo
    {
        public Strategy Strategy { get; set; } // Стратегия по которой была итерация
        public string Sid { get; set; } // sid btnplus
        public Exchange Exchange { get; set; }
        public string Cur1 { get; set; }
        public string Cur2 { get; set; }
        public Profit Profit { get; set; }
        public int BuyCounter { get; set; } // Счётчик выставленных BUY-ордеров
        public int SellCounter { get; set; }
        public int DCAStepTriggered { get; set; } // На каком шаге DCA была продажа
        public string Duration { get; set; } // sec in unix timestamp
    }

    /// <summary>
    /// DTO для итерации  Classic
    /// </summary>
    class ClassicIterationInfo : IIterationInfo
    {
        public Strategy Strategy { get; set; } // Стратегия по которой была итерация
        public string Sid { get; set; } // sid btnplus
        public Exchange Exchange { get; set; }
        public string Cur1 { get; set; }
        public string Cur2 { get; set; }
        public Profit Profit { get; set; }
        public int BuyCounter { get; set; } // Счётчик выставленных BUY-ордеров
        public int SellCounter { get; set; }
        public int DCAStepTriggered { get; set; } // На каком шаге DCA была продажа
        public string Duration { get; set; } // sec in unix timestamp
        // TODO Trailing Profit step info
    }
}
