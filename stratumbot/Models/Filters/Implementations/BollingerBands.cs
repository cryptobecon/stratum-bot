﻿using Newtonsoft.Json;
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
    class BollingerBands : IFilter
    {

        public string ID { get; set; } = "10";
        public string Name { get; set; } = "Bollinger Bands";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
        //public List<string> RequiredData { get; set; } = new List<string> { "CurrentPrice", "Klines" };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        public int Mode = 0; // price>UpperBB price>MiddleBB price>lowerBB price<UpperBB price<MiddleBB price<lowerBB // Higher or Lower
        public int Period { get; set; } = 0; // Период
        public string TimeFrame { get; set; } = "5m"; // Тайм-фрэйм
        public decimal Rate { get; set; } // Коэфицент — количество стандартных отклонений
        public decimal Indent { get; set; } = 0; // Отступ
        public int Duration { get; set; } = 0; // Продолжительность действия

        public decimal AllowedTime = 0; // Разрешенное время = Время последнего обновления + Duration

        public string DepthSide = "Bid"; // Стакан, из которого будет браться Current Price

        public string MyName { get; set; } = "Bollinger Bands"; // Отображаемое название 
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
                    Period = this.Period,
                    Rate = this.Rate,
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
        public BollingerBands(int higherOrlower, int period, decimal rate, string timeFrame, decimal indent, int duration, string depthSide)
        {
            this.RequiredDataInit();

            this.Mode = higherOrlower;
            this.Period = period;
            this.Rate = rate;
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

            // Filter Side
            decimal currentPrice = (this.DepthSide == "Bid") ? this.DataProvider.CurrentBuyPrice : this.DataProvider.CurrentSellPrice;

            //var bb = new BollingerBandsi();
            //var data = bb.Result(this.DataMiner.Klines[this.TimeFrame], this.Period, this.Rate);

            IEnumerable<BollingerBandsResult> results = 
                this.DataProvider.Quotes.GetBollingerBands(this.Period, this.Rate);

            

            // Высчитываем отступ
            decimal currentPoint = 0;


            if (Mode == 0) // "price>UpperBB"
            {

                // Высчитываем отступ
                currentPoint = (decimal)results.Last().UpperBand + Calc.AmountOfPercent(this.Indent, (decimal)results.Last().UpperBand);

                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {currentPrice} > {currentPoint}");
                if (currentPrice > currentPoint)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 1) // "price>MiddleBB"
            {
                // Высчитываем отступ
                currentPoint = (decimal)results.Last().Sma + Calc.AmountOfPercent(this.Indent, (decimal)results.Last().Sma);

                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {currentPrice} > {currentPoint}");
                if (currentPrice > currentPoint)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 2) // "price>lowerBB"
            {
                // Высчитываем отступ
                currentPoint = (decimal)results.Last().LowerBand + Calc.AmountOfPercent(this.Indent, (decimal)results.Last().LowerBand);

                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {currentPrice} > {currentPoint}");
                if (currentPrice > currentPoint)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 3) // "price<UpperBB"
            {
                // Высчитываем отступ
                currentPoint = (decimal)results.Last().UpperBand + Calc.AmountOfPercent(this.Indent, (decimal)results.Last().UpperBand);

                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {currentPrice} < {currentPoint}");
                if (currentPrice < currentPoint)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 4) // "price<MiddleBB"
            {
                // Высчитываем отступ
                currentPoint = (decimal)results.Last().Sma + Calc.AmountOfPercent(this.Indent, (decimal)results.Last().Sma);

                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {currentPrice} < {currentPoint}");
                if (currentPrice < currentPoint)
                {
                    this.Result = true;
                    this.AllowedTime = Time.CurrentSeconds() + this.Duration;
                }
                else
                    this.Result = false;
            }
            if (Mode == 5) // "price<lowerBB"
            {
                // Высчитываем отступ
                currentPoint = (decimal)results.Last().LowerBand  + Calc.AmountOfPercent(this.Indent, (decimal)results.Last().LowerBand);

                Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {currentPrice} < {currentPoint}");
                if (currentPrice < currentPoint)
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
                "indent: " + Indent.ToString() + "\n" +
                "currentPrice: " + currentPrice.ToString());

        }
    }
}