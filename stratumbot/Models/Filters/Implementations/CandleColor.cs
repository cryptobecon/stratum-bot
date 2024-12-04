using Newtonsoft.Json;
using stratumbot.Interfaces;
using stratumbot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using stratumbot.DTO;
using stratumbot.Models.Filters.DataProvider;

namespace stratumbot.Models.Filters
{
    class CandleColor : IFilter
    {
        public string ID { get; set; } = "22";
        public string Name { get; set; } = "Candle Color";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
        //public List<string> RequiredData { get; set; } = new List<string> { "Klines" };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        public int Mode = 0; // "=" // Операция = !=
        public int Side = 0; // "green" // green or red
        public int Period { get; set; } = 0; // Период
        public string TimeFrame { get; set; } = "5m"; // Тайм-фрэйм
        public int Duration { get; set; } = 0; // Продолжительность действия

        public decimal AllowedTime = 0; // Разрешенное время = Время последнего обновления + Duration

        public string DepthSide = "Bid"; // Стакан, из которого будет браться Current Price

        public string MyName { get; set; } = "Candle Color"; // Отображаемое название 
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
            [JsonProperty("Period")]
            public int Period { get; set; }
            [JsonProperty("TimeFrame")]
            public string TimeFrame { get; set; }
            [JsonProperty("Side")]
            public int Side { get; set; }
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
                    Period = this.Period,
                    TimeFrame = this.TimeFrame,
                    Side = this.Side,
                    Duration = this.Duration
                };

                string json = JsonConvert.SerializeObject(array);

                return json;
            }
        }

        // Конструктор
        public CandleColor(int mode, int side, int period, string timeFrame, int duration)
        {
            this.RequiredDataInit();

            this.Mode = mode;
            this.Period = period;
            this.TimeFrame = timeFrame;
            this.Side = side;
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
                    Period = this.Period + 1
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

            string needColor = (this.Side == 0) ? "green" : "red";

            // https://api.binance.com/api/v3/klines?symbol=BTCUSDT&interval=1m&limit=1


            if (Mode == 0 && this.Period == 0) // =
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{this.Side}/ ?: {GetCandleColor(this.DataProvider.Quotes.Last())} == {needColor}");
                if (GetCandleColor(this.DataProvider.Quotes.Last()) == needColor)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }

            if (Mode == 1 && this.Period == 0) // !=
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{this.Side}/ ?: {GetCandleColor(this.DataProvider.Quotes.Last())} != {needColor}");
                if (GetCandleColor(this.DataProvider.Quotes.Last()) != needColor)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }

            int countOfNeedColor = 0;

            for (int i = 1; i <= this.Period; i++)
            {
                if (GetCandleColor(this.DataProvider.Quotes.ToArray()[this.DataProvider.Quotes.Count() - i - 1]) == needColor)
                {
                    countOfNeedColor++;
                }
            }

            if (Mode == 0)
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{this.Side}/ ?: {countOfNeedColor} == {this.Period}");
                if (countOfNeedColor == this.Period) // Количество свечей нужного цвета = периоду
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }

            if (Mode == 1)
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{this.Side}/ ?: {countOfNeedColor} != {this.Period}");
                if (countOfNeedColor != this.Period) // Количество свечей нужного цвета = периоду
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
        }

        private string GetCandleColor(Quote candle)
        {
            if (candle.Open < candle.Close)
                return "green";
            else
                return "red";
        }
    }
}
