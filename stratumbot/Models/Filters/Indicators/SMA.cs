using stratumbot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Indicators
{
    class SMA : IIndicator
    {
        public string Name { get; set; } = "SMA";

        public List<decimal?> Result(decimal?[] data, int period)
        {
            decimal? lastPeriodSum = 0; // Сумма последних period штук данных
            List<decimal?> result = new List<decimal?>();

            int i = 0;
            foreach (var price in data)
            {

                lastPeriodSum += price;

                if (i + 1 < period)
                {
                    result.Add(null);
                }  
                else
                {
                    result.Add(lastPeriodSum / period);
                    lastPeriodSum -= data[i + 1 - period];
                }
                i++;
            }

            return result;
        }
    }
}
