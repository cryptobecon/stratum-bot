using Newtonsoft.Json;
using stratumbot.Interfaces;
using stratumbot.Models.Tools;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace stratumbot.Models
{
    /// <summary>
    /// DTO of Classic Long config file
    /// </summary>
    public class ClassicLongConfigText : IConfigText
    {
        public string Strategy { get; set; } = "Classic Long";
        public string Budget { get; set; }
        public string Cur1 { get; set; }
        public string Cur2 { get; set; }

        public string TargetProfitPercent { get; set; }
        public bool IsProfitTrailing { get; set; }
        public string TrailStepPercent { get; set; }
        public string ApproximationPercent { get; set; }
        public string UnApproximationPercent { get; set; }

        public bool IsMarketBuy { get; set; }

        public bool IsDCA { get; set; }
        public string DCAStepCount { get; set; }
        public string DCAProfitPercent { get; set; }
        public ObservableCollection<string[]> DCASteps { get; set; }

        [JsonProperty("DCAFilters")]
        public Dictionary<int, DCAFilter> DCAFilters { get; set; } // Фильтры по шагам

        public List<Filters.JsonFilter> FiltersBuy { get; set; } // TODO INDICATORS тут добавил и надо везде
        [JsonProperty("TargetPointBuy")] // именно в этом месте надо указывать как сохранять строки json
        public int TargetPointBuy { get; set; }

        public List<Filters.JsonFilter> FiltersSell { get; set; }
        [JsonProperty("TargetPointSell")]
        public int TargetPointSell { get; set; }

        public bool IsStopLoss { get; set; }
        public string StopLoss { get; set; }
        public string StopLossApproximation { get; set; }
        public List<Filters.JsonFilter> FiltersStopLoss { get; set; }
        [JsonProperty("TargetPointStopLoss")]
        public int TargetPointStopLoss { get; set; }
    }
}
