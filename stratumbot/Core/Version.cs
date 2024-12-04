using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Core
{
    class Version
    {
        public static string Current = "0.3.23"; // Версия

        // Какую сборку запрашивать на обновление
        public static int ProductCode { get; set; } = 333; // LITE

        //public static int ProductCode { get; set; } = 444; // FREE
        //public static int ProductCode { get; set; } = 555; // LITE
        //public static int ProductCode { get; set; } = 666; // PRO, INFINITI
    }
}
