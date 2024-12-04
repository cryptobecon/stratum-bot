using Newtonsoft.Json;
using stratumbot.Interfaces;
using stratumbot.Models.Tools;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace stratumbot.Models
{
    /// <summary>
    /// DTO of Scalping config file
    /// </summary>
    public class ScalpingConfigText : IConfigText
    {
        public string Strategy { get; set; } = "Scalping";

        public string Budget { get; set; }
        public string Cur1 { get; set; }
        public string Cur2 { get; set; }

        public string MinSpread { get; set; }
        public string OptSpread { get; set; }
        public string MinMarkup { get; set; }
        public string OptMarkup { get; set; }
        public string ZeroSell { get; set; }
        public string InTimeout { get; set; }

        public bool IsDCA { get; set; }
        public string DCAStepCount { get; set; }
        public string DCAProfitPercent { get; set; }
        public ObservableCollection<string[]> DCASteps { get; set; }

        [JsonProperty("DCAFilters")]
        public Dictionary<int, DCAFilter> DCAFilters { get; set; } // Фильтры по шагам

        public string FirsOredersAmountPercentIgnor { get; set; } = "30%";
        public string FirsOredersCountIgnor { get; set; } = "4";

        public bool IsStopLoss { get; set; }
        public string StopLoss { get; set; }

        public List<Filters.JsonFilter> FiltersBuy { get; set; } // TODO INDICATORS тут добавил и надо везде
        [JsonProperty("TargetPointBuy")] // именно в этом месте надо указывать как сохранять строки json
        public int TargetPointBuy { get; set; }
        //public List<Filters.JsonFilter> FiltersSell { get; set; } // TODO INDICATORS тут добавил и надо везде
        //[JsonProperty("TargetPointSell")]
        //public int TargetPointSell { get; set; }
    }
}
