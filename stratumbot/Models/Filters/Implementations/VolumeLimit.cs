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
    class VolumeLimit : IFilter
    {
        [JsonProperty("ID")]
        public string ID { get; set; } = "23";
        public string Name { get; set; } = "Volume Limit";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
        //public List<string> RequiredData { get; set; } = new List<string> { "CurrentVolume" };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        public string Cur1 { get; set; }
        public string Cur2 { get; set; }
        public int Mode = 0; // 0 - More; 1 - Less
        public decimal VolumeValue = 0; // Лимит по цене

        public int Duration { get; set; } = 0; // Продолжительность действия

        public decimal AllowedTime = 0; // Разрешенное время = Время последнего обновления + Duration

        public string DepthSide = "Bid"; // Стакан, из которого будет браться Current Price

        public string MyName { get; set; } = "Volume Limit"; // Отображаемое название 
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
            [JsonProperty("Cur1")]
            public string Cur1 { get; set; }
            [JsonProperty("Cur2")]
            public string Cur2 { get; set; }
            [JsonProperty("Mode")]
            public int Mode { get; set; }
            [JsonProperty("VolumeValue")]
            public decimal VolumeValue { get; set; }
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
                    VolumeValue = this.VolumeValue,
                    Duration = this.Duration
                };

                string json = JsonConvert.SerializeObject(array);

                return json;
            }
        }

        // Конструктор
        public VolumeLimit(string cur1, string cur2, int mode, decimal volumeValue, int duration)
        {
            this.RequiredDataInit();

            this.Cur1 = cur1;
            this.Cur2 = cur2;
            this.Mode = mode;
            this.VolumeValue = volumeValue;
            this.Duration = duration;
            this.Result = false;
        }

        /// <summary>
        /// Add required data type for the filter to the list
        /// </summary>
        public void RequiredDataInit()
        {
            this.RequiredDataTypes.Add(DataType.CurrentVolume);
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
            if (dataType == DataType.CurrentVolume)
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

            

            if (this.Mode == 0) // volume>limit
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {this.DataProvider.CurrentVolume} > {this.VolumeValue}");
                if (this.DataProvider.CurrentVolume > this.VolumeValue)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (this.Mode == 1) // volume<limit
            {
                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {this.DataProvider.CurrentVolume} < {this.VolumeValue}");
                if (this.DataProvider.CurrentVolume < this.VolumeValue)
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
