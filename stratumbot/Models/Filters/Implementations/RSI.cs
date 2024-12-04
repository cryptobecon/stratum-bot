using Newtonsoft.Json;
using Skender.Stock.Indicators;
using stratumbot.Interfaces;
using stratumbot.Models.Filters.DataProvider;
using stratumbot.Models.Indicators;
using stratumbot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Filters
{
    class RSI : IFilter
    {
        public string ID { get; set; } = "11";
        public string Name { get; set; } = "RSI";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
        //public List<string> RequiredData { get; set; } = new List<string> { "Candles" };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        public int Mode = 0; // 0 - Less(rsi<value); 1 -  More(rsi>value)
        public decimal PriceValue = 0; // Индекс цены
        public int Period { get; set; } = 0; // Период
        public string TimeFrame { get; set; } = "5m"; // Тайм-фрэйм
        public int Duration { get; set; } = 0; // Продолжительность действия

        public decimal AllowedTime = 0; // Разрешенное время = Время последнего обновления + Duration

        public string MyName { get; set; } = "RSI"; // Отображаемое название 
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
            [JsonProperty("Mode")]
            public int Mode { get; set; }
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
                    Mode = this.Mode,
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
        public RSI(int moreOrLess, decimal priceValue, int period, string timeFrame, int duration)
        {
            this.RequiredDataInit();

            this.Mode = moreOrLess;
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
                    Cur1 = cur1,
                    Cur2 = cur2,
                    TimeFrame = this.TimeFrame,
                    Period = this.Period + 100
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

            //var rsi = new RSIi();
            //var data = rsi.Result(this.DataMiner.Candles[this.TimeFrame], this.Period);

            IEnumerable<RsiResult> results = this.DataProvider.Quotes.GetRsi(this.Period);

            //Logs.Logger.Info(results.Last().Rsi.ToString());

            if (Mode == 0) // "rsi<value"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().Rsi} < {this.PriceValue}");
                if (results.Last().Rsi < this.PriceValue)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 1) // "rsi>value"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().Rsi} > {this.PriceValue}");
                if (results.Last().Rsi > this.PriceValue)
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
