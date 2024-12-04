using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Utility
{
    static class Time
    {
        // Текущее время в Unix TimeStamp (количество секунд)
        public static decimal CurrentSeconds()
        {
            return (decimal)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}
