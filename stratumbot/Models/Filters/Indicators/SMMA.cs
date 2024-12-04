﻿using stratumbot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Indicators
{
    class SMMA : IIndicator
    {
        public string Name { get; set; } = "SMMA";

        public List<decimal?> Result(decimal?[] data, int period)
        {
            var sma = new SMA();
            var smaData = sma.Result(data, period);
            decimal multiplier = (decimal)1 / (decimal)period;
            List<decimal?> result = new List<decimal?>();

            int i = 0;
            foreach (var price in smaData)
            {
                if (price == null)
                {
                    result.Add(null);
                }
                else
                {
                    var prev = result.ElementAtOrDefault(i - 1);
                    if (prev == null)
                    {
                        result.Add(price);
                        i++;
                        continue;
                    }
                    var ema = (data[i] - prev) * multiplier + prev;
                    result.Add(ema);
                }
                i++;
            }

            return result;
        }
    }
}
