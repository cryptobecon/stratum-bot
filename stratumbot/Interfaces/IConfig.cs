using stratumbot.Models.Filters;
using stratumbot.Models.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Interfaces
{
    /// <summary>
    /// Represent object config
    /// 
    /// In future it can be replace with Config.cs class object 
    /// which is inheritancing :
    /// IConfig(strategy field only), IConfigWithDCA(dca), IConfigTrade(curs,budget)
    /// 
    /// </summary>
    public interface IConfig
    {
        Strategy Strategy { get; set; }
        //Exchange Exchange { get; set; }
        decimal Budget { get; set; }
        string Cur1 { get; set; }
        string Cur2 { get; set; }

        bool IsDCA { get; set; } // Включёна ли функция усредненеия (DCA)
        //string DCAProfitPercentStr { get; set; } // Процент профита при DCA
        decimal DCAProfitPercent { get; set; }
        //string DCAStepCountStr { get; set; } // Количество DCA шагов
        int DCAStepCount { get; set; } // Количество DCA шагов
        //List<string[]> DCAStepsStr { get; set; } // Массив шагов DCA
        List<DCAStepConfig> DCASteps { get; set; } // Массив шагов DCA
        

        //Dictionary<int, DCAFilter> DCAFilters { get; set; } // Фильтры по шагам

        //List<JsonFilter> FiltersBuy { get; set; } // Список фильтров и индикаторов (ID, без самих настроек)
        //string TargetPointBuy { get; set; } // Проходное кол-во баллов по фильтрам
        //List<JsonFilter> FiltersSell { get; set; } // Список фильтров и индикаторов (ID, без самих настроек)
        //int TargetPointSell { get; set; } // Проходное кол-во баллов по фильтрам
    }
}
