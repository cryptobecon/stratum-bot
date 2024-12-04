using stratumbot.DTO;
using stratumbot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Indicators
{
    class MFIi : IIndicator
    {
        public string Name { get; set; } = "MFI";

        public decimal Result(List<Quote> data, int period)
        {
#pragma warning disable CS0219 // The variable 'typicalPrice' is assigned but its value is never used
            decimal typicalPrice = 0; // 
#pragma warning restore CS0219 // The variable 'typicalPrice' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'rowMoneyFlow' is assigned but its value is never used
            decimal rowMoneyFlow = 0; //  сырой денежный поток = typicalPrice * Volume
#pragma warning restore CS0219 // The variable 'rowMoneyFlow' is assigned but its value is never used

            List<decimal?> tpPrices = new List<decimal?>(); // TP = (High + Low + Close) / 3
            List<decimal?> rowMoneyFlows = new List<decimal?>(); // Row Money Flow = TP * Volume

            // TP
            foreach (var candle in data)
            {
                tpPrices.Add((candle.High + candle.Low + candle.Close) / 3);
            }

            // Row Money Flow 
            int count = 0;
            foreach (var candle in data)
            {
                rowMoneyFlows.Add(tpPrices[count] * candle.Volume);
            }

            decimal PMF = 0; // Positive Money Flow
            decimal NMF = 0; // Negative Money Flow

            count = 0;
            foreach (var candle in data)
            {
                if (count == 0)
                {
                    PMF += 0;
                    NMF += 0;
                    count++;
                    continue;
                }

                if (tpPrices[count] >= tpPrices[count-1])
                    PMF += (decimal)rowMoneyFlows[count];
                else
                    NMF += (decimal)rowMoneyFlows[count];

                count++;
            }

            decimal MoneyFlowRatio = PMF / NMF;

            decimal MFI = 100 - 100 / (1 + MoneyFlowRatio);

            return MFI;
        }
    }
}
