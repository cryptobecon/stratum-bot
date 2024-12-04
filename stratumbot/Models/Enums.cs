using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot
{
    // Биржи
    public enum Exchange
    {
        Binance = 1,
        BinanceFutures = 2,
        YoBit = 3,
        BinanceSpot = 4
    }

    // Стратегии
    public enum Strategy
    {
        Scalping = 1,
        ScalpingShort = 2, /* removed */
        ClassicLong = 3,
        ClassicShort = 4,
        Arbitrage = 5 /* removed */
    }

    // Уровни доступа
    public enum Level
    {
        BuyMarket = 1, // FREE
        BuyLimit, // LITE
        SellMarket, // PRO
        SellLimit // INFINITI
    }

    /*public enum OrderSideEnum
    {
        BUY = 1,
        SELL = 2
    }

    public enum OrderStatusEnum
    {
        New = 0,
        PartiallyFilled = 1,
        Filled = 2,
        Canceled = 3
    }*/

    class Enums
    {
    }
}
