using Newtonsoft.Json;
using stratumbot.Interfaces;
using stratumbot.Models.Filters.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Filters
{
    // TODO Times broken // It is need for filter Timer as i understood - create some another object to hodl this type information (maybe pu together with buyCounter and other counters, like iteration duration )
    class Timer : IFilter
    {

        public string ID { get; set; } = "21";
        public string Name { get; set; } = "Timer";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
       // public List<string> RequiredData { get; set; } = new List<string> { };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        public int Mode = 0; // Событие
        public int Duration { get; set; } = 0; // Секунд должно пройти с события (обычно Продолжительность действия)

   
        public string MyName { get; set; } = "Timer"; // Отображаемое название 
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
                    Duration = this.Duration
                };

                string json = JsonConvert.SerializeObject(array);

                return json;
            }
        }

        // Конструктор
        public Timer(int mode, int duration)
        {
            this.Mode = mode;
            this.Duration = duration;
            this.Result = false;
        }

        /// <summary>
        /// Add required data type for the filter to the list
        /// </summary>
        public void RequiredDataInit()
        {
            this.RequiredDataTypes.Add(DataType.Times);
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
            throw new Exception("GetOptions() DataType doesn't sent");
        }

        public void Compute()
        {
            double seconds = 0; // Прошло секунд с события
            
            // Начало итерации
            if (Mode == 0) { seconds = this.DataProvider.GetTimes().StartIteration; }
            // Первый BUY ордер
            if (Mode == 1) { seconds = this.DataProvider.GetTimes().FirstBuyOrder; }
            // Последний BUY ордер
            if (Mode == 2) { seconds = this.DataProvider.GetTimes().LastBuyOrder; }
            // Первый SELL ордер
            if (Mode == 3) { seconds = this.DataProvider.GetTimes().FirstSellOrder; }
            // Последний SELL ордер
            if (Mode == 4) { seconds = this.DataProvider.GetTimes().LastSellOrder; }

            Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {this.Duration} <= {seconds}");

            if (this.Duration <= seconds)
            {
                this.Result = true;
            }
            else
                this.Result = false;
        }
    }
}
