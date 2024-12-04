using Newtonsoft.Json;
using stratumbot.Interfaces;
using stratumbot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Indicators
{
    class RSIi : IIndicator
    {
        public string Name { get; set; } = "RSI";

        public List<decimal?> Result(decimal?[] data, int period)
        {

            List<decimal?> ascendingDays = new List<decimal?>(); // восходящие дни
            List<decimal?> descendingDays = new List<decimal?>(); // нисходящие дни
            List<decimal?> result = new List<decimal?>();
            int count = 0;
        
            foreach (var price in data)
            {
                if (count == 0)
                {
                    ascendingDays.Add(0);
                    descendingDays.Add(0);
                }
                else
                {
                    if (data[count] > data[count - 1])
                    {
                        ascendingDays.Add(data[count] - data[count - 1]);
                        descendingDays.Add(0);
                    }
                    else if (data[count] < data[count - 1])
                    {
                        ascendingDays.Add(0);
                        descendingDays.Add(data[count - 1] - data[count]);
                    }
                    else
                    {
                        ascendingDays.Add(0);
                        descendingDays.Add(0);
                    }
                }
                count++;
            }

            var smma = new SMMA();
            var smmaAscendingDays = smma.Result(ascendingDays.ToArray(), period);
            var smmaDescendingDays = smma.Result(descendingDays.ToArray(), period);

            count = 0;
            foreach (var price in data)
            {
                if (smmaDescendingDays[count] == 0)
                    result.Add(100);
                else
                    result.Add(100 - (100 / (1 + smmaAscendingDays[count] / smmaDescendingDays[count])));
                count++;
            }

            return result;
        }
    }
}
