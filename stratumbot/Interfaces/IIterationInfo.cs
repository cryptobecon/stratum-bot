using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Interfaces
{
    /// <summary>
    /// Интерфейс для DTO классов для итераций, т.к. разные стратегии (Пока скальпинг только)
    /// </summary>
    interface IIterationInfo
    {
        Strategy Strategy { get; set; }
        string Sid { get; set; } // sid btnplus
        Exchange Exchange { get; set; }
    }
}
