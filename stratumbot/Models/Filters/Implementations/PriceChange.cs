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
    class PriceChange : IFilter
    {
        public string ID { get; set; } = "3";
        public string Name { get; set; } = "Price Change";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
        //public List<string> RequiredData { get; set; } = new List<string> { "PriceChangePercent" };

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
        public int Duration { get; set; } = 0; // Продолжительность действия

        public decimal AllowedTime = 0; // Разрешенное время = Время последнего обновления + Duration

        public string MyName { get; set; } = "Price Change"; // Отображаемое название 
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
                    Duration = this.Duration
                };

                string json = JsonConvert.SerializeObject(array);

                return json;
            }
        }

        // Конструктор
        public PriceChange(string cur1, string cur2, int moreOrLess, int side, decimal priceValue, int duration)
        {
            this.RequiredDataInit();

            this.Cur1 = cur1;
            this.Cur2 = cur2;
            this.Mode = moreOrLess;
            this.Side = side;
            this.PriceValue = priceValue;
            this.Duration = duration;
            this.Result = false;
        }

        /// <summary>
        /// Add required data type for the filter to the list
        /// </summary>
        public void RequiredDataInit()
        {
            this.RequiredDataTypes.Add(DataType.PriceChangePercent);
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
            if (dataType == DataType.PriceChangePercent)
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

            if (this.Mode == 0) // (changed) More
            {
                // UP +
                if(Side == 0)
                {
                   // if (DataMiner.PriceChangePercent[this.Cur1 + this.Cur2] > 0) // Нафига эти проверки тут нужны?
                    {
                        Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{Side}/ ?: {this.DataProvider.PriceChangePercent} > {this.PriceValue}");
                        if (this.DataProvider.PriceChangePercent > this.PriceValue)
                        {
                            this.Result = true;
                            this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                        }
                        else
                            this.Result = false;
                    }
                }
                
                // DOWN -
                if(this.Side == 1)
                {
                    // if (DataMiner.PriceChangePercent[this.Cur1 + this.Cur2] < 0)
                    decimal negativePriceValue = 0 - this.PriceValue;
                    {
                        Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{Side}/ ?: {this.DataProvider.PriceChangePercent} < {this.PriceValue}");
                        if (this.DataProvider.PriceChangePercent > negativePriceValue)
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
                   // if (DataMiner.PriceChangePercent[this.Cur1 + this.Cur2] > 0)
                    {
                        Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{Side}/ ?: {this.DataProvider.PriceChangePercent} < {this.PriceValue}");
                        if (this.DataProvider.PriceChangePercent < this.PriceValue)
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
                    //  if (DataMiner.PriceChangePercent[this.Cur1 + this.Cur2] < 0)
                    decimal negativePriceValue = 0 - this.PriceValue;
                    {
                        Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{Side}/ ?: {this.DataProvider.PriceChangePercent} > {this.PriceValue}");
                        if (this.DataProvider.PriceChangePercent < negativePriceValue)
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
