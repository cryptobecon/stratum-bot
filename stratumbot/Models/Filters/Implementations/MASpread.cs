using Newtonsoft.Json;
using Skender.Stock.Indicators;
using stratumbot.Core;
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
    class MASpread : IFilter
    {

        public string ID { get; set; } = "17";
        public string Name { get; set; } = "MA Spread";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
        //public List<string> RequiredData { get; set; } = new List<string> { "Candles" };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        public int Mode { get; set; } = 0; // spread>diff, spread<diff
        public decimal Diff { get; set; } = 0;
        public int Line1 { get; set; } = 0; // SMA, EMA
        public int Period { get; set; } = 0; // Период
        public int Line2 { get; set; } = 0; // SMA, EMA
        public int Period2 { get; set; } = 0; // Период 2
        public string TimeFrame { get; set; } = "5m"; // Тайм-фрэйм
        public int Duration { get; set; } = 0; // Продолжительность действия

        public decimal AllowedTime = 0; // Разрешенное время = Время последнего обновления + Duration

        public string MyName { get; set; } = "MA Spread"; // Отображаемое название 
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

        class JsonObject
        {
            [JsonProperty("ID")]
            public string ID { get; set; }
            [JsonProperty("Mode")]
            public int Mode { get; set; }
            [JsonProperty("Diff")]
            public decimal Diff { get; set; }
            [JsonProperty("Line1")]
            public int Line1 { get; set; }
            [JsonProperty("Period")]
            public int Period { get; set; }
            [JsonProperty("Line2")]
            public int Line2 { get; set; }
            [JsonProperty("Period2")]
            public int Period2 { get; set; }
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
                    Line1 = this.Line1,
                    Period = this.Period,
                    Line2 = this.Line2,
                    Period2 = this.Period2,
                    TimeFrame = this.TimeFrame,
                    Diff = this.Diff,
                    Duration = this.Duration
                };

                string json = JsonConvert.SerializeObject(array);

                return json;
            }
        }

        // Конструктор
        public MASpread(int mode, decimal diff, int line1, int period, int line2, int period2, string timeFrame, int duration)
        {
            this.RequiredDataInit();

            this.Mode = mode;
            this.Diff = diff;
            this.Line1 = line1;
            this.Period = period;
            this.Line2 = line2;
            this.Period2 = period2;
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

            decimal? line1Price = 0;
            decimal? line2Price = 0;

            if (this.Line1 == 0) // Line1 = SMA
            {
                //var sma = new SMA();
                //var data = sma.Result(this.DataMiner.Candles[this.TimeFrame], this.Period);
                IEnumerable<SmaResult> results = this.DataProvider.Quotes.GetSma(this.Period);

                line1Price = results.Last().Sma;
            }
            else // Line1 = EMA
            {
                //var ema = new EMA();
                //var data = ema.Result(this.DataMiner.Candles[this.TimeFrame], this.Period);
                IEnumerable<EmaResult> results = this.DataProvider.Quotes.GetEma(this.Period);

                line1Price = results.Last().Ema;
            }

            if (this.Line2 == 0) // Line2 = SMA
            {
                //var sma = new SMA();
                //var data = sma.Result(this.DataMiner.Candles[this.TimeFrame], this.Period2);
                IEnumerable<SmaResult> results = this.DataProvider.Quotes.GetSma(this.Period);
                line2Price = results.Last().Sma;
            }
            else // Line2 = EMA
            {
                var ema = new EMA();
                IEnumerable<EmaResult> results = this.DataProvider.Quotes.GetEma(this.Period2);
                line2Price = results.Last().Ema;
            }

            decimal? spread = 0;
            decimal spreadPercent = 0;
            if (line1Price > line2Price)
            {
                spread = line1Price - line2Price;
                spreadPercent = Calc.PercentOfAmount((decimal)spread, (decimal)line1Price);
            } else
            {
                spread = line2Price - line1Price;
                spreadPercent = Calc.PercentOfAmount((decimal)spread, (decimal)line2Price);
            }
                
            

            if (Mode == 0) // "spread>diff"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {spreadPercent} > {this.Diff}");
                if (spreadPercent > this.Diff)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 1) // "spread<diff"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {spreadPercent} < {this.Diff}");
                if (spreadPercent < this.Diff)
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
