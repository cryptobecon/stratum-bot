using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using stratumbot.Core;
using stratumbot.DTO;
using stratumbot.Interfaces;
using stratumbot.Models.Strategies;
using stratumbot.Models.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models
{
    /// <summary>
    /// Класс для работы с резервным восстановлением потоков: DTO
    /// </summary>
    public class ThreadBackup
    {
        [JsonProperty("Action")]
        public string Action { get; set; } // Шаг, действие CHECK, BUY, SELL
        [JsonProperty("IsMustRecover")]
        public bool IsMustRecover { get; set; } // Нужно ли восстановить данный поток (выбирает пользователь)
        [JsonProperty("BackupFile")]
        public string BackupFile { get; set; } // Файл для восстановления 
        // Таблица рабочих потоков:
        [JsonProperty("Time")]
        public string Time { get; set; } // Дата-время создания потока
        [JsonProperty("Iteration")]
        public int Iteration { get; set; } // Кол-во итераций по потоку
        [JsonProperty("ProfitPercent")]
        public decimal ProfitPercent { get; set; } // Профит в %
        [JsonProperty("Profit")]
        public decimal Profit { get; set; } // Профит в базовой
        [JsonProperty("ProfitUSD")]
        public decimal ProfitUSD { get; set; } // Профит в USD
        [JsonProperty("Pair")]
        public string Pair { get; set; } // Пара
        [JsonProperty("Budget")]
        public string Budget { get; set; } // Бюджет
        [JsonProperty("DCAStep")]
        public int DCAStep { get; set; } // Текущий DCA шаг

        //public decimal Amount { get; set; } // Текущий объем
        [JsonProperty("Order")]
        public Order Order { get; set; } // Ордер, который проверялся до сбоя
        [JsonProperty("OrderPool")]
        public Trades OrderPool { get; set; }
        [JsonProperty("Config")]
        public IConfig Config { get; set; } // Конфиг стратегии

        [JsonProperty("Session")]
        public string Session { get; set; }

        [JsonProperty("BuyCounter")]
        public int BuyCounter { get; set; } // счёчик buy ордеров
        [JsonProperty("SellCounter")]
        public int SellCounter { get; set; }  // счёчик sell ордеров

        // DCA
        //public bool IsTriggered { get; set; } // Сработал ли DCA (выставлен хотя бы один BUY)
        //public List<DCAStep> Step { get; set; } // Параметры по шагам DCA
        //public int CurrentStep { get; set; } // Текущий шаг DCA

        public static ThreadBackup ReadBackup(string _json)
        {
            if (!_json.IsValidJson())
                throw new Exception("json для чтения бекапа инвалидный!");

            ThreadBackup backup = new ThreadBackup();

            var json = JObject.Parse(_json);

            backup.Action = (string)json["Action"];
            backup.IsMustRecover = true; // По умолчанию все чекбоксы вкл и потоки выделены на восстановление
            backup.Time = (string)json["Time"];
            backup.Iteration = (int)json["Iteration"];
            backup.ProfitPercent = (decimal)json["ProfitPercent"];
            backup.Profit = (decimal)json["Profit"];
            backup.ProfitUSD = (decimal)json["ProfitUSD"];
            backup.Pair = (string)json["Pair"];
            backup.Budget = (string)json["Budget"];
            backup.DCAStep = (int)json["DCAStep"];

            // DCA
            //backup.IsTriggered = (bool)json["IsTriggered"];
            //backup.Step = JsonConvert.DeserializeObject<List<DCAStep>>(json["Step"].ToString()); // TODO TEST
            //backup.CurrentStep = (int)json["CurrentStep"];

            //backup.Amount = Conv.dec(json["Amount"].ToString());

            backup.Order = new Order();
            if(json["Order"].ToString() != "")
            {
                backup.Order.Id = (string)json["Order"]["Id"];
                backup.Order.Cur1 = (string)json["Order"]["Cur1"];
                backup.Order.Cur2 = (string)json["Order"]["Cur2"];
                backup.Order.Time = (string)json["Order"]["Time"];
                backup.Order.Price = (decimal)json["Order"]["Price"];
                backup.Order.Amount = (decimal)json["Order"]["Amount"];
                backup.Order.Filled = (decimal)json["Order"]["Filled"];
                backup.Order.Remainder = (decimal)json["Order"]["Remainder"];
                backup.Order.Side = (OrderSide)(int)json["Order"]["Side"];
                backup.Order.Status = (OrderStatus)(int)json["Order"]["Status"];
            }
            

            backup.OrderPool = new Trades();
            if (json["OrderPool"].ToString() != "")
            {
                foreach (var buyOrder in json["OrderPool"]["BUY"])
                {
                    backup.OrderPool.BUY.Add(new DTO.Order
                    {
                        Id = (string)buyOrder["Id"],
                        Side = (OrderSide)(int)buyOrder["Side"],
                        Price = (decimal)buyOrder["Price"],
                        Amount = (decimal)buyOrder["Amount"],
                        Cur2 = (string)buyOrder["Cur2"]
                    });
                }
                foreach (var buyOrder in json["OrderPool"]["SELL"])
                {
                    backup.OrderPool.SELL.Add(new DTO.Order
                    {
                        Id = (string)buyOrder["Id"],
                        Side = (OrderSide)(int)buyOrder["Side"],
                        Price = (decimal)buyOrder["Price"],
                        Amount = (decimal)buyOrder["Amount"],
                        Cur2 = (string)buyOrder["Cur2"]
                    });
                }
            }

            backup.Session = (string)json["Session"];
            backup.BuyCounter = (int)json["BuyCounter"];
            backup.SellCounter = (int)json["SellCounter"];

            // Конфиг парсим
            Strategy strategy = (Strategy)(int)json["Config"]["Strategy"];

            if(strategy == Strategy.Scalping)
            {
                /*ScalpingConfigJson config = new ScalpingConfigJson()
                {
                    Strategy = Strategy.Scalping,
                    Exchange = (Exchange)(int)json["Config"]["Exchange"],
                    Budget = (decimal)json["Config"]["Budget"], // TODO can be some issue with % budget. before here was a string type
                    Cur1 = (string)json["Config"]["Cur1"],
                    Cur2 = (string)json["Config"]["Cur2"],
                    IsDCA = (bool)json["Config"]["IsDCA"],
                    DCAStepCountStr = (string)json["Config"]["DCAStepCountStr"],
                    DCAProfitPercentStr = (string)json["Config"]["DCAProfitPercentStr"],
                    DCAStepsStr = JsonConvert.DeserializeObject<List<string[]>>(json["Config"]["DCAStepsStr"].ToString()),
                    MinSpreadStr = (string)json["Config"]["MinSpreadStr"],
                    OptSpreadStr = (string)json["Config"]["OptSpreadStr"],
                    MinMarkupStr = (string)json["Config"]["MinMarkupStr"],
                    OptMarkupStr = (string)json["Config"]["OptMarkupStr"],
                    ZeroSellStr = (string)json["Config"]["ZeroSellStr"],
                    InTimeoutStr = (string)json["Config"]["InTimeoutStr"],
                    FirsOredersAmountPercentIgnorStr = (string)json["Config"]["FirsOredersAmountPercentIgnorStr"],
                    FirsOredersCountIgnorStr = (string)json["Config"]["FirsOredersCountIgnorStr"],
                    StopLossStr = (string)json["Config"]["StopLossStr"],
                    IsSellStart = (bool)json["Config"]["IsSellStart"],
                    BuyPriceForSellStartStr = (string)json["Config"]["BuyPriceForSellStartStr"],
                    AmountForSellStartStr = (string)json["Config"]["AmountForSellStartStr"],
                    FiltersBuy = JsonConvert.DeserializeObject<List<Filters.JsonFilter>>(json["Config"]["FiltersBuy"].ToString()),
                    TargetPointBuy = (string)json["Config"]["TargetPointBuy"]
                };
                */
                //backup.Config = config;
            }


            return backup;
            

        }

        /// <summary>
        /// Метод для сохранения объекта Бекап в файл.
        /// </summary>
        public static void Save(ThreadBackup _backup, int _tid = -1)
        {
            int tid = _tid; // Если не передали то он будет -1
            if (tid == -1)
                tid = TID.CurrentID;

            try
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {


                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(_backup, Formatting.Indented);

                    if (json.IsValidJson())
                    {
                        StreamWriter file = new StreamWriter($"Temp/Threads/{tid}.thread");
                        file.WriteLine(json);
                        file.Close();
                    }
                    else
                    {
                        throw new Exception("code 20");
                    }
                });
            } catch (Exception ex)
            {
                var vv = ex; // TODO INDICATORS eror OR delete despatcher И ВЫяснить в чем дело почему не сохраняет кофиг
            }
        }

        /// <summary>
        /// Метод для удаления бекапа
        /// </summary>
        public static void Delete(int _tid = -1)
        {
            int tid = _tid; // Если не передали то он будет -1
            if (tid == -1)
                tid = TID.CurrentID;

            File.Delete($"Temp/Threads/{tid}.thread");
        }
    }
}
