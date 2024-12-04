using Newtonsoft.Json;
using Skender.Stock.Indicators;
using stratumbot.Core;
using stratumbot.Interfaces;
using stratumbot.Models.Filters.DataProvider;
using stratumbot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Filters.Implementations
{
    class BollingerBandsWidth : IFilter
    {

        public string ID { get; set; } = "24";
        public string Name { get; set; } = "Bollinger Bands Width";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
        //public List<string> RequiredData { get; set; } = new List<string> { "CurrentPrice", "Klines" };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        public int Mode = 0; // BBW>value BBW<value // Higher or Lower
        public int Period { get; set; } = 0; // Период
        public string TimeFrame { get; set; } = "5m"; // Тайм-фрэйм
        public decimal Rate { get; set; } // Коэфицент — количество стандартных отклонений
   
        public decimal PriceValue { get; set; } = 0; 

        //public decimal Indent { get; set; } = 0; // Отступ
        public int Duration { get; set; } = 0; // Продолжительность действия

        public decimal AllowedTime = 0; // Разрешенное время = Время последнего обновления + Duration

        public string DepthSide = "Bid"; // Стакан, из которого будет браться Current Price

        public string MyName { get; set; } = "Bollinger Bands Width"; // Отображаемое название 
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
            [JsonProperty("Rate")]
            public decimal Rate { get; set; }
            [JsonProperty("TimeFrame")]
            public string TimeFrame { get; set; }
            [JsonProperty("PriceValue")]
            public decimal PriceValue { get; set; }
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
                    Rate = this.Rate,
                    TimeFrame = this.TimeFrame,
                    PriceValue = this.PriceValue,
                    Duration = this.Duration
                };

                string json = JsonConvert.SerializeObject(array);

                return json;
            }
        }

        // Конструктор
        public BollingerBandsWidth(int higherOrlower, int period, decimal rate, string timeFrame, decimal priceValue, int duration)
        {
            this.RequiredDataInit();

            this.Mode = higherOrlower;
            this.Period = period;
            this.Rate = rate;
            this.TimeFrame = timeFrame;
            this.PriceValue = priceValue;
            this.Duration = duration;
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
                    Period = this.Period
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

            //Logs.Logger.Error("period:" + this.Period.ToString());
            //Logs.Logger.Error("rate:" + this.Rate.ToString());
            //Logs.Logger.Error("dataproviderisnull:" + (this.DataProvider == null).ToString() );
            //Logs.Logger.Error("Quotesisnull:" + (this.DataProvider.Quotes == null).ToString() );

            IEnumerable<BollingerBandsResult> results =
                this.DataProvider.Quotes.GetBollingerBands(this.Period, this.Rate);



            // Computing current BBW value
            decimal currentPoint = ((decimal)results.Last().UpperBand - (decimal)results.Last().LowerBand) / (decimal)results.Last().Sma;


            if (Mode == 0) // "BBW>value"
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {this.PriceValue} > {currentPoint}");
                if (currentPoint > this.PriceValue)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 1) // "BBW<value"
            {
                 Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {this.PriceValue} > {currentPoint}");
                if (currentPoint < this.PriceValue)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }

            // TODO delete after debug
            Logs.Logger.ToFile("Upper: " + results.Last().UpperBand.ToString() + "\n" +
                "Midle: " + results.Last().Sma.ToString() + "\n" +
                "Lower: " + results.Last().LowerBand.ToString() + "\n" +
                "currentPoint: " + currentPoint.ToString() + "\n" +
                "PriceValue: " + this.PriceValue.ToString());

        }
    }
}
