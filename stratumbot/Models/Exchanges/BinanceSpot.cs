using BinanceNet = Binance.Net;
using BinanceNetClients = Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using stratumbot.Core;
using stratumbot.DTO;
using stratumbot.Interfaces;
using stratumbot.Models.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace stratumbot.Models.Exchanges
{
    public class BinanceSpot : IExchange
    {
        public Exchange Id { get; set; } = Exchange.Binance;
        public string Name { get; set; } = "Binance";

        /// <summary>
        /// Token for the thread stopping
        /// </summary>
        public CancellationTokenSource ts { get; set; }
        public CancellationToken cancellationToken { get; set; }

        #region События для Мои ордера

        public static event OpenOrderDelegate OpenOrderEvent; // TODO use by this.publicClient.OnOrderPlaced
        public static event CancelOrderDelegate CancelOrderEvent;
        public static event GetOrderInfoDelegate GetOrderInfoEvent;

        #endregion

        /// <summary>
        /// Maker fee (limit order)
        /// </summary>
        public decimal MakerFEE { get; set; } = (decimal)0.075;

        /// <summary>
        /// Taker fee (market order)
        /// </summary>
        public decimal TakerFEE { get; set; } = (decimal)0.075;

        //public decimal FEE { get; set; } = (decimal)0.075;

        public decimal MinAmount { get; set; } = 0;
        public decimal MinCost { get; set; } = 0;
        public decimal TickSize { get; set; } = 0;
        public decimal StepSize { get; set; } = 0;

        /// <summary>
        /// Private Client
        /// </summary>
        private BinanceNetClients.BinanceClient privateClient { get; set; }

        /// <summary>
        /// Public Client
        /// </summary>
        private BinanceNetClients.BinanceClient publicClient { get; set; }

        public Tokens Tokens { get; set; } = new Tokens();

        /// <summary>
        /// Available depth size limits
        /// </summary>
        private int[] DepthSize = new int[] { 5, 10, 20, 50, 100, 500, 1000, 5000 };

        public BinanceSpot()
        {
            // Set api tokens of this thread
            this.Tokens = TResource.GetAPI();
            
            // Check so the autofit will dont subscribe events more then once
            if (OpenOrderEvent == null)
            {
                OpenOrderEvent += OpenOrders.OpenOrderEventHandler; // Подписываем метод из статичкого класса на событие
                CancelOrderEvent += OpenOrders.CancelOrderEventHandler;
                GetOrderInfoEvent += OpenOrders.GetOrderInfoEventHandler;
            }
        }

        /// <summary>
        /// Public and private exchange client initialization
        /// </summary>
        public void ClientInit(int tid)
        {
            List<Microsoft.Extensions.Logging.ILogger> testlogger = 
                new List<Microsoft.Extensions.Logging.ILogger>();
            testlogger.Add(new LogAdapter(tid));

            // Init pablic client
            this.publicClient = new BinanceNetClients.BinanceClient(new BinanceClientOptions()
            {
                OutputOriginalData = true,
                LogWriters = testlogger,
                LogLevel = Microsoft.Extensions.Logging.LogLevel.Trace // was null
            });

            try
			{
                // Init private client
                this.privateClient = new BinanceNetClients.BinanceClient(new BinanceClientOptions()
                {
                    ApiCredentials = new ApiCredentials(Tokens.APIKey, Tokens.APISecret),
                    OutputOriginalData = true,
                    LogWriters = testlogger,
                    LogLevel = Microsoft.Extensions.Logging.LogLevel.Trace // was null
                });
            }
            catch
			{
                Logger.Debug("Not init private client (spot).");
			}

            
        }

        /// *********************************************************************
        ///             TRADE
        /// *********************************************************************

        /// <summary>
        /// Place a LIMIT order using OpenOrderAsync()
        /// </summary>
        public Order OpenLimitOrder(DTO.OrderSide orderSide, string cur1, string cur2, decimal amount, decimal price)
        {
            return this.OpenOrderAsync(orderSide, cur1, cur2, amount, price, "LIMIT").Result;
        }

        /// <summary>
        /// Place a MARKET order using OpenOrderAsync()
        /// </summary>
        public Order OpenMarketOrder(DTO.OrderSide orderSide, string cur1, string cur2, decimal? amount = null, decimal? price = null, decimal? quote = null)
        {
            return this.OpenOrderAsync(orderSide, cur1, cur2, amount, price, null, quote).Result;
            //return Task.Run(() => this.OpenOrderAsync(orderSide, cur1, cur2, amount), this.cancellationToken).Result;
        }

        /// <summary>
        /// Place an order
        /// </summary>
        /// <param name="_orderSide"></param>
        /// <param name="_cur1"></param>
        /// <param name="_cur2"></param>
        /// <param name="_amount"></param>
        /// <param name="_price"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public async Task<Order> OpenOrderAsync(DTO.OrderSide _orderSide, string _cur1, string _cur2, decimal? _amount = null, decimal? _price = null, string orderType = "MARKET", decimal? quote = null)

        {
			if (_amount != null)
			{
				if (_amount % this.StepSize != 0)
					throw new Exception($"code 3 {_amount} / {this.StepSize}"); // EX6
				if (_price != null && _price % this.TickSize != 0)
					throw new Exception($"code 4 {_price} / {this.TickSize}"); // EX7
			}
			

            //string amount = (_cur1 == "SHIB") ? Conv.s8(_amount, 0) : Conv.s8(_amount, 8);
            

            while (!this.cancellationToken.IsCancellationRequested)
            {
                Logger.Info(String.Format(_.Log26, (_orderSide == DTO.OrderSide.BUY) ? "BUY" : "SELL", (_price!=null) ? Conv.s8(_price, this.TickSize) : "MARKET")); // SIDE ордер по цене {price}

                var callResult = /*await*/ this.privateClient.SpotApi.Trading.PlaceOrderAsync
                (
                    _cur1 + _cur2,
                    (_orderSide == DTO.OrderSide.BUY) ? BinanceNet.Enums.OrderSide.Buy : BinanceNet.Enums.OrderSide.Sell,
                    (orderType == "LIMIT") ? SpotOrderType.Limit : SpotOrderType.Market,
                    _amount,
                    price: (_price == null) ? (decimal?)null : _price,
					quoteQuantity: quote,
					timeInForce: (orderType == "LIMIT") ? TimeInForce.GoodTillCanceled : (TimeInForce?)null,
                    receiveWindow: int.Parse(Settings.BinanceRecvWindow.ToString()),
                    ct: this.cancellationToken
                ).Result;

                if (!callResult.Success)
                {
                    // TODO all my errors or in bad case exception?
                    this.ErrorHandler(callResult.Error);
                }
                else
                {
                    Order order = new Order()
                    {
                        Cur1 = _cur1,
                        Cur2 = _cur2,
                        Id = callResult.Data.Id.ToString(),
                        Price = callResult.Data.AverageFillPrice ?? callResult.Data.Price,
                        Amount = callResult.Data.Quantity,
                        Filled = callResult.Data.QuantityFilled,
                        Remainder = callResult.Data.QuantityRemaining,
                        Side = (_orderSide == DTO.OrderSide.BUY) ? DTO.OrderSide.BUY : DTO.OrderSide.SELL,
                        Status = (callResult.Data.Status == BinanceNet.Enums.OrderStatus.New) ? DTO.OrderStatus.New : (callResult.Data.Status == BinanceNet.Enums.OrderStatus.Filled) ? DTO.OrderStatus.Filled : (callResult.Data.Status == BinanceNet.Enums.OrderStatus.PartiallyFilled) ? DTO.OrderStatus.PartiallyFilled : DTO.OrderStatus.Canceled,
                        Time = callResult.Data.CreateTime.ToString(),
                        FeeRate = callResult.Data.Type == SpotOrderType.Limit ? this.MakerFEE : this.TakerFEE
                    };

                    OpenOrderEvent(TID.CurrentID, TResource.Exchange[TID.CurrentID], TResource.Strategy[TID.CurrentID], order);

                    return order;
                }
            }

            throw new ManuallyStopException("code 6"); // Поптыки выставить ордер прекращены!
        }


        /// <summary>
        /// Get an order info using GetOrderInfoAsync()
        /// </summary>
        /// <param name="_order"></param>
        /// <returns></returns>
        public Order GetOrderInfo(Order _order)
        {
            return this.GetOrderInfoAsync(_order).Result;
        }

        /// <summary>
        /// Get an order info
        /// </summary>
        /// <param name="_order"></param>
        /// <returns></returns>
        public async Task<Order> GetOrderInfoAsync(Order _order)
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                var callResult = /*await*/ this.privateClient.SpotApi.Trading.GetOrderAsync
                (
                    _order.Cur1 + _order.Cur2,
                    orderId: long.Parse(_order.Id),
                    ct: this.cancellationToken
                ).Result;

                if (!callResult.Success)
                {
                    // TODO all my errors or in bad case exception
                    this.ErrorHandler(callResult.Error);
                }
                else
                {
                    Order order = new Order()
                    {
                        Cur1 = _order.Cur1,
                        Cur2 = _order.Cur2,
                        Id = callResult.Data.Id.ToString(),
                        Price = callResult.Data.AverageFillPrice ?? callResult.Data.Price,
                        Amount = callResult.Data.Quantity,
                        Filled = callResult.Data.QuantityFilled,
                        Remainder = callResult.Data.QuantityRemaining,
                        Side = _order.Side,
                        Status = (callResult.Data.Status == BinanceNet.Enums.OrderStatus.New) ? DTO.OrderStatus.New : (callResult.Data.Status == BinanceNet.Enums.OrderStatus.Filled) ? DTO.OrderStatus.Filled : (callResult.Data.Status == BinanceNet.Enums.OrderStatus.PartiallyFilled) ? DTO.OrderStatus.PartiallyFilled : DTO.OrderStatus.Canceled,
                        Time = callResult.Data.CreateTime.ToString(),
                        FeeRate = callResult.Data.Type == SpotOrderType.Limit ? this.MakerFEE : this.TakerFEE
                    };

                    GetOrderInfoEvent(TID.CurrentID, TResource.Exchange[TID.CurrentID], TResource.Strategy[TID.CurrentID], order);

                    return order;
                }
            }

            throw new ManuallyStopException("code 7"); // Поптыки получить инфу по ордеру прекращены!
        }

        /// <summary>
        /// Cancel an order using CancelOrderAsync()
        /// </summary>
        /// <param name="_order"></param>
        /// <returns></returns>
        public Order CancelOrder(Order _order)
        {
            return this.CancelOrderAsync(_order).Result;
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="_order"></param>
        /// <returns></returns>
        public async Task<Order> CancelOrderAsync(Order _order)
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                Logger.Info(_.Log42); // Отменяем ордер...

                var callResult = /*await*/ this.privateClient.SpotApi.Trading.CancelOrderAsync
                (
                    _order.Cur1 + _order.Cur2,
                    orderId: long.Parse(_order.Id),
                    ct: this.cancellationToken
                ).Result;

                if (!callResult.Success)
                {
                    Logger.Debug(_.Log29); // не удалось отменить ордер
                    this.ErrorHandler(callResult.Error);
                    Thread.Sleep(Settings.FailedQueryTimeout);
                }
                else
                {
                    CancelOrderEvent(callResult.Data.Id.ToString());

                    return new Order()
                    {
                        Cur1 = _order.Cur1,
                        Cur2 = _order.Cur2,
                        Id = callResult.Data.Id.ToString(),
                        Price = callResult.Data.Price,
                        Amount = callResult.Data.Quantity,
                        Filled = callResult.Data.QuantityFilled,
                        Remainder = callResult.Data.QuantityRemaining,
                        Side = _order.Side,
                        Status = (callResult.Data.Status == BinanceNet.Enums.OrderStatus.New) ? DTO.OrderStatus.New : (callResult.Data.Status == BinanceNet.Enums.OrderStatus.Filled) ? DTO.OrderStatus.Filled : (callResult.Data.Status == BinanceNet.Enums.OrderStatus.PartiallyFilled) ? DTO.OrderStatus.PartiallyFilled : DTO.OrderStatus.Canceled,
                        Time = callResult.Data.CreateTime.ToString(),
                        FeeRate = callResult.Data.Type == SpotOrderType.Limit ? this.MakerFEE : this.TakerFEE
                    };
                }
            }

            // TODO OrderFilledWhileWeCancelingException а где?
            throw new ManuallyStopException("code 8"); // Поптыки отменитт орлер прекращены!
        }

        /// *********************************************************************
        ///             ACCOUNT
        /// *********************************************************************

        /// <summary>
        /// Get user account balance info using GetBalanceAsync()
        /// </summary>
        public decimal GetBalance(string _cur)
        {
            return this.GetBalanceAsync(_cur).Result;
        }

        /// <summary>
        /// Get user account balance info
        /// </summary>
        /// <param name="_cur">Currecny</param>
        /// <returns>Free balance</returns>
        public async Task<decimal> GetBalanceAsync(string _cur)

        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                var callResult = /*await*/ this.privateClient.SpotApi.Account.GetAccountInfoAsync().Result;

                if (!callResult.Success)
                {
                    Logger.Info(_.Log31); // Не удалось получить баланс
                    this.ErrorHandler(callResult.Error);
                    System.Threading.Thread.Sleep(Settings.FailedQueryTimeout);
                }
                else
                {
                    foreach (var asset in ((BinanceNet.Objects.Models.Spot.BinanceBalance[])callResult.Data.Balances))
                    {
                        if (asset.Asset == _cur)
                            return asset.Available;
                    }
                }
            }

            throw new ManuallyStopException("code 10"); // EX4 Попытки получить баланс прекращены
        }

        /// *********************************************************************
        ///             MARKET INFO
        /// *********************************************************************

        /// <summary>
        /// Get the correct depth size based on the givin size. Get an equilal size or the next larger size.
        /// </summary>
        /// <param name="_limit"></param>
        /// <returns></returns>
        private int GetDepthLimit(int _limit)
        {
            if (_limit == 1)
            {
                return DepthSize[0];
            }
            else
            {
                foreach (var depthSize in DepthSize)
                {
                    if (_limit <= depthSize) return depthSize;
                }
            }

            throw new Exception("code 1"); // Not found the correct depth limit size
        }

        public void GetMinimals(string cur1, string cur2)
        {
            Task.Run( () => this.GetMinimalsAsync(cur1, cur2)).Wait();
            //this.GetMinimalsAsync(cur1, cur2).Wait();
        }

        /// <summary>
        /// Get exchange info about minimals
        /// </summary>
        /// <param name="cur1"></param>
        /// <param name="cur2"></param>
        public async Task GetMinimalsAsync(string cur1, string cur2)
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                var callResult = await this.publicClient.SpotApi.ExchangeData.GetExchangeInfoAsync(cur1 + cur2, this.cancellationToken);

                if (!callResult.Success)
                {
                    Logger.Info(_.Log30); // Не удалось получить минималки
                    this.ErrorHandler(callResult.Error);
                    System.Threading.Thread.Sleep(Settings.FailedQueryTimeout);
                }
                else
                {
					// Minimal amount of first currency (ex: 0.0001 ETH)
                    MinAmount = ((BinanceNet.Objects.Models.Spot.BinanceSymbol[])callResult.Data.Symbols)[0].LotSizeFilter.MinQuantity;
					// Amount step size (ex: +-0.0001 ETH)
					StepSize = ((BinanceNet.Objects.Models.Spot.BinanceSymbol[])callResult.Data.Symbols)[0].LotSizeFilter.StepSize;
					// Minimal total cost of second currency (ex: 10 USDT)
					MinCost = ((BinanceNet.Objects.Models.Spot.BinanceSymbol[])callResult.Data.Symbols)[0].MinNotionalFilter.MinNotional;
					// Price tick size (ex: +0.01 USDT)
					TickSize = ((BinanceNet.Objects.Models.Spot.BinanceSymbol[])callResult.Data.Symbols)[0].PriceFilter.TickSize;
                    break;
                }
            }
        }
        
        /// <summary>
        /// Get the orderbook
        /// </summary>
        /// <param name="cur1">First currency</param>
        /// <param name="cur2">Second (base) currency</param>
        /// <param name="limit">Number of orderbook items</param>
        /// <returns>(Dictionary<string, List<Depth>>) Orderbook</returns>
        public Dictionary<string, List<Depth>> GetDOM(string cur1, string cur2, int limit = 5)
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                var prices = this.publicClient.SpotApi.ExchangeData.GetOrderBookAsync(cur1 + cur2, this.GetDepthLimit(limit), this.cancellationToken).Result;

                if (!prices.Success)
                {
                    // TODO error
                    Logger.Debug("code 44");
                    this.ErrorHandler(prices.Error);
                }
                else
                {
                    // TODO SHIT
                    var DOMs = new Dictionary<string, List<Depth>>();
                    DOMs["asks"] = new List<Depth>();

                    foreach (var ask in prices.Data.Asks)
                    {
                        DOMs["asks"].Add(new Depth { Price = ask.Price, Amount = ask.Quantity, Cost = (ask.Price * ask.Quantity) });
                    }

                    DOMs["bids"] = new List<Depth>();

                    foreach (var bid in prices.Data.Bids)
                    {
                        DOMs["bids"].Add(new Depth { Price = bid.Price, Amount = bid.Quantity, Cost = (bid.Price * bid.Quantity) });
                    }

                    return DOMs;
                }
            }

            // TODO 25r42f2f52 if stop while filters chacking it can drop the app
            throw new AutoStopException("Попытки получить стаканы прекращены");
        }

        // TODO Put somewhere to another place/class
        private IEnumerable<Quote> Converter(IEnumerable<IBinanceKline> data)
        {
            foreach (var quote in data)
            {
                yield return new Quote
                {
                    Date = quote.CloseTime,
                    Open = quote.OpenPrice,
                    High = quote.HighPrice,
                    Low = quote.LowPrice,
                    Close = quote.ClosePrice,
                    Volume = quote.Volume
                };
            }
        }

        public KlineInterval ConvertStringInterval(string interval)
        {
            if (interval == "1m")
                return KlineInterval.OneMinute;
            if (interval == "3m")
                return KlineInterval.ThreeMinutes;
            if (interval == "5m")
                return KlineInterval.FiveMinutes;
            if (interval == "15m")
                return KlineInterval.FifteenMinutes;
            if (interval == "30m")
                return KlineInterval.ThirtyMinutes;
            if (interval == "1h")
                return KlineInterval.OneHour;
            if (interval == "2h")
                return KlineInterval.TwoHour;
            if (interval == "4h")
                return KlineInterval.FourHour;
            if (interval == "6h")
                return KlineInterval.SixHour;
            if (interval == "8h")
                return KlineInterval.EightHour;
            if (interval == "12h")
                return KlineInterval.TwelveHour;
            if (interval == "1d")
                return KlineInterval.OneDay;
            if (interval == "3d")
                return KlineInterval.ThreeDay;
            if (interval == "1w")
                return KlineInterval.OneWeek;
            if (interval == "1M")
                return KlineInterval.OneMonth;

            throw new Exception("No such a KlineInterval");
        }

        /// <summary>
        /// Get quotes
        /// </summary>
        /// <param name="cur1">First currency</param>
        /// <param name="cur2">Second (base) currency</param>
        /// <param name="interval">Time frame</param>
        /// <param name="limit">Period to compute filter's results</param>
        /// <returns></returns>
        public async Task<List<Quote>> GetQuotes(string cur1, string cur2, string interval, int limit)

        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                var result = /*await*/ this.publicClient.SpotApi.ExchangeData.GetKlinesAsync
                (
                    cur1 + cur2,
                    ConvertStringInterval(interval),
                    null,
                    null,
                    limit
                ).Result;

                if (!result.Success)
                {
                    Logs.Logger.ToFile("getquotes error:" + result.Error.ToString());
                    this.ErrorHandler(result.Error);
                }
                else
                {
                    return Converter(result.Data).ToList();
                }
            }

            // TODO error and check if it stop the thead when in doesnt need
            throw new AutoStopException("3453t3t54t5");
        }

        /// <summary>
        /// Get 24h price change percent
        /// </summary>
        /// <param name="cur1"></param>
        /// <param name="cur2"></param>
        /// <returns></returns>
        public decimal PriceChange(string cur1, string cur2)
        {
            // TODO add async + while (!this.cancellationToken.IsCancellationRequested)
            return this.publicClient.SpotApi.ExchangeData.GetTickerAsync(cur1 + cur2, this.cancellationToken).Result.Data.PriceChangePercent;

        }

        /// <summary>
        /// Get 24h volume of pair
        /// </summary>
        /// <param name="cur1"></param>
        /// <param name="cur2"></param>
        /// <returns></returns>
        public decimal Volume24h(string cur1, string cur2)
        {
            // TODO add async + while (!this.cancellationToken.IsCancellationRequested)
            return this.publicClient.SpotApi.ExchangeData.GetTickerAsync(cur1 + cur2).Result.Data.Volume;
        }

        /// <summary>
        /// Метод для конвертации базовой валюты в доллар
        /// </summary>
        public decimal ConvertBaseToDollar(decimal _amount, string _cur2)
        {
            // Долларовые валюты 
            if (new string[] { "USDT", "PAX", "TUSD", "USDC", "BUSD", "USDS" }.Contains(_cur2))
                return _amount * 1;

            // _cur2 / USD
            if (new string[] { "BTC", "ETH", "BNB", "TRX", "XRP", "EUR" }.Contains(_cur2))
            {
                var doms = this.GetDOM(_cur2, "USDT", 1);
                return _amount * doms["asks"][0].Price;
            }

            // USD / _cur2 
            if (new string[] { "RUB", "TRY" }.Contains(_cur2))
            {
                var doms = this.GetDOM("USDT", _cur2, 1);
                return _amount / doms["asks"][0].Price;
            }
            // also
            if (new string[] { "NGN" }.Contains(_cur2))
            {
                var doms = this.GetDOM("BUSD", _cur2, 1);
                return _amount / doms["asks"][0].Price;
            }

            throw new Exception(_.Log43); // Похоже новая базовая валюта
        }

        private void ErrorHandler(CryptoExchange.Net.Objects.Error error)
        {
            try
            {
                Logger.ToFile(error.ToString(), "[error] BinanceSpot.ErrorHandler");

                if (error.Message.Contains("balance"))
                {
                    Logger.Error(_.Log32); // На аккаунте недостаточно средаств 

                    // Проверка в BNB ли дело
                    if (this.GetBalance("BNB") < (decimal)0.0005)
                    {
                        Logger.Error(_.Log60); // Возможно нет BNB для оплаты комиссии
                        //Logger.Error(_.Log47);// На аккаунте недостаточно BNB для оплаты комиссии
                    }

                    Logger.Error(_.Log61); // Следующая попытка через 60 секунд

                    System.Threading.Thread.Sleep(60000);
                }
                else if (error.Message.Contains("not ex")) //
                {
                    Logger.Error("code 30");
                }
                else if (error.Message.Contains("Invalid symbol"))
                {
                    throw new InvalidParamException(_.Log33); // "Указаной пары нет на бирже!"
                }
                else if (error.Message.Contains("outside"))
                {
                    Logger.Error("code 31"); // nonce
                }
                else if (error.Message.Contains("ahead"))
                {
                    Logger.Error("code 32"); // nonce
                }
                else if (error.Message.Contains("UNKNOWN_ORDER"))
                {
                    Logger.Error("code 33");
                }
                else if (error.Message.Contains("Unknown order sent"))
                {
                    Logger.Error("code 34");
                    throw new OrderFilledWhileWeCancelingException(_.Log37); // Ордер вероятно исполнился пока отменялся
                }
                else if (error.Message.Contains("MIN_NOTIONAL"))
                {
                    Logger.Error("code 35");
                    // TODO automatocally
                    throw new ManuallyStopException("Остановка изза MIN_NOTIONAL"); 
                }
                else if (error.Message.Contains("502"))
                {
                    Logger.Error("code 36 Server error (502)");
                }
                else if (error.Message.Contains("500"))
                {
                    Logger.Error("code 37 Server error (500)");
                }
                else if (error.Message.Contains("504"))
                {
                    Logger.Error("code 39 Server error (504)");
                }
                else if (error.Message.Contains("Invalid API-key") || error.Message.Contains("API-key format invalid"))
                {
                    Logger.Error("API Key ERROR!");
                }
                else if (error.Message.Contains("unknown error occured while"))
                {
                    Logger.Error("An unknown error occured while processing the request");
                }
                else if (error.Message.Contains("Too much request"))
                {
                    Logger.Error("code 40 Too much request. Timeout for 60 sec...");
                    Thread.Sleep(60000);
                }
                else if (error.Message.Contains("Signature for this request is not valid"))
                {
                    Logger.Error("code 41");
                }
                else if (error.Message.Contains("Internal error"))
                {
                    Logger.Error("code 45 Exchange error");
                }
                /* NON EXCHANGE ERRORS */
                else if (error.Message.Contains("Request timed out"))
                {
                    Logger.Debug("code 42");
                }
                else if (error.Message.Contains("An error occurred while sending the request"))
                {
                    Logger.Error("code 43");
                }
                else
                {
                    Logger.Error(_.Log38); // Неизвестная ошибка! Покажите логи разработчику!
                }
            }
            catch (InvalidJsonException)
            {
                Logger.Error(_.Log39); // Сервер вернул неправильный ответ
            }
            catch (ManuallyStopException)
            {
                throw;
            }
            // TODO добавляю в 0,3,22 но опасная фигня
            catch (OrderFilledWhileWeCancelingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.ToFile($"[exception] BinanceSpot.ErrorHandler {ex.ToString()}");
            }
        }
    }
}