using Newtonsoft.Json;
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
    class OHLCPlusLimit : IFilter
    {
        public string ID { get; set; } = "8";
        public string Name { get; set; } = "OHLC+ Limit";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
        //public List<string> RequiredData { get; set; } = new List<string> { "CurrentPrice", "Klines" };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        public int Mode = 0; // "price<ohlc+" // Higher or Lower
        public int Source = 0; // o // price source : o, h, l, c, hl2, hlc3, ohlc4
        public int PriceType = 0; // Max // price type : max min avg
        public int Period { get; set; } = 0; // Период
        public string TimeFrame { get; set; } = "5m"; // Тайм-фрэйм
        public decimal Indent { get; set; } = 0; // Отступ
        public int Duration { get; set; } = 0; // Продолжительность действия

        public decimal AllowedTime = 0; // Разрешенное время = Время последнего обновления + Duration

        public string DepthSide = "Bid"; // Стакан, из которого будет браться Current Price

        public string MyName { get; set; } = "OHLC+ Limit"; // Отображаемое название 
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
            [JsonProperty("Source")]
            public int Source { get; set; }
            [JsonProperty("PriceType")]
            public int PriceType { get; set; }
            [JsonProperty("Period")]
            public int Period { get; set; }
            [JsonProperty("TimeFrame")]
            public string TimeFrame { get; set; }
            [JsonProperty("Indent")]
            public decimal Indent { get; set; }
            [JsonProperty("Duration")]
            public int Duration { get; set; }
            [JsonProperty("DepthSide")]
            public string DepthSide { get; set; }
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
                    Source = this.Source,
                    PriceType = this.PriceType,
                    Period = this.Period,
                    TimeFrame = this.TimeFrame,
                    Indent = this.Indent,
                    Duration = this.Duration,
                    DepthSide = this.DepthSide
                };

                string json = JsonConvert.SerializeObject(array);

                return json;
            }
        }

        // Конструктор
        public OHLCPlusLimit(int higherOrlower, int source, int priceType,  int period, string timeFrame, decimal indent, int duration, string depthSide)
        {
            this.RequiredDataInit();

            this.Mode = higherOrlower;
            this.Source = source;
            this.PriceType = priceType;
            this.Period = period;
            this.TimeFrame = timeFrame;
            this.Indent = indent;
            this.Duration = duration;
            this.DepthSide = depthSide;
            this.Result = false;
        }

        /// <summary>
        /// Add required data type for the filter to the list
        /// </summary>
        public void RequiredDataInit()
        {
            this.RequiredDataTypes.Add(DataType.Quotes);
            this.RequiredDataTypes.Add(DataType.CurrentPrice);
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
            if (dataType == DataType.CurrentPrice)
            {
                return new DataOptions
                {
                    Cur1 = cur1,
                    Cur2 = cur2
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

            // Filter Side
            decimal currentPrice = (this.DepthSide == "Bid") ? this.DataProvider.CurrentBuyPrice : this.DataProvider.CurrentSellPrice;

            var ohlcPlus = new OHLCPlus();
            var data = ohlcPlus.Result(this.DataProvider.Quotes, this.Period);

            decimal limitPrice = 0;

            if (PriceType == 0) // Max
            {
                if (Source == 0) // Open
                    limitPrice = data.OpenMax;
                if (Source == 1) // High
                    limitPrice = data.HighMax;
                if (Source == 2) // Low
                    limitPrice = data.LowMax;
                if (Source == 3) // Close
                    limitPrice = data.CloseMax;
                if (Source == 4) // HL/2
                    limitPrice = data.Hl2Max;
                if (Source == 5) // HLC/3
                    limitPrice = data.Hlc3Max;
                if (Source == 6) // OHLC/4
                    limitPrice = data.Ohlc4Max;
            }
            if (PriceType == 1) // Min
            {
                if (Source == 0) // Open
                    limitPrice = data.OpenMin;
                if (Source == 1) // High
                    limitPrice = data.HighMin;
                if (Source == 2) // Low
                    limitPrice = data.LowMin;
                if (Source == 3) // Close
                    limitPrice = data.CloseMin;
                if (Source == 4) // HL/2
                    limitPrice = data.Hl2Min;
                if (Source == 5) // HLC/3
                    limitPrice = data.Hlc3Min;
                if (Source == 6) // OHLC/4
                    limitPrice = data.Ohlc4Min;
            }
            if (PriceType == 2) // Avg
            {
                if (Source == 0) // Open
                    limitPrice = data.OpenAvg;
                if (Source == 1) // High
                    limitPrice = data.HighAvg;
                if (Source == 2) // Low
                    limitPrice = data.LowAvg;
                if (Source == 3) // Close
                    limitPrice = data.CloseAvg;
                if (Source == 4) // HL/2
                    limitPrice = data.Hl2Avg;
                if (Source == 5) // HLC/3
                    limitPrice = data.Hlc3Avg;
                if (Source == 6) // OHLC/4
                    limitPrice = data.Ohlc4Avg;
            }

            // Высчитываем отступ
            decimal currentPoint = limitPrice;
            currentPoint = limitPrice + Calc.AmountOfPercent(this.Indent, currentPoint);


            if (Mode == 0) // "price<ohlc+"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{Source}/ ?: {currentPrice} < {currentPoint}");
                if (currentPrice < currentPoint)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 1) // "price>ohlc+"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{Source}/ ?: {currentPrice} > {currentPoint}");
                if (currentPrice > currentPoint)
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
