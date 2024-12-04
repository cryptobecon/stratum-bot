using Newtonsoft.Json;
using stratumbot.Models;
using stratumbot.Models.Filters.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Interfaces
{
    public interface IFilter
    {
        //DataMiner DataMiner { get; set; } // TODO DELETE // Хранилище всех нужных данных для индикаторов
        //List<string> RequiredData { get; set; } // TODO DELETE // Спиок необходимых типов данных

        /// <summary>
        /// Identificator
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// Data provider
        /// </summary>
        DataProvider DataProvider { get; set; }

        /// <summary>
        /// List of required data types for the filter to send it to the DataProvider
        /// </summary>
        List<DataType> RequiredDataTypes { get; set; }

        /// <summary>
        /// Add required data type for the filter to the list
        /// </summary>
        void RequiredDataInit();

        /// <summary>
        /// Get options for specefic data type for the filter
        /// </summary>
        /// <param name="dataType">Data type of which options are needly</param>
        /// <param name="cur1">First currency</param>
        /// <param name="cur2">Second (base) currency</param>
        /// <returns>(DataOptions) options for specific data type</returns>
        DataOptions GetOptions(DataType dataType, string cur1 = null, string cur2 = null);


        bool Result { get; set; } // true or false 
        int Weight { get; set; } // Вес
        int CurrentWeight { get; set; } // Итоговый вес (0 либо Weight)
        string MyName { get; set; } // Отображаемое название Фильтра
        string Group { get; set; } // Группа
        System.Windows.Media.Brush Color { get; set; } // Цвет
        string FilterSide { get; set; } // Тип фильтра BUY / SELL
        string Json { get; } // JSOM настроек
        //string TimeFrame { get; set; }
    }
}
