using stratumbot.DTO;
using stratumbot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Indicators
{
    class NGAi : IIndicator
    {
        public string Name { get; set; } = "NGA";

        public decimal Result(IEnumerable<Quote> data, int period)
        {
            decimal max = 0; // Max цена в period периоде
            decimal min = 100000; // Min в period периоде
            decimal avg = 0; // Среднее

            foreach(var ohlc in data)
            {
                if (ohlc.Low < min)
                    min = ohlc.Low;
                if (ohlc.High > max)
                    max = ohlc.High;
            }

            avg = (max + min) / 2;

            return avg;
        }
    }
}
