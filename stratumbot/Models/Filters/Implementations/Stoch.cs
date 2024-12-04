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
    class Stoch : IFilter
    {
        public string ID { get; set; } = "12";
        public string Name { get; set; } = "Stoch";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
        //public List<string> RequiredData { get; set; } = new List<string> { "Klines" };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        public int Mode = 0; 
        public decimal PriceValue = 0; // Индекс цены

        /// <summary>
        /// K - lookbackPeriods
        /// </summary>
        public int Period { get; set; } = 0; // Период 1 %K Lenght

        /// <summary>
        /// D smoothing - signalPeriods
        /// </summary>
        public int Period2 { get; set; } = 0; // Период 2 %K Smoothing

        /// <summary>
        /// K smoothing - smoothPeriods - S
        /// </summary>
        public int Period3 { get; set; } = 0; // Период 3 %D Smoothing // Smooth (Сглаживание/Замедление)

        public string TimeFrame { get; set; } = "5m"; // Тайм-фрэйм
        public int Duration { get; set; } = 0; // Продолжительность действия

        public decimal AllowedTime = 0; // Разрешенное время = Время последнего обновления + Duration

        public string MyName { get; set; } = "Stoch"; // Отображаемое название 
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
                    TimeFrame = this.TimeFrame,
                    Duration = this.Duration
                };

                string json = JsonConvert.SerializeObject(array);

                return json;
            }
        }

        // Конструктор
        public Stoch(int moreOrLess, decimal priceValue, int period, int period2, int period3, string timeFrame, int duration)
        {
            this.RequiredDataInit();

            this.Mode = moreOrLess;
            this.PriceValue = priceValue;
            this.Period = period;
            this.Period2 = period2;
            this.Period3 = period3;
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
                    Period = this.Period + this.Period2
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

            //var stoch = new Stochi();
            //var data = stoch.Result(this.DataMiner.Klines[this.TimeFrame], this.Period, this.Period2, this.Period3);

            IEnumerable<StochResult> results = this.DataProvider.Quotes.GetStoch(this.Period, this.Period2, this.Period3);

            //Logs.Logger.Info("%K: " + results.Last().Oscillator.ToString());
            //Logs.Logger.Info("%D: " + results.Last().Signal.ToString());
            //Logs.Logger.Info("%J: " + results.Last().PercentJ.ToString());


            if (Mode == 0) // "K% и D% <= value"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().Oscillator} <= {this.PriceValue} && {results.Last().Signal} <= {this.PriceValue}");
                if (results.Last().Oscillator <= this.PriceValue && results.Last().Signal <= this.PriceValue)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 1) // "K% и D% >= value"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().Oscillator} >= {this.PriceValue} && {results.Last().Signal} >= {this.PriceValue}");
                if (results.Last().Oscillator >= this.PriceValue && results.Last().Signal >= this.PriceValue)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 2) // "K% < D%"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().Oscillator} < {results.Last().Signal}");
                if (results.Last().Oscillator < results.Last().Signal)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 3) // "K% >= D%"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().Oscillator} >= {results.Last().Signal}");
                if (results.Last().Oscillator >= results.Last().Signal)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 4) // "K% <= value"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().Oscillator} <= {this.PriceValue}");
                if (results.Last().Oscillator <= this.PriceValue)
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
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {results.Last().Oscillator} >= {this.PriceValue}");
                if (results.Last().Oscillator >= this.PriceValue)
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
