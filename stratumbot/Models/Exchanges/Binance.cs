using Newtonsoft.Json.Linq;
using stratumbot.Core;
using stratumbot.DTO;
using stratumbot.Interfaces;
using stratumbot.Models.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace stratumbot.Models.Exchanges
{
    public class RequestMethod
    {
        public string URL { get; set; }
        public string Method { get; set; }
        public bool Private { get; set; }
    }

    class Binance : IExchange
    {
        #region Temporary methods and fields
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<List<Quote>> GetQuotes(string cur1, string cur2, string interval, int limit)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            throw new Exception();
        }
        #endregion

        #region moved События для Мои ордера

        public static event OpenOrderDelegate OpenOrderEvent;
        public static event CancelOrderDelegate CancelOrderEvent;
        public static event GetOrderInfoDelegate GetOrderInfoEvent;

        #endregion

        // moved
        public Exchange Id { get; set; } = Exchange.Binance;
        // moved
        public string Name { get; set; } = "Binance";
        // DEPRECATED
        public string APIUrl = "https://api.binance.com/api/v3/";

        // moved
        public Tokens Tokens { get; set; } // Токены для доступа к бирже

        public decimal FEE { get; set; } = (decimal)0.075; // Комиссия

        private int[] DepthSize = new int[] { 5, 10, 20, 50, 100, 500, 1000, 5000 }; // Доступные размеры стаканов

        public decimal MinAmount { get; set; } // LOT_SIZE : minQty
        public decimal MinCost { get; set; } // MIN_NOTIONAL : minNotional
        public decimal TickSize { get; set; } // ? PRICE_FILTER : tickSize
        public decimal StepSize { get; set; } // ? LOT_SIZE : stepSize

        public bool IsTrading { get; set; } = true; // Безопасная остановка торговли внутри класса
        public CancellationTokenSource ts { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CancellationToken cancellationToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public decimal MakerFEE { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public decimal TakerFEE { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		// DEPRECATED Exchange's API methods
		public Dictionary<string, RequestMethod> Methods = new Dictionary<string, RequestMethod>
        {
            // Public
            ["time"] = (new RequestMethod { URL = "time", Method = "GET", Private = false }),
            ["depth"] = (new RequestMethod { URL = "depth", Method = "GET", Private = false }),
            ["exchangeInfo"] = (new RequestMethod { URL = "exchangeInfo", Method = "GET", Private = false }),
            ["klines"] = (new RequestMethod { URL = "klines", Method = "GET", Private = false }),
            ["ticker"] = (new RequestMethod { URL = "ticker/24hr", Method = "GET", Private = false }),
            // Private
            ["openOrder"] = (new RequestMethod { URL = "order", Method = "POST", Private = true }),
            ["orderInfo"] = (new RequestMethod { URL = "order", Method = "GET", Private = true }),
            ["cancelOrder"] = (new RequestMethod { URL = "order", Method = "DELETE", Private = true }),
            ["allOrders"] = (new RequestMethod { URL = "allOrders", Method = "GET", Private = true }),
            ["account"] = (new RequestMethod { URL = "account", Method = "GET", Private = true })
        };

        // moved
        public Binance()
        {
            this.Tokens = TResource.GetAPI(); // Указываем, что токены для работы это токены от данного потока
            if(OpenOrderEvent == null) // Проверка, чтобы автоподбор не подписывал события и потоки больше одного раза
            {
                OpenOrderEvent += OpenOrders.OpenOrderEventHandler; // Подписываем метод из статичкого класса на событие
                CancelOrderEvent += OpenOrders.CancelOrderEventHandler;
                GetOrderInfoEvent += OpenOrders.GetOrderInfoEventHandler;
            }
        }

        // DEPRECATED Получаем свечи
        public List<Quote> GetKlines(string _cur1, string _cur2, string _interval = "1m", int _limit = 20)
        {
            var json = (JToken)Request(Methods["klines"], "symbol=" + _cur1 + _cur2 + "&interval=" + _interval + "&limit=" + _limit);

            List<Quote> candles = new List<Quote>();

            foreach (var item in json)
            {
                var Open = item.First.Next;
                var High = item.First.Next.Next;
                var Low = item.First.Next.Next.Next;
                var Close = item.First.Next.Next.Next.Next;
                var Volume = item.First.Next.Next.Next.Next.Next;
                candles.Add(new Quote() { Open = Conv.dec(Open), High = Conv.dec(High), Low = Conv.dec(Low), Close = Conv.dec(Close), Volume = Conv.dec(Volume) });
            }
            return candles;
        }

        private int GetDepthLimit(int _limit)
        {
            if (_limit == 1)
            {
                return DepthSize[0];
            }
            else
            {
                // Подбираем размер стакана, такой что запрашиваемый лимит либо равен либо больше на один шаг чем нужно
                foreach (var depthSize in DepthSize)
                {
                    if (_limit <= depthSize)
                        return depthSize;
                }
            }
            throw new Exception("code 1"); // GetDepthLimit не подобрал Depth Limit

        }

        // moved Получить словарь стаканов (два списка из Depth) (bid & ask)
        public Dictionary<string, List<Depth>> GetDOM(string _cur1, string cur2, int _limit = 1)
        {
            int limit = this.GetDepthLimit(_limit);

            while (IsTrading)
            {
                var json = Request(Methods["depth"], "symbol=" + _cur1 + cur2 + "&limit=" + limit);

                if(json.ToString().Contains("bids"))
                {
                    var AskDOM = new List<Depth>();
                    var BidDOM = new List<Depth>();

                    // Заполняем стакан на продажу 
                    int count = 0;
                    var asks = json.Last.First;
                    foreach (var ask in asks)
                    {
                        var price = (decimal)ask[0];
                        var amount = (decimal)ask[1];
                        var cost = price * amount;
                        AskDOM.Add(new Depth { Price = price, Amount = amount, Cost = cost });
                        count++;
                        if (_limit == 1) // Для получения спреда не нужно дальше пробегать
                            break;
                        if (count == limit)
                            break;
                    }
                    // Заполняем стакан на покупку
                    count = 0;
                    var bids = json.Last.Previous.First;
                    foreach (var bid in bids)
                    {
                        var price = (decimal)(bid[0]);
                        var amount = (decimal)(bid[1]);
                        var cost = price * amount;
                        BidDOM.Add(new Depth { Price = price, Amount = amount, Cost = cost });
                        count++;
                        if (_limit == 1) // Для получения спреда не нужно дальше пробегать
                            break;
                        if (count == limit)
                            break;
                    }

                    var DOMs = new Dictionary<string, List<Depth>>();
                    DOMs["asks"] = AskDOM;
                    DOMs["bids"] = BidDOM;
                    return DOMs;
                } else
                {
                    // Error
                    Logger.Info(_.Log41); // Попытка получить стаканы
                    System.Threading.Thread.Sleep(Settings.FailedQueryTimeout);
                }
            }
            throw new ManuallyStopException("code 5"); // Попытки получить стаканы прекращены
        }

        // Открыть новый ордер общий
        public Order OpenOrder(OrderSide _orderSide, string _cur1, string _cur2, decimal _amount, decimal _price = 0, string orderType = "MARKET")
        {
            if (_amount % this.StepSize != 0)
                throw new Exception($"code 3 {_amount} / {this.StepSize}"); // EX6
            if (_price % this.TickSize != 0)
                throw new Exception($"code 4 {_price} / {this.TickSize}"); // EX7

            string amount = Conv.s8(_amount, 8);

            if (_cur1 == "SHIB")
            {
                amount = Conv.s8(_amount, 0);
            }
            

            string price = Conv.s8(_price, this.TickSize);
            string side = (_orderSide == OrderSide.BUY) ? "BUY" : "SELL";
            string priceStr = (orderType == "LIMIT") ? "&price=" + price : "";
            string timeInForce = (orderType == "LIMIT") ? "&timeInForce=GTC" : "";
            Logger.Info(String.Format(_.Log26, side, price)); // SIDE ордер по цене {price}

            while (IsTrading) // Бесконечные попытки совершить покупку
            {
                var response = Request(Methods["openOrder"], ("symbol=" + _cur1 + _cur2 + "&side=" + side + "&type=" + orderType + timeInForce + "&quantity=" + amount + priceStr + "&recvWindow=" + Settings.BinanceRecvWindow).Replace(",", "."));
                if (response.ToString().Contains("NEW") || response.ToString().Contains("FILLED") || response.ToString().Contains("PARTIALLY_FILLED"))
                {
                    Order order = ParseOrder(response, _cur1, _cur2); // Ok, order was created
                    OpenOrderEvent(TID.CurrentID, TResource.Exchange[TID.CurrentID], TResource.Strategy[TID.CurrentID], order);
                    return order;
                }
                else
                {
                    // Error
                    Logger.Info(_.Log27); // Попытка выставить ордер...
                    System.Threading.Thread.Sleep(Settings.FailedQueryTimeout);
                }
            }

            throw new ManuallyStopException("code 6"); // Поптыки выставить ордер прекращены!
        }

        // Открыть лимитный ордер
        public Order OpenLimitOrder(OrderSide _orderSide, string _cur1, string _cur2, decimal _amount, decimal _price)
        {
            return OpenOrder(_orderSide, _cur1, _cur2, _amount, _price, "LIMIT");
        }

        // Открыть маркет ордер
        public Order OpenMarketOrder(OrderSide _orderSide, string _cur1, string _cur2, decimal _amount, decimal _price = 0)
        {
            return OpenOrder(_orderSide, _cur1, _cur2, _amount); // price и MARKET не надо указывать
        }

        // Информация по ордеру
        public Order GetOrderInfo(Order _order)
        {
            while (IsTrading)
            {
                var response = Request(Methods["orderInfo"], ("symbol=" + _order.Cur1 + _order.Cur2 + "&orderId=" + _order.Id));
                if(response.ToString().Contains("status"))
                {
                    Order order = ParseOrder(response, _order.Cur1, _order.Cur2); // Ok
                    GetOrderInfoEvent(TID.CurrentID, TResource.Exchange[TID.CurrentID], TResource.Strategy[TID.CurrentID], order);
                    return order;
                } else
                {
                    Logger.Debug(_.Log28);
                    Thread.Sleep(Settings.FailedQueryTimeout);
                }
            }

            throw new ManuallyStopException("code 7"); // Поптыки получить инфу по ордеру прекращены!
        }

        // Распаршивает ордер
        private Order ParseOrder(JToken _json, string _cur1, string _cur2)
        {

            Order order = new Order
            {
                Cur1 = _cur1,
                Cur2 = _cur2,
                Id = _json["orderId"].ToString(),
                Price = ( Conv.dec(_json["price"]) == 0) ? (Conv.dec(_json["cummulativeQuoteQty"]) / Conv.dec(_json["executedQty"])) : Conv.dec(_json["price"]),
                Amount = Conv.dec(_json["origQty"]),
                Filled = Conv.dec(_json["executedQty"]),
                Remainder = Conv.dec(_json["origQty"]) - Conv.dec(_json["executedQty"]),
                Side = (_json["side"].ToString() == "BUY") ? OrderSide.BUY : OrderSide.SELL,
                Status = (_json["status"].ToString() == "NEW") ? OrderStatus.New :
                         (_json["status"].ToString() == "FILLED") ? OrderStatus.Filled :
                         (_json["status"].ToString() == "PARTIALLY_FILLED") ? OrderStatus.PartiallyFilled :
                         OrderStatus.Canceled,
                Time = _json.Value<string>("transactTime") ?? _json.Value<string>("time") ?? "0"
            };
            return order;
        }

        public Order CancelOrder(Order _order)
        {
            Logger.Info(_.Log42); // Отменяем ордер...
            while (IsTrading)
            {
                var response = Request(Methods["cancelOrder"], ("symbol=" + _order.Cur1 + _order.Cur2 + "&orderId=" + _order.Id));

                if (response.ToString().Contains("status")) // Ok
                {
                    Order order = ParseOrder(response, _order.Cur1, _order.Cur2); // Ok
                    CancelOrderEvent(order.Id);
                    return order;
                }
                else
                {
                    Logger.Debug(_.Log29); // не удалось отменить ордер
                    Thread.Sleep(Settings.FailedQueryTimeout);
                }
            }

            throw new ManuallyStopException("code 8"); // Поптыки отменитт орлер прекращены!
        }

        public /*async*/ void GetMinimals(string cur1, string cur2)
        {
            /*await*/ //Task.Run( () => {
            while (IsTrading)
            {
                var json = Request(Methods["exchangeInfo"]);

                if(json.ToString().Contains("symbols"))
                {
                    foreach (var symbol in json["symbols"])
                    {
                        if (symbol["symbol"].ToString() != (cur1 + cur2))
                            continue;

                        foreach (var filter in symbol["filters"])
                        {
                            if ((string)filter["filterType"] == "LOT_SIZE")
                            {
                                MinAmount = Conv.dec(filter["minQty"]);
                                StepSize = Conv.dec(filter["stepSize"]);
                            }
                            if ((string)filter["filterType"] == "MIN_NOTIONAL")
                            {
                                MinCost = Conv.dec(filter["minNotional"]);
                            }
                            if ((string)filter["filterType"] == "PRICE_FILTER")
                            {
                                TickSize = Conv.dec(filter["tickSize"]);
                            }
                        }
                    }
                    return; // Выходим из while, тк получили все минималки
                } else
                {
                    // Error
                    Logger.Info(_.Log30); // Не удалось получить минималки
                    System.Threading.Thread.Sleep(Settings.FailedQueryTimeout);
                }
            }

            throw new ManuallyStopException("code 9"); // Попытки получить минималки прекращены
            //});
        }

        public decimal GetBalance(string _cur)
        {
            while (IsTrading)
            {
                var json = Request(Methods["account"]);
                if(json.ToString().Contains("balances"))
                {
                    foreach (var balance in json["balances"])
                    {
                        if (balance["asset"].ToString() == _cur.ToUpper())
                        {
                            return Conv.dec(balance["free"]);
                        }
                    }
                } else
                {
                    // Error
                    Logger.Info(_.Log31); // Не удалось получить баланс
                    System.Threading.Thread.Sleep(Settings.FailedQueryTimeout);
                }
            }
            throw new ManuallyStopException("code 10"); // EX4 Попытки получить баланс прекращены
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

        public JToken GetServerTime()
        {
           return Request(Methods["time"]);
        }

        // Генерация SIGN параметра для запроса на Binance
        private string GenSignature(string _apiSecret, string _urlParam)
        {
            var keyByte = Encoding.UTF8.GetBytes(_apiSecret);
            byte[] inputBytes = Encoding.UTF8.GetBytes(_urlParam);
            using (var hmac = new System.Security.Cryptography.HMACSHA256(keyByte))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                StringBuilder hex1 = new StringBuilder(hashValue.Length * 2);
                foreach (byte b in hashValue)
                {
                    hex1.AppendFormat("{0:x2}", b);
                }
                return hex1.ToString();
            }
        }



        public JToken PublicRequest(RequestMethod _request, string _urlParam = null)
        {
            var web = new WebClient();
            web.Headers.Add("ForF: stratum-bot");
            var param = (_urlParam != null) ? "?" + _urlParam : "";
            Logger.ToFile($"[request] {APIUrl + _request.URL + param}");
            var response = web.DownloadString(APIUrl + _request.URL + param);
            Logger.ToFile($"[response] {response.Substring(0, Math.Min(Settings.ResponseLenth, response.Length))}");
            if (!response.IsValidJson())
                throw new InvalidJsonException(_.Log39); // Биржа вернула неправильный ответ
            return JToken.Parse(response);
        }

        public JToken PrivateRequest(RequestMethod _request, string _urlParam = null)
        {
            JToken json = GetServerTime();

            string parameters = _urlParam + "&timestamp=" + ((decimal)json["serverTime"] - 2000);
            parameters = parameters + "&signature=" + GenSignature(Tokens.APISecret, parameters);

            if (_request.Method != "GET") /* POST DELETE */
            {
                string address = APIUrl + _request.URL;

                Logger.ToFile($"[request] {address}?{parameters}");

                WebRequest webRequest = (HttpWebRequest)WebRequest.Create(address);
                webRequest.Method = _request.Method;
                webRequest.Timeout = 60000;
                ((HttpWebRequest)webRequest).UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Headers.Add("X-MBX-APIKEY", Tokens.APIKey);
                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(Encoding.UTF8.GetBytes(parameters), 0, parameters.Length);
                }
                using (Stream stream = webRequest.GetResponse().GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var response = reader.ReadToEnd();
                    Logger.ToFile($"[response] {response.Substring(0, Math.Min(Settings.ResponseLenth, response.Length))}");
                    if (!response.IsValidJson())
                        throw new InvalidJsonException(_.Log39); // Сервер вернул неправильный ответ
                    return JToken.Parse(response);
                }


            }
            else /* GET */
            {

                string url = APIUrl + _request.URL + "?" + parameters;
                Logger.ToFile($"[request] {url}");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add("X-MBX-APIKEY", Tokens.APIKey);
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                using (Stream stream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var response = reader.ReadToEnd();
                    Logger.ToFile($"[response] {response.Substring(0, Math.Min(Settings.ResponseLenth, response.Length))}");
                    if (!response.IsValidJson())
                        throw new InvalidJsonException(_.Log39); // Сервер вернул неправильный ответ
                    return JToken.Parse(response);
                }
            }

            throw new Exception("code 2"); // PrivateRequest error
        }

        public JToken Request(RequestMethod _request, string _urlParam = null)
        {
            while (IsTrading)
            {
                try
                {
                    if (!_request.Private)
                    {
                        return PublicRequest(_request, _urlParam);
                    }
                    else
                    {
                        return PrivateRequest(_request, _urlParam);
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Response == null)
                    {
                        Logger.ToFile($"[exception] code 29 {ex.ToString()}");
                    }
                    else
                    {
                        string response = "";
                        using (WebResponse webResponse = ex.Response)
                        using (Stream stream = webResponse.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {
                            response = reader.ReadToEnd();
                            Logger.ToFile($"[response] {response.Substring(0, Math.Min(Settings.ResponseLenth, response.Length))}");

                            if (response.Contains("balance"))
                            {
                                // Проверка в BNB ли дело
                                decimal bnbBalance = 0; 
                                var account = Request(Methods["account"]);
                                foreach (var balance in account["balances"])
                                {
                                    if ((string)balance["asset"] != "BNB")
                                        continue;

                                    bnbBalance = Conv.dec((string)balance["free"]);
                                    break;
                                }

                                if(bnbBalance < (decimal)0.001)
                                {
                                    Logger.Error(_.Log47);// На аккаунте недостаточно BNB для оплаты комиссии
                                } else
                                {
                                    Logger.Error(_.Log32); // На аккаунте недостаточно средаств 
                                }
                                
                                System.Threading.Thread.Sleep(60000);
                            }
                            else if (response.Contains("not ex")) //
                            {
                                Logger.Error("code 30");
                            }
                            else if (response.Contains("Invalid symbol"))
                            {
                                throw new InvalidParamException(_.Log33); // "Указаной пары нет на бирже!"
                            }
                            else if (response.Contains("outside"))
                            {
                                Logger.Error("code 31"); // nonce
                            }
                            else if (response.Contains("ahead"))
                            {
                                Logger.Error("code 32"); // nonce
                            }
                            else if (response.Contains("UNKNOWN_ORDER"))
                            {
                                Logger.Error("code 33");
                            }
                            else if (response.Contains("Unknown order sent"))
                            {
                                Logger.Error("code 34");
                                throw new OrderFilledWhileWeCancelingException(_.Log37); // Ордер вероятно исполнился пока отменялся
                            }
                            else if (response.Contains("MIN_NOTIONAL"))
                            {
                                Logger.Error("code 35");
                            }
                            else if (response.Contains("502"))
                            {
                                Logger.Error("code 36 Server error (502)");
                            }
                            else if (response.Contains("500"))
                            {
                                Logger.Error("code 37 Server error (500)");
                            }
                            else if (response.Contains("504"))
                            {
                                Logger.Error("code 39 Server error (504)");
                            }
                            else if (response.Contains("Invalid API-key") || response.Contains("API-key format invalid"))
                            {
                                Logger.Error("API Key ERROR!");
                            }
                            else if (response.Contains("unknown error occured while"))
                            {
                                Logger.Error("An unknown error occured while processing the request");
                            }
                            else if (response.Contains("Too much request"))
                            {
                                Logger.Error("code 40 Too much request. Timeout for 60 sec...");
                                Thread.Sleep(60000);
                            }
                            else if (response.Contains("Signature for this request is not valid"))
                            {
                                Logger.Error("code 41");
                            }
                            else
                            {
                                Logger.Error(_.Log38); // Неизвестная ошибка! Покажите логи разработчику!
                            }
                        }
                        if (!response.IsValidJson())
                            throw new InvalidJsonException(_.Log39); // Сервер вернул неправильный ответ
                        return JObject.Parse(response);
                    }
                }
                catch(InvalidJsonException)
                {
                    Logger.Error(_.Log39); // Сервер вернул неправильный ответ
                }
                catch (Exception ex) 
                {
                    Logger.ToFile($"[exception] {ex.ToString()}");
                }

                Logger.Error(_.Log40); // Ошибка при отправке запроса? Заново скоро будем
                Thread.Sleep(Settings.FailedQueryTimeout);
            }

            throw new ManuallyStopException("code 11"); // Поптыки отправить запрос прекращены!
        }

        public decimal PriceChange(string _cur1, string _cur2)
        {
            var response = Request(Methods["ticker"], ("symbol=" + _cur1 + _cur2));
            return Conv.dec(response["priceChangePercent"]);
        }

        public decimal Volume24h(string _cur1, string _cur2)
        {
            var response = Request(Methods["ticker"], ("symbol=" + _cur1 + _cur2));
            return Conv.dec(response["quoteVolume"]);
        }

        public void ClientInit(int tid)
        {
            throw new NotImplementedException();
        }

		public Order OpenMarketOrder(OrderSide orderSide, string cur1, string cur2, decimal? amount, decimal? price, decimal? quote)
		{
			throw new NotImplementedException();
		}
	}
}
