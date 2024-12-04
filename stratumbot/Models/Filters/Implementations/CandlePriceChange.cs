using Newtonsoft.Json;
using stratumbot.Core;
using stratumbot.Interfaces;
using stratumbot.Models.Filters.DataProvider;
using stratumbot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Filters
{
    class CandlePriceChange : IFilter
    {
        public string ID { get; set; } = "18";
        public string Name { get; set; } = "Candle Price Change";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
        //public List<string> RequiredData { get; set; } = new List<string> { "Candles" };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        public string Cur1 { get; set; }
        public string Cur2 { get; set; }
        public int Mode = 0; // 0 - More; 1 - Less
        public int Side = 0; // 0 - up; 1 - down
        public decimal PriceValue = 0; // На сколько изменилась цена (%)
        public int Period = 0; // 2 всегда т.к. мы будем сравнивать текущий и предпоследний
        public string TimeFrame { get; set; } = "5m"; // Тайм-фрэйм
        public int Duration { get; set; } = 0; // Продолжительность действия

        public decimal AllowedTime = 0; // Разрешенное время = Время последнего обновления + Duration

        public string MyName { get; set; } = "Candle Price Change"; // Отображаемое название 
        public string Group { get; set; } // Группа
        public int Weight { get; set; } = 0; // Балл
        public System.Windows.Media.Brush Color { get; set; } // Цвет

        public string FilterSide { get; set; } // Тип фильтра BUY / SELL

        bool result;
        public bool Result // Результат по фильтру 
        {
            get
            {
                this.Compute();
                return result;
            }
            set
            {
                result = value;
                this.CurrentWeight = (value) ? this.Weight : 0;
            }
        }

        // Получить (а предварительно рассчитать) кол-во баллов по фильтру
        int currentWeight;
        public int CurrentWeight
        {
            get
            {
                this.Compute();
                return currentWeight;
            }
            set { currentWeight = value; }
        }

        // Объект JSON
        class JsonObject
        {
            [JsonProperty("ID")]
            public string ID { get; set; }
            [JsonProperty("Cur1")]
            public string Cur1 { get; set; }
            [JsonProperty("Cur2")]
            public string Cur2 { get; set; }
            [JsonProperty("Mode")]
            public int Mode { get; set; }
            [JsonProperty("Side")]
            public int Side { get; set; }
            [JsonProperty("PriceValue")]
            public decimal PriceValue { get; set; }
            [JsonProperty("Period")]
            public int Period { get; set; }
            [JsonProperty("TimeFrame")]
            public string TimeFrame { get; set; }
            [JsonProperty("Duration")]
            public int Duration { get; set; }
        }

        // JSON настроек
        public string Json
        {
            get
            {
                var array = new JsonObject()
                {
                    ID = this.ID,
                    Cur1 = this.Cur1,
                    Cur2 = this.Cur2,
                    Mode = this.Mode,
                    Side = this.Side,
                    PriceValue = this.PriceValue,
                    Period = this.Period,
                    TimeFrame = this.TimeFrame,
                    Duration = this.Duration
                };

                string json = JsonConvert.SerializeObject(array);

                return json;
            }
        }

        // Конструктор
        public CandlePriceChange(string cur1, string cur2, int mode, int side, decimal priceValue, int period, string timeFrame, int duration)
        {
            this.RequiredDataInit();

            this.Cur1 = cur1;
            this.Cur2 = cur2;
            this.Mode = mode;
            this.Side = side;
            this.PriceValue = priceValue;
            this.Period = period;
            this.TimeFrame = timeFrame;
            this.Duration = duration;
            this.Result = false;
        }

        /// <summary>
        /// Add required data type for the filter to the list
        /// </summary>
        public void RequiredDataInit()
        {
            this.RequiredDataTypes.Add(DataType.Quotes);
        }


        /// <summary>
        /// Get options for specefic data type for the filter
        /// </summary>
        /// <param name="dataType">Data type of which options are needly</param>
        /// <param name="cur1">First currency</param>
        /// <param name="cur2">Second (base) currency</param>
        /// <returns>(DataOptions) options for specific data type</returns>
        public DataOptions GetOptions(DataType dataType, string cur1 = null, string cur2 = null)
        {
            if (dataType == DataType.Quotes)
            {
                return new DataOptions
                {
                    Cur1 = this.Cur1,
                    Cur2 = this.Cur2,
                    TimeFrame = this.TimeFrame,
                    Period = this.Period // TODO maybe here i can set period *3 if i need
                };
            }

            throw new Exception("GetOptions() DataType doesn't sent");
        }

        public void Compute()
        {
            // Продолжительность действия
            if (this.Duration != 0 && Time.CurrentSeconds() < this.AllowedTime)
            {
                this.Result = true;
                return;
            }

            decimal priceChangePercent = 0;

            // получить свечи
            // сравнить рост/падение текущей свечи относительно последней
            //var candles = this.DataMiner.Candles[this.TimeFrame];
            var candles = this.DataProvider.Quotes.Reverse().ToArray();
            if (candles[1].Close > candles[0].Close) // prev > last = падение
            {
                priceChangePercent = 100 - Calc.PercentOfAmount((decimal)candles[1].Close, (decimal)candles[0].Close);
            }
            else // prev < last = рост
            {
                priceChangePercent = Calc.PercentOfAmount((decimal)candles[0].Close, (decimal)candles[1].Close) - 100;
            }

            if (this.Mode == 0) // (changed) More
            {
                // UP
                if (Side == 0)
                {
                    if (priceChangePercent > 0)
                    {
                        Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{Side}/ ?: {priceChangePercent} > {this.PriceValue}");
                        if (priceChangePercent > this.PriceValue)
                        {
                            this.Result = true;
                            this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                        }
                        else
                            this.Result = false;
                    }
                }

                // DOWN
                if (this.Side == 1)
                {
                    if (priceChangePercent < 0)
                    {
                        Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{Side}/ ?: {priceChangePercent} < {this.PriceValue}");
                        if (priceChangePercent < this.PriceValue)
                        {
                            this.Result = true;
                            this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                        }
                        else
                            this.Result = false;
                    }
                }
            }
            if (this.Mode == 1) // (changed) Less
            {
                // UP
                if (Side == 0)
                {
                    if (priceChangePercent > 0)
                    {
                        Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{Side}/ ?: {priceChangePercent} < {this.PriceValue}");
                        if (priceChangePercent < this.PriceValue)
                        {
                            this.Result = true;
                            this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                        }
                        else
                            this.Result = false;
                    }
                }

                // DOWN
                if (this.Side == 1)
                {
                    if (priceChangePercent < 0)
                    {
                        Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{Side}/ ?: {priceChangePercent} > {this.PriceValue}");
                        if (priceChangePercent > this.PriceValue)
                        {
                            this.Result = true;
                            this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                        }
                        else
                            this.Result = false;
                    }
                }
            }
        }
    }
}
