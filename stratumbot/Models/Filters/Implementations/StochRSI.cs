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
    class StochRSI : IFilter
    {
        public string ID { get; set; } = "13";
        public string Name { get; set; } = "Stoch RSI";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
        //public List<string> RequiredData { get; set; } = new List<string> { "Candles" };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        public int Mode = 0;
        public decimal PriceValue = 0; // Индекс цены

        /// <summary>
        /// "K" - smoothPeriods (M)
        /// </summary>
        public int Period { get; set; } = 0;

        /// <summary>
        /// "D" - signalPeriods (G)
        /// </summary>
        public int Period2 { get; set; } = 0;

        /// <summary>
        /// "RSI Lenght" - rsiPeriods (R)
        /// </summary>
        public int Period4 { get; set; } = 0;

        /// <summary>
        /// "Stoch Length" - stochPeriods (S)
        /// </summary>
        public int Period3 { get; set; } = 0; // 

        public string TimeFrame { get; set; } = "5m"; // Тайм-фрэйм
        public int Duration { get; set; } = 0; // Продолжительность действия

        public decimal AllowedTime = 0; // Разрешенное время = Время последнего обновления + Duration

        public string MyName { get; set; } = "Stoch RSI"; // Отображаемое название 
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
            [JsonProperty("Period2")]
            public int Period2 { get; set; }
            [JsonProperty("Period3")]
            public int Period3 { get; set; }
            [JsonProperty("Period4")]
            public int Period4 { get; set; }
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
                    Period2 = this.Period2,
                    Period3 = this.Period3,
                    Period4 = this.Period4,
                    TimeFrame = this.TimeFrame,
                    Duration = this.Duration
                };

                string json = JsonConvert.SerializeObject(array);

                return json;
            }
        }

        // Конструктор
        public StochRSI(int moreOrLess, decimal priceValue, int period, int period2, int period3, int period4, string timeFrame, int duration)
        {
            this.RequiredDataInit();

            this.Mode = moreOrLess;
            this.PriceValue = priceValue;
            this.Period = period;
            this.Period2 = period2;
            this.Period3 = period3;
            this.Period4 = period4;
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
                    Period = Math.Max((this.Period4 + this.Period2 + this.Period + this.Period3), this.Period4 + 100)
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

            //var stoch = new StochRSIi();
            //var data = stoch.Result(this.DataMiner.Candles[this.TimeFrame], this.Period, this.Period2, this.Period3);

            // 
            IEnumerable<StochRsiResult> results = this.DataProvider.Quotes.GetStochRsi
                (this.Period4, this.Period3, this.Period2, this.Period);

            //Logs.Logger.Info("%K Oscillator: " + results.Last().StochRsi.ToString());
            //Logs.Logger.Info("%D Signal: " + results.Last().Signal.ToString());


            if (Mode == 0) // "K% и D% <= value"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().StochRsi} <= {this.PriceValue} && {results.Last().Signal} <= {this.PriceValue}");
                if (results.Last().StochRsi <= this.PriceValue && results.Last().Signal <= this.PriceValue)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 1) // "K% и D% >= value"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().StochRsi} >= {this.PriceValue} && {results.Last().Signal} >= {this.PriceValue}");
                if (results.Last().StochRsi >= this.PriceValue && results.Last().Signal >= this.PriceValue)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 2) // "K% < D%"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().StochRsi} < {results.Last().Signal}");
                if (results.Last().StochRsi < results.Last().Signal)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 3) // "K% >= D%"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().StochRsi} >= {results.Last().Signal}");
                if (results.Last().StochRsi >= results.Last().Signal)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 4) // "K% <= value"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().StochRsi} <= {this.PriceValue}");
                if (results.Last().StochRsi <= this.PriceValue)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 5) // "D% <= value"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().Signal} <= {this.PriceValue}");
                if (results.Last().Signal <= this.PriceValue)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 6) // "K% >= value"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().StochRsi} >= {this.PriceValue}");
                if (results.Last().StochRsi >= this.PriceValue)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 7) // "D% > value"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().Signal} > {this.PriceValue}");
                if (results.Last().Signal > this.PriceValue)
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
