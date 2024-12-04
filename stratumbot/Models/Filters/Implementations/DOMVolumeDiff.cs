using Newtonsoft.Json;
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
    class DOMVolumeDiff : IFilter
    {
        public string ID { get; set; } = "5";
        public string Name { get; set; } = "DOM Volume Diff";

        // Необходимые типы данных
        public List<string> RequiredData { get; set; } = new List<string> { "DOM" };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        public int Mode = 0; // 0 - <=; 1 - =>
        public int Side = 0; // 0 - Bids; 1 - Asks
        public int Period = 0; // Длинна стакана
        public decimal VolumeValue = 0; // Значение на которое объем должен отличаться
        public int Duration { get; set; } = 0; // Продолжительность действия

        public decimal AllowedTime = 0; // Разрешенное время = Время последнего обновления + Duration

        public string FilterSide { get; set; } // Тип фильтра BUY / SELL

        // Объект JSON
        class JsonObject
        {
            [JsonProperty("ID")]
            public string ID { get; set; }
            [JsonProperty("Mode")]
            public int Mode { get; set; }
            [JsonProperty("Side")]
            public int Side { get; set; }
            [JsonProperty("Period")]
            public int Period { get; set; }
            [JsonProperty("VolumeValue")]
            public decimal VolumeValue { get; set; }
            //public string TimeFrame { get; set; } // TODO зачем здесь таймфрэйм?
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
                    Side = this.Side,
                    Period = this.Period,
                    VolumeValue = this.VolumeValue,
                    Duration = this.Duration
                };

                string json = JsonConvert.SerializeObject(array);

                return json;
            }
        }

        // Конструктор
        public DOMVolumeDiff(int moreOrLess, int side, decimal volumeValue, int period, int duration)
        {
            this.RequiredDataInit();

            this.Mode = moreOrLess;
            this.Side = side;
            this.VolumeValue = volumeValue;
            this.Period = period;
            this.Duration = duration;
            this.Result = false;
        }

        /// <summary>
        /// Add required data type for the filter to the list
        /// </summary>
        public void RequiredDataInit()
        {
            this.RequiredDataTypes.Add(DataType.DOM);
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
            if (dataType == DataType.DOM)
            {
                return new DataOptions
                {
                    Cur1 = cur1,
                    Cur2 = cur2,
                    Period = this.Period // TODO period for dom?????
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



            var domVolDiff = new DOMVolDiff();
            var domDiff = domVolDiff.Result(this.DataProvider.DOM, this.Period);

            if (this.Mode == 0) // x <= y
            {
                if(this.Side == 0) // x = Bids
                {
                    Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{this.Side}/ ?: {domDiff["bidsVolumePercent"]} <= {this.VolumeValue}");
                    if (domDiff["bidsVolumePercent"] <= this.VolumeValue)
                    {
                        this.Result = true;
                        this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                    }
                    else
                        this.Result = false;
                }
                if (this.Side == 1) // x = Asks
                {
                    Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{this.Side}/ ?: {domDiff["asksVolumePercent"]} <= {this.VolumeValue}");
                    if (domDiff["asksVolumePercent"] <= this.VolumeValue)
                    {
                        this.Result = true;
                        this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                    }
                    else
                        this.Result = false;
                }
            }
            if (this.Mode == 1) // x >= y
            {
                if (this.Side == 0) // x = Bids
                {
                    Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{this.Side}/ ?: {domDiff["bidsVolumePercent"]} >= {this.VolumeValue}");
                    if (domDiff["bidsVolumePercent"] >= this.VolumeValue)
                    {
                        this.Result = true;
                        this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                    }
                    else
                        this.Result = false;
                }
                if (this.Side == 1) // x = Asks
                {
                    Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/{this.Side}/ ?: {domDiff["asksVolumePercent"]} >= {this.VolumeValue}");
                    if (domDiff["asksVolumePercent"] >= this.VolumeValue)
                    {
                        this.Result = true;
                        this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                    }
                    else
                        this.Result = false;
                }
            }
        }

        #region 

        // Стандатртные настройки
        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        public string MyName { get; set; } = "Price Limit"; // Отображаемое название 
        public string Group { get; set; } // Группа
        public int Weight { get; set; } = 0; // Балл
        public System.Windows.Media.Brush Color { get; set; } // Цвет

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

        #endregion
    }
}
