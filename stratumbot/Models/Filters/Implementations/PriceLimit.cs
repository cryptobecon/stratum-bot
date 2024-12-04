using Newtonsoft.Json;
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
    class PriceLimit : IFilter
    {
        [JsonProperty("ID")]
        public string ID { get; set; } = "4";
        public string Name { get; set; } = "Price Limit";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
        //public List<string> RequiredData { get; set; } = new List<string> { "CurrentPrice" };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        [JsonProperty("Mode")]
        public int Mode = 0; // 0 - More; 1 - Less
        [JsonProperty("PriceLimitValue")]
        public decimal PriceLimitValue = 0; // Лимит по цене
        public int Duration { get; set; } = 0; // Продолжительность действия

        public decimal AllowedTime = 0; // Разрешенное время = Время последнего обновления + Duration

        public string DepthSide = "Bid"; // Стакан, из которого будет браться Current Price

        public string MyName { get; set; } = "Price Limit"; // Отображаемое название 
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
            [JsonProperty("PriceLimitValue")]
            public decimal PriceLimitValue { get; set; }
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
                /*var array = new
                {

                    this.ID,
                    this.Mode,
                    this.PriceLimitValue
                };*/

                var array = new JsonObject()
                {

                    ID = this.ID,
                    Mode = this.Mode,
                    PriceLimitValue = this.PriceLimitValue,
                    Duration = this.Duration,
                    DepthSide = this.DepthSide
                };

                string json = JsonConvert.SerializeObject(array);

                return json;
            }
        }

        // Конструктор
        public PriceLimit(int moreOrLess, decimal priceValue, int duration, string depthSide)
        {
            this.RequiredDataInit();

            this.Mode = moreOrLess;
            this.PriceLimitValue = priceValue;
            this.Duration = duration;
            this.DepthSide = depthSide;
            this.Result = false;
        }

        /// <summary>
        /// Add required data type for the filter to the list
        /// </summary>
        public void RequiredDataInit()
        {
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

            if (this.Mode == 0) // price>limit
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {currentPrice} > {this.PriceLimitValue}");
                if (currentPrice > this.PriceLimitValue)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (this.Mode == 1) // price<limit
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {currentPrice} < {this.PriceLimitValue}");
                if (currentPrice < this.PriceLimitValue)
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
