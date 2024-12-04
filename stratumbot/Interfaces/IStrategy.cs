using stratumbot.DTO;
using stratumbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace stratumbot.Interfaces
{
    /// <summary>
    /// Strategy algorythm interface
    /// </summary>
    public interface IStrategy
    {
        void Trade(); // Главный метод торговли (Task здесь)
        void StopTrade(); // Метод останавливающий поток (работу внутри стратегии)
        CancellationTokenSource ts { get; set; }
        CancellationToken cancellationToken { get; set; }

        int BuyCounter { get; set; }
        int SellCounter { get; set; }

        IExchange Exchange { get; set; }

        Profit Profit { get; set; } // Объект соддержит в себе информацию по профиту

        //ThreadBackup Backup { get; set; } // Объект бекапа.
        //bool Recovering { get; set; } // Для восстановления потока. True если сейчас идёт восстановление. False обычный запуск потока.
    }
}
