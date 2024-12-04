using Newtonsoft.Json;
using stratumbot.Interfaces;
using stratumbot.Models.Filters;
using stratumbot.Models.Tools;
using System.Collections.Generic;

namespace stratumbot.Models.Strategies
{
    // TODO DELETE
    class ScalpingConfigJson_TO_DELETE : IConfig
    {

        [JsonProperty("Budget")]
        public decimal Budget { get; set; }



        [JsonProperty("Strategy")]
        public Strategy Strategy { get; set; }

        [JsonProperty("Exchange")]
        public Exchange Exchange { get; set; }

        //[JsonProperty("Budget")]
        //public string Budget { get; set; } // Бюджет указаный пользователем (возможно со знаком %)
        [JsonProperty("Cur1")]
        public string Cur1 { get; set; }
        [JsonProperty("Cur2")]
        public string Cur2 { get; set; }

        [JsonProperty("MinSpreadStr")]
        public string MinSpreadStr { get; set; }
        [JsonProperty("OptSpreadStr")]
        public string OptSpreadStr { get; set; }
        [JsonProperty("MinMarkupStr")]
        public string MinMarkupStr { get; set; }
        [JsonProperty("OptMarkupStr")]
        public string OptMarkupStr { get; set; }
        [JsonProperty("ZeroSellStr")]
        public string ZeroSellStr { get; set; }
        [JsonProperty("InTimeoutStr")]
        public string InTimeoutStr { get; set; }

        [JsonProperty("FirsOredersAmountPercentIgnorStr")]
        public string FirsOredersAmountPercentIgnorStr { get; set; }
        [JsonProperty("FirsOredersCountIgnorStr")]
        public string FirsOredersCountIgnorStr { get; set; }

        [JsonProperty("StopLossStr")]
        public string StopLossStr { get; set; }

        [JsonProperty("IsDCA")]
        public bool IsDCA { get; set; } // Включёна ли функция усредненеия (DCA)
        [JsonProperty("DCAProfitPercentStr")]
        public string DCAProfitPercentStr { get; set; }
        [JsonProperty("DCAStepCountStr")]
        public string DCAStepCountStr { get; set; }
        [JsonProperty("DCAStepsStr")]
        public List<string[]> DCAStepsStr { get; set; }

        [JsonProperty("DCAFilters")]
        public Dictionary<int, DCAFilter> DCAFilters { get; set; } // Фильтры по шагам

        [JsonProperty("IsSellStart")] // TODO DELETE is sell start
        public bool IsSellStart { get; set; } // Начать ли с продажи
        [JsonProperty("BuyPriceForSellStartStr")]
        public string BuyPriceForSellStartStr { get; set; }
        [JsonProperty("AmountForSellStartStr")]
        public string AmountForSellStartStr { get; set; }

        [JsonProperty("FiltersBuy")]
        public List<JsonFilter> FiltersBuy { get; set; } // Список фильтров и индикаторов (ID, без самих настроек)
        [JsonProperty("TargetPointBuy")]
        public string TargetPointBuy { get; set; }

        [JsonProperty("FiltersSell")]
        public List<JsonFilter> FiltersSell { get; set; } // Список фильтров и индикаторов (ID, без самих настроек)
        [JsonProperty("TargetPointSell")]
        public int TargetPointSell { get; set; }
        public decimal DCAProfitPercent { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int DCAStepCount { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public List<decimal[]> DCASteps { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        List<DCAStepConfig> IConfig.DCASteps { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
