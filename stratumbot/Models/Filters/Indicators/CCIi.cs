using stratumbot.DTO;
using stratumbot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Indicators
{
    class CCIi : IIndicator
    {
        public string Name { get; set; } = "CCI"; // Commodity Channel Index

        public decimal? Result(List<Quote> data, int period)
        {
            List<decimal?> tpPrices = new List<decimal?>(); // Массив типичных цен // TP = (High + Low + Close) / 3
            
            // Находим TP в массив
            foreach (var candle in data)
            {
                tpPrices.Add( ( (candle.High + candle.Low + candle.Close) / (decimal)3));
            }

            // Строим SMA по TP
            var sma = new SMA();
            var tpSmaNull = sma.Result(tpPrices.ToArray(), period); // SMA по типичным ценам (tpPrices)

            // Находим Mean deviation
            decimal? std = (decimal)0.0;
            for (int i = 0; i < period; i++)
            {
                std += Math.Abs( (decimal)(tpPrices[tpPrices.Count - i - 1] - tpSmaNull.Last()) );
            }
            std = std / period;

            // Находим CCI
            decimal? smaMinusTypicalPrice = tpPrices.Last() - tpSmaNull.Last();
            std = (decimal)0.015 * std;
            decimal? cciResult = smaMinusTypicalPrice / std;

            return cciResult;
        }
    }
}
