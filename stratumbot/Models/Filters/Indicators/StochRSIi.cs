using stratumbot.DTO;
using stratumbot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Indicators
{
    class StochRSIi : IIndicator
    {
        public string Name { get; set; } = "Stoch RSI";

        public Dictionary<string, decimal> Result(decimal?[] data, int kPeriod, int dPeriod, int smoothPeriod)
        {

            var rsi = new RSIi();
            var rsiData = rsi.Result(data, smoothPeriod); // TODO какой тут период!?

            // Очищаем от null данные для Stoch
            List<Quote> unnulledRSI = new List<Quote>();
            foreach (var rsiValue in rsiData)
            {
                if (rsiValue != null)
                    unnulledRSI.Add( new Quote { Close = (decimal)rsiValue, High = (decimal)rsiValue, Low = (decimal)rsiValue, Open = (decimal)rsiValue });
            }

            /*var stoch = new Stochi();
            var stochData = stoch.Result(unnulledRSI, kPeriod, dPeriod, smoothPeriod);*/

            List<decimal?> kLineFast = new List<decimal?>();
            List<decimal?> dLineSlow = new List<decimal?>();

            decimal lowMin = 999999;
            decimal highMax = 0;

            int count = 0;
            foreach (var price in unnulledRSI) // data.Last() - Текущая свеча
            {
                if ((count + 1) < smoothPeriod) // Внчале n штук ставим null т.к. не т данных для расётв
                    kLineFast.Add(null);
                else
                {
                    // min and max of n last periods
                    highMax = 0;
                    lowMin = 9999999;
                    for (int i = 0; i < smoothPeriod; i++)
                    {
                        // High
                        if (unnulledRSI[count - i].High > highMax)
                            highMax = unnulledRSI[count - i].High;
                        // Min
                        if (unnulledRSI[count - i].Low < lowMin)
                            lowMin = unnulledRSI[count - i].Low;
                    }
                    kLineFast.Add(((unnulledRSI[count].Close - lowMin) / (highMax - lowMin))  * 100);
                }
                count++;
            }

            var sma = new SMA();

            // Очищаем от null данные для SMA (K)
            List<decimal?> unnulledK = new List<decimal?>();
            foreach (var kk in kLineFast)
            {
                if (kk != null)
                    unnulledK.Add(kk);
            }
            kLineFast = sma.Result(unnulledK.ToArray(), kPeriod);

            // Очищаем от null данные для SMA (D)
            List<decimal?> unnulledD = new List<decimal?>();
            foreach (var dd in kLineFast)
            {
                if (dd != null)
                    unnulledD.Add(dd);
            }
            dLineSlow = sma.Result(unnulledD.ToArray(), dPeriod);

            var result = new Dictionary<string, decimal>();
            result["K"] = kLineFast.Last() ?? -666; // K линия
            result["D"] = dLineSlow.Last() ?? -666; // D линия

            return result;

            //return stochData;
        }
    }
}
